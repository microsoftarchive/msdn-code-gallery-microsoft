/****************************** Module Header ******************************\
* Module Name:    CustomerViewModel.cpp
* Project:        CppUnvsAppCommandBindInDT
* Copyright (c) Microsoft Corporation.
*
* This is a ViewModel class which defines properties and Command will be used
* by View.
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
#include "CustomerViewModel.h"
#include "DelegateCommand.h"
#include "Customer.h"
using namespace CppUnvsAppCommandBindInDT;

CustomerViewModel::CustomerViewModel()
{
	_isPropertyChangedObserved = false;

	// create a DeleteCommand instance
	DeleteCommand = ref new DelegateCommand(ref new ExecuteDelegate(this, &CustomerViewModel::ExecuteDeleteCommand), nullptr);

	Customers = InitializeSampleData::GetData();
}


void CustomerViewModel::ExecuteDeleteCommand(Platform::Object^ param)
{
	int id = (int)param;
	Customer^ cus = GetCustomerById(id);
	if (cus != nullptr)
	{
		unsigned int index = 0;
		Customers->IndexOf(cus, &index);
		Customers->RemoveAt(index);
	}
}

Customer^ CustomerViewModel::GetCustomerById(int id)
{	
	auto cus = std::find_if(begin(Customers), end(Customers), [=](Customer^ cus){
		if (id == cus->Id)
		{
			return true;
		}
		else
		{
			return false;
		}
	});
	if (cus != end(Customers))
	{
		return *cus;
	}
	else
	{
		return nullptr;
	}
	
}