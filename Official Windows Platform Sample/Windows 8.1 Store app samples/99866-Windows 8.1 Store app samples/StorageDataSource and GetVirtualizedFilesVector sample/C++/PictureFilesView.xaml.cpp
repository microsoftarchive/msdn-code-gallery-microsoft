//
// PictureFilesView.xaml.cpp
// Implementation of the PictureFilesView class
//

#include "pch.h"
#include "PictureFilesView.xaml.h"

using namespace SDKSample::DataSourceAdapter;

using namespace Windows::Storage;
using namespace Windows::Storage::BulkAccess;
using namespace Windows::Storage::FileProperties;
using namespace Windows::Storage::Search;
using namespace Windows::UI::Xaml::Navigation;

// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

PictureFilesView::PictureFilesView()
{
    InitializeComponent();
}

void PictureFilesView::OnNavigatedTo(NavigationEventArgs^ e)
{
    auto queryOptions = ref new QueryOptions();
    queryOptions->FolderDepth = FolderDepth::Deep;
    queryOptions->IndexerOption = IndexerOption::UseIndexerWhenAvailable;
    queryOptions->SortOrder->Clear();
    SortEntry sortEntry;
    sortEntry.PropertyName = "System.FileName";
    sortEntry.AscendingOrder = true;
    queryOptions->SortOrder->Append(sortEntry);

    auto fileQuery = KnownFolders::PicturesLibrary->CreateFileQueryWithOptions(queryOptions);
    const UINT32 size = 190; // default size for PicturesView mode
    auto fileInformationFactory = ref new FileInformationFactory(fileQuery, ThumbnailMode::PicturesView, size, ThumbnailOptions::UseCurrentScale, true);
    itemsViewSource->Source = fileInformationFactory->GetVirtualizedFilesVector();
}
