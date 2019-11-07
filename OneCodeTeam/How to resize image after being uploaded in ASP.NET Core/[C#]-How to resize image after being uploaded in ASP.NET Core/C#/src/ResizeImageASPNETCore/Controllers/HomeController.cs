using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Drawing;
using System.IO;

namespace ResizeImageASPNETCore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public FileStreamResult Index(IList<IFormFile> files)
        {
            using (Image img = Image.FromStream(files[0].OpenReadStream()))
            {
                Stream ms = new MemoryStream(img.Resize(100, 100).ToByteArray());
                
                return new FileStreamResult(ms, "image/jpg");
            }
        }
    }
}
