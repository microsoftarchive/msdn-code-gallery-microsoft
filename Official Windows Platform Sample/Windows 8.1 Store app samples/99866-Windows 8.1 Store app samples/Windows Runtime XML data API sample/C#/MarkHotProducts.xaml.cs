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
    public sealed partial class MarkHotProducts : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public MarkHotProducts()
        {
            this.InitializeComponent();

            Scenario2Init();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        /// <summary>
        /// This is the click handler for the 'Scenario2BtnDefault' button.
        /// This function will look up products and mark hot products.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Scenario2BtnDefault_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String xml;
                scenario2OriginalData.Document.GetText(Windows.UI.Text.TextGetOptions.None, out xml);

                var doc = new Windows.Data.Xml.Dom.XmlDocument();
                doc.LoadXml(xml);

                // Mark 'hot' attribute to '1' if 'sell10days' is greater than 'InStore'
                var xpath = "/products/product[Sell10day>InStore]/@hot";
                var hotAttributes = doc.SelectNodes(xpath);
                for (uint index = 0; index < hotAttributes.Length; index++)
                {
                    hotAttributes.Item(index).NodeValue = "1";
                }

                Scenario.RichEditBoxSetMsg(scenario2Result, doc.GetXml(), true);
                scenario2BtnSave.IsEnabled = true;  // enable Save button
            }
            catch (Exception exp)
            {
                Scenario.RichEditBoxSetError(scenario2Result, exp.Message);
            }
        }

        /// <summary>
        /// This is the click handler for the 'Scenario2BtnSave' button.
        /// This function is to save the new xml in which hot products are marked to a file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Scenario2BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String xml;
                scenario2Result.Document.GetText(Windows.UI.Text.TextGetOptions.None, out xml);

                var doc = new Windows.Data.Xml.Dom.XmlDocument();
                doc.LoadXml(xml);

                // save xml to a file
                var file = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("HotProducts.xml", Windows.Storage.CreationCollisionOption.GenerateUniqueName);
                await doc.SaveToFileAsync(file);

                Scenario.RichEditBoxSetMsg(scenario2Result, "Save to \"" + file.Path + "\" successfully.", true);
            }
            catch (Exception exp)
            {
                Scenario.RichEditBoxSetError(scenario2Result, exp.Message);
            }

            scenario2BtnSave.IsEnabled = false;
        }

        private async void Scenario2Init()
        {
            try
            {
                Windows.Data.Xml.Dom.XmlDocument doc = await Scenario.LoadXmlFile("markHotProducts", "products.xml");
                Scenario.RichEditBoxSetMsg(scenario2OriginalData, doc.GetXml(), true);
            }
            catch
            {
                Scenario.RichEditBoxSetError(scenario2Result, Scenario.LoadFileErrorMsg);
            }
        }

    }
}
