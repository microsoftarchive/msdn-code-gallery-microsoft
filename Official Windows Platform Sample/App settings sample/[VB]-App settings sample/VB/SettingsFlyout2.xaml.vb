' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

' The SettingsFlyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

Partial Public NotInheritable Class SettingsFlyout2
    Inherits SettingsFlyout

    Private isSecondContentLayer As Boolean = False

    Public Sub New()
        Me.InitializeComponent()

        ' Handle all key events when loaded into visual tree
        AddHandler Me.Loaded, Sub(sender, e) AddHandler Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated, AddressOf SettingsFlyout2_AcceleratorKeyActivated
        AddHandler Me.Unloaded, Sub(sender, e) RemoveHandler Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated, AddressOf SettingsFlyout2_AcceleratorKeyActivated
    End Sub

    ''' <summary>
    ''' This is the handler for the button Click event. Content of the second layer is dynamically 
    ''' generated and shown.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub button_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Attach BackClick handler to override default back button behavior
        AddHandler Me.BackClick, AddressOf SettingsFlyout2_BackClick

        ' Create second layer of content.
        Dim header As New TextBlock()
        header.Text = "Layer 2 Content Header"
        header.Style = CType(Application.Current.Resources("TitleTextBlockStyle"), Style)
        Dim tb As New TextBlock()
        tb.Text = "Layer 2 of content.  Click the back button to return to the previous content."
        tb.Style = CType(Application.Current.Resources("BodyTextBlockStyle"), Style)

        Dim sp As New StackPanel()
        sp.Children.Add(header)
        sp.Children.Add(tb)
        Me.Content = sp

        Me.isSecondContentLayer = True
    End Sub

    ''' <summary>
    ''' This is the handler for the SettingsFlyout2 BackClick event. Original content is restored 
    ''' and the event args are marked as handled.
    ''' This handler is only attached when the second content layer is visible.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub SettingsFlyout2_BackClick(ByVal sender As Object, ByVal e As BackClickEventArgs)
        ' Return to previous content and remove BackClick handler
        e.Handled = True
        Me.isSecondContentLayer = False
        Me.Content = Me.content1
        RemoveHandler Me.BackClick, AddressOf SettingsFlyout2_BackClick
    End Sub

    ''' <summary>
    ''' Invoked on every keystroke, including system keys such as Alt key combinations, when
    ''' this page is active and occupies the entire window.  Used to detect keyboard back 
    ''' navigation via Alt+Left key combination.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="args"></param>
    Private Sub SettingsFlyout2_AcceleratorKeyActivated(ByVal sender As CoreDispatcher, ByVal args As AcceleratorKeyEventArgs)
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

                ' If in second content layer, return to previous content
                ' Otherwise, dismiss the SettingsFlyout
                If Me.isSecondContentLayer Then
                    Me.isSecondContentLayer = False
                    Me.Content = Me.content1
                    RemoveHandler Me.BackClick, AddressOf SettingsFlyout2_BackClick
                Else
                    Me.Hide()
                End If
            End If
        End If
    End Sub
End Class

