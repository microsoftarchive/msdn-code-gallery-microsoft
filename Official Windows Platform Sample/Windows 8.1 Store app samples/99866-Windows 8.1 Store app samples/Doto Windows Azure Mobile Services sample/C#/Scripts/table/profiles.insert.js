// Copyright (c) Microsoft Corporation. All rights reserved

var profiles = tables.getTable('profiles');
var HttpStatusBadRequest = 400;

function insert(item, user, context) {

    if (!item.name && item.name.length === 0) {
        context.respond(HttpStatusBadRequest, 'A name must be provided.');
        return;
    }

    if (item.userId !== user.userId) {
        context.respond(HttpStatusBadRequest, 'A user can only insert a profile for their own userId.');
        return;
    }

    // Check if a user with the same userId already exists
    profiles.where({ userId: item.userId }).read({
        success: function (results) {
            if (results.length > 0) {
                context.respond(HttpStatusBadRequest, 'Profile already exists.');
                return;
            }

            // No such user exists, add a timestamp and process the insert
            item.created = new Date();
            context.execute();
        }
    });
}