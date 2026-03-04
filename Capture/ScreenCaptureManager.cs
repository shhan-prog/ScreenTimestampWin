using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using ScreenTimestampWin.Compositing;
using ScreenTimestampWin.Output;
using Application = System.Windows.Application;

namespace ScreenTimestampWin.Capture
{
    public static class ScreenCaptureManager
    {
        private static bool _isCapturing;

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int index);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hdc);

        private const int DESKTOPHORZRES = 118;
        private const int DESKTOPVERTRES = 117;
        private const int HORZRES = 8;
        private const int VERTRES = 10;

        public static void BeginCapture()
        {
            if (_isCapturing) return;
            _isCapturing = true;

            Application.Current.Dispatcher.Invoke(() =>
            {
                // 가상 스크린 전체를 덮는 오버레이
                var overlay = new OverlayWindow
                {
                    Left = SystemInformation.VirtualScreen.Left,
                    Top = SystemInformation.VirtualScreen.Top,
                    Width = SystemInformation.VirtualScreen.Width,
                    Height = SystemInformation.VirtualScreen.Height
                };

                overlay.Closed += async (_, _) =>
                {
                    if (overlay.Cancelled)
                    {
                        _isCapturing = false;
                        return;
                    }

                    var selectedRect = overlay.SelectedRect;

                    // 오버레이 좌표 → 스크린 절대 좌표
                    var screenX = (int)(SystemInformation.VirtualScreen.Left + selectedRect.X);
                    var screenY = (int)(SystemInformation.VirtualScreen.Top + selectedRect.Y);
                    var width = (int)selectedRect.Width;
                    var height = (int)selectedRect.Height;

                    // DPI 스케일 보정
                    var dpiScale = GetDpiScale();
                    screenX = (int)(screenX * dpiScale);
                    screenY = (int)(screenY * dpiScale);
                    width = (int)(width * dpiScale);
                    height = (int)(height * dpiScale);

                    // 오버레이가 사라진 후 캡처 (200ms 대기)
                    await Task.Delay(200);
                    CaptureRegion(screenX, screenY, width, height);
                    _isCapturing = false;
                };

                overlay.Show();
                overlay.Activate();
                overlay.Focus();
            });
        }

        private static double GetDpiScale()
        {
            var hdc = GetDC(IntPtr.Zero);
            var physicalW = GetDeviceCaps(hdc, DESKTOPHORZRES);
            var logicalW = GetDeviceCaps(hdc, HORZRES);
            ReleaseDC(IntPtr.Zero, hdc);
            return (double)physicalW / logicalW;
        }

        private static void CaptureRegion(int x, int y, int width, int height)
        {
            if (width < 2 || height < 2) return;

            using var bitmap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(x, y, 0, 0, new System.Drawing.Size(width, height));
            }

            ProcessCapture(bitmap);
        }

        private static void ProcessCapture(Bitmap capture)
        {
            var timestampBar = TimestampRenderer.Render();
            var composited = ImageCompositor.Composite(capture, timestampBar);

            ClipboardManager.Copy(composited);
            FileSaveManager.Save(composited);

            System.Media.SystemSounds.Exclamation.Play();

            timestampBar.Dispose();
            composited.Dispose();
        }
    }
}
