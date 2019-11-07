' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
'
Imports System

Namespace SampleSupport
    <AttributeUsage(AttributeTargets.Method, AllowMultiple:=False)> _
    Public NotInheritable Class DescriptionAttribute
        Inherits Attribute
        ' Methods
        Public Sub New(ByVal description As String)
            _description = description
        End Sub


        ' Properties
        Public ReadOnly Property Description As String
            Get
                Return _description
            End Get
        End Property


        ' Fields
        Private ReadOnly _description As String
    End Class
End Namespace

