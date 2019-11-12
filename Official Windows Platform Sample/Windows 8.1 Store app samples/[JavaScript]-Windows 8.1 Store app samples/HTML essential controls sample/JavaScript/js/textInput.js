//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/textInput.html", {
        ready: function (element, options) {
            //check which stylesheet is currently loaded
            initStyleSheetRadioButton();
        }
    });

})();

function makeBold() {
    // make the selected text bold
    document.execCommand('bold', false, null);
}

function spellCheckChange() {
    document.getElementById("spellCheckText").spellcheck = document.getElementById("spellCheckToggle").winControl.checked;
}

// To protect against untrusted code execution, all functions are required to be marked as supported for processing before they can be used inside a data-win-options attribute in HTML markup.
WinJS.Utilities.markSupportedForProcessing(spellCheckChange);

function reply() {
    var inputBox = document.getElementById("spellCheckText");

    if (!inputBox.readOnly) {
        return;     // already in reply mode
    }

    // a readonly text input control will not be spellchecked since it's not editable. It's good for displaying original email. Here readonly is turned off for reply.
    inputBox.readOnly = false;

    // temporarily turn off spellcheck, so that the following content edit will not be spellchecked.
    inputBox.spellcheck = false;
    inputBox.value = "\n\n-----Original Message-----\n" + inputBox.value;

    // turn back to user setting.
    inputBox.spellcheck = document.getElementById("spellCheckToggle").winControl.checked;
}
