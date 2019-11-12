'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports System
Imports System.Diagnostics
Imports System.Threading
Imports Windows.ApplicationModel.Background
Imports Windows.Storage
Imports Windows.Networking.NetworkOperators

' The namespace for the background tasks.
' A helper class for providing the application configuration.
Public NotInheritable Class ConfigStore
    ' For the sake of simplicity of the sample, the following authentication parameters are hard coded:
    Public Shared ReadOnly Property AuthenticationHost() As String
        Get
            Return "login.contoso.com"
        End Get
    End Property

    Public Shared ReadOnly Property UserName() As String
        Get
            Return "MyUserName"
        End Get
    End Property

    Public Shared ReadOnly Property Password() As String
        Get
            Return "MyPassword"
        End Get
    End Property

    Public Shared ReadOnly Property ExtraParameters() As String
        Get
            Return ""
        End Get
    End Property

    Public Shared ReadOnly Property MarkAsManualConnect() As Boolean
        Get
            Return False
        End Get
    End Property

    ' This flag is set by the foreground app to toogle authentication to be done by the
    ' background task handler.
    Public Shared Property AuthenticateThroughBackgroundTask() As Boolean
        Get
            Dim value As Object = Nothing
            If ApplicationData.Current.LocalSettings.Values.TryGetValue("background", value) AndAlso TypeOf value Is Boolean Then
                Return CBool(value)
            End If
            ' default value
            Return True
        End Get

        Set(value As Boolean)
            ApplicationData.Current.LocalSettings.Values("background") = value
        End Set
    End Property

    ' This item is set by the background task handler to pass an authentication event
    ' token to the foreground app.
    Public Shared Property AuthenticationToken() As String
        Get
            Dim value As Object = Nothing
            If ApplicationData.Current.LocalSettings.Values.TryGetValue("token", value) AndAlso TypeOf value Is String Then
                Return TryCast(value, String)
            End If
            Return ""
        End Get

        Set(value As String)
            ApplicationData.Current.LocalSettings.Values("token") = value
        End Set
    End Property
End Class
