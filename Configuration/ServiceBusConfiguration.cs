namespace VH.RemoteClipboard.Configuration
{
    public class ServiceBusConfiguration
    {
        public const string ServiceBusSectionName = "ServiceBus";

        public string ConnectionString { get; set; }

        public string TopicName { get; set; }
        
        public string SubscriptionName { get; set; }
    }
}
