//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario1.xaml.h
// Declaration of the Scenario1 class
//

#pragma once
#include "SpeakText.g.h"
using namespace Windows::Media::SpeechSynthesis;
using namespace Platform;

namespace SDKSample
{
	namespace VoiceSynthesisCPP
	{
		/// <summary>
		/// An empty page that can be used on its own or navigated to within a Frame.
		/// </summary>
		public ref class Scenario1 sealed
		{
		public:
			Scenario1();

		private:
			void BtnSpeak_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void BtnSaveToFile_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void ListboxVoiceChooser_DoubleTapped(Platform::Object^ sender, Windows::UI::Xaml::Input::DoubleTappedRoutedEventArgs^ e);
			void ListboxVoiceChooser_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e);
			void media_CurrentStateChanged(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

			// Internal functions
			void ListboxVoiceChooser_LoadVoices();
			void ShowFailureMessage(HRESULT hr);


			// this function will make the call to the speech synthesis engine, and determine
			// if it should use the SSML or Text implementation
			Concurrency::task<SpeechSynthesisStream ^> VoiceSynthesisCPP::Scenario1::GetSpeechStreamTask(String ^ text);

			// private member
			Windows::Media::SpeechSynthesis::SpeechSynthesizer^ synthesizer;
		};
	}
}
