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
using Windows.UI.Notifications;
using NotificationsExtensions.BadgeContent;

namespace Tiles
{
    public sealed partial class SendBadge : SDKTemplate.Common.LayoutAwarePage
    {
        #region TemplateCode
        MainPage rootPage = MainPage.Current;

        public SendBadge()
        {
            this.InitializeComponent();
            NumberOrGlyph.SelectedIndex = 0;
            GlyphList.SelectedIndex = 0;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        void NumberOrGlyph_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NumberOrGlyph.SelectedIndex == 0)
            {
                NumberPanel.Visibility = Visibility.Visible;
                GlyphPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                NumberPanel.Visibility = Visibility.Collapsed;
                GlyphPanel.Visibility = Visibility.Visible;
            }
        }

        void UpdateBadge_Click(object sender, RoutedEventArgs e)
        {
            bool useStrings = false;
            if (sender == UpdateBadgeWithStringManipulation)
            {
                useStrings = true;
            }

            if (NumberOrGlyph.SelectedIndex == 0)
            {
                int number;
                if (Int32.TryParse(NumberInput.Text, out number))
                {
                    if (useStrings)
                    {
                        UpdateBadgeWithNumberWithStringManipulation(number);
                    }
                    else
                    {
                        UpdateBadgeWithNumber(number);
                    }
                }
                else
                {
                    OutputTextBlock.Text = "Please enter a valid number!";
                }
            }
            else
            {
                if (useStrings)
                {
                    UpdateBadgeWithGlyphWithStringManipulation();
                }
                else
                {
                    UpdateBadgeWithGlyph(GlyphList.SelectedIndex);
                }
            }
        }
        #endregion TemplateCode

        void ClearBadge_Click(object sender, RoutedEventArgs e)
        {
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Clear();
            OutputTextBlock.Text = "Badge cleared";
        }

        void UpdateBadgeWithNumber(int number)
        {
            // Note: This sample contains an additional project, NotificationsExtensions.
            // NotificationsExtensions exposes an object model for creating notifications, but you can also modify the xml
            // of the notification directly. See the additional function UpdateBadgeWithNumberWithStringManipulation to see how to do it
            // by modifying strings directly

            BadgeNumericNotificationContent badgeContent = new BadgeNumericNotificationContent((uint)number);

            // send the notification to the app's application tile
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badgeContent.CreateNotification());

            OutputTextBlock.Text = badgeContent.GetContent();
        }

        void UpdateBadgeWithGlyph(int index)
        {
            // Note: This sample contains an additional project, NotificationsExtensions.
            // NotificationsExtensions exposes an object model for creating notifications, but you can also modify the xml
            // of the notification directly. See the additional function UpdateBadgeWithGlyphWithStringManipulation to see how to do it
            // by modifying strings directly                    

            // Note: usually this would be created with new BadgeGlyphNotificationContent(GlyphValue.Alert) or any of the values of GlyphValue
            BadgeGlyphNotificationContent badgeContent = new BadgeGlyphNotificationContent((GlyphValue)index);

            // send the notification to the app's application tile
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badgeContent.CreateNotification());

            OutputTextBlock.Text = badgeContent.GetContent();
        }      

        void UpdateBadgeWithNumberWithStringManipulation(int number)
        {
            // create a string with the badge template xml
            string badgeXmlString = "<badge value='" + number + "'/>";
            Windows.Data.Xml.Dom.XmlDocument badgeDOM = new Windows.Data.Xml.Dom.XmlDocument();
            try
            {
                // create a DOM
                badgeDOM.LoadXml(badgeXmlString);

                // load the xml string into the DOM, catching any invalid xml characters 
                BadgeNotification badge = new BadgeNotification(badgeDOM);

                // create a badge notification
                BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badge);
                OutputTextBlock.Text = badgeDOM.GetXml();
            }
            catch (Exception)
            {
                OutputTextBlock.Text = "Error loading the xml, check for invalid characters in the input";
            }
        }

        void UpdateBadgeWithGlyphWithStringManipulation()
        {
            // create a string with the badge template xml
            string badgeXmlString = "<badge value='" + ((ComboBoxItem)GlyphList.SelectedItem).Content.ToString() + "'/>";
            Windows.Data.Xml.Dom.XmlDocument badgeDOM = new Windows.Data.Xml.Dom.XmlDocument();
            try
            {
                // create a DOM
                badgeDOM.LoadXml(badgeXmlString);

                // load the xml string into the DOM, catching any invalid xml characters 
                BadgeNotification badge = new BadgeNotification(badgeDOM);

                // create a badge notification
                BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badge);

                OutputTextBlock.Text = badgeDOM.GetXml();
            }
            catch (Exception)
            {
                OutputTextBlock.Text = "Error loading the xml, check for invalid characters in the input";
            }
        }
    }
}
