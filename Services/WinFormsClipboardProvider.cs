using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using VH.RemoteClipboard.Events;

namespace VH.RemoteClipboard.Services
{
    public class WinFormsClipboardProvider : IClipboardProvider
    {
        private string oldClipboardValue;

        public event ClipboardChangedEventHandler ClipboardChanged;

        
        public async Task SetValueAsync(string value)
        {
            Clipboard.SetText(value);

            OnClipboardChange(value);

            await Task.CompletedTask;
        }

        protected virtual void OnClipboardChange(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || string.Equals(oldClipboardValue, value, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            oldClipboardValue = value;

            ClipboardChanged?.Invoke(this, new ClipboardChangedEventArgs(value));
        }
    }
}
