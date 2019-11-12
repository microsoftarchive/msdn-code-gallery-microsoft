//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/4-Navigate.html", {
        init: function (element, options) {
            var data = [
               { location: "/html/1-CreateNavBar.html", label: "Scenario 1", icon: 'viewall' },
               { location: "/html/2-UseData.html", label: "Scenario 2", icon: 'viewall' },
               { location: "/html/3-UseVerticalLayout.html", label: "Scenario 3", icon: 'viewall' },
               { location: "/html/4-Navigate.html", label: "Scenario 4", icon: 'viewall' },
               { location: "/html/5-UseSearchControl.html", label: "Scenario 5", icon: 'viewall' },
               { location: "/html/6-UseSplitButton.html", label: "Scenario 6", icon: 'viewall' }
            ];

            Data.items = new WinJS.Binding.List(data);
        }
    });
})();
