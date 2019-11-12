//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;

namespace MobileBroadbandComApi
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShowCapability : SDKTemplate.Common.LayoutAwarePage
    {
        private MBApiImplementation mbApiInstance;

        public ShowCapability()
        {
            this.InitializeComponent();

            // Get the MB API instance
            mbApiInstance = MBApiImplementation.GetInstance();

            // Initialize MBN managers
            mbApiInstance.InitializeManagers(); 
        } 
        
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Get the device capabilities
            mbApiInstance.GetDeviceCapability();
        }
    }
}
