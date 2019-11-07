// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF 
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
// PARTICULAR PURPOSE. 
// 
// Copyright (c) Microsoft Corporation. All rights reserved 

(function () {
    "use strict";

    WinJS.Namespace.define("WinJS.Samples", {
        AgeControlWithDispose: WinJS.Class.define(
            function (element) {
                this._element = element || document.createElement("div");
                this._element.winControl = this;

                this._disposed = false;
                this._submitButton;
                this._submitListener;
                this._datePicker;
                this._resultDiv;
                this._globalKeyUpListener;

                WinJS.Utilities.addClass(this._element, "win-disposable");

                // create all sub-controls
                this._createSubControls();

                // connect events to our handlers
                this._wireupEvents();
            },
            {
                element: {
                    get: function () {
                        return this._element;
                    }
                },
                _createSubControls: function () {
                    window.output("Creation: Creating sub controls", "sample", "status");

                    var containerDiv = document.createElement("div");
                    WinJS.Utilities.addClass(containerDiv, "ageControl");
                    this._element.appendChild(containerDiv);

                    // create the label
                    var label = document.createElement("div");
                    label.textContent = "Enter your birthday, then click 'Calculate!' or 'alt+c' to calculate your age in days.";
                    containerDiv.appendChild(label);

                    // create the birthday datepicker
                    var datePickerDiv = document.createElement("div");
                    this._datePicker = new WinJS.UI.DatePicker(datePickerDiv);
                    this._datePicker.current = new Date(1990, 0, 1);
                    containerDiv.appendChild(datePickerDiv);

                    // create a submit button inside a div
                    var submitContainer = document.createElement("div");
                    containerDiv.appendChild(submitContainer);
                    this._submitButton = document.createElement("button");
                    this._submitButton.textContent = "Calculate!";
                    submitContainer.appendChild(this._submitButton);

                    // create the result text div
                    this._resultDiv = document.createElement("div");
                    containerDiv.appendChild(this._resultDiv);
                },
                _wireupEvents: function () {
                    // connect the submit button click event to the handler
                    window.output("Creation: Connecting submit button event handler", "sample", "status");
                    this._submitListener = this._handleSubmitClick.bind(this);
                    this._submitButton.addEventListener("click", this._submitListener, false);

                    // connect event handlers for the global alt+c event
                    window.output("Creation: Connecting global key up event handler for alt+c", "sample", "status");
                    this._globalKeyUpListener = this._handleGlobalKeyUp.bind(this);
                    document.body.addEventListener('keyup', this._globalKeyUpListener, false);
                },
                _handleSubmitClick: function () {
                    this._showAge.bind(this)();
                },
                _handleGlobalKeyUp: function (e) {
                    if ((e.key === "c" && e.altKey)) {
                        this._showAge.bind(this)();
                    }
                },
                _showAge: function () {
                    // calculate age in days
                    var msecPerMinute = 1000 * 60;
                    var msecPerHour = msecPerMinute * 60;
                    var msecPerDay = msecPerHour * 24;

                    var birthdate = this._datePicker.current;
                    var interval = new Date().getTime() - birthdate.getTime();

                    var days = Math.floor(interval / msecPerDay);

                    // put calculated days in a result message
                    this._resultDiv.textContent = "Your age in days is " + days;
                },
                dispose: function () {
                    window.output("Dispose: Starting Age Control dispose function", "sample", "status");

                    if (this._disposed) {
                        return;
                    }

                    this._disposed = true;

                    window.output("Dispose: Disposing all child controls", "sample", "status");
                    // ensure dispose is called on the child DatePicker control
                    WinJS.Utilities.disposeSubTree(this.element);

                    window.output("Dispose: Disconnecting from submit button click event listener", "sample", "status");
                    // disconnect from submit button click handler
                    if (this._submitButton && this._submitListener) {
                        this._submitButton.removeEventListener("click", this._submitListener, false);
                    }

                    window.output("Dispose: Disconnecting from global key up event listener", "sample", "status");
                    // disconnect from global key up event handler
                    if (this._globalKeyUpListener) {
                        document.body.removeEventListener('keyup', this._globalKeyUpListener, false);
                    }

                    window.output("Dispose: finished disposing Age Control", "sample", "status");
                }
            }
        )
    });
})();