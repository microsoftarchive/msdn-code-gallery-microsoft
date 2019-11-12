using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;

namespace SetBingMapsKeys
{
    class Program
    {
        static void Main(string[] args)
        {
            SetBingMapsKey();
            Console.WriteLine("Bing Maps set successfully");

        }
        /// <summary>
        /// SetBingMapsKey() will will set Bing Maps key at web level
        /// </summary>
        
        static private void SetBingMapsKey()
        {
            // change "http:// localhost" with your sharepoint server name
            ClientContext context = new ClientContext("http://localhost");
            Web web = context.Web;
            // Replace Bing maps key with valid Key
            web.AllProperties["BING_MAPS_KEY"] = "<Valid Bing Maps Key>";
            web.Update();
            context.ExecuteQuery();
        }    

    }
}
