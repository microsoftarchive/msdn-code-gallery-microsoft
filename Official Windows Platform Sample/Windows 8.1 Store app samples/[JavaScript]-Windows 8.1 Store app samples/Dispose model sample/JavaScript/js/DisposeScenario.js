// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF 
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
// PARTICULAR PURPOSE. 
// 
// Copyright (c) Microsoft Corporation. All rights reserved 

(function () {
    "use strict";

    var pageLocation = "/html/DisposeScenario.html";
    var _createControl = true;

    WinJS.UI.Pages.define(pageLocation, {
        ready: function (element, options) {
            document.getElementById("createControl").addEventListener("click", createControlHandler, false);
            WinJS.Navigation.addEventListener("beforenavigate", beforeNavigate);
        }
    });

    function beforeNavigate(eventInfo) {
        // if we're navigating away from this page to another page, resume the paused task
        if (WinJS.Navigation.location === pageLocation) {
            if (!_createControl) {
                disposeControl();
                _createControl = true;
            }
        }
    }

    function createControlHandler() {
        var createControlButton = document.getElementById("createControl");
        var ageControl;

        if (_createControl) {
            WinJS.clearLog && WinJS.clearLog();

            window.output("Creating Age Calculator control...", "sample", "status");

            createControlButton.textContent = "Dispose Age Calculator control";
            ageControl = new WinJS.Samples.AgeControlWithDispose(document.getElementById("ageControlContainer"));
            _createControl = false;
        }
        else {
            WinJS.clearLog && WinJS.clearLog();
            createControlButton.textContent = "Create Age Calculator control";

            window.output("Calling dispose on Age Calculator control...", "sample", "status");
            disposeControl();
            _createControl = true;
        }

    }

    function disposeControl() {
        var ageControlContainer = document.getElementById("ageControlContainer");
        var ageControl = ageControlContainer.winControl;
        ageControl.dispose();

        ageControlContainer.innerHTML = "";
    }
})();
