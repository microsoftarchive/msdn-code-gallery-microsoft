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
Partial Public NotInheritable Class Mouse
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        Dim Buffer As String
        Dim MouseCapabilities As New Windows.Devices.Input.MouseCapabilities()

        Buffer = String.Format("There is {0} mouse present" & vbLf, If(MouseCapabilities.MousePresent <> 0, "a", "no"))
        Buffer &= String.Format("There is {0} vertical mouse wheel present" & vbLf, If(MouseCapabilities.VerticalWheelPresent <> 0, "a", "no"))
        Buffer &= String.Format("There is {0} horizontal mouse wheel present" & vbLf, If(MouseCapabilities.HorizontalWheelPresent <> 0, "a", "no"))
        Buffer &= String.Format("The user has {0}opted to swap the mouse buttons" & vbLf, If(MouseCapabilities.SwapButtons <> 0, "", "not "))
        Buffer &= String.Format("The mouse has {0} button(s)" & vbLf, MouseCapabilities.NumberOfButtons)

        MouseOutputTextBlock.Text = Buffer
    End Sub
End Class
