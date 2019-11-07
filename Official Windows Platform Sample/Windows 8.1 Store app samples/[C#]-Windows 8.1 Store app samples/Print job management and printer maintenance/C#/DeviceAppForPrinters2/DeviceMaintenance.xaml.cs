//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using SDKTemplate;

using System;
using System.Collections.Generic;

using Microsoft.Samples.Printing.PrinterExtension;
using Microsoft.Samples.Printing.PrinterExtension.Types;

namespace DeviceAppForPrinters2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DeviceMaintenanceScenario : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public DeviceMaintenanceScenario()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string defaultBidiQuery =
                "<bidi:Set xmlns:bidi=\"http://schemas.microsoft.com/windows/2005/03/printing/bidi\">\r\n" +
                "    <Query schema='\\Printer.Maintenance:CleanHead'>\r\n" +
                "        <BIDI_BOOL>false</BIDI_BOOL>\r\n" +
                "    </Query>\r\n" +
                "</bidi:Set>";

            BidiQueryInput.Text = defaultBidiQuery;
        }

        /// <summary>
        /// Event handler for clicks on the "Enumerate printers" button. Enumerates printers
        /// and updates the data binding on the ComboBox.
        /// </summary>
        private async void EnumeratePrinters_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                rootPage.NotifyUser("Enumerating printers. Please wait", NotifyType.StatusMessage);

                // Retrieve the running app's package family name, and enumerate associated printers.
                string currentPackageFamilyName = Windows.ApplicationModel.Package.Current.Id.FamilyName;

                // Enumerate associated printers.
                PrinterEnumeration pe = new PrinterEnumeration(currentPackageFamilyName);
                List<PrinterInfo> associatedPrinters = await pe.EnumeratePrintersAsync();

                // Update the data binding source on the combo box that displays the list of printers.
                PrinterComboBox.ItemsSource = associatedPrinters;
                if (associatedPrinters.Count > 0)
                {
                    PrinterComboBox.SelectedIndex = 0;
                    rootPage.NotifyUser(associatedPrinters.Count + " printers enumerated", NotifyType.StatusMessage);
                }
                else
                {
                    rootPage.NotifyUser(DisplayStrings.NoPrintersEnumerated, NotifyType.ErrorMessage);
                }
            }
            catch (Exception exception)
            {
                rootPage.NotifyUser("Caught an exception: " + exception.Message, NotifyType.ErrorMessage);
            }
        }

        /// <summary>
        /// Event handler for clicks on the "Send bidi request" button.
        /// </summary>
        private void SendBidiRequest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PrinterInfo queue = (PrinterInfo)PrinterComboBox.SelectedItem;

                // Retrieve a COM IPrinterExtensionContext object, using the static WinRT factory.
                // Then instantiate one "PrinterExtensionContext" object that allows operations on the COM object.
                Object comComtext = Windows.Devices.Printers.Extensions.PrintExtensionContext.FromDeviceId(queue.DeviceId);
                PrinterExtensionContext context = new PrinterExtensionContext(comComtext);

                // Create an instance of the callback object, and perform an asynchronous 'bidi set' operation.
                PrinterBidiSetRequestCallback callback = new PrinterBidiSetRequestCallback();

                // Add an event handler to the callback object's OnBidiResponseReceived event.
                // The event handler will be invoked once the Bidi response is received.
                callback.OnBidiResponseReceived += OnBidiResponseReceived;

                // Send the Bidi "Set" query asynchronously.
                IPrinterExtensionAsyncOperation operationContext
                    = context.Queue.SendBidiSetRequestAsync(BidiQueryInput.Text, callback);

                // Note: The 'operationContext' object can be used to cancel the operation if required.
            }
            catch (Exception exception)
            {
                rootPage.NotifyUser("Caught an exception: " + exception.Message, NotifyType.ErrorMessage);
            }
        }

        /// <summary>
        /// This method displays the Bidi response on the UI.
        /// </summary>
        internal async void OnBidiResponseReceived(object sender, string bidiResponse)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                BidiResponseOutput.Text = bidiResponse;
            });
        }
    }

    /// <summary>
    /// Represents the callback object that is invoked when a Bidi "Set" operation is completed.
    /// </summary>
    internal class PrinterBidiSetRequestCallback : IPrinterBidiSetRequestCallback
    {
        /// <summary>
        /// This method is invoked when the asynchronous Bidi "Set" operation is completed.
        /// </summary>
        public void Completed(string response, int statusHResult)
        {
            string result;

            if (statusHResult == (int)HRESULT.S_OK)
            {
                result = "The response is \r\n" + response;
            }
            else
            {
                result = "The HRESULT received is: 0x" + statusHResult.ToString("X") + "\r\n" +
                         "No Bidi response was received";
            }

            // Invoke the event handlers when the Bidi response is received.
            OnBidiResponseReceived(null, result);
        }

        /// <summary>
        /// This event will be invoked when the Bidi 'set' response is received.
        /// </summary>
        public event EventHandler<string> OnBidiResponseReceived;
    }

    internal enum HRESULT : int
    {
        S_OK = 0
    }
}
