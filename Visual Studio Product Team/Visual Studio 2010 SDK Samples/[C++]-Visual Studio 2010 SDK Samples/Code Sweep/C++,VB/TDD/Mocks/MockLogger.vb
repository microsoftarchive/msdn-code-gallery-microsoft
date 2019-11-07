'**************************************************************************

'Copyright (c) Microsoft Corporation. All rights reserved.
'This code is licensed under the Visual Studio SDK license terms.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

'**************************************************************************


Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.Build.Framework

Namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
    Friend Class MockLogger
        Implements ILogger
        Public Event OnError As EventHandler(Of BuildErrorEventArgs)
        Public Event OnWarning As EventHandler(Of BuildWarningEventArgs)
        Public Event OnMessage As EventHandler(Of BuildMessageEventArgs)
        Public Event OnBuildStart As EventHandler(Of BuildStartedEventArgs)

        Private _eventSource As IEventSource

#Region "ILogger Members"

        Public Sub Initialize(ByVal eventSource As IEventSource) Implements ILogger.Initialize
            _eventSource = eventSource
            AddHandler eventSource.ErrorRaised, AddressOf eventSource_ErrorRaised
            AddHandler eventSource.WarningRaised, AddressOf eventSource_WarningRaised
            AddHandler eventSource.MessageRaised, AddressOf eventSource_MessageRaised
            AddHandler eventSource.BuildStarted, AddressOf eventSource_BuildStarted
        End Sub

        Private Sub eventSource_BuildStarted(ByVal sender As Object, ByVal e As BuildStartedEventArgs)
            If OnBuildStartEvent IsNot Nothing Then
                RaiseEvent OnBuildStart(Me, e)
            End If
        End Sub

        Private Sub eventSource_MessageRaised(ByVal sender As Object, ByVal e As BuildMessageEventArgs)
            If OnMessageEvent IsNot Nothing Then
                RaiseEvent OnMessage(Me, e)
            End If
        End Sub

        Private Sub eventSource_WarningRaised(ByVal sender As Object, ByVal e As BuildWarningEventArgs)
            If OnWarningEvent IsNot Nothing Then
                RaiseEvent OnWarning(Me, e)
            End If
        End Sub

        Private Sub eventSource_ErrorRaised(ByVal sender As Object, ByVal e As BuildErrorEventArgs)
            If OnErrorEvent IsNot Nothing Then
                RaiseEvent OnError(Me, e)
            End If
        End Sub

        Public Property Parameters() As String Implements ILogger.Parameters
            Get
                Return ""
            End Get
            Set(ByVal value As String)
            End Set
        End Property

        Public Sub Shutdown() Implements ILogger.Shutdown
        End Sub

        Public Property Verbosity() As LoggerVerbosity Implements ILogger.Verbosity
            Get
                Return LoggerVerbosity.Normal
            End Get
            Set(ByVal value As LoggerVerbosity)
            End Set
        End Property

#End Region
    End Class
End Namespace
