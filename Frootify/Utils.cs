using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace Frootify
{
    public class Utils
    {
        public static string Encode(string str)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
        }

        public static string Decode(string encodedString)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(encodedString));
        }

        [Obsolete]
        public static (bool, string) UploadImage(string filePath)
        {
            try
            {
                var myAccount = new Account { ApiKey = Decode("NDkxMjU2MTYzODg1NjI3"), ApiSecret = Decode("MktlUUhwck9ydi1kZm1tM1VNcVVPS0V1bzV3"), Cloud = Decode("ZGNvZ2Rra3dh") };

                Cloudinary cloudinary = new Cloudinary(myAccount);

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(filePath)
                };
                var uploadResult = cloudinary.Upload(uploadParams);

                return (true, uploadResult.SecureUri.AbsoluteUri);
            }
            catch
            {
                return (false, string.Empty);
            }
        }

        public static BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                memoryStream.Position = 0;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }
    }
}
