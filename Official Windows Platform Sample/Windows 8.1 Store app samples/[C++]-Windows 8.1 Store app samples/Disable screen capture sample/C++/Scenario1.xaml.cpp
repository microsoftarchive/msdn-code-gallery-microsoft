//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario1.xaml.cpp
// Implementation of the Scenario1 class
//

#include "pch.h"
#include "Scenario1.xaml.h"

using namespace SDKSample::DisablingScreenCapture;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario1::Scenario1()
{
    InitializeComponent();

    //Setting the ApplicationView property IsScreenCaptureEnabled to true will allow screen capture.
    //This is the default setting for this property.

    Windows::UI::ViewManagement::ApplicationView::GetForCurrentView()->IsScreenCaptureEnabled = true;
}
