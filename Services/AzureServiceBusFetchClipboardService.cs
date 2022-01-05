using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using VH.RemoteClipboard.Configuration;
using VH.RemoteClipboard.Extensions;
using VH.RemoteClipboard.Mediator;

namespace VH.RemoteClipboard.Services
{
    public class AzureServiceBusRemoteClipboardService : IHostedService, IDisposable
    {
        private readonly ILogger logger;
        private readonly ServiceBusConfiguration serviceBusConfiguration;
        private readonly IMediator mediator;

        private ServiceBusProcessor processor;
        private readonly ServiceBusClient client;

        public AzureServiceBusRemoteClipboardService(
            ILogger<AzureServiceBusRemoteClipboardService> logger,
            IOptions<ServiceBusConfiguration> serviceBusOptions,
            ServiceBusClient client,
            IMediator mediator)
        {
            this.logger = logger;
            this.serviceBusConfiguration = serviceBusOptions.Value;
            this.client = client;
            this.mediator = mediator;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await PeekMessageAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
        }

        protected virtual void OnClipboardChange(string value)
        {
            mediator.NotifyWithText(this, value);
        }

        private async Task PeekMessageAsync()
        {
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

            OnClipboardChange(messageBody);

            logger.LogDebug("Fetched message [{messageBody}]", messageBody);

            logger.LogInformation("Remote clipboard data fetched at [{dateTimeNowUtc}]", DateTime.UtcNow);

            await Task.CompletedTask;
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
