/****************************** Module Header ******************************\
* Module Name:    RoomTalker.svc.cs
* Project:        CSASPNETAJAXWebChat
* Copyright (c) Microsoft Corporation
*
* The project illustrates how to design a simple AJAX web chat application. 
* We use jQuery, ASP.NET AJAX at client side and Linq to SQL at server side.
* In this sample, we could create a chat room and invite someone
* else to join in the room and start to chat.
* 
* In this file, we create an Ajax-enabled WCF service which used to be called
* from the client side.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
\*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;
using WebChat.Data;
using WebChat.Logic;

namespace WebChat.Services
{
    [ServiceContract(Namespace = "http://CSASPNETAJAXWebChat",
        SessionMode = SessionMode.Allowed)]
    [AspNetCompatibilityRequirements(
        RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Transition
    {

        [OperationContract]
        public void CreateChatRoom(string useralias,
            string roomName,
            string password,
            int maxUser,
            bool needPassword)
        {

            if (maxUser < 2)
            {
                maxUser = 2;
            }

            Guid roomid = ChatManager.CreateChatRoom(
                roomName, password, false, maxUser, needPassword);
        }

        [OperationContract]
        public ChatRoom JoinChatRoom(string roomid, string alias)
        {
            Guid rid;
            if (Guid.TryParse(roomid, out rid))
            {
                ChatManager.JoinChatRoom(rid, HttpContext.Current, alias);
                return new ChatRoom(rid);

            }
            else
            {
                return null;
            }

        }

        [OperationContract]
        public void LeaveChatRoom(string roomid)
        {
            if (roomid == null)
                roomid = GetGUIDFromQuery(
                    HttpContext.Current.Request.UrlReferrer.Query).ToString();
            Guid rid;
            if (Guid.TryParse(roomid, out rid))
            {
                ChatManager.LeaveChatRoom(rid, HttpContext.Current);
            }
            else
            {
                return;
            }
        }

        [OperationContract]
        public List<ChatRoom> GetChatRoomList()
        {
            List<tblChatRoom> list = ChatManager.GetChatRoomList();
            List<ChatRoom> result = new List<ChatRoom>();
            foreach (tblChatRoom room in list)
            {
                result.Add(new ChatRoom(room.ChatRoomID));
            }
            return result;
        }

        [OperationContract]
        public ChatRoom GetChatRoomInfo(string RoomID)
        {
            Guid rim;
            if (Guid.TryParse(RoomID, out rim))
            {
                return new ChatRoom(rim);
            }
            else
            {
                return null;
            }

        }

        [OperationContract]
        public List<RoomTalker> GetRoomTalkerList()
        {

            List<RoomTalker> result = new List<RoomTalker>();
            Guid roomid = GetGUIDFromQuery(
                HttpContext.Current.Request.UrlReferrer.Query);
            if (roomid != Guid.Empty)
            {
                List<tblTalker> talkerList =
                    ChatManager.GetRoomTalkerList(roomid);
                foreach (tblTalker talker in talkerList)
                {
                    result.Add(new RoomTalker(talker, HttpContext.Current));
                }
            }
            return result;

        }

        [OperationContract]
        public bool SendMessage(string message)
        {
            Guid roomid = GetGUIDFromQuery(
                HttpContext.Current.Request.UrlReferrer.Query);

            if (roomid != Guid.Empty)
            {
                tblTalker talker = 
                    ChatManager.FindTalker(roomid, HttpContext.Current);
                ChatManager.SendMessage(talker, message);
                return true;
            }
            else
            {
                return false;

            }
        }

        [OperationContract]
        public List<Message> RecieveMessage()
        {
            List<Message> result = new List<Message>();
            Guid roomid = GetGUIDFromQuery(
                HttpContext.Current.Request.UrlReferrer.Query);
            if (roomid != Guid.Empty)
            {
                List<tblMessagePool> messageList =
                    ChatManager.RecieveMessage(
                        ChatManager.GetChatRoom(roomid));

                foreach (tblMessagePool msg in messageList)
                {
                    result.Add(new Message(msg, HttpContext.Current));
                }
            }
            return result;

        }

        private Guid GetGUIDFromQuery(string query)
        {
            Guid rim;
            if (string.IsNullOrEmpty(query))
            {
                return Guid.Empty;
            }
            Regex reg = new Regex(
            "i=([0-9a-z]{8}-[0-9a-z]{4}-[0-9a-z]{4}-[0-9a-z]{4}-[0-9a-z]{12})");
            string gid = reg.Match(query).Groups[1].Value;
            if (Guid.TryParse(gid, out rim))
            {
                return rim;
            }
            else
            {
                return Guid.Empty;
            }

        }
    }
}
