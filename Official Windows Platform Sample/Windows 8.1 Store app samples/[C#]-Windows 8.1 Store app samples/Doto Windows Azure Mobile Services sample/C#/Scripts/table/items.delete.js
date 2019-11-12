// Copyright (c) Microsoft Corporation. All rights reserved

var items = tables.getTable('items');
var listMembers = tables.getTable('listMembers');

var HttpStatusBadRequest = 400;
var HttpStatusNotFound = 404;

function del(id, user, context) {

    // Find the item being deleted
    items.where({ id: id }).read({
        success: function (results) {
            if (results.length > 0) {
                var item = results[0];

                // Check if the user doing the deletion is a member
                // of the list that the item belongs to
                listMembers
                    .where({ userId: user.userId, listId: item.listId })
                    .read({
                        success: function (results) {
                            if (results.length > 0) {

                                // The user is a member, do the deletion
                                context.execute();
                            } else {
                                context.respond(HttpStatusBadRequest,
                                    'You cannot delete an item if you are not a member of the list containing that item.');
                            }
                        }
                    });
            } else {

                // They are trying to delete a non-existent item
                context.respond(HttpStatusNotFound);
            }
        }
    });
}