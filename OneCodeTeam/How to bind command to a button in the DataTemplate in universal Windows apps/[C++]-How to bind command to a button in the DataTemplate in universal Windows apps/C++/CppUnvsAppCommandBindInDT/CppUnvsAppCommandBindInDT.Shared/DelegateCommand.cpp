/****************************** Module Header ******************************\
* Module Name:    DelegateCommand.cpp
* Project:        CppUnvsAppCommandBindInDT
* Copyright (c) Microsoft Corporation.
*
* This class implements ICommand interface
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
#include "DelegateCommand.h"
using namespace CppUnvsAppCommandBindInDT;

DelegateCommand::DelegateCommand(ExecuteDelegate^ execute, CanExecuteDelegate^ canExecute)
	: executeDelegate(execute), canExecuteDelegate(canExecute)
{
}

void DelegateCommand::Execute(Object^ parameter)
{
	if (executeDelegate != nullptr)
	{
		executeDelegate(parameter);
	}
}

bool DelegateCommand::CanExecute(Object^ parameter)
{
	if (canExecuteDelegate == nullptr)
	{
		return true;
	}

	bool canExecute = canExecuteDelegate(parameter);

	if (lastCanExecute != canExecute)
	{
		lastCanExecute = canExecute;
		CanExecuteChanged(this, nullptr);
	}

	return lastCanExecute;
}