// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"
#include "MainHubView.xaml.h"
#include "ImageNavigationData.h"

using namespace Hilo;
using namespace Platform;
using namespace Windows::UI::Xaml::Controls;

// See http://go.microsoft.com/fwlink/?LinkId=267278 for info on how Hilo creates pages and navigates to pages.

MainHubView::MainHubView()
{
    InitializeComponent();
}

void MainHubView::OnPhotoItemClicked(Object^ sender, ItemClickEventArgs^ e)
{
    auto photo = dynamic_cast<Photo^>(e->ClickedItem);
    if (nullptr !=  photo)
    {
        ImageNavigationData imageData(photo);
        HiloPage::NavigateToPage(PageType::Image, imageData.SerializeToString());
    }
}