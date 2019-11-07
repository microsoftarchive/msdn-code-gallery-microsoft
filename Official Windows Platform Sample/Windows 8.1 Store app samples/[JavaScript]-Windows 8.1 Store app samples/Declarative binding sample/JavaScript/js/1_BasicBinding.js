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
    var page = WinJS.UI.Pages.define("/html/1_BasicBinding.html", {

        init: function (element, options) {
            // Our data source - not that this is not explicitly
            // bindable - that's done later.
            this.bindingSource = {
                text: "Initial text",
                color: {
                    red: 128,
                    green: 128,
                    blue: 128
                }
            };
        },

        ready: function (element, options) {
            // Hook up our inputs so that they update the
            // appropriate places in our binding source.

            // First, we call WinJS.Binding.as to get the bindable proxy object
            var b = WinJS.Binding.as(this.bindingSource);

            // The bindTextBox method wired up change events, defined below
            this.bindTextBox("#basicBindingInputText", b.text,
                function (value) { b.text = toStaticHTML(value); });

            this.bindTextBox("#basicBindingInputRed", b.color.red,
                function (value) { b.color.red = toStaticHTML(value); });

            this.bindTextBox("#basicBindingInputGreen", b.color.green,
                function (value) { b.color.green = toStaticHTML(value); });

            this.bindTextBox("#basicBindingInputBlue", b.color.blue,
                function (value) { b.color.blue = toStaticHTML(value); });

            // Now, hook up the declarative binding to our binding source.
            // This is done via a call to WinJS.Binding.processAll, passing the
            // target element and the binding source.

            WinJS.Binding.processAll(element.querySelector("#basicBindingOutput"), this.bindingSource)
                .done(function () {
                    // processAll is async. You can hook up a then or done handler
                    // if you need to wait for the binding to finish
                    WinJS.log && WinJS.log("Binding wireup complete", "sample", "status");
                });
        },



        //
        // Helper function to set up bindings on text boxes
        //
        bindTextBox: function (selector, initialValue, setterCallback) {
            var textBox = this.element.querySelector(selector);
            textBox.addEventListener("change", function (evt) {
                setterCallback(evt.target.value);
            }, false);
            textBox.value = initialValue;
        }

    });

    // Custom binding initializer to convert to CSS color
    // This method is called to set up the binding. It only gets called once.
    // Source is the source object, sourceProperty is the strings
    // supplied in the binding action, dest is the destination object,
    // and destProperty is the destination property string.
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
                dest.style.backgroundColor = rgb(source.color.red, source.color.green, source.color.blue);
            }

            // in this case, we're binding to a composite property. In order
            // to get things to fire properly, we're going to hook up
            // to multiple fields in the source object.
            // Make sure to return the result of calling bind - this allows the
            // binding system to properly clean up if the target element gets
            // removed from the DOM.
            return WinJS.Binding.bind(source, {
                color: {
                    red: setBackColor,
                    green: setBackColor,
                    blue: setBackColor,
                }
            });
        }
    );

    // A little helper function to convert from separate rgb values to a css color
    function rgb(r, g, b) { return "rgb(" + [r, g, b].join(",") + ")"; }

    // Binding initializers must be available as a public reference
    // in order to be used declaratively. "Publish" our binding
    // initializer via a namespace

    WinJS.Namespace.define("BasicBinding", {
        toCssColor: toCssColor
    });

})();
