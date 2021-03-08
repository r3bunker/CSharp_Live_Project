using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Drawing;
using TheatreCMS.Models;

namespace TheatreCMS.Controllers
{
    public class ImageUploadController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //file -> buyte[] (out string64)
        public static byte[] ImageBytes(HttpPostedFileBase file, out string imageBase64)
        {
            //Convert the file into a System.Drawing.Image type
            Image image = Image.FromStream(file.InputStream, true, true);
            //Convert that image into a Byte Array to facilitate storing the image in a database
            var converter = new ImageConverter();
            byte[] imageBytes = (byte[])converter.ConvertTo(image, typeof(byte[]));
            //Extract the String.Base64 representation of the image for inline, browser-side rendering during display
            imageBase64 = Convert.ToBase64String(imageBytes);
            //return Byte Array
            return imageBytes;
        }

        //byte[] -> smaller byte[]
        public static byte[] ImageThumbnail(byte[] imageBytes, int thumbWidth, int thumbHeight)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Image img = Image.FromStream(new MemoryStream(imageBytes));
                using (Image thumbnail = img.GetThumbnailImage(img.Width, img.Height, null, new IntPtr()))
                {
                    thumbnail.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }

        public static byte[] InsertPhoto(Image image)
        {
            var converter = new ImageConverter();
            byte[] imageBytes = (byte[])converter.ConvertTo(image, typeof(byte[]));
            return imageBytes;
        }

        public FileContentResult ImageView(int id, string table, int thumbWidth, int thumbHeight)
        {
            byte[] imgArray = null;
          
            //switch (table)
            //{
            //    case "Sponsor":
            //        var sponsor = db.Sponsors.Find(id);
            //        imgArray = sponsor.Logo;
            //        break;
            //    case "CastMembers":
            //        var castMembers = db.CastMembers.Find(id);
            //        imgArray = castMembers.Photo;
            //        break;
            //    case "DisplayInfo":
            //        var displayInfo = db.DisplayInfo.Find(id);
            //        imgArray = displayInfo.Image;
            //        break;
            //    case "Productions":
            //        var production = db.Productions.Find(id);
            //        imgArray = production.DefaultPhoto.PhotoId;
            //        break;
            //    case "ProductionPhotos":
            //        var productionPhotos = db.ProductionPhotos.Find(id);
            //        imgArray = productionPhotos.PhotoId;
            //        break;

            //    default:
            //        break;
            //}
            using (MemoryStream ms = new MemoryStream())
            using (Image thumbnail = Image.FromStream(new MemoryStream(imgArray)).GetThumbnailImage(thumbWidth, thumbHeight, null, new IntPtr()))
            {
                thumbnail.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.ToArray();
                //saves to db, but cannot plug into FileContentResult. Need diff way to combine methods.
            }
            //image not returning at input size, will need to fix later
            return new FileContentResult(imgArray, "image/jpg");
        }
    }
}