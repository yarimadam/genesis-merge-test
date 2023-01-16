// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Linq;
using CoreData.Infrastructure;
using CoreType.Types;
using IdentityModel;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource
                {
                    Name = "genesisAPI",
                    //ApiSecrets = { new Secret("secret") },
                    UserClaims =
                    {
                        JwtClaimTypes.Subject,
                        JwtClaimTypes.Id,
                        JwtClaimTypes.Email,
                        JwtClaimTypes.PhoneNumber,
                        JwtClaimTypes.Name,
                        JwtClaimTypes.FamilyName,
                        CustomJwtClaimTypes.RoleId,
                        CustomJwtClaimTypes.RoleName,
                        CustomJwtClaimTypes.TenantId,
                        CustomJwtClaimTypes.TenantType,
                        CustomJwtClaimTypes.TenantName,
                        CustomJwtClaimTypes.SubTenantIds,
                        CustomJwtClaimTypes.ShouldChangePassword
                    },
                    Description = "genesisAPI",
                    DisplayName = "genesisAPI",
                    Enabled = true,
                    Scopes = { new Scope("genesisAPI") }
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            var allowedCorsOrigins = ConfigurationManager.AllowedCorsOrigins;
            var secrets = ConfigurationManager.IdentityServerSharedSecrets;
            var clients = ConfigurationManager.GetSection("IdentityServer:Clients").Get<List<Client>>();

            return clients
                .Select((client, i) =>
                {
                    // Add all AllowedCorsOrigins to client.
                    client.AllowedCorsOrigins = allowedCorsOrigins;

                    // Add specified secret to client.
                    if (secrets.ContainsKey(client.ClientId))
                    {
                        var rawSecret = secrets[client.ClientId];
                        if (!string.IsNullOrEmpty(rawSecret))
                            client.ClientSecrets.Add(new Secret(rawSecret.Sha256()));
                    }

                    return client;
                });
        }
    }
}