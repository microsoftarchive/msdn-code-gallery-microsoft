//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="..\base-sdk.js" />
/// <reference path="..\WinJS\js\base.js" />

(function () {
    "use strict";

    function showCicle() {
        WinJS.Utilities.query("#schemaHandlerVideo")[0].src = "myscheme://circle";
    }

    function showSquare() {
        WinJS.Utilities.query("#schemaHandlerVideo")[0].src = "myscheme://square";
    }

    function showTriangle() {
        WinJS.Utilities.query("#schemaHandlerVideo")[0].src = "myscheme://triangle";
    }

    function schemaHandlerStop() {
        var vid = WinJS.Utilities.query("#schemaHandlerVideo")[0];
        vid.suppressErrors = true;
        vid.src = null;
    }

    var page = WinJS.UI.Pages.define("/html/SchemaHandler.html", {
        extensions: null,

        ready: function (element, options) {
            if (!this.extensions) {
                this.extensions = new Windows.Media.MediaExtensionManager();
                // Register custom scheme handler
                this.extensions.registerSchemeHandler("GeometricSource.GeometricSchemeHandler", "myscheme:");
            }

            WinJS.Utilities.query("#showCircle").listen("click", showCicle);
            WinJS.Utilities.query("#showSquare").listen("click", showSquare);
            WinJS.Utilities.query("#showTriangle").listen("click", showTriangle);
            WinJS.Utilities.query("#schemaHandlerVideo").listen("error", SdkSample.videoOnError);
        },

        unload: function () {
            schemaHandlerStop();
        }
    });
})();
