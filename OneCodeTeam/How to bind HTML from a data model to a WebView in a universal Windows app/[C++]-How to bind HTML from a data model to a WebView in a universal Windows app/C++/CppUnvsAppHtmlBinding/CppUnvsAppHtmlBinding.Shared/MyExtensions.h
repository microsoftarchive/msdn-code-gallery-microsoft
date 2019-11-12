/****************************** Module Header ******************************\
* Module Name:  MyExtensions.h
* Project:      CppUnvsAppHtmlBinding
* Copyright (c) Microsoft Corporation.
*
* This code sample shows how to bind HTML from a data model to a WebView.
* For more details, please refer to:
* http://blogs.msdn.com/b/wsdevsol/archive/2013/09/26/binding-html-to-a-webview-with-attached-properties.aspx
*
* MyExtensions class.
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


using namespace Windows::UI::Xaml;

namespace CppUnvsAppHtmlBinding
{
	public ref class MyExtensions sealed : DependencyObject
	{
	public:
		MyExtensions(void);

		static property DependencyProperty^ HTMLProperty {
			DependencyProperty^ get() { return _HTMLProperty; }
		}


		static Platform::String^ MyExtensions::GetHTML(DependencyObject^ obj) {
			return (Platform::String^)obj->GetValue(HTMLProperty);
		}

		static void MyExtensions::SetHTML(DependencyObject^ obj, Platform::String^ HTML) {
			obj->SetValue(HTMLProperty, HTML);
		}

	private:
		~MyExtensions(void);

		static DependencyProperty^ _HTMLProperty;
		static void OnHTMLChanged(DependencyObject^ d, DependencyPropertyChangedEventArgs^ e);


	};
}