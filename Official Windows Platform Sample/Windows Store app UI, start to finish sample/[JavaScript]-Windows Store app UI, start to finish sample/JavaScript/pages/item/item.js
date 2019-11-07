/*
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
*/
(function () {
    "use strict";

    var lView;

    WinJS.UI.Pages.define("/pages/item/item.html", {

        processed: function (element) {

            lView = element.querySelector("#itemList").winControl;
            return WinJS.Resources.processAll(element);
        },

        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
            var item = options.item;
            WinJS.Binding.processAll(element, item);

            // TODO: Initialize the page here.

            var itemList = element.querySelector("#itemList").winControl;
            itemList.itemDataSource = ControlsData.createFilteredControlList(item.groupKey).dataSource;
       
        },

        itemInvoked: WinJS.UI.eventHandler(function (eventInfo) {
            lView.itemDataSource.itemFromIndex(eventInfo.detail.itemIndex).done(function completed(item) {
                WinJS.Navigation.navigate(item.data.target, {item: item.data});
            });
            

        }),

        
    });
})();
