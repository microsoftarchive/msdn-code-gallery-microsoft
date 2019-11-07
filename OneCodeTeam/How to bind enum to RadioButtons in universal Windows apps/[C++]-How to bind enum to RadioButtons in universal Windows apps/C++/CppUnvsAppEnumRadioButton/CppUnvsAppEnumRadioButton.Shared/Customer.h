/****************************** Module Header ******************************\
* Module Name:  Customer.h
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
#pragma once
namespace CppUnvsAppEnumRadioButton
{	
	public enum class Sport
	{
		Football,
		Basketball,
		Baseball,
		Swimming		
	};
	[Windows::UI::Xaml::Data::Bindable]
	public ref class Customer sealed : Windows::UI::Xaml::Data::INotifyPropertyChanged
	{
	public:
		Customer(Platform::String^ name, int32 age, Platform::Boolean sex, Sport favouriteSport)
			: name(name), age(age), sex(sex), favouriteSport(favouriteSport)
		{
			_isPropertyChangedObserved = false;
		}
		
		property Platform::String^ Name
		{
			Platform::String^ get()
			{
				return name;
			}
			void set(Platform::String^ value)
			{
				if (!value->Equals(name))
				{
					name = value;
					OnPropertyChanged("Name");
				}
			}
		}
		property Platform::Boolean Sex
		{
			Platform::Boolean get()
			{
				return sex;
			}
			void set(Platform::Boolean value)
			{
				if (!value.Equals(sex))
				{
					sex = value;
					OnPropertyChanged("Sex");
				}
			}
		}
		property int32 Age
		{
			int32 get()
			{
				return age;
			}
			void set(int32 value)
			{
				if (!value.Equals(age))
				{
					age = value;
					OnPropertyChanged("Age");
				}
			}
		}
		
		// Boxed this enum object.
		property Platform::IBox<Sport>^ FavouriteSport
		{
			Platform::IBox<Sport>^ get()
			{
				return favouriteSport;
			}
			void set(Platform::IBox<Sport>^ value)
			{
				if (!value->Equals(favouriteSport))
				{
					favouriteSport = value;
					OnPropertyChanged("FavouriteSport");
				}
			}
		}
	private:		
		Platform::String^ name;
		int32 age;
		Platform::Boolean sex;
		Platform::IBox<Sport>^ favouriteSport;


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
		Customer()
		{
			_isPropertyChangedObserved = false;
		}

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
	[Windows::UI::Xaml::Data::Bindable]
	public ref class Customers sealed : Windows::UI::Xaml::Data::INotifyPropertyChanged
	{
	public:
		Customers();
		property Windows::Foundation::Collections::IObservableVector<Customer^>^ Items
		{
			Windows::Foundation::Collections::IObservableVector<Customer^>^ get()
			{
				return _items;
			}
			void set(Windows::Foundation::Collections::IObservableVector<Customer^>^ value)
			{
				if (!value->Equals(_items))
				{
					_items = static_cast<Platform::Collections::Vector<Customer^>^>(value);
					OnPropertyChanged("Items");
				}
			}
		}
	private:
		Platform::Collections::Vector<Customer^>^ _items;

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

