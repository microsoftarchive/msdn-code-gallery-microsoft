' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
'
Imports System
Imports System.Reflection

Namespace SampleSupport
    Public Class Sample
        ' Methods
        Public Sub New(ByVal harness As SampleHarness, ByVal method As MethodInfo, ByVal category As String, ByVal title As String, ByVal description As String, ByVal code As String)
            _Harness = harness
            _Method = method
            _Category = category
            _Title = title
            _Description = description
            _Code = code
        End Sub

        Public Sub Invoke()
            _Harness.InitSample()
            _Method.Invoke(_Harness, Nothing)
        End Sub

        Public Sub InvokeSafe()
            Try 
                Invoke()
            Catch exception As TargetInvocationException
                _Harness.HandleException(exception.InnerException)
            End Try
        End Sub

        Public Overrides Function ToString() As String
            Return Title
        End Function


        ' Properties
        Public ReadOnly Property Category As String
            Get
                Return _Category
            End Get
        End Property

        Public ReadOnly Property Code As String
            Get
                Return _Code
            End Get
        End Property

        Public ReadOnly Property Description As String
            Get
                Return _Description
            End Get
        End Property

        Public ReadOnly Property Harness As SampleHarness
            Get
                Return _Harness
            End Get
        End Property

        Public ReadOnly Property Method As MethodInfo
            Get
                Return _Method
            End Get
        End Property

        Public ReadOnly Property Title As String
            Get
                Return _Title
            End Get
        End Property


        ' Fields
        Private ReadOnly _Category As String
        Private ReadOnly _Code As String
        Private ReadOnly _Description As String
        Private ReadOnly _Harness As SampleHarness
        Private ReadOnly _Method As MethodInfo
        Private ReadOnly _Title As String
    End Class
End Namespace

