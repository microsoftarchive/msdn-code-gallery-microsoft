/****************************** Module Header ******************************\
* Module Name:    DelegateCommand.h
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
#pragma once
using namespace Windows::UI::Xaml::Input;
using namespace Windows::Foundation;
namespace CppUnvsAppCommandBindInDT
{
	public delegate void ExecuteDelegate(Platform::Object^ parameter);
	public delegate bool CanExecuteDelegate(Platform::Object^ parameter);
	public ref class DelegateCommand sealed : public ICommand
	{
	private:
		ExecuteDelegate^ executeDelegate;
		CanExecuteDelegate^ canExecuteDelegate;
		bool lastCanExecute;

	public:
		DelegateCommand(ExecuteDelegate^ execute, CanExecuteDelegate^ canExecute);

		virtual event EventHandler<Object^>^ CanExecuteChanged;
		virtual void Execute(Object^ parameter);
		virtual bool CanExecute(Object^ parameter);
	};
}


