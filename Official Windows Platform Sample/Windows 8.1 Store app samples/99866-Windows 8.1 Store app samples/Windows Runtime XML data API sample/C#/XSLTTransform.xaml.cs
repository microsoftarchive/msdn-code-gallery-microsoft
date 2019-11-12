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
using Windows.UI.Xaml.Media;
using SDKTemplate;
using System;

namespace Xml
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class XSLTTransform : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public XSLTTransform()
        {
            this.InitializeComponent();
            Scenario5Init();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        async void XsltInit()
        {
            try
            {
                Windows.Storage.StorageFolder storageFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("xsltTransform");
                Windows.Storage.StorageFile xmlFile = await storageFolder.GetFileAsync("xmlContent.xml");
                Windows.Storage.StorageFile xsltFile = await storageFolder.GetFileAsync("xslContent.xml");

                // load xml file
                Windows.Data.Xml.Dom.XmlDocument doc = await Windows.Data.Xml.Dom.XmlDocument.LoadFromFileAsync(xmlFile);
                Scenario.RichEditBoxSetMsg(scenario5Xml, doc.GetXml(), false);
                
                // load xslt file
                doc = await Windows.Data.Xml.Dom.XmlDocument.LoadFromFileAsync(xsltFile);
                
                // Display xml and xslt file content in the input fields
                Scenario.RichEditBoxSetMsg(scenario5Xslt, doc.GetXml(), false);
                Scenario.RichEditBoxSetMsg(scenario5Result, "", true);
            }
            catch
            {
                Scenario.RichEditBoxSetMsg(scenario5Xml, "", false);
                Scenario.RichEditBoxSetMsg(scenario5Xslt, "", false);
                Scenario.RichEditBoxSetError(scenario5Result, Scenario.LoadFileErrorMsg);
            }
        }

        void Scenario5Init()
        {
            XsltInit();
        }

        /// <summary>
        /// This is the click handler for the 'Scenario5BtnDefault' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Scenario5BtnDefault_Click(object sender, RoutedEventArgs e)
        {
            scenario5Xml.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
            scenario5Xslt.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
            Scenario.RichEditBoxSetMsg(scenario5Result, "", true);

            Windows.Data.Xml.Dom.XmlDocument doc, xsltDoc;
            String xml, xslt;
                
            // Get xml content from xml input field
            scenario5Xml.Document.GetText(Windows.UI.Text.TextGetOptions.None, out xml);
                
            // Get xslt content from xslt input field
            scenario5Xslt.Document.GetText(Windows.UI.Text.TextGetOptions.None, out xslt);

            if (null == xml || "" == xml.Trim())
            {
                Scenario.RichEditBoxSetError(scenario5Result, "Source XML can't be empty");
                return;
            }

            if (null == xslt || "" == xslt.Trim())
            {
                Scenario.RichEditBoxSetError(scenario5Result, "XSL content can't be empty");
                return;
            }

            try
            {
                // Load xml content
                doc = new Windows.Data.Xml.Dom.XmlDocument();
                doc.LoadXml(xml);
            }
            catch (Exception exp)
            {
                scenario5Xml.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                Scenario.RichEditBoxSetError(scenario5Result, exp.Message);
                return;
            }

            try
            {
                // Load xslt content
                xsltDoc = new Windows.Data.Xml.Dom.XmlDocument();
                xsltDoc.LoadXml(xslt);
            }
            catch (Exception exp)
            {
                scenario5Xslt.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                Scenario.RichEditBoxSetError(scenario5Result, exp.Message);
                return;
            }

            try
            {
                // Transform xml according to the style sheet declaration specified in xslt file
                var xsltProcessor = new Windows.Data.Xml.Xsl.XsltProcessor(xsltDoc);
                String transformedStr = xsltProcessor.TransformToString(doc);
                Scenario.RichEditBoxSetMsg(scenario5Result, transformedStr, true);
            }
            catch (Exception exp)
            {
                Scenario.RichEditBoxSetError(scenario5Result, exp.Message);
                return;
            }
        }

    }
}
