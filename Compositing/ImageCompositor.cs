using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ScreenTimestampWin.Compositing
{
    public static class ImageCompositor
    {
        // 출력 크기: 가로 4cm x 세로 8cm (300 DPI)
        private static readonly int OutputWidth = (int)(4.0 / 2.54 * 300);   // 472px
        private static readonly int OutputHeight = (int)(8.0 / 2.54 * 300);  // 945px
        // 시간영역 : 캡처영역 = 1 : 19
        private static readonly int BarHeight = OutputHeight / 20;

        public static Bitmap Composite(Bitmap capture, Bitmap timestampBar)
        {
            int captureH = OutputHeight - BarHeight;
            var result = new Bitmap(OutputWidth, OutputHeight);
            result.SetResolution(300, 300);

            using var g = Graphics.FromImage(result);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;

            // 배경을 타임스탬프 바 색상으로 채워 경계 틈 방지
            using var bgBrush = new SolidBrush(Color.FromArgb(255, 28, 28, 30));
            g.FillRectangle(bgBrush, 0, 0, OutputWidth, OutputHeight);

            // 캡처 이미지 → 상단
            g.DrawImage(capture, new Rectangle(0, 0, OutputWidth, captureH));

            // 타임스탬프 바 → 하단
            g.DrawImage(timestampBar, new Rectangle(0, captureH, OutputWidth, BarHeight));

            return result;
        }
    }
}
