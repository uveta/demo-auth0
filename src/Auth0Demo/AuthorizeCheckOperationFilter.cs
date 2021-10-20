using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Auth0Demo
{
    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        private readonly IOptions<AuthorizationOptions> _authorizationOptions;

        public AuthorizeCheckOperationFilter(IOptions<AuthorizationOptions> authorizationOptions)
        {
            _authorizationOptions = authorizationOptions;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var authorizeAttributes = new List<AuthorizeAttribute>();

            if (context.ApiDescription.ActionDescriptor is ControllerActionDescriptor actionDescriptor)
            {
                authorizeAttributes.AddRange(actionDescriptor.ControllerTypeInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>());
                authorizeAttributes.AddRange(actionDescriptor.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>());
            }

            if (authorizeAttributes.Count > 0)
            {
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

#pragma warning disable 8619
                IEnumerable<string> policies = authorizeAttributes
                    .Where(x => x.Policy is not null)
                    .Select(x => x.Policy)
                    .Distinct();
#pragma warning restore 8619
                var authorization = _authorizationOptions.Value;
                var scopes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var name in policies)
                {
                    var policy = authorization.GetPolicy(name);
                    if (policy is null) continue;
                    var claimRequirements = policy.Requirements.OfType<ClaimsAuthorizationRequirement>().ToList();
                    if (claimRequirements.Count == 0)
                    {
                        // TODO: how to get required scopes when defined by handler?
                        scopes.Add("orders:read");
                        scopes.Add("orders:full");
                    }
                    else
                    {
                        foreach (var claimRequirement in claimRequirements)
                        {
                            if (claimRequirement.AllowedValues is null) continue;
                            foreach (var requiredScope in claimRequirement.AllowedValues)
                            {
                                scopes.Add(requiredScope);
                            }
                        }
                    }
                }

                var oAuthScheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                };
                var requirement = new OpenApiSecurityRequirement
                {
                    [oAuthScheme] = scopes.ToList()
                };
                operation.Security.Add(requirement);
            }
        }
    }
}