' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System
Imports Windows.ApplicationModel.Background

Partial Public NotInheritable Class ScenarioInput1
    Inherits Page

    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As MainPage = Nothing

    Public Sub New()
        InitializeComponent()
        AddHandler RequestLockScreenAccess.Click, AddressOf RequestLockScreenAccess_Click
        AddHandler RemoveLockScreenAccess.Click, AddressOf RemoveLockScreenAccess_Click
        AddHandler QueryLockScreenAccess.Click, AddressOf QueryLockScreenAccess_Click
    End Sub

    Private Async Sub RequestLockScreenAccess_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim status As BackgroundAccessStatus = BackgroundAccessStatus.Unspecified
        Try
            status = Await BackgroundExecutionManager.RequestAccessAsync()
        Catch e1 As UnauthorizedAccessException
            ' An access denied exception may be thrown if two requests are issued at the same time
            ' For this specific sample, that could be if the user double clicks "Request access"
        End Try

        Select Case status
            Case BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity
                rootPage.NotifyUser("This app is on the lock screen and has access to Always-On Real Time Connectivity.", NotifyType.StatusMessage)
            Case BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity
                rootPage.NotifyUser("This app is on the lock screen and has access to Active Real Time Connectivity.", NotifyType.StatusMessage)
            Case BackgroundAccessStatus.Denied
                rootPage.NotifyUser("This app is not on the lock screen.", NotifyType.StatusMessage)
            Case BackgroundAccessStatus.Unspecified
                rootPage.NotifyUser("The user has not yet taken any action. This is the default setting and the app is not on the lock screen.", NotifyType.StatusMessage)
            Case Else
        End Select
    End Sub

    Private Sub RemoveLockScreenAccess_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        BackgroundExecutionManager.RemoveAccess()
        rootPage.NotifyUser("This app has been removed from the lock screen.", NotifyType.StatusMessage)
    End Sub

    Private Sub QueryLockScreenAccess_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Select Case BackgroundExecutionManager.GetAccessStatus()
            Case BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity
                rootPage.NotifyUser("This app is on the lock screen and has access to Always-On Real Time Connectivity.", NotifyType.StatusMessage)
            Case BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity
                rootPage.NotifyUser("This app is on the lock screen and has access to Active Real Time Connectivity.", NotifyType.StatusMessage)
            Case BackgroundAccessStatus.Denied
                rootPage.NotifyUser("This app is not on the lock screen.", NotifyType.StatusMessage)
            Case BackgroundAccessStatus.Unspecified
                rootPage.NotifyUser("The user has not yet taken any action. This is the default setting and the app is not on the lock screen.", NotifyType.StatusMessage)
            Case Else
        End Select
    End Sub


#Region "Template-Related Code - Do not remove"
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, MainPage)
    End Sub

#End Region
End Class
