/****************************** Module Header ******************************\
* Module Name:  MainPage.xaml.h
* Project:      CppUnivsAppHtmlBinding.WindowsPhone
* Copyright (c) Microsoft Corporation.
*
* This code sample shows how to bind HTML from a data model to a WebView.
* For more details, please refer to:
* http://blogs.msdn.com/b/wsdevsol/archive/2013/09/26/binding-html-to-a-webview-with-attached-properties.aspx
*
* MainPage.xaml.h
* Declaration of the MainPage class.
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
#include "MyExtensions.h"
namespace CppUnvsAppHtmlBinding
{
	[Windows::UI::Xaml::Data::Bindable]
	public ref class HTMLData sealed : Windows::UI::Xaml::Data::ICustomPropertyProvider
	{
	public:
		HTMLData() {}
		HTMLData(Platform::String^ _Name, Platform::String^ _HTML)
		{
			Name = _Name;
			HTML = _HTML;
		}

		property Platform::String^ Name
		{
			Platform::String^ get() { return _Name; }
			void set(Platform::String^ _value) { _Name = _value; }
		}

		property Platform::String^ HTML
		{
			Platform::String^ get() { return _HTML; }
			void set(Platform::String^ _value) { _HTML = _value; }
		}

		virtual Windows::UI::Xaml::Data::ICustomProperty^ GetCustomProperty(Platform::String^ name) { return nullptr; }
		virtual Windows::UI::Xaml::Data::ICustomProperty^ GetIndexedProperty(Platform::String^ name, Windows::UI::Xaml::Interop::TypeName type) { return nullptr; }
		virtual Platform::String^ GetStringRepresentation() { return Name; }


		property Windows::UI::Xaml::Interop::TypeName Type
		{
			virtual Windows::UI::Xaml::Interop::TypeName get() { return this->GetType(); }
		}

	private:
		Platform::String^ _Name;
		Platform::String^ _HTML;

	};

	public ref class MainPage sealed
	{
	public:
		MainPage();
	private:
		Platform::Collections::Vector<HTMLData^>^ HTMLStrings;
	protected:
		virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
	private:
		void Footer_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
	};
}
