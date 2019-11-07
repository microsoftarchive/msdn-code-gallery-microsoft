/****************************** Module Header ******************************\
* Module Name:    ChatRoom.cs
* Project:        CSASPNETAJAXWebChat
* Copyright (c) Microsoft Corporation
*
* The project illustrates how to design a simple AJAX web chat application. 
* We use jQuery, ASP.NET AJAX at client side and Linq to SQL at server side.
* In this sample, we could create a chat room and invite someone
* else to join in the room and start to chat.
* 
* In this file, we create a DataContract class which used to serialize the
* ChatRoom data to the client side.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
\*****************************************************************************/

using System;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace WebChat.Logic
{
    [DataContract]
    public class ChatRoom
    {
        [DataMember]
        public Guid RoomID { get; private set; }
        [DataMember]
        public string RoomName { get; private set; }
        [DataMember]
        public int MaxUser { get; private set; }
        [DataMember]
        public int CurrentUser { get; private set; }

        public ChatRoom(Guid id)
        {
            WebChat.Data.SessionDBDataContext db = new Data.SessionDBDataContext();
            var room = db.tblChatRooms.SingleOrDefault(r => r.ChatRoomID == id);
            if (room != null)
            {
                RoomID = id;
                RoomName = room.ChatRoomName;
                MaxUser = room.MaxUserNumber;
                CurrentUser = room.tblTalkers.Count(t => t.CheckOutTime == null);
            }
        }

    }
}