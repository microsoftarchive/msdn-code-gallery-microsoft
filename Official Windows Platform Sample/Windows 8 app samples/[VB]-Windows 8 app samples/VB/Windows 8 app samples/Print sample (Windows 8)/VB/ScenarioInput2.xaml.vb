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
Imports Windows.UI
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
    Private Async Sub InvokePrintButtonClick(sender As Object, e As RoutedEventArgs)
        ' Don't act when in snapped mode
        If ApplicationView.Value <> ApplicationViewState.Snapped Then
            Await Windows.Graphics.Printing.PrintManager.ShowPrintUIAsync()
        End If
    End Sub

    ''' <summary>
    ''' Provide print content for scenario 2 first page
    ''' </summary>
    Protected Overrides Sub PreparetPrintContent()
        If firstPage Is Nothing Then
            firstPage = New ScenarioOutput2()
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
        MyBase.OnNavigatedTo(e)
        ' Tell the user how to print
        rootPage.NotifyUser("Print contract registered with customization, use the Charms Bar to print.", NotifyType.StatusMessage)
    End Sub
#End Region
End Class
