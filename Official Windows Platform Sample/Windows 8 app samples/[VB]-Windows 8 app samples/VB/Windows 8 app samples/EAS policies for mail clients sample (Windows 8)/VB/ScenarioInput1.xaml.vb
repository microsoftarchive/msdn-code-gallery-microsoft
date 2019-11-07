' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System
Imports System.Linq
Imports System.Collections.Generic
Imports Windows.Foundation
Imports Windows.Foundation.Collections
Imports Windows.Graphics.Display
Imports Windows.UI.ViewManagement
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Controls.Primitives
Imports Windows.UI.Xaml.Data
Imports Windows.UI.Xaml.Input
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Navigation
Imports Windows.Security.ExchangeActiveSyncProvisioning
Imports SDKTemplate

Partial Public NotInheritable Class ScenarioInput1
    Inherits Page
    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As MainPage = Nothing

    Public Sub New()
        InitializeComponent()
    End Sub

#Region "Template-Related Code - Do not remove"
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, MainPage)

        ' We want to be notified with the OutputFrame is loaded so we can get to the content.
        AddHandler rootPage.OutputFrameLoaded, AddressOf rootPage_OutputFrameLoaded
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        RemoveHandler rootPage.OutputFrameLoaded, AddressOf rootPage_OutputFrameLoaded
    End Sub

#End Region

#Region "Use this code if you need access to elements in the output frame - otherwise delete"
    Private Sub rootPage_OutputFrameLoaded(sender As Object, e As Object)
        ' At this point, we know that the Output Frame has been loaded and we can go ahead
        ' and reference elements in the page contained in the Output Frame.

        ' Get a pointer to the content within the OutputFrame.
        Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)

        ' Go find the elements that we need for this scenario.
        ' ex: flipView1 = outputFrame.FindName("FlipView1") as FlipView;

    End Sub
#End Region

    Private Sub DebugPrint(Trace As String)
        Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)
        Dim Scenario1DebugArea As TextBox = TryCast(outputFrame.FindName("Scenario1DebugArea"), TextBox)
        Scenario1DebugArea.Text &= Trace & vbCr & vbLf
    End Sub



    Private Sub Launch_Click(sender As Object, e As RoutedEventArgs)


        Try
            Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)
            Dim CurrentDeviceInfor As EasClientDeviceInformation = New Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation()
            Dim DeviceID As TextBox = TryCast(outputFrame.FindName("DeviceID"), TextBox)
            DeviceID.Text = CurrentDeviceInfor.Id.ToString
            Dim OperatingSystem As TextBox = TryCast(outputFrame.FindName("OperatingSystem"), TextBox)
            OperatingSystem.Text = CurrentDeviceInfor.OperatingSystem
            Dim FriendlyName As TextBox = TryCast(outputFrame.FindName("FriendlyName"), TextBox)
            FriendlyName.Text = CurrentDeviceInfor.FriendlyName
            Dim SystemManufacturer As TextBox = TryCast(outputFrame.FindName("SystemManufacturer"), TextBox)
            SystemManufacturer.Text = CurrentDeviceInfor.SystemManufacturer
            Dim SystemProductName As TextBox = TryCast(outputFrame.FindName("SystemProductName"), TextBox)
            SystemProductName.Text = CurrentDeviceInfor.SystemProductName
            Dim SystemSku As TextBox = TryCast(outputFrame.FindName("SystemSku"), TextBox)
            SystemSku.Text = CurrentDeviceInfor.SystemSku
        Catch [Error] As Exception
            '
            ' Bad Parameter, Machine infor Unavailable errors are to be handled here.
            '
            DebugPrint([Error].ToString)
        End Try
    End Sub

    Private Sub Reset_Click(sender As Object, e As RoutedEventArgs)


        Try
            Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)
            Dim DeviceID As TextBox = TryCast(outputFrame.FindName("DeviceID"), TextBox)
            DeviceID.Text = ""
            Dim OperatingSystem As TextBox = TryCast(outputFrame.FindName("OperatingSystem"), TextBox)
            OperatingSystem.Text = ""
            Dim FriendlyName As TextBox = TryCast(outputFrame.FindName("FriendlyName"), TextBox)
            FriendlyName.Text = ""
            Dim SystemManufacturer As TextBox = TryCast(outputFrame.FindName("SystemManufacturer"), TextBox)
            SystemManufacturer.Text = ""
            Dim SystemProductName As TextBox = TryCast(outputFrame.FindName("SystemProductName"), TextBox)
            SystemProductName.Text = ""
            Dim SystemSku As TextBox = TryCast(outputFrame.FindName("SystemSku"), TextBox)
            SystemSku.Text = ""
        Catch [Error] As Exception
            '
            ' Bad Parameter, Machine infor Unavailable errors are to be handled here.
            '
            DebugPrint([Error].ToString)
        End Try
    End Sub
End Class

