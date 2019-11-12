using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;

namespace CLIENTSIDE_HTML5AudioObject.Features.HTML5AudioDemoFeature
{

    [Guid("9c859257-4ec5-47a9-bc21-5880a74fd849")]
    public class HTML5AudioDemoFeatureEventReceiver : SPFeatureReceiver
    {

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            //Get the current site collection and web site, ensuring proper disposal
            using (SPSite currentSite = (SPSite)properties.Feature.Parent)
            {
                using (SPWeb currentWeb = currentSite.RootWeb)
                {
                    //Set the master page values and update the SPWeb
                    currentWeb.MasterUrl = "/_catalogs/masterpage/HTML5.v4.master";
                    currentWeb.CustomMasterUrl = "/_catalogs/masterpage/HTML5.v4.master";
                    currentWeb.Update();
                }
            }
        }



        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            //We must set the master page back to the default: v4.master
            using (SPSite currentSite = (SPSite)properties.Feature.Parent)
            {
                using (SPWeb currentWeb = currentSite.RootWeb)
                {
                    currentWeb.MasterUrl = "/_catalogs/masterpage/v4.master";
                    currentWeb.CustomMasterUrl = "/_catalogs/masterpage/v4.master";
                    currentWeb.Update();
                }
            }
        }


    }
}
