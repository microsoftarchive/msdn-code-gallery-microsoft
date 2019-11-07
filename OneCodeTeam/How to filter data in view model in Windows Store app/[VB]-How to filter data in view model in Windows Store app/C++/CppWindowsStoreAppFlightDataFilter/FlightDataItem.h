/****************************** Module Header ******************************\
* Module Name:  FlightDataItem.h
* Project:      CppWindowsStoreAppFlightDataFilter
* Copyright (c) Microsoft Corporation.
*
* FlightDataItem
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
namespace CppWindowsStoreAppFlightDataFilter
{
	namespace Data
	{
		/// <summary>
		/// Generic item data model.
		/// </summary>
		[Windows::UI::Xaml::Data::Bindable]
		public ref class FlightDataItem sealed : public Windows::UI::Xaml::Data::ICustomPropertyProvider
		{
		public:
			property Platform::String^ DepartureCity { Platform::String^ get(); }
			property Platform::String^ DestinationCity { Platform::String^ get(); }
			property double Price { double get(); }
			property Platform::String^ DepartureTime { Platform::String^ get(); }
			property Platform::String^ ArrivalTime { Platform::String^ get(); }
			property int NumberOfConnections { int get(); }

			// Implementation of ICustomPropertyProvider provides a string representation for the object to be used as automation name for accessibility
			virtual Windows::UI::Xaml::Data::ICustomProperty^ GetCustomProperty(Platform::String^ name);
			virtual Windows::UI::Xaml::Data::ICustomProperty^ GetIndexedProperty(Platform::String^ name, Windows::UI::Xaml::Interop::TypeName type);
			virtual Platform::String^ GetStringRepresentation();
			property Windows::UI::Xaml::Interop::TypeName Type { virtual Windows::UI::Xaml::Interop::TypeName get(); }

		internal:
			FlightDataItem(Platform::String^ departureCity, Platform::String^ destCity, double price, Platform::String^ departTime,
				Platform::String^ arrivalTime, int numConnections);

		private:
			Platform::String^ _departureCity;
			Platform::String^ _destCity;
			double _price;
			Platform::String^ _departTime;
			Platform::String^ _arrivalTime;
			int _numConnections;
		};
	}
}
