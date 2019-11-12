'****************************** Module Header ******************************\
' Module Name:  MainViewModel.vb
' Project:      VBWindowsStoreAppFlightDataFilter
' Copyright (c) Microsoft Corporation.
'
' MainViewModel. 
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

Imports VBWindowsStoreAppFlightDataFilter.DataModel
Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Linq
Imports Windows.Data.Json
Imports Windows.Storage

Namespace ViewModel
    Public Class MainViewModel
        Implements INotifyPropertyChanged

        Public Sub New()
            '  Initialize Our Collections
            Flights = New ObservableCollection(Of FlightDataItem)()
            LoadFlightData()
            FilteredFlights = Flights
        End Sub

        Private _flights As ObservableCollection(Of FlightDataItem)

        Public Property Flights() As ObservableCollection(Of FlightDataItem)
            Get
                Return _flights
            End Get
            Set(value As ObservableCollection(Of FlightDataItem))
                _flights = value
                NotifyPropertyChanged("Flights")
            End Set
        End Property

        Private _filteredFlights As ObservableCollection(Of FlightDataItem)

        Public Property FilteredFlights() As ObservableCollection(Of FlightDataItem)
            Get
                Return _filteredFlights
            End Get
            Set(value As ObservableCollection(Of FlightDataItem))
                _filteredFlights = value
                NotifyPropertyChanged("FilteredFlights")
            End Set
        End Property

        Private _selectedPrice As Double

        Public Property SelectedPrice() As Double
            Get
                Return _selectedPrice
            End Get
            Set(value As Double)
                _selectedPrice = value
                NotifyPropertyChanged("SelectedPrice")
                RefreshFilteredData()
            End Set
        End Property


        Private Sub RefreshFilteredData()
            Dim fr = From fobjs In Flights Where fobjs.Price <= SelectedPrice Select fobjs
            '  This will limit the amount of view refreshes
            If FilteredFlights.Count = fr.Count() Then
                Return
            End If

            FilteredFlights = New ObservableCollection(Of FlightDataItem)(fr)
        End Sub


        Private Async Sub LoadFlightData()
            If Me._flights.Count <> 0 Then
                Return
            End If

            Dim dataUri As New Uri("ms-appx:///DataModel/FlightData.json")

            Dim file As StorageFile = Await StorageFile.GetFileFromApplicationUriAsync(dataUri)
            Dim jsonText As String = Await FileIO.ReadTextAsync(file)
            Dim _jsonObject As JsonObject = JsonObject.Parse(jsonText)
            Dim jsonArray As JsonArray = _jsonObject("Flights").GetArray()
            Dim minPrice As Double = 0
            Dim maxPrice As Double = 0

            For Each flight As JsonValue In jsonArray
                Dim flightObject As JsonObject = flight.GetObject()
                Flights.Add(New FlightDataItem(flightObject("DepartureCity").GetString(), flightObject("DestinationCity").GetString(), flightObject("Price").GetNumber(), flightObject("DepartureTime").GetString(), flightObject("ArrivalTime").GetString(), Convert.ToInt32(flightObject("NumberOfConnections").GetNumber())))

                If minPrice = 0 OrElse flightObject("Price").GetNumber() < minPrice Then
                    minPrice = flightObject("Price").GetNumber()
                End If

                If maxPrice = 0 OrElse flightObject("Price").GetNumber() > maxPrice Then
                    maxPrice = flightObject("Price").GetNumber()


                End If
            Next

            SelectedPrice = maxPrice

        End Sub

        Private Sub NotifyPropertyChanged(propName As String)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propName))
        End Sub

        Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
    End Class

End Namespace
