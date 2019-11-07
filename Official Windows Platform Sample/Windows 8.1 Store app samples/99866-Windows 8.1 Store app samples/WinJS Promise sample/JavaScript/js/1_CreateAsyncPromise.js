//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/1_CreateAsyncPromise.html", {
        ready: function (element, options) {
            WinJS.Utilities.query(".asynchronousEventButton", element).listen("click",
                this.fireAsynchronousEvent.bind(this));
            this.outputDiv = element.querySelector(".asyncPromiseOutput");
            this.outputDiv.innerText = "";
        },

        fireAsynchronousEvent: function () {
            var that = this;
            var num1 = 1;
            var num2 = 2;

            asyncAdd(num1, num2).
                then(function (v) {
                    that.output(" Chaining: First then with value = " + v + " after adding " + num1 + " to " + num2);
                    num1 = v;
                    num2 = 3;

                    // this return value is passed along to the next promise chain
                    return asyncAdd(v, num2);
                }).
                then(function (v) {
                    // this function receives the return value from the previous promise chain through the 'v' parameter
                    that.output("\r\n\r\n Chaining: Second then with value = " + v + " after adding " + num1 + " to " + num2);
                    num1 = v;
                    num2 = 4;

                    // this return value is passed along to the next promise chain
                    return asyncAdd(v, num2);
                }).
                then(function (v) {
                    // this function receives the return value from the previous promise chain through the 'v' parameter
                    that.output("\r\n\r\n Chaining: Final then with value = " + v + " after adding " + num1 + " to " + num2);
                }).done();
        },

        output: function (s) {
            this.outputDiv.innerText += s;
        }
    });

    // Do an add asynchronously (via setTimeout)
    function asyncAdd(num1, num2) {
        return new WinJS.Promise(function (complete) {
            setTimeout(function () {
                var sum = num1 + num2;

                // this promise is completed when the complete() function is called which also
                // returns the value (result) of the promise.
                complete(sum);
            }, 1000);
        });
    }
})();
