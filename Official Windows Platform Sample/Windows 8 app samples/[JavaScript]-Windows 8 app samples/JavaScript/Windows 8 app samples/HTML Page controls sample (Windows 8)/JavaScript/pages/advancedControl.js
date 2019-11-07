//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="//Microsoft.WinJS.0.6/js/base.js" />
/// <reference path="//Microsoft.WinJS.0.6/js/ui.js" />

(function () {
    "use strict";

    WinJS.UI.Pages.define("/pages/advancedControl.html", {

        // The load method is called to actually retrieve the information from
        // the uri provided. The default implementation calls the low level
        // fragment loader to retrieve the content. By overriding this, you are
        // taking control of the loading process.
        //
        // For the purposes of this sample, instead of using any external content,
        // we'll just supply some raw HTML.
        //
        // Expected uses of this overload would be if you were generating your
        // content via some other mechanism like a JavaScript templating library.
        load: function (uri) {
            this._log = ["load called"];

            // As the API is defined, you are required to return a promise for the content.
            // This makes sense, as downloading content from the web or disk is an async
            // operation. Since in this case we're returning static data, we return
            // a promise for our static data via WinJS.Promise.as
            return WinJS.Promise.as({ preamble: "<p>", postamble: "</p>" });
        },

        // The next method called in the page lifecycle is init.
        // At this point, external assets are not available yet.
        // This method is a good place to kick off async processes
        // that may take a while.
        //
        // init can return a promise that completes when init is finished.
        // In this case we aren't doing any init work that we want to delay
        // so this method doesn't return anything.
        init: function (element, options) {
            // Fake out something async, we'll finish this off in the
            // processed callback.
            this._asyncActivity = WinJS.Promise.timeout(1500);

            // Add a log entry - these will get rendered in the render method.
            this._log.push("init called");
        },

        // The render function is called after load and init
        // completes. It receives the result from the load promise, and then is
        // responsible for actually parenting it into the DOM.
        //
        // In this case, we'll just set the content manually.
        render: function (element, options, loadResult) {
            this._log.push("render called");

            // Render our contents
            var content = [];
            this._log.forEach(function (item) {
                content.push(loadResult.preamble + item + loadResult.postamble);
            });

            WinJS.Utilities.setInnerHTML(element, content.join(""));
        },

        // The processed method is called after render is complete and
        // the system has called WinJS.UI.processAll on the content.
        // It's a useful place to wait for async activity to finish as
        // well.
        //
        // You can return a promise here, that will delay the rest of the
        // process until the promise completes.
        processed: function (element, options) {
            return this._asyncActivity.then(function () {
                var p = document.createElement("p");
                p.textContent = "processed called";
                element.appendChild(p);
            });
        },

        // The ready method is called after everything has finished being adding to the DOM
        // and controls are processed. In most cases, this is the only method you'll need
        // to implement.
        ready: function (element, options) {
            var p = document.createElement("p");
            p.textContent = "This control was created";
            element.appendChild(p);
            WinJS.log && WinJS.log("Control creation complete", "sample", "status");
        },

        // This function is NOT part of the creation lifecycle. However, it demonstrates that
        // you can define arbitrary additional functions on page controls. In this case, it's
        // used by the hosting code.
        unloadExample: function () {
            WinJS.log && WinJS.log("Control has been unloaded", "sample", "status");
        }
    });
})();
