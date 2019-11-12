//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="//Microsoft.WinJS.2.0/js/base.js" />

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/3_CatchingErrors.html", {
        ready: function (element, options) {
            this.outputDiv = element.querySelector(".catchingErrorsOutput");
            WinJS.Utilities.query(".catchingErrorsButton", element)
                .listen("click", this.catchThrownExceptionInPromise.bind(this));
        },

        catchThrownExceptionInPromise: function () {
            var that = this;
            var count = 0;

            // WinJS.Promise.wrapError is used to take a synchronous exception
            // and turn it into a promise value
            WinJS.Promise.wrapError("Exception thrown inside a promise")
            .then(function () {
                // this code in the completion function never executes because the starting promise
                // is in the exception state, which is passed along to the next then() chain.
                count++;
            }).then(function () {
                // this code in the completion function never executes because the previous promise
                // is in the exception state, which is passed along to the next then() in the chain.
                count++;
            }).then(null, function (e) {
                // this then chain() does not have a completion function (uses null) instead, but it does provide an implementation
                // of an optional error function(e) which enables it to catch the error in the original promise.
                count++;
                that.output(" Catching exception with value " + e.toString());
                that.output("\r\n\r\n No other complete function is called. " +
                "The variable 'count' tracks code flow, it is only actually incremented inside the error function of the final " +
                "then() chain and expected to be equal to 1.\r\n\r\n  Value of 'count' is " + count);
            });
        },

        output: function (s) {
            this.outputDiv.innerText += s;
        }
    });
})();
