' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System

''' <summary>
''' Basic scenario for printing (how to register)
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
    Private Sub RegisterForPrintingButtonClick(ByVal sender As Object, ByVal e As RoutedEventArgs)
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
    Protected Overrides Sub PreparePrintContent()
        If firstPage Is Nothing Then
            firstPage = New ScenarioOutput1()
            Dim header As StackPanel = CType(firstPage.FindName("header"), StackPanel)
            header.Visibility = Windows.UI.Xaml.Visibility.Visible
        End If

        ' Add the (newley created) page to the printing root which is part of the visual tree and force it to go
        ' through layout so that the linked containers correctly distribute the content inside them.
        printingRoot.Children.Add(firstPage)
        printingRoot.InvalidateMeasure()
        printingRoot.UpdateLayout()
    End Sub

#Region "Navigation"
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, MainPage)
    End Sub
#End Region
End Class

