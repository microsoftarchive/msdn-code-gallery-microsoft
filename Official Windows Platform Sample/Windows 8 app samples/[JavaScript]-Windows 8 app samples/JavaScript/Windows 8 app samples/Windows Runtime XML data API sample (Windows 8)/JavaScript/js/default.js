//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "Xml in Windows Runtime";

    var scenarios = [
        { url: "/html/BuildNewRss.html", title: "Build New RSS" },
        { url: "/html/MarkHotProducts.html", title: "DOM Load/Save" },
        { url: "/html/XmlLoading.html", title: "Set Load Settings" },
        { url: "/html/GiftDispatch.html", title: "XPath Query" },
        { url: "/html/XSLTTransformation.html", title: "XSLT Transformation" },
    ];

    function activated(eventObject) {
        if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
            // Use setPromise to indicate to the system that the splash screen must not be torn down
            // until after processAll and navigate complete asynchronously.
            eventObject.setPromise(WinJS.UI.processAll().then(function () {
                // Navigate to either the first scenario or to the last running scenario
                // before suspension or termination.
                var url = WinJS.Application.sessionState.lastUrl || scenarios[0].url;
                return WinJS.Navigation.navigate(url);
            }));
        }
    }

    WinJS.Navigation.addEventListener("navigated", function (eventObject) {
        var url = eventObject.detail.location;
        var host = document.getElementById("contentHost");
        // Call unload method on current scenario, if there is one
        host.winControl && host.winControl.unload && host.winControl.unload();
        WinJS.Utilities.empty(host);
        eventObject.detail.setPromise(WinJS.UI.Pages.render(url, host, eventObject.detail.state).then(function () {
            WinJS.Application.sessionState.lastUrl = url;
        }));
    });

    WinJS.Namespace.define("SdkSample", {
        sampleTitle: sampleTitle,
        scenarios: scenarios
    });

    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();
})();

function loadXmlFile(foldername, filename, scenarioNumber) {
    var defaultBtn = document.getElementById(scenarioNumber + "BtnDefault");
    var orginalData = document.getElementById(scenarioNumber + "OriginalData");
    var output = document.getElementById(scenarioNumber + "Result");

    Windows.ApplicationModel.Package.current.installedLocation.getFolderAsync(foldername).then(function (folder) {
        folder.getFileAsync(filename).done(function (file) {
            var loadSettings = new Windows.Data.Xml.Dom.XmlLoadSettings;
            loadSettings.prohibitDtd = false;
            loadSettings.resolveExternals = false;
            Windows.Data.Xml.Dom.XmlDocument.loadFromFileAsync(file, loadSettings).then(function (doc) {
                orginalData.value = doc.getXml();
                defaultBtn.disabled = false;
            }, function (error) {
                output.value = "Error: Unable to load XML file";
                output.style.color = "red";
            });
        }, function (error) {
            output.value = error.description;
            output.style.color = "red";
        });
    }, function (error) {
        output.value = error.description;
        output.style.color = "red";
    });
};
