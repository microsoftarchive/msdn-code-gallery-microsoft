'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports System
Imports System.Collections.Generic
Imports Windows.Networking.Connectivity

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class ProfileLocalUsageData
    Inherits Global.SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As Global.SDKTemplate.MainPage = Global.SDKTemplate.MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub


    ''' <summary>
    ''' This is the click handler for the 'ProfileLocalUsageDataButton' button.  You would replace this with your own handler
    ''' if you have a button or buttons on this page.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ProfileLocalUsageData_Click(sender As Object, e As RoutedEventArgs)
        '
        'Get Internet Connection Profile and display local data usage for the profile for the past 1 hour
        '
        Dim localDataUsage As String = String.Empty
        Dim CurrTime As DateTime = DateTime.Now
        Dim TimeDiff As New TimeSpan(1, 0, 0)

        Try
            Dim InternetConnectionProfile As ConnectionProfile = NetworkInformation.GetInternetConnectionProfile()

            If InternetConnectionProfile Is Nothing Then
                rootPage.NotifyUser("Not connected to Internet" & vbLf, NotifyType.StatusMessage)
            Else
                Dim LocalUsage = InternetConnectionProfile.GetLocalUsage(CurrTime.Subtract(TimeDiff), CurrTime)

                localDataUsage = "Local Data Usage:" & vbLf
                localDataUsage &= " Bytes Sent     : " & LocalUsage.BytesSent.ToString & VBLF
                localDataUsage &= " Bytes Received : " & LocalUsage.BytesReceived.ToString & VBLF
                rootPage.NotifyUser(localDataUsage.ToString, NotifyType.StatusMessage)
            End If
        Catch ex As Exception
            rootPage.NotifyUser("Unexpected exception occured: " & ex.ToString, NotifyType.ErrorMessage)
        End Try
    End Sub
End Class
