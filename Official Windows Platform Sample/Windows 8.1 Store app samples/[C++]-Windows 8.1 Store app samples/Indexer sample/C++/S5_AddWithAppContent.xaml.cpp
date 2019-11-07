//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S5_AddWithAppContent.xaml.cpp
// Implementation of the S5_AddWithAppContent class
//

#include "pch.h"
#include "S5_AddWithAppContent.xaml.h"
#include "MainPage.xaml.h"
#include "Helpers.h"

using namespace SDKSample;
using namespace SDKSample::Indexer;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

S5_AddWithAppContent::S5_AddWithAppContent()
{
    InitializeComponent();
    InitializeRevisionNumber();
}

void Indexer::S5_AddWithAppContent::AddToIndex_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    AddAppContentFilesToIndexedFolder();
}
