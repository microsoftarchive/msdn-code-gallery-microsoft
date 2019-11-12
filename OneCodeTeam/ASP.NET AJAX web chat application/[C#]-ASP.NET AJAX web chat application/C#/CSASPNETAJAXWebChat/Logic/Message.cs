/****************************** Module Header ******************************\
* Module Name:    Message.cs
* Project:        CSASPNETAJAXWebChat
* Copyright (c) Microsoft Corporation
*
* The project illustrates how to design a simple AJAX web chat application. 
* We use jQuery, ASP.NET AJAX at client side and Linq to SQL at server side.
* In this sample, we could create a chat room and invite someone
* else to join in the room and start to chat.
* 
* In this file, we create a DataContract class which used to serialize the
* Message data to the client side.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
\*****************************************************************************/

using System;
using System.Web;
using System.Runtime.Serialization;

namespace WebChat.Logic
{
    [DataContract]
    public class Message
    {
        [DataMember]
        public string Talker { get; private set; }
        [DataMember]
        public string MessageData { get; private set; }
        [DataMember]
        public DateTime SendTime { get; private set; }
        
        [DataMember]
        public bool IsFriend { get; private set; }

        public Message(WebChat.Data.tblMessagePool message,HttpContext session)
        {
            Talker = message.tblTalker.tblSession.UserAlias;
            MessageData = message.message;
            SendTime = message.SendTime;
            IsFriend = (message.tblTalker.tblSession.SessionID != session.Session.SessionID);
        }
    }
}