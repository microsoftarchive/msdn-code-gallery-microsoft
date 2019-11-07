'****************************** Module Header ******************************\
' Module Name:  FlightDataItem.vb
' Project:      VBWindowsStoreAppFlightDataFilter
' Copyright (c) Microsoft Corporation.
'
' FlightDataItem. 
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

Namespace DataModel
    Public Class FlightDataItem
        Public Sub New(departureCity As [String], destCity As [String], price As Double, departTime As String, arrivalTime As String, numConnections As Integer)
            Me.DepartureCity = departureCity
            Me.DepartureTime = departTime
            Me.DestinationCity = destCity
            Me.ArrivalTime = arrivalTime
            Me.Price = price
            Me.NumberOfConnections = numConnections
        End Sub

        Public Property DepartureCity() As String
            Get
                Return m_DepartureCity
            End Get
            Private Set(value As String)
                m_DepartureCity = value
            End Set
        End Property
        Private m_DepartureCity As String
        Public Property DestinationCity() As String
            Get
                Return m_DestinationCity
            End Get
            Private Set(value As String)
                m_DestinationCity = value
            End Set
        End Property
        Private m_DestinationCity As String
        Public Property DepartureTime() As String
            Get
                Return m_DepartureTime
            End Get
            Private Set(value As String)
                m_DepartureTime = value
            End Set
        End Property
        Private m_DepartureTime As String
        Public Property ArrivalTime() As String
            Get
                Return m_ArrivalTime
            End Get
            Private Set(value As String)
                m_ArrivalTime = value
            End Set
        End Property
        Private m_ArrivalTime As String
        Public Property Price() As Double
            Get
                Return m_Price
            End Get
            Private Set(value As Double)
                m_Price = value
            End Set
        End Property
        Private m_Price As Double
        Public Property NumberOfConnections() As Integer
            Get
                Return m_NumberOfConnections
            End Get
            Private Set(value As Integer)
                m_NumberOfConnections = value
            End Set
        End Property
        Private m_NumberOfConnections As Integer

    End Class
End Namespace