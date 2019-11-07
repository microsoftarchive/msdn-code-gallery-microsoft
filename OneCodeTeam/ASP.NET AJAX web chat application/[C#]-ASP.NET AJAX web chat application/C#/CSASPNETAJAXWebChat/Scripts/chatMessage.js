/****************************** Module Header ******************************\
* Module Name:    chatMessage.js
* Project:        CSASPNETAJAXWebChat
* Copyright (c) Microsoft Corporation
*
* The project illustrates how to design a simple AJAX web chat application. 
* We use jQuery, ASP.NET AJAX at client side and Linq to SQL at server side.
* In this sample, we could create a chat room and invite someone
* else to join in the room and start to chat.
* 
* In this file, we define some JavaScript function to get the message list,
* to send a message, to get the talker list.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
\*****************************************************************************/

// Send message
function SendMessage(textbox) {
    if (textbox.value != "") {
        csaspnetajaxwebchat.transition.SendMessage(textbox.value, sendMessageCallBack);
        textbox.value = "";
    }
}
function sendMessageCallBack(args) {
    if (args) {
        UpdateLocalMessage();
    }
    else {
        alert(args);
    }
}

// Update message list
function UpdateLocalMessage() {
    csaspnetajaxwebchat.transition.RecieveMessage(UpdateMessageSuccessCallBack, UpdateMessageFaileCallBack);
}
function UpdateMessageFaileCallBack(args) {
    //csaspnetajaxwebchat.transition.LeaveChatRoom(null);
}
function UpdateMessageSuccessCallBack(args) {
    var container = $("#txtMessageList");
    container.html("");
    $(args).each(function (i) {
        var d = document.createElement("DIV");
        $(d)
                    .appendTo(container)
                    .addClass((this.IsFriend ? "_tlkFriend" : "_tlkMe"))
                    .end()
                    .append("<span class=\"_talker\">" + (this.IsFriend ? this.Talker : "I") + "</span>")
                    .append("<span> say at </span>")
                    .append("<span class=\"_time\">" + this.SendTime.format("MM/dd/yyyy HH:mm:ss") + "</span>")
                    .append("<span>: </span><BR /> ")
                    .append("<span class=\"_msg\">" + this.MessageData + "</span>");

    });
    container.scrollTop(container[0].scrollHeight - container.height());

    setTimeout(function () { UpdateLocalMessage(); }, 2000);
}

// Update talker list in the current chat room
function UpdateRoomTalkerList() {
    csaspnetajaxwebchat.transition.GetRoomTalkerList(UpdateRoomTalkerListSuccessCallBack);
}
function UpdateRoomTalkerListSuccessCallBack(args) {
    var lst = $("#lstUserList");
    lst.html("");
    $(args).each(function (i) {
        var l = document.createElement("OPTION");
        $(l)
                    .appendTo(lst)
                    .attr("text", (this.IsFriend ? this.TalkerAlias : "Me"))
                    .attr("value", this.TalkerSession)
                    .attr("title", this.TalkerSession);
    });

    setTimeout(function () { UpdateRoomTalkerList(); }, 2000);
}