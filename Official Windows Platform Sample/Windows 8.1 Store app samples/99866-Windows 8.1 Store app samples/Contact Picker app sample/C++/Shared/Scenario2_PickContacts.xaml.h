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
// Scenario2_PickContacts.xaml.h
// Declaration of the Scenario2_PickContacts class
//

#pragma once

#include "pch.h"
#include "Scenario2_PickContacts.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
	/// <summary>
	/// Feature scenario to select multiple contacts.
	/// </summary>
	public ref class Scenario2_PickContacts sealed
    {
    public:
		Scenario2_PickContacts();

    private:
        void PickContactsButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    };
}
