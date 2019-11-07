/****************************** Module Header ******************************\
 * Module Name:  MainViewModel.cs
 * Project:      CSWindowsStoreAppFlightDataFilter
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
using FlightDataFilter.DataModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Windows.Data.Json;
using Windows.Storage;

namespace FlightDataFilter.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            //  Initialize Our Collections
            Flights = new ObservableCollection<FlightDataItem>();
            LoadFlightData();
            FilteredFlights = Flights;
        }

        private ObservableCollection<FlightDataItem> _flights;

        public ObservableCollection<FlightDataItem> Flights
        {
            get { return _flights; }
            set { _flights = value; NotifyPropertyChanged("Flights"); }
        }

        private ObservableCollection<FlightDataItem> _filteredFlights;

        public ObservableCollection<FlightDataItem> FilteredFlights
        {
            get { return _filteredFlights; }
            set { _filteredFlights = value; NotifyPropertyChanged("FilteredFlights"); }
        }

        private double _selectedPrice;

        public double SelectedPrice
        {
            get { return _selectedPrice; }
            set { _selectedPrice = value; NotifyPropertyChanged("SelectedPrice"); RefreshFilteredData(); }
        }


        private void RefreshFilteredData()
        {
            var fr = from fobjs in Flights
                     where fobjs.Price <= SelectedPrice
                     select fobjs;

            //  This will limit the amount of view refreshes
            if (FilteredFlights.Count == fr.Count())
                return;

            FilteredFlights = new ObservableCollection<FlightDataItem>(fr);
        }


        private async void LoadFlightData()
        {
            if (this._flights.Count != 0)
                return;

            Uri dataUri = new Uri("ms-appx:///DataModel/FlightData.json");

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            string jsonText = await FileIO.ReadTextAsync(file);
            JsonObject jsonObject = JsonObject.Parse(jsonText);
            JsonArray jsonArray = jsonObject["Flights"].GetArray();
            double minPrice = 0;
            double maxPrice = 0;

            foreach (JsonValue flight in jsonArray)
            {
                JsonObject flightObject = flight.GetObject();
                Flights.Add(new FlightDataItem(flightObject["DepartureCity"].GetString(),
                                                       flightObject["DestinationCity"].GetString(),
                                                       flightObject["Price"].GetNumber(),
                                                       flightObject["DepartureTime"].GetString(),
                                                       flightObject["ArrivalTime"].GetString(),
                                                       Convert.ToInt32(flightObject["NumberOfConnections"].GetNumber())));

                if (minPrice == 0 || flightObject["Price"].GetNumber() < minPrice)
                    minPrice = flightObject["Price"].GetNumber();

                if (maxPrice == 0 || flightObject["Price"].GetNumber() > maxPrice)
                    maxPrice = flightObject["Price"].GetNumber();


            }

            SelectedPrice = maxPrice;

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }

}
