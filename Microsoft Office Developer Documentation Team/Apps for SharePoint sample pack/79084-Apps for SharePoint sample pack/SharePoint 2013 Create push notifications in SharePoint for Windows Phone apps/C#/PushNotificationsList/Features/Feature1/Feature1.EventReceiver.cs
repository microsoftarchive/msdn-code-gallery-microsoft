using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;

namespace PushNotificationsList.Features.Feature1
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("6fdb7a8a-a8d4-40cd-a79d-f6c4977fe56a")]
    public class Feature1EventReceiver : SPFeatureReceiver
    {
        internal const string PushNotificationFeatureId = "41E1D4BF-B1A2-47F7-AB80-D5D6CBBA3092";

        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            base.FeatureActivated(properties);
            SPWeb spWeb = (SPWeb)properties.Feature.Parent;

            ListCreator listCreator = new ListCreator();
            listCreator.CreateJobsList(spWeb);
            listCreator.CreateNotificationResultsList(spWeb);

            // Then activate the Push Notification Feature on the server.
            // The Push Notification Feature is not activated by default in a SharePoint Server installation.
            spWeb.Features.Add(new Guid(PushNotificationFeatureId), false);
        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {            
            SPWeb spWeb = (SPWeb)properties.Feature.Parent;
            spWeb.Features.Remove(new Guid(PushNotificationFeatureId), false);
            base.FeatureDeactivating(properties);
        }


        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
