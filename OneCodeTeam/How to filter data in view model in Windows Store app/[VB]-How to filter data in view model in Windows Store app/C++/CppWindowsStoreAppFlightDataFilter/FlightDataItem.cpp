/****************************** Module Header ******************************\
* Module Name:  FlightDataItem.cpp
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
#include "pch.h"
#include "FlightDataItem.h"

using namespace Platform;
using namespace CppWindowsStoreAppFlightDataFilter::Data;

FlightDataItem::FlightDataItem(Platform::String^ departureCity, Platform::String^ destCity, double price, Platform::String^ departTime,
	Platform::String^ arrivalTime, int numConnections):
	_departureCity(departureCity),
	_destCity(destCity),
	_price(price),
	_departTime(departTime),
	_arrivalTime(arrivalTime),
	_numConnections(numConnections)
{
}

String^ FlightDataItem::DepartureCity::get()
{
	return _departureCity;
}

String^ FlightDataItem::DestinationCity::get()
{
	return _destCity;
}

double FlightDataItem::Price::get()
{
	return _price;
}

String^ FlightDataItem::DepartureTime::get()
{
	return _departTime;
}

String^ FlightDataItem::ArrivalTime::get()
{
	return _arrivalTime;
}

int FlightDataItem::NumberOfConnections::get()
{
	return _numConnections;
}

Windows::UI::Xaml::Data::ICustomProperty^ FlightDataItem::GetCustomProperty(Platform::String^ name)
{
	return nullptr;
}

Windows::UI::Xaml::Data::ICustomProperty^ FlightDataItem::GetIndexedProperty(Platform::String^ name, Windows::UI::Xaml::Interop::TypeName type)
{
	return nullptr;
}

Platform::String^ FlightDataItem::GetStringRepresentation()
{
	return DepartureCity;
}

Windows::UI::Xaml::Interop::TypeName FlightDataItem::Type::get()
{
	return this->GetType();
}

