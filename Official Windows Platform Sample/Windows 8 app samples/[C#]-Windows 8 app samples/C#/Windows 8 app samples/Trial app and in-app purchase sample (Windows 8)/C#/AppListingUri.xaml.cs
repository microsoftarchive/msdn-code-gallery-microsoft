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
using System.Threading.Tasks;
using SDKTemplate;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace Store
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppListingUri : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        LicenseChangedEventHandler licenseChangeHandler = null;

        public AppListingUri()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await LoadAppListingUriProxyFileAsync();
        }

        /// <summary>
        /// Invoked when this page is about to unload
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (licenseChangeHandler != null)
            {
                CurrentAppSimulator.LicenseInformation.LicenseChanged -= licenseChangeHandler;
            }
            base.OnNavigatingFrom(e);
        }

        private async Task LoadAppListingUriProxyFileAsync()
        {
            StorageFolder proxyDataFolder = await Package.Current.InstalledLocation.GetFolderAsync("data");
            StorageFile proxyFile = await proxyDataFolder.GetFileAsync("app-listing-uri.xml");
            licenseChangeHandler = new LicenseChangedEventHandler(AppListingUriRefreshScenario);
            CurrentAppSimulator.LicenseInformation.LicenseChanged += licenseChangeHandler;
            await CurrentAppSimulator.ReloadSimulatorAsync(proxyFile);
        }

        private void AppListingUriRefreshScenario()
        {
        }

        private void DisplayLink_Click(object sender, RoutedEventArgs e)
        {
            rootPage.NotifyUser(CurrentAppSimulator.LinkUri.AbsoluteUri, NotifyType.StatusMessage);
        }
    }
}
