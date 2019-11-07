//*********************************************************
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF 
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
// PARTICULAR PURPOSE. 
// 
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// FindId.xaml.cpp
// Implementation of the FindId class
//

#include "pch.h"
#include "FindId.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::UnicodeSample;

using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Data::Text;

FindId::FindId()
{
    InitializeComponent();
}

/// <summary>
// This is a helper method that an app could create to find one or all available 
// ids within a string. 
/// </summary>
/// <param name="inputString">String that contains one or more ids</param>
/// <returns>List of individual ids found in the input string</returns>
Platform::Collections::Vector<String^>^ FindId::FindIdsInString(String^ inputString) 
{
    // List where we maintain the ids found in the input string
    Vector<String^>^ idList = ref new Vector<String^>();

    // Maintains the beginning the id found in the input string
    const wchar_t * idStart = nullptr;

    // Iterate through each of the characters in the string
    for (const wchar_t* character = inputString->Begin(); character < inputString->End(); character++)
    {
        unsigned int codepoint = *character;         
                
        // If the character is a high surrogate, then we need to read the next character to make
        // sure it is a low surrogate.  If we are at the last character in the input string, then
        // we have an error, since a high surrogate must be matched by a low surrogate.  Update
        // the code point with the surrogate pair.
        if (UnicodeCharacters::IsHighSurrogate(codepoint)) 
        {
            if (++character >= inputString->End()) 
            {
                throw ref new Platform::Exception(E_INVALIDARG, "Bad trailing surrogate at end of string");
            }

            codepoint = UnicodeCharacters::GetCodepointFromSurrogatePair(*(character -1), *character);
        }
                
        // Have we found an id start?
        if (idStart == nullptr) 
        {
            if (UnicodeCharacters::IsIdStart(codepoint)) 
            {
                // We found a character that is an id start.  In case we had a suplemmentary
                // character (high and low surrogate), then the index needs to offset by 1.
                idStart = UnicodeCharacters::IsSupplementary(codepoint) ?  character - 1 : character;             
            }         
        }         
        else if (!UnicodeCharacters::IsIdContinue(codepoint)) 
        {             
            // We have not found an id continue, so the id is complete.  We need to 
            // create the identifier string
            idList->Append(ref new String(idStart, character - idStart));
                    
            // Reset back the index start and re-examine the current code point 
            // in next iteration
            idStart = nullptr;
            character--;
        }     
    }

    // Do we have a pending id at the end of the string?
    if (idStart != nullptr) {

        //  We need to create the identifier string
        idList->Append(ref new String(idStart));
    }

    // Return the list of identifiers found in the string
    return idList;
}

/// <summary>
/// This method implements the scenario of finding all ids within a string.  It relies on
/// the FindIdsInString method to implement the logic.  This just takes care of finding and
/// logging the ids in the scenario window.
/// </summary>
/// <param name="scenarioString">String that contains one or more ids</param>
/// <returns>Output of the scenario to display</returns>
String^ FindId::DoFindIdsInStringScenario(String ^scenarioString) 
{
    // We keep results of the scenario here
    String^ results = "Found the following ids for \"" + scenarioString + "\":\n";

    // Iterate across each of the ids found in the string and add them to the results
    Vector<String^>^ idsInString = FindIdsInString(scenarioString);
    for (IIterator<String^>^ idIterator = idsInString->First(); idIterator->HasCurrent; idIterator->MoveNext()) 
    {
        results += idIterator->Current + "\n";
    }

    // End of the scenario
    results += "\n";
    return results;
}

/// <summary>
/// This is the click handler for the 'Display' button.  This runs through the scenarios in
/// the sample.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void UnicodeSample::FindId::Default_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // We keep results of the scenario here
    String^ results = "";

    // We run through a couple of scenarios, including some with surrogate pairs
    results += DoFindIdsInStringScenario(L"Hello, how are you?  I hope you are ok!");
    results += DoFindIdsInStringScenario(L"-->id<--");
    results += DoFindIdsInStringScenario(L"1id 2id 3id");
    results += DoFindIdsInStringScenario(L"id1 id2 id3");
    results += DoFindIdsInStringScenario(L"\xD840\xDC00_CJK_B_1 \xD840\xDC01_CJK_B_2 \xD840\xDC02_CJK_B_3");

    MainPage::Current->NotifyUser(results, NotifyType::StatusMessage);
}