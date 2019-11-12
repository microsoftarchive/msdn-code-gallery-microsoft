//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    // Define the class that handles drawing our rectangle. Note that
    // this definition does not itself define observable behavior.

    var RectangleSprite = WinJS.Class.define(
        function (x, y, r, g, b) {

            // This line is important: without it the observability
            // implementation won't get initialized correctly
            this._initObservable();

            // Set up our member data
            this.position = { x: x, y: y };
            this.r = r;
            this.g = g;
            this.b = b;
            this.dx = +3;
            this.dy = +3;

            this.timeout = null;
        }, {
            start: function () {
                var that = this;
                if (this.timeout === null) {
                    this.timeout = setInterval(function () { that._step(); }, 500);
                }

            },
            stop: function () {
                if (this.timeout !== null) {
                    clearInterval(this.timeout);
                    this.timeout = null;
                }
            },
            _step: function () {
                var x = this.position.x + this.dx;
                if (x < 0 || x > 150) {
                    this.dx = -this.dx;
                    x += this.dx;
                }

                var y = this.position.y + this.dy;
                if (y < 0 || y > 75) {
                    this.dy = -this.dy;
                    y += this.dy;
                }

                // This method is provided by the mixin
                this.setProperty("position", { x: x, y: y });
            }
        },
        {
            rectangleSize: 75
        });

    // Now we mix in the binding mixin and define the event properties
    WinJS.Class.mix(RectangleSprite,
        WinJS.Binding.mixin,
        WinJS.Binding.expandProperties({ position: 0, r: 0, g: 0, b: 0 })
    );

    // This helper class manages the contents of a text box.
    // It handles input validation - in this case making sure
    // the input is a valid number, as well as firing the
    // change notifications when values change.
    var NumericTextInput = WinJS.Class.define(
        function (element, selector, initialValue, valueChangeCallback) {
            this.element = element.querySelector(selector);
            this.element.value = initialValue;
            this._onchange();
            element.addEventListener("change", this._onchange.bind(this));
            this.onvaluechange = valueChangeCallback;
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

    var page = WinJS.UI.Pages.define("/html/3_CreatingBindableTypes.html", {
        init: function (element, options) {
            // The object we're binding change events to
            this.sprite = new RectangleSprite(10, 10, 128, 128, 128);

        },
        ready: function (element, options) {
            var that = this;
            this.canvas = element.querySelector(".creatingBindableTypesOutputCanvas");
            this.context = this.canvas.getContext("2d");

            // Binding to the various properties works
            this.sprite.bind("position", this._onPositionChange.bind(this));
            var colorChange = this._onColorChange.bind(this);
            this.sprite.bind("r", colorChange);
            this.sprite.bind("g", colorChange);
            this.sprite.bind("b", colorChange);

            this._bindInputs();

            element.querySelector(".creatingBindableTypesStart").addEventListener("click",
                function () { that.sprite.start(); }, false);
            element.querySelector(".creatingBindableTypesStop").addEventListener("click",
                function () { that.sprite.stop(); }, false);
        },

        // Hook up our inputs on the screen to update our sprite object
        _bindInputs: function () {
            var that = this;

            new NumericTextInput(this.element, ".creatingBindableTypesInputRed",
                this.sprite.r, function (v) { that.sprite.r = v; });
            new NumericTextInput(this.element, ".creatingBindableTypesInputGreen",
                this.sprite.g, function (v) { that.sprite.g = v; });
            new NumericTextInput(this.element, ".creatingBindableTypesInputBlue",
                this.sprite.b, function (v) { that.sprite.b = v; });
        },

        _onPositionChange: function (newValue, oldValue) {
            // on first call, oldValue won't be set
            if (oldValue) {
                this._erase(oldValue.x, oldValue.y);
            }
            this._draw();

        },

        _onColorChange: function (newValue, oldValue) {
            // We just redraw - we read the new colors from the sprite itself
            this._draw();
        },

        // Erase the current sprite from the canvas at it's previous position
        _erase: function (x, y) {
            this.context.fillStyle = "White";
            this.context.fillRect(x, y, RectangleSprite.rectangleSize, RectangleSprite.rectangleSize);
        },

        // Draw the sprite in it's current color at it's current location
        _draw: function () {
            var colors = [this.sprite.r, this.sprite.g, this.sprite.b];
            this.context.fillStyle = "rgb(" + colors.join(",") + ")";
            this.context.fillRect(this.sprite.position.x, this.sprite.position.y,
                RectangleSprite.rectangleSize, RectangleSprite.rectangleSize);
        }
    });
})();
