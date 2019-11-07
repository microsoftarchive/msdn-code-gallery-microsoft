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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Tiles
{
#if WINDOWS_PHONE_APP
    public sealed partial class PreviewAllTemplates : Page, IFileOpenPickerContinuable
#else
    public sealed partial class PreviewAllTemplates : Page
#endif
    {
        #region TemplateCode
        MainPage rootPage = MainPage.Current;
        ResourceLoader previewTileImageDescriptions = ResourceLoader.GetForCurrentView();

        public PreviewAllTemplates()
        {
            this.InitializeComponent();
            Branding.SelectedIndex = 0;
            TemplateList.SelectedIndex = 10;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

#if WINDOWS_PHONE_APP
        public async void ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs args)
        {
            await CopyImageToLocalFolderAsync(args.Files);
        }
#endif
        #endregion TemplateCode

        private async void ViewImages_Click(object sender, RoutedEventArgs e)
        {
            if (!AvailableImages.Text.Equals(String.Empty))
            {
                AvailableImages.Text = String.Empty;
                ViewImages.Content = "View local images";
            }
            else
            {
                string output = "ms-appx:///images/purpleSquare310x310.png \nms-appx:///images/blueWide310x150.png \nms-appx:///images/redWide310x150.png \nms-appx:///images/graySquare150x150.png \nms-appx:///images/orangeIcon.png \n";
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

#if WINDOWS_PHONE_APP
        private void CopyImages_Click(object sender, RoutedEventArgs e)
#else
        private async void CopyImages_Click(object sender, RoutedEventArgs e)
#endif
        {
            FileOpenPicker picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".gif");
            picker.CommitButtonText = "Copy";

#if WINDOWS_PHONE_APP
            picker.PickSingleFileAndContinue();
#else
            IReadOnlyList<StorageFile> files = await picker.PickMultipleFilesAsync();
            await CopyImageToLocalFolderAsync(files);
#endif
        }

        private void TemplateList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TileTemplate tileTemplate = TemplateList.SelectedItem as TileTemplate;
            string templateName = (tileTemplate != null) ? tileTemplate.Name : "TileSquare150x150Image";
            XmlDocument tileXml = TileUpdateManager.GetTemplateContent((TileTemplateType)TemplateList.SelectedIndex);

            OutputTextBlock.Text = MainPage.PrettyPrint(tileXml.GetXml().ToString());

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

            // Show any available description against the preview tile.
            if (!String.IsNullOrEmpty(TilePreviewDescription.Text = previewTileImageDescriptions.GetString(templateName)))
            {
                TilePreviewDescription.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                TilePreviewDescription.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }

#if WINDOWS_PHONE_APP
            // Iconic templates are designed to handle the badge differently.
            if ((TileTemplateType)TemplateList.SelectedIndex == TileTemplateType.TileSquare71x71IconWithBadge
                || (TileTemplateType)TemplateList.SelectedIndex == TileTemplateType.TileSquare150x150IconWithBadge
                || (TileTemplateType)TemplateList.SelectedIndex == TileTemplateType.TileWide310x150IconWithBadgeAndText)
            {
                SendBadge.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                SendBadge.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
#endif
        }

        void ClearTile_Click(object sender, RoutedEventArgs e)
        {
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            OutputTextBlock.Text = string.Empty;
            rootPage.NotifyUser("Tile cleared", NotifyType.StatusMessage);
        }

        void UpdateTileNotification_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem item = (ComboBoxItem)(Branding.SelectedItem);
            string branding = (item.Name.Equals("BrandName")) ? "Name" : item.Name;

            UpdateTile((TileTemplateType)TemplateList.SelectedIndex, branding);
        }

        void UpdateTile(TileTemplateType templateType, string branding)
        {
            // This example uses the GetTemplateContent method to get the notification as xml instead of using NotificationExtensions.

            XmlDocument tileXml = TileUpdateManager.GetTemplateContent(templateType);

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
                        if (String.IsNullOrEmpty(tileText))
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
                        if (String.IsNullOrEmpty(imageSource))
                        {
                            imageSource = "ms-appx:///images/redWide310x150.png";
                        }
                    }
                }
                imageElement.SetAttribute("src", imageSource);
            }

            // Set the branding on the notification as specified in the input.
            // The logo and display name are declared in the manifest.
            // Branding defaults to logo if omitted.
            XmlElement bindingElement = (XmlElement)tileXml.GetElementsByTagName("binding").Item(0);
            bindingElement.SetAttribute("branding", branding);

            // Set the language of the notification. Though this is optional, it is recommended to specify the language.
            string lang = Lang.Text; // this needs to be a BCP47 tag
            if (!String.IsNullOrEmpty(lang))
            {
                // Specify the language of the text in the notification.
                // This ensures the correct font is used to render the text.
                XmlElement visualElement = (XmlElement)tileXml.GetElementsByTagName("visual").Item(0);
                visualElement.SetAttribute("lang", lang);
            }

            TileNotification tile = new TileNotification(tileXml);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tile);

            OutputTextBlock.Text = MainPage.PrettyPrint(tileXml.GetXml());
            rootPage.NotifyUser("Tile notification sent", NotifyType.StatusMessage);
        }

        private async Task CopyImageToLocalFolderAsync(IReadOnlyList<StorageFile> files)
        {
            if (files.Count > 0)
            {
                OutputTextBlock.Text = "Image(s) copied to application data local storage: \n";
                foreach (StorageFile file in files)
                {
                    StorageFile copyFile = await file.CopyAsync(Windows.Storage.ApplicationData.Current.LocalFolder, file.Name, Windows.Storage.NameCollisionOption.GenerateUniqueName);
                    OutputTextBlock.Text += copyFile.Path + "\n ";
                }
                rootPage.NotifyUser("Image(s) copied to application data local storage.", NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser("Operation cancelled.", NotifyType.ErrorMessage);
            }
        }

        private void SendBadge_Click(object sender, RoutedEventArgs e)
        {
            BadgeNumericNotificationContent badgeContent = new BadgeNumericNotificationContent(81);
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badgeContent.CreateNotification());

            OutputTextBlock.Text = badgeContent.GetContent();
            rootPage.NotifyUser("Badge sent", NotifyType.StatusMessage);
        }

        private void ClearBadge_Click(object sender, RoutedEventArgs e)
        {
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Clear();
            OutputTextBlock.Text = string.Empty;
            rootPage.NotifyUser("Badge cleared", NotifyType.StatusMessage);
        }
    }

    public class TileTemplate
    {
        public string Name { get; private set; }
        public bool IsAvailable { get; private set; }
        public TileTemplate(string name, bool isAvailable)
        {
            this.Name = name;
            this.IsAvailable = isAvailable;
        }
        public override string ToString()
        {
            return Name;
        }
    }

    public class TileTemplateCollection : ObservableCollection<TileTemplate>
    {
        public TileTemplateCollection()
        {
            // Some tile templates are only available on Windows, and some only on WindowsPhone.
#if WINDOWS_PHONE_APP
            const bool windows = false;
            const bool phone = true;
#else
            const bool windows = true;
            const bool phone = false;
#endif

            Add(new TileTemplate("TileSquare150x150Image", windows | phone));
            Add(new TileTemplate("TileSquare150x150Block", windows | phone));
            Add(new TileTemplate("TileSquare150x150Text01", windows | phone));
            Add(new TileTemplate("TileSquare150x150Text02", windows | phone));
            Add(new TileTemplate("TileSquare150x150Text03", windows | phone));
            Add(new TileTemplate("TileSquare150x150Text04", windows | phone));
            Add(new TileTemplate("TileSquare150x150PeekImageAndText01", windows | phone));
            Add(new TileTemplate("TileSquare150x150PeekImageAndText02", windows | phone));
            Add(new TileTemplate("TileSquare150x150PeekImageAndText03", windows | phone));
            Add(new TileTemplate("TileSquare150x150PeekImageAndText04", windows | phone));
            Add(new TileTemplate("TileWide310x150Image", windows | phone));
            Add(new TileTemplate("TileWide310x150ImageCollection", windows | phone));
            Add(new TileTemplate("TileWide310x150ImageAndText01", windows | phone));
            Add(new TileTemplate("TileWide310x150ImageAndText02", windows | phone));
            Add(new TileTemplate("TileWide310x150BlockAndText01", windows | phone));
            Add(new TileTemplate("TileWide310x150BlockAndText02", windows | phone));
            Add(new TileTemplate("TileWide310x150PeekImageCollection01", windows | phone));
            Add(new TileTemplate("TileWide310x150PeekImageCollection02", windows | phone));
            Add(new TileTemplate("TileWide310x150PeekImageCollection03", windows | phone));
            Add(new TileTemplate("TileWide310x150PeekImageCollection04", windows | phone));
            Add(new TileTemplate("TileWide310x150PeekImageCollection05", windows | phone));
            Add(new TileTemplate("TileWide310x150PeekImageCollection06", windows | phone));
            Add(new TileTemplate("TileWide310x150PeekImageAndText01", windows | phone));
            Add(new TileTemplate("TileWide310x150PeekImageAndText02", windows | phone));
            Add(new TileTemplate("TileWide310x150PeekImage01", windows | phone));
            Add(new TileTemplate("TileWide310x150PeekImage02", windows | phone));
            Add(new TileTemplate("TileWide310x150PeekImage03", windows | phone));
            Add(new TileTemplate("TileWide310x150PeekImage04", windows | phone));
            Add(new TileTemplate("TileWide310x150PeekImage05", windows | phone));
            Add(new TileTemplate("TileWide310x150PeekImage06", windows | phone));
            Add(new TileTemplate("TileWide310x150SmallImageAndText01", windows | phone));
            Add(new TileTemplate("TileWide310x150SmallImageAndText02", windows | phone));
            Add(new TileTemplate("TileWide310x150SmallImageAndText03", windows | phone));
            Add(new TileTemplate("TileWide310x150SmallImageAndText04", windows | phone));
            Add(new TileTemplate("TileWide310x150SmallImageAndText05", windows | phone));
            Add(new TileTemplate("TileWide310x150Text01", windows | phone));
            Add(new TileTemplate("TileWide310x150Text02", windows));
            Add(new TileTemplate("TileWide310x150Text03", windows | phone));
            Add(new TileTemplate("TileWide310x150Text04", windows | phone));
            Add(new TileTemplate("TileWide310x150Text05", windows | phone));
            Add(new TileTemplate("TileWide310x150Text06", windows));
            Add(new TileTemplate("TileWide310x150Text07", windows));
            Add(new TileTemplate("TileWide310x150Text08", windows));
            Add(new TileTemplate("TileWide310x150Text09", windows | phone));
            Add(new TileTemplate("TileWide310x150Text10", windows));
            Add(new TileTemplate("TileWide310x150Text11", windows));
            Add(new TileTemplate("TileSquare310x310BlockAndText01", windows));
            Add(new TileTemplate("TileSquare310x310BlockAndText02", windows));
            Add(new TileTemplate("TileSquare310x310Image", windows));
            Add(new TileTemplate("TileSquare310x310ImageAndText01", windows));
            Add(new TileTemplate("TileSquare310x310ImageAndText02", windows));
            Add(new TileTemplate("TileSquare310x310ImageAndTextOverlay01", windows));
            Add(new TileTemplate("TileSquare310x310ImageAndTextOverlay02", windows));
            Add(new TileTemplate("TileSquare310x310ImageAndTextOverlay03", windows));
            Add(new TileTemplate("TileSquare310x310ImageCollectionAndText01", windows));
            Add(new TileTemplate("TileSquare310x310ImageCollectionAndText02", windows));
            Add(new TileTemplate("TileSquare310x310ImageCollection", windows));
            Add(new TileTemplate("TileSquare310x310SmallImagesAndTextList01", windows));
            Add(new TileTemplate("TileSquare310x310SmallImagesAndTextList02", windows));
            Add(new TileTemplate("TileSquare310x310SmallImagesAndTextList03", windows));
            Add(new TileTemplate("TileSquare310x310SmallImagesAndTextList04", windows));
            Add(new TileTemplate("TileSquare310x310Text01", windows));
            Add(new TileTemplate("TileSquare310x310Text02", windows));
            Add(new TileTemplate("TileSquare310x310Text03", windows));
            Add(new TileTemplate("TileSquare310x310Text04", windows));
            Add(new TileTemplate("TileSquare310x310Text05", windows));
            Add(new TileTemplate("TileSquare310x310Text06", windows));
            Add(new TileTemplate("TileSquare310x310Text07", windows));
            Add(new TileTemplate("TileSquare310x310Text08", windows));
            Add(new TileTemplate("TileSquare310x310TextList01", windows));
            Add(new TileTemplate("TileSquare310x310TextList02", windows));
            Add(new TileTemplate("TileSquare310x310TextList03", windows));
            Add(new TileTemplate("TileSquare310x310SmallImageAndText01", windows));
            Add(new TileTemplate("TileSquare310x310SmallImagesAndTextList05", windows));
            Add(new TileTemplate("TileSquare310x310Text09", windows));
            Add(new TileTemplate("TileSquare71x71IconWithBadge", phone));
            Add(new TileTemplate("TileSquare150x150IconWithBadge", phone));
            Add(new TileTemplate("TileWide310x150IconWithBadgeAndText", phone));
            Add(new TileTemplate("TileSquare71x71Image", phone));
        }
    }

    public class IsAvailableStyleSelector : StyleSelector
    {
        public Style DefaultStyle { get; set; }
        public Style UnavailableStyle { get; set; }

        protected override Windows.UI.Xaml.Style SelectStyleCore(object item, Windows.UI.Xaml.DependencyObject container)
        {
            if (item is TileTemplate)
            {
                TileTemplate tile = item as TileTemplate;
                if (tile.IsAvailable)
                {
                    return DefaultStyle;
                }
                else
                {
                    return UnavailableStyle;
                }
            }
            else if (item is TileGlyph)
            {
                TileGlyph glyph = item as TileGlyph;
                if (glyph.IsAvailable)
                {
                    return DefaultStyle;
                }
                else
                {
                    return UnavailableStyle;
                }
            }
            else
            {
                return DefaultStyle;
            }
        }
    }

}