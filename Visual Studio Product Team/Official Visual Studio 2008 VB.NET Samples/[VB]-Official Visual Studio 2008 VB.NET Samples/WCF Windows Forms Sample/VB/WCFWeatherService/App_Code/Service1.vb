Imports System
Imports System.ServiceModel
Imports System.Runtime.Serialization
Imports System.Collections

' A WCF service consists of a contract (defined below as IService1), 
' a class which implements that interface (see Service1), 
' and configuration entries that specify behaviors associated with 
' that implementation (see <system.serviceModel> in web.config)

Namespace Microsoft.ServiceModel.Samples


    <ServiceContract(Namespace:="http://Microsoft.ServiceModel.Samples")> _
    Public Interface IService1


        <OperationContract()> _
            Function GetWeatherData(ByVal localities As String()) As WeatherData()


        ' TODO: Add your service operations here

    End Interface

    ' Use a data contract as illustrated in the sample below to add composite 
    ' types to service operations
    Public Class Service1
        Implements IService1

        Public Sub New()
        End Sub




        Public Function GetWeatherData(ByVal localities As String()) As WeatherData() Implements IService1.GetWeatherData

            Dim weatherArray As New ArrayList()
            For Each locality As String In localities

                Dim data As New WeatherData(locality)
                weatherArray.Add(data)

            Next
            Return DirectCast(weatherArray.ToArray(GetType(WeatherData)), WeatherData())

        End Function

    End Class


    <DataContract(Namespace:="http://Microsoft.ServiceModel.Samples")> _
    Public Class WeatherData

        Public Sub New(ByVal locality As String)

            Dim rand As New Random()
            Dim highDelta As Integer = rand.Next(3)
            Dim lowDelta As Integer = rand.Next(3)
            Select Case locality

                Case "Los Angeles"
                    Me.Locality = "Los Angeles"
                    HighTemperature = 85 + highDelta
                    LowTemperature = 60 + lowDelta
                    Exit Select
                Case "Rio de Janeiro"
                    Me.Locality = "Rio de Janeiro"
                    HighTemperature = 55 + highDelta
                    LowTemperature = 40 + lowDelta
                    Exit Select
                Case "New York"
                    Me.Locality = "New York"
                    HighTemperature = 80 + highDelta
                    LowTemperature = 55 + lowDelta
                    Exit Select
                Case "London"
                    Me.Locality = "London"
                    HighTemperature = 65 + highDelta
                    LowTemperature = 45 + lowDelta
                    Exit Select
                Case "Paris"
                    Me.Locality = "Paris"
                    HighTemperature = 70 + highDelta
                    LowTemperature = 50 + lowDelta
                    Exit Select
                Case "Rome"
                    Me.Locality = "Rome"
                    HighTemperature = 80 + highDelta
                    LowTemperature = 60 + lowDelta
                    Exit Select
                Case "Cairo"
                    Me.Locality = "Cairo"
                    HighTemperature = 90 + highDelta
                    LowTemperature = 70 + lowDelta
                    Exit Select
                Case "Beijing"
                    Me.Locality = "Beijing"
                    HighTemperature = 85 + highDelta
                    LowTemperature = 60 + lowDelta
                    Exit Select

            End Select
            If HighTemperature < LowTemperature Then

                Dim temp As Integer = HighTemperature
                HighTemperature = LowTemperature
                LowTemperature = temp

            End If
            If HighTemperature < (LowTemperature - 8) Then

                HighTemperature += 5

            End If

        End Sub

        Private _highTemp As Integer

        <DataMember()> _
        Public Property HighTemperature() As Integer

            Get

                Return _highTemp

            End Get
            Set(ByVal value As Integer)

                _highTemp = value

            End Set

        End Property

        Private _lowTemp As Integer

        <DataMember()> _
        Public Property LowTemperature() As Integer

            Get

                Return _lowTemp

            End Get
            Set(ByVal value As Integer)

                _lowTemp = value

            End Set

        End Property

        Private _locality As String = "Los Angeles"

        <DataMember()> _
        Public Property Locality() As String

            Get

                Return _locality

            End Get
            Set(ByVal value As String)

                _locality = value

            End Set

        End Property

    End Class



End Namespace