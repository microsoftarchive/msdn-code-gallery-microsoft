//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

//
// ImageProtocols.xaml.cpp
// Implementation of the ImageProtocols class
//

#include "pch.h"
#include "ImageProtocols.xaml.h"

using namespace SDKSample::Tiles;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Notifications;
using namespace NotificationsExtensions::TileContent;
using namespace Windows::Storage::Pickers;
using namespace Windows::Storage;
using namespace concurrency;

ImageProtocols::ImageProtocols()
{
    InitializeComponent();
}

void ImageProtocols::OnNavigatedTo(NavigationEventArgs^ e)
{
    rootPage = MainPage::Current;
    LocalFolder->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    HTTP->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    ProtocolList->SelectedIndex = 0;
    imageRelativePath = "copy a file first";
}

void ImageProtocols::ProtocolList_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e)
{
    LocalFolder->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    HTTP->Visibility = Windows::UI::Xaml::Visibility::Collapsed;

    if (ProtocolList->SelectedIndex == 1)
    {
        LocalFolder->Visibility = Windows::UI::Xaml::Visibility::Visible;
    }
    else if (ProtocolList->SelectedIndex == 2)
    {
        HTTP->Visibility = Windows::UI::Xaml::Visibility::Visible;
    }
}

void ImageProtocols::SendTileNotification_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{	
    IWideTileNotificationContent^ tileContent;
    if (ProtocolList->SelectedIndex == 0) //using the ms-appx:/// protocol
    {
        auto wideContent = TileContentFactory::CreateTileWideImageAndText01();
        wideContent->TextCaptionWrap->Text = "The image is in the appx package";
        wideContent->Image->Src = "ms-appx:///redWide.png";
        wideContent->Image->Alt = "Red image";

        tileContent = wideContent;
    }
    else if (ProtocolList->SelectedIndex == 1) //using the ms-appdata:///local/ protocol
    {
        auto wideContent = TileContentFactory::CreateTileWideImage();		
        wideContent->Image->Src = imageRelativePath;
        wideContent->Image->Alt ="App data"; 
        tileContent = wideContent;
    }
    else if (ProtocolList->SelectedIndex == 2) //using http:// protocol
    {
        // Important - The Internet (Client) capability must be checked in the manifest in the Capabilities tab
        auto wideContent = TileContentFactory::CreateTileWidePeekImageCollection04();
        wideContent->BaseUri = HTTPBaseURI->Text;
        wideContent->TextBodyWrap->Text = "The base URI is " + HTTPBaseURI->Text;
        wideContent->ImageMain->Src = HTTPImage1->Text; 
        wideContent->ImageSmallColumn1Row1->Src = HTTPImage2->Text;
        wideContent->ImageSmallColumn1Row2->Src = HTTPImage3->Text;
        wideContent->ImageSmallColumn2Row1->Src = HTTPImage4->Text;
        wideContent->ImageSmallColumn2Row2->Src = HTTPImage5->Text;

        tileContent = wideContent;
    }

    if (tileContent != nullptr)
    {
        tileContent->RequireSquareContent = false;
        TileUpdateManager::CreateTileUpdaterForApplication()->Update(tileContent->CreateNotification());

        OutputTextBlock->Text = tileContent->GetContent();
    }
}

void ImageProtocols::PickImage_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (rootPage->EnsureUnsnapped())  
    {
        auto openPicker = ref new FileOpenPicker();
        openPicker->ViewMode = PickerViewMode::Thumbnail;
        openPicker->SuggestedStartLocation = PickerLocationId::PicturesLibrary;
        openPicker->FileTypeFilter->Append(".png");
        openPicker->FileTypeFilter->Append(".gif");

        task<StorageFile^> pickImageTask(openPicker->PickSingleFileAsync());
        TextBlock^ outputBlock = OutputTextBlock;
        pickImageTask.then([this, outputBlock](StorageFile^ file) 
        {
            if( file != nullptr )
            {
                // Get the returned file and copy it
                auto local = ApplicationData::Current->LocalFolder;		
                task<StorageFile^> copyFileTask(file->CopyAsync(local, file->Name, NameCollisionOption::ReplaceExisting));
                imageRelativePath = file->Name;
                copyFileTask.then([outputBlock,file](task<StorageFile^> copiedFileTask) 
                {
                    StorageFile^ copiedFile;
                    try
                    {
                        copiedFile = copiedFileTask.get();
                    }
                    catch (Platform::Exception^ e)
                    {
                        outputBlock->Text = "Error copying file";
                    }

                    if (copiedFile != nullptr)
                    {
                        outputBlock->Text = "File " + copiedFile->Name + " was copied from " + file->Name;
                    }
                });
            }
            else
            {
                outputBlock->Text = "File selection cancelled by user.";
            }
        });
    }
    else
    {
        OutputTextBlock->Text = "Cannot unsnap the sample application.";
    }
}
