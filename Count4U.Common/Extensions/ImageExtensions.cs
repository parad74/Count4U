using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace Count4U.Common.Extensions
{
    public static class ImageExtensions
    {
        public static BitmapImage ToBitmapImage(this Icon ico)
        {
            Bitmap bitmap = ico.ToBitmap();

            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            ms.Position = 0;
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();

            return bi;
        }
    }
}