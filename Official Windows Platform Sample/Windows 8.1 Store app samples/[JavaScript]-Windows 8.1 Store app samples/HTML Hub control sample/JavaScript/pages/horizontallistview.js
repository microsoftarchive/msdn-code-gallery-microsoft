//// Copyright (c) Microsoft Corporation. All rights reserved

// For an introduction to the Page Control template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232511
(function () {
    "use strict";

    var myData = new WinJS.Binding.List([
        { title: "Fire Hydrant", text: "Red", picture: "/images/circle_list1.jpg" },
        { title: "Fire Hydrant", text: "Yellow", picture: "/images/circle_list2.jpg" },
        { title: "Pothole Cover", text: "Gray", picture: "/images/circle_list3.jpg" },
        { title: "Sprinkler", text: "Yellow", picture: "/images/circle_list4.jpg" },
        { title: "Electrical Charger", text: "Yellow", picture: "/images/circle_list5.jpg" },
        { title: "Knob", text: "Red", picture: "/images/circle_list6.jpg" },
        { title: "Fire Hydrant", text: "Red", picture: "/images/circle_list1.jpg" },
        { title: "Fire Hydrant", text: "Yellow", picture: "/images/circle_list2.jpg" },
        { title: "Pothole Cover", text: "Gray", picture: "/images/circle_list3.jpg" },
        { title: "Fire Hydrant", text: "Red", picture: "/images/circle_list1.jpg" },
        { title: "Fire Hydrant", text: "Yellow", picture: "/images/circle_list2.jpg" },
        { title: "Pothole Cover", text: "Gray", picture: "/images/circle_list3.jpg" }

    ]);

    var myVerticalData = new WinJS.Binding.List([
            { title: "Fire Hydrant", text: "Red", picture: "/images/circle_list1.jpg" },
            { title: "Fire Hydrant", text: "Yellow", picture: "/images/circle_list2.jpg" },
            { title: "Pothole Cover", text: "Gray", picture: "/images/circle_list3.jpg" },
            { title: "Sprinkler", text: "Yellow", picture: "/images/circle_list4.jpg" },
            { title: "Electrical Charger", text: "Yellow", picture: "/images/circle_list5.jpg" },
            { title: "Knob", text: "Red", picture: "/images/circle_list6.jpg" }
    ]);

    WinJS.UI.Pages.define("/pages/horizontallistview.html", {
        processed: function (element, options) {
            this._listview = element.querySelector(".win-listview").winControl;
            this._listview.itemDataSource = this.myData.dataSource;
            this._onListViewResizeBound = this.onListViewResize.bind(this);
            window.addEventListener("resize", this._onListViewResizeBound);
            updateListViewLayout(this._listview);
        },


        // swap listview data source and layout when app is in small state
        onListViewResize: function () {
            if (document.body.contains(this._listview.element)) {
                updateListViewLayout(this._listview);
                return this._listview.layout;
            } else {
                return;
            }
        },

        unload: function () {
            window.removeEventListener("resize", this._onListViewResizeBound);
        },

        myData: {
            get: function () {
                return myData;
            }
        },

        myVerticalData: {
            get: function () {
                return myVerticalData;
            }
        }
    });

    function updateListViewLayout(listview) {
        if (document.body.clientWidth < 500) {
            if (!(listview.layout instanceof WinJS.UI.ListLayout)) {
                listview.itemDataSource = myVerticalData.dataSource;
                listview.layout = new WinJS.UI.ListLayout();
            }
        }
        else {
            if (listview.layout instanceof WinJS.UI.ListLayout) {
                listview.itemDataSource = myData.dataSource;
                listview.layout = new WinJS.UI.GridLayout();
            }
        }
    }

})();

