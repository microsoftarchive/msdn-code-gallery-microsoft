using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;

namespace WCM_StyledMasterPage.Features.StyledMasterPageFeature
{

    [Guid("cd2bfff3-6b17-4ebf-a84c-e02ddc3170da")]
    public class StyledMasterPageFeatureEventReceiver : SPFeatureReceiver
    {
        //When the feature is activated set the default and custom master page
        //properties to the new master page.
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            //Get the current web site, ensuring proper disposal
            using (SPWeb currentWeb = (SPWeb)properties.Feature.Parent)
            {
                //Set the master page values and update the SPWeb
                currentWeb.MasterUrl = "/_catalogs/masterpage/StyledExample.master";
                currentWeb.CustomMasterUrl = "/_catalogs/masterpage/StyledExample.master";
                currentWeb.Update();
            }
        }

        //When the feature is deactivated clean up by setting the master page back to v4.master
        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            //We must set the master page back to the default: v4.master
            using (SPWeb currentWeb = (SPWeb)properties.Feature.Parent)
            {
                currentWeb.MasterUrl = "/_catalogs/masterpage/v4.master";
                currentWeb.CustomMasterUrl = "/_catalogs/masterpage/v4.master";
                currentWeb.Update();
            }
        }

    }
}
