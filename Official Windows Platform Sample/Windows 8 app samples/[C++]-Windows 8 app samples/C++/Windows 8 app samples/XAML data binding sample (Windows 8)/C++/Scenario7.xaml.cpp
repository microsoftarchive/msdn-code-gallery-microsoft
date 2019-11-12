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
// Scenario7.xaml.cpp
// Implementation of the Scenario7 class
//

#include "pch.h"
#include "Scenario7.xaml.h"
#include "Team.h"

using namespace SDKSample::DataBinding;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

Scenario7::Scenario7()
{
    InitializeComponent();
	btnRemoveTeam->Click += ref new RoutedEventHandler(this, &Scenario7::BtnRemoveTeam_Click);
	Scenario7Reset(nullptr,nullptr);
}


void SDKSample::DataBinding::Scenario7::Scenario7Reset(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    auto teams = ref new Teams();
	teams->Items->VectorChanged += ref new VectorChangedEventHandler<Object^>(this, &Scenario7::VectorChanged);
	_observableTeams = teams;
	teamsCVS->Source = _observableTeams->Items;
	tbCollectionChangeStatus->Text = "";

}

void SDKSample::DataBinding::Scenario7::BtnRemoveTeam_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (_observableTeams->Items->Size > 0)
    {
		int index=0;
		if(lvTeams->SelectedItem != nullptr)
		{
			index = lvTeams->SelectedIndex;
		}
		_observableTeams->Items->RemoveAt(index);
    }
}

void SDKSample::DataBinding::Scenario7::VectorChanged(IObservableVector<Object^>^ sender, IVectorChangedEventArgs^ e)
{
	tbCollectionChangeStatus->Text = "Collection was changed. Count = " + _observableTeams->Items->Size.ToString();
}
