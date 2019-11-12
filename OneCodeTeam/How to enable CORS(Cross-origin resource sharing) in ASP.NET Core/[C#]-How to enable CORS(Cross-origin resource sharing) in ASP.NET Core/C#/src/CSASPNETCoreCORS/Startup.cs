using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CSASPNETCoreCORS
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            #region case 1:
            //services.AddCors();
            #endregion

            #region case 2,3:
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", builder =>
                {
                    builder.WithOrigins("http://localhost", "https://www.microsoft.com");
                });

                //options.AddPolicy("AllowAllOrigins", builder =>
                //{
                //    builder.AllowAnyOrigin();
                //    // or use below code
                //    //builder.WithOrigins("*");
                //});
            });
            #endregion

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            #region case 1:
            //app.UseCors(builder => builder.WithOrigins("http://localhost"));
            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
            #endregion

            #region case 2:
            //app.UseCors("AllowSpecificOrigin");
            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
            #endregion

            #region case 3:
            //see the controller attribute
            //like [EnableCors("AllowSpecificOrigin")]
            #endregion

            app.UseMvc();
        }
    }
}
