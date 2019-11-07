using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Office.RecordsManagement.PolicyFeatures;
using Microsoft.SharePoint;

namespace RM_CustomExpirationAction
{
    /// <summary>
    /// An Information Management Policy is a set of rules that govern how a certain content
    /// type in a certain list or library behaves. Out of the box, a policy can have up 
    /// to four policy items in it. These define 1. how user actions are audited, 2. how 
    /// items are labelled, 3. how items get barcodes for id purposes, 4. How long items
    /// are retained and what happens when they expire. Each of these 4 is called a Policy
    /// Feature and you can define extra policy features in code. 
    /// For the retention and expiration policy feature, you can define a custom Expiration 
    /// Action. There are 8 out-of-the-box Expiration Actions, such as Permanently Delete 
    /// and Declare Record. This code sample creates and deploys a custom expiration action.
    /// </summary>
    /// <remarks>
    /// To create and deploy a custom expiration action you must define a class that 
    /// implements the IExpirationAction interface and its OnExpiration method. Then use
    /// a feature receiver to install the action. 
    /// </remarks>
    class ExampleExpirationAction : IExpirationAction
    {
        public void OnExpiration(SPListItem item, XmlNode parametersData, DateTime expiredDate)
        {
            //As an example, we will simple create an item in the Announcements list
            //to point out that an item is about to expire
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPWeb currentWeb = item.Web)
                {
                    SPList announcementsList = currentWeb.Lists["Announcements"];
                    SPListItem newAnnouncement = announcementsList.Items.Add();
                    newAnnouncement["Title"] = "An item has expired";
                    newAnnouncement["Body"] = "Item is at: " + item.Url.ToString();
                    newAnnouncement.Update();
                }
            });
        }
    }
}
