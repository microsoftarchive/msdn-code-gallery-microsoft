using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//Used for the ClientContext class
using Microsoft.SharePoint.Client;
using System.Net;
using System.Xml;

namespace SPO_AccessAWebService
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //This example extends the SPO_AuthenticateUsingCSOM sample
            //After authenticating, this example connects to the Lists web
            //service and gets the items in the Documents library, then
            //lists them in the console window
            
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
            //Next, get the authentication cookie so we can use it to connect to the lists web service
            Console.WriteLine();
            CookieCollection authCookie = ClaimsClientContext.GetAuthenticatedCookies(siteURL, 700, 500);
            //listsWS is a Service Reference to the lists web service
            //In Solution Explorer edit this reference to point to an instance
            //of the service. This could be a local instance of SharePoint, rather
            //than SharePoint Online as it is just used to create a proxy class
            listsWS.Lists list = new listsWS.Lists();
            //Edit this URL to point to your SharePoint Online site
            list.Url = "http://www.contoso.com/teamsite/_vti_bin/Lists.asmx";
            list.CookieContainer = new CookieContainer();
            list.CookieContainer.Add(authCookie);
            //Next we create the query to list items in the Documents library
            //Edit this variable to be the name of the list you are interested in
            string listName = "Documents";
            //Put the view name here. In this case its the default view
            string viewName = "";
            //A good idea to limit the number of results
            string rowLimit = "5";
            //These are the XML elements passed to the query
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement query = xmlDoc.CreateElement("Query");
            XmlElement viewFields = xmlDoc.CreateElement("ViewFields");
            XmlElement queryOptions = xmlDoc.CreateElement("QueryOptions");
            //Add CAML markup to the query elements
            query.InnerXml = "<Where><Gt><FieldRef Name=\"ID\" />" +
                "<Value Type=\"Counter\">0</Value></Gt></Where>";
            viewFields.InnerXml = "<FieldRef Name =\"Title\" />";
            queryOptions.InnerXml = "<IncludeMandatoryColumns>FALSE</IncludeMandatoryColumns>" +
                "<DateInUtc>TRUE</DateInUtc>";
            //Run the query
            XmlNode nodes = list.GetListItems(listName,
                viewName,
                query,
                viewFields,
                rowLimit,
                null,
                string.Empty);
            string ixml = list.GetList(listName).InnerXml;
            Console.WriteLine("Retreiving Items...");
            //Loop through the results
            foreach (XmlNode node in nodes)
            {
                if (node.Name == "rs:data")
                {
                    for (int i = 0; i < node.ChildNodes.Count; i++)
                    {
                        if (node.ChildNodes[i].Name == "z:row")
                        {
                            //This is a node that corresponds to an item in the list
                            //Tell the user its Title
                            Console.WriteLine(node.ChildNodes[i].Attributes["ows_Title"].Value);
                        }
                    }
                }
            }
            //This line prevents the console disappearing before you can read the result
            //Alternatively, remove this line a run the project without debugging (CTRL-F5)
            Console.ReadKey();
        }
    }
}
