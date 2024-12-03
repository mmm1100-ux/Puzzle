using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace Helper
{
    public static class UploadUtility
    {
        public static string UploadFile(this IFormFile formFile, int projectId, string fileType = null)
        {
            string fileName = Guid.NewGuid().ToString();

            if (fileType == null)
            {
                fileType = Path.GetExtension(formFile.FileName);
            }

            string path = "wwwroot/project/" + DateTime.Today.Year + DateTime.Today.Month.ToString("D2") + DateTime.Today.Day.ToString("D2") + "/" + projectId;

            Directory.CreateDirectory(path);
            path = path + "/" + fileName + fileType;

            using (var stream = new FileStream(path, FileMode.Create))
            {
                formFile.CopyTo(stream);
            }

            string uploadPath = path.Replace("wwwroot", null);

            return uploadPath;
        }

        public static string UploadFile(this IFormFile formFile, string path)
        {
            string fileName = Guid.NewGuid().ToString();

            var fileType = Path.GetExtension(formFile.FileName);

             path = "wwwroot/upload/" + path + "/";

            Directory.CreateDirectory(path);
            path = path + "/" + fileName + fileType;

            using (var stream = new FileStream(path, FileMode.Create))
            {
                formFile.CopyTo(stream);
            }

            string uploadPath = path.Replace("wwwroot", null);

            return uploadPath;
        }
    }
}
