//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/addPublicLocalUri.html", {
        ready: function (element, options) {
            document.getElementById("addPublicLocalUri").addEventListener("click", addPublicLocalUri, false);
        }
    });

    function addPublicLocalUri() {
        MSApp.addPublicLocalApplicationUri("callWinRT.html");
    }

})();
