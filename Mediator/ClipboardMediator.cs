using VH.RemoteClipboard.Events;
using VH.RemoteClipboard.Models;

namespace VH.RemoteClipboard.Mediator;

public class ClipboardMediator : IMediator
{
    public event ClipboardChangedEventHandler LocalClipboardChanged;
    public event ClipboardChangedEventHandler RemoteClipboardChanged;

    public void NotifyLocalClipboardChanged(object sender, ClipboardValue value)
    {
        LocalClipboardChanged?.Invoke(sender, new ClipboardChangedEventArgs(value));
    }
    
    public void NotifyRemoteClipboardChanged(object sender, ClipboardValue value)
    {
        RemoteClipboardChanged?.Invoke(sender, new ClipboardChangedEventArgs(value));
    }
}