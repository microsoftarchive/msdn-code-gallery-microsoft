Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks

Imports Windows.ApplicationModel.Resources

Public Module LocalizedNamesLibrary
    Private resourceLoader As ResourceLoader = Nothing

    Public ReadOnly Property LibraryName() As String
        Get
            Dim name As String = ""
            GetLibraryName("string1", name)
            Return name
        End Get
    End Property

    Private Sub GetLibraryName(resourceName As String, ByRef resourceValue As String)
        If ResourceLoader Is Nothing Then
            ResourceLoader = New ResourceLoader("AppResourceClassLibraryVB/Resources")
        End If
        resourceValue = ResourceLoader.GetString(resourceName)
    End Sub

End Module


