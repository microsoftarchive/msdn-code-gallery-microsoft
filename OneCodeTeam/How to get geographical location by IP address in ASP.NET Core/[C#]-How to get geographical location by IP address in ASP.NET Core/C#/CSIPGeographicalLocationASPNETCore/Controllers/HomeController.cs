using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CSIPGeographicalLocationASPNETCore.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index(string ip)
        {
            if (string.IsNullOrEmpty(ip))
            {
                ip = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            }

            IPGeographicalLocation model = await IPGeographicalLocation.QueryGeographicalLocationAsync(ip);

            return View(model);
        }
    }
}
