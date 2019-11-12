using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;

namespace CreateMapView
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateMapView();
            Console.WriteLine("Map View is created Successfully");
        }
        private static void CreateMapView()
        {
            // Replace site URL and List Title with Valid values.
            ClientContext context = new ClientContext("<Site Url>");
            List oList = context.Web.Lists.GetByTitle("<List Title>");

            ViewCreationInformation viewCreationinfo = new ViewCreationInformation();
            //Replace <View Name> with the name you want to give to your map view 
            viewCreationinfo.Title = "<View Name>";
            viewCreationinfo.ViewTypeKind = ViewType.Html;
            View oView = oList.Views.Add(viewCreationinfo);

            oView.JSLink = "mapviewtemplate.js";

            oView.Update();
            context.ExecuteQuery();
        }

    }
}
