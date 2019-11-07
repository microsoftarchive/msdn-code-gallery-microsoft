using System;
using System.Security.Permissions;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;

namespace GENERAL_LoggingSiteEvents.WebEventReceiver
{
    /// <summary>
    /// Web Events
    /// </summary>
    public class WebEventReceiver : SPWebEventReceiver
    {
       /// <summary>
       /// A site collection was deleted. All the event handlers simply 
       /// call the LogWebEventProperties method and pass the event properties
       /// </summary>
       public override void SiteDeleted(SPWebEventProperties properties)
       {
           this.LogWebEventProperties(properties);
       }

       /// <summary>
       /// A site was deleted.
       /// </summary>
       public override void WebDeleted(SPWebEventProperties properties)
       {
           this.LogWebEventProperties(properties);
       }

       /// <summary>
       /// A site was moved.
       /// </summary>
       public override void WebMoved(SPWebEventProperties properties)
       {
           this.LogWebEventProperties(properties);
       }

       /// <summary>
       /// A site was provisioned.
       /// </summary>
       public override void WebProvisioned(SPWebEventProperties properties)
       {
           this.LogWebEventProperties(properties);
       }


       private void LogWebEventProperties(SPWebEventProperties properties)
       {
           // Specify the log list name.
           string listName = "WebEventLogger";

           // Create string builder object.
           StringBuilder sb = new StringBuilder();

           // Add properties that do not throw exceptions.
           sb.AppendFormat("Cancel: {0}\n", properties.Cancel);
           sb.AppendFormat("ErrorMessage: {0}\n", properties.ErrorMessage);
           sb.AppendFormat("EventType: {0}\n", properties.EventType);
           sb.AppendFormat("FullUrl: {0}\n", properties.FullUrl);
           sb.AppendFormat("NewServerRelativeUrl: {0}\n", properties.NewServerRelativeUrl);
           sb.AppendFormat("ParentWebId: {0}\n", properties.ParentWebId);
           sb.AppendFormat("ReceiverData: {0}\n", properties.ReceiverData);
           sb.AppendFormat("RedirectUrl: {0}\n", properties.RedirectUrl);
           sb.AppendFormat("ServerRelativeUrl: {0}\n", properties.ServerRelativeUrl);
           sb.AppendFormat("SiteId: {0}\n", properties.SiteId);
           sb.AppendFormat("Status: {0}\n", properties.Status);
           sb.AppendFormat("UserDisplayName: {0}\n", properties.UserDisplayName);
           sb.AppendFormat("UserLoginName: {0}\n", properties.UserLoginName);
           sb.AppendFormat("WebId: {0}\n", properties.WebId);
           sb.AppendFormat("Web: {0}\n", properties.Web);

           // Log the event to the list.
           this.EventFiringEnabled = false;
           LogEvent(properties.Web, listName, properties.EventType, sb.ToString());
           this.EventFiringEnabled = true;

       }

       /// <summary>
       /// Log the event to the specified list.
       /// </summary>
       /// <param name="web"></param>
       /// <param name="listName"></param>
       /// <param name="eventType"></param>
       /// <param name="details"></param>
       public static void LogEvent(SPWeb web, 
           string listName, 
           SPEventReceiverType eventType, 
           string details)
       {
           SPList logList = EnsureLogList(web.Site.RootWeb, listName);
           SPListItem logItem = logList.Items.Add();
           logItem["Title"] = string.Format("{0} triggered at {1}", eventType, DateTime.Now);
           logItem["Event"] = eventType.ToString();
           //logItem["Before"] = IsBeforeEvent(eventType);
           logItem["Date"] = DateTime.Now;
           logItem["Details"] = details;
           logItem.Update();
       } 

       /// <summary>
       /// Ensures that the Logs list with the specified name is created.
       /// </summary>
       /// <param name="web"></param>
       /// <param name="listName"></param>
       /// <returns></returns>
       private static SPList EnsureLogList(SPWeb web, string listName)
       {
           SPList list = null;
           try
           {
               list = web.Lists[listName];
           }
           catch
           {
               // Create list.
               Guid listGuid = web.Lists.Add(listName, listName, SPListTemplateType.GenericList);
               list = web.Lists[listGuid];
               list.OnQuickLaunch = true;

               // Add the fields to the list.
               // No need to add "Title" because it is added by default.
               // We use it to set the event name.
               list.Fields.Add("Event", SPFieldType.Text, true);
               list.Fields.Add("Before", SPFieldType.Boolean, true);
               list.Fields.Add("Date", SPFieldType.DateTime, true);
               list.Fields.Add("Details", SPFieldType.Note, false);

               // Specify which fields to view.
               SPView view = list.DefaultView;
               view.ViewFields.Add("Event");
               view.ViewFields.Add("Before");
               view.ViewFields.Add("Date");
               view.ViewFields.Add("Details");
               view.Update();

               list.Update();
           }
           return list;
       }
    }
}
