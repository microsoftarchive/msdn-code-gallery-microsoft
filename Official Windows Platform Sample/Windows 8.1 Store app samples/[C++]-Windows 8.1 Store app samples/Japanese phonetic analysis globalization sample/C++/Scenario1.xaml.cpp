//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario1.xaml.cpp
// Implementation of the Scenario1 class
//

#include "pch.h"
#include "Scenario1.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::JapanesePhoneticAnalysis;

using namespace Platform;

using namespace Windows::Foundation::Collections;
using namespace Windows::Globalization;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

Scenario1::Scenario1()
{
    InitializeComponent();
}

void SDKSample::JapanesePhoneticAnalysis::Scenario1::StartButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    MainPage::Current->NotifyUser("Analyzing...", NotifyType::StatusMessage);
    String^ input = this->InputTextBox->Text;
    String^ output = "";

    // Split the Japanese text input in the text field into multiple words.
    // Without the second boolean parameter of GetWords(), each element in the result corresponds to a single Japanese word.
    IVectorView<JapanesePhoneme^>^ words = JapanesePhoneticAnalyzer::GetWords(input);
    for each (JapanesePhoneme^ word in words)
    {
        // IsPhraseStart indicates whether the word is the first word of a segment or not.
        if (!output->IsEmpty() && word->IsPhraseStart)
        {
            // Output a delimiter before each segment.
            output += "/";
        }

        // DisplayText is the display text of the word, which has same characters as the input of GetWords().
        // YomiText is the reading text of the word, as known as Yomi, which basically consists of Hiragana characters.
        // However, please note that the reading can contains some non-Hiragana characters for some display texts such as emoticons or symbols.
        output += word->DisplayText + "(" + word->YomiText + ")";
    }

    // Display the result.
    this->OutputTextBox->Text = output;
    if (!input->IsEmpty() && output->IsEmpty())
    {
        // If the result from GetWords() is empty but the input is not empty,
        // it means the given input is too long to analyze.
        MainPage::Current->NotifyUser("Failed to get words from the input text.  The input text is too long to analyze.", NotifyType::ErrorMessage);
    }
    else
    {
        // Otherwise, the analysis has done successfully.
        MainPage::Current->NotifyUser("Got words from the input text successfully.", NotifyType::StatusMessage);
    }
}
