using VH.RemoteClipboard.Events;
using VH.RemoteClipboard.Models;

namespace VH.RemoteClipboard.Mediator;

public interface IMediator
{
    event ClipboardChangedEventHandler ClipboardChanged;

    void NotifyLocalClipboardChanged(object sender, ClipboardValue value);

    void NotifyRemoteClipboardChanged(object sender, ClipboardValue value);
}