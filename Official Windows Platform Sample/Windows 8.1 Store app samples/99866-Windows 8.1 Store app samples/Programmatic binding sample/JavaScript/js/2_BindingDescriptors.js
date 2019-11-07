//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="//Microsoft.WinJS.2.0/js/base.js" />

(function () {
    "use strict";

    // This helper class manages the contents of a text box.
    // It handles input validation - in this case making sure
    // the input is a valid number, as well as firing a change
    // notification when the values do change.
    var NumericTextInput = WinJS.Class.define(
        function (element, selector, initialValue, changeCallback) {
            this.element = element.querySelector(selector);
            this.element.value = initialValue;
            this._onchange();
            element.addEventListener("change", this._onchange.bind(this));
            this.onvaluechange = changeCallback;
        },
        {
            // This function is called when the value changes
            // to a new valid value. 
            onvaluechange: function (newValue) { },

            // Called when text box contents change
            _onchange: function () {
                var value = parseInt(this.element.value, 10);
                if (!isNaN(value)) {
                    this.onvaluechange(value);
                    this.element.color = "black";
                    WinJS.log && WinJS.log("", "sample", "status");
                } else {
                    this.element.color = "red";
                    WinJS.log && WinJS.log("Illegal value entered", "sample", "error");
                }
            }
        }
    );

    var page = WinJS.UI.Pages.define("/html/2_BindingDescriptors.html", {
        init: function (element, options) {
            // This is the binding source - it's an object with two nested sub-objects
            this.objectPosition = WinJS.Binding.as({
                position: { x: 10, y: 10},
                color: { r: 128, g: 128, b: 128 }
            });
            
            // Track the current position so that we can erase and redraw
            this.currentPosition = {
                x: this.objectPosition.position.x,
                y: this.objectPosition.position.y
            };
        },

        ready: function (element, options) {
            var canvas = element.querySelector(".bindingDescriptorsOutputCanvas");
            this.context = canvas.getContext("2d");

            // Establish bindings for the various fields. We can't just bind to
            // position or color, because doing so would only trigger change notification
            // when the entire object is replaced, not just changing the fields.

            WinJS.Binding.bind(this.objectPosition, {
                position: {
                    x: this.onPositionChange.bind(this),
                    y: this.onPositionChange.bind(this)
                },
                color: {
                    r: this.onColorChange.bind(this),
                    g: this.onColorChange.bind(this),
                    b: this.onColorChange.bind(this)
                }
            });

            this.bindInputEvents();
        },

        bindInputEvents: function () {
            // Connect the input fields on our HTML to our
            // binding source via code
            var that = this;

            this.inputs = [];
            
            // List of input fields and the properties they correspond to
            var inputFieldMap = [
                ["X",     this.objectPosition.position.x, function (v) { that.objectPosition.position.x = v; }],
                ["Y",     this.objectPosition.position.y, function (v) { that.objectPosition.position.y = v; }],
                ["Red",   this.objectPosition.color.r, function (v) { that.objectPosition.color.r = v; }],
                ["Green", this.objectPosition.color.g, function (v) { that.objectPosition.color.g = v; }],
                ["Blue",  this.objectPosition.color.b, function (v) { that.objectPosition.color.b = v; }]
            ];

            // Grab binding source so it's visible in the closure below
            var objectPosition = this.objectPosition;

            // Go over our input fields, wiring them up to the right values in
            // our observable object
            this.inputs = inputFieldMap.map(function (selector) {
                new NumericTextInput(that.element, ".bindingDescriptorsInput" + selector[0],
                    selector[1], selector[2]);
                input.onvaluechange = selector[2];
            });
        },

        // Location of rectangle has changed
        onPositionChange: function (newValue, oldValue) {
            this.erase();
            this.currentPosition = { x: this.objectPosition.position.x, y: this.objectPosition.position.y };
            this.draw();
        },

        // Redraw in new color
        onColorChange: function (newValue, oldValue) {
            this.draw();
        },

        erase: function () {
            this.context.fillStyle = "rgb(255,255,255)";
            this.context.fillRect(this.currentPosition.x, this.currentPosition.y, 75, 75);
        },

        draw: function () {
            var colors = [this.objectPosition.color.r,
                this.objectPosition.color.g,
                this.objectPosition.color.b];

            var fillColor = "rgb(" + colors.join(",") + ")";
            this.context.fillStyle = fillColor;
            this.context.fillRect(this.currentPosition.x, this.currentPosition.y, 75, 75);
        }
    });
})();
