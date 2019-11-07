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

    WinJS.UI.Pages.define("/pages/heroimage.html", {
        myData: {
            get: function () {
                return myData;
            }
        }
    });
})();
