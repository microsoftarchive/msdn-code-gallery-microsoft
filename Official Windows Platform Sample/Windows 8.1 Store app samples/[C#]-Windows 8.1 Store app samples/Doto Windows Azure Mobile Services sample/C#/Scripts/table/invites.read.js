// Copyright (c) Microsoft Corporation. All rights reserved

function read(query, user, context) {

    // Make sure invitees can only see invites directed to them
    query.where({ toUserId: user.userId });
    context.execute();
}