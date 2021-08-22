using System;
using System.Net.Http;
using System.Security.Authentication;
using DemoAPI.Enums;
using DemoAPI.Resolver;
using DemoAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DemoAPI.Helpers
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterInternalDependencies(this IServiceCollection services)
        {
            services.AddTransient<IFileHandler, FileHandler>();
            services.AddTransient<AWSService>();
            services.AddTransient<GCPService>();
            services.AddTransient<ProviderResolver>(serviceProvider => key => GetServiceProviderDeposit(key, serviceProvider));
            return services;
        }

        private static IProvider GetServiceProviderDeposit(Provider key, IServiceProvider serviceProvider)
        {
            return key switch
            {
                Provider.AWS => serviceProvider.GetService<AWSService>(),
                Provider.GCP => serviceProvider.GetService<GCPService>(),
                _ => throw new ArgumentOutOfRangeException(nameof(key), key, null)
            };
        }
    }
}