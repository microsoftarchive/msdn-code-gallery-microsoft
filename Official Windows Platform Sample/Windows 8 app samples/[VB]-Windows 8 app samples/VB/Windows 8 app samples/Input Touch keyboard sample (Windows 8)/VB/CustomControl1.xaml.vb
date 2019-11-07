' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports Windows.Foundation
Imports Windows.Foundation.Collections
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Controls.Primitives
Imports Windows.UI.Xaml.Input
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Navigation
Imports Windows.UI.Xaml.Automation.Peers

' needed for FrameworkElementAutomationPeer class
' The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

''' <summary>
''' Custom Text box control derived from Textbox class.
''' </summary>
Public Class CustomTextBox
    Inherits TextBox
    Public Sub New()
        Me.Background = New SolidColorBrush(Windows.UI.Colors.Coral)
        Me.BorderThickness = New Thickness(1)
        Me.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center
    End Sub
End Class

	''' <summary>
	''' A sample custom control that accepts user input and print out the virtual key code the user typed.  
	''' The control turns to yellow when it has input focus, otherwise it is gray.  Lost focus will also 
	''' clear the contents that is in the control.
	''' </summary>
Partial Public NotInheritable Class CustomInputBox1
    Inherits UserControl
    ''' <summary>
    ''' Loads the XAML UI contents and set properties requird for this custom control.
    ''' </summary>
    Public Sub New()
        Me.InitializeComponent()
        Me.IsTabStop = True
        Me.IsTapEnabled = True
    End Sub

    ''' <summary>
    ''' Create the Automation peer implementations for custom control (CustomInputBox1) to proivde the accessibility support.
    ''' </summary>
    ''' <returns>Automation Peer implementation for this control</returns>
    Protected Overrides Function OnCreateAutomationPeer() As AutomationPeer
        Return New CustomControl1AutomationPeer(Me)
    End Function

    ''' <summary>
    ''' Override the default event handler for GotFocus.
    ''' When the control got focus, indicate it has focus by highlighting the control by changing the background color to yellow.
    ''' </summary>
    ''' <param name="e">State information and event data associated with GotFocus event.</param>
    Protected Overrides Sub OnGotFocus(e As RoutedEventArgs)
        Me.myBorder.Background = New SolidColorBrush(Windows.UI.Colors.Yellow)
    End Sub

    ''' <summary>
    ''' Override the default event handler for LostFocus.
    ''' When the control lost focus, indicate it does not have focus by changing thethe background color to gray.
    ''' And the content is cleared.
    ''' </summary>
    ''' <param name="e">State information and event data associated with LostFocus event.</param>
    Protected Overrides Sub OnLostFocus(e As RoutedEventArgs)
        Me.myBorder.Background = New SolidColorBrush(Windows.UI.Colors.Gray)
        Me.myTextBlock.Text = ""
    End Sub

    ''' <summary>
    ''' Override the default event handler for Tapped.
    ''' Set input focus to the control when tapped on.
    ''' </summary>
    ''' <param name="e">State information and event data associated with Tapped event.</param>
    Protected Overrides Sub OnTapped(e As TappedRoutedEventArgs)
        Me.Focus(Windows.UI.Xaml.FocusState.Pointer)
    End Sub

    ''' <summary>
    ''' Override the default event handler for KeyDown.
    ''' Displays the text "A key is pressed" and the approximate time when the key is pressed.
    ''' </summary>
    ''' <param name="e">State information and event data associated with KeyDown event.</param>
    Protected Overrides Sub OnKeyDown(e As KeyRoutedEventArgs)
        Me.myTextBlock.Text = "A key was pressed @ " & DateTime.Now.ToString
    End Sub
End Class

	''' <summary>
	''' Automation Peer class for CustomInputBox1.  
	''' 
	''' Note: The difference between this and CustomControl2AutomationPeer is that this one does not implement 
	''' Text Pattern (ITextProvider) and Value Pattern (IValuePattern) interfaces.  So Touh keyboard does not show 
	''' automatically when user taps on the control with Touch or Pen.
	''' </summary>
Public Class CustomControl1AutomationPeer
    Inherits FrameworkElementAutomationPeer
    Private customInputBox As CustomInputBox1
    Private accClass As String = "CustomInputBoxClass1"

    Public Sub New(owner As CustomInputBox1)
        MyBase.New(owner)
        Me.customInputBox = owner
    End Sub

    ''' <summary>
    ''' Overrride GetClassNameCore and set the name of the class that defines the type associated with this control.
    ''' </summary>
    ''' <returns>The name of the control class</returns>
    Protected Overrides Function GetClassNameCore() As String
        Return Me.accClass
    End Function
End Class
