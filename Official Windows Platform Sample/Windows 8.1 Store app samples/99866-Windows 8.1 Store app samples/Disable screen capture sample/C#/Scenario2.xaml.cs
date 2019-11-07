//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
using SDKTemplate;
using System;

namespace DisablingScreenCapture
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario2 : SDKTemplate.Common.LayoutAwarePage
    {
        public Scenario2()
        {
            this.InitializeComponent();
            
            //Setting the ApplicationView property IsScreenCaptureEnabled to false will prevent screen capture
            ApplicationView.GetForCurrentView().IsScreenCaptureEnabled = false;
        }
    }
}
