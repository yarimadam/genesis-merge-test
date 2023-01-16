using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace CoreData.Common
{
    public static class RuleEngineManager
    {
        public static bool ExecuteConditionGroups(JObject conditionGroups, JObject entity)
        {
            bool result = true;
            var groupId = string.Empty;

            try
            {
                if (conditionGroups != null && conditionGroups.HasValues)
                {
                    var properties = conditionGroups.Value<JToken>("properties");
                    var conjunction = properties?.Value<string>("conjunction") ?? "OR";
                    var hasNot = properties?.Value<bool>("not") ?? false;
                    var mainGroupId = conditionGroups.Value<string>("id");
                    groupId = mainGroupId ?? ((JProperty) conditionGroups.Parent)?.Name;

                    Log.Debug("CommunicationMiddleware - ExecuteConditionGroups (Conjunction: {conjunction}, Has Not: {hasNot}, GroupId: {groupId})", conjunction, hasNot, groupId);

                    foreach (var child in conditionGroups.Value<JObject>("children1"))
                    {
                        var childObj = child.Value;
                        var type = childObj.Value<string>("type");

                        if (type == "rule")
                            result = ExecuteConditionRules((JObject) childObj, entity);
                        else
                            result = ExecuteConditionGroups((JObject) childObj, entity);

                        if (hasNot)
                            result = !result;

                        if (conjunction == "AND" && result == false)
                            break;
                        if (conjunction == "OR" && result == true)
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Log.Fatal(ex, "CommunicationMiddleware - ExecuteConditionGroups");
            }

            Log.Debug("CommunicationMiddleware - ExecuteConditionGroups (Result: {result}, GroupId: {groupId})", result, groupId);

            return result;
        }

        public static bool ExecuteConditionRules(JObject conditionGroups, JObject entity)
        {
            bool result = true;
            string field = string.Empty;
            string ruleId = ((JProperty) conditionGroups.Parent)?.Name;

            try
            {
                var property = conditionGroups.Value<JObject>("properties");

                Log.Debug("CommunicationMiddleware - ExecuteConditionRules  (Body: {body}, RuleId: {ruleId})", property.ToString(Formatting.None), ruleId);

                if (property.HasValues)
                {
                    field = FixSelector(property.Value<string>("field"));

                    if (!string.IsNullOrEmpty(field))
                    {
                        var conditions = new List<(string Value, string ValueSrc, string ValueType, object TargetValue)>();
                        var operatorType = property.Value<string>("operator");
                        var valueProp = property.GetValue("value");
                        var valueCount = valueProp?.Count() ?? 0;
                        var valueSrcProp = property.GetValue("valueSrc");
                        var valueTypeProp = property.GetValue("valueType");
                        //var conjunction = property.Value<string>("conjunction");

                        /*field = field.Substring(field.IndexOf(".") + 1);
                       valueSrc = valueSrc?.Substring(valueSrc.IndexOf(".") + 1)*/

                        for (int i = 0; i < valueCount; i++)
                        {
                            var value = valueProp?[i]?.Value<string>();
                            var valueSrc = FixSelector(valueSrcProp?[i]?.Value<string>());
                            var valueType = valueTypeProp?[i]?.Value<string>();
                            object targetValue = null;

                            if (valueSrc == null || valueSrc == "value")
                                targetValue = value;
                            else if (value != null)
                            {
                                var target = entity.SelectToken(value);
                                if (target != null)
                                    targetValue = target is JObject ? "{}" : target.Value<JValue>().Value;
                            }

                            conditions.Add((value, valueSrc, valueType, targetValue));
                        }

                        var source = entity.SelectToken(field);
                        var sourceValue = source is JObject ? "{}" : source?.Value<JValue>().Value;

                        result = EvaluateCondition(sourceValue, operatorType, conditions);
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Log.Fatal(ex, "CommunicationMiddleware - ExecuteConditionRules");
            }

            Log.Debug("CommunicationMiddleware - ExecuteConditionRules  (Result: {result}, Field: {field}, RuleId: {ruleId})", result, field, ruleId);

            return result;
        }

        public static bool EvaluateCondition(object sourceValueObj, string operatorType, List<(string Value, string ValueSrc, string ValueType, object TargetValue)> conditions)
        {
            bool result;
            dynamic sourceValue = null;
            List<dynamic> targetValues = null;

            try
            {
                // TODO Convert all values according to their types.
                var valueType = conditions.Any() ? conditions.First().ValueType : "object";
                var type = valueType switch
                {
                    "text" => typeof(string),
                    "number" => typeof(long),
                    "date" => typeof(DateTime),
                    "datetime" => typeof(DateTime),
                    "time" => typeof(DateTimeOffset),
                    "boolean" => typeof(bool),
                    _ => typeof(object)
                };

                sourceValue = sourceValueObj != null
                    ? type == typeof(object) ? sourceValueObj : Convert.ChangeType(sourceValueObj, type)
                    : null;

                targetValues = conditions
                    .Select(x => x.TargetValue)
                    .Select(x => type == typeof(object) ? x : Convert.ChangeType(x, type))
                    .ToList();

                result = operatorType switch
                {
                    "equal" => targetValues[0] == sourceValue,
                    "not_equal" => targetValues[0] != sourceValue,
                    "less" => targetValues[0] < sourceValue,
                    "less_or_equal" => targetValues[0] <= sourceValue,
                    "greater" => targetValues[0] > sourceValue,
                    "greater_or_equal" => targetValues[0] >= sourceValue,
                    "between" => sourceValue > targetValues[0] && sourceValue < targetValues[1],
                    "not_between" => !(sourceValue > targetValues[0] && sourceValue < targetValues[1]),
                    "is_empty" => sourceValue == null || sourceValue.Equals(""),
                    "is_not_empty" => !(sourceValue == null || sourceValue.Equals("")),
                    "like" => sourceValue != null && sourceValue.Contains(targetValues[0], StringComparison.OrdinalIgnoreCase),
                    "not_like" => sourceValue != null && !sourceValue.Contains(targetValues[0], StringComparison.OrdinalIgnoreCase),
                    _ => false
                };
            }
            catch (Exception ex)
            {
                result = false;
                Log.Fatal(ex, "CommunicationMiddleware - EvaluateCondition");
            }

            Log.Debug("CommunicationMiddleware - EvaluateCondition      (Source Value: {sourceValue}, Operator: {operatorType}, Target Values: {targetValues}, Result: {result})",
                sourceValue, operatorType, targetValues, result);

            return result;
        }

        private static string FixSelector(string selector)
        {
            if (!string.IsNullOrEmpty(selector))
                selector = Regex.Replace(selector, @"\.(\[(\^?)(\d+)\])", "$1");

            return selector;
        }
    }
}