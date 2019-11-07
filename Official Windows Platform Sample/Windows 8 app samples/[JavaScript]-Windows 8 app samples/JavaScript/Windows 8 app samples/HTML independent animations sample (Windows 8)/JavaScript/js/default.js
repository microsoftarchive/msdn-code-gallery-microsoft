//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved
var resetelement = {};
var eventcount = 0;

var scenarioTransformValues = {};
var scenarioDurationValues = {};
var scenarioDelayValues = {};
var scenarioTimFunValues = {};
var scenarioTransOrig = {};
var scenarioKeysValues = {};
var scenarioKeyfrom = {};
var scenarioKey50 = {};
var scenarioKeyto = {};

var animation2name = {};
var animation2duration = {};
var animations2delay = {};
var animation2KeysValues = {};

var g_tab = '&nbsp; &nbsp; &nbsp;';
var d_tab = '&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;';
var t_tab = '&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;';

(function () {
    "use strict";

    var sampleTitle = "Independent Animations Sample";

    var scenarios = [
        { url: "/html/scenario1.html", title: "Independent versus Dependent Animations" },
        { url: "/html/scenario2.html", title: "Transitioning 2D Transforms & Opacity" },
        { url: "/html/scenario3.html", title: "Transitioning 3D Transforms & Opacity" },
        { url: "/html/scenario4.html", title: "Transition Events" },
        { url: "/html/scenario5.html", title: "Animation of 2D Transforms & Opacity" },
        { url: "/html/scenario6.html", title: "Animation of 3D Transforms & Opacity" },
        { url: "/html/scenario7.html", title: "Animation Events" }
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
