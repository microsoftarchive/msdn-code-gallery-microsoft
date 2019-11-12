' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System
Imports Windows.Graphics.Printing
Imports Windows.UI.Xaml.Printing

Public Class BasePrintPage
    Inherits Page

#Region "Application Content Size Constants given in percents ( normalized )"

    ''' <summary>
    ''' The percent of app's margin width, content is set at 85% (0.85) of the area's width
    ''' </summary>
    Protected Const ApplicationContentMarginLeft As Double = 0.075

    ''' <summary>
    ''' The percent of app's margin height, content is set at 94% (0.94) of tha area's height
    ''' </summary>
    Protected Const ApplicationContentMarginTop As Double = 0.03

#End Region

    ''' <summary>
    ''' A pointer back to the main page which is used to gain access to the input and output frames and their content. 
    ''' </summary>
    Protected rootPage As MainPage = Nothing

    ''' <summary>
    ''' PrintDocument is used to prepare the pages for printing. 
    ''' Prepare the pages to print in the handlers for the Paginate, GetPreviewPage, and AddPages events.
    ''' </summary>
    Protected printDocument As PrintDocument = Nothing

    ''' <summary>
    ''' Marker interface for document source
    ''' </summary>
    Protected printDocumentSource As IPrintDocumentSource = Nothing

    ''' <summary>
    ''' A list of UIElements used to store the print preview pages.  This gives easy access
    ''' to any desired preview page.
    ''' </summary>
    Friend printPreviewPages As List(Of UIElement) = Nothing

    ''' <summary>
    ''' First page in the printing-content series
    ''' From this "virtual sized" paged content is split(text is flowing) to "printing pages"
    ''' </summary>
    Protected firstPage As FrameworkElement

    ''' <summary>
    ''' Factory method for every scenario that will create/generate print content specific to each scenario
    ''' For scenarios 1-5: it will create the first page from which content will flow
    ''' Scenario 6 uses a different approach
    ''' </summary>
    Protected Overridable Sub PreparePrintContent()
    End Sub

    Public Sub New()
        printPreviewPages = New List(Of UIElement)()
    End Sub

    ''' <summary>
    '''  Printing root property on each input page.
    ''' </summary>
    Protected Overridable ReadOnly Property PrintingRoot() As Canvas
        Get
            Return TryCast(FindName("_printingRoot"), Canvas)
        End Get
    End Property

    ''' <summary>
    ''' This is the event handler for PrintManager.PrintTaskRequested.
    ''' </summary>
    ''' <param name="sender">PrintManager</param>
    ''' <param name="e">PrintTaskRequestedEventArgs </param>
    Protected Overridable Sub PrintTaskRequested(ByVal sender As PrintManager, ByVal e As PrintTaskRequestedEventArgs)
        Dim printTask As PrintTask = Nothing
        printTask = e.Request.CreatePrintTask("VB Printing SDK Sample", Sub(sourceRequested)
                                                                            ' Print Task event handler is invoked when the print job is completed.
                                                                            ' Notify the user when the print operation fails.
                                                                            AddHandler printTask.Completed, Async Sub(s, args)
                                                                                                                If args.Completion = PrintTaskCompletion.Failed Then
                                                                                                                    Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub() rootPage.NotifyUser("Failed to print.", NotifyType.ErrorMessage))
                                                                                                                End If
                                                                                                            End Sub
                                                                            sourceRequested.SetSource(printDocumentSource)
                                                                        End Sub)
    End Sub

    ''' <summary>
    ''' This function registers the app for printing with Windows and sets up the necessary event handlers for the print process.
    ''' </summary>
    Protected Overridable Sub RegisterForPrinting()
        ' Create the PrintDocument.
        printDocument = New PrintDocument()

        ' Save the DocumentSource.
        printDocumentSource = printDocument.DocumentSource

        ' Add an event handler which creates preview pages.
        AddHandler printDocument.Paginate, AddressOf CreatePrintPreviewPages

        ' Add an event handler which provides a specified preview page.
        AddHandler printDocument.GetPreviewPage, AddressOf GetPrintPreviewPage

        ' Add an event handler which provides all final print pages.
        AddHandler printDocument.AddPages, AddressOf AddPrintPages

        ' Create a PrintManager and add a handler for printing initialization.
        Dim printMan As PrintManager = PrintManager.GetForCurrentView()
        AddHandler printMan.PrintTaskRequested, AddressOf PrintTaskRequested

        ' Initialize print content for this scenario
        PreparePrintContent()
    End Sub

    ''' <summary>
    ''' This function unregisters the app for printing with Windows.
    ''' </summary>
    Protected Overridable Sub UnregisterForPrinting()
        If printDocument Is Nothing Then
            Return
        End If

        RemoveHandler printDocument.Paginate, AddressOf CreatePrintPreviewPages
        RemoveHandler printDocument.GetPreviewPage, AddressOf GetPrintPreviewPage
        RemoveHandler printDocument.AddPages, AddressOf AddPrintPages

        ' Remove the handler for printing initialization.
        Dim printMan As PrintManager = PrintManager.GetForCurrentView()
        RemoveHandler printMan.PrintTaskRequested, AddressOf PrintTaskRequested

        PrintingRoot.Children.Clear()
    End Sub

    Protected Event pagesCreated As EventHandler

    ''' <summary>
    ''' This is the event handler for PrintDocument.Paginate. It creates print preview pages for the app.
    ''' </summary>
    ''' <param name="sender">PrintDocument</param>
    ''' <param name="e">Paginate Event Arguments</param>
    Protected Overridable Sub CreatePrintPreviewPages(ByVal sender As Object, ByVal e As PaginateEventArgs)
        ' Clear the cache of preview pages 
        printPreviewPages.Clear()

        ' Clear the printing root of preview pages
        PrintingRoot.Children.Clear()

        ' This variable keeps track of the last RichTextBlockOverflow element that was added to a page which will be printed
        Dim lastRTBOOnPage As RichTextBlockOverflow

        ' Get the PrintTaskOptions
        Dim printingOptions As PrintTaskOptions = (CType(e.PrintTaskOptions, PrintTaskOptions))

        ' Get the page description to deterimine how big the page is
        Dim pageDescription As PrintPageDescription = printingOptions.GetPageDescription(0)

        ' We know there is at least one page to be printed. passing null as the first parameter to
        ' AddOnePrintPreviewPage tells the function to add the first page.
        lastRTBOOnPage = AddOnePrintPreviewPage(Nothing, pageDescription)

        ' We know there are more pages to be added as long as the last RichTextBoxOverflow added to a print preview
        ' page has extra content
        Do While lastRTBOOnPage.HasOverflowContent AndAlso lastRTBOOnPage.Visibility = Windows.UI.Xaml.Visibility.Visible
            lastRTBOOnPage = AddOnePrintPreviewPage(lastRTBOOnPage, pageDescription)
        Loop

        If pagesCreatedEvent IsNot Nothing Then
            pagesCreatedEvent.Invoke(printPreviewPages, Nothing)
        End If

        Dim printDoc As PrintDocument = CType(sender, PrintDocument)

        ' Report the number of preview pages created
        printDoc.SetPreviewPageCount(printPreviewPages.Count, PreviewPageCountType.Intermediate)
    End Sub

    ''' <summary>
    ''' This is the event handler for PrintDocument.GetPrintPreviewPage. It provides a specific print preview page,
    ''' in the form of an UIElement, to an instance of PrintDocument. PrintDocument subsequently converts the UIElement
    ''' into a page that the Windows print system can deal with.
    ''' </summary>
    ''' <param name="sender">PrintDocument</param>
    ''' <param name="e">Arguments containing the preview requested page</param>
    Protected Overridable Sub GetPrintPreviewPage(ByVal sender As Object, ByVal e As GetPreviewPageEventArgs)
        Dim printDoc As PrintDocument = CType(sender, PrintDocument)

        printDoc.SetPreviewPage(e.PageNumber, printPreviewPages(e.PageNumber - 1))
    End Sub

    ''' <summary>
    ''' This is the event handler for PrintDocument.AddPages. It provides all pages to be printed, in the form of
    ''' UIElements, to an instance of PrintDocument. PrintDocument subsequently converts the UIElements
    ''' into a pages that the Windows print system can deal with.
    ''' </summary>
    ''' <param name="sender">PrintDocument</param>
    ''' <param name="e">Add page event arguments containing a print task options reference</param>
    Protected Overridable Sub AddPrintPages(ByVal sender As Object, ByVal e As AddPagesEventArgs)
        ' Loop over all of the preview pages and add each one to  add each page to be printied
        For i As Integer = 0 To printPreviewPages.Count - 1
            ' We should have all pages ready at this point...
            printDocument.AddPage(printPreviewPages(i))
        Next i

        Dim printDoc As PrintDocument = CType(sender, PrintDocument)

        ' Indicate that all of the print pages have been provided
        printDoc.AddPagesComplete()
    End Sub

    ''' <summary>
    ''' This function creates and adds one print preview page to the internal cache of print preview
    ''' pages stored in printPreviewPages.
    ''' </summary>
    ''' <param name="lastRTBOAdded">Last RichTextBlockOverflow element added in the current content</param>
    ''' <param name="printPageDescription">Printer's page description</param>
    Protected Overridable Function AddOnePrintPreviewPage(ByVal lastRTBOAdded As RichTextBlockOverflow, ByVal printPageDescription As PrintPageDescription) As RichTextBlockOverflow
        ' XAML element that is used to represent to "printing page"
        Dim page As FrameworkElement

        ' The link container for text overflowing in this page
        Dim textLink As RichTextBlockOverflow

        ' Check if this is the first page ( no previous RichTextBlockOverflow)
        If lastRTBOAdded Is Nothing Then
            ' If this is the first page add the specific scenario content
            page = firstPage
            'Hide footer since we don't know yet if it will be displayed (this might not be the last page) - wait for layout
            Dim footer As StackPanel = CType(page.FindName("footer"), StackPanel)
            footer.Visibility = Windows.UI.Xaml.Visibility.Collapsed
        Else
            ' Flow content (text) from previous pages
            page = New ContinuationPage(lastRTBOAdded)
        End If

        ' Set "paper" width
        page.Width = printPageDescription.PageSize.Width
        page.Height = printPageDescription.PageSize.Height

        Dim printableArea As Grid = CType(page.FindName("printableArea"), Grid)

        ' Get the margins size
        ' If the ImageableRect is smaller than the app provided margins use the ImageableRect
        Dim marginWidth As Double = Math.Max(printPageDescription.PageSize.Width - printPageDescription.ImageableRect.Width, printPageDescription.PageSize.Width * ApplicationContentMarginLeft * 2)
        Dim marginHeight As Double = Math.Max(printPageDescription.PageSize.Height - printPageDescription.ImageableRect.Height, printPageDescription.PageSize.Height * ApplicationContentMarginTop * 2)

        ' Set-up "printable area" on the "paper"
        printableArea.Width = firstPage.Width - marginWidth
        printableArea.Height = firstPage.Height - marginHeight

        ' Add the (newley created) page to the printing root which is part of the visual tree and force it to go
        ' through layout so that the linked containers correctly distribute the content inside them.            
        PrintingRoot.Children.Add(page)
        PrintingRoot.InvalidateMeasure()
        PrintingRoot.UpdateLayout()

        ' Find the last text container and see if the content is overflowing
        textLink = CType(page.FindName("continuationPageLinkedContainer"), RichTextBlockOverflow)

        ' Check if this is the last page
        If (Not textLink.HasOverflowContent) AndAlso textLink.Visibility = Windows.UI.Xaml.Visibility.Visible Then
            Dim footer As StackPanel = CType(page.FindName("footer"), StackPanel)
            footer.Visibility = Windows.UI.Xaml.Visibility.Visible
        End If

        ' Add the page to the page preview collection
        printPreviewPages.Add(page)

        Return textLink
    End Function

#Region "Navigation"
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, MainPage)

        ' init printing 
        RegisterForPrinting()
    End Sub

    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        UnregisterForPrinting()
    End Sub
#End Region
End Class

