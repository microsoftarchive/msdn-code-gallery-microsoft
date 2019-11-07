Imports System

Namespace SampleSupport
    <AttributeUsage((AttributeTargets.Method Or AttributeTargets.Class), AllowMultiple:=False)> _
    Public NotInheritable Class PrefixAttribute
        Inherits Attribute
        ' Methods
        Public Sub New(ByVal prefix As String)
            _prefix = prefix

        End Sub


        ' Properties
        Public ReadOnly Property Prefix As String
            Get
                Return _prefix
            End Get
        End Property


        ' Fields
        Private ReadOnly _prefix As String
    End Class
End Namespace

