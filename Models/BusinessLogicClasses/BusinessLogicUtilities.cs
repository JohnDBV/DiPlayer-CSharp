using DiPlayer_CSharp.Models.DataClasses;
using libZPlay;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace DiPlayer_CSharp.Models.BusinessLogicClasses
{
    public static class BusinessLogicUtilities
    {
        internal static bool IsPlaybackOnTheLastSecond(TStreamTime current, TStreamTime total)
        {
            if( (total.ms - current.ms) < 1000)
                return true;

            return false;
        }

        //internal static BitmapImage? CreateEmptyBitmapImage()
        //{
        //    //List<System.Windows.Media.Color> colors = new List<System.Windows.Media.Color>();
        //    //colors.Add(System.Windows.Media.Colors.Transparent);
        //    //BitmapPalette myPalette = new BitmapPalette(colors);

        //    //return BitmapSource.Create(1, 1, 96, 96, PixelFormats.BlackWhite, myPalette, new byte[] { 0 }, 1);

        //    //var bmptmp = BitmapSource.Create(1, 1, 96, 96, PixelFormats.Bgr24, null, new byte[3] { 0, 0, 0 }, 3);

        //    //return new TransformedBitmap(bmptmp, new ScaleTransform(1, 1));
        //    var tmp = BitmapImage.Create(1, 1, 1, 1, PixelFormats.BlackWhite, null, new byte[] { 0 }, 1);
        //    BitmapImage tmp234 = (BitmapImage)tmp;
        //    return BitmapSource.Create(1, 1, 1, 1, PixelFormats.BlackWhite, null, new byte[] { 0 }, 1) as BitmapImage;
        //}

        internal static BitmapImage? BitmapImageFromMemoryStream(MemoryStream bitStream)
        {
            BitmapImage bImg = new BitmapImage();
            bitStream.Position = 0;
            bImg.BeginInit();
            bImg.StreamSource = new MemoryStream(bitStream.ToArray());
            bImg.EndInit();
            bImg.Freeze();//let the object be shared across threads, after freezing it.

            bitStream.Close();

            return bImg;
        }


        public static BitmapImage CreateEmptyBitmapImage()
        {
            return Bitmap2BitmapImage(new Bitmap(1, 1));
        }
        public static BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                ms.Position = 0;
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();
                bi.Freeze();//let the object be shared across threads, after freezing it.

                return bi;
            }
        }
    }
}
