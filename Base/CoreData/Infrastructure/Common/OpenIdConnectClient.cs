using System;
using System.Net.Http;
using System.Threading.Tasks;
using CoreData.Common;
using CoreType;
using IdentityModel;
using IdentityModel.Client;

namespace CoreData.Infrastructure.Common
{
    public class OpenIdConnectClient : IDisposable
    {
        private readonly HttpClient httpClient;
        private DiscoveryDocumentResponse discoveryResponse;

        public OpenIdConnectClient()
        {
            httpClient = new HttpClient();
        }

        public async Task<DiscoveryDocumentResponse> GetDiscoveryDocumentAsync()
        {
            var response = await httpClient.GetDiscoveryDocumentAsync(ConfigurationManager.IdentityUrl);

            if (response.IsError)
                HandleException(response);

            return response;
        }

        public async Task<TokenResponse> RequestPasswordTokenAsync(IdentityUserTokenInput input)
        {
            if (string.IsNullOrEmpty(input.Email))
                throw new ArgumentNullException(nameof(input.Email));
            if (string.IsNullOrEmpty(input.Password))
                throw new ArgumentNullException(nameof(input.Password));

            if (string.IsNullOrEmpty(input.CustomClientId))
                input.CustomClientId = Constants.DEFAULT_IDENTITY_CLIENT_ID;
            if (string.IsNullOrEmpty(input.CustomSecretKey))
                input.CustomSecretKey = ConfigurationManager.GetIdentityServerSharedSecret(input.CustomClientId);

            discoveryResponse ??= await GetDiscoveryDocumentAsync();

            var response = await httpClient.RequestPasswordTokenAsync(
                new PasswordTokenRequest
                {
                    Scope = OidcConstants.StandardScopes.OpenId,
                    Address = discoveryResponse.TokenEndpoint,
                    UserName = input.Email,
                    Password = input.Password,
                    ClientId = input.CustomClientId,
                    ClientSecret = input.CustomSecretKey
                });

            if (response.IsError)
                HandleException(response);

            return response;
        }

        public async Task<(TokenResponse TokenResponse, UserInfoResponse UserInfoResponse)> GetUserInfoAsync(IdentityUserTokenInput input)
        {
            discoveryResponse ??= await GetDiscoveryDocumentAsync();

            TokenResponse tokenResponse = null;
            if (string.IsNullOrEmpty(input.Token))
            {
                tokenResponse = await RequestPasswordTokenAsync(input);

                input.Token = tokenResponse.AccessToken;
            }

            var response = await httpClient.GetUserInfoAsync(
                new UserInfoRequest
                {
                    Address = discoveryResponse.UserInfoEndpoint,
                    Token = input.Token
                });

            if (response.IsError)
                HandleException(response);

            return (tokenResponse, response);
        }

        private void HandleException(ProtocolResponse response)
        {
            if (response.Exception != null)
                response.Exception.ReThrow();
            else if (response is TokenResponse tokenResponse)
                throw new HttpRequestException(tokenResponse.ErrorDescription);
            else
                throw new HttpRequestException(response.Error);
        }

        public void Dispose()
        {
            httpClient?.Dispose();
        }
    }
}