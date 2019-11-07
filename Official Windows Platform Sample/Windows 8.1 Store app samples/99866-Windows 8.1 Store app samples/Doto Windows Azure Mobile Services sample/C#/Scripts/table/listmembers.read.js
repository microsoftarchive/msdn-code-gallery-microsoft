// Copyright (c) Microsoft Corporation. All rights reserved

function read(query, user, context) {

    // Make sure the user can only see his own memberships
    query.where({ userId: user.userId });
    context.execute();
}