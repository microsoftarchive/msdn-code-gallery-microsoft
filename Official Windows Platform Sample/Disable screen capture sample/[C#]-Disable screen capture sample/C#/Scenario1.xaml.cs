//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using SDKTemplate;
using System;


namespace DisablingScreenCapture
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    ///
    public sealed partial class Scenario1 : SDKTemplate.Common.LayoutAwarePage
    {
        public Scenario1()
        {
            this.InitializeComponent();

            //Setting the ApplicationView property IsScreenCaptureEnabled to true will allow screen capture.
            //This is the default setting for this property.
            ApplicationView.GetForCurrentView().IsScreenCaptureEnabled = true;
        }
    }
}
