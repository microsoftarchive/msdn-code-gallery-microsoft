using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;

namespace WCM_StarterMasterPage.Features.StarterMasterPageFeature
{
    /// <summary>
    /// This feature receiver sets the master page of the current site to be the
    /// master page deployed by the StarterMasterPageModule.
    /// </summary>

    [Guid("ae8d05aa-da52-4a45-8915-b0b79c4f0d66")]
    public class StarterMasterPageFeatureEventReceiver : SPFeatureReceiver
    {

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            //Get the current web site, ensuring proper disposal
            using (SPWeb currentWeb = (SPWeb)properties.Feature.Parent)
            {
                //Set the master page values and update the SPWeb
                currentWeb.MasterUrl = "/_catalogs/masterpage/_starter_publishing.master";
                currentWeb.CustomMasterUrl = "/_catalogs/masterpage/_starter_publishing.master";
                currentWeb.Update();
            }
        }

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
