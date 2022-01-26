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
    public class LocalClipboardDataProvider : IHostedService
    {
        private readonly ILogger logger;
        private readonly IMediator mediator;

        private readonly System.Timers.Timer timer;

        private readonly ServiceBusConfiguration serviceBusConfiguration;
        private readonly ServiceBusSender sender;

        public LocalClipboardDataProvider(ILogger<AzureServiceBusLocalClipboardService> logger, IMediator mediator)
        {
            this.logger = logger;
            this.mediator = mediator;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async void PublishClipboardValue(ClipboardValue clipboardValue)
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
