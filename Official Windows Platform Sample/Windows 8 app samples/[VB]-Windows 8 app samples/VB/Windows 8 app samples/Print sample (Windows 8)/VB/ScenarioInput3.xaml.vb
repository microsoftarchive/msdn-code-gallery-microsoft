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
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports Windows.Graphics.Printing

''' <summary>
''' Scenario that demos how to add customizations in the displayed options list.
''' </summary>
Partial Public NotInheritable Class ScenarioInput3
    Inherits BasePrintPage

    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' This is the event handler for PrintManager.PrintTaskRequested.
    ''' In order to ensure a good user experience, the system requires that the app handle the PrintTaskRequested event within the time specified by PrintTaskRequestedEventArgs.Request.Deadline.
    ''' Therefore, we use this handler to only create the print task.
    ''' The print settings customization can be done when the print document source is requested.
    ''' </summary>
    ''' <param name="sender">PrintManager</param>
    ''' <param name="e">PrintTaskRequestedEventArgs</param>        
    Protected Overrides Sub PrintTaskRequested(sender As PrintManager, e As PrintTaskRequestedEventArgs)
        Dim printTask As PrintTask = Nothing
        printTask = e.Request.CreatePrintTask("VB Printing SDK Sample",
                                              Sub(sourceRequested)

                                                  Dim displayedOptions As IList(Of String) = printTask.Options.DisplayedOptions

                                                  ' Choose the printer options to be shown.
                                                  ' The order in which the options are appended determines the order in which they appear in the UI
                                                  displayedOptions.Clear()
                                                  displayedOptions.Add(Windows.Graphics.Printing.StandardPrintTaskOptions.Copies)
                                                  displayedOptions.Add(Windows.Graphics.Printing.StandardPrintTaskOptions.Orientation)
                                                  displayedOptions.Add(Windows.Graphics.Printing.StandardPrintTaskOptions.MediaSize)
                                                  displayedOptions.Add(Windows.Graphics.Printing.StandardPrintTaskOptions.Collation)
                                                  displayedOptions.Add(Windows.Graphics.Printing.StandardPrintTaskOptions.Duplex)

                                                  ' Preset the default value of the printer option
                                                  printTask.Options.MediaSize = PrintMediaSize.NorthAmericaLegal

                                                  ''' <summary>
                                                  ''' Option change event handler
                                                  ''' </summary>
                                                  ''' <param name="s">The print task option details for which an option changed.</param>
                                                  ''' <param name="args">The event arguments containing the id of the changed option.</param>
                                                  AddHandler printTask.Completed,
                                                      Async Sub(s, args)
                                                          '' Notify the user when the print operation fails.
                                                          If args.Completion = PrintTaskCompletion.Failed Then
                                                              Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                                                              Sub()
                                                                  rootPage.NotifyUser("Failed to print.", NotifyType.ErrorMessage)
                                                              End Sub)
                                                          End If
                                                      End Sub

                                                  sourceRequested.SetSource(printDocumentSource)
                                              End Sub)
    End Sub

    ''' <summary>
    ''' Provide print content for scenario 3 first page
    ''' </summary>
    Protected Overrides Sub PreparetPrintContent()
        If firstPage Is Nothing Then
            firstPage = New ScenarioOutput3()
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
