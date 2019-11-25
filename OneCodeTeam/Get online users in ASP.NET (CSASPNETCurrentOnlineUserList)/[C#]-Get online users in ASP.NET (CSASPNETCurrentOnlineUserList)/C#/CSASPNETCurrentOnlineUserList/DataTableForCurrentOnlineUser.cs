/**************************** Module Header ********************************\
* Module Name:    DataTableForCurrentOnlineUser.cs
* Project:        CSASPNETCurrentOnlineUserList
* Copyright (c) Microsoft Corporation
*
* The Membership.GetNumberOfUsersOnline Method can get the number of online
* users,however many asp.net projects are not using membership.This project
* shows how to display a list of current online users' information without 
* using membership provider.

* This class is used to initialize the datatable which store the information
* of current online users.

* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
\***************************************************************************/

using System;
using System.Text;
using System.Data;
using System.Configuration;

namespace CSASPNETCurrentOnlineUserList
{
    public class DataTableForCurrentOnlineUser
    {
        private static DataTable _activeusers;
        private int _activeTimeout;
        private int _refreshTimeout;
        /// <summary>
        /// Initialization of UserOnlineTable.
        /// </summary> 
        private void UsersTableFormat()
        {
            if (_activeusers == null)
            {
                _activeusers = new DataTable("ActiveUsers");
                DataColumn myDataColumn;
                System.Type mystringtype;
                mystringtype = System.Type.GetType("System.String");
                System.Type mytimetype;
                mytimetype = System.Type.GetType("System.DateTime");
                myDataColumn = new DataColumn("Ticket", mystringtype);
                _activeusers.Columns.Add(myDataColumn);
                myDataColumn = new DataColumn("UserName", mystringtype);
                _activeusers.Columns.Add(myDataColumn);
                myDataColumn = new DataColumn("TrueName", mystringtype);
                _activeusers.Columns.Add(myDataColumn);
                myDataColumn = new DataColumn("RoleID", mystringtype);
                _activeusers.Columns.Add(myDataColumn);
                myDataColumn = new DataColumn("RefreshTime", mytimetype);
                _activeusers.Columns.Add(myDataColumn);
                myDataColumn = new DataColumn("ActiveTime", mytimetype);
                _activeusers.Columns.Add(myDataColumn);
                myDataColumn = new DataColumn("ClientIP", mystringtype);
                _activeusers.Columns.Add(myDataColumn);
            }
        }

        public DataTableForCurrentOnlineUser()
        {
            // Initialize the datatable which used to store the information
            // of current online user.
            UsersTableFormat();

            // Initialization of User's active time(minute).
            try
            {
                _activeTimeout = int.Parse(ConfigurationManager.AppSettings["ActiveTimeout"]);
            }
            catch
            {
                _activeTimeout = 60;
            }

            // Initialization of refresh time(minute).
            try
            {
                _refreshTimeout = int.Parse(ConfigurationManager.AppSettings["RefreshTimeout"]);
            }
            catch
            {
                _refreshTimeout = 1;
            }
        }
        public DataTable ActiveUsers
        {
            get { return _activeusers.Copy(); }
        }

        /// <summary>
        /// Sign in method.
        /// </summary>
        public void Login(UserEntity user, bool singleLogin)
        {
            // Clear the record of user who is off line.
            DelTimeOut();
            if (singleLogin)
            {
                // Let the user who is already login sign out.
                this.Logout(user.UserName, false);
            }
            DataRow _myrow;
            try
            {
                _myrow = _activeusers.NewRow();
                _myrow["Ticket"] = user.Ticket.Trim();
                _myrow["UserName"] = user.UserName.Trim();
                _myrow["TrueName"] = "" + user.TrueName.Trim();
                _myrow["RoleID"] = "" + user.RoleID.Trim();
                _myrow["ActiveTime"] = DateTime.Now;
                _myrow["RefreshTime"] = DateTime.Now;
                _myrow["ClientIP"] = user.ClientIP.Trim();
                _activeusers.Rows.Add(_myrow);
            }
            catch
            {
                throw;
            }
            _activeusers.AcceptChanges();

        }

        /// <summary>
        /// Sign out the user，depend on ticket or username.
        /// </summary> 
        private void Logout(string strUserKey, bool byTicket)
        {
            // Clear the record of user who is off line.
            DelTimeOut();
            strUserKey = strUserKey.Trim();
            string _strExpr;
            _strExpr = byTicket ? "Ticket='" + strUserKey + "'" : "UserName='" + strUserKey + "'";
            DataRow[] _curuser;
            _curuser = _activeusers.Select(_strExpr);
            if (_curuser.Length > 0)
            {
                for (int i = 0; i < _curuser.Length; i++)
                {
                    _curuser[i].Delete();
                }
            }
            _activeusers.AcceptChanges();
        }

        /// <summary>
        /// Sign out the user depend on ticket.
        /// </summary>
        /// <param name="strTicket">the ticket of user</param>
        public void Logout(string strTicket)
        {
            this.Logout(strTicket, true);
        }

        /// <summary>
        /// Clear the record of user who is off line.
        /// </summary>
        private bool DelTimeOut()
        {
            string _strExpr;
            _strExpr = "ActiveTime < '" + DateTime.Now.AddMinutes(0 - _activeTimeout) +
                "'or RefreshTime < '" + DateTime.Now.AddMinutes(0 - _refreshTimeout) + "'";
            DataRow[] _curuser;
            _curuser = _activeusers.Select(_strExpr);
            if (_curuser.Length > 0)
            {
                for (int i = 0; i < _curuser.Length; i++)
                {
                    _curuser[i].Delete();
                }
            }
            _activeusers.AcceptChanges();
            return true;
        }

        /// <summary>
        /// Update the last active time of user.
        /// </summary>
        public void ActiveTime(string strTicket)
        {
            DelTimeOut();
            string _strExpr;
            _strExpr = "Ticket='" + strTicket + "'";
            DataRow[] _curuser;
            _curuser = _activeusers.Select(_strExpr);
            if (_curuser.Length > 0)
            {
                for (int i = 0; i < _curuser.Length; i++)
                {
                    _curuser[i]["ActiveTime"] = DateTime.Now;
                    _curuser[i]["RefreshTime"] = DateTime.Now;
                }
            }
            _activeusers.AcceptChanges();
        }

        /// <summary>
        /// Update the time when the page refresh or the page get a request.
        /// </summary>
        public void RefreshTime(string strTicket)
        {
            DelTimeOut();
            string _strExpr;
            _strExpr = "Ticket='" + strTicket + "'";
            DataRow[] _curuser;
            _curuser = _activeusers.Select(_strExpr);
            if (_curuser.Length > 0)
            {
                for (int i = 0; i < _curuser.Length; i++)
                {
                    _curuser[i]["RefreshTime"] = DateTime.Now;
                }
            }
            _activeusers.AcceptChanges();
        }

        private UserEntity SingleUser(string strUserKey, bool byTicket)
        {
            strUserKey = strUserKey.Trim();
            string _strExpr;
            UserEntity user = new UserEntity();
            _strExpr = byTicket ? "Ticket='" + strUserKey + "'" : "UserName='" + strUserKey + "'";
            DataRow[] _curuser;
            _curuser = _activeusers.Select(_strExpr);
            if (_curuser.Length > 0)
            {
                user.Ticket = (string)_curuser[0]["Ticket"];
                user.UserName = (string)_curuser[0]["UserName"];
                user.TrueName = (string)_curuser[0]["TrueName"];
                user.RoleID = (string)_curuser[0]["RoleID"];
                user.ActiveTime = (DateTime)_curuser[0]["ActiveTime"];
                user.RefreshTime = (DateTime)_curuser[0]["RefreshTime"];
                user.ClientIP = (string)_curuser[0]["ClientIP"];
            }
            else
            {
                user.UserName = "";
            }
            return user;
        }

        /// <summary>
        /// Search the user by ticket.
        /// </summary>
        public UserEntity SingleUser_byTicket(string strTicket)
        {
            return this.SingleUser(strTicket, true);
        }

        /// <summary>
        /// Search the user by username.
        /// </summary>
        public UserEntity SingleUser_byUserName(string strUserName)
        {
            return this.SingleUser(strUserName, false);
        }

        /// <summary>
        /// Check whether the user is online by using ticket.
        /// </summary>
        public bool IsOnline_byTicket(string strTicket)
        {
            return (bool)(this.SingleUser(strTicket, true).UserName != "");
        }

        /// <summary>
        /// Check whether the user is online by using username.
        /// </summary>
        public bool IsOnline_byUserName(string strUserName)
        {
            return (bool)(this.SingleUser(strUserName, false).UserName != "");
        }
    }
}