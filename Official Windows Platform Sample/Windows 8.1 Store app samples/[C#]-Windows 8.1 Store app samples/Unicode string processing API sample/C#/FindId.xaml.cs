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

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Data.Text;
using SDKTemplate;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace UnicodeSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FindId : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public FindId()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        /// <summary>
        // This is a helper method that an app could create to find one or all available 
        // ids within a string. 
        /// </summary>
        /// <param name="inputString">String that contains one or more ids</param>
        /// <returns>List of individual ids found in the input string</returns>
        private List<String> FindIdsInString(String inputString)
        {
            // List where we maintain the ids found in the input string
            List<String> idList = new List<String>();

            // Maintains the beginning index of the id found in the input string
            int indexIdStart = -1;

            // Iterate through each of the characters in the string
            for (int i = 0; i < inputString.Length; i++) 
            {
                uint codepoint = inputString[i];         
                
                // If the character is a high surrogate, then we need to read the next character to make
                // sure it is a low surrogate.  If we are at the last character in the input string, then
                // we have an error, since a high surrogate must be matched by a low surrogate.  Update
                // the code point with the surrogate pair.
                if (UnicodeCharacters.IsHighSurrogate(codepoint))         
                {
                    if (++i >= inputString.Length)             
                    {
                        throw new ArgumentException("Bad trailing surrogate at end of string");
                    }
                    codepoint = UnicodeCharacters.GetCodepointFromSurrogatePair(inputString[i - 1], inputString[i]);
                }  
                
                // Have we found an id start?
                if (indexIdStart == -1)         
                {
                    if (UnicodeCharacters.IsIdStart(codepoint))
                    {
                        // We found a character that is an id start.  In case we had a suplemmentary
                        // character (high and low surrogate), then the index needs to offset by 1.
                        indexIdStart = UnicodeCharacters.IsSupplementary(codepoint) ? i - 1 : i;
                    }
                }             
                else if (!UnicodeCharacters.IsIdContinue(codepoint))         
                {             
                    // We have not found an id continue, so the id is complete.  We need to 
                    // create the identifier string
                    idList.Add(inputString.Substring(indexIdStart, i - indexIdStart));
                    
                    // Reset back the index start and re-examine the current code point 
                    // in next iteration
                    indexIdStart = -1;
                    i--;
                }     
            }

            // Do we have a pending id at the end of the string?
            if (indexIdStart != -1)
            {
                //  We need to create the identifier string
                idList.Add(inputString.Substring(indexIdStart));
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
        private string DoFindIdsInStringScenario(string scenarioString)
        {
            // We keep results of the scenario here
            StringBuilder results = new StringBuilder();
            results.AppendLine("Found the following ids for \"" + scenarioString + "\":");

            // Iterate across each of the ids found in the string and add them to the results
            foreach (string id in FindIdsInString(scenarioString))
            {
                results.AppendLine(id);
            }

            // End of the scenario
            results.AppendLine();
            return results.ToString();
        }

        /// <summary>
        /// This is the click handler for the 'Display' button.  This runs through the scenarios in
        /// the sample.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Default_Click(object sender, RoutedEventArgs e)
        {
            // We keep results of the scenario here
            StringBuilder results = new StringBuilder();

            // We run through a couple of scenarios, including some with surrogate pairs
            results.Append(DoFindIdsInStringScenario("Hello, how are you?  I hope you are ok!"));
            results.Append(DoFindIdsInStringScenario("-->id<--"));
            results.Append(DoFindIdsInStringScenario("1id 2id 3id"));
            results.Append(DoFindIdsInStringScenario("id1 id2 id3"));
            results.Append(DoFindIdsInStringScenario("\uD840\uDC00_CJK_B_1 \uD840\uDC01_CJK_B_2 \uD840\uDC02_CJK_B_3"));

            rootPage.NotifyUser(results.ToString(), NotifyType.StatusMessage);
        }
    }
}
