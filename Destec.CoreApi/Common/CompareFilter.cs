using System.Globalization;

namespace System
{
    public static class CompareFilter
    {
        public static bool ContainsIgnoreNonSpacing(this string value, string filter)
        {
            if (string.IsNullOrEmpty(filter))
                return true;
            if (string.IsNullOrEmpty(value))
                return false;

            return CultureInfo.CurrentCulture.CompareInfo.IndexOf(value, filter, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0;
        }
    }
}
