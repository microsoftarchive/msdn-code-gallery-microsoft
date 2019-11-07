// Copyright (c) Microsoft Corporation. All rights reserved

var listMembers = tables.getTable('listMembers');
var HttpStatusBadRequest = 400;
var HttpStatusNotFound = 404;

function del(id, user, context) {

    // Find the list membership we are about to delete
    listMembers.where({ id: id }).read({
        success: function (results) {
            if (results.length === 0) {

                // That membership doesn't exist
                context.respond(HttpStatusNotFound, 'Cannot delete a membership that does not exist.');
                return;
            }

            var membership = results[0];
            if (membership.userId !== user.userId) {
                context.respond(HttpStatusBadRequest, 'You cannot remove another user from a list.');
                return;
            }

            // Check if this is the last user who is a member of that list
            listMembers.where({ listId: membership.listId }).read({
                success: function (results) {
                    context.execute();
                    if (results.length === 1) {
                        deleteItems(membership.listId);
                    }
                }
            });
        }
    });

    function deleteItems(listId) {
        // This user is the last user of that list, so we need to do some cleanup.

        // Delete all items that belong to that list
        var sqlItems = 'DELETE FROM items WHERE listId = ?';
        mssql.query(sqlItems, [listId], {
            error: function (error) {
                // The query might error if we have never inserted items
                // into those tables, which is fine because there is 
                // nothing to delete in that case
            }
        });

        // Delete any pending invites for that list
        var sqlInvites = 'DELETE FROM invites WHERE listId = ?';
        mssql.query(sqlInvites, [listId], {
            error: function (error) {
                // Same as above
            }
        });
    }
}