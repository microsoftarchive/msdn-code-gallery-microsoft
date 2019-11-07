//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/2-UseData.html", {
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
            document.body.querySelector('#useTemplate').addEventListener('invoked', this.navbarInvoked.bind(this));
        },

        navbarInvoked: function (ev) {
            var navbarCommand = ev.detail.navbarCommand;
            WinJS.log && WinJS.log(navbarCommand.label + " NavBarCommand invoked", "sample", "status");
            document.querySelector('select').focus();
        }
    });
})();