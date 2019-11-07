//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    
    var urlBox;

    var page = WinJS.UI.Pages.define("/html/scenario1.html", {
        ready: function (element, options) {
            urlBox = document.getElementById("serverUrlField");
            document.getElementById("registerChannel").addEventListener("click", registerChannel, false);
            document.getElementById("closeChannel").addEventListener("click", closeChannel, false);

            if (!SampleNotifications.notifier) {
                SampleNotifications.notifier = new SampleNotifications.Notifier();
            }
        }
    });

    function registerChannel() {
        SampleNotifications.notifier.openChannelAndUploadAsync(urlBox.value).done(function (channel) {
            SampleNotifications.currentChannel = channel;
        }, function (error) {
            if (error.message) {
                WinJS.log && WinJS.log(error.message, "sample", "error");
            }
            else if (error.statusText)
            {
                WinJS.log && WinJS.log(error.statusText, "sample", "error");
            }
            else
            {
                WinJS.log && WinJS.log("An undefined error occurred.", "sample", "error");
            }
        }, function (progress) {
            WinJS.log && WinJS.log(progress, "sample", "status");
        });
    }

    function closeChannel() {
        if (SampleNotifications.currentChannel) {
            try {
                SampleNotifications.currentChannel.close();
                SampleNotifications.currentChannel = null;
            } catch (e) {
                WinJS.log && WinJS.log(e.message, "sample", "error");
            }
            WinJS.log && WinJS.log("Channel has been closed", "sample", "status");
        } else {
            WinJS.log && WinJS.log("No channel is open.", "sample", "error");
        }
        // It would be appropriate here to notify your server of the fact
        // the channel has been closed.
    }

})();
