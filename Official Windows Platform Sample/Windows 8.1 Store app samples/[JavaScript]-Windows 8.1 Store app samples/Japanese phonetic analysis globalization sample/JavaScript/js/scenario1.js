//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario1.html", {
        ready: function (element, options) {
            document.getElementById("StartButton").addEventListener("click", startButton_Click, false);
        }
    });

    function startButton_Click() {
        WinJS.log && WinJS.log("Analyzing...", "sample", "status");
        var input = document.getElementById("InputTextBox").value;
        var output = "";

        // Split the Japanese text input in the text field into multiple words.
        // Without the second boolean parameter of getWords(), each element in the result corresponds to a single Japanese word.
        var words = Windows.Globalization.JapanesePhoneticAnalyzer.getWords(input);
        for (var i = 0; i < words.length; i++) {
            var word = words[i];

            // isPhraseStart indicates whether the word is the first word of a segment or not.
            if (output !== "" && word.isPhraseStart) {
                // Output a delimiter before each segment.
                output += "/";
            }

            // displayText is the display text of the word, which has same characters as the input of getWords().
            // yomiText is the reading text of the word, as known as Yomi, which basically consists of Hiragana characters.
            // However, please note that the reading can contains some non-Hiragana characters for some display texts such as emoticons or symbols.
            output += word.displayText + "(" + word.yomiText + ")";
        }

        // Display the result.
        document.getElementById("OutputTextBox").value = output;
        if (input !== "" && output === "") {
            // If the result from getWords() is empty but the input is not empty,
            // it means the given input is too long to analyze.
            WinJS.log && WinJS.log("Failed to get words from the input text.  The input text is too long to analyze.", "sample", "error");
        } else {
            // Otherwise, the analysis has done successfully.
            WinJS.log && WinJS.log("Got words from the input text successfully.", "sample", "status");
        }
    }
})();
