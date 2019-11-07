//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#pragma once

#include <collection.h>

using namespace Concurrency;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Media::Playlists;
using namespace Windows::Storage;
using namespace Windows::Storage::Pickers;
using namespace Windows::UI::ViewManagement;

namespace SDKSample
{
    public value struct Scenario
    {
        String^ Title;
        String^ ClassName;
    };

    partial ref class MainPage
    {
    public:
        static property String^ FEATURE_NAME 
        {
            String^ get() 
            {  
                return ref new String(L"Playlists sample"); 
            }
        }

        static property Array<Scenario>^ scenarios 
        {
            Array<Scenario>^ get() 
            { 
                return scenariosInner; 
            }
        }

    internal:
        static property Array<String^>^ audioExtensions
        {
            Array<String^>^ get()
            {
                return audioExtensionsInner;
            }
        }
        static property Array<String^>^ playlistExtensions
        {
            Array<String^>^ get()
            {
                return playlistExtensionsInner;
            }
        }
        static property Playlist^ playlist
        {
            Playlist^  get()
            {
                return playlistInner;
            }
            void set(Playlist^ value)
            {
                playlistInner = value;
            }
        }

        static FileOpenPicker^ CreateFilePicker(Array<String^>^ extensions);
        static task<Playlist^> PickPlaylist();
        static bool EnsureUnsnapped();

    private:
        static Array<Scenario>^ scenariosInner;
        static Array<String^>^ audioExtensionsInner;
        static Array<String^>^ playlistExtensionsInner;
        static Playlist^ playlistInner;
    };
}
