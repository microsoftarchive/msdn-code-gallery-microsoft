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

    WinJS.UI.Pages.define("/pages/semanticzoom.html", {

        // This function is called whenever a user navigates to this page.
        ready: function (element, options) {
            this._hub = element.querySelector(".win-hub").winControl;
            this._onHeaderInvokedBound = this.onHeaderInvoked.bind(this);
            window.addEventListener("headerinvoked", this._onHeaderInvokedBound);

            // Reset the scroll position from the history state.
            var oldScrollPosition = WinJS.Navigation.state && WinJS.Navigation.state.oldScrollPosition;
            if (oldScrollPosition) {
                this._hub.scrollPosition = oldScrollPosition;
            }
        },

        //navigate to deeper levels by invoking interactive headers
        onHeaderInvoked: function (ev) {
            var index = ev.detail.index;
            var section = ev.detail.section;
            var hub = this._hub;


            // Store the scroll position so it can be retrieved if user comes "back" to this page.
            WinJS.Navigation.state = WinJS.Navigation.state || {};
            WinJS.Navigation.state.oldScrollPosition = hub.scrollPosition;

            //check that the correct section is invoked
            if (index === 2) {
                WinJS.Navigation.navigate("/pages/listview.html");
            }

            if (index === 3) {
                WinJS.Navigation.navigate("/pages/video.html");
            }
        },

        unload: function () {
            // Respond to navigations away from this page.
            window.removeEventListener("headerinvoked", this._onHeaderInvokedBound);

        },

        myData: {
            get: function () {
                return myData;
            }
        }

    });
})();

