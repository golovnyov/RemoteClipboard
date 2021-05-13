using System.Threading.Tasks;

namespace VH.RemoteClipboard.Services
{
    public interface IShareClipboardService
    {
        Task ShareClipboardDataAsync(string value);
    }
}