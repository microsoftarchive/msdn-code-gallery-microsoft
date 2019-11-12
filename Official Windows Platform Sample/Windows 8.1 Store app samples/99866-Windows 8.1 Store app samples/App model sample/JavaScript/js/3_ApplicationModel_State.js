//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="//Microsoft.WinJS.2.0/js/base.js" />

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/3_ApplicationModel_State.html", {
        ready: function (element, options) {
            this.manually = false;
            this.stateFile = "state";

            // Set up this pointer for saveState method.
            // We need to save this so we can unbind it
            // when we navigate away from the page.

            this.saveState = this.saveState.bind(this);

            //
            // Hook up to checkpoint event - event will
            // get fired when application needs to save
            // state.
            //
            WinJS.Application.addEventListener("checkpoint",
                this.saveState);

            this.element.querySelector("#saveStateButton")
                .addEventListener("click", this.fireCheckpointEvent.bind(this));
            this.element.querySelector("#retrieveStateButton")
                .addEventListener("click", this.getState.bind(this));

            this.checkpointOutput = element.querySelector("#checkpointOutput");
            this.stateValue = element.querySelector("#stateValue");
        },

        //
        // This method causes the checkpoint event to be raised
        // manually.
        //

        fireCheckpointEvent: function () {
            this.manually = true;

            // Trigger checkpoint
            WinJS.Application.checkpoint();
        },

        //
        // Event handler for the Retrieve State button. Load our state
        // from the file.
        //
        getState: function () {
            var that = this;
            WinJS.Application.local.exists(this.stateFile)
                .then(function (exists) {
                    if (exists) {
                        return WinJS.Application.local.readText(that.stateFile);
                    }
                    return "";
                }).done(function (str) {
                    if (str) {
                        that.checkpointOutput.innerText =
                            "appState = { x : " + JSON.parse(str).x + " }";
                    } else {
                        that.checkpointOutput.innerText = "No saved state yet";
                    }
                });
        },

        //
        // Event handler for checkpoint - pull the user's data out of the
        // UI and write it out to as a file, as an example.
        //
        saveState: function () {
            var that = this;
            var appState = { x: this.stateValue.value };
            var str = this.manually ? "manually" : "automatically";

            this.manually = false;
            WinJS.Application.local.writeText(this.stateFile, JSON.stringify(appState))
                .done(function () {
                    that.checkpointOutput.innerText = "state is saved to local folder " + str;
                });
        },

        unload: function () {
            WinJS.Application.removeEventListener("checkpoint", this.saveState);
        }
    });
})();
