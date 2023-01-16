using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CoreData.CacheManager;
using CoreData.Common;
using CoreType.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json;

namespace CoreSvc.Common
{
    public static class Helper
    {
        public static ServiceDefinitionsRef CacheServiceDefinitions(bool externallyCached = false, params string[] includedAssemblies)
        {
            const string dictionaryKey = "ServiceDefinitions";
            var lockKey = $"{dictionaryKey}_lock";
            var token = Guid.NewGuid();

            try
            {
                var serviceInfos = GetServiceDefinitions(includedAssemblies);

                if (DistributedCache.LockTake(lockKey, token.ToString()))
                {
                    var cachedServiceInfos = DistributedCache.Get<ServiceDefinitionsRef>(dictionaryKey, true);

                    if (cachedServiceInfos?.Namespaces != null && cachedServiceInfos.Namespaces.Any())
                    {
                        serviceInfos.Namespaces = serviceInfos.Namespaces
                            .Union(cachedServiceInfos.Namespaces
                                .Where(x => !serviceInfos.Namespaces
                                    .Any(y => x.Namespace.Equals(y.Namespace))))
                            .ToList();
                    }

                    if (cachedServiceInfos?.Models != null && cachedServiceInfos.Models.Any())
                    {
                        serviceInfos.Models = serviceInfos.Models
                            .Union(cachedServiceInfos.Models
                                .Where(x => !serviceInfos.Models
                                    .ContainsKey(x.Key)))
                            .ToDictionary(s => s.Key, s => s.Value);
                    }

                    if (!externallyCached)
                        DistributedCache.Set(dictionaryKey, JsonConvert.SerializeObject(serviceInfos),
                            TimeSpan.MaxValue);

                    return serviceInfos;
                }
            }
            finally
            {
                DistributedCache.LockRelease(lockKey, token.ToString());
            }

            return null;
        }

        public static ServiceDefinitionsRef GetServiceDefinitions(string[] includedAssemblies)
        {
            var controllerType = typeof(Controller);
            var models = new Dictionary<string, PropertyTypeWithRef>();
            var assemblyName = Assembly.GetEntryAssembly()?.GetName().Name;

            var additionalTypes = new Dictionary<Type, BindingFlags?>
            {
                { typeof(UIHelperMethods), BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly }
            };

            var actionDescriptors = ServiceLocator.Current.GetService<IActionDescriptorCollectionProvider>()?.ActionDescriptors.Items.Cast<ControllerActionDescriptor>();

            var namespaces = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(c => controllerType.IsAssignableFrom(c)
                            && c.IsClass
                            && !c.IsInterface
                            && !c.IsAbstract
                            && c.Namespace != null
                            && c.FullName != null
                            && !c.Name.Equals("BaseController")
                            && !c.FullName.StartsWith("Microsoft", StringComparison.Ordinal))
                .Concat(additionalTypes.Keys)
                .Select(c =>
                {
                    var bindingFlags = additionalTypes.GetValueOrDefault(c) ?? BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;
                    var methods = c.GetMethods(bindingFlags);
                    var _namespace = includedAssemblies != null && includedAssemblies.Any(a => a.Equals(c.Assembly.FullName))
                        ? assemblyName + c.Namespace.Replace(c.Assembly.GetName().Name, "")
                        : c.Namespace;
                    var controllerActionDescriptor = actionDescriptors?
                        .Where(a => a.ControllerTypeInfo == c)
                        .ToList();

                    return new NamespaceTypeRef
                    {
                        Namespace = _namespace,
                        Controllers = new List<ControllerTypeRef>
                        {
                            new ControllerTypeRef
                            {
                                FullName = c.FullName,
                                AQN = ReflectionHelper.SimplifyAQN(c.AssemblyQualifiedName),
                                Name = c.Name,
                                Methods = methods
                                    .Where(m =>
                                    {
                                        if (additionalTypes.ContainsKey(c))
                                            return true;

                                        return m.IsPublic && !m.IsDefined(typeof(NonActionAttribute));
                                    })
                                    .Select(m =>
                                    {
                                        var relativeUrl = controllerActionDescriptor?
                                            .FirstOrDefault(x => x.MethodInfo == m)?
                                            .ActionName;

                                        #region Return Type

                                        PropertyTypeBase returnType = null;
                                        var returnParameterType = m.ReturnParameter?.ParameterType;
                                        if (returnParameterType != null)
                                        {
                                            // Gets underlying type of task to simulate mvc pipeline if its a api method. 
                                            if (relativeUrl != null)
                                                returnParameterType = ReflectionHelper.GetUnderlyingTaskType(returnParameterType);

                                            ReflectionHelper.GetProperties(returnParameterType, ref models, false, 5);

                                            returnType = new PropertyTypeBase
                                            {
                                                Name = returnParameterType.Name,
                                                Type = returnParameterType.Name,
                                                AQN = ReflectionHelper.SimplifyAQN(returnParameterType.AssemblyQualifiedName)
                                            };
                                        }

                                        #endregion

                                        #region Request Parameters

                                        var parameters = m.GetParameters()
                                            .Select(p =>
                                            {
                                                ReflectionHelper.GetProperties(p.ParameterType, ref models, false, 5);

                                                PropertyTypeBase parameter = new PropertyTypeBase
                                                {
                                                    Name = p.Name,
                                                    Type = p.ParameterType.Name,
                                                    AQN = ReflectionHelper.SimplifyAQN(p.ParameterType.AssemblyQualifiedName)
                                                };

                                                return parameter;
                                            })
                                            .ToList();

                                        #endregion

                                        return new MethodTypeRef
                                        {
                                            Name = m.Name,
                                            Url = relativeUrl != null ? $"{c.FullName}/{relativeUrl}" : null,
                                            ReturnType = returnType,
                                            Parameters = parameters
                                        };
                                    }).ToList()
                            }
                        }
                    };
                })
                .GroupBy(x => x.Namespace)
                .Select((x, y) => new NamespaceTypeRef
                {
                    Namespace = x.First().Namespace,
                    Controllers = x.SelectMany(zz => zz.Controllers.Where(z => z.Methods.Any())).ToList()
                })
                .Where(x => x.Controllers.Any())
                .OrderBy(x => x.Namespace)
                .ToList();

            return new ServiceDefinitionsRef
            {
                Namespaces = namespaces,
                Models = models
            };
        }
    }
}