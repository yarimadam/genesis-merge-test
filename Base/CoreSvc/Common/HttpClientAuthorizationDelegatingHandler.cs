using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace CoreSvc.Common
{
    public class HttpClientAuthorizationDelegatingHandler
        : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpClientAuthorizationDelegatingHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // TODO Replace all unsupported varients like "::0", "0.0.0.0"
            if (request.RequestUri.AbsoluteUri.Contains("0.0.0.0"))
                request.RequestUri = new Uri(request.RequestUri.AbsoluteUri.Replace("0.0.0.0", "localhost"));

            var authHeader = _httpContextAccessor?.HttpContext?.Request.Headers[HeaderNames.Authorization];
            if (!string.IsNullOrEmpty(authHeader))
                request.Headers.Add(HeaderNames.Authorization, new List<string> { authHeader });

            var bearerToken = await GetToken();
            if (bearerToken != null)
                request.SetBearerToken(bearerToken);

            return await base.SendAsync(request, cancellationToken);
        }

        private async Task<string> GetToken()
        {
            if (_httpContextAccessor?.HttpContext != null)
                return await _httpContextAccessor.HttpContext.GetTokenAsync(Constants.ACCESS_TOKEN_NAME);

            return null;
        }
    }
}