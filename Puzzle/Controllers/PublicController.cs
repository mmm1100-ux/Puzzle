using Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Puzzle.Data;
using System.Linq;
using Helper;
using System;
using Puzzle.Extensions;
using System.Text;
using System.Drawing.Drawing2D;
using Enums;

namespace Puzzle.Controllers
{
    public class PublicController : Controller
    {
        [Route("photo/{width}/{*url}")]
        [Route("picture/{*url}")]
        public IActionResult Photo(string url, int width, long quality = 75, ImageFormat format = null)
        {
            if (System.IO.File.Exists(Path.Combine("wwwroot", url.Trim('/').Trim('\\'))))
            {
                if (width > 2160)
                {
                    width = 2160;
                }

                using var image = Image.FromFile(Path.Combine("wwwroot", url.Trim('/').Trim('\\')));

                if (image.Width < width)
                {
                    width = image.Width;
                }

                var bitmap = new Bitmap(image, width, image.Height * width / image.Width);

                using var memoryStream = new MemoryStream();

                var myEncoderParameters = new EncoderParameters(1);
                var myImageCodecInfo = GetEncoderInfo(image.RawFormat);

                if (format != null)
                {
                    myImageCodecInfo = GetEncoderInfo(format);
                }

                var myEncoder = System.Drawing.Imaging.Encoder.Quality;
                var myEncoderParameter = new EncoderParameter(myEncoder, quality);
                myEncoderParameters.Param[0] = myEncoderParameter;

                bitmap.Save(memoryStream, myImageCodecInfo, myEncoderParameters);

                byte[] byteArray = memoryStream.ToArray();

                Response.Headers.Add("Cache-Control", "public, max-age=2592000");

                return File(byteArray, myImageCodecInfo.MimeType);
            }
            else
            {
                return NotFound();
            }
        }

        [Route("watermark/{width}/{*url}")]
        public IActionResult Watermark(string url, int width)
        {
            var extension = url.Split(".").Last();
            url = url.Replace("." + extension, null).Replace("plus", "+").Decrypt();

            if (System.IO.File.Exists(Path.Combine("wwwroot", url.Trim('/').Trim('\\'))))
            {
                var image = Image.FromFile(Path.Combine("wwwroot", url.Trim('/').Trim('\\')));
                var watermark = Image.FromFile("wwwroot/images/app/watermark.png");
                var watermarkWhite = Image.FromFile("wwwroot/images/app/watermark-white.png");

                if (width > 1440)
                {
                    width = 1440;
                }
                else if (image.Width < width)
                {
                    width = image.Width;
                }

                var height = width * image.Height / image.Width;

                var watermarkWidth = width / 4.0;
                var watermarkHeight = watermarkWidth * watermark.Height / watermark.Width;

                long quality = 100;

                if (width == 240)
                {
                    watermarkWidth = 0;
                    watermarkHeight = 0;

                    quality = 75;
                }

                var bitmap = new Bitmap(image, width, height);

                using var imageGraphics = Graphics.FromImage(bitmap);

                using var memoryStream = new MemoryStream();

                int offset = width / 20;

                imageGraphics.DrawImage(watermark, new Rectangle(new Point(offset, (height - offset - watermarkHeight.ToInt())), new Size(watermarkWidth.ToInt(), watermarkHeight.ToInt())));

                imageGraphics.DrawImage(watermarkWhite, new Rectangle(new Point((width - offset - watermarkWidth.ToInt()), offset), new Size(watermarkWidth.ToInt(), watermarkHeight.ToInt())));

                var myEncoderParameters = new EncoderParameters(1);
                //var myImageCodecInfo = GetEncoderInfo(image.RawFormat);
                var myImageCodecInfo = GetEncoderInfo(ImageFormat.Jpeg);
                var myEncoder = System.Drawing.Imaging.Encoder.Quality;
                var myEncoderParameter = new EncoderParameter(myEncoder, quality);
                myEncoderParameters.Param[0] = myEncoderParameter;

                bitmap.Save(memoryStream, myImageCodecInfo, myEncoderParameters);

                byte[] byteArray = memoryStream.ToArray();

                Response.Headers.Add("Cache-Control", "public, max-age=2592000");

                return File(byteArray, myImageCodecInfo.MimeType);
            }
            else
            {
                return NotFound();
            }
        }

        private static ImageCodecInfo GetEncoderInfo(ImageFormat imageFormat)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].FormatID == imageFormat.Guid)
                    return encoders[j];
            }
            return null;
        }

        [HttpPost("public/avatar")]
        [Authorize]
        public IActionResult Avatar(IFormFile image)
        {
            using var db = new PuzzleDbContext();

            var userId = User.GetUserId();

            var user = db.Users.Where(x => x.Id == userId).First();

            if (image != null)
            {
                user.Image = image.UploadFile("avatar");

                db.SaveChanges();
            }

            return Ok();
        }
    }
}
