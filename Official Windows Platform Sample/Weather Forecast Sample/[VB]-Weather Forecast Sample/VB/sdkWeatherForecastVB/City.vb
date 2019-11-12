'
'   Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
'   Use of this sample source code is subject to the terms of the Microsoft license 
'   agreement under which you licensed this sample source code and is provided AS-IS.
'   If you did not accept the terms of the license agreement, you are not authorized 
'   to use this sample source code.  For the terms of the license, please see the 
'   license agreement between you and Microsoft.
'  
'   To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
'  
'
Imports System.ComponentModel


Public Class City
    Implements INotifyPropertyChanged

    Private _cityName As String

    Private _latitude As String

    Private _longitude As String


    Public Property CityName() As String
        Get
            Return _cityName
        End Get
        Set(ByVal value As String)
            If value <> _cityName Then
                _cityName = value
                NotifyPropertyChanged("CityName")
            End If
        End Set
    End Property

    Public Property Latitude() As String
        Get
            Return _latitude
        End Get
        Set(ByVal value As String)
            If value <> _latitude Then
                _latitude = value
                NotifyPropertyChanged("Latitude")
            End If
        End Set
    End Property

    Public Property Longitude() As String
        Get
            Return _longitude
        End Get
        Set(ByVal value As String)
            If value <> _longitude Then
                _longitude = value
                NotifyPropertyChanged("Longitude")
            End If
        End Set
    End Property


    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged


    ''' <summary>
    ''' Constructor with full city information
    ''' </summary>
    Public Sub New(ByVal cityName As String, ByVal latitude As String, ByVal longitude As String)
        Me.CityName = cityName
        Me.Latitude = latitude
        Me.Longitude = longitude
    End Sub

    ''' <summary>
    ''' Raise the PropertyChanged event and pass along the property that changed
    ''' </summary>
    Private Sub NotifyPropertyChanged(ByVal [property] As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs([property]))
    End Sub


End Class
