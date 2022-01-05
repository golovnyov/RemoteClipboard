using VH.RemoteClipboard.Events;
using VH.RemoteClipboard.Models;

namespace VH.RemoteClipboard.Mediator
{
    public interface IMediator
    {
        event ClipboardChangedEventHandler ClipboardChanged;

        void Notify(object sender, ClipboardValue value);
    }
}