//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using NotificationsExtensions.BadgeContent;
using SDKTemplate;
using System;
using System.Collections.ObjectModel;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Tiles
{
    public sealed partial class SendBadge : Page
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
        #endregion TemplateCode

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
            if ((Button)sender == UpdateBadgeWithStringManipulation)
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
                    OutputTextBlock.Text = string.Empty;
                    rootPage.NotifyUser("Please enter a valid number!", NotifyType.ErrorMessage);
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

        void ClearBadge_Click(object sender, RoutedEventArgs e)
        {
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Clear();
            OutputTextBlock.Text = string.Empty;
            rootPage.NotifyUser("Badge cleared", NotifyType.StatusMessage);
        }

        void UpdateBadgeWithNumber(int number)
        {
            // Note: This sample contains an additional project, NotificationsExtensions.
            // NotificationsExtensions exposes an object model for creating notifications, but you can also modify the xml
            // of the notification directly. See the additional function UpdateBadgeWithNumberWithStringManipulation to see how to do it
            // by modifying strings directly.

            BadgeNumericNotificationContent badgeContent = new BadgeNumericNotificationContent((uint)number);

            // Send the notification to the application’s tile.
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badgeContent.CreateNotification());

            OutputTextBlock.Text = badgeContent.GetContent();
            rootPage.NotifyUser("Badge sent", NotifyType.StatusMessage);
        }

        void UpdateBadgeWithGlyph(int index)
        {
            // Note: This sample contains an additional project, NotificationsExtensions.
            // NotificationsExtensions exposes an object model for creating notifications, but you can also modify the xml
            // of the notification directly. See the additional function UpdateBadgeWithGlyphWithStringManipulation to see how to do it
            // by modifying strings directly.

            // Note: usually this would be created with new BadgeGlyphNotificationContent(GlyphValue.Alert) or any of the values of GlyphValue.
            BadgeGlyphNotificationContent badgeContent = new BadgeGlyphNotificationContent((GlyphValue)index);

            // Send the notification to the application’s tile.
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badgeContent.CreateNotification());

            OutputTextBlock.Text = badgeContent.GetContent();
            rootPage.NotifyUser("Badge sent", NotifyType.StatusMessage);
        }

        void UpdateBadgeWithNumberWithStringManipulation(int number)
        {
            // Create a string with the badge template xml.
            string badgeXmlString = "<badge value='" + number + "'/>";
            Windows.Data.Xml.Dom.XmlDocument badgeDOM = new Windows.Data.Xml.Dom.XmlDocument();
            try
            {
                // Create a DOM.
                badgeDOM.LoadXml(badgeXmlString);

                // Load the xml string into the DOM, catching any invalid xml characters.
                BadgeNotification badge = new BadgeNotification(badgeDOM);

                // Create a badge notification and send it to the application’s tile.
                BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badge);

                OutputTextBlock.Text = badgeDOM.GetXml();
                rootPage.NotifyUser("Badge sent", NotifyType.StatusMessage);
            }
            catch (Exception)
            {
                OutputTextBlock.Text = string.Empty;
                rootPage.NotifyUser("Error loading the xml, check for invalid characters in the input", NotifyType.ErrorMessage);
            }
        }

        void UpdateBadgeWithGlyphWithStringManipulation()
        {
            // Create a string with the badge template xml.
            string badgeXmlString = "<badge value='" + ((TileGlyph)GlyphList.SelectedItem).Name.ToString() + "'/>";
            Windows.Data.Xml.Dom.XmlDocument badgeDOM = new Windows.Data.Xml.Dom.XmlDocument();
            try
            {
                // Create a DOM.
                badgeDOM.LoadXml(badgeXmlString);

                // Load the xml string into the DOM, catching any invalid xml characters.
                BadgeNotification badge = new BadgeNotification(badgeDOM);

                // Create a badge notification and send it to the application’s tile.
                BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badge);

                OutputTextBlock.Text = badgeDOM.GetXml();
                rootPage.NotifyUser("Badge sent", NotifyType.StatusMessage);
            }
            catch (Exception)
            {
                OutputTextBlock.Text = string.Empty;
                rootPage.NotifyUser("Error loading the xml, check for invalid characters in the input", NotifyType.ErrorMessage);
            }
        }
    }

    public class TileGlyph
    {
        public string Name { get; private set; }
        public bool IsAvailable { get; private set; }
        public TileGlyph(string name, bool isAvailable)
        {
            this.Name = name;
            this.IsAvailable = isAvailable;
        }
        public override string ToString()
        {
            return Name;
        }
    }

    public class TileGlyphCollection : ObservableCollection<TileGlyph>
    {
        public TileGlyphCollection()
        {
            // Some glyphs are only available on Windows
#if WINDOWS_PHONE_APP
            const bool windows = false;
            const bool phone = true;
#else
            const bool windows = true;
            const bool phone = false;
#endif

            Add(new TileGlyph("none", windows | phone));
            Add(new TileGlyph("activity", windows));
            Add(new TileGlyph("alert", windows | phone));
            Add(new TileGlyph("available", windows));
            Add(new TileGlyph("away", windows));
            Add(new TileGlyph("busy", windows));
            Add(new TileGlyph("newMessage", windows));
            Add(new TileGlyph("paused", windows));
            Add(new TileGlyph("playing", windows));
            Add(new TileGlyph("unavailable", windows));
            Add(new TileGlyph("error", windows));
            Add(new TileGlyph("attention", windows | phone));
            Add(new TileGlyph("alarm", windows));
        }
    }
}