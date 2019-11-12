//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="//Microsoft.WinJS.0.6/js/base.js" />


(function () {
    "use strict";

    // Notice: this scenario is itself a page control.
    var page = WinJS.UI.Pages.define("/html/renderingPageControls.html", {
        ready: function (element, options) {

            // Render the page control via a call to WinJS.UI.Pages.render. This lets
            // you render a page control by referencing it via url.
            var renderHost = element.querySelector(".renderingPageControls-renderedControl");
            WinJS.UI.Pages.render("/pages/SamplePageControl.html", renderHost, {
                controlText: "This control created by calling WinJS.UI.Pages.render",
                message: "Render control"
            }).done();

            // Render the page control by creating the control.

            var constructedHost = element.querySelector(".renderingPageControls-createdProgrammatically");
            new Controls_PageControls.SamplePageControl(constructedHost, {
                controlText: "This control created by calling the constructor directly",
                message: "Constructed control"
            });

            // The other two page controls were created via declarative mechanisms.
            // See /pages/samplePageControl.js for the implementation of the page control
            // that's being loaded here.
        }
    });



})();
