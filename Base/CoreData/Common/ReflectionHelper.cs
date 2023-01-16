using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CoreType.Types;

namespace CoreData.Common
{
    public static class ReflectionHelper
    {
        private static readonly Type Type = typeof(Type);
        private static readonly Type TaskType = typeof(Task);
        private static readonly Regex AqnSimplifyRegex = new Regex(@", Version=.*?(?:\]|$)", RegexOptions.Compiled);

        private static readonly Type[] KnownTypes =
        {
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(TimeSpan),
            typeof(string),
            typeof(decimal),
            typeof(object)
        };

        public static long GetPrimaryId<TEntity>(TEntity model)
        {
            if (model == null) return 0;
            var modelType = typeof(TEntity);
            var a = modelType.GetProperties()
                .FirstOrDefault(x => Attribute.IsDefined(x, typeof(KeyAttribute), true));

            return a != null ? Convert.ToInt64(a.GetValue(model)) : int.MaxValue;
        }

        public static void SetPrimaryId<TEntity, TValue>(TEntity model, TValue value)
        {
            var modelType = typeof(TEntity);
            var a = modelType.GetProperties()
                .FirstOrDefault(x => x.GetCustomAttributes(typeof(KeyAttribute), true).Any());

            a?.SetValue(model, Convert.ToInt32(value));
        }

        public static List<PropertyTypeWithRef> GetProperties(Type objType, bool includeSelf = false, short maxDepth = 5, short depth = -1)
        {
            var models = new Dictionary<string, PropertyTypeWithRef>();
            return GetProperties(objType, ref models, includeSelf, maxDepth, depth);
        }

        public static List<PropertyTypeWithRef> GetProperties(Type objType, ref Dictionary<string, PropertyTypeWithRef> models, bool includeSelf = false, short maxDepth = 5, short depth = -1)
        {
            // TODO Handle methods return as Task<T>

            if (objType.IsIgnoredType() || !includeSelf && objType.IsKnownType())
                return null;

            var res = new List<PropertyTypeWithRef>();
            models ??= new Dictionary<string, PropertyTypeWithRef>();
            depth++;

            if (TryGetModel(ref models, objType, maxDepth - depth, out var existingModel))
            {
                res.Add(existingModel);
            }
            else
            {
                if (!objType.IsIgnoredType() && !objType.IsKnownType())
                {
                    var properties = objType
                        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(x => !IsIgnoredType(x));

                    foreach (var property in properties)
                    {
                        var propertyType = property.PropertyType;
                        if (TryGetModel(ref models, property, maxDepth - depth, out var existingPropertyModel))
                        {
                            res.Add(existingPropertyModel);
                        }
                        else if (propertyType.IsKnownType())
                        {
                            var propertyTypeTemp = new PropertyTypeWithRef
                            {
                                Name = property.Name,
                                Type = propertyType.Name,
                                AQN = propertyType.AssemblyQualifiedName,
                                RemainingDepth = maxDepth
                            };

                            AddToModel(ref models, propertyTypeTemp);

                            res.Add(propertyTypeTemp);
                        }
                        else if (typeof(IEnumerable).IsAssignableFrom(propertyType))
                        {
                            List<PropertyTypeWithRef> childProperties = null;
                            if (depth < maxDepth && propertyType.GenericTypeArguments.Length > 0)
                            {
                                childProperties = new List<PropertyTypeWithRef>();
                                foreach (var typeArgument in propertyType.GenericTypeArguments)
                                {
                                    var childPropertiesTemp = GetProperties(typeArgument, ref models, true, maxDepth, depth) ?? new List<PropertyTypeWithRef>();

                                    AddToModel(ref models, childPropertiesTemp);

                                    childProperties = childProperties
                                        .Union(childPropertiesTemp)
                                        .ToList();
                                }
                            }

                            var propertyTypeTemp = new PropertyTypeWithRef
                            {
                                Name = property.Name,
                                Type = propertyType.Name,
                                AQN = propertyType.AssemblyQualifiedName,
                                Properties = childProperties?.Select(ConvertPropertyTypeToRef).ToList(),
                                RemainingDepth = maxDepth - depth
                            };

                            AddToModel(ref models, propertyTypeTemp);

                            res.Add(propertyTypeTemp);
                        }
                        else if (Nullable.GetUnderlyingType(propertyType) is var underlyingType && underlyingType != null)
                        {
                            var childProperties = depth < maxDepth
                                ? GetProperties(underlyingType, ref models, false, maxDepth, depth)?
                                    .ToList()
                                : null;

                            AddToModel(ref models, childProperties);

                            var propertyTypeTemp = new PropertyTypeWithRef
                            {
                                Name = property.Name,
                                Type = underlyingType.Name,
                                AQN = underlyingType.AssemblyQualifiedName,
                                Properties = childProperties?.Select(ConvertPropertyTypeToRef).ToList(),
                                RemainingDepth = maxDepth - depth
                            };

                            AddToModel(ref models, propertyTypeTemp);

                            res.Add(propertyTypeTemp);
                        }
                        else if (propertyType.IsEnum)
                        {
                            var enumType = Enum.GetUnderlyingType(propertyType);
                            var enumPropertType = GetProperties(enumType, ref models, true, maxDepth, depth)?
                                .FirstOrDefault();

                            if (enumPropertType == null)
                                return null;

                            AddToModel(ref models, enumPropertType);

                            var enumNames = Enum.GetNames(propertyType);
                            var propertyTypeTemp = new PropertyTypeWithRef
                            {
                                Name = propertyType.Name,
                                Type = propertyType.Name,
                                AQN = propertyType.AssemblyQualifiedName,
                                Properties = enumNames.Select(enumName =>
                                {
                                    enumPropertType.Name = enumName;
                                    return enumPropertType;
                                }).Select(ConvertPropertyTypeToRef).ToList(),
                                RemainingDepth = maxDepth - depth
                            };

                            AddToModel(ref models, propertyTypeTemp);

                            res.Add(propertyTypeTemp);
                        }
                        else
                        {
                            var childProperties = depth < maxDepth
                                ? GetProperties(propertyType, ref models, false, maxDepth, depth)?
                                    .ToList()
                                : null;

                            AddToModel(ref models, childProperties);

                            var propertyTypeTemp = new PropertyTypeWithRef
                            {
                                Name = property.Name,
                                Type = propertyType.Name,
                                AQN = propertyType.AssemblyQualifiedName,
                                Properties = childProperties?.Select(ConvertPropertyTypeToRef).ToList(),
                                RemainingDepth = maxDepth - depth
                            };

                            AddToModel(ref models, propertyTypeTemp);

                            res.Add(propertyTypeTemp);
                        }
                    }
                }

                var propertyRes = new PropertyTypeWithRef
                {
                    Name = objType.Name,
                    Type = objType.Name,
                    AQN = objType.AssemblyQualifiedName,
                    Properties = res.Any()
                        ? res
                            .Select(ConvertPropertyTypeToRef)
                            .ToList()
                        : null,
                    RemainingDepth = maxDepth - depth
                };

                AddToModel(ref models, propertyRes);

                if (includeSelf)
                    return new List<PropertyTypeWithRef> { propertyRes };
            }

            return res;
        }

        private static bool IsKnownType(this Type propertyType)
        {
            return propertyType.IsPrimitive || KnownTypes.Contains(propertyType);
        }

        private static bool IsIgnoredType(this Type type, string propertyName = null)
        {
            return type == Type
                   || (type.AssemblyQualifiedName != null && type.AssemblyQualifiedName.StartsWith("System.Reflection"));
        }

        private static bool IsIgnoredType(this PropertyInfo propertyInfo)
        {
            var propertyType = propertyInfo.PropertyType;
            var propertyName = propertyInfo.Name;

            if (propertyInfo.DeclaringType == TaskType)
                return propertyName == nameof(Task.CreationOptions) || propertyName == nameof(Task.AsyncState);

            return propertyType.IsIgnoredType(propertyName);
        }

        public static string SimplifyAQN(string aqn)
        {
            return aqn != null ? AqnSimplifyRegex.Replace(aqn, string.Empty) : string.Empty;
        }

        public static bool IsNumericType(this Type o)
        {
            switch (Type.GetTypeCode(o))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        private static bool AddToModel(ref Dictionary<string, PropertyTypeWithRef> models, List<PropertyTypeWithRef> propertyTypeList)
        {
            if (propertyTypeList == null)
                return false;

            foreach (var propertyTypeRef in propertyTypeList)
                AddToModel(ref models, propertyTypeRef);

            return true;
        }

        private static bool AddToModel(ref Dictionary<string, PropertyTypeWithRef> models, PropertyTypeWithRef propertyTypeWithRef)
        {
            if (propertyTypeWithRef.AQN != null)
            {
                propertyTypeWithRef.AQN = SimplifyAQN(propertyTypeWithRef.AQN);
                if (models.TryGetValue(propertyTypeWithRef.AQN, out var existingModel))
                {
                    if (propertyTypeWithRef.Properties != null
                        && (existingModel.Properties == null || propertyTypeWithRef.Properties.Count > existingModel.Properties.Count))
                    {
                        models.Remove(propertyTypeWithRef.AQN);
                        models.Add(propertyTypeWithRef.AQN, propertyTypeWithRef);
                        return true;
                    }
                }
                else
                {
                    models.Add(propertyTypeWithRef.AQN, propertyTypeWithRef);
                    return true;
                }

                // if (propertyTypeWithRef.Properties != null)
                //     foreach (var prop in propertyTypeWithRef.Properties)
                //     {
                //         AddToModel(ref models,(PropertyTypeWithRef)prop )//prop.ConvertToPropertyType(models));
                //     }
            }

            return false;
        }

        private static bool TryGetModel(ref Dictionary<string, PropertyTypeWithRef> models, MemberInfo memberInfo, int remainingDepth, out PropertyTypeWithRef value)
        {
            value = null;

            var aqn = SimplifyAQN(memberInfo is PropertyInfo pi ? pi.PropertyType.AssemblyQualifiedName : (memberInfo as Type)?.AssemblyQualifiedName);
            if (aqn != null && models.TryGetValue(aqn, out var internalValue))
            {
                var shouldReturn = ((internalValue.Properties == null || !internalValue.Properties.Any()) && internalValue.RemainingDepth > 0)
                                   || internalValue.RemainingDepth >= remainingDepth;

                if (shouldReturn)
                    value = new PropertyTypeWithRef
                    {
                        Name = memberInfo.Name,
                        Type = internalValue.Type,
                        AQN = internalValue.AQN,
                        Properties = internalValue.Properties,
                        RemainingDepth = internalValue.RemainingDepth
                    };
                else
                    value = null;

                return shouldReturn;
            }

            return false;
        }

        public static PropertyTypeBase ConvertPropertyTypeToRef(PropertyTypeWithRef propertyTypeWithRef)
        {
            return new PropertyTypeBase
            {
                Name = propertyTypeWithRef.Name,
                Type = propertyTypeWithRef.Type,
                AQN = propertyTypeWithRef.AQN
            };
        }

        public static PropertyTypeWithRef ConvertPropertyRefToType(PropertyTypeBase propertyType)
        {
            return new PropertyTypeWithRef
            {
                Name = propertyType.Name,
                Type = propertyType.Type,
                AQN = propertyType.AQN
            };
        }

        public static MethodType ConvertToMethodType(this MethodTypeRef controllerTypeRef, Dictionary<string, PropertyTypeWithRef> models, short maxDepth = 5, short depth = -1)
        {
            return new MethodType
            {
                Name = controllerTypeRef.Name,
                Url = controllerTypeRef.Url,
                ReturnType = controllerTypeRef.ReturnType.ConvertToPropertyType(models, maxDepth),
                Parameters = controllerTypeRef.Parameters
                    .Select(x => x.ConvertToPropertyType(models, maxDepth))
                    .ToList()
            };
        }

        public static ControllerType ConvertToControllerType(this ControllerTypeRef controllerTypeRef, Dictionary<string, PropertyTypeWithRef> models, bool ignoreNonServices = false,
            short maxDepth = 5, short depth = -1)
        {
            return new ControllerType
            {
                Name = controllerTypeRef.Name,
                FullName = controllerTypeRef.FullName,
                AQN = controllerTypeRef.AQN,
                Methods = controllerTypeRef.Methods
                    .Where(x => !ignoreNonServices || !string.IsNullOrEmpty(x.Url))
                    .Select(x => x.ConvertToMethodType(models, maxDepth, depth))
                    .ToList()
            };
        }

        public static NamespaceType ConvertToNamespaceType(this NamespaceTypeRef namespaceTypeRef, Dictionary<string, PropertyTypeWithRef> models, bool ignoreNonServices = false, short maxDepth = 5,
            short depth = -1)
        {
            return new NamespaceType
            {
                Namespace = namespaceTypeRef.Namespace,
                Controllers = namespaceTypeRef.Controllers
                    .Select(x => x.ConvertToControllerType(models, ignoreNonServices, maxDepth, depth))
                    .Where(x => x.Methods.Any())
                    .ToList()
            };
        }

        public static ServiceDefinitions ConvertToServiceDefinition(this ServiceDefinitionsRef serviceDefinitionsRef, bool ignoreNonServices = false, short maxDepth = 5)
        {
            return new ServiceDefinitions
            {
                Models = null,
                Namespaces = serviceDefinitionsRef
                    .Namespaces
                    .Select(x => x.ConvertToNamespaceType(serviceDefinitionsRef.Models, ignoreNonServices, maxDepth))
                    .Where(x => x.Controllers.Any())
                    .ToList()
            };
        }

        public static List<PropertyType> GetModels(this ServiceDefinitionsRef serviceDefinitionsRef, List<string> aqnFilter = null, short maxDepth = 5)
        {
            aqnFilter = aqnFilter?.Distinct().ToList();

            return serviceDefinitionsRef
                .Models
                .Select(x => x.Value)
                .Where(model => aqnFilter == null || aqnFilter.Any(typeName => typeName.Equals(model.AQN)))
                .Select(model => model.ConvertToPropertyType(serviceDefinitionsRef.Models, maxDepth, 1))
                .ToList();
        }

        public static PropertyType ConvertToPropertyType(this PropertyTypeBase propertyTypeWithRef, [NotNull] Dictionary<string, PropertyTypeWithRef> models, short maxDepth = 5, short depth = -1)
        {
            if (depth <= maxDepth && models.TryGetValue(propertyTypeWithRef.AQN ?? "", out var existingModel))
            {
                depth++;

                return new PropertyType
                {
                    Name = propertyTypeWithRef.Name,
                    AQN = propertyTypeWithRef.AQN,
                    Type = existingModel.Type,
                    Properties = existingModel.Properties?
                        .Select(x => x.ConvertToPropertyType(models, maxDepth, depth))
                        .Where(x => x != null)
                        .ToList()
                };
            }

            return null;
        }

        public static PropertyType ConvertToPropertyType(this PropertyTypeWithRef propertyTypeWithRef, [NotNull] Dictionary<string, PropertyTypeWithRef> models, short maxDepth = 5, short depth = -1)
        {
            if (depth <= maxDepth && models.TryGetValue(propertyTypeWithRef.AQN ?? "", out var existingModel))
            {
                depth++;

                return new PropertyType
                {
                    Name = propertyTypeWithRef.Name,
                    AQN = propertyTypeWithRef.AQN,
                    Type = existingModel.Type,
                    Properties = existingModel.Properties?
                        .Select(x => x.ConvertToPropertyType(models, maxDepth, depth))
                        .Where(x => x != null)
                        .ToList()
                };
            }

            return null;
        }

        public static Delegate CreateMethod(MethodInfo methodInfo, object target)
        {
            List<Type> args = new List<Type>(methodInfo.GetParameters().Select(p => p.ParameterType));
            Type delegateType;

            if (methodInfo.ReturnType == typeof(void))
            {
                delegateType = Expression.GetActionType(args.ToArray());
            }
            else
            {
                args.Add(methodInfo.ReturnType);
                delegateType = Expression.GetFuncType(args.ToArray());
            }

            return target == null
                ? Delegate.CreateDelegate(delegateType, methodInfo)
                : Delegate.CreateDelegate(delegateType, target, methodInfo);
        }

        public static Type GetUnderlyingTaskType(Type type)
        {
            if (type.BaseType == typeof(Task))
                return type.IsGenericType
                    ? type.GenericTypeArguments.First()
                    : typeof(void);

            return type;
        }
    }
}