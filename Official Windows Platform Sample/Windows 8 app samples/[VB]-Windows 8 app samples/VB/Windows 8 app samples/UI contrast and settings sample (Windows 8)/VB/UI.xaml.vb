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
Partial Public NotInheritable Class UI
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
        Dim UserSettings As New Windows.UI.ViewManagement.UISettings()
        Dim Color As Windows.UI.Color

        Buffer = String.Format("Hand Preference {0}" & vbLf, If(UserSettings.HandPreference = Windows.UI.ViewManagement.HandPreference.LeftHanded, "left", "right"))
        Buffer &= String.Format("Cursor Size {0} x {1}" & vbLf, UserSettings.CursorSize.Width, UserSettings.CursorSize.Height)
        Buffer &= String.Format("Scrollbar Size {0} x {1}" & vbLf, UserSettings.ScrollBarSize.Width, UserSettings.ScrollBarSize.Height)
        Buffer &= String.Format("Scrollbar Arrow Size {0} x {1}" & vbLf, UserSettings.ScrollBarArrowSize.Width, UserSettings.ScrollBarArrowSize.Height)
        Buffer &= String.Format("Scrollbar Thumb Box Size {0} x {1}" & vbLf, UserSettings.ScrollBarThumbBoxSize.Width, UserSettings.ScrollBarThumbBoxSize.Height)
        Buffer &= String.Format("Message Duration {0}" & vbLf, UserSettings.MessageDuration)
        Buffer &= String.Format("Animations Enabled {0}" & vbLf, If(UserSettings.AnimationsEnabled, "true", "false"))
        Buffer &= String.Format("Caret Browsing Enabled {0}" & vbLf, If(UserSettings.CaretBrowsingEnabled, "true", "false"))
        Buffer &= String.Format("Caret Blink Rate {0}" & vbLf, UserSettings.CaretBlinkRate)
        Buffer &= String.Format("Caret Width {0}" & vbLf, UserSettings.CaretWidth)
        Buffer &= String.Format("Double Click Time {0}" & vbLf, UserSettings.DoubleClickTime)
        Buffer &= String.Format("Mouse Hover Time {0}" & vbLf, UserSettings.MouseHoverTime)

        Buffer &= "System Colors: " & vbLf

        Color = UserSettings.UIElementColor(Windows.UI.ViewManagement.UIElementType.ActiveCaption)
        Buffer &= String.Format(vbTab & "Active Caption: {0}, {1}, {2}" & vbLf, Color.R, Color.G, Color.B)
        Color = UserSettings.UIElementColor(Windows.UI.ViewManagement.UIElementType.Background)
        Buffer &= String.Format(vbTab & "Background: {0}, {1}, {2}" & vbLf, Color.R, Color.G, Color.B)
        Color = UserSettings.UIElementColor(Windows.UI.ViewManagement.UIElementType.ButtonFace)
        Buffer &= String.Format(vbTab & "Button Face: {0}, {1}, {2}" & vbLf, Color.R, Color.G, Color.B)
        Color = UserSettings.UIElementColor(Windows.UI.ViewManagement.UIElementType.ButtonText)
        Buffer &= String.Format(vbTab & "Button Text: {0}, {1}, {2}" & vbLf, Color.R, Color.G, Color.B)
        Color = UserSettings.UIElementColor(Windows.UI.ViewManagement.UIElementType.CaptionText)
        Buffer &= String.Format(vbTab & "Caption Text: {0}, {1}, {2}" & vbLf, Color.R, Color.G, Color.B)
        Color = UserSettings.UIElementColor(Windows.UI.ViewManagement.UIElementType.GrayText)
        Buffer &= String.Format(vbTab & "Gray/Disabled Text: {0}, {1}, {2}" & vbLf, Color.R, Color.G, Color.B)
        Color = UserSettings.UIElementColor(Windows.UI.ViewManagement.UIElementType.Highlight)
        Buffer &= String.Format(vbTab & "Highlight: {0}, {1}, {2}" & vbLf, Color.R, Color.G, Color.B)
        Color = UserSettings.UIElementColor(Windows.UI.ViewManagement.UIElementType.HighlightText)
        Buffer &= String.Format(vbTab & "Highlighted Text: {0}, {1}, {2}" & vbLf, Color.R, Color.G, Color.B)
        Color = UserSettings.UIElementColor(Windows.UI.ViewManagement.UIElementType.Hotlight)
        Buffer &= String.Format(vbTab & "Hotlight: {0}, {1}, {2}" & vbLf, Color.R, Color.G, Color.B)
        Color = UserSettings.UIElementColor(Windows.UI.ViewManagement.UIElementType.InactiveCaption)
        Buffer &= String.Format(vbTab & "Inactive Caption: {0}, {1}, {2}" & vbLf, Color.R, Color.G, Color.B)
        Color = UserSettings.UIElementColor(Windows.UI.ViewManagement.UIElementType.InactiveCaptionText)
        Buffer &= String.Format(vbTab & "Inactive Caption Text: {0}, {1}, {2}" & vbLf, Color.R, Color.G, Color.B)
        Color = UserSettings.UIElementColor(Windows.UI.ViewManagement.UIElementType.Window)
        Buffer &= String.Format(vbTab & "Window: {0}, {1}, {2}" & vbLf, Color.R, Color.G, Color.B)
        Color = UserSettings.UIElementColor(Windows.UI.ViewManagement.UIElementType.WindowText)
        Buffer &= String.Format(vbTab & "Window Text: {0}, {1}, {2}" & vbLf, Color.R, Color.G, Color.B)

        UIOutputTextBlock.Text = Buffer
    End Sub
End Class
