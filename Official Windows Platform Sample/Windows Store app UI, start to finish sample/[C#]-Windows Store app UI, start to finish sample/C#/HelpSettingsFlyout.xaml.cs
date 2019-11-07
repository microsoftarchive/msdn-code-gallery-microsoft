using AppUIBasics.Data;
//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace AppUIBasics
{
    public sealed partial class HelpSettingsFlyout : SettingsFlyout
    {
        public HelpSettingsFlyout()
        {
            this.InitializeComponent();
            Loaded += AboutFlyout_Loaded;
        }

        void AboutFlyout_Loaded(object sender, RoutedEventArgs e)
        {
            ContentWebView.Height = this.ActualHeight - 180;

            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();

            string HTMLOpenTags = loader.GetString("HTMLOpenTags");
            string HTMLCloseTags = loader.GetString("HTMLCloseTags");

            string contentString = string.Empty;

            if (this.DataContext != null)
            {
                ControlInfoDataItem item = this.DataContext as ControlInfoDataItem;
                if (item != null)
                {
                    contentString = item.Content.ToString();
                    this.IconSource = new BitmapImage(new Uri(item.ImagePath));
                }
            }
            else
            {
                contentString = loader.GetString("helpFlyoutContent");
            }

            ContentWebView.NavigateToString(HTMLOpenTags + contentString + HTMLCloseTags);
        }
    }
}
