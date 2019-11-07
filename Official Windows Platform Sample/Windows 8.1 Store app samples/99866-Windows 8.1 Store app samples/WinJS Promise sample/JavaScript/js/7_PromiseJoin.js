//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="//Microsoft.WinJS.2.0/js/base.js" />

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/7_PromiseJoin.html", {
        ready: function (element, options) {
            this.outputDiv = element.querySelector(".promiseJoinOutput");
            this.clearOutput();

            WinJS.Utilities.query(".promiseJoinButton", element)
                .listen("click", this.joinPromises.bind(this));
        },

        joinPromises: function () {
            var that = this;

            this.clearOutput();
            this.output(" Creating two Promises" + "\r\n\r\n The first takes 2 seconds and the second takes 3 seconds ");

            // create an array to hold promises
            var p = [];
            var index = 0;

            // create a promise that completes in 2000ms, then add it to the promise array
            p[index++] = WinJS.Promise.timeout(2000).
            then(function () {
                that.output("\r\n\r\n\r\n First promise is fullfilled after 2 seconds \n");
            }
            );

            // create a promise that completes in 3000ms, then add it to the promise array
            p[index++] = WinJS.Promise.timeout(3000).
            then(function () {
                that.output("\r\n\r\n\r\n Second promise is fullfilled after 3 seconds \n");
            }
            );

            // pass the promise array to Promise.join() to create a promise that will not be completed until all
            // promises have been completed.
            WinJS.Promise.join(p).
            then(function () {
                that.output("\r\n\r\n\r\n Calling join will ensure this function will not be called until the both promises have been completed.");
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

