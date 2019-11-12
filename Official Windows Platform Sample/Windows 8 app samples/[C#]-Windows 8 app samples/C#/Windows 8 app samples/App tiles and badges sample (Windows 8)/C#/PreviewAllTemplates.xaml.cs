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
using Windows.Data.Xml.Dom;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Pickers;
using System.Collections.Generic;
using Windows.Storage;

namespace Tiles
{
    public sealed partial class PreviewAllTemplates : SDKTemplate.Common.LayoutAwarePage
    {
        #region  TemplateCode
        MainPage rootPage = MainPage.Current;

        public PreviewAllTemplates()
        {
            this.InitializeComponent();
            Branding.SelectedIndex = 0;
            TemplateList.SelectedIndex = 10;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        async void ViewImages_Click(object sender, RoutedEventArgs e)
        {
            if (!AvailableImages.Text.Equals(String.Empty))
            {
                AvailableImages.Text = String.Empty;
                ViewImages.Content = "View local images";
            }
            else
            {
                string output = "ms-appx:///images/bluewide.png \nms-appx:///images/redWide.png \nms-appx:///images/graySquare.png \n";
                IReadOnlyList<StorageFile> files = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFilesAsync();
                foreach (StorageFile file in files)
                {
                    if (file.FileType.Equals(".png") || file.FileType.Equals(".jpg") || file.FileType.Equals(".jpeg") || file.FileType.Equals(".gif"))
                    {
                        output += "ms-appdata:///local/" + file.Name + " \n";
                    }
                }
                ViewImages.Content = "Hide local images";
                AvailableImages.Text = output;
            }
        }

        async void CopyImages_Click(object sender, RoutedEventArgs e)
        {
            if (rootPage.EnsureUnsnapped())
            {
                FileOpenPicker picker = new Windows.Storage.Pickers.FileOpenPicker();
                picker.ViewMode = PickerViewMode.Thumbnail;
                picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                picker.FileTypeFilter.Add(".jpg");
                picker.FileTypeFilter.Add(".jpeg");
                picker.FileTypeFilter.Add(".png");
                picker.FileTypeFilter.Add(".gif");
                picker.CommitButtonText = "Copy";
                IReadOnlyList<StorageFile> files = await picker.PickMultipleFilesAsync();
                OutputTextBlock.Text = "Image(s) copied to application data local storage: \n";
                foreach (StorageFile file in files)
                {
                    StorageFile copyFile = await file.CopyAsync(Windows.Storage.ApplicationData.Current.LocalFolder, file.Name, Windows.Storage.NameCollisionOption.GenerateUniqueName);
                    OutputTextBlock.Text += copyFile.Path + "\n ";
                }
            }
            else
            {
                OutputTextBlock.Text = "Cannot unsnap the sample application.";
            }
        }

        private void TemplateList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = TemplateList.SelectedItem as ComboBoxItem;
            string templateName = (item != null) ? item.Name : "TileSquareImage";
            XmlDocument tileXml = TileUpdateManager.GetTemplateContent((TileTemplateType)TemplateList.SelectedIndex);

            XmlNodeList tileTextAttributes = tileXml.GetElementsByTagName("text");
            for (int i = 0; i < TextInputs.Children.Count; i++)
            {
                if (i < tileTextAttributes.Length)
                {
                    TextInputs.Children[i].Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else
                {
                    TextInputs.Children[i].Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
            }

            XmlNodeList tileImageAttributes = tileXml.GetElementsByTagName("image");
            for (int i = 0; i < ImageInputs.Children.Count; i++)
            {
                if (i < tileImageAttributes.Length)
                {
                    ImageInputs.Children[i].Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else
                {
                    ImageInputs.Children[i].Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
            }

            Preview.Source = new BitmapImage(new Uri("ms-appx:///images/tiles/" + templateName + ".png"));
        }
        #endregion TemplateCode

        void ClearTile_Click(object sender, RoutedEventArgs e)
        {
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            OutputTextBlock.Text = "Tile cleared";
        }

        void UpdateTileNotification_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem item = (ComboBoxItem)(Branding.SelectedItem);
            string branding = (item.Name.Equals("BrandName")) ? "Name" : item.Name;

            UpdateTile((TileTemplateType)TemplateList.SelectedIndex, branding);
        }

        void UpdateTile(TileTemplateType templateType, string branding)
        {
            // This example uses the GetTemplateContent method to get the notification as xml instead of NotificationExtensions            

            XmlDocument tileXml = TileUpdateManager.GetTemplateContent(templateType);
            #region input handling

            XmlNodeList textElements = tileXml.GetElementsByTagName("text");
            for (int i = 0; i < textElements.Length; i++)
            {
                string tileText = String.Empty;
                StackPanel panel = TextInputs.Children[i] as StackPanel;
                if (panel != null)
                {
                    TextBox box = panel.Children[1] as TextBox;
                    if (box != null)
                    {
                        tileText = box.Text;
                        if (tileText.Equals(""))
                        {
                            tileText = "Text field " + i;
                        }
                    }
                }
                textElements.Item((uint)i).AppendChild(tileXml.CreateTextNode(tileText));
            }

            XmlNodeList imageElements = tileXml.GetElementsByTagName("image");
            for (int i = 0; i < imageElements.Length; i++)
            {
                XmlElement imageElement = (XmlElement)imageElements.Item((uint)i);
                string imageSource = String.Empty;
                StackPanel panel = ImageInputs.Children[i] as StackPanel;
                if (panel != null)
                {
                    TextBox box = panel.Children[1] as TextBox;
                    if (box != null)
                    {
                        imageSource = box.Text;
                        if (imageSource.Equals(String.Empty))
                        {
                            imageSource = "ms-appx:///images/redWide.png";
                        }
                    }
                }
                imageElement.SetAttribute("src", imageSource);
            }
            #endregion

            // Set the branding on the notification as specified in the input
            // The logo and display name are declared in the manifest
            // Branding defaults to logo if omitted
            XmlElement bindingElement = (XmlElement)tileXml.GetElementsByTagName("binding").Item(0);
            bindingElement.SetAttribute("branding", branding);

            // optionally, set the language of the notification
            string lang = Lang.Text; // this needs to be a BCP47 tag
            if (!String.IsNullOrEmpty(lang))
            {
                // specify the language of the text in the notification 
                // this ensure the correct font is used to render the text
                XmlElement visualElement = (XmlElement)tileXml.GetElementsByTagName("visual").Item(0);
                visualElement.SetAttribute("lang", lang);
            }

            TileNotification tile = new TileNotification(tileXml);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tile);

            OutputTextBlock.Text = MainPage.PrettyPrint(tileXml.GetXml());
        }
    }
}