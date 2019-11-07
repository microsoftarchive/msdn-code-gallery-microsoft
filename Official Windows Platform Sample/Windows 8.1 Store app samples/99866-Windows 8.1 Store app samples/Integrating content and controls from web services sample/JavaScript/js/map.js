//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    var bingAppId = "";

    window.addEventListener("message", receiveMessage, false);

    function receiveMessage(e) {
        if (e.origin === "ms-appx://" + document.location.host) {
            bingAppId = e.data;
            initialize();
        }
    }

    function initialize() {
        var mapOptions = {
            credentials: bingAppId,
            center: new Microsoft.Maps.Location(47.733762, -122.146974),
            mapTypeId: Microsoft.Maps.MapTypeId.road,
            zoom: 7,
            showLogo: false
        };
        return new Microsoft.Maps.Map(document.getElementById("map"), mapOptions);
    }

    document.addEventListener("DOMContentLoaded", initialize, false);
})();
