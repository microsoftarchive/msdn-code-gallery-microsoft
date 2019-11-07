// Copyright (c) Microsoft Corporation. All rights reserved

var listMembers = tables.getTable('listMembers');
var invites = tables.getTable('invites');

var HttpStatusOk = 200;
var HttpStatusBadRequest = 400;

function update(item, user, context) {
    
    invites.where({ id : item.id }).read({
        success : function (results) {
            if (results[0].toUserId !== user.userId) {
                context.respond(HttpStatusBadRequest, 'Only the invitee can accept or reject an invite.');
                return;
            }
            processInvite(item);
        }
    });
    
    function processInvite(item) {
        
        if (item.approved) {

            // If an invite is updated and marked approved, that means the 
            // invitee has accepted, so add the invitee to the desired list
            listMembers.insert({
                userId: item.toUserId,
                listId: item.listId,
                name: item.listName,
            }, 
            { 
                success: deleteInvite 
            });
        } else {

            // If an invite is updated, but approved !== true then we assume the invite
            // is being rejected. An alternative approach would have the client delete
            // the invite directly 
            deleteInvite();
        }
    }
    
    function deleteInvite() {

        // We have taken the necessary action on this invite, 
        // so delete it
        invites.del(item.id, {
            success: function () {
                context.respond(HttpStatusOk, item);
            }
        });
    }
}