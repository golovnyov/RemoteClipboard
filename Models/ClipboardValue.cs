using System;

namespace VH.RemoteClipboard.Models
{
    public class ClipboardValue : IEquatable<ClipboardValue>
    {
        private string textValue;

        public bool IsText { get; set; }

        public void SetText(string text)
        {
            IsText = true;

            textValue = text;
        }

        public string GetText()
        {
            return textValue;
        }

        public bool Equals(ClipboardValue other)
        {
            return this == other;
        }

        public override bool Equals(Object obj)
        {
            return base.Equals(obj) || Equals(this, obj as ClipboardValue);
        }

        public override int GetHashCode()
        {
            return this.GetText().GetHashCode();
        }

        public static bool operator ==(ClipboardValue obj1, ClipboardValue obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(ClipboardValue obj1, ClipboardValue obj2)
        {
            return !Equals(obj1, obj2);
        }

        private static bool Equals(ClipboardValue obj1, ClipboardValue obj2)
        {
            if (obj1 is null ^ obj2 is null)
            {
                return false;
            }

            return string.Equals(obj1.GetText(), obj2.GetText(), StringComparison.Ordinal);
        }
    }
}
