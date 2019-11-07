'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports System
Imports ListViewInteraction.Expression.Blend.SampleData.SampleDataSource
Imports Windows.Foundation

Partial Public NotInheritable Class Scenario2
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    ' sample data - See SampleData folder
    Private messageData As MessageData = Nothing

    Public Sub New()
        Me.InitializeComponent()

        ' initializing sample data
        messageData = New MessageData()
        ' setting the ListView source to the sample data 
        ItemListView.ItemsSource = messageData.Collection
        ' making sure the first item is the selected item
        ItemListView.SelectedIndex = 0
    End Sub

#Region "Data Visualization"
    ''' <summary>
    ''' We will visualize the data item in asynchronously in multiple phases for improved panning user experience 
    ''' of large lists.  In this sample scneario, we will visualize different parts of the data item
    ''' in the following order:
    ''' 
    '''     1) Title and placeholder for Image (visualized synchronously - Phase 0)
    '''     2) Subtilte (visualized asynchronously - Phase 1)
    '''     3) Image (visualized asynchronously - Phase 2)
    '''
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="args"></param>
    Private Sub ItemListView_ContainerContentChanging(ByVal sender As ListViewBase, ByVal args As ContainerContentChangingEventArgs)
        Dim iv As Scenario2ItemViewer = TryCast(args.ItemContainer.ContentTemplateRoot, Scenario2ItemViewer)

        If args.InRecycleQueue = True Then
            iv.ClearData()
        ElseIf args.Phase = 0 Then
            iv.ShowTitle(TryCast(args.Item, Item))

            ' Register for async callback to visualize Title asynchronously
            args.RegisterUpdateCallback(ContainerContentChangingDelegate)
        ElseIf args.Phase = 1 Then
            iv.ShowSubtitle()
            args.RegisterUpdateCallback(ContainerContentChangingDelegate)
        ElseIf args.Phase = 2 Then
            iv.ShowImage()
        End If

        ' For imporved performance, set Handled to true since app is visualizing the data item
        args.Handled = True
    End Sub

    ''' <summary>
    ''' Managing delegate creation to ensure we instantiate a single instance for 
    ''' optimal performance. 
    ''' </summary>
    Private ReadOnly Property ContainerContentChangingDelegate() As TypedEventHandler(Of ListViewBase, ContainerContentChangingEventArgs)
        Get
            If _delegate Is Nothing Then
                _delegate = New TypedEventHandler(Of ListViewBase, ContainerContentChangingEventArgs)(AddressOf ItemListView_ContainerContentChanging)
            End If
            Return _delegate
        End Get
    End Property
    Private _delegate As TypedEventHandler(Of ListViewBase, ContainerContentChangingEventArgs)

#End Region
End Class
