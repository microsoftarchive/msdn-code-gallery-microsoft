//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="//Microsoft.WinJS.2.0/js/base.js" />

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/5_PromiseAs.html", {
        ready: function (element, options) {
            this.outputDiv = element.querySelector(".promiseAsOutput");
            this.clearOutput();

            WinJS.Utilities.query(".promiseAsButton", element)
                .listen("click", this.addNumbers.bind(this));
        },

        addNumbers: function () {
            var that = this;
            var count = 1;
            var num1 = count++;
            var num2 = count++;
            this.clearOutput();

            promiseAdd(num1, num2).
            then(function (v) {
                that.output(" Chaining: " + num1 + " + " + num2 + " = " + v + " in a promise");
                num1 = v;
                num2 = count++;

                // wrap the regular non-synchronous method in Promise.as() to treat it as a promise
                return WinJS.Promise.as(nonPromiseAdd(num1, num2));
            }).
            then(function (v) {
                that.output("\r\n\r\n Chaining: " + num1 + " + " + num2 + " = " + v + " in a function converted to a promise using Promise.as");
                num1 = v;
                num2 = count++;
                return promiseAdd(num1, num2);
            }).
            then(function (v) {
                that.output("\r\n\r\n Chaining: " + num1 + " + " + num2 + " = " + v + " in a promise");
                num1 = v;
                num2 = count++;

                // wrap the regular non-synchronous method in Promise.as() to treat it as a promise
                return WinJS.Promise.as(nonPromiseAdd(num1, num2));
            }).
            then(function (v) {
                that.output("\r\n\r\n Chaining: " + num1 + " + " + num2 + " = " + v + " in a function converted to a promise using Promise.as");
            });

        },
        clearOutput: function () {
            this.outputDiv.innerText = "";
        },

        output: function (s) {
            this.outputDiv.innerText += s;
        }
    });

    // add two numbers together synchronously
    function nonPromiseAdd(num1, num2) {
        return num1 + num2;
    }

    // add two numbers together using a promise
    function promiseAdd(num1, num2) {
        return WinJS.Promise.timeout(1000)
            .then(function () {
                return num1 + num2;
            });
    }

})();
