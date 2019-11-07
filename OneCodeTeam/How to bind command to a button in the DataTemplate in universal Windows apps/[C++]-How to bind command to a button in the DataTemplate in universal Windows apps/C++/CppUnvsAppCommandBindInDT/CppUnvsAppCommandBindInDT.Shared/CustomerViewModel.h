/****************************** Module Header ******************************\
* Module Name:    CustomerViewModel.h
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
#pragma once
#include "Customer.h"
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml::Input;
namespace CppUnvsAppCommandBindInDT
{
	[Windows::UI::Xaml::Data::Bindable]
	public ref class CustomerViewModel sealed : Windows::UI::Xaml::Data::INotifyPropertyChanged
	{
	public:
		CustomerViewModel();
		
		property IObservableVector<Customer^>^ Customers
		{
			IObservableVector<Customer^>^ get()
			{
				return m_customers;
			}
			void set(IObservableVector<Customer^>^ value)
			{
				if (!value->Equals(m_customers))
				{
					m_customers = value;
					OnPropertyChanged("Customers");
				}
			}
		}

		property ICommand^ DeleteCommand;
	private:
		IObservableVector<Customer^>^ m_customers;

		void ExecuteDeleteCommand(Platform::Object^ param);
		Customer^ GetCustomerById(int id);

#pragma region INotifyPropertyChanged 
	private:
		bool _isPropertyChangedObserved;
		event Windows::UI::Xaml::Data::PropertyChangedEventHandler^ _privatePropertyChanged;

	protected:
		/// <summary> 
		/// Notifies listeners that a property value has changed. 
		/// </summary> 
		/// <param name="propertyName">Name of the property used to notify listeners.</param> 
		void OnPropertyChanged(Platform::String^ propertyName)
		{
			if (_isPropertyChangedObserved)
			{
				PropertyChanged(this, ref new Windows::UI::Xaml::Data::PropertyChangedEventArgs(propertyName));
			}
		}

	public:
		

		// in c++, it is not necessary to include definitions of add, remove, and raise. 
		//  these definitions have been made explicitly here so that we can check if the  
		//  event has listeners before firing the event 
		virtual event Windows::UI::Xaml::Data::PropertyChangedEventHandler^ PropertyChanged
		{
			virtual Windows::Foundation::EventRegistrationToken add(Windows::UI::Xaml::Data::PropertyChangedEventHandler^ e)
			{
				_isPropertyChangedObserved = true;
				return _privatePropertyChanged += e;
			}
			virtual void remove(Windows::Foundation::EventRegistrationToken t)
			{
				_privatePropertyChanged -= t;
			}

		protected:
			virtual void raise(Object^ sender, Windows::UI::Xaml::Data::PropertyChangedEventArgs^ e)
			{
				if (_isPropertyChangedObserved)
				{
					_privatePropertyChanged(sender, e);
				}
			}
		}
#pragma endregion 

	};
}


