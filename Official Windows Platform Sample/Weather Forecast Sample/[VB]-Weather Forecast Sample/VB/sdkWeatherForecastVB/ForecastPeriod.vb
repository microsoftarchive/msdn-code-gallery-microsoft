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



''' <summary>
''' Class for holding the forecast for a particular time period
''' </summary>
Public Class ForecastPeriod
    Implements INotifyPropertyChanged
#Region "member variables"

    Private _timeName As String

    Private _temperature As Integer

    Private _chancePrecipitation As Integer

    Private _weatherType As String

    Private _textForecast As String

    Private _conditionIcon As String

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

#End Region

    Public Sub New()
    End Sub


    Public Property TimeName() As String
        Get
            Return _timeName
        End Get
        Set(ByVal value As String)
            If value <> _timeName Then
                Me._timeName = value
                NotifyPropertyChanged("TimeName")
            End If
        End Set
    End Property

    Public Property Temperature() As Integer
        Get
            Return _temperature
        End Get
        Set(ByVal value As Integer)
            If value <> _temperature Then
                Me._temperature = value
                NotifyPropertyChanged("Temperature")
            End If
        End Set
    End Property


    Public Property ChancePrecipitation() As Integer
        Get
            Return _chancePrecipitation
        End Get
        Set(ByVal value As Integer)
            If value <> _chancePrecipitation Then
                Me._chancePrecipitation = value
                NotifyPropertyChanged("ChancePrecipitation")
            End If
        End Set
    End Property

    Public Property WeatherType() As String
        Get
            Return _weatherType
        End Get
        Set(ByVal value As String)
            If value <> _weatherType Then
                Me._weatherType = value
                NotifyPropertyChanged("WeatherType")
            End If
        End Set
    End Property

    Public Property TextForecast() As String
        Get
            Return _textForecast
        End Get
        Set(ByVal value As String)
            If value <> _textForecast Then
                Me._textForecast = value
                NotifyPropertyChanged("TextForecast")
            End If
        End Set
    End Property

    Public Property ConditionIcon() As String
        Get
            Return _conditionIcon
        End Get
        Set(ByVal value As String)
            If value <> _conditionIcon Then
                Me._conditionIcon = value
                NotifyPropertyChanged("ConditionIcon")
            End If
        End Set
    End Property

    Private Sub NotifyPropertyChanged(ByVal [property] As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs([property]))
    End Sub

End Class
