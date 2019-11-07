//// Copyright (c) Microsoft Corporation. All rights reserved



(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario2.html", {
        ready: function (element, options) {
            document.getElementById("button1").addEventListener("click", doSomethingBackward, false);
            document.getElementById("button2").addEventListener("click", doSomethingForward, false);
        }
    });

    function doSomethingForward() {
        var pivotControl = document.getElementById("pivotScenario2").winControl;
        var pivotControlLength = pivotControl.items.length;
        var pivotSelected = pivotControl.selectedIndex;

        if (pivotSelected < pivotControlLength-1) {
            pivotControl.selectedIndex++;
        }
        else {
            pivotControl.selectedIndex = 0;
        }
    }

    function doSomethingBackward() {
        var pivotControl = document.getElementById("pivotScenario2").winControl;
        var pivotControlLength = pivotControl.items.length;
        var pivotSelected = pivotControl.selectedIndex;

        if (pivotSelected > 0) {
            pivotControl.selectedIndex--;
        }
        else {
            pivotControl.selectedIndex = pivotControlLength-1;
        }
    }
})();
