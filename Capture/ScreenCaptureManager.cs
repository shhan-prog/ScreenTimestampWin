using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScreenTimestampWin.Compositing;
using ScreenTimestampWin.Output;
using Application = System.Windows.Application;

namespace ScreenTimestampWin.Capture
{
    public static class ScreenCaptureManager
    {
        private static bool _isCapturing;

        public static void BeginCapture()
        {
            if (_isCapturing) return;
            _isCapturing = true;

            Application.Current.Dispatcher.Invoke(() =>
            {
                // 윈도우 위치/크기는 OverlayWindow.OnSourceInitialized에서
                // 물리 픽셀 단위로 가상 스크린 전체에 정확히 맞춰짐
                var overlay = new OverlayWindow();

                overlay.Closed += async (_, _) =>
                {
                    if (overlay.Cancelled)
                    {
                        _isCapturing = false;
                        return;
                    }

                    var selectedRect = overlay.SelectedRect;
                    var dpi = overlay.Dpi;
                    var vs = SystemInformation.VirtualScreen;

                    // selectedRect는 WPF 윈도우 내 DIP → 물리 픽셀로 변환
                    // 윈도우 좌상단(DIP 0,0)이 가상 스크린의 (vs.Left, vs.Top) 물리 픽셀에 위치
                    var screenX = vs.Left + (int)(selectedRect.X * dpi.DpiScaleX);
                    var screenY = vs.Top + (int)(selectedRect.Y * dpi.DpiScaleY);
                    var width = (int)(selectedRect.Width * dpi.DpiScaleX);
                    var height = (int)(selectedRect.Height * dpi.DpiScaleY);

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
