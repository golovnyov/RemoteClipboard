using VH.RemoteClipboard.Events;
using VH.RemoteClipboard.Models;

namespace VH.RemoteClipboard.Mediator;

public interface IMediator
{
    event ClipboardChangedEventHandler LocalClipboardChanged;

    event ClipboardChangedEventHandler RemoteClipboardChanged;

    void NotifyLocalClipboardChanged(object sender, ClipboardValue value);

    void NotifyRemoteClipboardChanged(object sender, ClipboardValue value);
}