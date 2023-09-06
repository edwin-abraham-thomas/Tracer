using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracer.Interceptors;

namespace Tracer.Extensions.ServiceCollectionExtensions;

public static class AddTraceableServicesExtensions
{
    public static void AddTraceableTransient<TInterface, TImplementation>(this IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        bool trace = true;
        if (trace)
        {
            services.TryAddSingleton<IProxyGenerator, ProxyGenerator>();
            services.TryAddScoped<PublicMethodInterceptor>();

            services.AddTransient<TImplementation>();
            services.AddTransient(provider =>
            {
                var proxyGenerator = provider.GetRequiredService<IProxyGenerator>();
                var implementation = provider.GetRequiredService<TImplementation>();
                var interceptor = provider.GetRequiredService<PublicMethodInterceptor>();
                return proxyGenerator.CreateInterfaceProxyWithTarget<TInterface>(implementation, interceptor);
            });
        }
        else
        {
            services.AddTransient<TInterface, TImplementation>();
        }
    }
}
