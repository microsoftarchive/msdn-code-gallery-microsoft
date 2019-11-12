Imports Microsoft.SharePoint

Namespace Client

    'This class is the proxy that runs on Web-Front End
    'servers. Web parts etc use this class to call the
    'Service Application and call its methods.
    Public NotInheritable Class DayNamerServiceClient

        Private _serviceContext As SPServiceContext
        Private _result As String

        Public Sub New(ByVal serviceContext As SPServiceContext)
            If serviceContext Is Nothing Then
                Throw New ArgumentNullException("serviceContext")
            End If

            _serviceContext = serviceContext

        End Sub

        'Implement the methods defined in the contract
        Public Function TodayIs() As String
            _result = String.Empty

            'Run the call against the application proxy
            DayNamerServiceApplicationProxy.Invoke(_serviceContext, AddressOf todayIsCallback)

            Return _result
        End Function

        Public Sub todayIsCallback(ByVal appProxy As DayNamerServiceApplicationProxy)
            _result = appProxy.TodayIs()
        End Sub

        Public Function TodayAdd(ByVal daysToAdd As Integer) As String
            _result = String.Empty

            'Run the call against the application proxy
            DayNamerServiceApplicationProxy.Invoke(_serviceContext, AddressOf todayAddCallback)

            Return _result
        End Function

        Public Sub todayAddCallback(ByVal appProxy As DayNamerServiceApplicationProxy)
            _result = appProxy.TodayAdd(10)
        End Sub

    End Class

End Namespace
