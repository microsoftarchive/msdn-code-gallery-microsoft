using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;

namespace GENERAL_EventReceiver.AnnouncementEventReceiver
{
    /// <summary>
    /// List Item Events. If you add other event handlers to this receiver, ensure
    /// that you also add corresponding Receiver elements to Elements.xml
    /// </summary>
    public class AnnouncementEventReceiver : SPItemEventReceiver
    {

       /// <summary>
       /// This event fires after an item is added. It's a good place to modify the item.
       /// This is an asynchronous event
       /// </summary>
       public override void ItemAdded(SPItemEventProperties properties)
       {
           //Get the item
           SPListItem addedItem = properties.ListItem;
           //Add a creation date
           addedItem["Body"] += "This item was added on: " + DateTime.Today;
           addedItem.Update();
       }

       /// <summary>
       /// This event fires before an item is modified.
       /// This is a synchronous event.
       /// </summary>
       public override void ItemUpdating(SPItemEventProperties properties)
       {
           //Get the item and the parent list
           SPListItem updatedItem = properties.ListItem;
           using (SPWeb currentWeb = properties.Web)
           {
               //Get the Tasks list in the same SPWeb
               SPList tasksList = currentWeb.Lists["Tasks"];
               //Create an item to log the update
               SPListItem newAnnouncement = tasksList.Items.Add();
               newAnnouncement["Title"] = "Review an updated item";
               newAnnouncement["Body"] = "Please review an announcement modification. The updated item's title was: " + updatedItem.Title;
               newAnnouncement.Update();
           }
       }

    }
}
