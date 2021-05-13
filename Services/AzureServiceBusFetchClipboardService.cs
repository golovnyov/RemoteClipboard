using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VH.RemoteClipboard.Configuration;

namespace VH.RemoteClipboard.Services
{
    public class AzureServiceBusFetchClipboardService : IFetchClipboardService, IDisposable
    {
        private readonly ILogger logger;
        private readonly IClipboardProvider clipboard;
        private readonly ServiceBusConfiguration serviceBusConfiguration;

        private ServiceBusProcessor processor;
        private ServiceBusClient client;

        public AzureServiceBusFetchClipboardService(
            ILogger<AzureServiceBusFetchClipboardService> logger,
            IOptions<ServiceBusConfiguration> serviceBusOptions,
            IClipboardProvider clipboard)
        {
            this.logger = logger;
            serviceBusConfiguration = serviceBusOptions.Value;
            this.clipboard = clipboard;
        }

        public async Task FetchClipboardDataAsync()
        {
            await PeekMessageAsync();
        }

        private async Task PeekMessageAsync()
        {
            client = new ServiceBusClient(serviceBusConfiguration.ConnectionString);

            var options = new ServiceBusProcessorOptions() { ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete };

            processor = client.CreateProcessor(serviceBusConfiguration.TopicName, serviceBusConfiguration.SubscriptionName, options);

            // add handler to process messages
            processor.ProcessMessageAsync += ProcessMessageHandlerAsync;

            // add handler to process any errors
            processor.ProcessErrorAsync += ErrorHandler;

            // start processing 
            await processor.StartProcessingAsync();
        }

        private async Task ProcessMessageHandlerAsync(ProcessMessageEventArgs processMessageEventArgs)
        {
            logger.LogDebug("Processing message");

            string messageBody = processMessageEventArgs.Message.Body.ToString();

            await clipboard.SetValueAsync(messageBody);

            logger.LogDebug("Fetched message [{messageBody}]", messageBody);

            logger.LogInformation("Remote clipboard data fetched at [{dateTimeNowUtc}]", DateTime.UtcNow);
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            logger.LogError(args.Exception, "An error ocurred while processing message");

            return Task.CompletedTask;
        }

        #region Dispose

        private bool disposedValue;

        protected virtual async void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    await processor?.StopProcessingAsync();

                    await processor.DisposeAsync();

                    await client.DisposeAsync();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
        }

        #endregion
    }
}
