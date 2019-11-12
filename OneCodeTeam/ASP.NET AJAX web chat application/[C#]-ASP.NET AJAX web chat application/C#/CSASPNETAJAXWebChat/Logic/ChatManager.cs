/****************************** Module Header ******************************\
* Module Name:    ChatManager.cs
* Project:        CSASPNETAJAXWebChat
* Copyright (c) Microsoft Corporation
*
* The project illustrates how to design a simple AJAX web chat application. 
* We use jQuery, ASP.NET AJAX at client side and Linq to SQL at server side.
* In this sample, we could create a chat room and invite someone
* else to join in the room and start to chat.
* 
* In this file, we use Linq to control the data in the database.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
\*****************************************************************************/

using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using WebChat.Data;

namespace WebChat.Logic
{
    public class ChatManager
    {
        #region Send & Recieve Message

        public static bool SendMessage(tblTalker talker, string message)
        {
            try
            {
                SessionDBDataContext db = new SessionDBDataContext();
                tblMessagePool msgpool = new tblMessagePool();
                msgpool.message = message;
                msgpool.SendTime = DateTime.Now;
                msgpool.talkerID = talker.TalkerID;
                db.tblMessagePools.InsertOnSubmit(msgpool);
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static List<tblMessagePool> RecieveMessage(tblChatRoom room)
        {
            SessionDBDataContext db = new SessionDBDataContext();
            if (db.tblMessagePools.Count(
                msg => room.tblTalkers.Contains(msg.tblTalker)) > 0)
            {
                return (from messages in db.tblMessagePools
                        where messages.tblTalker.ChatRoomID == room.ChatRoomID
                        select messages).ToList();
            }
            else
            {
                return null;
            }
        }

        private static void TryToDeleteChatMessageList(Guid roomid)
        {
            SessionDBDataContext db = new SessionDBDataContext();
            var chatroom = GetChatRoom(roomid);
            if (chatroom.tblTalkers.Count(t => t.CheckOutTime == null) == 0)
            {
                var list = from m in db.tblMessagePools
                           where m.tblTalker.ChatRoomID == roomid
                           select m;
                db.tblMessagePools.DeleteAllOnSubmit(list);
                db.SubmitChanges();
            }
        }

        #endregion

        #region ChatRoom Management

        public static Guid CreateChatRoom(string roomName, string password,
            bool isLock, int maxUserNumber, bool needPassword)
        {
            SessionDBDataContext db = new SessionDBDataContext();
            tblChatRoom room = new tblChatRoom();
            room.ChatRoomID = Guid.NewGuid();
            room.ChatRoomName = roomName;
            room.ChatRoomPassword = password;
            room.IsLock = isLock;
            room.MaxUserNumber = maxUserNumber;
            room.NeedPassword = needPassword;
            db.tblChatRooms.InsertOnSubmit(room);
            db.SubmitChanges();
            return room.ChatRoomID;
        }

        public static tblChatRoom GetChatRoom(Guid roomid)
        {
            SessionDBDataContext db = new SessionDBDataContext();
            return db.tblChatRooms.SingleOrDefault(r => r.ChatRoomID == roomid);

        }

        public static bool IsRoomFull(Guid roomID)
        {
            SessionDBDataContext db = new SessionDBDataContext();
            var rsl = db.tblChatRooms.Single(room => room.ChatRoomID == roomID);
            if (rsl != null)
            {
                return rsl.MaxUserNumber == rsl.tblTalkers.Count(
                    t => t.CheckOutTime == null);
            }
            else
            {
                return false;
            }

        }

        public static List<tblChatRoom> GetChatRoomList()
        {
            SessionDBDataContext db = new SessionDBDataContext();
            return db.tblChatRooms.ToList();
        }

        public static bool JoinChatRoom(Guid ChatRoomID, HttpContext context,
            string alias)
        {
            if (!ChatManager.IsRoomFull(ChatRoomID))
            {
                SessionDBDataContext db = new SessionDBDataContext();
                if (db.tblSessions.Count(
                    s => s.SessionID == context.Session.SessionID) == 0)
                {
                    ChatManager.CreateSession(context, alias);
                }
                var session = ChatManager.GetSession(context);
                if (db.tblTalkers.Count(t => t.ChatRoomID == ChatRoomID && 
                    t.SessionID == session.UID && t.CheckOutTime == null) > 0)
                {
                    return false;
                }
                else
                {
                    tblTalker talker = new tblTalker();
                    talker.ChatRoomID = ChatRoomID;
                    talker.CheckInTime = DateTime.Now;
                    talker.CheckOutTime = null;
                    talker.SessionID = session.UID;
                    db.tblTalkers.InsertOnSubmit(talker);
                    db.SubmitChanges();
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public static tblTalker FindTalker(Guid ChatRoomID, HttpContext context)
        {
            SessionDBDataContext db = new SessionDBDataContext();
            var rsl = db.tblTalkers.FirstOrDefault(
                t => t.ChatRoomID == ChatRoomID && 
                t.SessionID == ChatManager.GetSession(context).UID);
            return rsl;

        }

        public static List<tblTalker> GetRoomTalkerList(Guid ChatRoomID)
        {
            SessionDBDataContext db = new SessionDBDataContext();
            var rsl = from d in db.tblTalkers
                      where d.CheckOutTime == null && d.ChatRoomID == ChatRoomID
                      select d;
            return rsl.ToList();

        }

        public static void LeaveChatRoom(Guid ChatRoomID, HttpContext context)
        {
            SessionDBDataContext db = new SessionDBDataContext();
            tblSession session = ChatManager.GetSession(context);
            if (session != null)
            {
                var talker = db.tblTalkers.FirstOrDefault(
                    t => t.ChatRoomID == ChatRoomID &&
                    t.SessionID == session.UID && t.CheckOutTime == null);

                if (talker != null)
                {
                    talker.CheckOutTime = DateTime.Now;
                    db.SubmitChanges();
                }
            }
            TryToDeleteChatMessageList(ChatRoomID);
        }
        #endregion

        #region Chat Session Management

        public static tblSession GetSession(HttpContext context)
        {
            SessionDBDataContext db = new SessionDBDataContext();
            var session = db.tblSessions.FirstOrDefault(
                s => s.SessionID == context.Session.SessionID);
            return session;
        }

        public static bool SessionExist(HttpContext context)
        {
            return ChatManager.GetSession(context) != null;
        }

        public static bool CreateSession(HttpContext context,
            string userAlias)
        {
            try
            {
                SessionDBDataContext db = new SessionDBDataContext();

                tblSession session = new tblSession();
                session.SessionID = context.Session.SessionID;
                session.IP = context.Request.UserHostAddress;
                if (string.IsNullOrEmpty(userAlias))
                    userAlias = session.IP;
                session.UserAlias = userAlias;
                db.tblSessions.InsertOnSubmit(session);
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}