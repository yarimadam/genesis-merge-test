using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CoreData.Common;
using CoreType.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CoreSvc.Filters
{
    public class AuthorizationHeaderParameterOperationFilter : IOperationFilter
    {
        private static readonly List<IOpenApiAny> ALL_LANGUAGE_CODES = new List<IOpenApiAny>(
            ReflectionHelper.GetProperties(typeof(Translations), false, 0)
                .Select(x => new OpenApiString(x.Name))
                .ToList());

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var authorizeAttribute = context.MethodInfo.GetCustomAttribute<AuthorizeAttribute>(true)
                                     ?? context.MethodInfo.DeclaringType?.GetCustomAttribute<AuthorizeAttribute>(true);
            var allowAnonymousAttribute = context.MethodInfo.GetCustomAttribute<AllowAnonymousAttribute>(true)
                                          ?? context.MethodInfo.DeclaringType?.GetCustomAttribute<AllowAnonymousAttribute>(true);

            if (authorizeAttribute != null && allowAnonymousAttribute == null)
            {
                operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });

                var oAuthScheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                };

                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        [oAuthScheme] = new[] { authorizeAttribute.Policy }
                    }
                };
            }

            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            //Add custom header to change language
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-Accept-Language",
                In = ParameterLocation.Header,
                Description = "(Optional) Sets and overrides which language is preferred.",
                Required = false,
                Schema = new OpenApiSchema
                {
                    Type = "Enum",
                    MinLength = 2,
                    MaxLength = 2,
                    Enum = ALL_LANGUAGE_CODES
                },
            });
            //Optionally, this part can be activated.
            //operation.Parameters.Add(new OpenApiParameter
            //{
            //    Name = "X-TenantId",
            //    In = ParameterLocation.Header,
            //    Description = "(Optional) Sets and overrides Tenant.",
            //    Required = false,
            //    Schema = new OpenApiSchema
            //    {
            //        Type = "int",
            //    }
            //});
        }
    }
}