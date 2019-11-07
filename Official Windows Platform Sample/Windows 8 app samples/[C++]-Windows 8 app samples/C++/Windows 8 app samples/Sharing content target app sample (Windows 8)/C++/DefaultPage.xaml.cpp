// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// DefaultPage.xaml.cpp
// Implementation of the DefaultPage class
//

#include "pch.h"
#include "DefaultPage.xaml.h"

using namespace ShareTarget;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

DefaultPage::DefaultPage()
{
    InitializeComponent();
}

void DefaultPage::Footer_Click(Object^ sender, RoutedEventArgs^ e)
{
    auto uri = ref new Uri(safe_cast<String^>(safe_cast<HyperlinkButton^>(sender)->Tag));
    Windows::System::Launcher::LaunchUriAsync(uri);
}