using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;

namespace CSAngular2RoutingInAspNetCore
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            /*you need choose one of these two case in your web app************************************************/
            #region when static file case
            app.Use(async (context, next) =>
                {
                    await next();
                    if (context.Response.StatusCode == 404 && !Path.HasExtension(context.Request.Path.Value))
                    {
                        context.Request.Path = "/index.html";
                        await next();
                    }
                })
                .UseDefaultFiles(new DefaultFilesOptions { DefaultFileNames = new List<string> { "index.html" } })
                .UseStaticFiles()
                .UseMvc();
            #endregion

            #region when mvc case
            //app.UseStaticFiles()
            //   .UseMvc(routes =>
            //   {
            //       routes.MapRoute(
            //           name: "default",
            //           template: "{controller=Home}/{action=Index}");
            //       routes.MapSpaFallbackRoute("spa-fallback", new { controller = "Home", action = "Index" });
            //   });
            #endregion
        }
    }
}
