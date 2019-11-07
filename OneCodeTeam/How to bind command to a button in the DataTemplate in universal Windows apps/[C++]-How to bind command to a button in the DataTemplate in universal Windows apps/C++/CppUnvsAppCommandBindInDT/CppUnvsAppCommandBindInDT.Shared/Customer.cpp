/****************************** Module Header ******************************\
* Module Name:    Customer.cpp
* Project:        CppUnvsAppCommandBindInDT
* Copyright (c) Microsoft Corporation.
*
* This is a Model class
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
#include "Customer.h"
using namespace CppUnvsAppCommandBindInDT;

Platform::Collections::Vector<Customer^>^ InitializeSampleData::GetData()
{
	Platform::Collections::Vector<Customer^>^ Customers = ref new Platform::Collections::Vector<Customer^>;

	Customers->Append(ref new Customer(1, "Allen", true, 25, true));
	Customers->Append(ref new Customer(2, "Carter", true, 26, true));
	Customers->Append(ref new Customer(3, "Rose", true, 30, false));
	Customers->Append(ref new Customer(4, "Daisy", false, 20, false));
	Customers->Append(ref new Customer(5, "Mary", false, 15, true));
	Customers->Append(ref new Customer(6, "Ray", true, 39, false));
	Customers->Append(ref new Customer(7, "Sherry", false, 55, false));
	Customers->Append(ref new Customer(8, "Lisa", false, 32, false));
	Customers->Append(ref new Customer(9, "Judy", false, 19, true));
	Customers->Append(ref new Customer(10, "Jack", true, 28, false));
	Customers->Append(ref new Customer(11, "May", false, 20, false));
	Customers->Append(ref new Customer(12, "Vivian", false, 32, true));

	return Customers;
}
