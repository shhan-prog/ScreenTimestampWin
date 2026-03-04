using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;

namespace ScreenTimestampWin.Output
{
    public static class ClipboardManager
    {
        public static void Copy(Bitmap image)
        {
            // PNG 데이터로 클립보드에 복사
            using var ms = new MemoryStream();
            image.Save(ms, ImageFormat.Png);

            var dataObject = new DataObject();
            dataObject.SetData("PNG", ms);
            dataObject.SetImage(ConvertToWpfImage(image));

            Clipboard.SetDataObject(dataObject, true);
        }

        private static System.Windows.Media.Imaging.BitmapSource ConvertToWpfImage(Bitmap bitmap)
        {
            var hBitmap = bitmap.GetHbitmap();
            try
            {
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    System.IntPtr.Zero,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(hBitmap);
            }
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern bool DeleteObject(System.IntPtr hObject);
    }
}
