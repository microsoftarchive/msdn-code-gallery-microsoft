' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.ComponentModel
''' <summary>
''' Represents a MaskedTextBox pre-masked with US Phone Format
''' </summary>
''' <remarks></remarks>
Public Class PhoneBox
    Inherits System.Windows.Forms.MaskedTextBox
    Public Sub New()
        MyBase.New()
        MyBase.Mask = "(999)000-0000"
        Me.Name = "PhoneBox"
    End Sub

    <System.ComponentModel.DefaultValue("(999)000-0000")> _
    Public Shadows Property Mask() As String
        Get
            Return MyBase.Mask
        End Get
        Set(ByVal value As String)
            MyBase.Mask = value
        End Set
    End Property
End Class
