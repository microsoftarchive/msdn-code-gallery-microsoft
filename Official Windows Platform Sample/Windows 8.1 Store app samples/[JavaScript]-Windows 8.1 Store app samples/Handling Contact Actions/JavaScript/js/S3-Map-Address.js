//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/S3-Map-Address.html", {
        processed: function (element, mapArgs) {
            // mapArgs is the parameter passed to navigation in the activated event handler.
            if (mapArgs) {
                var address = mapArgs.address;
                WinJS.log && WinJS.log("Map address activation was received. The street address to map is " +
                    (address.streetAddress ? address.streetAddress : "unspecified") + ".", "sample", "status");
            }
        },
        ready: function (element, options) {
        }
    });
})();
