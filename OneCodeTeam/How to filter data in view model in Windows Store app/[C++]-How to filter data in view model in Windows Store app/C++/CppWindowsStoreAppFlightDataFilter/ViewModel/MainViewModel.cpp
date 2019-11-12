/****************************** Module Header ******************************\
* Module Name:  MainViewModel.cpp
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
#include "pch.h"
#include "MainViewModel.h"
using namespace CppWindowsStoreAppFlightDataFilter::ViewModel;
using namespace Platform::Collections;
using namespace Windows::Foundation::Collections;
using namespace concurrency;
using namespace Windows::ApplicationModel::Resources::Core;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::Storage;
using namespace Windows::Data::Json;

MainViewModel::MainViewModel()
{
	_isPropertyChangedObserved = false;
	Flights = ref new Vector < FlightDataItem^>();
	LoadFlightData();
}

IObservableVector<FlightDataItem^>^ MainViewModel::Flights::get()
{
	return _flights;
}

void MainViewModel::Flights::set(IObservableVector<FlightDataItem^>^ value)
{
	_flights = dynamic_cast<Vector<FlightDataItem^>^ >(value);
	if (_flights != nullptr)
	{
		OnPropertyChanged("Flight");
	}
}

IObservableVector<FlightDataItem^>^ MainViewModel::FilteredFlights::get()
{
	return _filteredFlights;
}

void MainViewModel::FilteredFlights::set(IObservableVector<FlightDataItem^>^ value)
{
	_filteredFlights = dynamic_cast<Vector<FlightDataItem^>^ > (value);
	if (_filteredFlights != nullptr)
	{
		OnPropertyChanged("FilteredFlights");
	}
}

double MainViewModel::SelectedPrice::get()
{
	return _selectedPrice;
}

void MainViewModel::SelectedPrice::set(double value)
{
	if (_selectedPrice != value)
	{
		_selectedPrice = value;
		OnPropertyChanged("SelectedPrice");
		RefreshFilteredData();
	}
}

void MainViewModel::LoadFlightData()
{
	if (_flights->Size != 0)
	{
		return;
	}

	double minPrice = 0.0;
	double maxPrice = 0.0;

	Uri^ uri = ref new Uri("ms-appx:///DataModel/FlightData.json");
	create_task(StorageFile::GetFileFromApplicationUriAsync(uri))
		.then([](StorageFile^ storageFile)
	{
		return FileIO::ReadTextAsync(storageFile);
	})
		.then([this, &minPrice, &maxPrice](Platform::String^ jsonText)
	{
		JsonObject^ jsonObject = JsonObject::Parse(jsonText);
		auto jsonVector = jsonObject->GetNamedArray("Flights")->GetView();
		
		for (const auto &flights : jsonVector)
		{
			JsonObject^ flightObject = flights->GetObject();
			FlightDataItem^ flightItem = ref new FlightDataItem(flightObject->GetNamedString("DepartureCity"),
				flightObject->GetNamedString("DestinationCity"),
				flightObject->GetNamedNumber("Price"),
				flightObject->GetNamedString("DepartureTime"),
				flightObject->GetNamedString("ArrivalTime"),
				static_cast<int>(flightObject->GetNamedNumber("NumberOfConnections")));

			_flights->Append(flightItem);

			if (minPrice > flightObject->GetNamedNumber("Price"))
			{
				minPrice = flightObject->GetNamedNumber("Price");
			}
			if (maxPrice < flightObject->GetNamedNumber("Price"))
			{
				maxPrice = flightObject->GetNamedNumber("Price");
			}
		};

		// Initialize SelectedPrice and FilteredFlights properties.
		FilteredFlights = Flights;
		SelectedPrice = maxPrice;
	})
		.then([](task<void> t)
	{
		try
		{
			t.get();
		}
		catch (Platform::COMException^ e)
		{
			OutputDebugString(e->Message->Data());
			// TODO: If App can recover from exception,
			// remove throw; below and add recovery code.
			throw;
		}
	});
}

void MainViewModel::RefreshFilteredData()
{
	Vector<FlightDataItem^>^ filteredFlightsVec = ref new Vector<FlightDataItem^>();
	
	std::for_each(begin(this->_flights), end(this->_flights), [filteredFlightsVec, this](FlightDataItem^ flightDataItem)
	{
		if (flightDataItem->Price <= SelectedPrice)
		{
			filteredFlightsVec->Append(flightDataItem);
		}
	});

	//  This will limit the amount of view refreshes
	if (filteredFlightsVec->Size == _filteredFlights->Size)
	{
		return;
	}

	FilteredFlights = filteredFlightsVec;
}

