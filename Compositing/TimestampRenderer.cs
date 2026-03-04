using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using ScreenTimestampWin.Utilities;

namespace ScreenTimestampWin.Compositing
{
    public static class TimestampRenderer
    {
        private const int RenderWidth = 1600;
        private const int RenderHeight = 200;

        public static Bitmap Render()
        {
            var bitmap = new Bitmap(RenderWidth, RenderHeight);
            bitmap.SetResolution(300, 300);

            using var g = Graphics.FromImage(bitmap);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            // 다크 배경
            using var bgBrush = new SolidBrush(Color.FromArgb(255, 28, 28, 30));
            g.FillRectangle(bgBrush, 0, 0, RenderWidth, RenderHeight);

            var now = DateTime.Now;
            var timeStr = DateFormatterKorean.TimeString(now);
            var dateStr = DateFormatterKorean.DateString(now);

            using var font = new Font("Segoe UI", 48f, FontStyle.Regular, GraphicsUnit.Point);
            using var textBrush = new SolidBrush(Color.White);

            var timeSize = g.MeasureString(timeStr, font);
            var dateSize = g.MeasureString(dateStr, font);

            const float rightPadding = 16f;
            const float lineSpacing = 0f;

            float totalTextHeight = timeSize.Height + lineSpacing + dateSize.Height;

            // 두 텍스트 중 넓은 쪽 기준으로 공통 중심 X 계산
            float maxWidth = Math.Max(timeSize.Width, dateSize.Width);
            float centerX = RenderWidth - rightPadding - maxWidth / 2;

            float timeX = centerX - timeSize.Width / 2;
            float dateX = centerX - dateSize.Width / 2;

            float baseY = (RenderHeight - totalTextHeight) / 2;
            float timeY = baseY;
            float dateY = baseY + timeSize.Height + lineSpacing;

            g.DrawString(timeStr, font, textBrush, timeX, timeY);
            g.DrawString(dateStr, font, textBrush, dateX, dateY);

            return bitmap;
        }
    }
}
