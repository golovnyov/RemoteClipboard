namespace VH.RemoteClipboard.Configuration
{
    public class ServiceBusConfiguration
    {
        public const string ServiceBusSectionName = "ServiceBus";

        public string ConnectionString { get; set; }

        public string QueueName { get; set; }
    }
}
