//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario2.xaml.cpp
// Implementation of the Scenario2 class
//

#include "pch.h"
#include "Scenario2.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::JapanesePhoneticAnalysis;

using namespace Platform;

using namespace Windows::Foundation::Collections;
using namespace Windows::Globalization;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

Scenario2::Scenario2()
{
    InitializeComponent();
}

void SDKSample::JapanesePhoneticAnalysis::Scenario2::StartButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    MainPage::Current->NotifyUser("Analyzing...", NotifyType::StatusMessage);
    String^ input = this->InputTextBox->Text;
    bool monoRuby = this->MonoRubyCheckBox->IsChecked != nullptr && this->MonoRubyCheckBox->IsChecked->Value;
    String^ output = "";

    // Split the Japanese text input in the text field into multiple words.
    // Based on the second boolean parameter of GetWords(), the unit of segmentation changes.
    // When the second parameter is true, each element in the result corresponds to one or more characters which have an atomic reading.
    // When the second parameter is false, each element in the result corresponds to a single Japanese word.
    IVectorView<JapanesePhoneme^>^ words = JapanesePhoneticAnalyzer::GetWords(input, monoRuby);
    for each (JapanesePhoneme^ word in words)
    {
        // IsPhraseStart indicates whether the word is the first word of a segment or not.
        if (!output->IsEmpty() && word->IsPhraseStart)
        {
            // Output a delimiter before each segment.
            output += "/";
        }

        // DisplayText property has the display text of the word, which has same characters as the input of GetWords().
        // YomiText property has the reading text of the word, as known as Yomi, which basically consists of Hiragana characters.
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
