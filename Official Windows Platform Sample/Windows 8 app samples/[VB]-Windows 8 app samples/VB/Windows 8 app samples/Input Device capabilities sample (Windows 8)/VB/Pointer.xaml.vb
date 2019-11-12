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

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Pointer
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Private Function PointerType(PointerDevice As Windows.Devices.Input.PointerDevice) As String
        If PointerDevice.PointerDeviceType = Windows.Devices.Input.PointerDeviceType.Mouse Then
            Return "mouse"
        ElseIf PointerDevice.PointerDeviceType = Windows.Devices.Input.PointerDeviceType.Pen Then
            Return "pen"
        Else
            Return "touch"
        End If
    End Function

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        Dim Buffer As String

        Buffer = "List of all pointer devices: " & vbLf & vbLf

        Dim PointerDeviceList = Windows.Devices.Input.PointerDevice.GetPointerDevices()
        Dim displayIndex As Integer = 1

        For Each PointerDevice As Windows.Devices.Input.PointerDevice In PointerDeviceList
            Buffer &= String.Format("Pointer device " & displayIndex & ":" & vbLf)
            Buffer &= String.Format("This pointer device type is " & PointerType(PointerDevice) & vbLf)
            Buffer &= String.Format("This pointer device is " & (If(PointerDevice.IsIntegrated, "not ", "")) & "external" & vbLf)
            Buffer &= String.Format("This pointer device has a maximum of " & PointerDevice.MaxContacts & " contacts" & vbLf)
            Buffer &= String.Format("The physical device rect is " & PointerDevice.PhysicalDeviceRect.X.ToString & ", " & PointerDevice.PhysicalDeviceRect.Y.ToString & ", " & PointerDevice.PhysicalDeviceRect.Width.ToString & ", " & PointerDevice.PhysicalDeviceRect.Height.ToString & vbLf)
            Buffer &= String.Format("The screen rect is " & PointerDevice.ScreenRect.X.ToString & ", " & PointerDevice.ScreenRect.Y.ToString & ", " & PointerDevice.ScreenRect.Width.ToString & ", " & PointerDevice.ScreenRect.Height.ToString & vbLf & vbLf)
        Next

        PointerOutputTextBlock.Text = Buffer
    End Sub
End Class
