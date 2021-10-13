using System;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Hosting;

namespace ShoppingListWeb.Models
{
    public static class PictureConverter
    {
        public static Image Base64ToImage(string imageBase64)
        {
            if(string.IsNullOrEmpty(imageBase64))
                return null;

            byte[] bytes = Convert.FromBase64String(imageBase64);
            Image image;

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
            }

            return image;
        }

        public static string PostedImageToBase64(HttpPostedFileBase file)
        {
            if (file is null)
                return string.Empty;

            string imageBase64 = string.Empty;
            string picturePath = Path.GetFileName(file.FileName);
            string pictureFullPath = Path.Combine(HostingEnvironment.MapPath("~/images"), picturePath);
            file.SaveAs(pictureFullPath);

            using (Image image = Image.FromFile(pictureFullPath))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    image.Save(memoryStream, image.RawFormat);
                    byte[] imageBytes = memoryStream.ToArray();

                    imageBase64 = Convert.ToBase64String(imageBytes);
                }
            }

            return imageBase64;
        }

    }
}