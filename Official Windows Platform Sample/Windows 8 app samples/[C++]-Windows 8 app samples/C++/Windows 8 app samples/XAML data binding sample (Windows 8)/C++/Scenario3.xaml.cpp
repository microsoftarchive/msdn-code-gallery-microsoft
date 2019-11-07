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
// Scenario3.xaml.cpp
// Implementation of the Scenario3 class
//

#include "pch.h"
#include "Scenario3.xaml.h"
#include "Employee.h"

using namespace SDKSample::DataBinding;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario3::Scenario3()
{
	
	InitializeComponent();
	
	_employee = ref new Employee();
	Output->DataContext = _employee;
	_employee->PropertyChanged += ref new PropertyChangedEventHandler(this, &Scenario3::employeeChanged);
	
	Scenario3Reset(nullptr,nullptr);
}

void SDKSample::DataBinding::Scenario3::Scenario3Reset(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	_employee->Name = "Jane Doe";
	_employee->Organization = "Contoso";
			
	tbBoundDataModelStatus->Text = "";
}

void Scenario3::employeeChanged(Object^ sender, PropertyChangedEventArgs^ e)
{
	tbBoundDataModelStatus->Text = "The property:'" + e->PropertyName + "' was changed";
}
