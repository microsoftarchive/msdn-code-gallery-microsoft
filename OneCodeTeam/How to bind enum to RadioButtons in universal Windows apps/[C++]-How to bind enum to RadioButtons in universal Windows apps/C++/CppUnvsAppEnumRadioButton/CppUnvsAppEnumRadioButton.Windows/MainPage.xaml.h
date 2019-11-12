/****************************** Module Header ******************************\
* Module Name:  MainPage.xaml.h
* Project:      CppUnvsAppEnumRadioButton.Windows
* Copyright (c) Microsoft Corporation.
*
* This sample demonstrates how to bind enum to RadioButton
*
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma once

#include "MainPage.g.h"
#include "BoolToStringConverter.h"
#include "EnumToStringConverter.h"
#include "Customer.h"
namespace CppUnvsAppEnumRadioButton
{
	
	public ref class MainPage sealed
	{
	public:
		MainPage();
	private:
		Customer^ m_selectedCustomer;
		Customers^ m_customers;
		void Footer_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void CustomerGridView_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e);
	};
}
