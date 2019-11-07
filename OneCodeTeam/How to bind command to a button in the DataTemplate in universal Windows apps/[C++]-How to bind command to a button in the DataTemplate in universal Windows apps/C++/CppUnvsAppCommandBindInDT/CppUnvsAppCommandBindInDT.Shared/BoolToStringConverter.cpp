/****************************** Module Header ******************************\
* Module Name:    BoolToStringConverter.cpp
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
#include "pch.h"
#include "BoolToStringConverter.h"
using namespace CppUnvsAppCommandBindInDT;

BoolToSexConverter::BoolToSexConverter()
{

}

BoolToSexConverter::~BoolToSexConverter()
{

}

Object^ BoolToSexConverter::Convert(Object^ value, TypeName targetType, Object^ parameter, String^ language)
{
	return (bool)value ? "Male" : "Female";
}

Object^ BoolToSexConverter::ConvertBack(Object^ value, TypeName targetType, Object^ parameter, String^ language)
{
	Platform::String^ sex = (Platform::String^)value;
	if (sex == "Female")
	{
		return false;
	}
	else
		return true;
}

BoolToVipConverter::BoolToVipConverter()
{

}

BoolToVipConverter::~BoolToVipConverter()
{

}

Object^ BoolToVipConverter::Convert(Object^ value, TypeName targetType, Object^ parameter, String^ language)
{
	return (bool)value ? "Yes" : "No";
}

Object^ BoolToVipConverter::ConvertBack(Object^ value, TypeName targetType, Object^ parameter, String^ language)
{
	Platform::String^ vip = (Platform::String^)value;
	if (vip == "No")
	{
		return false;
	}
	else
		return true;
}