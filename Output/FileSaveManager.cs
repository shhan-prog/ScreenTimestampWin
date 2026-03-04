using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ScreenTimestampWin.Output
{
    public static class FileSaveManager
    {
        public static void Save(Bitmap image)
        {
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HHmmss");
            var fileName = $"ScreenTimestamp_{timestamp}.png";
            var filePath = Path.Combine(desktopPath, fileName);

            image.Save(filePath, ImageFormat.Png);
        }
    }
}
