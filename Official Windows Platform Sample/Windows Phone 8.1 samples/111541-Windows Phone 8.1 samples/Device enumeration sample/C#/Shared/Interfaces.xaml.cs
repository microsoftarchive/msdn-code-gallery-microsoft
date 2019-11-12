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
using Windows.UI.Xaml.Media.Imaging;
using SDKTemplate;
using System;

using Windows.Devices.Enumeration;
using Windows.Devices.Enumeration.Pnp;


namespace DeviceEnumeration
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Interfaces : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Interfaces()
        {
            this.InitializeComponent();
        }

        void InterfaceClasses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InterfaceClasses.SelectedItem == PrinterInterfaceClass)
            {
                InterfaceClassGuid.Text = "{0ECEF634-6EF0-472A-8085-5AD023ECBCCD}";
            }
            else if (InterfaceClasses.SelectedItem == WebcamInterfaceClass)
            {
                InterfaceClassGuid.Text = "{E5323777-F976-4F5B-9B55-B94699C46E44}";
            }
            else if (InterfaceClasses.SelectedItem == WpdInterfaceClass)
            {
                InterfaceClassGuid.Text = "{6AC27878-A6FA-4155-BA85-F98F491D4F33}";
            }
        }

        async void EnumerateDeviceInterfaces(object sender, RoutedEventArgs eventArgs)
        {
            FocusState focusState = EnumerateInterfacesButton.FocusState;
            InterfaceClassGuid.IsTabStop = false;
            EnumerateInterfacesButton.IsEnabled = false;

            DeviceInterfacesOutputList.Items.Clear();

            try
            {
                var selector = "System.Devices.InterfaceClassGuid:=\"" + InterfaceClassGuid.Text + "\"";
                var interfaces = await DeviceInformation.FindAllAsync(selector, null);

                rootPage.NotifyUser(interfaces.Count + " device interface(s) found\n\n", NotifyType.StatusMessage);
                foreach (DeviceInformation deviceInterface in interfaces)
                {
                    DeviceThumbnail thumbnail = await deviceInterface.GetThumbnailAsync();
                    DeviceThumbnail glyph = await deviceInterface.GetGlyphThumbnailAsync();
                    DeviceInterfacesOutputList.Items.Add(new DisplayItem(deviceInterface, thumbnail, glyph));
                }
            }
            catch (ArgumentException)
            {
                //The ArgumentException gets thrown by FindAllAsync when the GUID isn't formatted properly
                //The only reason we're catching it here is because the user is allowed to enter GUIDs without validation
                //In normal usage of the API, this exception handling probably wouldn't be necessary when using known-good GUIDs
                rootPage.NotifyUser("Caught ArgumentException. Verify that you've entered a valid interface class GUID.", NotifyType.ErrorMessage);
            }

            EnumerateInterfacesButton.IsEnabled = true;
            EnumerateInterfacesButton.Focus(focusState);
            InterfaceClassGuid.IsTabStop = true;
        }

        class DisplayItem
        {
            public DisplayItem(DeviceInformation deviceInterface, DeviceThumbnail thumbnail, DeviceThumbnail glyph)
            {
                name = (string)deviceInterface.Properties["System.ItemNameDisplay"];

                id = "ID: " + deviceInterface.Id;
                isEnabled = "IsEnabled: " + deviceInterface.IsEnabled;
                thumb = thumbnail;
                glyphThumb = glyph;
            }

            public string Name { get { return name; } }
            public string Id { get { return id; } }
            public string IsEnabled { get { return isEnabled; } }            
            public BitmapImage Thumbnail { 
                get {
                    var bmp = new BitmapImage();
                    bmp.SetSource(thumb);
                    return bmp;
                }
            }

            public BitmapImage GlyphThumbnail
            {
                get
                {
                    var bmp = new BitmapImage();
                    bmp.SetSource(glyphThumb);
                    return bmp;
                }
            }

            readonly string name;
            readonly string id;
            readonly string isEnabled;
            readonly DeviceThumbnail thumb;
            readonly DeviceThumbnail glyphThumb;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}
