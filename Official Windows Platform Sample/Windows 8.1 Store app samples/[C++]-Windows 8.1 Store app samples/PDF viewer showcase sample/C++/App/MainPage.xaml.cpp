// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "MainPage.xaml.h"

using namespace concurrency;
using namespace Platform;
using namespace Windows::ApplicationModel;
using namespace Windows::Data::Pdf;
using namespace Windows::Foundation;
using namespace Windows::Storage;
using namespace Windows::Storage::Pickers;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace PdfShowcase;

MainPage::MainPage()
{
    InitializeComponent();
}

/// <summary>
/// This overridden function is called whenever this page is navigated to
/// </summary>
void MainPage::OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e)
{
    // Load and render PDF file from the APPX Assets
    LoadDefaultFile();
}
/// <summary>
/// This function loads PDF file from the assets
/// </summary>
void MainPage::LoadDefaultFile()
{
    // Getting installed location of this app
    StorageFolder^ installedLocation = Package::Current->InstalledLocation;

    // Creating task to get the sample file from the assets folder
    create_task(installedLocation->GetFileAsync("Assets\\Sample.pdf")).then([this](_In_ StorageFile^ pdfFile)
    {
        loadedFile = pdfFile;
        LoadPDF(pdfFile);
    });
}

/// <summary>
/// Function to load the PDF file selected by the user
/// </summary>
/// <param name="pdfFile">StorageFile object of the selected PDF file</param>
void MainPage::LoadPDF(_In_ StorageFile^ pdfFile)
{
    // Creating task to load the PDF file and render pages in zoomed-in and zoomed-out view
    // For password protected documents one needs to call the function as is, handle the exception 
    // returned from LoadFromFileAsync and then call it again by providing the appropriate document 
    // password.
    create_task([this, pdfFile]()
    {
        return PdfDocument::LoadFromFileAsync(pdfFile);
    }).then([this](_In_ task<PdfDocument^> loadedDocTask)
    {
        try
        {
            pdfDocument = loadedDocTask.get();
        }
        catch (Platform::COMException^ e)
        {
        }
        Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([this]()
        {
            if (pdfDocument != nullptr)
            {
                InitializeZoomedInView();

                InitializeZoomedOutView();
            }
        }));
    });
}

/// <summary>
/// Function to initialize ZoomedInView of Semantic Zoom control
/// </summary>
void MainPage::InitializeZoomedInView()
{
    // Page Size is set to zero for items in main view so that pages of original size are rendered
    Size pageSize;

    pageSize.Width = Window::Current->Bounds.Width;
    pageSize.Height = Window::Current->Bounds.Height;

    // Main view items are rendered on a VSIS surface as they can be resized (optical zoom)
    zoomedInView = ref new ListView();
    zoomedInView->Style = zoomedInViewStyle;
    zoomedInView->ItemTemplate = zoomedInViewItemTemplate;
    zoomedInView->ItemsPanel = zoomedInViewItemsPanelTemplate;
    zoomedInView->Template = zoomedInViewControlTemplate;
    pdfDataSourceZoomedInView = ref new PdfViewModel::PdfDocViewModel(pdfDocument, pageSize, PdfViewModel::SurfaceType::VirtualSurfaceImageSource);
    zoomedInView->ItemsSource = pdfDataSourceZoomedInView;

    semanticZoom->ZoomedInView = zoomedInView;
}

/// <summary>
/// Function to initialize ZoomedOutView of Semantic Zoom control
/// </summary>
void MainPage::InitializeZoomedOutView()
{
    // Page Size is set to zero for items in main view so that pages of original size are rendered
    Size pageSize;

    // Page size for thumbnail view is set to 300px as this gives good view of the thumbnails on all resolutions
    pageSize.Width = (float)safe_cast<double>(this->Resources->Lookup("thumbnailWidth"));
    pageSize.Height = (float)safe_cast<double>(this->Resources->Lookup("thumbnailHeight"));

    // Thumbnail view items are rendered on a SIS surface as they are of fixed size
    pdfDataSourceZoomedOutView = ref new PdfViewModel::PdfDocViewModel(pdfDocument, pageSize, PdfViewModel::SurfaceType::SurfaceImageSource);

    zoomedOutView = ref new GridView();
    zoomedOutView->Style = zoomedOutViewStyle;
    zoomedOutView->ItemTemplate = zoomedOutViewItemTemplate;
    zoomedOutView->ItemsPanel = zoomedOutViewItemsPanelTemplate;
    zoomedOutView->ItemContainerStyle = zoomedOutViewItemContainerStyle;
    zoomedOutView->ItemsSource = pdfDataSourceZoomedOutView;
    semanticZoom->ZoomedOutView = zoomedOutView;
}

/// <summary>
/// Event Handler for handling application suspension
/// </summary>
void MainPage::OnSuspending(_In_ Object^ /*sender*/, _In_ SuspendingEventArgs^ /*args*/)
{
    // Hint to the driver that the app is entering an idle state and that its memory
    // can be temporarily used for other apps.
    pdfDataSourceZoomedInView->Trim();

    pdfDataSourceZoomedOutView->Trim();
}

/// <summary>
/// Open File click handler for command bar
/// This function loads the PDF file selected by the user
/// </summary>
void MainPage::OnOpenFileClick(_In_ Object^ /*sender*/, _In_ RoutedEventArgs^ /*e*/)
{
    // Launching FilePicker
    FileOpenPicker^ openPicker = ref new FileOpenPicker();
    openPicker->SuggestedStartLocation = PickerLocationId::DocumentsLibrary;
    openPicker->ViewMode = PickerViewMode::List;
    openPicker->FileTypeFilter->Append(L".pdf");

    // Creating sync task for PickSingleFileAsync
    create_task(openPicker->PickSingleFileAsync()).then([this](_In_ StorageFile^ pdfFile)
    {
        if (pdfFile != nullptr)
        {
            // Validating if selected file is not the same as file currently loaded
            if (loadedFile->Path != pdfFile->Path)
            {
                loadedFile = pdfFile;
                LoadPDF(pdfFile);
            }
        }
    });
}

/// <summary>
/// Event Handler for ViewChanged event of ScrollViewer for zoomedout view
/// This method is invoked to recreate VSIS surface of new width/height and re-render the page image at high resolution
/// </summary>
/// <param name="sender">Scroll Viewer</param>
/// <param name="e">ScrollViewerViewChangedEventArgs</param>
void MainPage::EventHandlerViewChanged(_In_ Object^ sender, _In_ ScrollViewerViewChangedEventArgs^ e)
{
    if (!e->IsIntermediate)
    {
        auto scrollViewer = dynamic_cast<ScrollViewer^>(sender);

        // Reloading pages at new zoomFactor
        if (scrollViewer != nullptr)
        {
            pdfDataSourceZoomedInView->UpdatePages(scrollViewer->ZoomFactor);
        }
    }
}

/// <summary>
/// Event handler for ViewChangeStarted event for SemanticZoom
/// </summary>
/// <param name="e">SemanticZoomViewChangedEventArgs</param>
void MainPage::EventHandlerViewChangeStarted(_In_ Object^ /*sender*/, _In_ SemanticZoomViewChangedEventArgs^ e)
{
    auto sourceItem = dynamic_cast<PdfViewModel::PdfPageViewModel^>(e->SourceItem->Item);
    if (sourceItem != nullptr)
    {
        unsigned int pageIndex = sourceItem->PageIndex;
        if (pdfDataSourceZoomedInView->Size > pageIndex)
        {
            // Transitioning from Zooomed Out View to Zoomed In View
            if (semanticZoom->IsZoomedInViewActive)
            {
                // Getting destination item from Zoomed-In-View
                Object^ destinationItem = pdfDataSourceZoomedInView->GetAt(pageIndex);

                if (destinationItem != nullptr)
                {
                    e->DestinationItem->Item = destinationItem;
                }
            }
            // Transitioning from Zooomed In View to Zoomed Out View
            else
            {
                // Getting destination item from Zoomed-In-View
                Object^ destinationItem = pdfDataSourceZoomedOutView->GetAt(pageIndex);

                if (destinationItem != nullptr)
                {
                    e->DestinationItem->Item = destinationItem;
                }
            }
        }
    }
}