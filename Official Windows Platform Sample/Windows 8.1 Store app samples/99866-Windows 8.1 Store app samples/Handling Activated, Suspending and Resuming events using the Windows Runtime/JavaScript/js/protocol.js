//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/protocol.html", {
        processed: function (element, eventObject) {
            // During an initial activation this event is called before activation completes.
            // Do any initialization work that is required for the initial UI to be complete.
            var output = "";
            if (eventObject) {
                // Protocol format currently supported in this sample is: sdksampleprotocol:domain?src=[some url]
                output = "This is Protocol activation.";
                output += "<br/>Protocol format used for this activation: " + eventObject.uri.rawUri + "<br/>";
            } else {
                output = "To activate this application using protocol activation find \"Run\" and then type the following \"sdksampleprotocol:domain?src=[some url]\". The output will be displayed here.";
            }
            document.getElementById("protocolOutput").innerHTML = output;
        },

        ready: function (element, eventObject) {
            // During an initial activation this event is called after activation has completed.
            // Do any initialization work that is not related to getting the initial UI set up.
        }
    });
})();
