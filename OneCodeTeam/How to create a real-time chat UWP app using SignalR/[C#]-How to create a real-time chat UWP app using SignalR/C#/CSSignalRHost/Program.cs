using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;
using System;

namespace CSSignalRHost
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "http://127.0.0.1:8080";
            using (WebApp.Start(url))
            {
                Console.WriteLine($"running... on {url}");
                Console.ReadLine();
            }
        }
    }
    
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
    }
}
