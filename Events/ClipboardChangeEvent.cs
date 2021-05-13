namespace VH.RemoteClipboard.Events
{
    public class ClipboardChangedEventArgs
    {
        public ClipboardChangedEventArgs(string value)
            : this(value, null)
        {
        }

        public ClipboardChangedEventArgs(string value, string oldValue)
        {
            Text = value;
            OldText = oldValue;
        }

        public string OldText { get; private set; }

        public string Text { get; private set; }
    }

    public delegate void ClipboardChangedEventHandler(object sender, ClipboardChangedEventArgs eventArgs);
}
