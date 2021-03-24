using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TextCopy;

namespace VH.RemoteClipboard
{
    public class Worker : IHostedService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IClipboard clipboard;

        public Worker(ILogger<Worker> logger, IClipboard clipboard)
        {
            _logger = logger;
            this.clipboard = clipboard;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await clipboard.SetTextAsync("Text to place in clipboard1");

            _logger.LogDebug("Setting value to clipboard {}");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping application.");
        }
    }
}
