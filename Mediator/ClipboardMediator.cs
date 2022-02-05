using VH.RemoteClipboard.Events;
using VH.RemoteClipboard.Models;

namespace VH.RemoteClipboard.Mediator;

public class ClipboardMediator : IMediator
{
    private ClipboardValue previousValue;

    public event ClipboardChangedEventHandler ClipboardChanged;

    public void NotifyLocalClipboardChanged(object sender, ClipboardValue value)
    {
        if (previousValue is null)
        {
            previousValue = value;

            return;
        }
        
        if (previousValue == value)
        {
            return;
        }

        InvokeEvent(sender, value);
    }

    public void NotifyRemoteClipboardChanged(object sender, ClipboardValue value)
    {
        InvokeEvent(sender, value);
    }

    private void InvokeEvent(object sender, ClipboardValue value)
    {
        previousValue = value;

        ClipboardChanged?.Invoke(sender, new ClipboardChangedEventArgs(value));
    }
}