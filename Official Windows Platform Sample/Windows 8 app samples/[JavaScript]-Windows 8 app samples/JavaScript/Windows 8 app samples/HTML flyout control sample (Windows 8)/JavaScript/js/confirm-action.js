//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    // track whether the item was bought or not
    var bought;

    var page = WinJS.UI.Pages.define("/html/confirm-action.html", {
        ready: function (element, options) {
            document.getElementById("buyButton").addEventListener("click", showConfirmFlyout, false);
            document.getElementById("confirmButton").addEventListener("click", confirmOrder, false);
            document.getElementById("confirmFlyout").addEventListener("afterhide", onDismiss, false);
        }

                
    });

    //Show the flyout
    function showConfirmFlyout() {
        bought = false;
        WinJS.log && WinJS.log("", "sample", "status");

        var buyButton = document.getElementById("buyButton");
        document.getElementById("confirmFlyout").winControl.show(buyButton);
    }

    // When the Buy button is pressed, hide the flyout since the user is done with it.
    function confirmOrder() {
        bought = true;
        WinJS.log && WinJS.log("You have completed your purchase.", "sample", "status");
        document.getElementById("confirmFlyout").winControl.hide();
    }

    // On dismiss of the flyout, determine if it closed because the user pressed the buy button. If not, then the
    // flyout was light dismissed.
    function onDismiss() {
        if (!bought) {
            WinJS.log && WinJS.log("The purchase was not completed.", "sample", "status");
        }
    }

})();
