using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreData;
using CoreData.CacheManager;
using CoreData.Common;
using CoreSvc.Common;
using CoreSvc.Filters;
using CoreType.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreSvc.Controllers
{
    [Authorize]
    [DefaultRoute]
    [Resources(Constants.ResourceCodes.Definitions)]
    public class ServiceDefinitionController : BaseController
    {
        [HttpPost]
        [ClaimRequirement(ActionType.List)]
        public async Task<ResponseWrapper<ServiceDefinitionsRef>> List()
        {
            var serviceDefinitions = await DistributedCache.GetAsync<ServiceDefinitionsRef>("ServiceDefinitions", true);

            var namespaceTypeRefs = serviceDefinitions.Namespaces
                .Select(n =>
                {
                    n.Controllers = n.Controllers
                        .Select(c =>
                        {
                            c.Methods = c.Methods
                                .Where(m => m.Url != null)
                                .ToList();

                            return c;
                        })
                        .Where(c => c.Methods.Any())
                        .ToList();

                    return n;
                })
                .Where(x => x.Controllers.Any())
                .ToList();

            var genericResponse = new ResponseWrapper<ServiceDefinitionsRef>
            {
                Data = new ServiceDefinitionsRef
                {
                    Namespaces = namespaceTypeRefs
                },
                Message = LocalizedMessages.PROCESS_SUCCESSFUL,
                Success = true
            };

            return genericResponse;
        }

        [HttpPost]
        [ClaimRequirement(ActionType.List)]
        public async Task<ResponseWrapper> GetTypeParameters([FromBody] List<string> typeAQNs)
        {
            var genericResponse = new ResponseWrapper();
            var resModels = new List<PropertyType>();
            var resClasses = new List<ControllerType>();
            var resMethods = new List<MethodType>();

            if (typeAQNs != null && typeAQNs.Any())
            {
                typeAQNs = typeAQNs
                    .Select(ReflectionHelper.SimplifyAQN)
                    .ToList();

                var serviceDefinitions =
                    await DistributedCache.GetAsync<ServiceDefinitionsRef>("ServiceDefinitions", true);

                const int maxDepth = 5;

                var classes = serviceDefinitions.Namespaces
                    .SelectMany(x => x.Controllers)
                    .Where(x => typeAQNs.Any(typeAQN => typeAQN.Equals(x.AQN)))
                    .Select(x => x.ConvertToControllerType(serviceDefinitions.Models, false, maxDepth))
                    .ToList();

                var methods = serviceDefinitions.Namespaces
                    .SelectMany(x => x.Controllers)
                    .SelectMany(x => x.Methods)
                    .Where(x => typeAQNs.Any(typeAQN =>
                        typeAQN.Equals(x.Url, StringComparison.InvariantCultureIgnoreCase)))
                    .Select(x => x.ConvertToMethodType(serviceDefinitions.Models, maxDepth))
                    .ToList();

                var typeParameters = serviceDefinitions
                    .GetModels(typeAQNs, maxDepth);

                foreach (var typeName in typeAQNs)
                {
                    var tempClass = classes.FirstOrDefault(x => typeName.Equals(x.AQN));

                    if (tempClass != null)
                        resClasses.Add(tempClass);
                    else
                    {
                        var tempMethod = methods.FirstOrDefault(y =>
                            typeName.Equals(y.Url, StringComparison.InvariantCultureIgnoreCase));

                        if (tempMethod != null)
                            resMethods.Add(tempMethod);
                        else
                        {
                            var tempModel = typeParameters.FirstOrDefault(x => typeName.Equals(x.AQN));

                            // Fallback to reflection
                            if (tempModel == null)
                            {
                                var type = Type.GetType(typeName, false);
                                if (type != null)
                                {
                                    var models = new Dictionary<string, PropertyTypeWithRef>();
                                    tempModel = ReflectionHelper.GetProperties(type, ref models, true, maxDepth)
                                        .FirstOrDefault()?
                                        .ConvertToPropertyType(models);
                                }
                            }

                            if (tempModel != null)
                                resModels.Add(tempModel);
                        }
                    }
                }
            }

            if (resModels.Any() || resClasses.Any() || resMethods.Any())
            {
                genericResponse.Data = new
                {
                    Models = resModels,
                    Classes = resClasses,
                    Methods = resMethods
                };
                genericResponse.Message = LocalizedMessages.PROCESS_SUCCESSFUL;
                genericResponse.Success = true;
            }
            else
                genericResponse.Message = LocalizedMessages.PROCESS_FAILED;

            return genericResponse;
        }
    }
}