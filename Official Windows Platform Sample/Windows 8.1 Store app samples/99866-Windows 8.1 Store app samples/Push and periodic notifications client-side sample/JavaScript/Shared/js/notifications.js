//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {

    var appTileIdKey = "appTileIds";
    var mainAppTileKey = "mainAppTile";
    var daysToRenew = 15 * 24 * 60 * 60 * 1000; // Renew if older than 15 days

    WinJS.Namespace.define("SampleNotifications", {
        Notifier: WinJS.Class.define(function () {
            var appTileIds;
            try {
                appTileIds = JSON.parse(Windows.Storage.ApplicationData.current.localSettings.values[appTileIdKey]);
            } catch (ex) { }

            if (appTileIds) {
                var that = this;
                appTileIds.forEach(function (itemId) {
                    try {
                        that._urls[itemId] = JSON.parse(Windows.Storage.ApplicationData.current.localSettings.values[itemId]);
                    } catch (ex) { }
                });
            }

        }, {
            _urls: {}, // Stores push notification channel URL for primary and secondary tiles in all applications for this package
            _updateUrl: function (url, channelUri, itemId, isPrimaryTile) {
                if (isPrimaryTile === undefined || isPrimaryTile === true) {
                    itemId = mainAppTileKey;
                }

                var shouldSerializeTileIds = !this._urls[itemId];
                var now = new Date();
                var storedData = { url: url, channelUri: channelUri, isAppId: isPrimaryTile, renewed: now.getTime() };
                this._urls[itemId] = storedData;

                Windows.Storage.ApplicationData.current.localSettings.values[itemId] = JSON.stringify(storedData);

                if (shouldSerializeTileIds) {
                    this._saveAppTileIds();
                }
            },
            _saveAppTileIds: function () {
                var appTileIds = [];
                for (var itemId in this._urls) {
                    appTileIds.push(itemId);
                }
                Windows.Storage.ApplicationData.current.localSettings.values[appTileIdKey] = JSON.stringify(appTileIds);
            },
            renewAllAsync: function (force) {
                var now = new Date();
                var promises = [];
                for (var itemId in this._urls) {
                    var value = this._urls[itemId];
                    if (force || (now.getTime() - value.renewed > daysToRenew)) {
                        if (itemId === mainAppTileKey) {
                            promises.push(this.openChannelAndUploadAsync(value.url));
                        } else {
                            promises.push(this.openChannelAndUploadAsync(value.url, itemId, value.isAppId));
                        }
                    }
                }

                return WinJS.Promise.join(promises);
            },
            openChannelAndUploadAsync: function (url, inputItemId, isPrimaryTile) {
                var itemId;
                if (!inputItemId) {
                    isPrimaryTile = true;
                    itemId = mainAppTileKey;
                } else {
                    itemId = inputItemId;
                }

                var that = this;
                return new WinJS.Promise(function (completed, failed, progress) {
                    // The progress handler is meant for communicating back with the UI
                    // Your code likely will not need such verbose progress updates
                    progress("Opening a channel");
                    var channelOperation;
                    if (isPrimaryTile) {
                        if (inputItemId) {
                            // Primary tile for another application in the package
                            channelOperation = Windows.Networking.PushNotifications.PushNotificationChannelManager.createPushNotificationChannelForApplicationAsync(itemId);
                        } else {
                            // Primary tile for this application
                            channelOperation = Windows.Networking.PushNotifications.PushNotificationChannelManager.createPushNotificationChannelForApplicationAsync();
                        }
                    } else {
                        // Secondary tile
                        channelOperation = Windows.Networking.PushNotifications.PushNotificationChannelManager.createPushNotificationChannelForSecondaryTileAsync(itemId);
                    }

                    channelOperation.done(function (newChannel) {
                        progress("New channel has been opened: " + newChannel.uri);

                        var tileData = that._urls[itemId];
                        // Upload the channel URI if the client hasn't recorded sending the same uri to the server
                        if (tileData && newChannel.uri === tileData.channelUri) {
                            progress("URI already uploaded");
                            that._updateUrl(url, newChannel.uri, itemId, isPrimaryTile);
                            completed(newChannel);
                        } else {
                            WinJS.xhr({
                                type: "POST",
                                url: url,
                                headers: { "Content-Type": "application/x-www-form-urlencoded" },
                                data: "channelUri=" + encodeURIComponent(newChannel.uri) + "&itemId=" + encodeURIComponent(itemId)
                            }).done(function (request) {

                                // Only update the data on the client if uploading the channel URI succeeds.
                                // If it fails, you may considered setting another background task, trying again, etc.
                                // OpenChannelAndUploadAsync will throw an exception if upload fails
                                progress("Uploaded to server. Response: " + request.response);
                                that._updateUrl(url, newChannel.uri, itemId, isPrimaryTile);
                                completed(newChannel);
                            }, failed);
                        }
                    }, failed);
                });
            }
        })
    });
})();
