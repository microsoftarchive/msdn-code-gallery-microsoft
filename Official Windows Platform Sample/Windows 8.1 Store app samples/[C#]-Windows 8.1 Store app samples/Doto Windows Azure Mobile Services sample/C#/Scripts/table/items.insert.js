// Copyright (c) Microsoft Corporation. All rights reserved

var listMembers = tables.getTable('listMembers');
var devices = tables.getTable('devices');
var profiles = tables.getTable('profiles');

var HttpStatusBadRequest = 400;
var HttpStatusForbidden = 403;
var HttpStatusNotFound = 404;

function insert(item, user, context) {

    // Check if the user is a member of the list where he
    // is trying to insert an item
    listMembers.where({ 
            userId: user.userId, listId: item.listId 
        }).read({
            success: authorizeInsert
        });

    function authorizeInsert(results) {
        if (results.length === 0) {
            context.respond(HttpStatusBadRequest,
                'You are not authorized to add items to a list of which you are not a member.');
            return;
        }

        // The user is a member, add a timestamp and process
        // the insert
        item.created = new Date();
        context.execute({ 
            success: function() {

                // Return the success to the user so they don't
                // have to wait while we send the notificatons
                context.respond();
                getProfile();
            }
        });
    }
    
    function getProfile(){
        profiles.where({ userId : user.userId }).read({
            success: function(results) {
                sendNotifications(results[0]);
            }
        });
    }

    function sendNotifications(profile) {

        // We want to notify every member of the list where the item
        // was inserted of the new item. First, find the deviceId for every 
        // member of the list
        var sql =
        "select devices.id, devices.channelUri, listMembers.name from devices " +
        "inner join listMembers on devices.userId = listMembers.userId " +
        "where listMembers.listId = ? and devices.userId <> ?";
        mssql.query(sql, [item.listId, user.userId], {
            success: function (results) {
                console.log(results);
                results.forEach(function (result) {
                    push.wns.sendTileWideText05 (result.channelUri, {
                        text1: result.name,
                        text2: item.text,
                        image1src: profile.imageUri,
                    }, {
                        error: function (err) {

                            // The notification address for this device has expired, so
                            // remove this device. This may happen routinely as part of
                            // how push notifications work.
                            if (err.statusCode === HttpStatusForbidden || err.statusCode === HttpStatusNotFound) {
                                devices.del(result.id);
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