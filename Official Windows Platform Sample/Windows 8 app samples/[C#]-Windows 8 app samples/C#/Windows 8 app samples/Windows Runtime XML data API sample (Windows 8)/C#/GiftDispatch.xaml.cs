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
using System.Text;

namespace Xml
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GiftDispatch : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public GiftDispatch()
        {
            this.InitializeComponent();
            Scenario4Init();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        async void Scenario4Init()
        {
            try
            {
                Windows.Data.Xml.Dom.XmlDocument doc = await Scenario.LoadXmlFile("giftDispatch", "employees.xml");
                Scenario.RichEditBoxSetMsg(scenario4OriginalData, doc.GetXml(), true);
            }
            catch
            {
                Scenario.RichEditBoxSetError(scenario4Result, Scenario.LoadFileErrorMsg);
            }
        }

        /// <summary>
        /// This is the click handler for the 'Scenario4BtnDefault' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Scenario4BtnDefault_Click(object sender, RoutedEventArgs e)
        {
            String xml;
            scenario4OriginalData.Document.GetText(Windows.UI.Text.TextGetOptions.None, out xml);

            var doc = new Windows.Data.Xml.Dom.XmlDocument();
            doc.LoadXml(xml);

            var thisYear = 2012;    // Here we don't use DateTime.Now.Year to get the current year so that all gifts can be delivered.
            var previousOneYear = thisYear - 1;
            var previousFiveYear = thisYear - 5;
            var previousTenYear = thisYear - 10;

            var xpath = new String[3];
            // select >= 1 year and < 5 years
            xpath[0] = "descendant::employee[startyear <= " + previousOneYear + " and startyear > " + previousFiveYear + "]";
            // select >= 5 years and < 10 years
            xpath[1] = "descendant::employee[startyear <= " + previousFiveYear + " and startyear > " + previousTenYear + "]";
            // select >= 10 years
            xpath[2] = "descendant::employee[startyear <= " + previousTenYear + "]";

            var Gifts = new String[3] { "Gift Card", "XBOX", "Windows Phone" };

            var output = new StringBuilder();
            for (uint i = 0; i < 3; i++)
            {
                var employees = doc.SelectNodes(xpath[i]);
                for (uint index = 0; index < employees.Length; index++)
                {
                    var employeeName = employees.Item(index).SelectSingleNode("descendant::name");
                    var department = employees.Item(index).SelectSingleNode("descendant::department");

                    output.AppendFormat("[{0}]/[{1}]/[{2}]\n", employeeName.FirstChild.NodeValue, department.FirstChild.NodeValue, Gifts[i]);
                }
            }

            Scenario.RichEditBoxSetMsg(scenario4Result, output.ToString(), true);
        }
    }
}
