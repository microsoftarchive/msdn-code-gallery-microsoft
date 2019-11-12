/****************************** Module Header ******************************\
* Module Name:    RoomTalker.cs
* Project:        CSASPNETAJAXWebChat
* Copyright (c) Microsoft Corporation
*
* The project illustrates how to design a simple AJAX web chat application. 
* We use jQuery, ASP.NET AJAX at client side and Linq to SQL at server side.
* In this sample, we could create a chat room and invite someone
* else to join in the room and start to chat.
* 
* In this file, we create a DataContract class which used to serialize the
* talker data to the client side.
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
    public class RoomTalker
    {
        [DataMember]
        public string TalkerAlias { get; private set; }
        [DataMember]
        public string TalkerSession { get; private set; }
        [DataMember]
        public string TalkerIP { get; private set; }
        [DataMember]
        public bool IsFriend { get; private set; }

        public RoomTalker(WebChat.Data.tblTalker talker, HttpContext context)
        {
            TalkerAlias = talker.tblSession.UserAlias;
            TalkerIP = talker.tblSession.IP;
            TalkerSession = talker.tblSession.SessionID;
            IsFriend = (TalkerSession != context.Session.SessionID);
        }
    }
}