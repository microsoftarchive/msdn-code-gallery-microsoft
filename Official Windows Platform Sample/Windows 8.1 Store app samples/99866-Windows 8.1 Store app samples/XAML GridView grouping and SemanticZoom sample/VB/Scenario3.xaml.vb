'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
'
'*********************************************************

Imports System

Partial Public NotInheritable Class Scenario3
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    ' represents the sample data structure - see SampleData folder
    Private _storeData As Expression.Blend.SampleData.SampleDataSource.StoreData = Nothing

    Public Sub New()
        Me.InitializeComponent()

        ' creates a new instance of the sample data
        _storeData = New Expression.Blend.SampleData.SampleDataSource.StoreData()

        ' sets the list of categories to the groups from the sample data
        Dim dataLetter As List(Of Expression.Blend.SampleData.SampleDataSource.GroupInfoList(Of Object)) = _storeData.GetGroupsByLetter()
        ' sets the CollectionViewSource in the XAML page resources to the data groups
        cvs2.Source = dataLetter
        ' sets the items source for the zoomed out view to the group data as well
        TryCast(SemanticZoom.ZoomedOutView, ListViewBase).ItemsSource = cvs2.View.CollectionGroups
    End Sub

    ' changes the SemanticZoom view from zoomed in/out depending on current state
    Private Sub scenario3switchViewsClickHandler(ByVal sender As Object, ByVal e As RoutedEventArgs)
        SemanticZoom.ToggleActiveView()
    End Sub


#Region "Data Visualization"
    ''' <summary>
    ''' We will visualize the data item in asynchronously in multiple phases for improved panning user experience 
    ''' of large lists.  In this sample scneario, we will visualize different parts of the data item
    ''' in the following order:
    ''' 
    '''     1) Placeholders (visualized synchronously - Phase 0)
    '''     2) Tilte (visualized asynchronously - Phase 1)
    '''     3) Image (visualized asynchronously - Phase 2)
    '''
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="args"></param>
    Private Sub ItemsGridView_ContainerContentChanging(ByVal sender As ListViewBase, ByVal args As ContainerContentChangingEventArgs)
        Dim iv As ItemViewer = TryCast(args.ItemContainer.ContentTemplateRoot, ItemViewer)

        If args.InRecycleQueue = True Then
            iv.ClearData()
        ElseIf args.Phase = 0 Then
            iv.ShowPlaceholder(TryCast(args.Item, Expression.Blend.SampleData.SampleDataSource.Item))

            ' Register for async callback to visualize Title asynchronously
            args.RegisterUpdateCallback(ContainerContentChangingDelegate)
        ElseIf args.Phase = 1 Then
            iv.ShowTitle()
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
                _delegate = New TypedEventHandler(Of ListViewBase, ContainerContentChangingEventArgs)(AddressOf ItemsGridView_ContainerContentChanging)
            End If
            Return _delegate
        End Get
    End Property
    Private _delegate As TypedEventHandler(Of ListViewBase, ContainerContentChangingEventArgs)

#End Region 'Data Visualization

End Class
