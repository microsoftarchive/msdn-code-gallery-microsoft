' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
'
Imports System

Namespace SampleSupport
    <AttributeUsage((AttributeTargets.Method Or AttributeTargets.Class), AllowMultiple:=False)> _
    Public NotInheritable Class TitleAttribute
        Inherits Attribute
        ' Methods
        Public Sub New(ByVal title As String)
            _title = title
        End Sub


        ' Properties
        Public ReadOnly Property Title As String
            Get
                Return _title
            End Get
        End Property


        ' Fields
        Private ReadOnly _title As String
    End Class
End Namespace

