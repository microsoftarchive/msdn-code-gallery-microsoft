/****************************** Module Header ******************************\
* Module Name:    BoolToStringConverter.h
* Project:        CppUnvsAppCommandBindInDT
* Copyright (c) Microsoft Corporation.
*
* This is a Converter which converts between Boolean type and String type
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
using namespace Platform;
using namespace Windows::UI::Xaml::Interop;
namespace CppUnvsAppCommandBindInDT
{
	[Windows::Foundation::Metadata::WebHostHidden]
	public ref class BoolToSexConverter sealed : Windows::UI::Xaml::Data::IValueConverter
	{
	public:
		BoolToSexConverter();
		virtual Object^ Convert(Object^ value, TypeName targetType, Object^ parameter, String^ language);
		virtual Object^ ConvertBack(Object^ value, TypeName targetType, Object^ parameter, String^ language);
	private:
		~BoolToSexConverter();
	};

	[Windows::Foundation::Metadata::WebHostHidden]
	public ref class BoolToVipConverter sealed : Windows::UI::Xaml::Data::IValueConverter
	{
	public:
		BoolToVipConverter();
		virtual Object^ Convert(Object^ value, TypeName targetType, Object^ parameter, String^ language);
		virtual Object^ ConvertBack(Object^ value, TypeName targetType, Object^ parameter, String^ language);
	private:
		~BoolToVipConverter();
	};
}

