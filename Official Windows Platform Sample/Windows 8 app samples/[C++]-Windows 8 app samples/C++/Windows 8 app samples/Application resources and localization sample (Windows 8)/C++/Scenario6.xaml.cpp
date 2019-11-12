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
// Scenario6.xaml.cpp
// Implementation of the Scenario6 class
//

#include "pch.h"
#include "Scenario6.xaml.h"

using namespace SDKSample::ApplicationResources;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::ApplicationModel::Resources;
using namespace AppResourceClassLibrary;

Scenario6::Scenario6()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario6::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void Scenario6::Scenario6Button_Show_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = safe_cast<Button^>(sender);
    if (b != nullptr)
    {
        auto resourceLoader = ref new ResourceLoader("AppResourceClassLibrary/Resources");
        this->Scenario6TextBlock->Text = "Retrieved from app: " + resourceLoader->GetString("string1");
		
		auto lib = ref new AppResourceClassLibrary::Class1();
		this->Scenario6TextBlock2->Text = "Retrieved from class library: " + lib->GetString();
    }
}

