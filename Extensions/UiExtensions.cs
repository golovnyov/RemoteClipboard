using System;

namespace VH.RemoteClipboard.Extensions
{
    public static class UiExtensions
    {
        public static string PrepareClipboardText(this string value, int trimToLength = 75)
        {
            var trimmedValue = value?.Trim();

            return trimmedValue?.Substring(0, Math.Min(trimmedValue.Length, trimToLength));
        }
    }
}
