//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="//Microsoft.WinJS.2.0/js/base.js" />

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/4_FragmentScriptCSS.html", {
        ready: function (element, options) {
            this.fragmentScriptCSSDiv = element.querySelector("#fragmentScriptCSSDiv");

            WinJS.Utilities.query("#fragmentScriptCSSButton", element)
                .listen("click", this.defaultScript.bind(this));
        },

        defaultScript: function () {
            var that = this;

            this.fragmentScriptCSSDiv.innerHTML = "";

            // Read fragment from the HMTL file and load it into the div.  This
            // fragment also loads linked CSS and JavaScript specified in the fragment
            WinJS.UI.Fragments.renderCopy("/html/4_FragmentScriptCSS_fragment.html",
                this.fragmentScriptCSSDiv)
                .done(function (fragment) {
                        // After the fragment is loaded into the target element,
                        // CSS and JavaScript referenced in the fragment are loaded.  The
                        // fragment loads script that defines an initialization function,
                        // so we can now call it to initialize the fragment's contents.
                        FragmentScriptCSS_Fragment.fragmentLoad(fragment);
                        WinJS.log && WinJS.log("successfully loaded fragment, change date to fire change event.", "sample", "status");
                    },

                    function (error) {
                        WinJS.log && WinJS.log("error loading fragment: " + error, "sample", "error");
                    }
                );
        }
    });

    function doSomething() {
        WinJS.log && WinJS.log("Error message here", "sample", "error");
    }
})();
