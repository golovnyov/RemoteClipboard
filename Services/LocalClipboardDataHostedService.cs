using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TextCopy;

namespace VH.RemoteClipboard.Services
{
    public class LocalClipboardDataHostedService : IHostedService
    {
        private readonly ILogger logger;
        private readonly IClipboard clipboard;
        private readonly IShareClipboardService shareClipboardService;
        private readonly ILocalClipboardCurrent localClipboardCurrent;

        public LocalClipboardDataHostedService(
            ILogger<LocalClipboardDataHostedService> logger,
            IClipboard clipboard,
            IShareClipboardService shareClipboardService,
            ILocalClipboardCurrent localClipboardCurrent)
        {
            this.logger = logger;
            this.clipboard = clipboard;
            this.shareClipboardService = shareClipboardService;
            this.localClipboardCurrent = localClipboardCurrent;
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

            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopping {localClipboardDataHostedService}...", nameof(LocalClipboardDataHostedService));

            await Task.CompletedTask;
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

            if (!string.IsNullOrWhiteSpace(clipboardData) && !clipboardData.Equals(localClipboardCurrent.Value, StringComparison.OrdinalIgnoreCase))
            {
                logger.LogDebug("Setting value to clipboard [{clipboardData}]", clipboardData);

                localClipboardCurrent.Value = clipboardData;

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
