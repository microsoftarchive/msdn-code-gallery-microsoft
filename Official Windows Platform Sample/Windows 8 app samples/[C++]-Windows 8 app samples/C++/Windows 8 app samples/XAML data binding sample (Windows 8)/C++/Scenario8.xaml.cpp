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

Scenario8::Scenario8()
{
    InitializeComponent();
	Scenario8Reset(nullptr,nullptr);
}

void SDKSample::DataBinding::Scenario8::Scenario8Reset(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if(employees != nullptr)
	{
		employees->VectorChanged -= vectorChangedHandlerToken;
	}

	employees = ref new GeneratorIncrementalLoadingClass(1000);
	vectorChangedHandlerToken = employees->VectorChanged += ref new BindableVectorChangedEventHandler(this, &Scenario8::BindableVectorChanged);

	employeesCVS->Source = employees;
            
    tbCollectionChangeStatus->Text = "";

}

void SDKSample::DataBinding::Scenario8::BindableVectorChanged(Windows::UI::Xaml::Interop::IBindableObservableVector^ sender, Object^ e)
{
	tbCollectionChangeStatus->Text = "Collection was changed. Count = " + employees->Size;
}
