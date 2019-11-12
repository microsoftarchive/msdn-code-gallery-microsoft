// Copyright (c) Microsoft Corporation. All rights reserved

var listMembers = tables.getTable('listMembers');
var invites = tables.getTable('invites');
var devices = tables.getTable('devices');
var profiles = tables.getTable('profiles');

var HttpStatusOk = 200;
var HttpStatusBadRequest = 400;
var HttpStatusForbidden = 403;
var HttpStatusNotFound = 404;

function insert(item, user, context) {
    if (item.fromUserId !== user.userId) {
        context.respond(HttpStatusBadRequest, 'You cannot pretend to be another user when you issue an invite.');
        return;
    }
    if (item.toUserId === user.userId) {
        context.respond(HttpStatusBadRequest, 'You cannot invite yourself to lists.');
        return;
    }

    // Check if the user sending the invite is a member of the list he is inviting someone to
    listMembers
        .where({ userId: user.userId, listId: item.listId })
        .read({ success: checkIsMemberOfList });

    function checkIsMemberOfList(results) {
        if (results.length === 0) {
            context.respond(HttpStatusBadRequest, 'You have to be a member of a list to invite another user to that list.');
            return;
        }

        // Check if the invitee is already a member of the list he is being invited to
        listMembers
            .where({ userId: item.toUserId, listId: item.listId })
            .read({ success: checkUserListMembership });
    }

    function checkUserListMembership(results) {
        if (results.length > 0) {
            context.respond(HttpStatusBadRequest, 'The user is already a member of that list.');
            return;
        }
        
        // The invitee is not a member of the list, but he may already have a pending invite
        invites
            .where({ toUserId: item.toUserId, listId: item.listId })
            .read({ success: checkRedundantInvite });
    }

    function checkRedundantInvite(results) {
        if (results.length > 0) {
            context.respond(HttpStatusBadRequest, 'This user already has a pending invite to this list.');
            return;
        }

        // Everything checks out, process the invite
        processInvite(results);
    }

    function processInvite(results) {
        item.approved = false;
        context.execute({
            success: function (results) {
                context.respond();
                getProfile(results);
            }
        });
    }
    
    function getProfile(results) {
        profiles.where({ userId : user.userId }).read({
            success: function(profileResults) {
                sendNotifications(results, profileResults[0]);
            }
        });
    }

    function sendNotifications(results, profile) {

        // Send push notifictions to all devices registered to 
        // the invitee
        devices.where({ userId: item.toUserId }).read({
            success: function (results) {
                results.forEach(function (device) {
                    push.wns.sendToastText02(device.channelUri, {
                        text1: 'doto',
                        text2: 'You have been invited to list "' + item.listName +
                            '" by ' + item.fromUserName + '.'
                    }, {
                        succees: function(data) {
                            console.log(data);
                        },
                        error: function (err) {

                            // The notification address for this device has expired, so
                            // remove this device. This may happen routinely as part of
                            // how push notifications work.
                            if (err.statusCode === HttpStatusForbidden || err.statusCode === HttpStatusNotFound) {
                                devices.del(device.id);
                            } else {
                                console.log("Problem sending push notification", err);
                            }
                        }
                    });
                }); 
             }
        });
    }
}

