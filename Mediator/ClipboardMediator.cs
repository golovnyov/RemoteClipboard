using Microsoft.Extensions.Logging;
using VH.RemoteClipboard.Events;
using VH.RemoteClipboard.Models;

namespace VH.RemoteClipboard.Mediator
{
    public class ClipboardMediator : IMediator
    {
        private readonly ILogger logger;
        private ClipboardValue clipboardValue;

        public event ClipboardChangedEventHandler ClipboardChanged;

        public ClipboardMediator(ILogger<IMediator> logger)
        {
            this.logger = logger;
        }

        public void Notify(object sender, ClipboardValue value)
        {
            if (clipboardValue == value)
            {
                return;
            }

            clipboardValue = value;

            ClipboardChanged?.Invoke(sender, new ClipboardChangedEventArgs(value));
        }
    }
}
