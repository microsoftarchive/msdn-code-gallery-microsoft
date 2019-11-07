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
Partial Public Class MainPage
    Inherits PhoneApplicationPage
    ' Constructor
    Public Sub New()
        InitializeComponent()
        CityList.ItemsSource = App.cityList
    End Sub

    ''' <summary>
    ''' Event handler called when user selects a city to get a forecast for
    ''' </summary>
    Private Sub CityList_SelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        ' if an item is selected
        If CityList.SelectedIndex <> -1 Then
            ' get the currently selected city and pass the information to the 
            ' forecast details page
            Dim curCity As City = CType(CityList.SelectedItem, City)
            Me.NavigationService.Navigate(New Uri("/ForecastPage.xaml?City=" & curCity.CityName & "&Latitude=" & curCity.Latitude & "&Longitude=" & curCity.Longitude, UriKind.Relative))
        End If

    End Sub

    ''' <summary>
    ''' Event handler called when user navigates away from this page
    ''' </summary>
    Protected Overrides Sub OnNavigatedFrom(ByVal args As NavigationEventArgs)
        ' make sure no item is highlighted in the list of cities
        CityList.SelectedIndex = -1
        CityList.SelectedItem = Nothing
    End Sub

End Class
