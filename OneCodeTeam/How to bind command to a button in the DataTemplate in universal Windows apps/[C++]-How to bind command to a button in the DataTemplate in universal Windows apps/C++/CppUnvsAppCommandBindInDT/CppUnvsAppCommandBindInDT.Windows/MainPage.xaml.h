/****************************** Module Header ******************************\
* Module Name:    MainPage.xaml.h
* Project:        CppUnvsAppCommandBindInDT.Windows
* Copyright (c) Microsoft Corporation.
*
* This sample demonstrates how to bind Command in DataTemplate
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma once

#include "MainPage.g.h"
#include "BoolToStringConverter.h"
#include "CustomerViewModel.h"
namespace CppUnvsAppCommandBindInDT
{
	public ref class MainPage sealed
	{
	public:
		MainPage();
	private:
		CustomerViewModel^ m_cusViewModel;
		void Footer_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void Page_SizeChanged(Platform::Object^ sender, Windows::UI::Xaml::SizeChangedEventArgs^ e);
	};
}
