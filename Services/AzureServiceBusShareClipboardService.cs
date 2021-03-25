using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VH.RemoteClipboard.Configuration;

namespace VH.RemoteClipboard.Services
{
    public class AzureServiceBusShareClipboardService : IShareClipboardService
    {
        private readonly ILogger logger;
        private readonly ServiceBusConfiguration serviceBusConfiguration;

        public AzureServiceBusShareClipboardService(ILogger<AzureServiceBusShareClipboardService> logger, IOptions<ServiceBusConfiguration> serviceBusOptions)
        {
            this.logger = logger;
            serviceBusConfiguration = serviceBusOptions.Value;
        }

        public async Task ShareClipboardDataAsync(string value)
        {
            await ReceiveAndDeleteMessagesAsync();

            await SendMessageAsync(value);
        }

        private async Task SendMessageAsync(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            logger.LogDebug($"Sending a single message to the queue: {serviceBusConfiguration.QueueName}");

            await using (ServiceBusClient client = new ServiceBusClient(serviceBusConfiguration.ConnectionString))
            {
                ServiceBusSender sender = client.CreateSender(serviceBusConfiguration.QueueName);

                ServiceBusMessage message = new ServiceBusMessage(value);

                await sender.SendMessageAsync(message);

                logger.LogDebug($"Sent a single message to the queue: {serviceBusConfiguration.QueueName}");
            }
        }

        private async Task ReceiveAndDeleteMessagesAsync()
        {
            logger.LogDebug($"Clearing the queue: {serviceBusConfiguration.QueueName}");

            await using (ServiceBusClient client = new ServiceBusClient(serviceBusConfiguration.ConnectionString))
            {
                var receiver = client.CreateReceiver(serviceBusConfiguration.QueueName, new ServiceBusReceiverOptions() { ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete });

                ServiceBusReceivedMessage msg = await receiver.ReceiveMessageAsync(TimeSpan.FromSeconds(1));

                while (msg != null)
                {
                    logger.LogDebug("Receiving and deleting message");
                    msg = await receiver.ReceiveMessageAsync(TimeSpan.FromSeconds(1));
                }
            }
        }
    }
}
