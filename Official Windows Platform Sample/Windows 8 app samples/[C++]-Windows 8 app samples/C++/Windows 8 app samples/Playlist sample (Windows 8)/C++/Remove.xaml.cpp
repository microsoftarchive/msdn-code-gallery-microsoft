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
// Remove.xaml.cpp
// Implementation of the Remove class
//

#include "pch.h"
#include "Remove.xaml.h"

using namespace SDKSample::Playlists;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Remove::Remove()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Remove::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void Remove::PickPlaylistButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // Make sure app is unsnapped before invoking the file picker
    if(!MainPage::EnsureUnsnapped())
    {
        OutputStatus->Text = "Unable to unsnap the app.";
        return;
    }

    // Pick a file, load it as a playlist, then remove the last item in the playlist.
    MainPage::PickPlaylist().then([this](task<Playlist^> playlistTask)
    {
        try
        {
            MainPage::playlist = playlistTask.get();

            if (MainPage::playlist->Files->Size > 0) 
            {
                // Remove the last item.
                MainPage::playlist->Files->RemoveAtEnd();
                task<void>(MainPage::playlist->SaveAsync()).then([this]()
                {
                    OutputStatus->Text = "The last item in the playlist was removed.";
                });
            }
            else
            {
                OutputStatus->Text = "No items in playlist.";
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
