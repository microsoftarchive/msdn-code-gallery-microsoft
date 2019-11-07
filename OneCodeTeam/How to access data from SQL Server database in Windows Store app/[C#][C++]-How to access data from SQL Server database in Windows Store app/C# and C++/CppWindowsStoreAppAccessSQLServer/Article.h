/******************************* Module Header ******************************\
* Module Name:  Article.h
* Project:      CppWindowsStoreAppAccessSQLServer
* Copyright (c) Microsoft Corporation.
*
* Article class. 
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\****************************************************************************/
#include "pch.h"
using namespace Platform;

namespace CPPWindowsStoreAppAccessSQLServer
{
	[Windows::UI::Xaml::Data::Bindable]
	public ref class Article sealed
	{
	public:
		property String^ Title
		{
			String^ get(){ return title; }
			void set(String^ data){ title = data; }
		}
		property String^ Text
		{
			String^ get(){ return text; }
			void set(String^ data){ text = data; }
		}

	private:
		String^ title;
		String^ text;
	};
}