////
//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF 
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
//// PARTICULAR PURPOSE. 
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/FindId.html", {
        ready: function (element, options) {
            document.getElementById("displayButton").addEventListener("click", doDisplay, false);
        }
    });

    // This is a helper method that an app could create to find one or all available 
    // ids within a string.  It takes as an input a string that contains one or more
    // ids.  It returns an array of individual ids found in the input string.
    function findIdsInString(inputString) {

        // List where we maintain the ids found in the input string
        var idList = new Array();

        // Maintains the beginning index of the id found in the input string
        var indexIdStart = -1;

        // Iterate through each of the characters in the string
        for (var i = 0; i < inputString.length; i++)  {

            var codepoint = inputString.charCodeAt(i);         
                
            // If the character is a high surrogate, then we need to read the next character to make
            // sure it is a low surrogate.  If we are at the last character in the input string, then
            // we have an error, since a high surrogate must be matched by a low surrogate.  Update
            // the code point with the surrogate pair.
            if (Windows.Data.Text.UnicodeCharacters.isHighSurrogate(codepoint)) {
                    
                if (++i >= inputString.Length) {
                    throw "Bad trailing surrogate at end of string";
                }

                codepoint = Windows.Data.Text.UnicodeCharacters.getCodepointFromSurrogatePair(inputString.charCodeAt(i - 1), inputString.charCodeAt(i));
            }
                
            // Have we found an id start?
            if (indexIdStart === -1) {
                if (Windows.Data.Text.UnicodeCharacters.isIdStart(codepoint)) {

                    // We found a character that is an id start.  In case we had a suplemmentary
                    // character (high and low surrogate), then the index needs to offset by 1.
                    indexIdStart = Windows.Data.Text.UnicodeCharacters.isSupplementary(codepoint) ?  i - 1 : i;             
                }         
            }         
            else if (!Windows.Data.Text.UnicodeCharacters.isIdContinue(codepoint)) {             

                // We have not found an id continue, so the id is complete.  We need to 
                // create the identifier string
                idList.push(inputString.substring(indexIdStart, i));
                    
                // Reset back the index start and re-examine the current code point 
                // in next iteration
                indexIdStart = -1;
                i--;
            }     
        }

        // Do we have a pending id at the end of the string?
        if (indexIdStart !== -1) {

            //  We need to create the identifier string
            idList.push(inputString.substring(indexIdStart));
        }

        // Return the list of identifiers found in the string
        return idList;
    }

    /// This method implements the scenario of finding all ids within a string.  It relies on
    /// the findIdsInString method to implement the logic.  This just takes care of finding and
    /// logging the ids in the scenario window.  Takes a string that contains one or more ids
    /// as an input, and returns a string that contains the output of the scenario to display
    function doFindIdsInStringScenario(scenarioString) {

        // We keep results of the scenario here
        var results = "Found the following ids for \"" + scenarioString + "\":\n";

        // Iterate across each of the ids found in the string and add them to the results
        var idsInString = findIdsInString(scenarioString);
        for (var idIndex in idsInString) {
            results += idsInString[idIndex] + "\n";
        }

        // End of the scenario
        results += "\n";
        return results;
    }

    /// This is the click handler for the 'Display' button.  This runs through the scenarios in
    /// the sample.
    function doDisplay() {

        // We keep results of the scenario here
        var results = "";

        // We run through a couple of scenarios, including some with surrogate pairs
        results += doFindIdsInStringScenario("Hello, how are you?  I hope you are ok!");
        results += doFindIdsInStringScenario("-->id<--");
        results += doFindIdsInStringScenario("1id 2id 3id");
        results += doFindIdsInStringScenario("id1 id2 id3");
        results += doFindIdsInStringScenario("\uD840\uDC00_CJK_B_1 \uD840\uDC01_CJK_B_2 \uD840\uDC02_CJK_B_3");

        WinJS.log && WinJS.log(results, "sample", "status");
    }
})();
