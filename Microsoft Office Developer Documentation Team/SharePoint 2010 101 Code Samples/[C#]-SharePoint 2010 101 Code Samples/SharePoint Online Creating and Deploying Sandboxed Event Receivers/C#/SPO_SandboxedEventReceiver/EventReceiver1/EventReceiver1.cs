using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;

namespace SPO_SandboxedEventReceiver.EventReceiver1
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class EventReceiver1 : SPItemEventReceiver
    {
       /// <summary>
       /// An item was added.
       /// </summary>
       public override void ItemAdded(SPItemEventProperties properties)
       {
           //This example illustrates a sandboxed event receiver
           //Because it works in the sandbox, you can use it in 
           //SharePoint Online by deploying its .wsp file to the 
           //solutions gallery. The event receiver modifies items
           //added to the Announcements list.
           
           //To test this solution before deployment, set the Site URL 
           //property of the project to match your test SharePoint farm, then
           //use F5

           //To deploy this project to your SharePoint Online site, upload
           //the SPO_SandboxedWebPart.wsp solution file from the bin/debug
           //folder to your solution gallery. Then activate the solution.

           //Get the item that was added
           SPListItem addedItem = properties.ListItem;
           if (checkSandbox())
           {
               //This event receiver is running in the sandbox
               addedItem["Body"] += "This item was modified by an event receiver running in the sandbox";
           }
           else
           {
               //This event receiver is running ouside the sandbox
               addedItem["Body"] += "This item was modified by an event receiver running outside the sandbox";
           }
           //Save the changes
           addedItem.Update();
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
