Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols

<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class PredictorService
    Inherits System.Web.Services.WebService

    Dim rand As New System.Random
    Dim qResponse() As String = {"No Way!", "Calm down and ask again", "Of Course!", "Ask me later", "Yawn", "Never"}

    <WebMethod()> _
    Public Function Ask(ByVal qData As String) As String
        If qData.Equals("") Then Return "Type louder, I didn't hear you."
        Return qResponse(rand.Next(0, 6))
    End Function
End Class
