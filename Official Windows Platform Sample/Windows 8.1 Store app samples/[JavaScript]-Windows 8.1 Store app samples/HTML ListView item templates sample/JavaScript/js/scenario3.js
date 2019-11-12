//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario3.html", {
        ready: function (element, options) {
            var listView = element.querySelector("#listView").winControl;
            listView.forceLayout();

            listView.itemTemplate = regularListIconTemplate;
            var CurrentTemplate_regularListIconTemplate = true;

            // Swap data templates
            function swapTemplate() {
                if (CurrentTemplate_regularListIconTemplate) {
                    listView.itemTemplate = regularListIconTextTemplate;
                    CurrentTemplate_regularListIconTemplate = false;
                } else {
                    listView.itemTemplate = regularListIconTemplate;
                    CurrentTemplate_regularListIconTemplate = true;
                }
            }

            element.querySelector("#swapTemplateButton").addEventListener("click", swapTemplate, false);
        }
    });
})();
