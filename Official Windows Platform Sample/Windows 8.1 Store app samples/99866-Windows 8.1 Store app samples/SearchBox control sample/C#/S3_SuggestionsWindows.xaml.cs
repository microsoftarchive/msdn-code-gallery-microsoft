//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************
using SDKTemplate;
using System;
using Windows.ApplicationModel.Search;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SearchBox
{
    /// <summary>
    /// Provides local file suggestions to the SearchBox
    /// </summary>
    public sealed partial class S3_SuggestionsWindows : SDKTemplate.Common.LayoutAwarePage
    {
        public S3_SuggestionsWindows()
        {
            this.InitializeComponent();
        }

        private void SetLocalContentSuggestions(bool isLocal)
        {
            // Have Windows provide suggestions from local files.
            // This code should be placed in your app's global scope and run as soon as your app is launched.
            var settings = new LocalContentSuggestionSettings();
            settings.Enabled = isLocal;
            if (isLocal)
            {
                settings.Locations.Add(KnownFolders.MusicLibrary);
                settings.AqsFilter = "kind:Music";
            }
            SearchBoxSuggestions.SetLocalContentSuggestionSettings(settings);
        }

        /// <summary>
        /// Called when query submitted in SearchBox
        /// </summary>
        /// <param name="sender">The Xaml SearchBox</param>
        /// <param name="e">Event when user submits query</param>
        private void SearchBoxEventsQuerySubmitted(object sender, SearchBoxQuerySubmittedEventArgs e)
        {
            MainPage.Current.NotifyUser(e.QueryText, NotifyType.StatusMessage);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SetLocalContentSuggestions(true);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            SetLocalContentSuggestions(false);
        }
    }
}
