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
// Clear.xaml.cpp
// Implementation of the Clear class
//

#include "pch.h"
#include "Clear.xaml.h"

using namespace SDKSample::Playlists;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Clear::Clear()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Clear::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void Clear::PickPlaylistButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
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
        
            if (MainPage::playlist != nullptr)
            {
                // Clear playlist.
                MainPage::playlist->Files->Clear();
                task<void>(MainPage::playlist->SaveAsync()).then([this]()
                {
                    OutputStatus->Text = "Playlist cleared.";
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
