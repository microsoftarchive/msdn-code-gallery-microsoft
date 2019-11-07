//
// PictureItemsView.xaml.cpp
// Implementation of the PictureItemsView class
//

#include "pch.h"
#include "PictureItemsView.xaml.h"

using namespace SDKSample::DataSourceAdapter;

using namespace Windows::Storage;
using namespace Windows::Storage::BulkAccess;
using namespace Windows::Storage::FileProperties;
using namespace Windows::Storage::Search;
using namespace Windows::UI::Xaml::Navigation;

// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

PictureItemsView::PictureItemsView()
{
    InitializeComponent();
}

void PictureItemsView::OnNavigatedTo(NavigationEventArgs^ e)
{
    auto queryOptions = ref new QueryOptions();
    queryOptions->FolderDepth = FolderDepth::Shallow;
    queryOptions->IndexerOption = IndexerOption::UseIndexerWhenAvailable;
    queryOptions->SortOrder->Clear();
    SortEntry sortEntry;
    sortEntry.PropertyName = "System.IsFolder";
    sortEntry.AscendingOrder = false;
    queryOptions->SortOrder->Append(sortEntry);
    sortEntry.PropertyName = "System.ItemName";
    sortEntry.AscendingOrder = true;
    queryOptions->SortOrder->Append(sortEntry);

    auto fileQuery = KnownFolders::PicturesLibrary->CreateItemQueryWithOptions(queryOptions);
    const UINT32 size = 190; // default size for PicturesView mode
    auto fileInformationFactory = ref new FileInformationFactory(fileQuery, ThumbnailMode::PicturesView, size, ThumbnailOptions::UseCurrentScale, true);
    itemsViewSource->Source = fileInformationFactory->GetVirtualizedItemsVector();
}
