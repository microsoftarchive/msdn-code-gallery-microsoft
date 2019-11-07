using Windows.Storage;
using Windows.Storage.BulkAccess;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;
using Windows.UI.Xaml.Navigation;

// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

namespace DataSourceAdapter
{
    /// <summary>
    /// A page that displays a collection of item previews.  In the Split Application this page
    /// is used to display and select one of the available groups.
    /// </summary>
    public sealed partial class PictureFilesView : SDKTemplate.Common.LayoutAwarePage
    {
        public PictureFilesView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var queryOptions = new QueryOptions();
            queryOptions.FolderDepth = FolderDepth.Deep;
            queryOptions.IndexerOption = IndexerOption.UseIndexerWhenAvailable;
            queryOptions.SortOrder.Clear();
            var sortEntry = new SortEntry();
            sortEntry.PropertyName = "System.FileName";
            sortEntry.AscendingOrder = true;
            queryOptions.SortOrder.Add(sortEntry);

            var fileQuery = KnownFolders.PicturesLibrary.CreateFileQueryWithOptions(queryOptions);
            const uint size = 190; // default size for PicturesView mode
            var fileInformationFactory = new FileInformationFactory(fileQuery, ThumbnailMode.PicturesView, size, ThumbnailOptions.UseCurrentScale, true);
            itemsViewSource.Source = fileInformationFactory.GetVirtualizedFilesVector();
        }
    }
}
