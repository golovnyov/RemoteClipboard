using VH.RemoteClipboard.Models;

namespace VH.RemoteClipboard.Services
{
    public interface IPublishClipboardService
    {
        void PublishClipboardValue(ClipboardValue clipboardValue);
    }
}