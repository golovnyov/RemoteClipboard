using VH.RemoteClipboard.Models;

namespace VH.RemoteClipboard.Events
{
    public class ClipboardChangedEventArgs
    {
        public ClipboardChangedEventArgs(ClipboardValue value)
        {
            ClipboardValue = value;
        }

        public ClipboardValue ClipboardValue { get; private set; }
    }

    public delegate void ClipboardChangedEventHandler(object sender, ClipboardChangedEventArgs eventArgs);
}
