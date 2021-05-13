using System.Threading.Tasks;
using VH.RemoteClipboard.Events;

namespace VH.RemoteClipboard.Services
{
    public interface IClipboardProvider
    {
        event ClipboardChangedEventHandler ClipboardChanged;

        Task SetValueAsync(string value);
    }
}