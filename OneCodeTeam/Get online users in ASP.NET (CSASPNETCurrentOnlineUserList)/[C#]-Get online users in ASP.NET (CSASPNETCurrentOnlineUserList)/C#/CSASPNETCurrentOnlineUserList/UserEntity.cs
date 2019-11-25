/**************************** Module Header ********************************\
* Module Name:    UserEntity.cs
* Project:        CSASPNETCurrentOnlineUserList
* Copyright (c) Microsoft Corporation
*
* The Membership.GetNumberOfUsersOnline Method can get the number of online
* users,however many asp.net projects are not using membership.This project
* shows how to display a list of current online users' information without 
* using membership provider.

* This class is used as user's entity. 
*

* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
\***************************************************************************/

using System;
using System.Text;

namespace CSASPNETCurrentOnlineUserList
{
    public class UserEntity
    {
        public UserEntity()
        { }
        // Ticket.
        private string _ticket;

        // UserName.
        private string _username;

        // TrueName.
        private string _truename;

        // Role.
        private string _roleid;

        // Last refresh time of page.
        private DateTime _refreshtime;

        // Last active time of user.
        private DateTime _activetime;

        // Ip address of user.
        private string _clientip;    

        public string Ticket
        {
            get
            {
                return _ticket;
            }
            set
            {
                _ticket = value;
            }
        }
        public string UserName
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
            }
        }
        public string TrueName
        {
            get
            {
                return _truename;
            }
            set
            {
                _truename = value;
            }
        }
        public string RoleID
        {
            get
            {
                return _roleid;
            }
            set
            {
                _roleid = value;
            }
        }
        public DateTime RefreshTime
        {
            get
            {
                return _refreshtime;
            }
            set
            {
                _refreshtime = value;
            }
        }
        public DateTime ActiveTime
        {
            get
            {
                return _activetime;
            }
            set
            {
                _activetime = value;
            }
        }
        public string ClientIP
        {
            get
            {
                return _clientip;
            }
            set
            {
                _clientip = value;
            }
        }
    }
}