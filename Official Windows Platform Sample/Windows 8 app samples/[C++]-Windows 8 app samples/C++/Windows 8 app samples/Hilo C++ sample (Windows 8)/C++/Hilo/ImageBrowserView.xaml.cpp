// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"
#include "ImageBrowserViewModel.h"
#include "ImageNavigationData.h"
#include "ImageBrowserView.xaml.h"

using namespace Hilo;
using namespace Platform;
using namespace Windows::UI::Xaml::Controls;

// See http://go.microsoft.com/fwlink/?LinkId=267278 for info on how Hilo creates pages and navigates to pages.

ImageBrowserView::ImageBrowserView()
{
    this->NavigationCacheMode = Windows::UI::Xaml::Navigation::NavigationCacheMode::Required;
    InitializeComponent();
}

void ImageBrowserView::OnPhotoItemClicked(Object^ sender, ItemClickEventArgs^ e)
{
    auto photo = dynamic_cast<Photo^>(e->ClickedItem);
    if (nullptr !=  photo)
    {
        ImageNavigationData imageData(photo);
        HiloPage::NavigateToPage(PageType::Image, imageData.SerializeToString());
    }
}

void ImageBrowserView::OnViewChangeCompleted(Object^ sender, SemanticZoomViewChangedEventArgs^ e)
{
    bool displayAppBar = false;

    if (!e->IsSourceZoomedInView)
    {
        displayAppBar = true;
        auto viewModel = dynamic_cast<ImageBrowserViewModel^>(DataContext);
        auto monthBlock = dynamic_cast<MonthBlock^>(e->SourceItem->Item);
        if (nullptr != monthBlock)
        {
            // Locate the first photo for selected month. This photo was previously cached by the view model.
            auto photo = viewModel->GetPhotoForYearAndMonth(monthBlock->Group->Year, monthBlock->Month);
            if (nullptr != photo)
            {
                // Scroll the month groups grid to the first photo of the selected month.
                MonthPhotosGridView->ScrollIntoView(photo);
            }
        }
    }

    ImageBrowserViewBottomAppBar->Visibility = displayAppBar ? Windows::UI::Xaml::Visibility::Visible : Windows::UI::Xaml::Visibility::Collapsed;
}

void Hilo::ImageBrowserView::OnZoomedOutGridPointerEntered(Platform::Object^ sender, Windows::UI::Xaml::Input::PointerRoutedEventArgs^ e)
{
    auto grid = dynamic_cast<Grid^>(sender);
    if (grid != nullptr)
    {
        // Disable the SemanticZoom control from transitioning from the 
        // zoomed-out view to the zoomed-in view when the user hovers over 
        // a month that does not contain pictures.
        IMonthBlock^ monthBlock = dynamic_cast<IMonthBlock^>(grid->DataContext);
        if (monthBlock == nullptr || !monthBlock->HasPhotos)
        {
            SemanticZoom->CanChangeViews = false;
        }
        else
        {
            SemanticZoom->CanChangeViews = true;
        }
    }
}
