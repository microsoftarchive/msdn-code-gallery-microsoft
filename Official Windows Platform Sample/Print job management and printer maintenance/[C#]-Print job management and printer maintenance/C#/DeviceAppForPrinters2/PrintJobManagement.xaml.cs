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
using System.Text;
using System.Collections.Generic;

using Microsoft.Samples.Printing.PrinterExtension;
using Microsoft.Samples.Printing.PrinterExtension.Types;

namespace DeviceAppForPrinters2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PrintJobManagementScenario : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public PrintJobManagementScenario()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// This method is invoked when the scenario is navigated away from.
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // If a print queue view is currently being displayed, remove it.
            if (currentPrinterQueueView != null)
            {
                currentPrinterQueueView.OnChanged -= OnPrinterQueueViewChanged;
                currentPrinterQueueView = null;
            }
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
        /// This method is invoked when a printer is selected on the UI.
        /// Displays the print jobs in the selected printer's queue.
        /// </summary>
        private void Printer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // Remove the current printer queue view (if any) before displaying the new view.
                if (currentPrinterQueueView != null)
                {
                    currentPrinterQueueView.OnChanged -= OnPrinterQueueViewChanged;
                    currentPrinterQueueView = null;
                }

                // Retrieve a COM IPrinterExtensionContext object, using the static WinRT factory.
                // Then instantiate one "PrinterExtensionContext" object that allows operations on the COM object.
                PrinterInfo queue = (PrinterInfo)PrinterComboBox.SelectedItem;
                Object comComtext = Windows.Devices.Printers.Extensions.PrintExtensionContext.FromDeviceId(queue.DeviceId);
                PrinterExtensionContext context = new PrinterExtensionContext(comComtext);

                // Display the printer queue view.
                const int FirstPrintJobEnumerated = 0;
                const int LastPrintJobEnumerated = 10;

                currentPrinterQueueView = context.Queue.GetPrinterQueueView(FirstPrintJobEnumerated, LastPrintJobEnumerated);
                currentPrinterQueueView.OnChanged += OnPrinterQueueViewChanged;
            }
            catch (Exception exception)
            {
                rootPage.NotifyUser("Caught an exception: " + exception.Message, NotifyType.ErrorMessage);
            }
        }

        /// <summary>
        /// This method is invoked when a print job is selected on the UI.
        /// Displays the details of the selected print job.
        /// </summary>
        private void PrintJob_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // Display details of the selected print job.
                IPrintJob job = (IPrintJob)PrintJobListBox.SelectedItem;
                if (job != null)
                {
                    PrintJobDetails.Text =
                        "Details of print job: " + job.Name + "\r\n" +
                        "Pages printed: " + job.PrintedPages + "/" + job.TotalPages + "\r\n" +
                        "Submission time: " + job.SubmissionTime + "\r\n" +
                        "Job status: " + DisplayablePrintJobStatus.ToString(job.Status);
                }
                else
                {
                    PrintJobDetails.Text = "Please select a print job";
                }
            }
            catch (Exception exception)
            {
                rootPage.NotifyUser("Caught an exception: " + exception.Message, NotifyType.ErrorMessage);
            }
        }

        /// <summary>
        /// This is the event handler for clicks on the 'Cancel print job' button.
        /// </summary>
        private void CancelPrintJob_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IPrintJob job = (IPrintJob)PrintJobListBox.SelectedItem;
                job.RequestCancel();
            }
            catch (Exception exception)
            {
                rootPage.NotifyUser("Caught an exception: " + exception.Message, NotifyType.ErrorMessage);
            }
        }

        /// <summary>
        /// This callback method is invoked when print jobs are enumerated or when there is a change
        /// to any of the print job fields.
        /// </summary>
        private async void OnPrinterQueueViewChanged(object sender, PrinterQueueViewEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                // Update the data binding on the ListBox that displays print jobs.
                PrintJobListBox.ItemsSource = e.Collection;
                if (PrintJobListBox.Items.Count > 0)
                {
                    // If there are print jobs in the current view, mark the first job as selected.
                    PrintJobListBox.SelectedIndex = 0;
                }
            });
        }

        /// <summary>
        ///  Represents the current printer queue view being displayed.
        /// </summary>
        private IPrinterQueueView currentPrinterQueueView = null;
    }

    /// <summary>
    /// Contains methods to convert PrintJobStatus bit fields to a display string.
    /// </summary>
    internal class DisplayablePrintJobStatus
    {
        /// <summary>
        /// Converts the PrintJobStatus bit fields to a display string.
        /// </summary>
        internal static string ToString(PrintJobStatus printJobStatus)
        {
            StringBuilder statusString = new StringBuilder();

            // Iterate through each of the PrintJobStatus bits that are set and convert it to a display string.
            foreach (var printJobStatusDisplayName in printJobStatusDisplayNames)
            {
                if ((printJobStatusDisplayName.Key & printJobStatus) != 0)
                {
                    statusString.Append(printJobStatusDisplayName.Value);
                }
            }

            int stringlen = statusString.Length;
            if (stringlen > 0)
            {
                // Trim the trailing comma from the string.
                return statusString.ToString(0, stringlen - 1);
            }
            else
            {
                // If no print job status field was set, display "Not available".
                return "Not available";
            }
        }

        /// <summary>
        /// Static constructor that initializes the display name for the PrintJobStatus field.
        /// </summary>
        static DisplayablePrintJobStatus()
        {
            printJobStatusDisplayNames = new Dictionary<PrintJobStatus, string>();

            printJobStatusDisplayNames.Add(PrintJobStatus.Paused, "Paused,");
            printJobStatusDisplayNames.Add(PrintJobStatus.Error, "Error,");
            printJobStatusDisplayNames.Add(PrintJobStatus.Deleting, "Deleting,");
            printJobStatusDisplayNames.Add(PrintJobStatus.Spooling, "Spooling,");
            printJobStatusDisplayNames.Add(PrintJobStatus.Printing, "Printing,");
            printJobStatusDisplayNames.Add(PrintJobStatus.Offline, "Offline,");
            printJobStatusDisplayNames.Add(PrintJobStatus.PaperOut, "Out of paper,");
            printJobStatusDisplayNames.Add(PrintJobStatus.Printed, "Printed,");
            printJobStatusDisplayNames.Add(PrintJobStatus.Deleted, "Deleted,");
            printJobStatusDisplayNames.Add(PrintJobStatus.BlockedDeviceQueue, "Blocked device queue,");
            printJobStatusDisplayNames.Add(PrintJobStatus.UserIntervention, "User intervention required,");
            printJobStatusDisplayNames.Add(PrintJobStatus.Restarted, "Restarted,");
            printJobStatusDisplayNames.Add(PrintJobStatus.Complete, "Complete,");
            printJobStatusDisplayNames.Add(PrintJobStatus.Retained, "Retained,");
        }
        
        /// <summary>
        /// Private constructor to prevent default instantiation.
        /// </summary>
        private DisplayablePrintJobStatus() { }

        /// <summary>
        /// Contains the mapping between PrintJobStatus fields and display strings.
        /// </summary>
        private static Dictionary<PrintJobStatus, string> printJobStatusDisplayNames;
    }
}
