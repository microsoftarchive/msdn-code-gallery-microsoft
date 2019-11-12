//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
(function () {
    "use strict";

    WinJS.UI.Pages.define("/pages/chainedAsync/chainedAsync.html", {
        ready: function (element, options) {
            startChain.addEventListener("click", function () {
                goodPromise().
                   then(
                      function () {
                          return goodPromise();
                      }).
                   then(
                      function () {
                          return badPromise();
                      }).
                   then(
                      function () {
                          return goodPromise();
                      }).
                   done(
                      function () {
                          // This *shouldn't* get called
                      },
                      function (err) {
                          document.getElementById('output').innerText = err.toString();
                      });
            });

        }
    });

    function goodPromise() {
        return new WinJS.Promise(function (comp, err, prog) {
            try {
                comp();
            } catch (ex) {
                err(ex)
            }
        });
    }

    // ERROR: This returns an errored-out promise.
    function badPromise() {
        return new WinJS.Promise(function (comp, err, prog) {
            try {
                var newError = new Error("I broke my promise :(");
                throw newError;

                // This won’t get called …
                comp();
            } catch (ex) {
                err(ex);
            }
        });
    }
})();
