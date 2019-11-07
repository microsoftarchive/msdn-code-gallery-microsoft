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

Array<Scenario>^ MainPage::scenariosInner = ref new Array<Scenario>  
{
    // The format here is the following:
    //     { "Description for the sample", "Fully quaified name for the class that implements the scenario" }
    { "Create a playlist", "SDKSample.Playlists.Create"},
    { "Display a playlist", "SDKSample.Playlists.Display"},
    { "Add items to a playlist", "SDKSample.Playlists.Add"},
    { "Remove an item from a playlist", "SDKSample.Playlists.Remove"},
    { "Clear a playlist", "SDKSample.Playlists.Clear"}
};

Array<String^>^ MainPage::audioExtensionsInner = ref new Array<String^>
{
    L".wma",  L".mp3",  L".mp2",  L".aac", L".adt", L".adts", L".m4a"
};

Array<String^>^ MainPage::playlistExtensionsInner = ref new Array<String^>
{
    L".m3u", L".wpl", L".zpl"
};

Playlist^ MainPage::playlistInner = nullptr;

FileOpenPicker^ MainPage::CreateFilePicker(Array<String^>^ extensions)
{
    FileOpenPicker^ picker = ref new FileOpenPicker();
    picker->SuggestedStartLocation = PickerLocationId::MusicLibrary;

    // Load the picker with the desired extensions.
    for(unsigned int i = 0; i < extensions->Length; i++)
    {
        picker->FileTypeFilter->Append(extensions[i]);
    }

    return picker;
}

task<Playlist^> MainPage::PickPlaylist()
{
    // Pick a file, then load it as a playlist.
    FileOpenPicker^ picker = MainPage::CreateFilePicker(MainPage::playlistExtensions);
    return task<StorageFile^>(picker->PickSingleFileAsync()).then([](StorageFile^ file)
    {
        return Playlist::LoadAsync(file);
    });
}

bool MainPage::EnsureUnsnapped()
{
    bool success = true;

    // Try to unsnap if we are currently snapped
    if(ApplicationView::Value == ApplicationViewState::Snapped)
    {
        success = ApplicationView::TryUnsnap();
    }

    return success;
}
