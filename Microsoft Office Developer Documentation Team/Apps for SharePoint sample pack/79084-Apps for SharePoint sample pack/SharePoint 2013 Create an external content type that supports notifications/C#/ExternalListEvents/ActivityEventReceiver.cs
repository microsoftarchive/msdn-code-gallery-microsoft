using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace ExternalListEvents
{
    public class ActivityEventReceiver: SPItemEventReceiver
    {
        public override void ItemAdded(SPItemEventProperties properties)
        {
            SPList activities = properties.Web.Lists["Activities"];
            SPListItem task = activities.Items.Add();
            task["Title"] = "Follow up";
            task["Message"] = Encoding.UTF8.GetString(properties.ExternalNotificationMessage.ToArray());
            task.Update();
        }
    }
}
