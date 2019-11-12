//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System;
using Windows.UI.ApplicationSettings;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace AzureMobileSendEmail
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        public MainPage()
        {
            this.InitializeComponent();

            // Populate the sample title from the constant in the Constants.cs file.
            SetFeatureName(FEATURE_NAME);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            SettingsPane.GetForCurrentView().CommandsRequested += this.SetCommandsRequested;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            SettingsPane.GetForCurrentView().CommandsRequested -= this.SetCommandsRequested;
        }

        private void SetCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            SettingsCommand cmd = new SettingsCommand(
                 "sample",
                 "Feedback",
                 (x) =>
                 {
                     // create a new instance of the flyout
                     SettingsFlyout settings = new SettingsFlyout();
                     settings.Title = "Provide Feedback";
                     BitmapImage bitmap = new BitmapImage(new Uri("ms-appx:///Assets/SmallLogo.png"));
                     settings.IconSource = bitmap;

                     // set the content for the flyout
                     var settingsContent = new FeedbackContent();
                     settingsContent.FeedbackSent += (s, e) =>
                     {
                         settings.Hide();
                     };

                     settings.HorizontalContentAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch;
                     settings.Content = settingsContent;

                     // open it
                     settings.Show();
                 });

            args.Request.ApplicationCommands.Add(cmd);
        }

        async void Footer_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(((HyperlinkButton)sender).Tag.ToString()));
        }

        private void SetFeatureName(string str)
        {
            FeatureName.Text = str;
        }
    }
}
