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

    var nav = WinJS.Navigation;
    var session = WinJS.Application.sessionState;
    var util = WinJS.Utilities;
    var controlGroupsListView;
    var controlsListView;


    var hubPage = WinJS.UI.Pages.define("/pages/hub/hub.html", {
        processed: function (element) {
            return WinJS.Resources.processAll(element);
        },

        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
            var hub = element.querySelector(".hub").winControl;
            hub.onloadingstatechanged = function (args) {
                if (args.srcElement === hub.element && args.detail.loadingState === "complete") {
                    this._hubReady(hub);
                    hub.onloadingstatechanged = null;
                }
            }.bind(this);

            hub.onheaderinvoked = function (args) {
                args.detail.section.onheaderinvoked(args);
            };

            controlGroupsListView =
                element.querySelector("#controlsBycontrolGroupsListView");
            controlsListView =
                element.querySelector("#allControlsListView");

        },

        unload: function () {
            // TODO: Respond to navigations away from this page.
            session.hubScroll = document.querySelector(".hub").winControl.scrollPosition;
        },

        updateLayout: function (element) {
            /// <param name="element" domElement="true" />

            // TODO: Respond to changes in layout.
        },

        _hubReady: function (hub) {
            /// <param name="hub" type="WinJS.UI.Hub" />
            if (typeof session.hubScroll === "number") {
                hub.scrollPosition = session.hubScroll;
            }

            // TODO: Initialize the hub sections here.
        },

        itemInvoked: WinJS.UI.eventHandler(function (eventInfo) {

           
            controlsListView.winControl.itemDataSource.itemFromIndex(eventInfo.detail.itemIndex).done(function completed(item) {
                WinJS.Navigation.navigate(item.data.target, {item: item.data});
            });
            

        }),

        groupInvoked: WinJS.UI.eventHandler(function (eventInfo) {

            controlGroupsListView.winControl.itemDataSource.itemFromIndex(eventInfo.detail.itemIndex).done(function completed(item) {

                WinJS.Navigation.navigate("/pages/item/item.html", { item: item.data});
            });
            

        }),

    });

})();