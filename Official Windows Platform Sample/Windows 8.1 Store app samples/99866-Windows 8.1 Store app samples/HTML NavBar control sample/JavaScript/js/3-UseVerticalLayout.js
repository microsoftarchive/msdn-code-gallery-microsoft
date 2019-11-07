//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/3-UseVerticalLayout.html", {
        init: function (element, options) {
            var categoryNames = ["Picks for you", "Popular", "New Releases", "Top Paid", "Top Free",
            "Games", "Social", "Entertainment", "Photo", "Music & Video",
            "Sports", "Books & Reference", "News & Weather", "Health & Fitness", "Food & Dining",
            "Lifestyle", "Shopping", "Travel", "Finance", "Productivity",
            "Tools", "Security", "Business", "Education", "Government"];

            var categoryItems = [];
            for (var i = 0; i < categoryNames.length; i++) {
                categoryItems[i] = {
                    label: categoryNames[i]
                };
            }

            Data.categoryList = new WinJS.Binding.List(categoryItems);
        },

        ready: function (element, options) {
            // The NavBar instantiates its children on the WinJS scheduler as an idle job to help
            // startup and navigation performance. Once the NavBar has instantiated its children
            // we can set the layout.
            document.querySelector('#useVerticalLayout').addEventListener('childrenprocessed', changeLayout);

            window.addEventListener('resize', changeLayout);
            document.body.querySelector('#useVerticalLayout').addEventListener('invoked', this.navbarInvoked.bind(this));
            document.getElementById("switchWidth").addEventListener("click", switchWidth, false);
        },

        navbarInvoked: function (ev) {
            var navbarCommand = ev.detail.navbarCommand;
            WinJS.log && WinJS.log(navbarCommand.label + " NavBarCommand invoked", "sample", "status");
            document.querySelector('select').focus();
        },

        unload: function () {
            window.removeEventListener('resize', changeLayout);
        }
    });

    function changeLayout() {
        // First check to make sure that the NavBar instantiated the NavBarContainer since it loads it asynchronously.
        var globalNavBarContainer = document.body.querySelector('#useVerticalLayout .globalNav').winControl;
        if (globalNavBarContainer) {
            var orientation = window.innerWidth > 500 ? "horizontal" : "vertical";
            var categoryNavBarContainer = document.body.querySelector('#useVerticalLayout .categoryNav').winControl;

            if (globalNavBarContainer.layout !== orientation) {
                globalNavBarContainer.layout = orientation;
            }
            if (categoryNavBarContainer.layout !== orientation) {
                categoryNavBarContainer.layout = orientation;
            }
        }
    }

    function switchWidth() {
        var globalNavBarContainer = document.body.querySelector('#useVerticalLayout .globalNav').winControl;
        var categoryNavBarContainer = document.body.querySelector('#useVerticalLayout .categoryNav').winControl;

        if ((globalNavBarContainer.fixedSize === false) && (categoryNavBarContainer.fixedSize === false)) {
            globalNavBarContainer.fixedSize = true;
            categoryNavBarContainer.fixedSize = true;
            document.getElementById("switchWidth").innerText = "Switch to dynamic width";
        }
        else {
            globalNavBarContainer.fixedSize = false;
            categoryNavBarContainer.fixedSize = false;
            document.getElementById("switchWidth").innerText = "Switch to fixed width";
        }
    }
})();
