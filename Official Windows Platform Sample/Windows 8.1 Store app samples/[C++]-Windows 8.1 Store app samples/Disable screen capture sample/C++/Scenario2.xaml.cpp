//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario2.xaml.cpp
// Implementation of the Scenario2 class
//

#include "pch.h"
#include "Scenario2.xaml.h"

using namespace SDKSample::DisablingScreenCapture;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario2::Scenario2()
{
    InitializeComponent();

    //Setting the ApplicationView property IsScreenCaptureEnabled to false will prevent screen capture
    Windows::UI::ViewManagement::ApplicationView::GetForCurrentView()->IsScreenCaptureEnabled = false;
}
