using System.Threading.Tasks;

namespace VH.RemoteClipboard.Services
{
    public interface ILocalClipboardService
    {
        Task ShareClipboardDataAsync(string value);
    }
}