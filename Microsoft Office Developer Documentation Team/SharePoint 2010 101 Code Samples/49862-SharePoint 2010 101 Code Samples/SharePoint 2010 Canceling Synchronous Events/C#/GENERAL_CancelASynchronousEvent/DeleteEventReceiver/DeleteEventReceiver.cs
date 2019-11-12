using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;

namespace GENERAL_CancelASynchronousEvent.DeleteEventReceiver
{
    /// <summary>
    /// This example cancels the synchronous ItemDeleting event and logs
    /// it's actions to the Tasks list.
    /// </summary>
    public class DeleteEventReceiver : SPItemEventReceiver
    {
       /// <summary>
       /// An item is being deleted.
       /// </summary>
       public override void ItemDeleting(SPItemEventProperties properties)
       {
           //Prevent the item being deleted by cancelling the event
           properties.Cancel = true;
           properties.ErrorMessage = "You cannot delete any items from this list";
           //Get the item
           SPListItem updatedItem = properties.ListItem;
           using (SPWeb currentWeb = properties.Web)
           {
               //Get the Tasks list in the same SPWeb
               SPList tasksList = currentWeb.Lists["Tasks"];
               //Create an item to log the update
               SPListItem newTask = tasksList.Items.Add();
               newTask["Title"] = "Educate User";
               newTask["Body"] = "The following user tried to delete an announcement: " + properties.UserDisplayName;
               newTask.Update();
           } 
       }


    }
}
