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
// Scenario4.xaml.cpp
// Implementation of the Scenario4 class
//

#include "pch.h"
#include "Scenario9.xaml.h"
using namespace SDKSample::ApplicationResources;
using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::ApplicationModel::Resources;
using namespace Windows::ApplicationModel::Resources::Core;


Scenario9::Scenario9()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario9::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
	Scenario9TextBlock->Text = "";
}

void SDKSample::ApplicationResources::Scenario9::Scenario9Button_Show_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = safe_cast<Button^>(sender);
    if (b != nullptr)
    {
		auto selectedLanguage = Scenario9ComboBox->SelectedValue;
		if (selectedLanguage != nullptr)
		{
			auto context = ref new ResourceContext();
			context->QualifierValues->Insert("language", selectedLanguage->ToString());

			auto resourceStringMap = ResourceManager::Current->MainResourceMap->GetSubtree("Resources");
			auto resourceCandidate = resourceStringMap->GetValue("string1", context);
			if (resourceCandidate)
			{
				Scenario9TextBlock->Text = resourceCandidate->ValueAsString;
			}
		}
    }
}

