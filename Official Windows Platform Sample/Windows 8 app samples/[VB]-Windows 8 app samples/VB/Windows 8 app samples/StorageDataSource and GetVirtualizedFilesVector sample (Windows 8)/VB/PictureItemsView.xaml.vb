Imports Windows.Storage
Imports Windows.Storage.BulkAccess
Imports Windows.Storage.FileProperties
Imports Windows.Storage.Search
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation

Partial Public NotInheritable Class PictureItemsView
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        Dim queryOptions = New QueryOptions()
        queryOptions.FolderDepth = FolderDepth.Shallow
        queryOptions.IndexerOption = IndexerOption.UseIndexerWhenAvailable
        queryOptions.SortOrder.Clear()
        Dim sortEntry = New SortEntry()
        sortEntry.PropertyName = "System.IsFolder"
        sortEntry.AscendingOrder = False
        queryOptions.SortOrder.Add(sortEntry)
        sortEntry = New SortEntry()
        sortEntry.PropertyName = "System.ItemName"
        sortEntry.AscendingOrder = True
        queryOptions.SortOrder.Add(sortEntry)


        Dim ItemQuery = KnownFolders.PicturesLibrary.CreateFileQueryWithOptions(queryOptions)
        Const size As UInteger = 190 'default size for PicturesView mode
        Dim FileInformationFactory = New FileInformationFactory(ItemQuery, ThumbnailMode.PicturesView, size, ThumbnailOptions.UseCurrentScale, True)
        itemsViewSource.Source = fileInformationFactory.GetVirtualizedFilesVector()
    End Sub
End Class

Public Class FileFolderInformationTemplateSelector
    Inherits DataTemplateSelector
    Public Property FileInformationTemplate() As DataTemplate
        Get
            Return m_FileInformationTemplate
        End Get
        Set(value As DataTemplate)
            m_FileInformationTemplate = value
        End Set
    End Property
    Private m_FileInformationTemplate As DataTemplate
    Public Property FolderInformationTemplate() As DataTemplate
        Get
            Return m_FolderInformationTemplate
        End Get
        Set(value As DataTemplate)
            m_FolderInformationTemplate = value
        End Set
    End Property
    Private m_FolderInformationTemplate As DataTemplate

    Protected Overrides Function SelectTemplateCore(item As Object, container As DependencyObject) As DataTemplate
        Dim folder = TryCast(item, FolderInformation)
        Return If((folder Is Nothing), FileInformationTemplate, FolderInformationTemplate)
    End Function
End Class
