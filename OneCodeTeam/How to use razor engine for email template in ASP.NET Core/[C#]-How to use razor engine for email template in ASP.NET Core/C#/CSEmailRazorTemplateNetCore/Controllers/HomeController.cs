using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RazorLight;
using System.IO;
using CSEmailRazorTemplateNetCore.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CSEmailRazorTemplateNetCore.Controllers
{
    public class HomeController : Controller
    {
        public string Index()
        {
            string templatePath = $@"{Directory.GetCurrentDirectory()}\EmailTemplates";

            IRazorLightEngine engine = EngineFactory.CreatePhysical(templatePath);

            var model = new Notification
            {
                Name = "Jone",
                Title = "Test Email",
                Content = "This is a test"
            };

            string result = engine.Parse("template.cshtml", model);

            return result;
        }
    }
}
