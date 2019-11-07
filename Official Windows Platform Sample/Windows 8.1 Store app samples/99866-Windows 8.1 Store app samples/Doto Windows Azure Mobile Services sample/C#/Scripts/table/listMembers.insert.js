// Copyright (c) Microsoft Corporation. All rights reserved

var HttpStatusBadRequest = 400;

function insert(item, user, context) {
    if (item.listId) {
        context.respond(HttpStatusBadRequest, 'Use insert only to create new lists. To add a new user to an ' +
		'existing list, they have to be invited and they will be added as soon as they ' +
		'accept the invite.');
    }

    // Doto doesn't have a table to centrally store lists. For simplicity, the list name
    // is stored redundantly across the memberships (since the name cannot be updated, this poses no problem).
    // To generate a unique listId, we take the name, date and a random value.
    item.listId = new Date().getTime() + "-" + item.name + "-" + Math.random();
    context.execute();
}