//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/transitionContents.html", {
        ready: function (element, options) {
            runAnimation.addEventListener("click", transitionBetweenContent, false);
            output1.style.display = "block";
            output2.style.display = "none";
        }
    });

    function transitionBetweenContent() {
        var incoming;
        var outgoing;

        // Assign incoming and outgoing
        if (output2.style.display === "none") {
            incoming = output2;
            outgoing = output1;
        } else {
            incoming = output1;
            outgoing = output2;
        }

        // Run the exitContent animation and then the enterContent animation
        // Use the recommended offset by leaving the offset argument empty to get the best performance
        WinJS.UI.Animation.exitContent(output, null).done( function () {
            outgoing.style.display = "none";
            incoming.style.display = "block";
            return WinJS.UI.Animation.enterContent(output, null);
        });
    }
})();
