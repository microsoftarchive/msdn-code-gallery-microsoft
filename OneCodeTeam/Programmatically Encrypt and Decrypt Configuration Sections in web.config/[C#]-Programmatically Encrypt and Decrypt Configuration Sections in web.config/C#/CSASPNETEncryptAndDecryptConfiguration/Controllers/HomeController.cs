using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace CSASPNETEncryptAndDecryptConfiguration.Controllers
{
    public class HomeController : Controller
    {

        private const string provider = "RSAProtectedConfigurationProvider";  //Use RSA Provider to encrypt configuration sections
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EncryptConfig(string sectionName)
        {
            if (string.IsNullOrEmpty(sectionName))
            {
                return null;
            }

            Configuration config = WebConfigurationManager.OpenWebConfiguration(Request.ApplicationPath);
            ConfigurationSection section = config.GetSection(sectionName);
            section.SectionInformation.ProtectSection(provider);
            config.Save();
            return Content("Success");
        }

        public ActionResult DecryptConfig(string sectionName)
        {
            if (string.IsNullOrEmpty(sectionName))
            {
                return null;
            }
            Configuration config = WebConfigurationManager.OpenWebConfiguration(Request.ApplicationPath);
            ConfigurationSection section = config.GetSection(sectionName);
            if (section.SectionInformation.IsProtected)
            {
                section.SectionInformation.UnprotectSection();
                config.Save();
            }
            return Content("Success");
        }
    }
}