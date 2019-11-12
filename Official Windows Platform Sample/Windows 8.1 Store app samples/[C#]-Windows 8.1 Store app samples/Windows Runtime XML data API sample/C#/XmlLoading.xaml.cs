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

namespace Xml
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class XmlLoading : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public XmlLoading()
        {
            this.InitializeComponent();

            Scenario3Init();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        async void Scenario3Init()
        {
            try
            {
                Windows.Data.Xml.Dom.XmlDocument doc = await Scenario.LoadXmlFile("loadExternaldtd", "xmlWithExternaldtd.xml");
                Scenario.RichEditBoxSetMsg(scenario3OriginalData, doc.GetXml(), true);
            }
            catch
            {
                Scenario.RichEditBoxSetError(scenario3Result, Scenario.LoadFileErrorMsg);
            }
        }

        /// <summary>
        /// This is the click handler for the 'Scenario3BtnDefault' button.  You would replace this with your own handler
        /// if you have a button or buttons on this page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Scenario3BtnDefault_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // get load settings
                var loadSettings = new Windows.Data.Xml.Dom.XmlLoadSettings();
                if (true == scenario3RB1.IsChecked)
                {
                    loadSettings.ProhibitDtd = true;        // DTD is prohibited
                    loadSettings.ResolveExternals = false;  // Disable the resolve to external definitions such as external DTD
                }
                else if (true == scenario3RB2.IsChecked)
                {
                    loadSettings.ProhibitDtd = false;       // DTD is not prohibited
                    loadSettings.ResolveExternals = false;  // Disable the resolve to external definitions such as external DTD
                }
                else if (true == scenario3RB3.IsChecked)
                {
                    loadSettings.ProhibitDtd = false;       // DTD is not prohibited
                    loadSettings.ResolveExternals = true;   // Enable the resolve to external definitions such as external DTD
                }

                var folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("loadExternaldtd");
                var file = await folder.GetFileAsync("xmlWithExternaldtd.xml");

                // Load xml file with external DTD
                var doc = await Windows.Data.Xml.Dom.XmlDocument.LoadFromFileAsync(file, loadSettings);
                Scenario.RichEditBoxSetMsg(scenario3Result, "The file is loaded successfully.", true);
            }
            catch (Exception exp)
            {
                // After loadSettings.ProhibitDtd is set to true, the exception is expected as the sample XML contains DTD
                Scenario.RichEditBoxSetError(scenario3Result, "Error: DTD is prohibited");
            }
        }

        async void LaunchUri(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(((HyperlinkButton)sender).Tag.ToString()));
        }

    }
}
