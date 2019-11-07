Imports System
Imports System.ServiceModel
Imports System.Runtime.Serialization


' A WCF service consists of a contract (defined below as IPredictorService), 
' a class which implements that interface (see PredictorService), 
' and configuration entries that specify behaviors associated with 
' that implementation (see <system.serviceModel> in web.config)
<ServiceContract([Namespace]:="Microsoft.ServiceReference.Samples")> _
Public Interface IPredictorService

    <OperationContract()> _
    Function Ask(ByVal question As String) As String


End Interface

Public Class PredictorService
    Implements IPredictorService

    Dim rand As New System.Random
    Dim qResponse() As String = {"No Way!", "Calm down and ask again", "Of Course!", "Ask me later", "Yawn", "Never"}
    Dim i As Integer = 0

    Public Function Ask(ByVal qData As String) As String Implements IPredictorService.Ask
        If qData.Equals("") Then Return "Type louder, I didn't hear you."
        Return qResponse(rand.Next(0, 6))
    End Function

End Class

