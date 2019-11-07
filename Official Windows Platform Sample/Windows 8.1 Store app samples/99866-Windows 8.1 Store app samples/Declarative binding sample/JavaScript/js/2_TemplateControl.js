//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="//Microsoft.WinJS.2.0/js/base.js" />
/// <reference path="//Microsoft.WinJS.2.0/js/ui.js" />

(function () {
    "use strict";

    // We want a bunch of these objects, so we're using
    // WinJS.Binding.define to declare an observable type.

    var DataSource = WinJS.Binding.define({
        text: "",
        color: { r: 0, g: 0, b: 0 }
    });

    var page = WinJS.UI.Pages.define("/html/2_TemplateControl.html", {
        init: function (element, options) {
            // Our actual data - we don't need to use WinJS.Binding.as on these
            // objects as they're already observable (since they were created by
            // WinJS.Binding.define).
            this.sourceObjects = [
                // This creates the observable object defined above and initializes its properties
                // from the data object passed in.
                new DataSource({ text: "First object", color: { r: 192, g: 64, b: 64 } }),
                new DataSource({ text: "Second object", color: { r: 64, g: 192, b: 64 } }),
                new DataSource({ text: "Third object", color: { r: 51, g: 153, b: 255 } })
            ];
        },
        ready: function (element, options) {
            var prefix = "#templateControlInput";

            this.textBox = element.querySelector(prefix + "TextInput");
            this.redTextBox = element.querySelector(prefix + "Red");
            this.greenTextBox = element.querySelector(prefix + "Green");
            this.blueTextBox = element.querySelector(prefix + "Blue");
            element.querySelector("#templateControlObjectSelector")
                .addEventListener("change", this.sourceObjectChange.bind(this), false);

            // The data source the UI is currently manipulating
            this.dataSource = this.sourceObjects[0];

            // Hook up the input events
            this.bindInputs();

            //
            // Render our template controls into the output DIV
            //
            var templateControl = element.querySelector("#templateControlTemplate").winControl;

            // The div to render into
            var renderHere = element.querySelector("#templateControlRenderTarget");

            this.sourceObjects.forEach(function (o) {
                templateControl.render(o).then(function (e) {
                    renderHere.appendChild(e);
                });
            });
        },

        bindInputs: function () {
            // Hook up event handlers for changes in the UI
            var that = this;

            this.textBox.addEventListener("change", function (evt) {
                that.dataSource.text = toStaticHTML(that.textBox.value);
            }, false);

            that.redTextBox.addEventListener("change", function () {
                that.dataSource.color.r = toStaticHTML(that.redTextBox.value);
            }, false);

            that.greenTextBox.addEventListener("change", function () {
                that.dataSource.color.g = toStaticHTML(that.greenTextBox.value);
            }, false);

            that.blueTextBox.addEventListener("change", function () {
                that.dataSource.color.b = toStaticHTML(that.blueTextBox.value);
            }, false);

            this.updateInputs();
        },

        updateInputs: function () {
            // Update the UI to match the contents of seleted data source.
            this.textBox.value = this.dataSource.text;
            this.redTextBox.value = this.dataSource.color.r;
            this.greenTextBox.value = this.dataSource.color.g;
            this.blueTextBox.value = this.dataSource.color.b;
        },

        // Event handler that triggers when the "object to change"
        // dropdown changes.
        sourceObjectChange: function (eventObject) {
            this.dataSource = this.sourceObjects[eventObject.target.selectedIndex];
            this.updateInputs();
        }
    });

    //
    // A binding initializer used to convert separate r,g,b properties
    // of an object to a single css color definition
    //
    // Note the function is wrapped in a call to WinJS.Binding.initializer.
    // This marks this function as usable in declarative markup.
    var toCssColor = WinJS.Binding.initializer(
        function toCssColor(source, sourceProperty, dest, destProperty) {
            // in this case, we're binding to a composite property. In order
            // to get things to fire properly, we're going to hook up
            // to multiple fields in the source object.

            // helper method to actually set the style
            function setBackColor() {
                dest.style.backgroundColor = rgb(source.color.r, source.color.g, source.color.b);
            }

            // helper function to convert to css style rgb syntax
            function rgb(r, g, b) {
                return "rgb(" + r + "," + g + "," + b + ")";
            }

            // in this case, we're binding to a composite property. In order
            // to get things to fire properly, we're going to hook up
            // to multiple fields in the source object.
            // Make sure to return the result of calling bind - this allows the
            // binding system to properly clean up if the target element gets
            // removed from the DOM.
            return WinJS.Binding.bind(source, {
                color: {
                    r: setBackColor,
                    g: setBackColor,
                    b: setBackColor,
                }
            });
        }
    );

    //
    // Binding initializers have to be public, so publish this out
    // via a namespace
    //
    WinJS.Namespace.define("TemplateControl", {
        toCssColor: toCssColor
    });

})();
