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
// Scenario1.xaml.cpp
// Implementation of the Scenario1 class
//

#include "pch.h"
#include "Scenario1.xaml.h"
#include "PictureFilesView.xaml.h"

using namespace SDKSample::DataSourceAdapter;

using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::UI::Xaml::Navigation;

Scenario1::Scenario1()
{
    InitializeComponent();
}

void Scenario1::OnNavigatedTo(NavigationEventArgs^ e)
{
    token = RunButton->Click += ref new RoutedEventHandler(this, &Scenario1::RunButton_Click);
}

void Scenario1::OnNavigatedFrom(NavigationEventArgs^ e)
{
    RunButton->Click -= token;
}

void Scenario1::RunButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    MainPage::Current->Frame->Navigate(TypeName(PictureFilesView::typeid));
}
