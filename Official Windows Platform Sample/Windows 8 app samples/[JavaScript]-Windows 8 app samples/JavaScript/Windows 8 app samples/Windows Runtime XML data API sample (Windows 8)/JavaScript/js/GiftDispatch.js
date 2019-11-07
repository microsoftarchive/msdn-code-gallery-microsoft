//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/GiftDispatch.html", {
        ready: function (element, options) {
            document.getElementById("scenario4BtnDefault").addEventListener("click", scenario4Dispatch, false);
            scenario4Inialize();
        }
    });

    function scenario4Inialize() {
        loadXmlFile("giftDispatch", "employees.xml", "scenario4");
    }
    function scenario4Dispatch() {
        var doc = new Windows.Data.Xml.Dom.XmlDocument;
        doc.loadXml(document.getElementById("scenario4OriginalData").value);

        var currentDate = new Date();
        var thisYear = 2012;    // we don't use currentDate.getFullYear() to get current year so that all gifts can be delivered.
        var previousOneYear = thisYear - 1;
        var previousFiveYear = thisYear - 5;
        var previousTenYear = thisYear - 10;

        var xpath = new Array();
        // select >= 1 year and < 5 years
        xpath[0] = "descendant::employee[startyear <= " + previousOneYear + " and startyear > " + previousFiveYear + "]";
        // select >= 5 years and < 10 years
        xpath[1] = "descendant::employee[startyear <= " + previousFiveYear + " and startyear > " + previousTenYear + "]";
        // select >= 10 years
        xpath[2] = "descendant::employee[startyear <= " + previousTenYear + "]";

        var Gifts = new Array("Gift Card", "XBOX", "Windows Phone");

        var output = new Array();
        for (var i = 0; i < 3; i++) {
            var employees = doc.selectNodes(xpath[i]);
            for (var index = 0; index < employees.length; index++) {
                var employeeName = employees.item(index).selectSingleNode("descendant::name");
                var department = employees.item(index).selectSingleNode("descendant::department");

                output.push("[", employeeName.firstChild.nodeValue, "]/[", department.firstChild.nodeValue, "]/[", Gifts[i], "]\n");
            }
        }

        document.getElementById("scenario4Result").value = output.join("");
    }
})();
