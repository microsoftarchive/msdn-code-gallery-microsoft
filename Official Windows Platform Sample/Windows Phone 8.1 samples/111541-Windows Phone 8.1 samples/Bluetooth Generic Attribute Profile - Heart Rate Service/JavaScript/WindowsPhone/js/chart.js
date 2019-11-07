//// Copyright (c) Microsoft Corporation. All rights reserved


//// Because of limited screen space the chart is not displayed on the Windows Phone platform.
(function () {
    'use strict';

    // Define the class actually responsible for rendering the chart
    var renderer = WinJS.Class.define(function () { }, {

        makeVisible: function (/* canvasId */) {
            // Chart control is not displayed on the Windows Phone platform.
        },

        plot: function (/* data */) {
            // Chart control is not displayed on the Windows Phone platform.
        }
    });

    WinJS.Namespace.define('Chart', {
        renderer: renderer,
    });
})();
