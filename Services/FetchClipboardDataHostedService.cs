using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TextCopy;

namespace VH.RemoteClipboard.Services
{
    public class FetchClipboardDataHostedService : IHostedService
    {
        private readonly ILogger logger;
        private readonly IClipboard clipboard;
        private readonly IShareClipboardService shareClipboardService;

        private string ClipboardDataCache = null;

        public FetchClipboardDataHostedService(ILogger<FetchClipboardDataHostedService> logger, IClipboard clipboard, IShareClipboardService shareClipboardService)
        {
            this.logger = logger;
            this.clipboard = clipboard;
            this.shareClipboardService = shareClipboardService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            RunWatcherTimer();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopping application.");
        }

        private void RunWatcherTimer()
        {
            var myTimer = new System.Timers.Timer();

            myTimer.Elapsed += new ElapsedEventHandler(HandleElapsedTimer);
            myTimer.Interval = 5000;
            myTimer.Enabled = true;
        }

        private async void HandleElapsedTimer(object sender, ElapsedEventArgs e)
        {
            string clipboardData = clipboard.GetText();

            if (string.IsNullOrWhiteSpace(clipboardData) || clipboardData.Equals(ClipboardDataCache, StringComparison.OrdinalIgnoreCase))
            {
                logger.LogDebug("Clipboard data is empty or has not been changed");

                return;
            }

            logger.LogDebug("Setting value to clipboard [{clipboardData}]", clipboardData);

            ClipboardDataCache = clipboardData;

            await shareClipboardService.ShareClipboardDataAsync(clipboardData);
        }
    }
}
