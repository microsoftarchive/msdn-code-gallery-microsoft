//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/button.html", {
        ready: function (element, options) {
            //check which stylesheet is currently loaded
            initStyleSheetRadioButton();
        }
    });

})();

function sendMessage() {
    var data = document.getElementById("message").value;

    // put your code here to send out message. Recommend using AJAX to avoid page switch.

    document.getElementById("sendMessageResult").innerText = "Your message has been sent: " + data;
}

function calculate1Plus1() {
    var result = 1 + 1;
    document.getElementById("calculate1Plus1Result").innerText = "The result is " + result + ".";
}
