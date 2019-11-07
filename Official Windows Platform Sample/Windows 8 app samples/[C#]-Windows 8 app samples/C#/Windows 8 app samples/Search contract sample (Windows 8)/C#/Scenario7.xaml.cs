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

using Windows.ApplicationModel.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SearchContract
{
    public sealed partial class Scenario7 : SDKTemplate.Common.LayoutAwarePage
    {
        public Scenario7()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Turn on type to search.
            SearchPane.GetForCurrentView().ShowOnKeyboardInput = true;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Turn off type to search.
            SearchPane.GetForCurrentView().ShowOnKeyboardInput = false;
        }
    }
}
