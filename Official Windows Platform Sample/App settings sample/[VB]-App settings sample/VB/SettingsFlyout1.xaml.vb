' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

' The SettingsFlyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

Partial Public NotInheritable Class SettingsFlyout1
    Inherits SettingsFlyout

    Public Sub New()
        Me.InitializeComponent()

        ' Handle all key events when loaded into visual tree
        AddHandler Me.Loaded, Sub(sender, e) AddHandler Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated, AddressOf SettingsFlyout1_AcceleratorKeyActivated
        AddHandler Me.Unloaded, Sub(sender, e) RemoveHandler Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated, AddressOf SettingsFlyout1_AcceleratorKeyActivated
    End Sub

    ''' <summary>
    ''' Invoked on every keystroke, including system keys such as Alt key combinations, when
    ''' this page is active and occupies the entire window.  Used to detect keyboard back 
    ''' navigation via Alt+Left key combination.
    ''' </summary>
    ''' <param name="sender">Instance that triggered the event.</param>
    ''' <param name="args">Event data describing the conditions that led to the event.</param>
    Private Sub SettingsFlyout1_AcceleratorKeyActivated(ByVal sender As CoreDispatcher, ByVal args As AcceleratorKeyEventArgs)
        ' Only investigate further when Left is pressed
        If args.EventType = CoreAcceleratorKeyEventType.SystemKeyDown AndAlso args.VirtualKey = VirtualKey.Left Then
            Dim coreWindow = Window.Current.CoreWindow
            Dim downState = CoreVirtualKeyStates.Down

            ' Check for modifier keys
            ' The Menu VirtualKey signifies Alt
            Dim menuKey As Boolean = (coreWindow.GetKeyState(VirtualKey.Menu) And downState) = downState
            Dim controlKey As Boolean = (coreWindow.GetKeyState(VirtualKey.Control) And downState) = downState
            Dim shiftKey As Boolean = (coreWindow.GetKeyState(VirtualKey.Shift) And downState) = downState

            If menuKey AndAlso (Not controlKey) AndAlso (Not shiftKey) Then
                args.Handled = True
                Me.Hide()
            End If
        End If
    End Sub
End Class

