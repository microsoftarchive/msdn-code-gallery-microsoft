Imports Windows.Storage
Imports Windows.Storage.BulkAccess
Imports Windows.Storage.FileProperties
Imports Windows.Storage.Search
Imports Windows.UI.Xaml.Navigation

''' <summary>
''' A page that displays a collection of item previews.  In the Split Application this page
''' is used to display and select one of the available groups.
''' </summary>
Partial Public NotInheritable Class PictureFilesView
    Inherits SDKTemplate.Common.LayoutAwarePage
    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        Dim queryOptions = New QueryOptions()
        queryOptions.FolderDepth = FolderDepth.Deep
        queryOptions.IndexerOption = IndexerOption.UseIndexerWhenAvailable
        queryOptions.SortOrder.Clear()
        Dim sortEntry = New SortEntry()
        sortEntry.PropertyName = "System.FileName"
        sortEntry.AscendingOrder = True
        queryOptions.SortOrder.Add(sortEntry)

        Dim fileQuery = KnownFolders.PicturesLibrary.CreateFileQueryWithOptions(queryOptions)
        Const size As UInteger = 190 'default size for PicturesView mode
        Dim fileInformationFactory = New FileInformationFactory(fileQuery, ThumbnailMode.PicturesView, size, ThumbnailOptions.UseCurrentScale, True)
        itemsViewSource.Source = fileInformationFactory.GetVirtualizedFilesVector()


    End Sub
End Class
