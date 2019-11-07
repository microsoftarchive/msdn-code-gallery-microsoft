' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

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
Imports Windows.UI.Xaml.Media.Imaging
Imports Windows.UI.Xaml.Navigation
Imports Windows.Graphics.Printing
Imports Windows.UI.Xaml.Printing
Imports Windows.UI.Xaml.Documents
Imports SDKTemplate

''' <summary>
''' Basic scenario for modern printing (how to register)
''' </summary>
Partial Public NotInheritable Class ScenarioInput1
    Inherits BasePrintPage

    ''' <summary>
    ''' A boolean which tracks whether the app has been registered for printing
    ''' </summary>
    Private isRegisteredForPrinting As Boolean = False

    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Register' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub RegisterForPrintingButtonClick(sender As Object, e As RoutedEventArgs)
        Dim clickedButton As Button = TryCast(sender, Button)

        ' Check to see if the application is registered for printing
        If isRegisteredForPrinting Then
            ' Unregister for printing 
            UnregisterForPrinting()

            ' Change the text on the button
            clickedButton.Content = "Register"

            ' Tell the user that the print contract has been unregistered
            rootPage.NotifyUser("Print contract unregistered.", NotifyType.StatusMessage)
        Else
            ' Register for printing               
            RegisterForPrinting()

            ' Change the text on the button
            clickedButton.Content = "Unregister"

            ' Tell the user that the print contract has been registered
            rootPage.NotifyUser("Print contract registered, use the Charms Bar to print.", NotifyType.StatusMessage)
        End If

        ' Toggle the value of isRegisteredForPrinting
        isRegisteredForPrinting = Not isRegisteredForPrinting
    End Sub

    ''' <summary>
    ''' Provide print content for scenario 1 first page
    ''' </summary>
    Protected Overrides Sub PreparetPrintContent()

        If firstPage Is Nothing Then
            firstPage = New ScenarioOutput1()
            Dim header As StackPanel = firstPage.FindName("header")
            header.Visibility = Windows.UI.Xaml.Visibility.Visible
        End If

        ' Add the (newley created) page to the printing root which is part of the visual tree and force it to go
        ' through layout so that the linked containers correctly distribute the content inside them.
        PrintingRootCanvas.Children.Add(firstPage)
        PrintingRootCanvas.InvalidateMeasure()
        PrintingRootCanvas.UpdateLayout()
    End Sub

#Region "Navigation"
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, MainPage)
    End Sub
#End Region

End Class