//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

//
// Scenario2.xaml.cpp
// Implementation of the Scenario2 class
//

#include "pch.h"
#include "Scenario2.xaml.h"
#include "PictureItemsView.xaml.h"

using namespace SDKSample::DataSourceAdapter;

using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::UI::Xaml::Navigation;

Scenario2::Scenario2()
{
    InitializeComponent();
}

void Scenario2::OnNavigatedTo(NavigationEventArgs^ e)
{
    token = RunButton->Click += ref new RoutedEventHandler(this, &Scenario2::RunButton_Click);
}

void Scenario2::OnNavigatedFrom(NavigationEventArgs^ e)
{
    RunButton->Click -= token;
}

void Scenario2::RunButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    MainPage::Current->Frame->Navigate(TypeName(PictureItemsView::typeid));
}
