' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System.Threading.Tasks
Imports Windows.ApplicationModel.Background
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate

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

    Private Sub RequestLockScreenAccess_Click(sender As Object, e As RoutedEventArgs)
        RequestLockScreenAccessAsync()
    End Sub

    Private Async Sub RequestLockScreenAccessAsync()
        Dim status As BackgroundAccessStatus = Await BackgroundExecutionManager.RequestAccessAsync
        Select Case status
            Case BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity
                rootPage.NotifyUser("This app is on the lock screen and has access to Always-On Real Time Connectivity.", NotifyType.StatusMessage)
                Exit Select
            Case BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity
                rootPage.NotifyUser("This app is on the lock screen and has access to Active Real Time Connectivity.", NotifyType.StatusMessage)
                Exit Select
            Case BackgroundAccessStatus.Denied
                rootPage.NotifyUser("This app is not on the lock screen.", NotifyType.StatusMessage)
                Exit Select
            Case BackgroundAccessStatus.Unspecified
                rootPage.NotifyUser("The user has not yet taken any action. This is the default setting and the app is not on the lock screen.", NotifyType.StatusMessage)
                Exit Select
            Case Else
                Exit Select
        End Select
    End Sub

    Private Sub RemoveLockScreenAccess_Click(sender As Object, e As RoutedEventArgs)
        BackgroundExecutionManager.RemoveAccess()
        rootPage.NotifyUser("This app has been removed from the lock screen.", NotifyType.StatusMessage)
    End Sub

    Private Sub QueryLockScreenAccess_Click(sender As Object, e As RoutedEventArgs)
        Select Case BackgroundExecutionManager.GetAccessStatus
            Case BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity
                rootPage.NotifyUser("This app is on the lock screen and has access to Always-On Real Time Connectivity.", NotifyType.StatusMessage)
                Exit Select
            Case BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity
                rootPage.NotifyUser("This app is on the lock screen and has access to Active Real Time Connectivity.", NotifyType.StatusMessage)
                Exit Select
            Case BackgroundAccessStatus.Denied
                rootPage.NotifyUser("This app is not on the lock screen.", NotifyType.StatusMessage)
                Exit Select
            Case BackgroundAccessStatus.Unspecified
                rootPage.NotifyUser("The user has not yet taken any action. This is the default setting and the app is not on the lock screen.", NotifyType.StatusMessage)
                Exit Select
            Case Else
                Exit Select
        End Select
    End Sub

#Region "Template-Related Code - Do not remove"
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, MainPage)
    End Sub
#End Region

End Class
