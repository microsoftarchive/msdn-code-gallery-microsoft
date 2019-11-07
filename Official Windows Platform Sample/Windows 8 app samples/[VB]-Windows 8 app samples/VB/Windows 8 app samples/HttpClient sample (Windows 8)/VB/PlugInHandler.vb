Imports System.Net.Http
Imports System.Threading

Public Class PlugInHandler
    Inherits MessageProcessingHandler
    Public Sub New(innerHandler As HttpMessageHandler)
        MyBase.New(innerHandler)
    End Sub

    ' Process the request before sending it
    Protected Overrides Function ProcessRequest(request As HttpRequestMessage, cancellationToken As CancellationToken) As HttpRequestMessage
        If request.Method = HttpMethod.Get Then
            request.Headers.Add("Custom-Header", "CustomRequestValue")
        End If
        Return request
    End Function

    ' Process the response before returning it to the user
    Protected Overrides Function ProcessResponse(response As HttpResponseMessage, cancellationToken As CancellationToken) As HttpResponseMessage
        If response.RequestMessage.Method = HttpMethod.Get Then
            response.Headers.Add("Custom-Header", "CustomResponseValue")
        End If
        Return response
    End Function
End Class
