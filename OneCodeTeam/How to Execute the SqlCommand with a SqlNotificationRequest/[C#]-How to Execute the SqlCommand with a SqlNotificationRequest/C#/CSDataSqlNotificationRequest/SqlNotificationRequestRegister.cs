/****************************** Module Header ******************************\
Module Name:  SqlNotificationRequestRegister.cs
Project:      CSDataSqlNotificationRequest
Copyright (c) Microsoft Corporation.

After executing a command on database to get data, the data may be changed by 
the other clients. If the application needs the latest data, it needs a notification 
from the server. In this application, we will demonstrate how to execute the 
SqlCommand with a SqlNotificationRequest.
We can use this class to create a SqlNotificationRequest.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/


using System;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace CSDataSqlNotificationRequest
{
    public class SqlNotificationRequestRegister
    {
        private readonly String serviceName;
        private readonly Int32 notificationTimeout;

        private readonly String connectionString;
        private readonly String listenSql;


        private SqlNotificationRequest request;
        private Task listenTask;
        private SqlCommand cmd = null;

        // This event will be invoked after the data is changed.
        public event EventHandler OnChanged;

        public SqlNotificationRequestRegister(String listenSqlStrig, String service, Int32 timeout,
            String connString)
        {
            listenSql = listenSqlStrig;
            serviceName = service;
            notificationTimeout = timeout;
            connectionString = connString;

            RegisterSqlNotificationRequest();
        }

        /// <summary>
        /// Begin to monitor the Service Broker queue
        /// </summary>
        public void StartSqlNotification()
        {
            listenTask = new Task(Listen);

            listenTask.Start();
        }

        /// <summary>
        /// Create a SqlNotificationRequest and invoke the event.
        /// </summary>
        private void RegisterSqlNotificationRequest()
        {
            request = new SqlNotificationRequest();
            request.UserData = new Guid().ToString();
            request.Options = String.Format("Service={0};", serviceName);
            request.Timeout = notificationTimeout;

            if (OnChanged != null)
            {
                OnChanged(this, null);
            }
        }

        /// <summary>
        /// Monitoring the Service Broker queue.
        /// </summary>
        private void Listen()
        {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (cmd = new SqlCommand(listenSql, conn))
                    {
                        if (conn.State != ConnectionState.Open)
                        {
                            conn.Open();
                        }

                        cmd.CommandTimeout = notificationTimeout + 120;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                for (int i = 0; i <= reader.FieldCount - 1; i++)
                                    Debug.WriteLine(reader[i].ToString());
                            }
                        }
                    }
                }

            RegisterSqlNotificationRequest();
        }

        public void StopSqlNotification()
        {
            if (cmd != null)
            {
                cmd.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(Boolean disposed)
        {
            if (disposed)
            {
                StopSqlNotification();
            }
        }

        public SqlNotificationRequest NotificationRequest
        { get { return request; } }
    }
}
