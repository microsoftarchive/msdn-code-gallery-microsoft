using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CSFileIOWithDBASPNETCore.Models;
using System.IO;
using System.Drawing;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CSFileIOWithDBASPNETCore.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            using (ImageDBContext dbContext = new ImageDBContext())
            {
                List<Guid> iamgeIds = dbContext.Images.Select(m => m.Id).ToList();
                return View(iamgeIds);
            }
        }

        [HttpPost]
        public IActionResult UploadImage(IList<IFormFile> files)
        {
            IFormFile uploadedImage = files.FirstOrDefault();
            if (uploadedImage == null || uploadedImage.ContentType.ToLower().StartsWith("image/"))
            {
                using (ImageDBContext dbContext = new ImageDBContext())
                {
                    MemoryStream ms = new MemoryStream();
                    uploadedImage.OpenReadStream().CopyTo(ms);

                    System.Drawing.Image image = System.Drawing.Image.FromStream(ms);

                    Models.Image imageEntity = new Models.Image()
                    {
                        Id = Guid.NewGuid(),
                        Name = uploadedImage.Name,
                        Data = ms.ToArray(),
                        Width = image.Width,
                        Height = image.Height,
                        ContentType = uploadedImage.ContentType
                    };

                    dbContext.Images.Add(imageEntity);

                    dbContext.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public FileStreamResult ViewImage(Guid id)
        {
            using (ImageDBContext dbContext = new ImageDBContext())
            {
                Models.Image image = dbContext.Images.FirstOrDefault(m => m.Id == id);

                MemoryStream ms = new MemoryStream(image.Data);

                return new FileStreamResult(ms, image.ContentType);
            }
        }
    }
}
