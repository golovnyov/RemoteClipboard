using VH.RemoteClipboard.Mediator;
using VH.RemoteClipboard.Models;

namespace VH.RemoteClipboard.Extensions
{
    public static class MediatorExtensions
    {
        public static void NotifyWithText(this IMediator clipboardMediator, object sender, string text)
        {
            var cpbValue = new ClipboardValue();

            cpbValue.SetText(text);

            clipboardMediator.Notify(sender, cpbValue);
        }
    }
}
