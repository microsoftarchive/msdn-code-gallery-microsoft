//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    window.output = function (s) {
        var outputEl = document.getElementById("output");
        outputEl.innerText += "\n" + s;
    };
})();