' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved


Imports System.Collections
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks
Imports SDKTemplate
Imports Windows.Foundation
Imports Windows.Graphics.Display
Imports Windows.Graphics.Printing
Imports Windows.UI.Text
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Documents
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Media.Imaging
Imports Windows.UI.Xaml.Navigation
Imports Windows.UI.Xaml.Printing
Imports SDKTemplate.MainPage
Imports Windows.UI


Public Class BasePrintPage
    Inherits Page

#Region "Application Content Size Constants given in percents ( normalized )"
    ''' <summary>
    ''' The percent of app's margin width, content is set at 85% (0.85) of the area's width
    ''' </summary>
    Private Const ApplicationContentMarginLeft As Double = 0.075

    ''' <summary>
    ''' The percent of app's margin height, content is set at 94% (0.94) of tha area's height
    ''' </summary>
    Private Const ApplicationContentMarginTop As Double = 0.03
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
    Protected printPreviewPages As List(Of UIElement) = Nothing

    Public Sub New()
        printPreviewPages = New List(Of UIElement)
    End Sub

    ''' <summary>
    ''' First page in the printing-content series
    ''' From this "virtual sized" paged content is split(text is flowing) to "printing pages"
    ''' </summary>
    Protected firstPage As FrameworkElement

    ''' <summary>
    ''' Factory method for every scenario that will create/generate print content specific to each scenario
    ''' For scenarios 1-5: it will create the first page from which content will flow
    ''' </summary>
    Protected Overridable Sub PreparetPrintContent()
    End Sub

    ''' <summary>
    '''  Printing root property on each input page.
    ''' </summary>
    Protected Overridable ReadOnly Property PrintingRootCanvas As Canvas
        Get
            Return TryCast(FindName("printingRoot"), Canvas)
        End Get
    End Property

    ''' <summary>
    ''' This is the event handler for PrintManager.PrintTaskRequested.
    ''' In order to ensure a good user experience, the system requires that the app handle the PrintTaskRequested event within the time specified 
    ''' by PrintTaskRequestedEventArgs->Request->Deadline.
    ''' Therefore, we use this handler to only create the print task.
    ''' The print settings customization can be done when the print document source is requested.
    ''' </summary>
    ''' <param name="sender">The print manager for which a print task request was made.</param>
    ''' <param name="e">The print taks request associated arguments.</param>
    Protected Overridable Sub PrintTaskRequested(sender As PrintManager, e As PrintTaskRequestedEventArgs)
        Dim printTask As PrintTask = Nothing
        printTask = e.Request.CreatePrintTask("VB Printing SDK Sample", Sub(sourceRequested)

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
    ''' This function registers the app for printing with Windows and sets up the necessary event handlers for the print process.
    ''' </summary>
    Protected Overridable Sub RegisterForPrinting()
        ' Create the PrintDocument.
        printDocument = New PrintDocument

        ' Save the DocumentSource.
        printDocumentSource = printDocument.DocumentSource

        ' Add an event handler which creates preview pages.
        AddHandler printDocument.Paginate, AddressOf CreatePrintPreviewPages

        ' Add an event handler which provides a specified preview page.
        AddHandler printDocument.GetPreviewPage, AddressOf GetPrintPreviewPage

        ' Add an event handler which provides all final print pages.
        AddHandler printDocument.AddPages, AddressOf AddPrintPages

        ' Create a PrintManager and add a handler for printing initialization.
        Dim printMan As PrintManager = PrintManager.GetForCurrentView
        AddHandler printMan.PrintTaskRequested, AddressOf PrintTaskRequested

        ' Initialize print content for this scenario
        PreparetPrintContent()
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
        Dim printMan As PrintManager = PrintManager.GetForCurrentView
        RemoveHandler printMan.PrintTaskRequested, AddressOf PrintTaskRequested

        PrintingRootCanvas.Children.Clear()
    End Sub

    Protected Event pagesCreated As EventHandler

    ''' <summary>
    ''' This is the event handler for PrintDocument.Paginate. It creates print preview pages for the app.
    ''' </summary>
    ''' <param name="sender">PrintDocument</param>
    ''' <param name="e">Paginate Event Arguments</param>
    Protected Overridable Sub CreatePrintPreviewPages(sender As Object, e As PaginateEventArgs)
        ' Clear the cache of preview pages 
        printPreviewPages.Clear()

        ' Clear the printing root of preview pages
        PrintingRootCanvas.Children.Clear()

        ' This variable keeps track of the last RichTextBlockOverflow element that was added to a page which will be printed
        Dim lastRTBOOnPage As RichTextBlockOverflow

        ' Get the PrintTaskOptions
        Dim printingOptions As PrintTaskOptions = DirectCast(e.PrintTaskOptions, PrintTaskOptions)

        ' Get the page description to deterimine how big the page is
        Dim pageDescription As PrintPageDescription = printingOptions.GetPageDescription(0)

        ' We know there is at least one page to be printed. passing null as the first parameter to
        ' AddOnePrintPreviewPage tells the function to add the first page.
        lastRTBOOnPage = AddOnePrintPreviewPage(Nothing, pageDescription)

        ' For this app: we know there are more pages to be added as long as the last RichTextBlockOverflow 
        ' added to a print preview page has extra content and is visible
        While lastRTBOOnPage.HasOverflowContent And lastRTBOOnPage.Visibility = Xaml.Visibility.Visible
            lastRTBOOnPage = AddOnePrintPreviewPage(lastRTBOOnPage, pageDescription)
        End While

        RaiseEvent pagesCreated(printPreviewPages, Nothing)

        Dim printDoc As PrintDocument = sender

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
    Protected Overridable Sub GetPrintPreviewPage(sender As Object, e As GetPreviewPageEventArgs)
        Dim printDoc As PrintDocument = sender

        printDoc.SetPreviewPage(e.PageNumber, printPreviewPages(e.PageNumber - 1))
    End Sub

    ''' <summary>
    ''' This is the event handler for PrintDocument.AddPages. It provides all pages to be printed, in the form of
    ''' UIElements, to an instance of PrintDocument. PrintDocument subsequently converts the UIElements
    ''' into a pages that the Windows print system can deal with.
    ''' </summary>
    ''' <param name="sender">PrintDocument</param>
    ''' <param name="e">Add page event arguments containing a print task options reference</param>
    Protected Overridable Sub AddPrintPages(sender As Object, e As AddPagesEventArgs)
        Dim printDoc As PrintDocument = sender

        ' Loop over all of the preview pages and add each one to  add each page to be printied
        For i = 0 To printPreviewPages.Count - 1
            printDoc.AddPage(printPreviewPages(i))
        Next

        ' Indicate that all of the print pages have been provided
        printDoc.AddPagesComplete()
    End Sub

    ''' <summary>
    ''' This function creates and adds one print preview page to the internal cache of print preview
    ''' pages stored in m_printPreviewPages.
    ''' </summary>
    ''' <param name="lastRTBOAdded">Last RichTextBlockOverflow element added in the current content</param>
    ''' <param name="printPageDescription">Printer's page description</param>
    Protected Overridable Function AddOnePrintPreviewPage(lastRTBOAdded As RichTextBlockOverflow, printPageDescription As PrintPageDescription) As RichTextBlockOverflow

        ' XAML element that is used to represent to "printing page"
        Dim page As FrameworkElement = Nothing

        ' The link container for text overflowing in this page
        Dim textLink As RichTextBlockOverflow = Nothing

        ' Check if this is the first page ( no previous RichTextBlockOverflow)
        If lastRTBOAdded Is Nothing Then
            ' If this is the first page add the specific scenario content
            page = firstPage
            ' Hide footer since we don't know yet if it will be displayed (this might not be the last page) - wait for layout
            Dim footer As StackPanel = page.FindName("footer")
            footer.Visibility = Xaml.Visibility.Collapsed
        Else
            ' Flow content (text) from previous pages
            page = New ContinuationPage(lastRTBOAdded)
        End If

        ' Set "paper" width
        page.Width = printPageDescription.PageSize.Width
        page.Height = printPageDescription.PageSize.Height

        Dim printableArea As Grid = page.FindName("printableArea")

        ' Get the margins size
        ' If the ImageableRect is smaller than the app provided margins use the ImageableRect
        Dim marginWidth As Double = Math.Max(printPageDescription.PageSize.Width - printPageDescription.ImageableRect.Width, printPageDescription.PageSize.Width * ApplicationContentMarginLeft * 2)
        Dim marginHeight As Double = Math.Max(printPageDescription.PageSize.Height - printPageDescription.ImageableRect.Height, printPageDescription.PageSize.Height * ApplicationContentMarginTop * 2)

        ' Set-up "printable area" on the "paper"
        printableArea.Width = firstPage.Width - marginWidth
        printableArea.Height = firstPage.Height - marginHeight

        ' Add the (newley created) page to the printing root which is part of the visual tree and force it to go
        ' through layout so that the linked containers correctly distribute the content inside them.
        PrintingRootCanvas.Children.Add(page)
        PrintingRootCanvas.InvalidateMeasure()
        PrintingRootCanvas.UpdateLayout()

        ' Find the last text container and see if the content is overflowing
        textLink = page.FindName("continuationPageLinkedContainer")

        ' Check if this is the last page
        If Not textLink.HasOverflowContent And textLink.Visibility = Xaml.Visibility.Visible Then
            Dim footer As StackPanel = page.FindName("footer")
            footer.Visibility = Xaml.Visibility.Visible
        End If

        ' Add the page to the page preview collection
        printPreviewPages.Add(page)

        Return textLink

    End Function

#Region "Navigation"
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, MainPage)

        ' init printing 
        RegisterForPrinting()
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        UnregisterForPrinting()
    End Sub
#End Region

End Class
