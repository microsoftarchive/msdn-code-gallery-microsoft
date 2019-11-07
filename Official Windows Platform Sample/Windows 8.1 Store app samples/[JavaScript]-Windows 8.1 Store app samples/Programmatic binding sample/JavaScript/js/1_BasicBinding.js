//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="//Microsoft.WinJS.2.0/js/base.js" />

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/1_BasicBinding.html", {
        init: function(element, options) {
            // Define the source object that we're binding to.
            // We use WinJS.Binding.as to set up the observable
            // behavior.

            this.bindSource = WinJS.Binding.as({ x: "initial value for x", y: "initial value for y" });
        },

        ready: function (element, options) {
            this.xInput = element.querySelector(".basicBindingX");
            this.yInput = element.querySelector(".basicBindingY");
            this.xOutput = element.querySelector(".basicBindingXOutput");
            this.yOutput = element.querySelector(".basicBindingYOutput");

            this.bindToDataSource();
            this.bindToInputChanges();
        },

        bindToDataSource: function () {
            // The object we're binding to was created in the init
            // method. Hook up the change events for each property.

            // First, the x property
            this.bindSource.bind("x", this.onXChanged.bind(this));
            
            // And the y property
            this.bindSource.bind("y", this.onYChanged.bind(this));
        },

        bindToInputChanges: function () {
            // This method hooks up to changes in the input fields
            // in the HTML. Each field change will set the value
            // of one of the properties on our binding source. That
            // will in turn cause the change events to fire, and
            // update the output display via the onXChanged and
            // onYChanged methods.

            // Grab binding source so it's avialable in closures below
            var bindSource = this.bindSource;

            // Set up the X input field
            WinJS.Utilities.query(".basicBindingX", this.element)
                // Set up change event handler to update binding source
                // on input changes.
                .listen("change", function () {
                    bindSource.x = this.value;
                })
                // Set the initial value to match binding source
                .forEach(function (e) {
                    e.value = bindSource.x;
                });

            WinJS.Utilities.query(".basicBindingY", this.element)
                // Set up change event handler to update binding source
                // on input changes.
                .listen("change", function () {
                    bindSource.y = this.value;
                })
                // Set the initial value to match binding source
                .forEach(function (e) {
                    e.value = bindSource.y;
                });
        },

        onXChanged: function (newValue, oldValue) {
            // This function is called when the x property of the
            // binding source changes
            this.xOutput.textContent = newValue;
        },

        onYChanged: function (newValue, oldValue) {
            // This function is called when the y property of the
            // binding source changes
            this.yOutput.textContent = newValue;
        }
    });

})();
