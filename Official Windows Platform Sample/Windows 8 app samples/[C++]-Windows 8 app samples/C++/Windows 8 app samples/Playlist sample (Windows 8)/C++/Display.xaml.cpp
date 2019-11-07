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
// Display.xaml.cpp
// Implementation of the Display class
//

#include "pch.h"
#include "Display.xaml.h"

using namespace SDKSample::Playlists;

using namespace Windows::Storage::FileProperties;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Display::Display()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Display::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void Display::PickPlaylistButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
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

            OutputStatus->Text = "Playlist content:\n";
            if (MainPage::playlist->Files->Size == 0)
            {
                OutputStatus->Text += "(playlist is empty)";
            }
            else
            {
                // create a vector to store the async operations
                std::vector<task<MusicProperties^>> results(MainPage::playlist->Files->Size);
                for (unsigned int i = 0; i < MainPage::playlist->Files->Size; i++)
                {
                    StorageFile^ file = (StorageFile^)(MainPage::playlist->Files->GetAt(i));
                    results[i] = task<MusicProperties^>(file->Properties->GetMusicPropertiesAsync());
                }

                // wait for all of the async operations to complete and then output the results
                when_all(results.begin(), results.end()).then([this](std::vector<MusicProperties^> results)
                {
                    for (unsigned int i = 0; i < results.size(); i++)
                    {
                        MusicProperties^ properties = results[i];
                        OutputStatus->Text += "\n";
                        OutputStatus->Text += "Title: " + properties->Title + "\n";
                        OutputStatus->Text += "Album: " + properties->Album + "\n";
                        OutputStatus->Text += "Artist: " + properties->Artist + "\n";
                    }
                });
            }
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
