// Copyright (c) Microsoft Corporation. All rights reserved

var HttpStatusOk = 200;
var HttpStatusBadRequest = 400;

function insert(item, user, request) {

    // We don't trust the client, we always set the user on the server
    item.userId = user.userId;

    // Require an installationId
    if (!item.installationId || item.installationId.length === 0) {
        request.respond(HttpStatusBadRequest, "InstallationId is required");
        return;
    }

    // Find any records that match this device already (user and installationId combo)
    var devices = tables.getTable('devices');
    devices.where({
        userId: item.userId,
        installationId: item.installationId
    }).read({
        success: function (results) {
            if (results.length == 1) {

                // This device already exists, so don't insert the new entry,
                // update the channelUri (if it's different)
                if (item.channelUri === results[0].channelUri) {
                    request.respond(HttpStatusOk, results[0]);
                    return;
                }

                // Otherwise, update the notification id 
                results[0].channelUri = item.channelUri;
                devices.update(results[0], {
                    success: function () {
                        request.respond(HttpStatusOk, results[0]);
                        return;
                    }
                });
            } else {
                request.execute();
            }
        }
    });
}