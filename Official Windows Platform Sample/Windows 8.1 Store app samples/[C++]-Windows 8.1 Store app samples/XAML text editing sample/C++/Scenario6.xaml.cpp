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
// Scenario6.xaml.cpp
// Implementation of the Scenario6 class
//

#include "pch.h"
#include "Scenario6.xaml.h"

using namespace SDKSample::TextEditing;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::UI;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Text;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams ;

Scenario6::Scenario6()
{
    InitializeComponent();
}

void Scenario6::BoldButtonClick(Object^ sender, RoutedEventArgs^ e)
{
    ITextSelection^ selectedText = editor->Document->Selection;
	if (selectedText != nullptr)
    {
        ITextCharacterFormat^ charFormatting = selectedText->CharacterFormat;
        charFormatting->Bold = FormatEffect::Toggle;
        selectedText->CharacterFormat = charFormatting;
    }
}

void Scenario6::ItalicButtonClick(Object^ sender, RoutedEventArgs^ e)
{
    ITextSelection^ selectedText = editor->Document->Selection;
    if (selectedText != nullptr)
    {
        ITextCharacterFormat^ charFormatting = selectedText->CharacterFormat;
        charFormatting->Italic = FormatEffect::Toggle;
        selectedText->CharacterFormat = charFormatting;
    }
}

void Scenario6::FontColorButtonClick(Object^ sender, RoutedEventArgs^ e)
{
    fontColorPopup->IsOpen = true;
    fontColorButton->Focus(Windows::UI::Xaml::FocusState::Keyboard);
}

void Scenario6::FindBoxTextChanged(Object^ sender, TextChangedEventArgs^ e)
{
    String^ textToFind = findBox->Text;
	
	if (textToFind != "")
        FindAndHighlightText(textToFind);
}

void Scenario6::FontColorButtonLostFocus(Object^ sender, RoutedEventArgs^ e)
{
    fontColorPopup->IsOpen = false;
}

void Scenario6::FindBoxLostFocus(Object^ sender, RoutedEventArgs^ e)
{
    ClearAllHighlightedWords();
}

void Scenario6::FindBoxGotFocus(Object^ sender, RoutedEventArgs^ e)
{
    String^ textToFind = findBox->Text;

	if (textToFind != "")
        FindAndHighlightText(textToFind);
}

void Scenario6::ClearAllHighlightedWords()
{
    ITextCharacterFormat^ charFormat;
    for (unsigned int i = 0; i < m_highlightedWords.size() ; i++)
    {
        charFormat = m_highlightedWords[i]->CharacterFormat;
        charFormat->BackgroundColor = Colors::Transparent;
        m_highlightedWords[i]->CharacterFormat = charFormat;
    }

    m_highlightedWords.clear();
}

void Scenario6::FindAndHighlightText(String^ textToFind)
{
    ClearAllHighlightedWords();

    ITextRange^ searchRange = editor->Document->GetRange(0, 1000000000);
	searchRange->Move(TextRangeUnit::Character,0) ;

    bool textFound = true;
    do
    {
        if (searchRange->FindText(textToFind, 1000000000, FindOptions::None) < 1)
            textFound = false;
        else
        {
            m_highlightedWords.push_back(searchRange->GetClone());

            ITextCharacterFormat^ charFormatting = searchRange->CharacterFormat;
            charFormatting->BackgroundColor = Colors::Yellow;
            searchRange->CharacterFormat = charFormatting;
        }
    } while (textFound);
}

void Scenario6::ColorButtonClick(Object^ sender, RoutedEventArgs^ e)
{
	Button^ clickedColor = dynamic_cast<Button^>(sender);

    ITextCharacterFormat^ charFormatting = editor->Document->Selection-> CharacterFormat;
    if(clickedColor->Name == "black")
		charFormatting->ForegroundColor = Colors::Black;
	else
		if(clickedColor->Name == "gray")
			charFormatting->ForegroundColor = Colors::Gray;
		else
			if(clickedColor->Name == "darkgreen")
				charFormatting->ForegroundColor = Colors::DarkGreen;
			else
				if(clickedColor->Name == "green")
					charFormatting->ForegroundColor = Colors::Green;
				else
					if(clickedColor->Name == "blue")
						charFormatting->ForegroundColor = Colors::Blue;

    editor->Document->Selection->CharacterFormat = charFormatting;

    editor->Focus(Windows::UI::Xaml::FocusState::Keyboard);
    fontColorPopup->IsOpen = false;
}

void Scenario6::LoadContentAsync()
{
	IAsyncOperation<StorageFile^>^ getFileResult = Windows::ApplicationModel::Package::Current->InstalledLocation->GetFileAsync("lorem.rtf"); 
	StorageFile^ file = getFileResult->GetResults() ;

	IAsyncOperation<IRandomAccessStream^>^ openFileResults = file->OpenAsync(FileAccessMode::Read) ;
	IRandomAccessStream^ randAccStream = openFileResults->GetResults() ;

    editor->Document->LoadFromStream(TextSetOptions::FormatRtf, randAccStream);
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario6::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::TextEditing::Scenario6::Default_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = safe_cast<Button^>(sender);
    if (b != nullptr)
    {
        rootPage->NotifyUser("You clicked the " + b->Name + " button", NotifyType::StatusMessage);
    }
}

void SDKSample::TextEditing::Scenario6::Other_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = safe_cast<Button^>(sender);
    if (b != nullptr)
    {
        rootPage->NotifyUser("You clicked the " + b->Name + " button", NotifyType::StatusMessage);
    }
}
