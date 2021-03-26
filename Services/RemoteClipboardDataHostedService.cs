using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace VH.RemoteClipboard.Services
{
    public class RemoteClipboardDataHostedService : IHostedService
    {
        private readonly ILogger logger;
        private readonly IFetchClipboardService fetchClipboardService;

        public RemoteClipboardDataHostedService(ILogger<RemoteClipboardDataHostedService> logger, IFetchClipboardService fetchClipboardService)
        {
            this.logger = logger;
            this.fetchClipboardService = fetchClipboardService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogDebug("Setting up fetching shared clipboard data");

            try
            {
                await fetchClipboardService.FetchClipboardDataAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error ocurred while fetching and setting data in local clipboard");
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopping {remoteClipboardDataHostedService}...", nameof(RemoteClipboardDataHostedService));

            using ((IDisposable)fetchClipboardService)
            {
            }

            await Task.CompletedTask;

            logger.LogInformation("Stopped {localClipboardDataHostedService}.", nameof(LocalClipboardDataHostedService));
        }
    }
}
