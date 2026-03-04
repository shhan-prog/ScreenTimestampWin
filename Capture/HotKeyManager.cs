using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace ScreenTimestampWin.Capture
{
    public class HotKeyManager
    {
        private const int WM_HOTKEY = 0x0312;
        private const int HOTKEY_ID = 1;
        private const uint MOD_CONTROL = 0x0002;
        private const uint MOD_SHIFT = 0x0004;
        private const uint VK_2 = 0x32;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private HwndSource? _source;
        private IntPtr _hwnd;

        public void Register()
        {
            // 메시지 전용 윈도우 생성
            var parameters = new HwndSourceParameters("ScreenTimestampHotKey")
            {
                Width = 0,
                Height = 0,
                WindowStyle = 0x800000 // WS_POPUP (not visible)
            };
            _source = new HwndSource(parameters);
            _source.AddHook(WndProc);
            _hwnd = _source.Handle;

            RegisterHotKey(_hwnd, HOTKEY_ID, MOD_CONTROL | MOD_SHIFT, VK_2);
        }

        public void Unregister()
        {
            if (_hwnd != IntPtr.Zero)
            {
                UnregisterHotKey(_hwnd, HOTKEY_ID);
            }
            _source?.Dispose();
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY && wParam.ToInt32() == HOTKEY_ID)
            {
                ScreenCaptureManager.BeginCapture();
                handled = true;
            }
            return IntPtr.Zero;
        }
    }
}
