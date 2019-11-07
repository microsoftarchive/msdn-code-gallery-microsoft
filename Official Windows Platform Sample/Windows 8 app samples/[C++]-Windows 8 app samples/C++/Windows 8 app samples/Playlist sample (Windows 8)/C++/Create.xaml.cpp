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
// Create.xaml.cpp
// Implementation of the Create class
//

#include "pch.h"
#include "Create.xaml.h"

using namespace SDKSample::Playlists;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Create::Create()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Create::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void Create::PickAudioButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // Make sure app is unsnapped before invoking the file picker
    if(!MainPage::EnsureUnsnapped())
    {
        OutputStatus->Text = "Unable to unsnap the app.";
        return;
    }

    // Pick multiple files, then create the playlist.
    FileOpenPicker^ picker = MainPage::CreateFilePicker(MainPage::audioExtensions);

    task<IVectorView<StorageFile^>^>(picker->PickMultipleFilesAsync()).then([this](IVectorView<StorageFile^>^ files)
    {
        if(files->Size > 0)
        {
            MainPage::playlist = ref new Playlist();

            for each (StorageFile^ file in files)
            {
                MainPage::playlist->Files->Append(file);
            }

            StorageFolder^ folder = KnownFolders::MusicLibrary;
            String^ name = "Sample";
            NameCollisionOption collisionOption = NameCollisionOption::ReplaceExisting;
            PlaylistFormat format = PlaylistFormat::WindowsMedia;

            task<StorageFile^>(MainPage::playlist->SaveAsAsync(folder, name, collisionOption, format)).then([this](StorageFile^ file)
            {
                OutputStatus->Text = "The playlist " + file->Name + " was created and saved.";

                // Reset playlist so subsequent SaveAsync calls don't fail
                MainPage::playlist = nullptr;
            });
        }
        else
        {
            OutputStatus->Text = "No files picked.";
        }
    });
}
