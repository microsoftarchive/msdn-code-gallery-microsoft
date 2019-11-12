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
// Scenario8.xaml.cpp
// Implementation of the Scenario8 class
//

#include "pch.h"
#include "Scenario8.xaml.h"

using namespace SDKSample::ApplicationResources;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Platform;
using namespace Windows::ApplicationModel::Resources::Core;


Scenario8::Scenario8()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario8::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}


void SDKSample::ApplicationResources::Scenario8::Scenario8Button_Show_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = safe_cast<Button^>(sender);
    if (b != nullptr)
    {
		String^ newAppLanguage = "";

		auto selectedLanguage = Scenario8ComboBox->SelectedValue;
		if (selectedLanguage != nullptr)
		{
			newAppLanguage = selectedLanguage->ToString(); 				
		}
		if (Windows::Globalization::ApplicationLanguages::PrimaryLanguageOverride != newAppLanguage)
		{
			Windows::Globalization::ApplicationLanguages::PrimaryLanguageOverride = newAppLanguage;
			// flush the internal cache
            ResourceManager::Current->DefaultContext->Reset();
		}
		auto resourceStringMap = ResourceManager::Current->MainResourceMap->GetSubtree("Resources");
		Scenario8TextBlock->Text = resourceStringMap->GetValue("string1")->ValueAsString;
    }
}

