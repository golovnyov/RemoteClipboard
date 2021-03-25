using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace VH.RemoteClipboard.Services
{
    public class AzureServiceBusShareClipboardService : IShareClipboardService
    {
        private readonly ILogger _logger;

        public AzureServiceBusShareClipboardService(ILogger<AzureServiceBusShareClipboardService> logger)
        {
            _logger = logger;
        }

        public async Task ShareClipboardDataAsync(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                _logger.LogDebug("Empty clibpoard data");

                return;
            }
        }
    }
}
