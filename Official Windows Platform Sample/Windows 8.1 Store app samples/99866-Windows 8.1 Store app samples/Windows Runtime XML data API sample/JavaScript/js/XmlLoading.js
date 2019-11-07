//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/XmlLoading.html", {
        ready: function (element, options) {
            document.getElementById("scenario3BtnDefault").addEventListener("click", scenario3Load, false);

            scenario3Inialize();
        }
    });

    function scenario3Inialize() {
        loadXmlFile("loadExternaldtd", "xmlWithExternaldtd.xml", "scenario3");
    }

    function scenario3Load() {
        // get load settings
        var loadSettings = new Windows.Data.Xml.Dom.XmlLoadSettings;
        if (document.getElementById("scenario3Option1").checked === true)
        {
            loadSettings.prohibitDtd = true;          // DTD is prohibited
            loadSettings.resolveExternals = false;    // Disable the resolve to external definitions such as external DTD
        }
        if (document.getElementById("scenario3Option2").checked === true)
        {
            loadSettings.prohibitDtd = false;         // DTD is not prohibited
            loadSettings.resolveExternals = false;    // Disable the resolve to external definitions such as external DTD
        }
        if (document.getElementById("scenario3Option3").checked === true)
        {
            loadSettings.prohibitDtd = false;        // DTD is not prohibited
            loadSettings.resolveExternals = true;    // Enable the resolve to external definitions such as external DTD
        }
        
        // Load xml file with external DTD
        Windows.ApplicationModel.Package.current.installedLocation.getFolderAsync("loadExternaldtd").then(function (externalDtdFolder) {
            externalDtdFolder.getFileAsync("xmlWithExternaldtd.xml").done(function (file) {
                Windows.Data.Xml.Dom.XmlDocument.loadFromFileAsync(file, loadSettings).then(function (doc) {

                    document.getElementById("scenario3Result").value = "The file is loaded successfully.";
                    document.getElementById("scenario3Result").style.color = "black";

                }, function (error) {    // an exception is gotten in loading xml file

                    // After loadSettings.ProhibitDtd is set to true, the exception is expected as the sample XML contains DTD
                    document.getElementById("scenario3Result").value = "Error: DTD is prohibited.";
                    document.getElementById("scenario3Result").style.color = "red";

                });
            });
        });
    }
})();
