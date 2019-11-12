using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;

namespace BTLE_BackgroundTasksForToasts
{
    public sealed class ToastBackgroundTask : IBackgroundTask
    {
        // A character to be used as a divider between devices/services/characteristics. 
        public static char ToastSplit
        {
            get
            {
                return '|';
            }
        }

        // The "Run" function is called whenever the ToastBackgroundTask is executed.
        // If one wanted to put in some other sort of background functionality, the entry point
        // would be a corresponding "Run" function. 

        // Dependencies:
        // Background task access is requested in the GlobalSettings.cs file upon initialization
        // of the application.
        // Background tasks are registered in the BECharacteristicModel.cs file. 
        // NOTE: The background project (BTLE_BackgroundTasksForToasts) needs to be linked to the original
        // (BTLE_Explorer) in the Package.appxmanifest file in order for any of this to compile.. 
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // Make sure this function doesn't return until it's done.
            BackgroundTaskDeferral taskDeferral = taskInstance.GetDeferral();

            // Get the toast template 
            ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText04;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

            // Set up toast string
            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            string printMe = (string) ApplicationData.Current.LocalSettings.Values[taskInstance.Task.Name];
            if (printMe == null)
            {
                // Possible that we may have unregistered the toast and removed the string. If this is the case, stop. 
                taskDeferral.Complete(); 
                return; 
            }
            string[] parts = printMe.Split(ToastSplit);
            if (parts.Length == 3) 
            {
                // parts.Length should be 3 (device name, service name, characteristic name); do this to be safe. 
                toastTextElements[0].AppendChild(toastXml.CreateTextNode("D: " + parts[0] + "; S: " + parts[1]));
                toastTextElements[1].AppendChild(toastXml.CreateTextNode(parts[2] + " -- Value Changed!"));
            }

            // Set up toast image
            XmlNodeList toastImageElements = toastXml.GetElementsByTagName("image");
            ((XmlElement)toastImageElements[0]).SetAttribute("src", "///Images/SmallLogo.scale-240.png");
            ((XmlElement)toastImageElements[0]).SetAttribute("alt", "BTLE_Explorer Toast Graphic");

            // Set up toast pop-up attributes that we care to change. 
            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            ((XmlElement)toastNode).SetAttribute("duration", "short");           

            // Make and show the toast
            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);

            // We're done now.
            taskDeferral.Complete(); 
        }
    }
}
