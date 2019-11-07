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
Partial Public Class ForecastPage
    Inherits PhoneApplicationPage
    Private forecast As Forecast

    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Event handler to handle when this page is navigated to
    ''' </summary>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        ' get the city, latitude, and longitude out of the query string 
        Dim cityName As String = Me.NavigationContext.QueryString("City")
        Dim latitude As String = Me.NavigationContext.QueryString("Latitude")
        Dim longitude As String = Me.NavigationContext.QueryString("Longitude")

        forecast = New Forecast()

        ' get the forecast for the given latitude and longitude
        forecast.GetForecast(latitude, longitude)

        ' set data context for page to this forecast
        DataContext = forecast

        ' set source for ForecastList box in page to our list of forecast time periods
        ForecastList.ItemsSource = forecast.ForecastList
    End Sub


    ''' <summary>
    ''' Make sure no item can be selected in the forecast list box
    ''' </summary>
    Private Sub ForecastList_SelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        ForecastList.SelectedIndex = -1
        ForecastList.SelectedItem = Nothing

    End Sub

End Class
