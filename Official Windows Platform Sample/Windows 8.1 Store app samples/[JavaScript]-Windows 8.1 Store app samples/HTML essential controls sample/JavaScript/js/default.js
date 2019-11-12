//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "Essential controls sample";

    var scenarios = [
        { url: "/html/range-intro.html", title: "Range - Introduction" },
        { url: "/html/range-seekBar.html", title: "Range - Video Seek Bar Example" },
        { url: "/html/range-equalizer.html", title: "Range - Multi-touch Equalizer Example" },
        { url: "/html/range-bingMaps.html", title: "Range - Bing Maps Zoom Level Control Example" },
        { url: "/html/progress-intro.html", title: "Progress - Introduction" },
        { url: "/html/progress-download.html", title: "Progress - File Download Example" },
        { url: "/html/button.html", title: "Button" },
        { url: "/html/checkbox.html", title: "Checkbox" },
        { url: "/html/radioButton.html", title: "Radio Button" },
        { url: "/html/textInput.html", title: "Text Input Control" },
        { url: "/html/select.html", title: "Select Control" },
        { url: "/html/fileUpload.html", title: "File Upload Control" },
        { url: "/html/formLayout.html", title: "Form Layout" },
        { url: "/html/darkLightStylesheets.html", title: "Dark and Light Stylesheets" }
    ];

    function activated(eventObject) {
        if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
            // many styling samples in this application have two styles, one for light background and one for dark background. here is to initialize them in light style since the page background will be in white color when loaded.
            WinJS.Utilities.removeClass(document.body, "ui-dark");
            WinJS.Utilities.addClass(document.body, "ui-light");

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

    WinJS.Navigation.addEventListener("navigated", function (evt) {
        var url = evt.detail.location;
        var host = document.getElementById("contentHost");
        // Call unload method on current scenario, if there is one
        host.winControl && host.winControl.unload && host.winControl.unload();
        WinJS.Utilities.empty(host);
        WinJS.UI.Pages.render(url, host).then(function () {
            WinJS.Application.sessionState.lastUrl = url;
        });
    });

    WinJS.Namespace.define("SdkSample", {
        sampleTitle: sampleTitle,
        scenarios: scenarios
    });

    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();
})();

function switchStyleDark() {
    var uiLightString = "ui-light.css";
    var uiDarkString = "ui-dark.css";
    for (var i = 0; i < document.styleSheets.length; i++) {
        if (document.styleSheets[i].href &&
            document.styleSheets[i].href.lastIndexOf(uiDarkString) === (document.styleSheets[i].href.length - uiDarkString.length)) {
            document.styleSheets[i].disabled = false;
        }
        if (document.styleSheets[i].href &&
            document.styleSheets[i].href.lastIndexOf(uiLightString) === (document.styleSheets[i].href.length - uiLightString.length)) {
            document.styleSheets[i].disabled = true;
        }
    }

    // switch the class on body tag as a master selector for dark/light versions of styling examples
    WinJS.Utilities.removeClass(document.body, "ui-light");
    WinJS.Utilities.addClass(document.body, "ui-dark");
}

function switchStyleLight() {
    var uiLightString = "ui-light.css";
    var uiDarkString = "ui-dark.css";
    for (var i = 0; i < document.styleSheets.length; i++) {
        if (document.styleSheets[i].href &&
            document.styleSheets[i].href.lastIndexOf(uiDarkString) === (document.styleSheets[i].href.length - uiDarkString.length)) {
            document.styleSheets[i].disabled = true;
        }
        if (document.styleSheets[i].href &&
            document.styleSheets[i].href.lastIndexOf(uiLightString) === (document.styleSheets[i].href.length - uiLightString.length)) {
            document.styleSheets[i].disabled = false;
        }
    }

    WinJS.Utilities.removeClass(document.body, "ui-dark");
    WinJS.Utilities.addClass(document.body, "ui-light");
}

function initStyleSheetRadioButton() {
    var uiLightString = "ui-light.css";
    var uiDarkString = "ui-dark.css";
    for (var i = 0; i < document.styleSheets.length; i++) {
        if (document.styleSheets[i].href &&
            document.styleSheets[i].href.lastIndexOf(uiDarkString) === (document.styleSheets[i].href.length - uiDarkString.length) &&
            document.styleSheets[i].disabled === false) {
            document.getElementById("darkStyle").checked = true;
            document.getElementById("lightStyle").checked = false;
        }
        else if (document.styleSheets[i].href &&
            document.styleSheets[i].href.lastIndexOf(uiLightString) === (document.styleSheets[i].href.length - uiLightString.length) &&
            document.styleSheets[i].disabled === false) {
            document.getElementById("darkStyle").checked = false;
            document.getElementById("lightStyle").checked = true;
        }
    }
}
