/****************************** Module Header ******************************\
* Module Name:  Customer.cpp
* Project:      CppUnvsAppEnumRadioButton
* Copyright (c) Microsoft Corporation.
*
* This is the demo data
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
#include "Customer.h"


using namespace CppUnvsAppEnumRadioButton;


Customers::Customers()
{
	_isPropertyChangedObserved = false;

	static Platform::Collections::Vector<Customer^>^ Customers = ref new Platform::Collections::Vector<Customer^>;
	if (Customers->Size == 0)
	{
		Customers->Append(ref new Customer("Allen", 25, true, Sport::Basketball));
		Customers->Append(ref new Customer("Carter", 26, true, Sport::Basketball));
		Customers->Append(ref new Customer("Rose", 30, true, Sport::Swimming));
		Customers->Append(ref new Customer("Dove", 33, true, Sport::Football));
		Customers->Append(ref new Customer("Mary", 30, false, Sport::Swimming));
		Customers->Append(ref new Customer("William", 42, true, Sport::Basketball));
		Customers->Append(ref new Customer("Daisy", 16, false, Sport::Swimming));
		Customers->Append(ref new Customer("Elena", 17, false, Sport::Football));
		Customers->Append(ref new Customer("Tracy", 35, false, Sport::Basketball));
		Customers->Append(ref new Customer("Alex", 23, true, Sport::Basketball));
		Customers->Append(ref new Customer("Mike", 50, true, Sport::Football));
		Customers->Append(ref new Customer("Lisa", 23, false, Sport::Basketball));
		Customers->Append(ref new Customer("Andrew", 19, true, Sport::Football));
		Customers->Append(ref new Customer("Steve", 39, true, Sport::Swimming));
		Customers->Append(ref new Customer("Jim", 14, true, Sport::Basketball));
	}	

	Items = Customers;
}
