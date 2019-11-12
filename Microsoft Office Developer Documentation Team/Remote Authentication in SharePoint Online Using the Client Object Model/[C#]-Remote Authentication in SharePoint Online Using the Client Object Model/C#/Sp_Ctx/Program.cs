using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using System.Net;
using MSDN.Samples.ClaimsAuth;

namespace Sp_Ctx
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length < 1) { Console.WriteLine("SP_Ctx <url>"); return; }

            string targetSite = args[0];
            using (ClientContext ctx = ClaimClientContext.GetAuthenticatedContext(targetSite))
            {
                if (ctx != null)
                {
                    ctx.Load(ctx.Web); // Query for Web
                    ctx.ExecuteQuery(); // Execute
                    Console.WriteLine(ctx.Web.Title);
                }
            }
            Console.ReadLine();
        }
    }
}
