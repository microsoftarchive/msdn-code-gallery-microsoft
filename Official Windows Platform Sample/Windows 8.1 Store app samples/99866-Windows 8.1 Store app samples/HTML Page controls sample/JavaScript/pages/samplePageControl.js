//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="//Microsoft.WinJS.0.6/js/base.js" />
/// <reference path="//Microsoft.WinJS.0.6/js/ui.js" />


(function () {
    "use strict";

    var ControlConstructor = WinJS.UI.Pages.define("/pages/samplePageControl.html", {
        // This function is called after the page control contents
        // have been loaded, controls have been activated, and
        // the resulting elements have been parented to the DOM.
        ready: function (element, options) {
            options = options || {};
            this._data = WinJS.Binding.as({ controlText: options.controlText, message: options.message });

            // Data bind to the child tree to set the control text
            WinJS.Binding.processAll(element, this._data);

            // Hook up the click handler on our button
            WinJS.Utilities.query("button", element).listen("click",
                // JavaScript gotcha - use function.bind to make sure the this reference
                // inside the event callback points to the control object, not to
                // window
                this._onclick.bind(this));

            // WinJS controls can be manipulated via code in the page control too
            WinJS.Utilities.query(".samplePageControl-toggle", element).listen("change",
                this._ontoggle.bind(this));
        },

        // Getter/setter for the controlText property.
        controlText: {
            get: function () { return this._data.controlText; },
            set: function (value) { this._data.controlText = value; }
        },

        // Event handler that was wired up in the ready method
        _onclick: function (evt) {
            WinJS.log && WinJS.log(this._data.message + " button was clicked", "sample", "status");
        },

        // Event handler for when the toggle control switches
        _ontoggle: function (evt) {
            var toggleControl = evt.target.winControl;
            WinJS.log && WinJS.log(this._data.message + " toggle is now " + toggleControl.checked, "sample", "status");
        }
    });

    // The following lines expose this control constructor as a global.
    // This lets you use the control as a declarative control inside the
    // data-win-control attribute.

    WinJS.Namespace.define("Controls_PageControls", {
        SamplePageControl: ControlConstructor
    });
})();
