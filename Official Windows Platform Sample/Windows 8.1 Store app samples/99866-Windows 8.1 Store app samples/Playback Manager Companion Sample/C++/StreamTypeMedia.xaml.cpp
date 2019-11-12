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
// StreamTypeMedia.xaml.cpp
// Implementation of the StreamTypeMedia class
//

#include "pch.h"
#include "StreamTypeMedia.xaml.h"

using namespace SDKSample::PlaybackManager2;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Media;

StreamTypeMedia::StreamTypeMedia()
	: _playback(nullptr)
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void StreamTypeMedia::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Get a pointer to our main page.
    rootPage = MainPage::Current;
}


void PlaybackManager2::StreamTypeMedia::Default_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	if(_playback == nullptr)
	{
		_playback = ref new Playback(Output);
	}

	_playback->SetAudioCategory(AudioCategory::BackgroundCapableMedia);
	_playback->SetSourceStream("folk_rock.mp3");
}
