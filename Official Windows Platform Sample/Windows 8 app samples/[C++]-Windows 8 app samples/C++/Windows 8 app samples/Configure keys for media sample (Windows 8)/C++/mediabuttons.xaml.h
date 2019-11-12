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
// mediabuttons.xaml.h
// Declaration of the mediabuttons class
//

#pragma once


#include "pch.h"
#include "mediabuttons.g.h"
#include "MainPage.xaml.h"

using namespace Windows::Foundation;
using namespace Windows::Storage;
using namespace Platform;
using namespace Concurrency;

namespace SDKSample
{
    namespace MediaButtons
    {
    	ref class Song sealed
    	{
        public:
            Song(StorageFile^ file)
            {
                fileInner = file;
            }
    		
    
            property StorageFile^ File {
                StorageFile^ get(){return fileInner;}
            }
    
    			
            property String^ Artist {
                String^ get(){return artistInner;}
    			void set(String^ propArtist)
    			{
    				artistInner = propArtist;
    			}
            }
    
            property String^ Track {
                String^ get(){return trackInner;}
    			void set(String^ propTrack)
    			{
    				trackInner = propTrack;
    			}
            }
    
        private:
            StorageFile^ fileInner;
            String^ artistInner;
            String^ trackInner;
        };
    
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class mediabuttons sealed
        {
        public:
            mediabuttons();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
    
    	private:
            MainPage^ rootPage;
            void SelectFiles_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void Play_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    
    		void SetMediaElementSourceAsync(Song^ song);
            void CreatePlaylist(Windows::Foundation::Collections::IVectorView<StorageFile^>^ files);
    		void SetCurrentPlayingAsync(int playlistIndex);
            void DisplayStatus(Platform::String^ text);
            Platform::String^ GetTimeStampedMessage(Platform::String^ EventCalled);
    
            void MediaElement_MediaEnded(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void MediaElement_MediaOpened(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void MediaElement_CurrentStateChanged(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    
            void PlayPause(Object^ sender, Object^ e);
            void Play(Object^ sender, Object^ e);
            void Stop(Object^ sender, Object^ e);
            void Pause(Object^ sender, Object^ e);
            void NextTrack(Object^ sender, Object^ e);
            void PrevTrack(Object^ sender, Object^ e);
            void FastForward(Object^ sender, Object^ e);
            void Rewind(Object^ sender, Object^ e);
            void ChannelUp(Object^ sender, Object^ e);
            void ChannelDown(Object^ sender, Object^ e);
            void Record(Object^ sender, Object^ e);
        
        private:
            bool IsInitialized;
            Windows::UI::Core::CoreDispatcher^ _dispatcher;
    
            bool _wasPlaying;
            int _currentSongIndex;
            int _playlistCount;
    		EventRegistrationToken _nextTrackPressedEventToken;
    		EventRegistrationToken _prevTrackPressedEventToken;
            Platform::Collections::Vector<Song^>^ _playlist;
    
        };
    }
}
