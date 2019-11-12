//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/Scenario4.html", {
        ready: function (element, options) {
            document.getElementById("Scenario4Launch").addEventListener("click", deleteAllCreds, false);
        }
    });




    function deleteAllCreds() {
        try {
            var vault = new Windows.Security.Credentials.PasswordVault();
            var creds = vault.retrieveAll();
            var num = vault.retrieveAll().size;

            for (var i = 0; i < creds.size; i++) {
                try {
                    vault.remove(creds.getAt(i));
                }
                catch (e) {
                }
            }

            document.getElementById("Scenario4Status").value = "Number of credentials removed: " + num;
        }
        catch (e) {
            document.getElementById("Scenario4Status").value = e.description;
        }
    }



})();
