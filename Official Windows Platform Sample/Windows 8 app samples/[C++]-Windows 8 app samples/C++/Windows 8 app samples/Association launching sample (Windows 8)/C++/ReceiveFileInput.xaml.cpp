// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// ReceiveFileInput.xaml.cpp
// Implementation of the ReceiveFileInput class
//

#include "pch.h"
#include "ReceiveFileInput.xaml.h"

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

ReceiveFileInput::ReceiveFileInput()
{
    InitializeComponent();
}

ReceiveFileInput::~ReceiveFileInput()
{
}

void ReceiveFileInput::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Get a pointer to our main page.
    rootPage = dynamic_cast<MainPage^>(e->Parameter);

    // Display the result of the file activation if we got here as a result of being activated for a file.
    if (rootPage->FileEvent != nullptr)
    {
        rootPage->NotifyUser("File activation received. The number of files received is " + rootPage->FileEvent->Files->Size + ". The first received file is " + rootPage->FileEvent->Files->GetAt(0)->Name + ".", NotifyType::StatusMessage);
    }
}
