//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// LocalSchemeHandler.xaml.cpp
// Implementation of the LocalSchemeHandler class
//

#include "pch.h"
#include "Scenario2_LocalSchemeHandler.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::MediaExtensions;

using namespace Windows::Foundation;
using namespace Windows::Media;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

LocalSchemeHandler::LocalSchemeHandler()
{
    InitializeComponent();

	extensionManager = ref new MediaExtensionManager();
    extensionManager->RegisterSchemeHandler("GeometricSource.GeometricSchemeHandler", "myscheme:");
}

void LocalSchemeHandler::Circle_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Video->Source = ref new Uri("myscheme://circle");
    Video->Play();
}

void LocalSchemeHandler::Square_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Video->Source = ref new Uri("myscheme://square");
    Video->Play();
}

void LocalSchemeHandler::Triangle_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Video->Source = ref new Uri("myscheme://triangle");
    Video->Play();
}
