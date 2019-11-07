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
// Scenario3.xaml.h
// Declaration of the Scenario3 class
//

#pragma once

#include "pch.h"
#include "Scenario3.g.h"
#include "MainPage.xaml.h"
#include "Employee.h"

namespace SDKSample
{
    namespace DataBinding
    {
    	/// <summary>
    	/// An empty page that can be used on its own or navigated to within a Frame.
    	/// </summary>
    	public ref class Scenario3 sealed
    	{
    	public:
    		Scenario3();
    	private:
    		property Employee^ _employee;
    		MainPage^ rootPage;
    		void Scenario3Reset(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    		void employeeChanged(Object^ sender, PropertyChangedEventArgs^ e);
    	};
    }
}
