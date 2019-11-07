' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports SDKTemplate
Imports Windows.Foundation
Imports Windows.Foundation.Collections
Imports Windows.Graphics.Printing
Imports Windows.Graphics.Printing.OptionDetails
Imports Windows.UI.Text
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

<FlagsAttribute()>
Friend Enum DisplayContent As Integer
    ''' <summary>
    ''' Show only text
    ''' </summary>
    Text = 1

    ''' <summary>
    ''' Show only images
    ''' </summary>
    Images = 2

    ''' <summary>
    ''' Show a combination of images and text
    ''' </summary>
    TextAndImages = 3
End Enum

''' <summary>
''' Scenario that demos how to add custom options (specific for the application)
''' </summary>
Partial Public NotInheritable Class ScenarioInput4
    Inherits BasePrintPage

    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' A flag that determines if text & images are to be shown
    ''' </summary>
    Friend imageText As DisplayContent = DisplayContent.TextAndImages

    ''' <summary>
    ''' Helper getter for text showing
    ''' </summary>
    Private ReadOnly Property ShowText As Boolean
        Get
            Return (imageText And DisplayContent.Text) = DisplayContent.Text
        End Get
    End Property

    ''' <summary>
    ''' Helper getter for image showing
    ''' </summary>
    Private ReadOnly Property ShowImage As Boolean
        Get
            Return (imageText And DisplayContent.Images) = DisplayContent.Images
        End Get
    End Property

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

                                                  Dim pageFormat As PrintCustomItemListOptionDetails = printDetailedOptions.CreateItemListOption("PageContent", "Pictures")
                                                  pageFormat.AddItem("PicturesText", "Pictures and text")
                                                  pageFormat.AddItem("PicturesOnly", "Pictures only")
                                                  pageFormat.AddItem("TextOnly", "Text only")

                                                  ' Add the custom option to the option list
                                                  displayedOptions.Add("PageContent")

                                                  AddHandler printDetailedOptions.OptionChanged, AddressOf printDetailedOptions_OptionChanged

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

    Private Sub printDetailedOptions_OptionChanged(sender As PrintTaskOptionDetails, args As PrintTaskOptionChangedEventArgs)
        ' Listen for PageContent changes
        Dim optionId As String = TryCast(args.OptionId, String)
        If String.IsNullOrEmpty(optionId) Then
            Exit Sub
        End If

        If optionId = "PageContent" Then
            Dim ignored = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                                 printDocument.InvalidatePreview()
                                                                                             End Sub)
        End If
    End Sub

    ''' <summary>
    ''' Provide print content for scenario 4 first page
    ''' </summary>
    Protected Overrides Sub PreparetPrintContent()
        If firstPage Is Nothing Then
            firstPage = New ScenarioOutput4()
            Dim header As StackPanel = firstPage.FindName("header")
            header.Visibility = Windows.UI.Xaml.Visibility.Visible
        End If

        ' Add the (newley created) page to the printing root which is part of the visual tree and force it to go
        ' through layout so that the linked containers correctly distribute the content inside them.
        PrintingRootCanvas.Children.Add(firstPage)
        PrintingRootCanvas.InvalidateMeasure()
        PrintingRootCanvas.UpdateLayout()
    End Sub

#Region "Preview"
    Protected Overrides Sub CreatePrintPreviewPages(sender As Object, e As PaginateEventArgs)
        ' Get PageContent property
        Dim printDetailedOptions As PrintTaskOptionDetails = PrintTaskOptionDetails.GetFromPrintTaskOptions(e.PrintTaskOptions)
        Dim pageContent As String = TryCast(printDetailedOptions.Options("PageContent").Value, String).ToLowerInvariant

        ' Set the text & image display flag
        imageText = DirectCast((Convert.ToInt32(pageContent.Contains("pictures")) << 1) Or Convert.ToInt32(pageContent.Contains("text")), DisplayContent)

        MyBase.CreatePrintPreviewPages(sender, e)
    End Sub


    Protected Overrides Function AddOnePrintPreviewPage(lastRTBOAdded As RichTextBlockOverflow, printPageDescription As PrintPageDescription) As RichTextBlockOverflow

        ' Check if we need to hide/show text & images for this scenario
        ' Since all is rulled by the first page (page flow), here is where we must start
        If lastRTBOAdded Is Nothing Then
            ' Get a refference to page objects
            Dim pageContent As Grid = firstPage.FindName("printableArea")
            Dim scenarioImage As Image = firstPage.FindName("scenarioImage")
            Dim mainText As RichTextBlock = firstPage.FindName("textContent")
            Dim firstLink As RichTextBlockOverflow = firstPage.FindName("firstLinkedContainer")
            Dim continuationLink As RichTextBlockOverflow = firstPage.FindName("continuationPageLinkedContainer")

            ' Hide(collapse) and move elements in different grid cells depending by the viewable content(only text, only pictures)

            scenarioImage.Visibility = If(ShowImage, Windows.UI.Xaml.Visibility.Visible, Windows.UI.Xaml.Visibility.Collapsed)
            firstLink.SetValue(Grid.ColumnSpanProperty, If(ShowImage, 1, 2))

            scenarioImage.SetValue(Grid.RowProperty, If(ShowText, 2, 1))
            scenarioImage.SetValue(Grid.ColumnProperty, If(ShowText, 1, 0))

            pageContent.ColumnDefinitions(0).Width = New GridLength(If(ShowText, 6, 4), GridUnitType.Star)
            pageContent.ColumnDefinitions(1).Width = New GridLength(If(ShowText, 4, 6), GridUnitType.Star)

            ' Break the text flow if the app is not printing text in order not to spawn pages with blank content
            mainText.Visibility = If(ShowText, Windows.UI.Xaml.Visibility.Visible, Windows.UI.Xaml.Visibility.Collapsed)
            continuationLink.Visibility = If(ShowText, Windows.UI.Xaml.Visibility.Visible, Windows.UI.Xaml.Visibility.Collapsed)

            ' Hide header if printing only images
            Dim header As StackPanel = firstPage.FindName("header")
            header.Visibility = If(ShowText, Windows.UI.Xaml.Visibility.Visible, Windows.UI.Xaml.Visibility.Collapsed)
        End If

        ' Continue with the rest of the base printing layout processing (paper size, printable page size)
        Return MyBase.AddOnePrintPreviewPage(lastRTBOAdded, printPageDescription)
    End Function

#End Region

#Region "Navigation"
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        MyBase.OnNavigatedTo(e)

        ' Tell the user how to print
        rootPage.NotifyUser("Print contract registered with customization, use the Charms Bar to print.", NotifyType.StatusMessage)
    End Sub
#End Region

End Class
