using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using VH.RemoteClipboard.Configuration;
using VH.RemoteClipboard.Events;
using VH.RemoteClipboard.Mediator;

namespace VH.RemoteClipboard.Services
{
    public class AzureServiceBusLocalClipboardService : IHostedService
    {
        private readonly ILogger logger;
        private readonly IMediator mediator;
        private readonly ServiceBusConfiguration serviceBusConfiguration;
        private readonly ServiceBusSender sender;

        public AzureServiceBusLocalClipboardService(
            ILogger<AzureServiceBusLocalClipboardService> logger,
            IOptions<ServiceBusConfiguration> serviceBusOptions,
            ServiceBusClient serviceBusClient, 
            IMediator mediator)
        {
            this.logger = logger;
            this.serviceBusConfiguration = serviceBusOptions.Value;
            this.sender = serviceBusClient.CreateSender(serviceBusConfiguration.TopicName);
            this.mediator = mediator;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            mediator.ClipboardChanged += Mediator_ClipboardChanged;
        }

        private async void Mediator_ClipboardChanged(object sender, ClipboardChangedEventArgs eventArgs)
        {
            await SendMessageAsync(eventArgs.ClipboardValue.GetText());

            logger.LogInformation("Local clipboard data shared at [{dateTimeNowUtc}]", DateTime.UtcNow);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
        }

        private async Task SendMessageAsync(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            logger.LogDebug($"Sending a single message to the topic: {serviceBusConfiguration.TopicName}");

            ServiceBusMessage message = new(value);

            message.ApplicationProperties[nameof(ServiceBusMessage.To)] = serviceBusConfiguration.SubscriptionName;

            await sender.SendMessageAsync(message);

            logger.LogDebug($"Sent a single message to the topic: {serviceBusConfiguration.TopicName}");
        }
    }
}
