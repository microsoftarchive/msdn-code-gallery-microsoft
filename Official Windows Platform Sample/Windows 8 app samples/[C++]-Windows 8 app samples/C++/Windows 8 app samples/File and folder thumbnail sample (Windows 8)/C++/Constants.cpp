//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Storage;
using namespace Windows::Storage::FileProperties;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Media::Imaging;

Array<Scenario>^ MainPage::scenariosInner = ref new Array<Scenario>
{
    { "Display a thumbnail for a picture",    "SDKSample.FileThumbnails.Scenario1" },
    { "Display album art for a song",         "SDKSample.FileThumbnails.Scenario2" },
    { "Display an icon for a document",       "SDKSample.FileThumbnails.Scenario3" },
    { "Display a thumbnail for a folder",     "SDKSample.FileThumbnails.Scenario4" },
    { "Display a thumbnail for a file group", "SDKSample.FileThumbnails.Scenario5" },
};

String^ Errors::NoExifThumbnail   = "No result (no EXIF thumbnail or cached thumbnail available for fast retrieval)";
String^ Errors::NoThumbnail       = "No result (no thumbnail could be obtained from the selected file)";
String^ Errors::NoAlbumArt        = "No result (no album art available for this song)";
String^ Errors::NoIcon            = "No result (no icon available for this document type)";
String^ Errors::NoImages          = "No result (no thumbnail could be obtained from the selected folder - make sure that the folder contains images)";
String^ Errors::FileGroupEmpty    = "No result (unexpected error: retrieved file group was null)";
String^ Errors::FileGroupLocation = "File groups are only available for library locations, please select a folder from one of your libraries";
String^ Errors::Cancel            = "No result (operation cancelled, no item selected)";

Array<String^>^ FileExtensions::Document = ref new Array<String^>
{
    ".doc",
    ".docx",
    ".xls",
    ".xlsx",
    ".ppt",
    ".pptx",
    ".pdf",
    ".txt",
    ".rtf",
};

Array<String^>^ FileExtensions::Image = ref new Array<String^>
{
    ".jpg",
    ".png",
    ".bmp",
    ".gif",
    ".tif",
};

Array<String^>^ FileExtensions::Music = ref new Array<String^>
{
    ".mp3",
    ".wma",
    ".m4a",
    ".aac",
};

BitmapImage^ MainPage::GetPlaceHolderImage()
{
    if (_placeholder == nullptr)
    {
        _placeholder = ref new BitmapImage(ref new Uri(BaseUri->AbsoluteUri, "Assets/placeholder-sdk.png"));
    }

    return _placeholder;
}

bool MainPage::EnsureUnsnapped()
{
    // FilePicker APIs will not work if the application is in a snapped state.
    // If an app wants to show a FilePicker while snapped, it must attempt to unsnap first
    bool unsnapped = ((ApplicationView::Value != ApplicationViewState::Snapped) || ApplicationView::TryUnsnap());
    if (!unsnapped)
    {
        NotifyUser("Cannot unsnap the sample application", NotifyType::StatusMessage);
    }

    return unsnapped;
}

void MainPage::ResetOutput(Image^ image, TextBlock^ output, TextBlock^ outputDetails)
{
    image->Source = GetPlaceHolderImage();
    NotifyUser("", NotifyType::ErrorMessage);
    NotifyUser("", NotifyType::StatusMessage);
    output->Text = "";
    if (outputDetails != nullptr)
    {
        outputDetails->Text = "";
    }
}

void MainPage::DisplayResult(Image^ image, TextBlock^ textBlock, String^ thumbnailModeName, size_t size, IStorageItem^ item, StorageItemThumbnail^ thumbnail, bool isGroup)
{
    BitmapImage^ bitmapImage = ref new BitmapImage();

    bitmapImage->SetSource(thumbnail);
    image->Source = bitmapImage;

    String^ itemType = isGroup ? "Group" : item->IsOfType(StorageItemTypes::File) ? "File" : "Folder";
    textBlock->Text = "ThumbnailMode." + thumbnailModeName + "\n"
        + itemType + " used: " + item->Name + "\n"
        + "Requested size: " + size.ToString() + "\n"
        + "Returned size: " + thumbnail->OriginalWidth.ToString() + "x" + thumbnail->OriginalHeight.ToString();
}
