﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TextCopy;
using VH.RemoteClipboard.Configuration;
using VH.RemoteClipboard.Services;

namespace VH.RemoteClipboard
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
             Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<FetchClipboardDataHostedService>();
                    services.AddHostedService<SetClipboardDataHostedService>();

                    services.AddScoped<IShareClipboardService, AzureServiceBusShareClipboardService>();
                    services.AddScoped<IFetchClipboardService, AzureServiceBusFetchClipboardService>();

                    services.InjectClipboard();

                    services.Configure<ServiceBusConfiguration>(hostContext.Configuration.GetSection(ServiceBusConfiguration.ServiceBusSectionName));
                });
    }
}