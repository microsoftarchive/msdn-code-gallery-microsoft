' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System

''' <summary>
''' Scenario that demos how to call the Print UI on demand
''' </summary>
Partial Public NotInheritable Class ScenarioInput2
    Inherits BasePrintPage

    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Print' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub InvokePrintButtonClick(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Await Windows.Graphics.Printing.PrintManager.ShowPrintUIAsync()
    End Sub

    ''' <summary>
    ''' Provide print content for scenario 2 first page
    ''' </summary>
    Protected Overrides Sub PreparePrintContent()
        If firstPage Is Nothing Then
            firstPage = New ScenarioOutput2()
            Dim header As StackPanel = CType(firstPage.FindName("header"), StackPanel)
            header.Visibility = Windows.UI.Xaml.Visibility.Visible
        End If

        ' Add the (newley created) page to the printing root which is part of the visual tree and force it to go
        ' through layout so that the linked containers correctly distribute the content inside them.
        printingRoot.Children.Add(firstPage)
        printingRoot.InvalidateMeasure()
        printingRoot.UpdateLayout()
    End Sub

    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        MyBase.OnNavigatedTo(e)
        ' Tell the user how to print
        rootPage.NotifyUser("Print contract registered with customization, use the Charms Bar to print.", NotifyType.StatusMessage)
    End Sub
End Class

