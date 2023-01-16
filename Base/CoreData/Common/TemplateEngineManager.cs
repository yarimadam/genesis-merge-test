using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using CoreType.DBModels;
using CoreType.Models;
using CoreType.Types;
using Newtonsoft.Json.Linq;
using Serilog;
using SmartFormat;
using SmartFormat.Core.Settings;

namespace CoreData.Common
{
    public static class TemplateEngineManager
    {
        private static readonly SmartFormatter smartFormatter;
        private static readonly SmartFormatter smartFormatterRaw;

        static TemplateEngineManager()
        {
            smartFormatterRaw ??= GetFormatter(ErrorAction.ThrowError, ErrorAction.ThrowError);

#if DEBUG
            smartFormatter ??= GetFormatter(ErrorAction.MaintainTokens, ErrorAction.MaintainTokens);
#else
            smartFormatter ??= GetFormatter(ErrorAction.Ignore, ErrorAction.Ignore);
#endif
        }

        public static SmartFormatter GetFormatter(ErrorAction? formatErrorAction = null, ErrorAction? parseErrorAction = null)
        {
            SmartFormatter formatter;
            if (formatErrorAction != null || parseErrorAction != null)
            {
                formatter = Smart.CreateDefaultSmartFormat();

                formatErrorAction ??= smartFormatter.Settings.FormatErrorAction;
                parseErrorAction ??= smartFormatter.Settings.ParseErrorAction;

                formatter.Settings.FormatErrorAction = formatErrorAction.Value;
                formatter.Settings.ParseErrorAction = parseErrorAction.Value;
            }
            else
                formatter = smartFormatter;

            return formatter;
        }

        public static string Format(string template, Dictionary<string, object> parameters, ErrorAction? formatErrorAction = null, ErrorAction? parseErrorAction = null)
        {
            if (string.IsNullOrEmpty(template))
                return string.Empty;

            parameters = ParseMethodsAsParams(ref template, parameters);

            return GetFormatter(formatErrorAction, parseErrorAction).Format(template, parameters);
        }

        public static Dictionary<string, object> ParseMethodsAsParams(ref string template, Dictionary<string, object> parameters)
        {
            try
            {
                var matches = new Regex(@"{(\w+?\.(\w+?)\(([\s\S]*?)\))").Matches(template);

                if (matches.Any())
                {
                    var session = SessionAccessor.GetSession();
                    var instance = Activator.CreateInstance(typeof(UIHelperMethods), session);
                    var allMethods = typeof(UIHelperMethods).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                    var uniqMatches = matches
                        .Where(x => x.Groups.Count > 3)
                        .GroupBy(x => x.Groups[1].Value)
                        .Select(x => x.First())
                        .ToList();

                    foreach (Match match in uniqMatches)
                    {
                        try
                        {
                            var selector = match.Groups[1].Value;
                            var methodName = match.Groups[2].Value;
                            var paramsStr = match.Groups[3].Value
                                .Split(",")
                                .Where(x => !string.IsNullOrEmpty(x))
                                .Select(x => smartFormatterRaw.Format(x.Trim().Trim('"').Trim('\''), parameters))
                                .ToArray();

                            var foundMethod = allMethods.FirstOrDefault(x => x.Name == methodName && x.GetParameters().Length == paramsStr.Length);
                            if (foundMethod != null)
                            {
                                var methodParams = foundMethod.GetParameters();
                                var typedParams = paramsStr
                                    .Select((str, ind) => Convert.ChangeType(str, methodParams[ind].ParameterType))
                                    .ToArray();

                                var responseObject = foundMethod.Invoke(instance, typedParams);
                                var newSelector = Guid.NewGuid().ToString();

                                parameters.TryAdd(newSelector, responseObject);
                                template = template.Replace(selector, newSelector);
                            }
                            else
                                Log.Error("Method name or parameters are not matched ! Method name: {methodName}", methodName);
                        }
                        catch (Exception ex)
                        {
                            Log.Fatal(ex, "CommunicationMiddleware - ParseMethodsAsParams - loop");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "CommunicationMiddleware - ParseMethodsAsParams");
            }

            return parameters;
        }

        public static CommunicationDefinitions Format(CommunicationDefinitions communicationDefinitions, Dictionary<string, object> parameters)
        {
            var encodedJson = JObject.Parse(communicationDefinitions.SmsFormData).Encode();

            communicationDefinitions.SmsFormData = Format(encodedJson, parameters);

            return communicationDefinitions;
        }

        public static CommunicationTemplates Format(CommunicationTemplates communicationTemplates, Dictionary<string, object> parameters)
        {
            communicationTemplates.EmailSubject = Format(communicationTemplates.EmailSubject, parameters);
            communicationTemplates.EmailBody = Format(communicationTemplates.EmailBody, parameters);
            communicationTemplates.EmailRecipients = Format(communicationTemplates.EmailRecipients, parameters);
            communicationTemplates.EmailCcs = Format(communicationTemplates.EmailCcs, parameters);
            communicationTemplates.EmailBccs = Format(communicationTemplates.EmailBccs, parameters);
            communicationTemplates.EmailSenderName = Format(communicationTemplates.EmailSenderName, parameters);

            communicationTemplates.SmsBody = Format(communicationTemplates.SmsBody, parameters);
            communicationTemplates.SmsRecipients = Format(communicationTemplates.SmsRecipients, parameters);

            return communicationTemplates;
        }

        public static Communication GetFormattedCommunication(string commTemplateName, object requestObject, object responseObject)
        {
            var communicationTemplate = CommunicationManager.GetCommunicationTemplate(commTemplateName);
            var communicationDefinition = CommunicationManager.GetCommunicationDefinition(communicationTemplate.CommDefinitionId);

            return GetFormattedCommunication(communicationDefinition, communicationTemplate, requestObject, responseObject);
        }

        public static Communication GetFormattedCommunication(CommunicationDefinitions communicationDefinition, CommunicationTemplates communicationTemplate, object requestObject,
            object responseObject)
        {
            var session = SessionAccessor.GetSession() ?? new SessionContext();

            var parameters = new Dictionary<string, object>
            {
                { "Request", JObject.FromObject(requestObject) },
                { "Response", JObject.FromObject(responseObject) },
                { "Template", communicationTemplate },
                { "Definition", communicationDefinition },
                { "Session", session }
            };

            var communication = new Communication
            {
                CommunicationDefinitions = Format(communicationDefinition, parameters),
                CommunicationTemplates = Format(communicationTemplate, parameters)
            };

            return communication;
        }
    }
}