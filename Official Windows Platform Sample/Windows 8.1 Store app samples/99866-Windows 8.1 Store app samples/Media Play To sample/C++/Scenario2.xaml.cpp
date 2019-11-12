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
// Scenario2.xaml.cpp
// Implementation of the Scenario2 class
//

#include "pch.h"
#include "Scenario2.xaml.h"

using namespace SDKSample::PlayTo;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::Foundation;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;
using namespace Windows::Storage::Pickers;
using namespace Windows::Media::PlayTo;
using namespace concurrency;

Scenario2::Scenario2()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario2::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
	dispatcher = Window::Current->Dispatcher;

	playlistTimer = ref new DispatcherTimer();
	TimeSpan interval = { 10 * 10000000 };
	playlistTimer->Interval = interval;
	timerTickToken = playlistTimer->Tick += ref new EventHandler<Platform::Object^>(this, &Scenario2::playlistTimer_Tick);

	playToManager = PlayToManager::GetForCurrentView();
	sourceRequestedToken = playToManager->SourceRequested += ref new TypedEventHandler<PlayToManager^, PlayToSourceRequestedEventArgs^>(this, &Scenario2::playToManager_SourceRequested);
	playListPlayNext();
}

void Scenario2::playToManager_SourceRequested(Windows::Media::PlayTo::PlayToManager^ sender, Windows::Media::PlayTo::PlayToSourceRequestedEventArgs^ e)
{
	auto deferral = e->SourceRequest->GetDeferral();

	Windows::UI::Core::DispatchedHandler^ handler = ref new Windows::UI::Core::DispatchedHandler([=] (){

		e->SourceRequest->SetSource(ImageSource->PlayToSource);
		deferral->Complete();

	}, Platform::CallbackContext::Any);

	auto action = dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, handler);
}

void Scenario2::playlistTimer_Tick(Platform::Object^ sender, Platform::Object^ e)
{
    if (Playlist != nullptr)
    {
        playListPlayNext();
    }
}

void Scenario2::OnNavigatedFrom(NavigationEventArgs^ e)
{
	playToManager->SourceRequested -= sourceRequestedToken;
	playlistTimer->Tick -= timerTickToken;
}

void Scenario2::playSlideshow_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = safe_cast<Button^>(sender);
    if (b != nullptr)
    {
		playlistTimer->Start();
        rootPage->NotifyUser("Playlist playing", NotifyType::StatusMessage);
    }
}


void Scenario2::pauseSlideshow_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = safe_cast<Button^>(sender);
    if (b != nullptr)
    {
		playlistTimer->Stop();
        rootPage->NotifyUser("Playlist paused", NotifyType::StatusMessage);
    }
}


void Scenario2::previousItem_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = safe_cast<Button^>(sender);
    if (b != nullptr)
    {
		Playlist->SelectedIndex = (Playlist->SelectedIndex - 1 >= 0) ? Playlist->SelectedIndex - 1 : Playlist->Items->Size - 1;
        rootPage->NotifyUser("Previous item selected in the list", NotifyType::StatusMessage);
    }
}


void Scenario2::nextItem_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = safe_cast<Button^>(sender);
    if (b != nullptr)
    {
		playListPlayNext();
        rootPage->NotifyUser("Next item selected in the list", NotifyType::StatusMessage);
    }
}

void Scenario2::playListPlayNext()
{
    if (Playlist != nullptr)
    {
		Playlist->SelectedIndex = (Playlist->SelectedIndex + 1 < (int)Playlist->Items->Size) ? Playlist->SelectedIndex + 1 : 0;
    }
}

void Scenario2::Playlist_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e)
{
	if (Playlist != nullptr)
    {
		ListBoxItem^ selectedItem = dynamic_cast<ListBoxItem^>(Playlist->SelectedItem);
		ImageSource->Source = ref new BitmapImage(ref new Uri("ms-appx:///Assets/" + selectedItem->Content->ToString()));
		rootPage->NotifyUser(selectedItem->Content->ToString() + " selected from the list", NotifyType::StatusMessage);
    }
}
