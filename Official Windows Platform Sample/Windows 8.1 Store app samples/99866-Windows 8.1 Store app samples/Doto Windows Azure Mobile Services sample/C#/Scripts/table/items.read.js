// Copyright (c) Microsoft Corporation. All rights reserved

var listMembers = tables.getTable('listMembers');

function read(query, user, context) {

    // We want to make sure the user can only see an item
    // if he is a member of the list that item belongs to. 
    // First, get the listIds of the lists that the user is
    // a member of.
    listMembers
        .where({ userId: user.userId })
        .select('listId')
        .read({
            success: function (results) {

                // Attach an extra clause to the user's query 
                // that forces it to look only in lists the user
                // is a member of
                query.where(function (members) {
                    return this.listId in members;
                }, results);
                context.execute();
            }
       });
}