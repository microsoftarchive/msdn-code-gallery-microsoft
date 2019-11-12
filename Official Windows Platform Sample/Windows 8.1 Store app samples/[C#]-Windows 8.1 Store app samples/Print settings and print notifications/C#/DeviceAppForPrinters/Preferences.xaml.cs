//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;

using Windows.Devices.Printers.Extensions;
using Microsoft.Samples.Printers.Extensions;

namespace DeviceAppForPrinters
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Preferences : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        PrintTaskConfiguration configuration;
        Object printerExtensionContext;
        PrintHelperClass printHelper;

        // The features in this sample were chosen because they're available on a wide range
        // of printer drivers.
        private string[] features = { "PageOrientation", "PageOutputColor", "PageMediaSize", "PageMediaType" };
        private string[] selections = { null, null, null, null };

        public Preferences()
        {
            this.InitializeComponent();

            configuration = rootPage.Config;
            printerExtensionContext = rootPage.Context;
            printHelper = new PrintHelperClass(printerExtensionContext);

            // Disable scenario navigation by hiding the scenario list UI elements
            ((UIElement)rootPage.FindName("Scenarios")).Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            ((UIElement)rootPage.FindName("ScenarioListLabel")).Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            ((UIElement)rootPage.FindName("DescriptionText")).Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            DisplaySettings();
        }
        
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (null == configuration)
            {
                rootPage.NotifyUser("Configuration arguments cannot be null", NotifyType.ErrorMessage);
                return;
            }

            // Add an event listener for saverequested (the back button of the flyout is pressed).
            configuration.SaveRequested += OnSaveRequested;
        }

        /// <summary>
        /// Displays the advanced print setting information.
        /// </summary>
        /// <param name="refreshed">Boolean representing the method should check for constraints,
        /// defaults to false unless specified.</param>
        private void DisplaySettings(bool constraints=false)
        {
            PrintOptions.Visibility = Windows.UI.Xaml.Visibility.Visible;
            WaitPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            // Fill in the drop-down select controls for some common printing features.
            TextBlock[] featureLabels = { PageOrientationLabel, PageOutputColorLabel, PageMediaSizeLabel, PageMediaTypeLabel };
            ComboBox[] featureBoxes = { PageOrientationBox, PageOutputColorBox, PageMediaSizeBox, PageMediaTypeBox };

            for (int i = 0; i < features.Length; i++)
            {
                // Only display a feature if it exists
                featureLabels[i].Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                featureBoxes[i].Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                string feature = features[i];

                // Check whether the currently selected printer's capabilities include this feature.
                if (!printHelper.FeatureExists(feature))
                {
                    continue;
                }

                // Fill in the labels so that they display the display name of each feature.
                featureLabels[i].Text = printHelper.GetFeatureDisplayName(feature);
                string[] index = printHelper.GetOptionInfo(feature, "Index");
                string[] displayName = printHelper.GetOptionInfo(feature, "DisplayName");
                string selectedOption = printHelper.GetSelectedOptionIndex(feature);

                // Unless specified, do not get constraints
                bool[] constrainedList = constraints ? printHelper.GetOptionConstraints(feature) : new bool[index.Length];

                // Populate the combo box with the options for the current feature.
                PopulateBox(featureBoxes[i], index, displayName, selectedOption, constrainedList);
                selections[i] = selectedOption;

                // Everytime the selection for a feature changes, we update our local cached set of selections.
                featureBoxes[i].SelectionChanged += OnFeatureOptionsChanged;

                // Show existing features
                featureLabels[i].Visibility = Windows.UI.Xaml.Visibility.Visible;
                featureBoxes[i].Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
        }

        /// <summary>
        /// Populate the combo box with the options for the current feature.
        /// </summary>
        /// <param name="box">The combo box to be populated.</param>
        /// <param name="index">The index of the option.</param>
        /// <param name="displayName">The display name that the user sees instead of the index.</param>
        /// <param name="selectedOption">The option that is selected right now.</param>
        void PopulateBox(ComboBox box, string[] index, string[] displayName, string selectedOption, bool[] constrainedList)
        {
            // Clear the combobox of any options from previous UI refresh before repopulating it.
            box.SelectionChanged -= OnFeatureOptionsChanged;
            box.Items.Clear();
            // There should be only one displayName for each possible option.
            if (index.Length == displayName.Length)
            {
                for (int i = 0; i < index.Length; i++)
                {
                    // Create a new DisplayItem so the user will see the friendly displayName instead of the index.
                    ComboBoxItem newItem = new ComboBoxItem();
                    newItem.Content = displayName[i];
                    newItem.DataContext = index[i];
                    newItem.Foreground = constrainedList[i] ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
                    box.Items.Add(newItem);

                    // Display current selected option as selected in the combo box.
                    if (selectedOption == index[i])
                    {
                        box.SelectedIndex = i;
                        box.Foreground = newItem.Foreground;
                    }
                }
            }
        }

        

        /// <summary>
        /// When the user changed any selection, update local cached selection.
        /// </summary>
        /// <param name="sender" type="Windows.UI.Xaml.Combobox">The combobox whose selection has been changed.</param>
        /// <param name="args"></param>
        private void OnFeatureOptionsChanged(object sender, SelectionChangedEventArgs args)
        {
            ComboBox comboBox = sender as ComboBox;

            for (int i = 0; i < features.Length; i++)
            {
                if (features[i] + "Box" == comboBox.Name)
                {
                    selections[i] = (comboBox.SelectedItem as ComboBoxItem).DataContext as string;
                }
            }
        }

        /// <summary>
        /// The event handler that is fired when the user clicks on the back button.
        /// Saves the print ticket.
        /// This is fired on a different thread than the main thread so will need a dispatcher to get back on the main thread to access objects created on the main thread.
        /// </summary>
        /// <param name="sender">The back button.</param>
        /// <param name="args">Arguments passed in by the event.</param>
        async private void OnSaveRequested(object sender, PrintTaskConfigurationSaveRequestedEventArgs args)
        {
            if (null == printHelper || null == printerExtensionContext || null == args)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    rootPage.NotifyUser("onSaveRequested: args, printHelper, and context cannot be null", NotifyType.ErrorMessage);
                });
                return;
            }

            // Get the request object, which has the save method that allows saving updated print settings.
            PrintTaskConfigurationSaveRequest request = args.Request;

            if (null == request)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    rootPage.NotifyUser("onSaveRequested: request cannot be null", NotifyType.ErrorMessage);
                });
                return;
            }

            PrintTaskConfigurationSaveRequestedDeferral deferral = request.GetDeferral();

            // Two separate messages are dispatched to:
            // 1) put up a popup panel,
            // 2) set the each options to the print ticket and attempt to save it,
            // 3) tear down the popup panel if the print ticket could not be saved.
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                PrintOptions.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                WaitPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
            });

            // Go through all the feature select elements, look up the selected
            // option name, and update the context
            // for each feature
            for (var i = 0; i < features.Length; i++)
            {
                // Set the feature's selected option in the context's print ticket.
                // The printerExtensionContext object is updated with each iteration of this loop
                printHelper.SetFeatureOption(features[i], selections[i]);
            }
            
            bool ticketSaved;
            try
            {
                // This save request will throw an exception if ticket validation fails.
                // When the exception is thrown, the app flyout will remain.
                // If you want the flyout to remain regardless of outcome, you can call
                // request.Cancel(). This should be used sparingly, however, as it could
                // disrupt the entire the print flow and will force the user to 
                // light dismiss to restart the entire experience.
                request.Save(printerExtensionContext);

                if (configuration != null)
                {
                    configuration.SaveRequested -= OnSaveRequested;
                }
                ticketSaved = true;
            }
            catch (Exception exp)
            {
                // Check if the HResult from the exception is from an invalid ticket, otherwise rethrow the exception
                if (exp.HResult.Equals(unchecked((int)0x8007000D))) // E_INVALID_DATA
                {
                    ticketSaved = false;
                }
                else
                {
                    throw;
                }
            }

            // If ticket isn't saved, refresh UI and notify user
            if (!ticketSaved)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    rootPage.NotifyUser("Failed to save the print ticket", NotifyType.ErrorMessage);
                    DisplaySettings(true);
                });
            }
            deferral.Complete();
        }

    }
}
