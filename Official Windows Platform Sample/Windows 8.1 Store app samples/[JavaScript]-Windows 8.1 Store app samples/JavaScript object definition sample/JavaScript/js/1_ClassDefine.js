//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <reference path="//Microsoft.WinJS.2.0/js/base.js" />

(function () {
    "use strict";

    // A simple "class" definition - using WinJS to define a constructor function
    // and add some properties to it's prototype.
    var Rect = WinJS.Class.define(
        // Constructor function
        function () { },
        {
            // These members are added to the constructors prototype object. You
            // can define data here...
            x: 0,
            y: 0,
            width: 0,
            height: 0,

            // Or you can define member functions.
            area: function () { return this.width * this.height; },
            toString: function () {
                return "rectangle (" +
                [this.x, this.y, this.width, this.height].join(", ") +
                ")";
            }
        }
    );

    // Some functions creating instances of our Rect type
    function defaultRect() {
        WinJS.log && WinJS.log("Default state of new Rect object", "sample", "status");
        return new Rect();
    }

    function rectWithPropertyChanges() {
        WinJS.log && WinJS.log("Status of rect after properties get set", "sample", "status");
        var r = new Rect();
        r.width = 5;
        r.height = 4;
        return r;
    }

    var page = WinJS.UI.Pages.define("/html/1_ClassDefine.html", {
        ready: function (element, options) {
            // WinJS.Utilities.QueryCollection makes it easier to find
            // and manipulate DOM elements. Here, we wrap our root
            // element in a query collection, and set up some event handlers
            var root = new WinJS.Utilities.QueryCollection(element);
            this.outputTable = root.query(".winClassDefineOutputTable");
            
            this.hideOutputTable();
            root.query(".winClassDefineCreateInstance").listen("click", this.showDefaultRect.bind(this));
            root.query(".winClassDefineCreateAndSetProperties").listen("click", this.showRectWithPropertyChanges.bind(this));
        },

        hideOutputTable: function () {
            this.outputTable.setStyle("visibility", "hidden");
        },
        
        showOutputTable: function () {
            this.outputTable.setStyle("visibility", "visible");
        },

        showDefaultRect: function () {
            this.showOutputTable();
            this.displayRect(defaultRect());
        },

        showRectWithPropertyChanges: function () {
            this.showOutputTable();
            this.displayRect(rectWithPropertyChanges());
        },

        displayRect: function (rect) {
            this.updateRow("X", rect.x, rect.hasOwnProperty("x"));
            this.updateRow("Y", rect.y, rect.hasOwnProperty("y"));
            this.updateRow("Width", rect.width, rect.hasOwnProperty("width"));
            this.updateRow("Height", rect.height, rect.hasOwnProperty("height"));
            this.updateRow("Area", rect.area(), rect.hasOwnProperty("area"));
            this.updateRow("ToString", rect.toString(), rect.hasOwnProperty("toString"));
        },

        updateRow: function (rowIdSuffix, propertyValue, isOwnProperty) {
            var tr = this.outputTable.query(".winClassDefine" + rowIdSuffix)[0];
            tr.cells[1].innerText = propertyValue;
            tr.cells[2].innerText = isOwnProperty ? "no" : "yes";
        }
    });
})();
