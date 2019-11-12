//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/receive-file.html", {
        processed: function (element, files) {
            // During an initial activation this event is called before the system splash screen is torn down.
            // Files is the parameter passed to navigation in the activated event handler.
            if (files) {
                WinJS.log && WinJS.log("File activation received. The number of files received is " + files.size + ". The first received file is " + files[0].name + ".", "sample", "status");
            }
        },

        ready: function (element, options) {
            // During an initial activation this event is called after the system splash screen is torn down.
            // Do any initialization work that is not related to getting the initial UI set up.
        }
    });
})();
