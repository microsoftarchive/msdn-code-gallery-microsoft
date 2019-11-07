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
// Add.xaml.cpp
// Implementation of the Add class
//

#include "pch.h"
#include "Add.xaml.h"

using namespace SDKSample::Playlists;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Add::Add()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Add::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void Add::PickPlaylistButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // Make sure app is unsnapped before invoking the file picker
    if(!MainPage::EnsureUnsnapped())
    {
        OutputStatus->Text = "Unable to unsnap the app.";
        return;
    }

    MainPage::PickPlaylist().then([this](task<Playlist^> playlistTask)
    {
        try
        {
            MainPage::playlist = playlistTask.get();
            OutputStatus->Text = "Playlist loaded";
        }
        catch (NullReferenceException^ ex)
        {
            OutputStatus->Text = "No playlist picked.";
        }
        catch(AccessDeniedException^ ex)
        {
            OutputStatus->Text = "Access is denied, cannot load playlist.";
        }
    });
}

void Add::PickAudioButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (MainPage::playlist != nullptr) 
    {
        // Make sure app is unsnapped before invoking the file picker
        if(!MainPage::EnsureUnsnapped())
        {
            OutputStatus->Text = "Unable to unsnap the app.";
            return;
        }

        // Pick some files, then add them to the playlist.
        FileOpenPicker^ picker = MainPage::CreateFilePicker(MainPage::audioExtensions);
        task<IVectorView<StorageFile^>^>(picker->PickMultipleFilesAsync()).then([this](IVectorView<StorageFile^>^ files)
        {
            if(files->Size > 0)
            {
                for each (StorageFile^ file in files)
                {
                    MainPage::playlist->Files->Append(file);
                }

                // [this, files] allows the use of local variable "files" in lambda context
                task<void>(MainPage::playlist->SaveAsync()).then([this, files]()
                {
                    OutputStatus->Text = files->Size + " files added to the playlist.";
                });
            }
            else
            {
                OutputStatus->Text = "No file was selected.";
            }
        });
    }
    else
    {
        OutputStatus->Text = "Pick playlist first.";
    }
}
