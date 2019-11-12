//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario2.xaml.h
// Declaration of the Scenario2 class
//

#pragma once
#include "SpeakSSML.g.h"
using namespace Windows::Media::SpeechSynthesis;
using namespace Platform;

namespace SDKSample
{
	namespace VoiceSynthesisCPP
	{
		/// <summary>
		/// An empty page that can be used on its own or navigated to within a Frame.
		/// </summary>
		public ref class Scenario2 sealed
		{
		public:
			Scenario2();

		private:
			void DoSomething_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void BtnSpeak_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void BtnSaveToFile_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void ListboxVoiceChooser_DoubleTapped(Platform::Object^ sender, Windows::UI::Xaml::Input::DoubleTappedRoutedEventArgs^ e);
			void ListboxVoiceChooser_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e);
			void media_CurrentStateChanged(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

			// Internal functions
			void ListboxVoiceChooser_LoadVoices();
			void ShowFailureMessage(HRESULT hr);
            void UpdateSSMLText();


			// this function will make the call to the speech synthesis engine, and determine
			// if it should use the SSML or Text implementation
			Concurrency::task<SpeechSynthesisStream ^> VoiceSynthesisCPP::Scenario2::GetSpeechStreamTask(String ^ text);

			// private member
			Windows::Media::SpeechSynthesis::SpeechSynthesizer^ synthesizer;
		};
	}
}
