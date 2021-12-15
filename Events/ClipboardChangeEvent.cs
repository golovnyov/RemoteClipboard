namespace VH.RemoteClipboard.Events
{
    public class ClipboardChangedEventArgs
    {
        public ClipboardChangedEventArgs(string value)
        {
            Text = value;
        }

        public string Text { get; private set; }
    }

    public delegate void ClipboardChangedEventHandler(object sender, ClipboardChangedEventArgs eventArgs);
}
