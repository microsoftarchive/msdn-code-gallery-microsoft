//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using SDKTemplate;

using System;
using Windows.ApplicationModel.Search;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SearchContract
{
    public sealed partial class Scenario4 : SDKTemplate.Common.LayoutAwarePage
    {
        public Scenario4()
        {
            this.InitializeComponent();
        }

        private void SetLocalContentSuggestions(bool isLocal)
        {
            // Have Windows provide suggestions from local files.
            // This code should be placed in your apps global scope and run as soon as your app is launched.
            var settings = new LocalContentSuggestionSettings();
            settings.Enabled = isLocal;
            if (isLocal)
            {
                settings.Locations.Add(KnownFolders.MusicLibrary);
                settings.AqsFilter = "kind:Music";
            }
            SearchPane.GetForCurrentView().SetLocalContentSuggestionSettings(settings);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            MainPage.Current.NotifyUser("Use the search pane to submit a query", NotifyType.StatusMessage);
            SetLocalContentSuggestions(true);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            SetLocalContentSuggestions(false);
        }
    }
}
