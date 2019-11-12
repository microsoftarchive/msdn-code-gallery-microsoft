using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;

namespace GENERAL_FeatureReceiver.Features.ExampleFeature
{
    /// <summary>
    /// This class, inheriting from SPFeatureReceiver, can include methods that execute
    /// when a feature is activated, deactivating, installed, and uninstalling. Commonly,
    /// code is run in the FeatureActivated handler to complete the set up of a feature.
    /// In FeatureDeactivating it is important to clean up any changes made in FeatureActivated.
    /// In Visual Studio, to add a Feature Receiver, right-click a Feature and then click 
    /// "Add Event Receiver". 
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("274de11a-b7c2-4b1d-9f6a-a83382aed926")]
    public class ExampleFeatureEventReceiver : SPFeatureReceiver
    {
        //This event executes just after the feature is activated
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            //This feature has the Web scope, so the properties.Feature.Parent property returns an SPWeb object
            SPWeb currentWeb = (SPWeb)properties.Feature.Parent;
            //NOTE: If the scope was Site, Parent would return a SPSite.
            //If the scope was Web Application, Parent would return a SPWebApplication.
            currentWeb.Description += " The Example Feature is activated!";
            currentWeb.Update();
        }

        //This event fires when a user deactivates the feature
        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            //In this hanlder you must make sure you clean up the changes you made in FeatureActivated
            SPWeb currentWeb = (SPWeb)properties.Feature.Parent;
            currentWeb.Description = currentWeb.Description.Replace(" The Example Feature is activated!", String.Empty);
            currentWeb.Update();
        }

    }
}
