using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreData.CacheManager;
using CoreData.Common;
using CoreData.Infrastructure.Producers;
using CoreType.DBModels;
using CoreType.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json.Linq;
using Serilog;

namespace CoreSvc.Middlewares
{
    public class CommunicationMiddleware
    {
        private readonly RequestDelegate _next;

        public CommunicationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var nextInvoked = false;

            try
            {
                var routeValues = context.GetRouteData()?.Values;

                if (routeValues == null
                    || routeValues.Count == 0
                    || !routeValues.ContainsKey("controller")
                    || !routeValues.ContainsKey("action"))
                    return;

                var controllerName = $"{routeValues["controller"]}Controller";
                var methodName = routeValues["action"].ToString();

                var serviceDefinitions = await DistributedCache.GetAsync<ServiceDefinitionsRef>("ServiceDefinitions", true);

                var serviceDefinition = serviceDefinitions?
                    .Namespaces
                    .Where(x => x.Namespace.StartsWith(AppDomain.CurrentDomain.FriendlyName, StringComparison.Ordinal))
                    .SelectMany(q => q.Controllers)
                    .Where(x => x.Name.Equals(controllerName, StringComparison.InvariantCultureIgnoreCase))
                    .SelectMany(q => q.Methods)
                    .Where(m => m.Url != null)
                    .FirstOrDefault(x => x.Name.Equals(methodName, StringComparison.InvariantCultureIgnoreCase));

                if (serviceDefinition != null)
                {
                    Log.Debug("CommunicationMiddleware - Service definitions acquired from cache.");

                    var communicationTemplates = await DistributedCache.GetAsync<List<CommunicationTemplates>>("CommunicationTemplates");
                    communicationTemplates = communicationTemplates?
                        .Where(x => !string.IsNullOrEmpty(x.ServiceUrls) && x.ServiceUrls.Split("|").Contains(serviceDefinition.Url))
                        .ToList();

                    if (communicationTemplates != null && communicationTemplates.Any())
                    {
                        Log.Information("CommunicationMiddleware - Communication template(s) matched with requested service url.");

                        #region Request

                        var requestBody = await context.Request.ReadBodyAsync();
                        var requestObject = !string.IsNullOrWhiteSpace(requestBody) ? JObject.Parse(requestBody).ToCamelCase() : new JObject();

                        if (context.Request.QueryString.HasValue)
                        {
                            Dictionary<string, object> queryDict = context.Request.Query
                                .ToDictionary(s => s.Key, s => s.Value.Count == 1 ? (object) s.Value.ToString() : s.Value.ToArray());
                            var additionalRequestObj = JObject.FromObject(queryDict);

                            requestObject.Merge(additionalRequestObj);
                        }

                        #endregion

                        #region Response

                        JObject responseObject;
                        using (context.Response.PrepareToRead(out var originalBody))
                        {
                            try
                            {
                                //invoke the rest of the pipeline
                                nextInvoked = true;
                                await _next.Invoke(context);
                            }
                            catch (Exception)
                            {
                                // Revert body to original stream before throw
                                context.Response.Body = originalBody;
                                throw;
                            }

                            string responseBody = await context.Response.ReadBodyAsync(originalBody);
                            responseObject = !string.IsNullOrWhiteSpace(responseBody) ? JObject.Parse(responseBody).ToCamelCase() : new JObject();
                        }

                        #endregion

                        var tempObj = new JObject(new JProperty("Request", requestObject));
                        tempObj.Merge(new JObject(new JProperty("Response", responseObject)));

                        foreach (var communicationTemplate in communicationTemplates)
                        {
                            try
                            {
                                Log.Information("CommunicationMiddleware - Communication template with id '{CommTemplateId}' matched and processing.",
                                    communicationTemplate.CommTemplateId);

                                var requestConditions = !string.IsNullOrWhiteSpace(communicationTemplate.RequestConditions) ? JObject.Parse(communicationTemplate.RequestConditions) : new JObject();

                                if (RuleEngineManager.ExecuteConditionGroups(requestConditions, tempObj))
                                {
                                    var communicationDefinitions = await DistributedCache.GetAsync<List<CommunicationDefinitions>>("CommunicationDefinitions");

                                    if (communicationDefinitions != null)
                                    {
                                        var communicationDefinition = communicationDefinitions.FirstOrDefault(x => x.CommDefinitionId == communicationTemplate.CommDefinitionId);

                                        if (communicationDefinition != null)
                                        {
                                            Log.Information(
                                                "CommunicationMiddleware - Communication definition(id:{CommDefinitionId}) found according to template(id:{CommTemplateId})"
                                                , communicationDefinition.CommDefinitionId, communicationTemplate.CommTemplateId);

                                            var communication = TemplateEngineManager.GetFormattedCommunication(communicationDefinition, communicationTemplate, requestObject, responseObject);

                                            if (communicationDefinition.CommDefinitionType == 1)
                                                CommMailProducer.Produce(communication);
                                            else
                                                CommSMSProducer.Produce(communication);

                                            Log.Information(
                                                "CommunicationMiddleware - Successfully produced communication task according to communication definition(id:{CommDefinitionId})"
                                                , communicationDefinition.CommDefinitionId);
                                        }
                                        else
                                            Log.Warning("CommunicationMiddleware - There is no communication definition matched with template ! (TemplateId = {Id})",
                                                communicationTemplate.CommTemplateId);
                                    }
                                    else
                                        Log.Information("CommunicationMiddleware - There is no communication definition ! (TemplateId = {Id})", communicationTemplate.CommTemplateId);
                                }
                                else
                                    Log.Debug("CommunicationMiddleware - Condition execution returned {Result} based on request parameters.", false);
                            }
                            catch (Exception ex)
                            {
                                Log.Fatal(ex, "CommunicationMiddleware - CommunicationTemplates - loop");
                            }
                        }
                    }
                    else
                        Log.Debug("CommunicationMiddleware - There is no communication template !");
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "CommunicationMiddleware - Invoke");
                throw;
            }
            finally
            {
                if (!nextInvoked)
                    await _next.Invoke(context);
            }
        }
    }
}