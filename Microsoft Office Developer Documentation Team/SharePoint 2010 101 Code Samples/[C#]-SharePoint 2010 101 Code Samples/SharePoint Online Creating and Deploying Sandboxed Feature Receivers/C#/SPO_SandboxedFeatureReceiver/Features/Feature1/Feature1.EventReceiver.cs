using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;

namespace SPO_SandboxedFeatureReceiver.Features.Feature1
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("dbc6ac94-874b-443c-91d5-9f87af9e43fb")]
    public class Feature1EventReceiver : SPFeatureReceiver
    {
        
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            //This example illustrates a sandboxed feature receiver
            //Because it works in the sandbox, you can use it in 
            //SharePoint Online by deploying its .wsp file to the 
            //solutions gallery. The feature receiver modifies the
            //description of any site where it is activated.

            //To test this solution before deployment, set the Site URL 
            //property of the project to match your test SharePoint farm, then
            //use F5. Try deactivating the deployed feature - the extra
            //description should disappear from the site homepage

            //To deploy this project to your SharePoint Online site, upload
            //the SPO_SandboxedWebPart.wsp solution file from the bin/debug
            //folder to your solution gallery. Then activate the solution.

            //using keyword ensures proper disposal
            using (SPWeb currentWeb = (SPWeb)properties.Feature.Parent)
            {
                if (checkSandbox())
                {
                    //The feature receiver is Sandboxed. Set the description.
                    currentWeb.Description += " This description was set by a feature receiver running in the sandbox";
                }
                else
                {
                    //The feature receiver is not Sandboxed. Set the description.
                    currentWeb.Description += " This description was set by a feature receiver running outside the sandbox";
                }
                //Save the new description
                currentWeb.Update();
            }
        }
        
        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            //It's important that the Feature Deactivating event
            //reverses any changes made in Feature Activated so 
            //the web is cleaned up.
            using (SPWeb currentWeb = (SPWeb)properties.Feature.Parent)
            {
                if (checkSandbox())
                {
                    currentWeb.Description = currentWeb.Description.Replace(" This description was set by a feature receiver running in the sandbox", string.Empty);
                }
                else
                {
                    currentWeb.Description = currentWeb.Description.Replace(" This description was set by a feature receiver running outside the sandbox", string.Empty);
                }
                currentWeb.Update();
            }
        }

        private bool checkSandbox()
        {
            //This method returns true only if the code is running in the sandbox
            if (System.AppDomain.CurrentDomain.FriendlyName.Contains("Sandbox"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
