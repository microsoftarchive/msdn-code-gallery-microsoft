Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks

Imports Windows.ApplicationModel.Resources
Public NotInheritable Class LocalizedNamesLibrary
    Private Sub New()
    End Sub
    Shared resourceLoader As ResourceLoader = Nothing

    Public Shared ReadOnly Property LibraryName() As String
        Get
            Dim strname As String = ""
            GetLibraryName("string1", strname)
            Return strname
        End Get
    End Property

    Private Shared Sub GetLibraryName(resourceName As String, resourceValue As String)
        If resourceLoader Is Nothing Then
            resourceLoader = New ResourceLoader("AppResourceClassLibrary/Resources")
        End If
        resourceValue = resourceLoader.GetString(resourceName)
    End Sub

End Class
