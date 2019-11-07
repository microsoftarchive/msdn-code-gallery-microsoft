using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebRoleAPI.Models;

namespace WebRoleAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Task.Run(() => {
                // initialization
                string outPutFileName = string.Format("{1}\\initial-Log-{0}.txt", DateTime.UtcNow.ToString("yyyyMMdd-HHmmss"), RoleEnvironment.GetLocalResource("LocalStorage1").RootPath);

                using (StreamWriter file = new StreamWriter(outPutFileName))
                {

                    string initStart = "{0}: Role initialization starts";
                    file.WriteLine(initStart, DateTime.UtcNow.ToString("HHmmss"));
                }

                System.Threading.Thread.Sleep(300000);

                InitializedModel.Initialization();

                using (StreamWriter file = new StreamWriter(outPutFileName, true))
                {
                    string initEnd = "{0}: Role initialization completed";

                    file.WriteLine(initEnd, DateTime.UtcNow.ToString("HHmmss"));
                }
            });
        }
    }
}
