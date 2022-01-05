using System.Threading.Tasks;
using VH.RemoteClipboard.Events;

namespace VH.RemoteClipboard.Services
{
    public interface IRemoteClipboardService
    {
        event ClipboardChangedEventHandler ClipboardChanged;
    }
}