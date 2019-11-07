' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
'
Imports System

Namespace SampleSupport
    <AttributeUsage(AttributeTargets.Method, AllowMultiple:=True)> _
    Public NotInheritable Class LinkedClassAttribute
        Inherits Attribute
        ' Methods
        Public Sub New(ByVal className As String)
            _className = className
        End Sub


        ' Properties
        Public ReadOnly Property ClassName As String
            Get
                Return _className
            End Get
        End Property


        ' Fields
        Private ReadOnly _className As String
    End Class
End Namespace

