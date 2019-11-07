/****************************** Module Header ******************************\
* Module Name:  BoolToStringConverter.cpp
* Project:      CppUnvsAppEnumRadioButton
* Copyright (c) Microsoft Corporation.
*
* This is converter of Boolean type and String type
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
#include "EnumToStringConverter.h"
#include "Customer.h"
using namespace CppUnvsAppEnumRadioButton::Common;

EnumToStringConverter::EnumToStringConverter()
{
}


EnumToStringConverter::~EnumToStringConverter()
{
}

Object^ EnumToStringConverter::Convert(Object^ value, TypeName targetType, Object^ parameter, String^ language)
{
	Sport sport = (Sport)value;
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
		sportString = "BaseBall";
		break;
	case Sport::Swimming:
		sportString = "Swimming";
		break;
	default:
		break;
	}

	return sportString;
}

Object^ EnumToStringConverter::ConvertBack(Object^ value, TypeName targetType, Object^ parameter, String^ language)
{
	return nullptr;
}