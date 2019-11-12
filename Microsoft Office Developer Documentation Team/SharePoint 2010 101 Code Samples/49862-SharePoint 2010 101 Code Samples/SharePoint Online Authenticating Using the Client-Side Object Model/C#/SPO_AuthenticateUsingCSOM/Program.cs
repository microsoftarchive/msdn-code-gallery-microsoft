using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//Used for the ClientContext class
using Microsoft.SharePoint.Client;

namespace SPO_AuthenticateUsingCSOM
{
    class Program
    {
        
        [STAThread]
        static void Main(string[] args)
        {
            //This example is based on an MSDN Code Sample by Robert Bogue
            //You can download the original sample here: http://code.msdn.microsoft.com/Remote-Authentication-in-b7b6f43c
            //A full discussion of the coding techniques is here: http://msdn.microsoft.com/en-us/library/hh147177.aspx

            //Adjust this string to point to your site on Office 365
            string siteURL = "http://www.contoso.com/teamsite";
            Console.WriteLine("Opening Site: " + siteURL);

            //Call the ClaimClientContext class to do claims mode authentication
            using (ClientContext context = ClaimsClientContext.GetAuthenticatedContext(siteURL))
            {
                if (context != null)
                {
                    //We have the client context object so claims-based authentication is complete
                    //Find out about the SP.Web object
                    context.Load(context.Web); 
                    context.ExecuteQuery(); 
                    //Display the name of the SharePoint site 
                    Console.WriteLine(context.Web.Title);
                }
            }
            //This line prevents the console disappearing before you can read the result
            //Alternatively, remove this line a run the project without debugging (CTRL-F5)
            Console.ReadKey();
        }
    }
}
