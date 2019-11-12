' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Text.RegularExpressions
Imports SDKTemplate
Imports Windows.Foundation
Imports Windows.Foundation.Collections
Imports Windows.Graphics.Printing
Imports Windows.Graphics.Printing.OptionDetails
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Controls.Primitives
Imports Windows.UI.Xaml.Data
Imports Windows.UI.Xaml.Documents
Imports Windows.UI.Xaml.Input
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Media.Imaging
Imports Windows.UI.Xaml.Navigation
Imports Windows.UI.Xaml.Printing

Class InvalidPageException
    Inherits Exception

    Public Sub New(message As String)
        MyBase.New(message)
    End Sub
End Class

''' <summary>
''' Scenario that demos a page range implementation
''' </summary>
Partial Public NotInheritable Class ScenarioInput5
    Inherits BasePrintPage

    Private pageRangeEditVisible As Boolean = False

    Private pageList As List(Of Integer)

    ''' <summary>
    ''' Flag used to determine if content selection mode is on
    ''' </summary>
    Private selectionMode As Boolean

    ''' <summary>
    ''' This is the original number of pages before processing(filtering) in ScenarioInput5_pagesCreated
    ''' </summary>
    Private totalPages As Integer

    Public Sub New()
        InitializeComponent()

        pageList = New List(Of Integer)

        AddHandler pagesCreated, AddressOf ScenarioInput5_pagesCreated
    End Sub

    ''' <summary>
    ''' Removes pages that are not in the given range
    ''' </summary>
    ''' <param name="sender">The list of preview pages</param>
    ''' <param name="e"></param>
    ''' <note> Handling preview for page range
    ''' Developers have the control over how the preview should look when the user specifies a valid page range.
    ''' There are three common ways to handle this:
    ''' 1) Preview remains unaffected by the page range and all the pages are shown independent of the specified page range.
    ''' 2) Preview is changed and only the pages specified in the range are shown to the user.
    ''' 3) Preview is changed, showing all the pages and graying out the pages not in page range.
    ''' We chose option (2) for this sample, developers can choose their preview option.
    ''' </note>
    Private Sub ScenarioInput5_pagesCreated(sender As Object, e As EventArgs)

        totalPages = printPreviewPages.Count

        If pageRangeEditVisible Then
            '' ignore page range if there are any invalid pages regarding current context
            If Not pageList.Exists(Function(page) page > printPreviewPages.Count) Then
                Dim i As Integer = printPreviewPages.Count
                While i > 0 AndAlso pageList.Count > 0
                    If Me.pageList.Contains(i) = False Then
                        printPreviewPages.RemoveAt(i - 1)
                    End If
                    i -= 1
                End While
            End If
        End If
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
                                                  Dim printDetailedOptions As PrintTaskOptionDetails = PrintTaskOptionDetails.GetFromPrintTaskOptions(printTask.Options)

                                                  Dim displayedOptions As IList(Of String) = printDetailedOptions.DisplayedOptions

                                                  ' Choose the printer options to be shown.
                                                  ' The order in which the options are appended determines the order in which they appear in the UI
                                                  displayedOptions.Clear()
                                                  displayedOptions.Add(Windows.Graphics.Printing.StandardPrintTaskOptions.Copies)
                                                  displayedOptions.Add(Windows.Graphics.Printing.StandardPrintTaskOptions.Orientation)
                                                  displayedOptions.Add(Windows.Graphics.Printing.StandardPrintTaskOptions.ColorMode)

                                                  ' Create a new list option

                                                  Dim pageFormat As PrintCustomItemListOptionDetails = printDetailedOptions.CreateItemListOption("PageRange", "Page Range")
                                                  pageFormat.AddItem("PrintAll", "Print all")
                                                  pageFormat.AddItem("PrintSelection", "Print Selection")
                                                  pageFormat.AddItem("PrintRange", "Print Range")

                                                  ' Add the custom option to the option list
                                                  displayedOptions.Add("PageRange")

                                                  ' Create new edit option
                                                  Dim pageRangeEdit As PrintCustomTextOptionDetails = printDetailedOptions.CreateTextOption("PageRangeEdit", "Range")

                                                  ' Register the handler for the option change event
                                                  AddHandler printDetailedOptions.OptionChanged, AddressOf printDetailedOptions_OptionChanged

                                                  ''' <summary>
                                                  ''' Option change event handler
                                                  ''' </summary>
                                                  ''' <param name="s">The print task option details for which an option changed.</param>
                                                  ''' <param name="args">The event arguments containing the id of the changed option.</param>
                                                  AddHandler printTask.Completed,
                                                      Async Sub(s, args)

                                                          pageRangeEditVisible = False
                                                          selectionMode = False
                                                          pageList.Clear()

                                                          '' Notify the user when the print operation fails.
                                                          If args.Completion = PrintTaskCompletion.Failed Then
                                                              Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                                                              Sub()
                                                                  rootPage.NotifyUser("Failed to print.", NotifyType.ErrorMessage)
                                                              End Sub)
                                                          End If

                                                          Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                                                          Sub()
                                                              ' Restore first page to its default layout
                                                              ' Undo any changes made by a text selection
                                                              ShowContent(Nothing)
                                                          End Sub)
                                                      End Sub

                                                  sourceRequested.SetSource(printDocumentSource)
                                              End Sub)
    End Sub

    ''' <summary>
    ''' Provide print content for scenario 5 first page
    ''' </summary>
    Protected Overrides Sub PreparetPrintContent()
        If firstPage Is Nothing Then
            firstPage = New ScenarioOutput5()
            Dim header As StackPanel = firstPage.FindName("header")
            header.Visibility = Windows.UI.Xaml.Visibility.Visible
        End If

        ' Add the (newley created) page to the printing root which is part of the visual tree and force it to go
        ' through layout so that the linked containers correctly distribute the content inside them.
        PrintingRootCanvas.Children.Add(firstPage)
        PrintingRootCanvas.InvalidateMeasure()
        PrintingRootCanvas.UpdateLayout()
    End Sub

    ''' <summary>
    ''' Option change event handler
    ''' </summary>
    ''' <param name="sender">PrintTaskOptionsDetails object</param>
    ''' <param name="args">the event arguments containing the changed option id</param>
    Private Sub printDetailedOptions_OptionChanged(sender As PrintTaskOptionDetails, args As PrintTaskOptionChangedEventArgs)

        If args.OptionId Is Nothing Then
            Exit Sub
        End If

        Dim optionId As String = args.OptionId.ToString

        ' Handle change in Page Range Option

        If optionId = "PageRange" Then
            Dim pageRange As IPrintOptionDetails = sender.Options(optionId)
            Dim pageRangeValue As String = pageRange.Value.ToString

            selectionMode = False

            Select Case pageRangeValue
                Case "PrintRange"
                    ' Add PageRangeEdit custom option to the option list
                    sender.DisplayedOptions.Add("PageRangeEdit")
                    pageRangeEditVisible = True
                    Dim ignored = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                                                      Sub()
                                                          ' Restore original page layout
                                                          ShowContent(Nothing)
                                                      End Sub)
                    Exit Select
                Case "PrintSelection"
                    Dim ignored = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                                                      Sub()
                                                          Dim outputContent As ScenarioOutput5 = DirectCast(rootPage.OutputFrame.Content, ScenarioOutput5)
                                                          ShowContent(outputContent.SelectedText)
                                                      End Sub)
                    RemovePageRangeEdit(sender)
                    Exit Select
                Case Else
                    Dim ignored = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                                                      Sub()
                                                          ' Restore original page layout
                                                          ShowContent(Nothing)
                                                      End Sub)
                    RemovePageRangeEdit(sender)
                    Exit Select
            End Select

            Refresh()
        ElseIf optionId = "PageRangeEdit" Then
            Dim pageRange As IPrintOptionDetails = sender.Options(optionId)
            ' Expected range format (p1,p2...)*, (p3-p9)* ...
            If Not Regex.IsMatch(pageRange.Value.ToString, "^\s*\d+\s*(\-\s*\d+\s*)?(\,\s*\d+\s*(\-\s*\d+\s*)?)*$") Then
                pageRange.ErrorText = "Invalid Page Range (eg: 1-3, 5)"
            Else
                pageRange.ErrorText = String.Empty
                Try
                    GetPagesInRange(pageRange.Value.ToString)
                    Refresh()
                Catch ipex As InvalidPageException
                    pageRange.ErrorText = ipex.Message
                End Try
            End If
        End If
    End Sub

    Private Sub ShowContent(selectionText As String)

        Dim hasSelection As Boolean = Not String.IsNullOrEmpty(selectionText)
        selectionMode = hasSelection

        ' Hide/show images depending by the selected text
        Dim header As StackPanel = firstPage.FindName("header")
        header.Visibility = If(hasSelection, Windows.UI.Xaml.Visibility.Collapsed, Windows.UI.Xaml.Visibility.Visible)
        Dim pageContent As Grid = firstPage.FindName("printableArea")
        pageContent.RowDefinitions(0).Height = GridLength.Auto

        Dim scenarioImage As Image = firstPage.FindName("scenarioImage")
        scenarioImage.Visibility = If(hasSelection, Windows.UI.Xaml.Visibility.Collapsed, Windows.UI.Xaml.Visibility.Visible)

        ' Expand the middle paragraph on the full page if printing only selected text
        Dim firstLink As RichTextBlockOverflow = firstPage.FindName("firstLinkedContainer")
        firstLink.SetValue(Grid.ColumnSpanProperty, If(hasSelection, 2, 1))

        ' Clear(hide) current text and add only the selection if a selection exists
        Dim mainText As RichTextBlock = firstPage.FindName("textContent")

        Dim textSelection As RichTextBlock = firstPage.FindName("textSelection")

        ' Main (default) scenario text
        mainText.Visibility = If(hasSelection, Windows.UI.Xaml.Visibility.Collapsed, Windows.UI.Xaml.Visibility.Visible)
        mainText.OverflowContentTarget = If(hasSelection, Nothing, firstLink)

        ' Scenario text-blocks used for displaying selection
        textSelection.OverflowContentTarget = If(hasSelection, firstLink, Nothing)
        textSelection.Visibility = If(hasSelection, Windows.UI.Xaml.Visibility.Visible, Windows.UI.Xaml.Visibility.Collapsed)
        textSelection.Blocks.Clear()

        ' Force the visual root to go through layout so that the linked containers correctly distribute the content inside them.
        PrintingRootCanvas.InvalidateArrange()
        PrintingRootCanvas.InvalidateMeasure()
        PrintingRootCanvas.UpdateLayout()

        ' Add the text selection if any
        If hasSelection Then
            Dim inlineText As Run = New Run()
            inlineText.Text = selectionText

            Dim paragraph As Paragraph = New Paragraph()
            paragraph.Inlines.Add(inlineText)

            textSelection.Blocks.Add(paragraph)
        End If
    End Sub

    Protected Overrides Function AddOnePrintPreviewPage(lastRTBOAdded As RichTextBlockOverflow, printPageDescription As PrintPageDescription) As RichTextBlockOverflow
        Dim textLink As RichTextBlockOverflow = MyBase.AddOnePrintPreviewPage(lastRTBOAdded, printPageDescription)

        ' Don't show footer in selection mode
        If selectionMode Then
            Dim page As FrameworkElement = printPreviewPages(printPreviewPages.Count - 1)
            Dim footer As StackPanel = page.FindName("footer")
            footer.Visibility = Windows.UI.Xaml.Visibility.Collapsed
        End If

        Return textLink
    End Function

    ''' <summary>
    ''' Removes the PageRange edit from the charm window
    ''' </summary>
    ''' <param name="printTaskOptionDetails">Details regarding PrintTaskOptions</param>
    Private Sub RemovePageRangeEdit(printTaskOptionDetails As PrintTaskOptionDetails)
        If pageRangeEditVisible Then
            Dim lastDisplayedOption As String = printTaskOptionDetails.DisplayedOptions.FirstOrDefault(Function(p) p.Contains("PageRangeEdit"))
            If Not String.IsNullOrEmpty(lastDisplayedOption) Then
                printTaskOptionDetails.DisplayedOptions.Remove(lastDisplayedOption)
            End If
            pageRangeEditVisible = False
            pageList.Clear()
        End If
    End Sub

    Private Sub Refresh()
        ' Refresh
        Dim ignored = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                                          Sub()
                                              ' Refresh preview
                                              printDocument.InvalidatePreview()
                                          End Sub)
    End Sub

    ReadOnly enumerationSeparator As Char() = New Char() {","c}
    ReadOnly rangeSeparator As Char() = New Char() {"-"c}

    ''' <summary>
    ''' This is where we parse the range field
    ''' </summary>
    ''' <param name="pageRange">the page range value</param>
    Private Sub GetPagesInRange(pageRange As String)
        Dim rangeSplit As String() = pageRange.Split(enumerationSeparator)

        ' Clear the previous values
        pageList.Clear()

        For Each range As String In rangeSplit
            ' Interval
            If range.Contains("-") Then
                Dim limits As String() = range.Split(rangeSeparator)
                Dim startlimit As Integer = Integer.Parse(limits(0))
                Dim endLimit As Integer = Integer.Parse(limits(1))

                If ((startlimit < 1) Or (endLimit > totalPages) Or (startlimit >= endLimit)) Then
                    Throw New InvalidPageException(String.Format("Invalid page(s) in range {0} - {1}", startlimit, endLimit))
                End If

                For i = startlimit To endLimit
                    pageList.Add(i)
                Next
                Continue For
            End If

            ' Single page

            Dim pageNr = Integer.Parse(range.Trim())

            If pageNr < 1 Then
                Throw New InvalidPageException(String.Format("Invalid page {0}", pageNr))
            End If

            If pageNr > totalPages Then
                Throw New InvalidPageException(String.Format("Invalid page {0}", pageNr))
            End If

            pageList.Add(pageNr)
        Next
    End Sub

#Region "Navigation"
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        MyBase.OnNavigatedTo(e)

        ' Tell the user how to print
        rootPage.NotifyUser("Print contract registered with customization, use the Charms Bar to print.", NotifyType.StatusMessage)
    End Sub
#End Region

End Class
