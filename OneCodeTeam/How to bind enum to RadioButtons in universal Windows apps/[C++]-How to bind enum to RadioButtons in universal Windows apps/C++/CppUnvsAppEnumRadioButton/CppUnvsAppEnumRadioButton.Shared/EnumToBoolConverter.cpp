/****************************** Module Header ******************************\
* Module Name:  EnumToBoolConverter.cpp
* Project:      CppUnvsAppEnumRadioButton
* Copyright (c) Microsoft Corporation.
*
* This is converter of Enum type and Boolean type
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
#include "pch.h"
#include "EnumToBoolConverter.h"
#include "Customer.h"
using namespace CppUnvsAppEnumRadioButton::Common;

EnumToBoolConverter::EnumToBoolConverter()
{

}

EnumToBoolConverter::~EnumToBoolConverter()
{

}

Object^ EnumToBoolConverter::Convert(Object^ value, TypeName targetType, Object^ parameter, String^ language)
{
	Type^ type = value->GetType();
	String^ str = type->FullName;
	Sport sport = safe_cast<Sport>(value);
	Platform::String^ sportString = "";
	switch (sport)
	{
	case Sport::Football:
		sportString = "Football";
		break;
	case Sport::Basketball:
		sportString = "Basketball";
		break;
	case Sport::Baseball:
		sportString = "Baseball";
		break;
	case Sport::Swimming:
		sportString = "Swimming";
		break;
	default:
		break;
	}
	String^ paraString = (String^)parameter;
	
	return sportString == paraString;
}

Object^ EnumToBoolConverter::ConvertBack(Object^ value, TypeName targetType, Object^ parameter, String^ language)
{
	if (static_cast<bool>(value) == false)
	{
		return Windows::UI::Xaml::DependencyProperty::UnsetValue;
	}
	Platform::String^ sportString = safe_cast<Platform::String^>(parameter);
	Sport favouriteSport;
	if (sportString == "Football")
	{
		favouriteSport = Sport::Football;
	}
	else if (sportString == "Basketball")
	{
		favouriteSport = Sport::Basketball;
	}
	else if (sportString == "Baseball")
	{
		favouriteSport = Sport::Baseball;
	}
	else if (sportString == "Swimming")
	{
		favouriteSport = Sport::Swimming;
	}
	return favouriteSport;
}