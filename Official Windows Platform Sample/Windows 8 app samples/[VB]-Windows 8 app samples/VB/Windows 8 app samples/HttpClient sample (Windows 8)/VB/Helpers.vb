Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Net.Http
Imports System.Text
Imports System.Threading.Tasks
Imports Windows.UI.Xaml.Controls

Friend NotInheritable Class Helpers
    Private Sub New()
    End Sub
    Friend Shared Async Function DisplayTextResult(response As HttpResponseMessage, output As TextBox) As Task
        Dim responseBodyAsText As String
        output.Text &= response.StatusCode & " " & response.ReasonPhrase & Environment.NewLine
        responseBodyAsText = Await response.Content.ReadAsStringAsync()
        responseBodyAsText = responseBodyAsText.Replace("<br>", Environment.NewLine)
        ' Insert new lines
        output.Text &= responseBodyAsText
    End Function
End Class

