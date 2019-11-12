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
Imports System.Collections.Generic
Imports Windows.ApplicationModel.Activation
Imports Windows.Foundation
Imports Windows.Storage
Imports Windows.Storage.Pickers.Provider
Imports Windows.UI.Core
Imports Windows.UI.ViewManagement
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls

Namespace Global.SDKTemplate
    Public Enum NotifyType
        StatusMessage
        ErrorMessage
    End Enum

    Public Class Scenario
        Public Property Title() As String
            Get
                Return m_Title
            End Get
            Set(value As String)
                m_Title = value
            End Set
        End Property
        Private m_Title As String

        Public Property ClassType() As Type
            Get
                Return m_ClassType
            End Get
            Set(value As Type)
                m_ClassType = value
            End Set
        End Property
        Private m_ClassType As Type

        Public Overrides Function ToString() As String
            Return Title
        End Function
    End Class

    Friend Class Status
        Friend Shared ReadOnly FileAdded As String = "File added to the basket."
        Friend Shared ReadOnly FileRemoved As String = "File removed from the basket."
        Friend Shared ReadOnly FileAddFailed As String = "Couldn't add file to the basket."
    End Class
End Namespace
