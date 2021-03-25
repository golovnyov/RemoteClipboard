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
            try
            {
                RunWatcherTimer();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error ocurred while fetching and sharing clipboard data");
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopping application.");
        }

        private void RunWatcherTimer()
        {
            System.Timers.Timer myTimer = new System.Timers.Timer();

            myTimer.Elapsed += new ElapsedEventHandler(HandleElapsedTimer);
            myTimer.Interval = 5000;
            myTimer.Enabled = true;
        }

        private async void HandleElapsedTimer(object sender, ElapsedEventArgs e)
        {
            System.Timers.Timer myTimer = sender as System.Timers.Timer;

            myTimer.Enabled = false;

            string clipboardData = clipboard.GetText();

            if (!string.IsNullOrWhiteSpace(clipboardData) && !clipboardData.Equals(ClipboardDataCache, StringComparison.OrdinalIgnoreCase))
            {
                logger.LogDebug("Setting value to clipboard [{clipboardData}]", clipboardData);

                ClipboardDataCache = clipboardData;

                await shareClipboardService.ShareClipboardDataAsync(clipboardData);
            }
            else
            {
                logger.LogDebug("Clipboard data is empty or has not been changed");
            }

            myTimer.Enabled = true;
        }
    }
}
