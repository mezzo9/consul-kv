using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace OrderService
{
    public static class ConsulHelper
    {
        public static IServiceCollection AddConsulConfig(this IServiceCollection services, string consulAddress)
        {
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
            {
                consulConfig.Address = new Uri(consulAddress);
            }));
            return services;
        }

        public static IApplicationBuilder UseConsul(this IApplicationBuilder app, IConfiguration configuration)
        {
            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
            var logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>().CreateLogger("AppExtensions");
            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            var what = configuration.GetSection("WhatAmI").Get<WhatAmI>();
            var consulAddress = configuration.GetValue<string>("Consul");
            var appPort = configuration.GetValue<int>("WebServicePort");
            var appAddress = configuration.GetValue<string>("WebAddress");
            var httpCheck = new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(30),
                Interval = TimeSpan.FromSeconds(10),
                HTTP = $"http://{appAddress}:{appPort}/HealthCheck"
            };

            var tcpCheck = new AgentServiceCheck
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(30),
                Interval = TimeSpan.FromSeconds(10),
                TCP = $"{appAddress}:{appPort}"
            };
            //var uri = new Uri(address);
            var registration = new AgentServiceRegistration
            {
                ID = what.ServiceName + appPort,
                Name = what.ServiceName,
                Address = appAddress,
                Port = appPort,
                Checks = new[] { tcpCheck, httpCheck }
            };

            logger.LogInformation("Registering with Consul");
            consulClient.Agent.ServiceDeregister(registration.ID).ConfigureAwait(true);
            consulClient.Agent.ServiceRegister(registration).ConfigureAwait(true);

            lifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation("Unregistering from Consul");
            });

            return app;
        }
    }

    public class WhatAmI
    {
        public string ServiceName { get; set; }

    }
}
