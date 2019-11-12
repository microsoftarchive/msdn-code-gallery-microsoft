/****************************** Module Header ******************************\
 * Module Name:  FlightDataItem.cs
 * Project:      CSWindowsStoreAppFlightDataFilter
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
using System;

namespace FlightDataFilter.DataModel
{
    public class FlightDataItem
    {
        public FlightDataItem(String departureCity, String destCity, double price, string departTime, string arrivalTime, int numConnections)
        {
            this.DepartureCity = departureCity;
            this.DepartureTime = departTime;
            this.DestinationCity = destCity;
            this.ArrivalTime = arrivalTime;
            this.Price = price;
            this.NumberOfConnections = numConnections;
        }

        public string DepartureCity { get; private set; }
        public string DestinationCity { get; private set; }
        public string DepartureTime { get; private set; }
        public string ArrivalTime { get; private set; }
        public double Price { get; private set; }
        public int NumberOfConnections { get; private set; }

    }
}
