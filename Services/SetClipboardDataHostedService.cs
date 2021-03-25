using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TextCopy;

namespace VH.RemoteClipboard.Services
{
    public class SetClipboardDataHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IClipboard clipboard;

        public SetClipboardDataHostedService(ILogger<SetClipboardDataHostedService> logger, IClipboard clipboard)
        {
            _logger = logger;
            this.clipboard = clipboard;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Setting value to clipboard {}");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping application.");
        }
    }
}
