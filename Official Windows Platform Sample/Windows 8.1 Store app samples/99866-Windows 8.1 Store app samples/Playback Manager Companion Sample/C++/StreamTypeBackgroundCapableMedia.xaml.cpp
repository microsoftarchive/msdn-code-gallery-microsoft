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
// StreamTypeBackgroundCapableMedia.xaml.cpp
// Implementation of the StreamTypeBackgroundCapableMedia class
//

#include "pch.h"
#include "StreamTypeBackgroundCapableMedia.xaml.h"

using namespace SDKSample::PlaybackManager2;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Media;

StreamTypeBackgroundCapableMedia::StreamTypeBackgroundCapableMedia()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void StreamTypeBackgroundCapableMedia::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Get a pointer to our main page.
    rootPage = MainPage::Current;
}


void StreamTypeBackgroundCapableMedia::Default_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	Playback->SetAudioCategory(AudioCategory::BackgroundCapableMedia);
    Playback->SelectFile();
}
