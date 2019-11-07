using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint;


namespace AddGeolocationFieldSample
{
    class Program
    {    
        static void Main(string[] args)
        {
            AddGeolocationField();

            Console.WriteLine("Geolocation field added successfully!");
        }

        /// <summary>
        /// AddGeoLocationField will add a Location field to a sharepoint List
        /// </summary>
       
        private static void AddGeolocationField()
        {
            // change "http:// localhost" with your sharepoint server name
            ClientContext context = new ClientContext(@"http://localhost");
            //Replace the <List Title> with valid list name. 
            List oList = context.Web.Lists.GetByTitle("<List Title>");

            //DisplayName will be displayed as the name of the newly added Location field on the list
            oList.Fields.AddFieldAsXml("<Field Type='Geolocation' DisplayName='Location'/>"
                                        , true, AddFieldOptions.AddToAllContentTypes);
            oList.Update();
            context.ExecuteQuery();
        } 

    }
}
