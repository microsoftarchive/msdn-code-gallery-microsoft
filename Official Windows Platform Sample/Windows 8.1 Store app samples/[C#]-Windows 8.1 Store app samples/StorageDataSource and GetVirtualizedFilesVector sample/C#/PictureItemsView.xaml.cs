using Windows.Storage;
using Windows.Storage.BulkAccess;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

namespace DataSourceAdapter
{
    public sealed partial class PictureItemsView : SDKTemplate.Common.LayoutAwarePage
    {
        public PictureItemsView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var queryOptions = new QueryOptions();
            queryOptions.FolderDepth = FolderDepth.Shallow;
            queryOptions.IndexerOption = IndexerOption.UseIndexerWhenAvailable;
            queryOptions.SortOrder.Clear();
            var sortEntry = new SortEntry();
            sortEntry.PropertyName = "System.IsFolder";
            sortEntry.AscendingOrder = false;
            queryOptions.SortOrder.Add(sortEntry);
            sortEntry = new SortEntry();
            sortEntry.PropertyName = "System.ItemName";
            sortEntry.AscendingOrder = true;
            queryOptions.SortOrder.Add(sortEntry);

            var itemQuery = KnownFolders.PicturesLibrary.CreateItemQueryWithOptions(queryOptions);
            const uint size = 190; // default size for PicturesView mode
            var fileInformationFactory = new FileInformationFactory(itemQuery, ThumbnailMode.PicturesView, size, ThumbnailOptions.UseCurrentScale, true);
            itemsViewSource.Source = fileInformationFactory.GetVirtualizedItemsVector();
        }
    }

    public class FileFolderInformationTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FileInformationTemplate { get; set; }
        public DataTemplate FolderInformationTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var folder = item as FolderInformation;
            return (folder == null) ? FileInformationTemplate : FolderInformationTemplate;
        }
    }
}
