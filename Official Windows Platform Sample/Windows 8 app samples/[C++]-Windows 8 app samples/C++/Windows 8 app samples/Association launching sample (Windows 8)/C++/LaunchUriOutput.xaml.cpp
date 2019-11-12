// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// LaunchUriOutput.xaml.cpp
// Implementation of the LaunchUriOutput class
//

#include "pch.h"
#include "LaunchUriOutput.xaml.h"
#include "MainPage.xaml.h"

using namespace AssociationLaunching;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

LaunchUriOutput::LaunchUriOutput()
{
    InitializeComponent();
}

LaunchUriOutput::~LaunchUriOutput()
{
}

#pragma region Template-Related Code - Do not remove
void LaunchUriOutput::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Get a pointer to our main page.
    rootPage = dynamic_cast<MainPage^>(e->Parameter);
}

void LaunchUriOutput::OnNavigatedFrom(NavigationEventArgs^ e)
{
}

#pragma endregion
