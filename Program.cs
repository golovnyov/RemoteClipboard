using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VH.RemoteClipboard.Configuration;
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
            SetApplicationDefaults();

            var host = CreateHostBuilder(args).Build();

            var mainForm = host.Services.GetRequiredService<MainForm>();

            Application.Run(mainForm);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
               .ConfigureServices((hostContext, services) =>
               {
                   services.AddSingleton<MainForm>();

                   services.AddScoped<IShareClipboardService, AzureServiceBusShareClipboardService>();
                   services.AddScoped<IFetchClipboardService, AzureServiceBusFetchClipboardService>();

                   services.AddSingleton<IClipboardProvider, WinFormsClipboardProvider>();

                   services.Configure<ServiceBusConfiguration>(hostContext.Configuration.GetSection(ServiceBusConfiguration.ServiceBusSectionName));
               });

        private static void SetApplicationDefaults()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        }
    }
}
