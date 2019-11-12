//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="//Microsoft.WinJS.2.0/js/base.js" />

(function () {
    "use strict";

    // The class that we start with
    var Rect = WinJS.Class.define(
        function () { }, {
            x: 0,
            y: 0,
            width: 0,
            height: 0,
            area: function () { return this.width * this.height; },
            toString: function () {
                return "rectangle (" +
                [this.x, this.y, this.width, this.height].join(", ") +
                ")";
            }
        });

    WinJS.UI.Pages.define("/html/2_Mixins.html", {
        ready: function (element, options) {
            // Display the properties of the type's prototype before the mixin
            this.displayPrototypeProperties(Rect, 0);

            // Do the mixin - this adds the properties of the mixed-in object
            // to the prototype of the destination
            WinJS.Class.mix(Rect, WinJS.Utilities.eventMixin);

            // Display properties in the second column to compare
            this.displayPrototypeProperties(Rect, 1);
        },

        // Fill in the output result table.
        displayPrototypeProperties: function (obj, column) {
            var that = this;
            var table = this.element.querySelector(".winClassMixinResultsTable");

            var proto = obj.prototype;
            var rowNum = 0;
            var tableCell;
            var prop;

            // Return the <td> element at r, c in the table, creating a new row
            // or cell if required
            function getTableCell(r, c) {
                var row = getOrAddRow(r);
                for (var index = row.cells.length; index <= c; index++) {
                    row.insertCell(index);
                }
                return row.cells[c];
            }

            // Return the <tr> element for row r, creating a new one if necessary
            function getOrAddRow(r) {
                var body = table.querySelector("tbody");
                var rows = body.querySelectorAll("tr");
                if (rows.length > r) {
                    return rows[r];
                }

                var row = document.createElement("tr");
                body.appendChild(row);
                return row;
            }

            for (prop in proto) {
                if (proto.hasOwnProperty(prop)) {
                    tableCell = getTableCell(rowNum, column);
                    tableCell.innerHTML = prop;
                    rowNum++;
                }
            }
        }
    });
})();
