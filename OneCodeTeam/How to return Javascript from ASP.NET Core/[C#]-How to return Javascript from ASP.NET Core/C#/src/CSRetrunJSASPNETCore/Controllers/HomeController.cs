using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

namespace CSRetrunJSASPNETCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public HomeController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public JavaScriptResult GetJS()
        {
            string webRootPath = _hostingEnvironment.WebRootPath;

            string result = System.IO.File.ReadAllText(webRootPath + "/JavaScript.js");
            result += System.IO.File.ReadAllText(webRootPath + "/JavaScript1.js");

            return new JavaScriptResult(result);
        }
    }

    public class JavaScriptResult : ContentResult
    {
        public JavaScriptResult(string script)
        {
            this.Content = script;
            this.ContentType = "application/javascript";
        }
    }
}
