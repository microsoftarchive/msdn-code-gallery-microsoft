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
// Scenario1_PickContact.xaml.h
// Declaration of the Scenario1_PickContact class
//

#pragma once

#include "pch.h"
#include "Scenario1_PickContact.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
	/// <summary>
	/// Feature scenario to select a single contact.
	/// </summary>
	public ref class Scenario1_PickContact sealed
    {
    public:
		Scenario1_PickContact();

    private:
        void PickAContactButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    };
}
