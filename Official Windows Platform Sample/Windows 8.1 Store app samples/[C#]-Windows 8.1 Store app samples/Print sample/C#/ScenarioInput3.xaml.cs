// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Linq;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Graphics.Printing;
using Windows.UI.Xaml.Printing;
using Windows.UI.Xaml.Documents;
using SDKTemplateCS;

namespace PrintSample
{
    /// <summary>
    /// Scenario that demos how to add customizations in the displayed options list.
    /// </summary>
    public sealed partial class ScenarioInput3 : BasePrintPage
    {
        public ScenarioInput3()
        {
            InitializeComponent();
        }
 
        /// <summary>
        /// This is the event handler for PrintManager.PrintTaskRequested.
        /// In order to ensure a good user experience, the system requires that the app handle the PrintTaskRequested event within the time specified by PrintTaskRequestedEventArgs.Request.Deadline.
        /// Therefore, we use this handler to only create the print task.
        /// The print settings customization can be done when the print document source is requested.
        /// </summary>
        /// <param name="sender">PrintManager</param>
        /// <param name="e">PrintTaskRequestedEventArgs</param>        
        protected override void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs e)
        {
            PrintTask printTask = null;
            printTask = e.Request.CreatePrintTask("C# Printing SDK Sample", 
                                                  sourceRequestedArgs =>
                                                  {
                                                      IList<string> displayedOptions = printTask.Options.DisplayedOptions;

                                                      // Choose the printer options to be shown.
                                                      // The order in which the options are appended determines the order in which they appear in the UI
                                                      displayedOptions.Clear();
                                                      displayedOptions.Add(Windows.Graphics.Printing.StandardPrintTaskOptions.Copies);
                                                      displayedOptions.Add(Windows.Graphics.Printing.StandardPrintTaskOptions.Orientation);
                                                      displayedOptions.Add(Windows.Graphics.Printing.StandardPrintTaskOptions.MediaSize);
                                                      displayedOptions.Add(Windows.Graphics.Printing.StandardPrintTaskOptions.Collation);
                                                      displayedOptions.Add(Windows.Graphics.Printing.StandardPrintTaskOptions.Duplex);

                                                      // Preset the default value of the printer option
                                                      printTask.Options.MediaSize = PrintMediaSize.NorthAmericaLegal;

                                                      // Print Task event handler is invoked when the print job is completed.
                                                      printTask.Completed += async (s, args) =>
                                                      {
                                                          // Notify the user when the print operation fails.
                                                          if (args.Completion == PrintTaskCompletion.Failed)
                                                          {
                                                              await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                                                              {
                                                                  rootPage.NotifyUser("Failed to print.", NotifyType.ErrorMessage);
                                                              });
                                                          }
                                                      };

                                                      sourceRequestedArgs.SetSource(printDocumentSource);
                                                  });
        }

        /// <summary>
        /// Provide print content for scenario 3 first page
        /// </summary>
        protected override void PreparePrintContent()
        {
            if (firstPage == null)
            {
                firstPage = new ScenarioOutput3();
                StackPanel header = (StackPanel)firstPage.FindName("header");
                header.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }

            // Add the (newley created) page to the printing root which is part of the visual tree and force it to go
            // through layout so that the linked containers correctly distribute the content inside them.
            PrintingRoot.Children.Add(firstPage);
            PrintingRoot.InvalidateMeasure();
            PrintingRoot.UpdateLayout();
        }

        #region Navigation
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            base.OnNavigatedTo(e);
            // Tell the user how to print
            rootPage.NotifyUser("Print contract registered with customization, use the Charms Bar to print.", NotifyType.StatusMessage);
        }
        #endregion
    }
}
