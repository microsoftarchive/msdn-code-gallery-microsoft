//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario2.html", {
        ready: function (element, options) {
            WinJS.UI.processAll().done(function () {
                document.getElementById("removeItem").addEventListener("click", removeFromScheduleClick, false);
                document.getElementById("refreshList").addEventListener("click", refreshListView, false);

                listView = document.getElementById("itemList").winControl;
                refreshListView();
            });
        }
    });

    var Notifications = Windows.UI.Notifications;
    var listView;

    // Create a list view representation of all the scheduled tiles and toasts
    function refreshListView() {
        var scheduledToasts = Notifications.ToastNotificationManager.createToastNotifier().getScheduledToastNotifications();
        var scheduledTiles = Notifications.TileUpdateManager.createTileUpdaterForApplication().getScheduledTileNotifications();

        var toastLength = scheduledToasts.length;
        var tileLength = scheduledTiles.length;
        var bindingList = new WinJS.Binding.List();

        for (var i = 0; i < toastLength; i++) {
            bindingList.push(extractData(scheduledToasts[i], false));
        }

        for (i = 0; i < tileLength; i++) {
            bindingList.push(extractData(scheduledTiles[i], true));
        }

        listView.selection.clear().done(function () {
            listView.itemDataSource = bindingList.dataSource;
        });
    }

    // Helper function to take data from toast and turn it into an object for binding
    function extractData(notification, isTile) {
        // Notice that notification ID is stored, and not a reference to the notification
        return {
            itemType: isTile ? "Tile" : "Toast",
            itemId: notification.id,
            dueTime: notification.deliveryTime.toLocaleTimeString(),
            inputString: notification.content.getElementsByTagName("text")[0].innerText,
            isTile: isTile
        };
    }

    // Remove selected notifications from schedule
    function removeFromScheduleClick() {
        listView.selection.getItems().done(function (items) {
            items.forEach(function (iter) {
                var info = iter.data;
                removeFromSchedule(info.itemId, info.isTile);
            });
        });
        WinJS.log && WinJS.log("Selected notifications have been removed.", "sample", "status");
        refreshListView();
    }

    // Remove the notification by checking the list of scheduled notifications for a notification with matching ID.
    // While it would be possible to manage the notifications by storing a reference to each notification, such practice
    // causes memory leaks by not allowing the notifications to be collected once they have shown.
    // It's important to create unique IDs for each notification if they are to be managed later.
    function removeFromSchedule(itemId, isTile) {
        var scheduled;
        var notifier;
        if (isTile) {
            notifier = Notifications.TileUpdateManager.createTileUpdaterForApplication();
            scheduled = notifier.getScheduledTileNotifications();
        } else {
            notifier = Notifications.ToastNotificationManager.createToastNotifier();
            scheduled = notifier.getScheduledToastNotifications();
        }

        for (var i = 0, len = scheduled.length; i < len; i++) {
            if (scheduled[i].id === itemId) {
                notifier.removeFromSchedule(scheduled[i]);
            }
        }
    }

})();
