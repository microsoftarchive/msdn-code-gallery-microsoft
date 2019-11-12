// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// S2Formatter.cpp
// Implementation of the S2Formatter class
//

#include "pch.h"
#include "S2Formatter.h"

using namespace SDKSample::DataBinding;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Globalization::NumberFormatting;

S2Formatter::S2Formatter()
{
}

S2Formatter::~S2Formatter()
{
}

Object^ S2Formatter::Convert(Object^ value, TypeName targetType, Object^ parameter, String^ language)
{
	String^ _grade = "";
	String^ _valueString = "";
	//try parsing the value to int
	int _value = ((Windows::Foundation::IPropertyValue^)value)->GetInt32();
	if (_value < 50)
		_grade = "F";
	else if (_value < 60)
		_grade = "D";
	else if (_value < 70)
		_grade = "C";
	else if (_value < 80)
		_grade = "B";
	else if (_value < 90)
		_grade = "A";
	else if (_value < 100)
		_grade = "A+";
	else if (_value == 100)
		_grade = "SUPER STAR!";
	return _grade;
}

Object^ S2Formatter::ConvertBack(Object^ value, TypeName targetType, Object^ parameter, String^ language)
{
	return nullptr; //doing one-way binding so this is not required.
}