//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="//Microsoft.WinJS.2.0/js/base.js" />

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/8_PromiseAny.html", {
        ready: function (element, options) {
            this.outputDiv = element.querySelector(".promiseAnyOutput");
            this.clearOutput();

            WinJS.Utilities.query(".promiseAnyButton", element)
                .listen("click", this.anyPromises.bind(this));
        },

        anyPromises: function () {
            var that = this;

            this.clearOutput();
            this.output(" Creating two Promises" + "\r\n\r\n The first takes 3 seconds and the second takes 1 second ");

            // create empty array to hold the promises
            var p = [];
            var index = 0;

            // create a promise that will complete in 3000ms and add it to the array of promises
            p[index++] = WinJS.Promise.timeout(3000).
                then(function () {
                    that.output("\r\n\r\n\r\n First promise is fullfilled after the second promise. It takes 3 seconds \n");
                }
            );
            // create a promise that will complete in 1000ms and add it to the array of promises
            p[index++] = WinJS.Promise.timeout(1000).
                then(function () {
                    that.output("\r\n\r\n\r\n Second promise is fullfilled before first promise is over. It takes 1 second \n");
                }
            );

            // pass the array of 2 promices to Promise.any() so it will complete as soon as the first promise in the array completes.
            WinJS.Promise.any(p).
                then(function () {
                    that.output("\r\n\r\n\r\n Calling any will enforce that at least one of the two promises is over before showing this message");
                });
        },

        clearOutput: function () {
            this.outputDiv.innerText = "";
        },

        output: function (s) {
            this.outputDiv.innerText += s;
        }
    });
})();
