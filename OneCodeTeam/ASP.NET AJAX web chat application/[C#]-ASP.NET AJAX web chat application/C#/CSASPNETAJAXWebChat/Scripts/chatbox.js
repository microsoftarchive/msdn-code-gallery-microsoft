/****************************** Module Header ******************************\
* Module Name:    chatbox.js
* Project:        CSASPNETAJAXWebChat
* Copyright (c) Microsoft Corporation
*
* The project illustrates how to design a simple AJAX web chat application. 
* We use jQuery, ASP.NET AJAX at client side and Linq to SQL at server side.
* In this sample, we could create a chat room and invite someone
* else to join in the room and start to chat.
* 
* In this file, we define some JavaScript function to open a popup window
* which is used to load a chat room.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
\*****************************************************************************/

Type.registerNamespace('WebChat');
WebChat.ChatBox = function () {
    WebChat.ChatBox.initializeBase(this);
    this._title = "";
    this._iframe = null;
    this._element = null;
    this._roomid = null;

}
var lockBox = new Array();
WebChat.ChatBox.prototype = {
    get_title: function () {
        return this._title;
    },
    set_title: function (val) {
        this._title = val;
    },
    get_roomid: function () {
        return this._roomid;
    },
    set_roomid: function (val) {
        this._roomid = val;
    },
    open: function (RoomId, RoomName) {
        if ($.inArray(RoomId, lockBox) == -1) {
            this._roomid = RoomId;
            this._title = RoomName;
            this._element = document.createElement("DIV");
            this._element.style.display = "none";
            this._iframe = document.createElement("IFRAME");
            this._iframe.src = "chatbox.aspx?i=" + RoomId;
            this._iframe.style.width = "400px";
            this._iframe.frameBorder = 0;
            this._iframe.style.height = "260px";
            this._iframe.scrolling = "no";
            this._element.appendChild(this._iframe);
            document.body.appendChild(this._element);
            var obj = this;
            $(this._element).dialog({ show: 'slide', title: this._title, width: 430, beforeClose: function () { obj.quit(obj._roomid); } });
            lockBox.push(RoomId);
            $addHandler(window, "unload", function () {
                obj.quit(obj._roomid);
            });
            $("#_cu_" + RoomId).html(Number($("#_cu_" + RoomId).html()) + 1);
        }
        else {
            ShowMessageBox("Exception", "You have joined the chat room");
        }
    },
    quit: function (roomid) {
        ShowMessageBox("Leave Chat Room", "Please waiting...");
        csaspnetajaxwebchat.transition.LeaveChatRoom(roomid, function () {
            lockBox = $.map(lockBox, function (n) {
                return n != roomid ? n : null;
            });

            $("#_cu_" + roomid).html(Number($("#_cu_" + roomid).html()) - 1);
            CloseMessageBox();
        });

    }
}

