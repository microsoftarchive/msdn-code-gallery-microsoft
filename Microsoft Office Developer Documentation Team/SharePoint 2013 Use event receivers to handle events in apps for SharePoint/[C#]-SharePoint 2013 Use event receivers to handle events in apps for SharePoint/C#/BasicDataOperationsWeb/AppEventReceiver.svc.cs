using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;

namespace BasicDataOperationsWeb
{
    public class AppEventReceiver : IRemoteEventService
    {
        public SPRemoteEventResult ProcessEvent(SPRemoteEventProperties properties)
        {
            // When an app event occurs, log it.
            LogAppEvents(properties.EventType.ToString());
            return new SPRemoteEventResult();
        }

        public void ProcessOneWayEvent(SPRemoteEventProperties properties)
        {
            // This method is not used by app events.
        }

        public static void LogAppEvents(string eventType)
        {
            // Creates a log folder for app events, names the folder "SPAppEventLogs" in the 
            // \My Documents folder, and then creates a file that identifies the event and a 
            // date/time stamp when the app event occurred.
            // You must include an exception handler in the app event receiver, because 
            // unhandled exceptions can prevent the app from loading.
            try
            {
                string folder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "SPAppEventLog");

                // Create the "SPAppEventLogs" folder if it doesn't already exist.
                if (!System.IO.Directory.Exists(folder))
                    System.IO.Directory.CreateDirectory(folder);

                // Name the log file using the date/time stamp when the app event receiver
                // fired, such as "AppInstalled20121130030240329.log". Note that this is 
                // only a simple procedure for illustrative purpose; you can replace this 
                // with any code you like to respond to the event.
                string path = string.Format("{0}\\{1}{2}.log", folder,
                    eventType, DateTime.Now.ToString("yyyyMMddhhmmssfff"));

                // Create the log file.
                using (System.IO.FileStream fs = System.IO.File.Create(path))
                {
                    fs.Flush();
                    fs.Close();
                }
            }
            catch (Exception)
            {
            }

            return;
        }
    }
}
