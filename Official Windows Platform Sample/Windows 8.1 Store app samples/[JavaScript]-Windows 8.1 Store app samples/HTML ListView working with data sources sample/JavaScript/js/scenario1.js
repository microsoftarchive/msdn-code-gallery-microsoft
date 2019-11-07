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
            WinJS.Utilities.query("#savekey").listen("click", saveKey);
            WinJS.Utilities.query("#runquery").listen("click", doSearch);
            loadKey();
        }
    });

    // Save the dev key in app state and toggle the control enablement
    function saveKey() {
        var devkey = document.getElementById("devkey").value;
        if (devkey !== "" && devkey.length > 12) {
            try {
                var localSettings = Windows.Storage.ApplicationData.current.localSettings;
                localSettings.values["devkey"] = devkey;
            }
            catch (err) {
                //do nothing;
            }
            toggleControls(true);
            doSearch();

        } else {
            toggleControls(false);
        }
    }

    // load the key from app state
    function loadKey() {
        var devkey = "";

        try {
            var localSettings = Windows.Storage.ApplicationData.current.localSettings;
            devkey = localSettings.values["devkey"];
            if (devkey === null || devkey === undefined) {
                devkey = "";
            }
        }
        catch (err) {
            devkey = "";
        }

        if (devkey !== "" && devkey.length > 12) {
            document.getElementById("devkey").value = devkey;
            toggleControls(true);

        } else {
            toggleControls(false);
        }

    }

    // Toggles the enablement of the controls when we have a dev key
    function toggleControls(enabled) {
        document.getElementById("queryCtrls").disabled = (!enabled);
        if (enabled) {
            document.getElementById("savekey").className = "action secondary";
            document.getElementById("runquery").className = "action";

        } else {
            document.getElementById("savekey").className = "action";
            document.getElementById("runquery").className = "action secondary";
        }
    }

    // Initializes the data adapter and pass to the listview
    // Called when the search button is pressed.
    // The code for the data adapter is in js/BingImageSearchDataSource.js
    function doSearch() {
        var devkey = document.getElementById("devkey").value;
        if (devkey !== "" && devkey.length > 12) {
            var searchTerm = document.getElementById("query").value;

            var listview = document.getElementById("listview1").winControl;
            var myTemplate = document.getElementById("itemTemplate");

            //Create the bing itemDataSource
            var myDataSrc = new bingImageSearchDataSource.datasource(devkey, searchTerm);

            // Set the properties on the list view to use the itemDataSource
            listview.itemDataSource = myDataSrc;
            listview.itemTemplate = myTemplate;
        }
    }

})();
