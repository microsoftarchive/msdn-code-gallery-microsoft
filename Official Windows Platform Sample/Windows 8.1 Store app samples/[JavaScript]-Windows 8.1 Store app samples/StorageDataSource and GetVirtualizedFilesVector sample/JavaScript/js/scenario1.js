//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/scenario1.html", {
        ready: function (element, options) {
            document.getElementById("pictures-datasource-Launch").addEventListener("click", loadView, false);
        }
    });

    function loadView() {
        var mode = document.getElementById("mode").options[document.getElementById("mode").selectedIndex].value;
        // Navigate to the listview page to view the Pictures Library in the Listview
        if (mode === "programmatic") {
            WinJS.Navigation.navigate("/html/scenario1ProgrammaticListview.html");
        } else {
            WinJS.Navigation.navigate("/html/scenario1DeclarativeListview.html");
        }
    }
})();
