using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Windows.Forms;
using VH.RemoteClipboard.Configuration;
using VH.RemoteClipboard.Mediator;
using VH.RemoteClipboard.Services;

namespace VH.RemoteClipboard
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;

            SetApplicationDefaults();

            var host = CreateHostBuilder(args).Build();

            var mainForm = host.Services.GetRequiredService<MainForm>();

            var logger = host.Services.GetRequiredService<ILogger<MainForm>>();

            try
            {
               var hostRunTask = host.RunAsync();

                Application.Run(mainForm);

                hostRunTask.Wait();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred.");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
               .ConfigureServices((hostContext, services) =>
               {
                   services.AddSingleton<MainForm>();
                   services.AddSingleton<IMediator, ClipboardMediator>();

                   services.AddHostedService<AzureServiceBusLocalClipboardService>();
                   services.AddHostedService<AzureServiceBusRemoteClipboardService>();

                   services.Configure<ServiceBusConfiguration>(hostContext.Configuration.GetSection(ServiceBusConfiguration.ServiceBusSectionName));

                   services.AddAzureClients(cfb =>
                   {
                       cfb
                       .AddServiceBusClient(hostContext.Configuration["ServiceBus:ConnectionString"])
                       .ConfigureOptions(options =>
                       {
                           options.RetryOptions.Delay = TimeSpan.FromMilliseconds(50);
                           options.RetryOptions.MaxDelay = TimeSpan.FromSeconds(5);
                           options.RetryOptions.MaxRetries = 3;
                       });
                   });
               });

        private static void SetApplicationDefaults()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        }
    }
}
