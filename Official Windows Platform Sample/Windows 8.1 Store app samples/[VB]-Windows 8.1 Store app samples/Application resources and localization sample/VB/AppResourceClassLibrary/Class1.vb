Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks

Imports Windows.ApplicationModel.Resources
Namespace AppResourceClassLibrary
    Public NotInheritable Class LocalizedNamesLibrary

        Private Sub New()
        End Sub
        Private Shared resourceLoader As ResourceLoader = Nothing

        Public Shared ReadOnly Property LibraryName() As String
            Get
                Dim name As String = Nothing
                GetLibraryName("string1", name)
                Return name
            End Get
        End Property

        Private Shared Sub GetLibraryName(ByVal resourceName As String, ByRef resourceValue As String)
            If resourceLoader Is Nothing Then
                resourceLoader = ResourceLoader.GetForCurrentView("AppResourceClassLibrary/Resources")
            End If
            resourceValue = resourceLoader.GetString(resourceName)
        End Sub

    End Class
End Namespace
