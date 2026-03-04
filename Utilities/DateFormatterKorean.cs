using System;
using System.Globalization;

namespace ScreenTimestampWin.Utilities
{
    public static class DateFormatterKorean
    {
        private static readonly CultureInfo KoreanCulture = new("ko-KR");

        public static string TimeString(DateTime dt)
        {
            return dt.ToString("tt h:mm", KoreanCulture);
        }

        public static string DateString(DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd", KoreanCulture);
        }
    }
}
