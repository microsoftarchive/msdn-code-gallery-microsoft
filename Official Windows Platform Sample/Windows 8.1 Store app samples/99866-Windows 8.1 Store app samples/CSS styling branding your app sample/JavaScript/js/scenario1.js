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
            // Sets event handlers for buttons which enable different stylesheets

            document.getElementById("defaultStyleButton").addEventListener("click", function (mouseEvent) { loadStyleSample("defaultStyleSheet"); }, false);
            document.getElementById("uiDarkButton").addEventListener("click", function (mouseEvent) { loadStyleSample("uiDarkStyleSheet"); }, false);
            document.getElementById("uiLightButton").addEventListener("click", function (mouseEvent) { loadStyleSample("uiLightStyleSheet"); }, false);
            document.getElementById("desertStyleButton").addEventListener("click", function (mouseEvent) { loadStyleSample("desertStyleSheet"); }, false);
            document.getElementById("oceanStyleButton").addEventListener("click", function (mouseEvent) { loadStyleSample("oceanStyleSheet"); }, false);
        }
    });

    // An object that enumerates the possible style sheets a user can activate.
    var styles = {
        "defaultStyleSheet": {
            status: "Using Style Sheets: none",
            uiStyleSheet: null,
            additionalStyleSheet: null
        },
        "uiDarkStyleSheet": {
            status: "Using Style Sheets: ui-dark.css",
            uiStyleSheet: "//Microsoft.WinJS.2.0/css/ui-dark.css",
            additionalStyleSheet: null
        },
        "uiLightStyleSheet": {
            status: "Using Style Sheets: ui-light.css",
            uiStyleSheet: "//Microsoft.WinJS.2.0/css/ui-light.css",
            additionalStyleSheet: null
        },
        "desertStyleSheet": {
            status: "Using Style Sheets: ui-light.css, desertStyle.css",
            uiStyleSheet: "//Microsoft.WinJS.2.0/css/ui-light.css",
            additionalStyleSheet: "/css/desertStyle.css"
        },
        "oceanStyleSheet": {
            status: "Using Style Sheets: ui-light.css, oceanStyle.css",
            uiStyleSheet: "//Microsoft.WinJS.2.0/css/ui-light.css",
            additionalStyleSheet: "/css/oceanStyle.css"
        }
    };

    // Load a style sheet into the output IFrame
    function loadStyleSample(styleId) {

        var iframe = document.getElementById("outputIframe");
        var uiStyleSheet = iframe.contentDocument.getElementById("ui-stylesheet");
        var additionalStyleSheet = iframe.contentDocument.getElementById("additional-stylesheet");

        if (styles[styleId].uiStyleSheet) {
            uiStyleSheet.setAttribute("href", styles[styleId].uiStyleSheet);
            uiStyleSheet.disabled = false;
        } else {
            uiStyleSheet.disabled = true;
        }

        if (styles[styleId].additionalStyleSheet) {
            additionalStyleSheet.setAttribute("href", styles[styleId].additionalStyleSheet);
            additionalStyleSheet.disabled = false;
        } else {
            additionalStyleSheet.disabled = true;
        }

        WinJS.log && WinJS.log(styles[styleId].status, "sample", "status");
    }

    function onScenarioChanged() {
        WinJS.log && WinJS.log("", "sample", "status");
    }
})();
