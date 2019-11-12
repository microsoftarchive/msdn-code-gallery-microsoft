using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CSDetermineOSAndBrowserASPNETCore.Controllers
{
    public class HomeController : Controller
    {
        // GET: /<controller>/
        public ActionResult Index(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
            {
                userAgent = Request.Headers["User-Agent"];
            }

            ViewBag.userAgent = userAgent;

            UserAgent.UserAgent ua = new UserAgent.UserAgent(userAgent);

            var aa = ua.Browser;

            return View(ua);
        }
    }
}
