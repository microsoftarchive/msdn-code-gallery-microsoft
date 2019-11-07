' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
'
Imports System

Namespace SampleSupport
    <AttributeUsage(AttributeTargets.Method, AllowMultiple:=True)> _
    Public NotInheritable Class LinkedPropertyAttribute
        Inherits Attribute
        ' Methods
        Public Sub New(ByVal methodName As String)
            _methodName = methodName
        End Sub


        ' Properties
        Public ReadOnly Property MethodName As String
            Get
                Return _methodName
            End Get
        End Property


        ' Fields
        Private ReadOnly _methodName As String
    End Class
End Namespace

