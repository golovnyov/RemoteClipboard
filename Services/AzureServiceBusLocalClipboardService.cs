using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using VH.RemoteClipboard.Configuration;
using VH.RemoteClipboard.Mediator;
using VH.RemoteClipboard.Models;

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

            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        private void Mediator_ClipboardChanged(object sender, Events.ClipboardChangedEventArgs eventArgs)
        {
            PublishClipboardValue(eventArgs.ClipboardValue);
        }

        private async void PublishClipboardValue(ClipboardValue clipboardValue)
        {
            if (clipboardValue is null)
            {
                throw new ArgumentNullException(nameof(clipboardValue));
            }

            await SendMessageAsync(clipboardValue.GetText());
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
