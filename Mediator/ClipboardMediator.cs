using VH.RemoteClipboard.Events;
using VH.RemoteClipboard.Models;

namespace VH.RemoteClipboard.Mediator
{
    public class ClipboardMediator : IMediator
    {
        public event ClipboardChangedEventHandler ClipboardChanged;

        public void Notify(object sender, ClipboardValue value)
        {
            ClipboardChanged?.Invoke(sender, new ClipboardChangedEventArgs(value));
        }
    }
}
