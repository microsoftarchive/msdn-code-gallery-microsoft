' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System
Imports Windows.Graphics.Printing

Namespace Global.PrintSample
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
        Protected Overrides Sub PrintTaskRequested(ByVal sender As PrintManager, ByVal e As PrintTaskRequestedEventArgs)
            Dim printTask As PrintTask = Nothing
            printTask = e.Request.CreatePrintTask("VB Printing SDK Sample", Sub(sourceRequestedArgs)
                                                                                ' Choose the printer options to be shown.
                                                                                ' The order in which the options are appended determines the order in which they appear in the UI
                                                                                ' Preset the default value of the printer option
                                                                                ' Print Task event handler is invoked when the print job is completed.
                                                                                ' Notify the user when the print operation fails.
                                                                                Dim displayedOptions As IList(Of String) = printTask.Options.DisplayedOptions
                                                                                displayedOptions.Clear()
                                                                                displayedOptions.Add(Windows.Graphics.Printing.StandardPrintTaskOptions.Copies)
                                                                                displayedOptions.Add(Windows.Graphics.Printing.StandardPrintTaskOptions.Orientation)
                                                                                displayedOptions.Add(Windows.Graphics.Printing.StandardPrintTaskOptions.MediaSize)
                                                                                displayedOptions.Add(Windows.Graphics.Printing.StandardPrintTaskOptions.Collation)
                                                                                displayedOptions.Add(Windows.Graphics.Printing.StandardPrintTaskOptions.Duplex)
                                                                                printTask.Options.MediaSize = PrintMediaSize.NorthAmericaLegal
                                                                                AddHandler printTask.Completed, Async Sub(s, args)
                                                                                                                    If args.Completion = PrintTaskCompletion.Failed Then
                                                                                                                        Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub() rootPage.NotifyUser("Failed to print.", NotifyType.ErrorMessage))
                                                                                                                    End If
                                                                                                                End Sub
                                                                                sourceRequestedArgs.SetSource(printDocumentSource)
                                                                            End Sub)
        End Sub

        ''' <summary>
        ''' Provide print content for scenario 3 first page
        ''' </summary>
        Protected Overrides Sub PreparePrintContent()
            If firstPage Is Nothing Then
                firstPage = New ScenarioOutput3()
                Dim header As StackPanel = CType(firstPage.FindName("header"), StackPanel)
                header.Visibility = Windows.UI.Xaml.Visibility.Visible
            End If

            ' Add the (newley created) page to the printing root which is part of the visual tree and force it to go
            ' through layout so that the linked containers correctly distribute the content inside them.
            PrintingRoot.Children.Add(firstPage)
            PrintingRoot.InvalidateMeasure()
            PrintingRoot.UpdateLayout()
        End Sub

#Region "Navigation"
        Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)

            MyBase.OnNavigatedTo(e)
            ' Tell the user how to print
            rootPage.NotifyUser("Print contract registered with customization, use the Charms Bar to print.", NotifyType.StatusMessage)
        End Sub
#End Region
    End Class
End Namespace
