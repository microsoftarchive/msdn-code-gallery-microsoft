//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/directory.html", {
        ready: function (element, options) {
            document.getElementById("directoryGetInstalledLocation").addEventListener("click", directoryGetInstalledLocation, false);
            directoryDisplayOutput();
        }
    });

    function directoryGetInstalledLocation() {
        // This will give the Installed Location.  You can use this location to access files, if you need them.
        var pkg = Windows.ApplicationModel.Package.current;
        var installedLocation = pkg.installedLocation;

        var html = "Installed Location: " + installedLocation.path;

        directoryDisplayOutput(html);
    }

    function directoryDisplayOutput(output) {
        output = output || "";

        document.getElementById("directoryOutput").innerHTML = output;
    }
})();
