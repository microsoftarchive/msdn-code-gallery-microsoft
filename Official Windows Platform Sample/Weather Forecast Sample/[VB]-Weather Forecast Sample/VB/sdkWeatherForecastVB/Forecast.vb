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
Imports System.Collections.ObjectModel
Imports System.IO
Imports System.Xml.Linq



Public Class Forecast
    Implements INotifyPropertyChanged

#Region "member variables"

    ' name of city forecast is for

    Private _cityName As String
    ' elevation of city

    Private _height As Integer
    ' source of information

    Private _credit As String

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

#End Region


#Region "Accessors"

    ' collection of forecasts for each time period
    Public Property ForecastList() As ObservableCollection(Of ForecastPeriod)

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

    Public Property Height() As Integer
        Get
            Return _height
        End Get
        Set(ByVal value As Integer)
            If value <> _height Then
                _height = value
                NotifyPropertyChanged("Height")
            End If
        End Set
    End Property

    Public Property Credit() As String
        Get
            Return _credit
        End Get
        Set(ByVal value As String)
            If value <> _credit Then
                _credit = value
                NotifyPropertyChanged("Credit")
            End If
        End Set
    End Property

#End Region


#Region "constructors"

    Public Sub New()
        ForecastList = New ObservableCollection(Of ForecastPeriod)()
    End Sub

#End Region


#Region "private Helpers"

    ''' <summary>
    ''' Raise the PropertyChanged event and pass along the property that changed
    ''' </summary>
    Private Sub NotifyPropertyChanged(ByVal [property] As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs([property]))
    End Sub

#End Region

    ''' <summary>
    ''' Get a forecast for the given latitude and longitude
    ''' </summary>
    Public Sub GetForecast(ByVal latitude As String, ByVal longitude As String)
        ' form the URI
        Dim fullUri As New UriBuilder("http://forecast.weather.gov/MapClick.php")
        fullUri.Query = "lat=" & latitude & "&lon=" & longitude & "&FcstType=dwml"

        ' initialize a new WebRequest
        Dim forecastRequest As HttpWebRequest = CType(WebRequest.Create(fullUri.Uri), HttpWebRequest)

        ' set up the state object for the async request
        Dim forecastState As New ForecastUpdateState()
        forecastState.AsyncRequest = forecastRequest

        ' start the asynchronous request
        forecastRequest.BeginGetResponse(New AsyncCallback(AddressOf HandleForecastResponse), forecastState)
    End Sub

    ''' <summary>
    ''' Handle the information returned from the async request
    ''' </summary>
    ''' <param name="asyncResult"></param>
    Private Sub HandleForecastResponse(ByVal asyncResult As IAsyncResult)
        ' get the state information
        Dim forecastState As ForecastUpdateState = CType(asyncResult.AsyncState, ForecastUpdateState)
        Dim forecastRequest As HttpWebRequest = CType(forecastState.AsyncRequest, HttpWebRequest)

        ' end the async request
        forecastState.AsyncResponse = CType(forecastRequest.EndGetResponse(asyncResult), HttpWebResponse)

        Dim streamResult As Stream

        Dim newCredit As String = ""
        Dim newCityName As String = ""
        Dim newHeight As Integer = 0

        ' create a temp collection for the new forecast information for each 
        ' time period
        Dim newForecastList As New ObservableCollection(Of ForecastPeriod)()

        Try
            ' get the stream containing the response from the async call
            streamResult = forecastState.AsyncResponse.GetResponseStream()

            ' load the XML
            Dim xmlWeather As XElement = XElement.Load(streamResult)

            ' start parsing the XML.  You can see what the XML looks like by 
            ' browsing to: 
            ' http://forecast.weather.gov/MapClick.php?lat=44.52160&lon=-87.98980&FcstType=dwml

            ' find the source element and retrieve the credit information
            Dim xmlCurrent As XElement = xmlWeather.Descendants("source").First()
            newCredit = CStr(xmlCurrent.Element("credit"))

            ' find the source element and retrieve the city and elevation information
            xmlCurrent = xmlWeather.Descendants("location").First()
            newCityName = CStr(xmlCurrent.Element("city"))
            Dim temp = CInt(xmlCurrent.Element("height"))
            newHeight = CInt(Fix(temp))

            ' find the first time layout element
            xmlCurrent = xmlWeather.Descendants("time-layout").First()

            Dim timeIndex As Integer = 1

            ' search through the time layout elements until you find a node 
            ' contains at least 12 time periods of information. Other nodes can be ignored
            Do While xmlCurrent.Elements("start-valid-time").Count() < 12
                xmlCurrent = xmlWeather.Descendants("time-layout").ElementAt(timeIndex)
                timeIndex += 1
            Loop

            Dim newPeriod As ForecastPeriod

            ' For each time period element, create a new forecast object and store
            ' the time period name.
            ' Time periods vary depending on the time of day the data is fetched.  
            ' You may get "Today", "Tonight", "Monday", "Monday Night", etc
            ' or you may get "Tonight", "Monday", "Monday Night", etc
            ' or you may get "This Afternoon", "Tonight", "Monday", "Monday Night", etc
            For Each curElement In xmlCurrent.Elements("start-valid-time")
                Try
                    newPeriod = New ForecastPeriod()
                    newPeriod.TimeName = CStr(curElement.Attribute("period-name"))
                    newForecastList.Add(newPeriod)
                Catch e1 As FormatException

                End Try
            Next curElement

            ' now read in the weather data for each time period
            GetMinMaxTemperatures(xmlWeather, newForecastList)
            GetChancePrecipitation(xmlWeather, newForecastList)
            GetCurrentConditions(xmlWeather, newForecastList)
            GetWeatherIcon(xmlWeather, newForecastList)
            GetTextForecast(xmlWeather, newForecastList)


            ' copy the data over
            ' copy forecast object over
            ' copy the list of forecast time periods over
            Deployment.Current.Dispatcher.BeginInvoke(Sub()
                                                          Credit = newCredit
                                                          Height = newHeight
                                                          CityName = newCityName
                                                          ForecastList.Clear()
                                                          For Each forecastPeriod In newForecastList
                                                              ForecastList.Add(forecastPeriod)
                                                          Next forecastPeriod
                                                      End Sub)


        Catch e2 As FormatException
            ' there was some kind of error processing the response from the web
            ' additional error handling would normally be added here
            Return
        End Try

    End Sub


    ''' <summary>
    ''' Get the minimum and maximum temperatures for all the time periods
    ''' </summary>
    Private Sub GetMinMaxTemperatures(ByVal xmlWeather As XElement, ByVal newForecastList As ObservableCollection(Of ForecastPeriod))
        Dim xmlCurrent As XElement

        ' Find the temperature parameters.   if first time period is "Tonight",
        ' then the Daily Minimum Temperatures are listed first.
        ' Otherwise the Daily Maximum Temperatures are listed first
        xmlCurrent = xmlWeather.Descendants("parameters").First()

        Dim minTemperatureIndex As Integer = 1
        Dim maxTemperatureIndex As Integer = 0

        ' If "Tonight" is the first time period, then store Daily Minimum 
        ' Temperatures first, then Daily Maximum Temperatuers next
        If newForecastList.ElementAt(0).TimeName = "Tonight" Then
            minTemperatureIndex = 0
            maxTemperatureIndex = 1

            ' get the Daily Minimum Temperatures
            For Each curElement In xmlCurrent.Elements("temperature").ElementAt(0).Elements("value")
                newForecastList.ElementAt(minTemperatureIndex).Temperature = Integer.Parse(curElement.Value)

                minTemperatureIndex += 2
            Next curElement

            ' then get the Daily Maximum Temperatures
            For Each curElement In xmlCurrent.Elements("temperature").ElementAt(1).Elements("value")
                newForecastList.ElementAt(maxTemperatureIndex).Temperature = Integer.Parse(curElement.Value)

                maxTemperatureIndex += 2
            Next curElement

            ' otherwise we have a daytime time period first
        Else
            ' get the Daily Maximum Temperatures
            For Each curElement In xmlCurrent.Elements("temperature").ElementAt(0).Elements("value")
                newForecastList.ElementAt(maxTemperatureIndex).Temperature = Integer.Parse(curElement.Value)

                maxTemperatureIndex += 2
            Next curElement

            ' then get the Daily Minimum Temperatures
            For Each curElement In xmlCurrent.Elements("temperature").ElementAt(1).Elements("value")
                newForecastList.ElementAt(minTemperatureIndex).Temperature = Integer.Parse(curElement.Value)

                minTemperatureIndex += 2
            Next curElement
        End If


    End Sub


    ''' <summary>
    ''' Get the chance of precipitation for all the time periods
    ''' </summary>
    Private Sub GetChancePrecipitation(ByVal xmlWeather As XElement, ByVal newForecastList As ObservableCollection(Of ForecastPeriod))
        Dim xmlCurrent As XElement

        ' now find the probablity of precipitation for each time period
        xmlCurrent = xmlWeather.Descendants("probability-of-precipitation").First()

        Dim elementIndex As Integer = 0

        For Each curElement In xmlCurrent.Elements("value")
            Try
                newForecastList.ElementAt(elementIndex).ChancePrecipitation = Integer.Parse(curElement.Value)
                ' some values are nil
            Catch e1 As FormatException
                newForecastList.ElementAt(elementIndex).ChancePrecipitation = 0
            End Try

            elementIndex += 1
        Next curElement

    End Sub


    ''' <summary>
    ''' Get the current conditions for all the time periods
    ''' </summary>
    Private Sub GetCurrentConditions(ByVal xmlWeather As XElement, ByVal newForecastList As ObservableCollection(Of ForecastPeriod))
        Dim xmlCurrent As XElement
        Dim elementIndex As Integer = 0

        ' now get the current weather conditions for each time period
        xmlCurrent = xmlWeather.Descendants("weather").First()

        For Each curElement In xmlCurrent.Elements("weather-conditions")
            Try
                newForecastList.ElementAt(elementIndex).WeatherType = CStr(curElement.Attribute("weather-summary"))
            Catch e1 As FormatException
                newForecastList.ElementAt(elementIndex).WeatherType = ""
            End Try

            elementIndex += 1
        Next curElement


    End Sub


    ''' <summary>
    ''' Get the link to the weather icon for all the time periods
    ''' </summary>
    ''' <param name="xmlWeather"></param>
    ''' <param name="newForecastList"></param>
    Private Sub GetWeatherIcon(ByVal xmlWeather As XElement, ByVal newForecastList As ObservableCollection(Of ForecastPeriod))
        Dim xmlCurrent As XElement
        Dim elementIndex As Integer = 0

        ' get a link to the weather icon for each time period
        xmlCurrent = xmlWeather.Descendants("conditions-icon").First()

        For Each curElement In xmlCurrent.Elements("icon-link")
            Try
                newForecastList.ElementAt(elementIndex).ConditionIcon = CStr(curElement.Value)
            Catch e1 As FormatException
                newForecastList.ElementAt(elementIndex).ConditionIcon = ""
            End Try

            elementIndex += 1
        Next curElement

    End Sub

    ''' <summary>
    ''' Get the long text forecast for all the time periods
    ''' </summary>
    Private Sub GetTextForecast(ByVal xmlWeather As XElement, ByVal newForecastList As ObservableCollection(Of ForecastPeriod))
        Dim xmlCurrent As XElement
        Dim elementIndex As Integer = 0

        ' get a text forecast for each time period
        xmlCurrent = xmlWeather.Descendants("wordedForecast").First()

        For Each curElement In xmlCurrent.Elements("text")
            Try
                newForecastList.ElementAt(elementIndex).TextForecast = CStr(curElement.Value)
            Catch e1 As FormatException
                newForecastList.ElementAt(elementIndex).TextForecast = ""
            End Try

            elementIndex += 1
        Next curElement

    End Sub

End Class


''' <summary>
''' State information for our BeginGetResponse async call
''' </summary>
Public Class ForecastUpdateState
    Public Property AsyncRequest() As HttpWebRequest
    Public Property AsyncResponse() As HttpWebResponse
End Class
