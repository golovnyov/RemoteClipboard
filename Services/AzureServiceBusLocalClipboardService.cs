using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VH.RemoteClipboard.Configuration;

namespace VH.RemoteClipboard.Services
{
    public class AzureServiceBusLocalClipboardService : ILocalClipboardService
    {
        private readonly ILogger logger;
        private readonly ServiceBusConfiguration serviceBusConfiguration;

        public AzureServiceBusLocalClipboardService(ILogger<AzureServiceBusLocalClipboardService> logger, IOptions<ServiceBusConfiguration> serviceBusOptions)
        {
            this.logger = logger;
            serviceBusConfiguration = serviceBusOptions.Value;
        }

        public async Task ShareClipboardDataAsync(string value)
        {
            await SendMessageAsync(value);

            logger.LogInformation("Local clipboard data shared at [{dateTimeNowUtc}]", DateTime.UtcNow);
        }

        private async Task SendMessageAsync(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            logger.LogDebug($"Sending a single message to the topic: {serviceBusConfiguration.TopicName}");

            await using (ServiceBusClient client = new(serviceBusConfiguration.ConnectionString))
            {
                ServiceBusSender sender = client.CreateSender(serviceBusConfiguration.TopicName);

                ServiceBusMessage message = new(value);

                message.ApplicationProperties[nameof(ServiceBusMessage.To)] = serviceBusConfiguration.SubscriptionName;

                await sender.SendMessageAsync(message);

                logger.LogDebug($"Sent a single message to the topic: {serviceBusConfiguration.TopicName}");
            }
        }
    }
}
