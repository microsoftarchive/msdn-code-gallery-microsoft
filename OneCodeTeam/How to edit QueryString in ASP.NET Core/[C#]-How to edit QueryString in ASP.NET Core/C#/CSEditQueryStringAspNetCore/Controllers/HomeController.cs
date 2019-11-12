using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CSEditQueryStringAspNetCore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            string originalUrl = "https://somehost.org/somepath/somepage?someparam=test#soem fragment";

            var qpe = new QueryParameterEditor(originalUrl);

            qpe["page"] = "1";
            qpe.SetQueryParam("page", "2").RemoveQueryParam("someparam");

            ViewBag.OriginalUrl = originalUrl;
            ViewBag.EditedUrl = qpe.ToString();

            return View();
        }
    }
}