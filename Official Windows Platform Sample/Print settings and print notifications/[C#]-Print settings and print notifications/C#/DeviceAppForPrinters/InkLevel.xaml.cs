//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;

using Windows.UI.Xaml.Media.Imaging;
using Windows.Devices.Printers.Extensions;
using Windows.Devices.Enumeration;
using Windows.Devices.Enumeration.Pnp;
using Microsoft.Samples.Printers.Extensions;

namespace DeviceAppForPrinters
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InkLevel : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        PrintHelperClass printHelper;

        private const string keyPrinterName = "BA5857FA-DE2C-4A4A-BEF2-49D8B4130A39";
        private const string keyAsyncUIXML = "55DCA47A-BEE9-43EB-A7C8-92ECA2FA0685";
        Windows.Storage.ApplicationDataContainer settings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public InkLevel()
        {
            this.InitializeComponent();

            // Disables scenario navigation by hiding the navigation UI elements.
            ((UIElement)rootPage.FindName("Scenarios")).Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            ((UIElement)rootPage.FindName("ScenarioListLabel")).Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            ((UIElement)rootPage.FindName("DescriptionText")).Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            rootPage.NotifyUser("Notification updated", NotifyType.StatusMessage);
            DisplayBackgroundTaskTriggerDetails();

            // Clearing the live tile status
            Windows.UI.Notifications.TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            Windows.UI.Notifications.BadgeUpdateManager.CreateBadgeUpdaterForApplication().Clear();
        }

        /// <summary>
        /// Invoked when this page is navigated away from.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Unsubscribe from the OnInkLevelReceived event.
            if (printHelper != null)
            {
                printHelper.OnInkLevelReceived -= OnInkLevelReceived;
                printHelper = null;
            }
        }

        void DisplayBackgroundTaskTriggerDetails()
        {
            String outputText = "\r\n";

            try
            {
                string printerName = settings.Values[keyPrinterName].ToString();
                outputText += ("Printer name from background task triggerDetails: " + printerName);
            }
            catch (Exception)
            {
                outputText += ("No printer name retrieved from background task triggerDetails ");
            }

            outputText += "\r\n";
            try
            {
                string asyncUIXML = settings.Values[keyAsyncUIXML].ToString();
                outputText += ("AsyncUI xml from background task triggerDetails: " + asyncUIXML);
            }
            catch (Exception)
            {
                outputText += ("No asyncUI xml retrieved from background task triggerDetails ");
            }

            ToastOutput.Text += outputText;
        }

        /// <summary>
        /// Enumerates the printers by the following steps.
        ///     1. Searching through all devices interfaces for printers.
        ///     2. Getting the container for each printer device.
        ///     3. Checking for association by comparing each container's PackageFamilyName property
        /// </summary>
        /// <param name="sender" type = "Windows.UI.Xaml.Controls.Button">A pointer to the button that the user hit to enumerate printers</param>
        /// <param name="e">Arguments passed in by the event.</param>
        async void EnumerateAssociatedPrinters(object sender, RoutedEventArgs e)
        {
            // Reset output text and associated printer array.
            AssociatedPrinters.Items.Clear();
            BidiOutput.Text = "";

            // GUID string for printers.
            string printerInterfaceClass = "{0ecef634-6ef0-472a-8085-5ad023ecbccd}";
            string selector = "System.Devices.InterfaceClassGuid:=\"" + printerInterfaceClass + "\"";

            // By default, FindAllAsync does not return the containerId for the device it queries.
            // We have to add it as an additonal property to retrieve. 
            string containerIdField = "System.Devices.ContainerId";
            string[] propertiesToRetrieve = new string[] { containerIdField };

            // Asynchronously find all printer devices.
            DeviceInformationCollection deviceInfoCollection = await DeviceInformation.FindAllAsync(selector, propertiesToRetrieve);

            // For each printer device returned, check if it is associated with the current app.
            for (int i = 0; i < deviceInfoCollection.Count; i++)
            {
                DeviceInformation deviceInfo = deviceInfoCollection[i];
                FindAssociation(deviceInfo, deviceInfo.Properties[containerIdField].ToString());
            }
        }

        /// <summary>
        /// Check if a printer is associated with the current application, if it is, add its interfaceId to a list of associated interfaceIds.
        /// 
        ///     For each different app, it will have a different correctPackageFamilyName.
        ///     Look in the Visual Studio packagemanifest editor to see what it is for your app.
        /// </summary>
        /// <param name="deviceInfo">The deviceInformation of the printer.</param>
        async void FindAssociation(DeviceInformation deviceInfo, string containerId)
        {

            // Specifically telling CreateFromIdAsync to retrieve the AppPackageFamilyName. 
            string packageFamilyName = "System.Devices.AppPackageFamilyName";
            string[] containerPropertiesToGet = new string[] { packageFamilyName };

            // CreateFromIdAsync needs braces on the containerId string.
            string containerIdwithBraces = "{" + containerId + "}";

            // Asynchoronously getting the container information of the printer.
            PnpObject containerInfo = await PnpObject.CreateFromIdAsync(PnpObjectType.DeviceContainer, containerIdwithBraces, containerPropertiesToGet);

            // Printers could be associated with other device apps, only the ones with package family name
            // matching this app's is associated with this app. The packageFamilyName for this app will be found in this app's packagemanifest
            string appPackageFamilyName = "Microsoft.SDKSamples.DeviceAppForPrinters.CS_8wekyb3d8bbwe";
            var prop = containerInfo.Properties;

            // If the packageFamilyName of the printer container matches the one for this app, the printer is associated with this app.
            string[] packageFamilyNameList = (string[])prop[packageFamilyName];
            if (packageFamilyNameList != null)
            {
                for (int j = 0; j < packageFamilyNameList.Length; j++)
                {
                    if (packageFamilyNameList[j].Equals(appPackageFamilyName))
                    {
                        AddToList(deviceInfo);
                    }
                }
            }
        }

        /// <summary>
        /// Adds the printer to the selection box to allow the user to select it.
        /// </summary>
        /// <param name="deviceInfo">Contains the device information of the printer to be added to the combo box.</param>
        void AddToList(DeviceInformation deviceInfo)
        {
            // Creating a new display item so the user sees the friendly name instead of the interfaceId.
            ComboBoxItem item = new ComboBoxItem();
            item.Content = deviceInfo.Properties["System.ItemNameDisplay"] as string;
            item.DataContext = deviceInfo.Id;
            AssociatedPrinters.Items.Add(item);

            // If this is the first printer to be added to the combo box, select it.
            if (AssociatedPrinters.Items.Count == 1)
            {
                AssociatedPrinters.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Sends a ink status query to the selected printer.
        /// </summary>
        /// <param name="sender" type = "Windows.UI.Xaml.Controls.Button">A pointer to the button that the user hit to enumerate printers</param>
        /// <param name="e">Arguments passed in by the event.</param>
        void GetInkStatus(object sender, RoutedEventArgs e)
        {
            if (AssociatedPrinters.Items.Count > 0)
            {
                // Get the printer that the user has selected to query.
                ComboBoxItem selectedItem = AssociatedPrinters.SelectedItem as ComboBoxItem;

                // The interfaceId is retrieved from the detail field.
                string interfaceId = selectedItem.DataContext as string;

                try
                {
                    // Unsubscribe existing ink level event handler, if any.
                    if (printHelper != null)
                    {
                        printHelper.OnInkLevelReceived -= OnInkLevelReceived;
                        printHelper = null;
                    }

                    object context = Windows.Devices.Printers.Extensions.PrintExtensionContext.FromDeviceId(interfaceId);

                    // Use the PrinterHelperClass to retrieve the bidi data and display it.
                    printHelper = new PrintHelperClass(context);
                    try
                    {
                        printHelper.OnInkLevelReceived += OnInkLevelReceived;
                        printHelper.SendInkLevelQuery();

                        rootPage.NotifyUser("Ink level query successful", NotifyType.StatusMessage);
                    }
                    catch (Exception)
                    {
                        rootPage.NotifyUser("Ink level query unsuccessful", NotifyType.ErrorMessage);
                    }
                }
                catch (Exception)
                {
                    rootPage.NotifyUser("Error retrieving PrinterExtensionContext from InterfaceId", NotifyType.ErrorMessage);
                }
            }
        }

        /// <summary>
        /// This event handler method is invoked when ink level data is available.
        /// </summary>
        private void OnInkLevelReceived(object sender, string response)
        {
            BidiOutput.Text = response;
        }
    }
}
