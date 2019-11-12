using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.Devices.Printers.Extensions;
using Windows.Data.Xml.Dom;

namespace BackgroundTask
{
    /// <summary>
    /// This class is the entry point for background notification tasks, as declared in the manifest
    /// The Run method is called when an event is fired.  We then create and show a toast.  Clicking this
    /// toast will take the user to the App
    /// </summary>
    public sealed class PrintBackgroundTask : Windows.ApplicationModel.Background.IBackgroundTask
    {
        //
        // Save the printer name and asyncUI xml
        //
        private const string keyPrinterName = "BA5857FA-DE2C-4A4A-BEF2-49D8B4130A39";
        private const string keyAsyncUIXML = "55DCA47A-BEE9-43EB-A7C8-92ECA2FA0685";
        Windows.Storage.ApplicationDataContainer settings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public void Run(Windows.ApplicationModel.Background.IBackgroundTaskInstance taskInstance)
        {
            PrintNotificationEventDetails details = (PrintNotificationEventDetails)taskInstance.TriggerDetails;
            settings.Values[keyPrinterName] = details.PrinterName;
            settings.Values[keyAsyncUIXML] = details.EventData;

            // With the print notification event details we can choose to show a toast, update the tile, or update the badge.
            // It is not recommended to always show a toast, especially for non-actionable events, as it may become annoying for most users.
            // User may even just turn off all toasts from this app, which is not a desired outcome.
            // For events that does not require user's immediate attention, it is recommended to update the tile/badge and not show a toast.
            ShowToast(details.PrinterName, details.EventData);
            UpdateTile(details.PrinterName, details.EventData);
            UpdateBadge();
        }

        void UpdateTile(string printerName, string bidiMessage)
        {
            TileUpdater tileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
            tileUpdater.Clear();

            XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150Text09);
            XmlNodeList tileTextAttributes = tileXml.GetElementsByTagName("text");
            tileTextAttributes[0].InnerText = printerName;
            tileTextAttributes[1].InnerText = bidiMessage;

            TileNotification tileNotification = new TileNotification(tileXml);
            tileNotification.Tag = "tag01";
            tileUpdater.Update(tileNotification);
        }

        void UpdateBadge()
        {
            XmlDocument badgeXml = BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeGlyph);
            XmlElement badgeElement = (XmlElement)badgeXml.SelectSingleNode("/badge");
            badgeElement.SetAttribute("value", "error");

            var badgeNotification = new BadgeNotification(badgeXml);
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badgeNotification);
        }

        void ShowToast(string title, string body)
        {
            //
            // Get Toast template
            //
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

            //
            // Pass to app as eventArgs.detail.arguments
            //
            ((XmlElement)toastXml.SelectSingleNode("/toast")).SetAttribute("launch", title);

            //
            // The ToastText02 template has 2 text nodes (a header and a body)
            // Assign title to the first one, and body to the second one
            //
            XmlNodeList textList = toastXml.GetElementsByTagName("text");
            textList[0].AppendChild(toastXml.CreateTextNode(title));
            textList[1].AppendChild(toastXml.CreateTextNode(body));

            //
            // Show the Toast
            //
            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}
