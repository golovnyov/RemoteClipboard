using System;
using System.Threading.Tasks;

namespace VH.RemoteClipboard.Services
{
    public interface IFetchClipboardService
    {
        Task FetchClipboardDataAsync();
    }
}