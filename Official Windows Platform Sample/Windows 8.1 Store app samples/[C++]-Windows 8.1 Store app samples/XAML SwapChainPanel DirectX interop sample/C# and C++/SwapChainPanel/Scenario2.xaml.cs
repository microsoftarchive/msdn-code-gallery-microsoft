//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using SDKTemplate;
using DirectXPanels;
using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace SwapChainPanel
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario2 : SDKTemplate.Common.LayoutAwarePage
    {
        private const string DefaultFileExtension = ".gif";

        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        // The active data model.
        private Model.DrawingAttributes _dataContext;

        public Scenario2()
        {
            this.InitializeComponent();
            // Create a new data model and set it as the page's data context.
            _dataContext = new Model.DrawingAttributes();
            Output.DataContext = _dataContext;

            rootPage.SizeChanged += MainPage_SizeChanged;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            UpdateDrawingPanelLayout();
            DrawingPanel.StartProcessingInput();
        }

        /// <summary>
        /// Invoked when this page will no longer be displayed in a Frame.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            DrawingPanel.StopProcessingInput();
        }

        /// <summary>
        /// Event handler for the RecognitionResultsUpdated event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// Displays handwriting recognition results from DrawingPanel.
        /// </remarks>
        private void DrawingPanel_RecognitionResultsUpdated(object sender, RecognitionResultUpdatedEventArgs e)
        {
            RecognitionResultsTextBlock.Text = string.Empty;

            if (e.Results != null)
            {
                foreach (var result in e.Results)
                {
                    RecognitionResultsTextBlock.Text += result + " ";
                }
            }
        }

        /// <summary>
        /// Event handler for the Save button click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SaveButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (!DrawingPanel.HasContent)
            {
                rootPage.NotifyUser("Must have content before saving.", NotifyType.ErrorMessage);
                return;
            }

            // Saves the current drawing to a GIF-compatible file the user selects.
            FileSavePicker picker = new FileSavePicker();
            picker.FileTypeChoices.Add(new KeyValuePair<string, IList<string>>("SCPDraw", new List<string>() { DefaultFileExtension }));
            picker.DefaultFileExtension = DefaultFileExtension;
            picker.SuggestedFileName = "drawing";
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

            var file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    await DrawingPanel.SaveStrokesToStreamAsync(stream);
                }

                rootPage.NotifyUser("Saved drawing to file: " + file.Name, NotifyType.StatusMessage);
            }
        }

        /// <summary>
        /// Event handler for the Load button click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void LoadButton_Clicked(object sender, RoutedEventArgs e)
        {
            // Loads a previously saved drawing.
            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(DefaultFileExtension);
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

            var file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                using (var stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    try
                    {
                        await DrawingPanel.LoadStrokesFromStreamAsync(stream);
                        DrawingPanel.Update();
                    }
                    catch (Exception)
                    {
                        rootPage.NotifyUser("Unable to load file.", NotifyType.ErrorMessage);
                    }
                }
            }
        }

        /// <summary>
        /// Event handler for the Replay button click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ReplayButton_Clicked(object sender, RoutedEventArgs e)
        {
            // Replays a previously saved drawing, drawing each strokein the order the author originally drew them.
            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(DefaultFileExtension);
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

            var file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                using (var stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    // Draw each stroke segment over a 50ms interval.
                    try
                    {
                        DrawingPanel.BeginStrokesReplayFromStream(stream, 50);
                    }
                    catch (Exception)
                    {
                        rootPage.NotifyUser("Unable to load file.", NotifyType.ErrorMessage);
                    }
                }
            }
        }

        /// <summary>
        /// Event handler for the main page SizeChanged event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateDrawingPanelLayout();
        }

        /// <summary>
        /// Updates size and position of elements on the page when the size changes.
        /// </summary>
        private void UpdateDrawingPanelLayout()
        {
            DrawingPanel.Width = (MainPage.Current.FindName("ContentRoot") as FrameworkElement).ActualWidth - (DrawingPanelBorder.BorderThickness.Left + DrawingPanelBorder.BorderThickness.Right);
        }
    }
}
