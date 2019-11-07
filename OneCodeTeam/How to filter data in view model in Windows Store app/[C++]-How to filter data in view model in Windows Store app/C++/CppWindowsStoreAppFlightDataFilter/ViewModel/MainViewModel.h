/****************************** Module Header ******************************\
* Module Name:  MainViewModel.h
* Project:      CppWindowsStoreAppFlightDataFilter
* Copyright (c) Microsoft Corporation.
*
* MainViewModel
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
#include "FlightDataItem.h"
using namespace CppWindowsStoreAppFlightDataFilter::Data;

namespace CppWindowsStoreAppFlightDataFilter
{
	namespace ViewModel
	{

		[Windows::Foundation::Metadata::WebHostHidden]
		[Windows::UI::Xaml::Data::Bindable]
		public ref class MainViewModel sealed : public Windows::UI::Xaml::Data::INotifyPropertyChanged
		{
		public:
			MainViewModel();

			property Windows::Foundation::Collections::IObservableVector<FlightDataItem^>^ Flights
			{
				Windows::Foundation::Collections::IObservableVector<FlightDataItem^>^ get();
				void set(Windows::Foundation::Collections::IObservableVector<FlightDataItem^>^ value);
			}

			property Windows::Foundation::Collections::IObservableVector<FlightDataItem^>^ FilteredFlights
			{
				Windows::Foundation::Collections::IObservableVector<FlightDataItem^>^ get();
				void set(Windows::Foundation::Collections::IObservableVector<FlightDataItem^>^ value);
			}

			property double SelectedPrice
			{
				double get();
				void set(double value);
			}

		private:
			void RefreshFilteredData();
			void LoadFlightData();

			Platform::Collections::Vector<FlightDataItem^>^ _flights;
			Platform::Collections::Vector<FlightDataItem^>^ _filteredFlights;
			double _selectedPrice;

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

			// In c++, it is not necessary to include definitions of add, remove, and raise.
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
}
