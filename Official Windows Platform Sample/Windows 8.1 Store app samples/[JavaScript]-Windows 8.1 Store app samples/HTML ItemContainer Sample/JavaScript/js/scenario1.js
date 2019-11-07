//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario1.html", {
        ready: function (element, options) {
            document.getElementById("button1").addEventListener("click", doSomething1, false);
        }
    });

    function doSomething1() {

        var selectedFlavors = new Array();
        var cartonString = "Your carton ";
        var isThereOneFlavor = false;

        selectedFlavors[0] = document.querySelector("#scen1-item1").winControl.selected;
        selectedFlavors[1] = document.querySelector("#scen1-item2").winControl.selected;
        selectedFlavors[2] = document.querySelector("#scen1-item3").winControl.selected;

        for (var i = 0; i < 3; i++) {
            if (selectedFlavors[i]) {
                isThereOneFlavor = true;
                break;
            }
        }

        var flavors = [];
        if (isThereOneFlavor) {
            if (selectedFlavors[0]) {
                flavors.push("Vanilla");
            }
            if (selectedFlavors[1]) {
                flavors.push("Strawberry");
            }
            if (selectedFlavors[2]) {
                flavors.push("Orange");
            }
            cartonString += "has: " + flavors.join(", ");
        } else {
            cartonString += "is empty. Add some flavors!";
        }

        WinJS.log && WinJS.log(cartonString, "sample", "status");
    }


})();
