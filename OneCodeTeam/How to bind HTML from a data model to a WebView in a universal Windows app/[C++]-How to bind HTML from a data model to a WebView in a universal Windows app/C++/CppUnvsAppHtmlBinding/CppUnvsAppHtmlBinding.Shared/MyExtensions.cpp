/****************************** Module Header ******************************\
* Module Name:  MyExtensions.cpp
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
#include "pch.h"
#include "MyExtensions.h"

using namespace Platform;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Interop;

namespace CppUnvsAppHtmlBinding
{
	MyExtensions::MyExtensions(void)
	{
	}


	MyExtensions::~MyExtensions(void)
	{
	}


	DependencyProperty^ MyExtensions::_HTMLProperty = DependencyProperty::RegisterAttached(
		"HTML",
		TypeName(String::typeid),
		TypeName(MyExtensions::typeid),
		ref new PropertyMetadata(
		nullptr,
		ref new PropertyChangedCallback(OnHTMLChanged)
		)
		);

	void MyExtensions::OnHTMLChanged(DependencyObject^ d, DependencyPropertyChangedEventArgs^ e)
	{
		WebView^ wv = (WebView^)d;
		String^ value = (String^)e->NewValue;

		wv->NavigateToString(value);
	}



}
