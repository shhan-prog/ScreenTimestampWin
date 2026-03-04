using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using ScreenTimestampWin.Capture;
using Application = System.Windows.Application;

namespace ScreenTimestampWin
{
    public partial class App : Application
    {
        private NotifyIcon? _trayIcon;
        private HotKeyManager? _hotKeyManager;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            SetupTrayIcon();
            SetupHotKey();
        }

        private void SetupTrayIcon()
        {
            _trayIcon = new NotifyIcon
            {
                Icon = CreateDefaultIcon(),
                Text = "ScreenTimestamp",
                Visible = true
            };

            var menu = new ContextMenuStrip();
            menu.Items.Add("영역 캡처", null, (_, _) => ScreenCaptureManager.BeginCapture());
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("종료", null, (_, _) => Shutdown());

            _trayIcon.ContextMenuStrip = menu;
            _trayIcon.DoubleClick += (_, _) => ScreenCaptureManager.BeginCapture();
        }

        private void SetupHotKey()
        {
            _hotKeyManager = new HotKeyManager();
            _hotKeyManager.Register();
        }

        private static Icon CreateDefaultIcon()
        {
            var bmp = new Bitmap(16, 16);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.FromArgb(28, 28, 30));
                using var font = new Font("Segoe UI", 7f, System.Drawing.FontStyle.Bold);
                g.DrawString("ST", font, Brushes.White, -1, 1);
            }
            var handle = bmp.GetHicon();
            return Icon.FromHandle(handle);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _hotKeyManager?.Unregister();
            if (_trayIcon != null)
            {
                _trayIcon.Visible = false;
                _trayIcon.Dispose();
            }
            base.OnExit(e);
        }
    }
}
