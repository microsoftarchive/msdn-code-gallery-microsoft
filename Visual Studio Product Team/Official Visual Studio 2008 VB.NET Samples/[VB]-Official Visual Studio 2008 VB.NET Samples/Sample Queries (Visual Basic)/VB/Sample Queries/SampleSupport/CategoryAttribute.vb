Imports System

Namespace SampleSupport
    <AttributeUsage(AttributeTargets.Method, AllowMultiple:=False)> _
    Public NotInheritable Class CategoryAttribute
        Inherits Attribute
        ' Methods
        Public Sub New(ByVal category As String)
            _category = category
        End Sub


        ' Properties
        Public ReadOnly Property Category As String
            Get
                Return _category
            End Get
        End Property


        ' Fields
        Private ReadOnly _category As String
    End Class
End Namespace

