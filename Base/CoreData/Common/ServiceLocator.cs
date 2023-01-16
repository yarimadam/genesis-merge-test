using System;
using System.Collections.Generic;
using CoreType.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CoreData.Common
{
    public class ServiceLocator
    {
        private readonly IServiceProvider _currentServiceProvider;
        private static IServiceProvider _serviceProvider;

        public static ServiceLocator Current => _serviceProvider != null ? new ServiceLocator(_serviceProvider) : null;

        public ServiceLocator(IServiceProvider currentServiceProvider)
        {
            _currentServiceProvider = currentServiceProvider;
        }

        public static void SetLocatorProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object GetService(Type serviceType)
        {
            return _currentServiceProvider.GetService(serviceType);
        }

        public TService GetService<TService>()
        {
            return _currentServiceProvider.GetService<TService>();
        }

        public IEnumerable<TService> GetServices<TService>()
        {
            return _currentServiceProvider.GetServices<TService>();
        }

        public IServiceProvider GetServices()
        {
            return _currentServiceProvider;
        }

        #region Extensions

        public static SessionAccessor GetSessionAccessor()
        {
            return Current?.GetService<SessionAccessor>();
        }

        public static SessionContext GetSession()
        {
            // Tries to get session from current http context.
            var session = GetHttpContextAccessor()?.HttpContext?.RequestServices.GetService<SessionContext>();

            // Tries to get session from the services.
            session ??= Current?.GetService<SessionContext>();

            // Tries to get session from newly created service scope.
            session ??= Current?.GetServices().CreateScope().ServiceProvider.GetService<SessionContext>();

            session ??= new SessionContext();

            return session;
        }

        public static IHttpContextAccessor GetHttpContextAccessor()
        {
            return Current?.GetService<IHttpContextAccessor>();
        }

        #endregion
    }
}