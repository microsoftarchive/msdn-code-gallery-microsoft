/****************************** Module Header ******************************\
* Module Name:  Person.h
* Project:      CppUnvsAppJsonToWebService
* Copyright (c) Microsoft Corporation.
*
* This is post data class
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
namespace CppUnvsAppJsonToWebService
{
	public ref class Person sealed
	{
	public:
		Person(void){}

		property Platform::String^ Name;
		property int Age;
	private:
		~Person(void){}
	};
}