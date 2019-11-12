// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// ReceiveUriOutput.xaml.cpp
// Implementation of the ReceiveUriOutput class
//

#include "pch.h"
#include "ReceiveUriOutput.xaml.h"
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

ReceiveUriOutput::ReceiveUriOutput()
{
    InitializeComponent();
}

ReceiveUriOutput::~ReceiveUriOutput()
{
}

#pragma region Template-Related Code - Do not remove
void ReceiveUriOutput::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Get a pointer to our main page.
    rootPage = dynamic_cast<MainPage^>(e->Parameter);
}

void ReceiveUriOutput::OnNavigatedFrom(NavigationEventArgs^ e)
{
}

#pragma endregion