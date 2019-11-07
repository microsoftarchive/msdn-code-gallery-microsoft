/****************************** Module Header ******************************\
Module Name:  RegularlyNotificationRegister.cs
Project:      CSEFAutoUpdate
Copyright (c) Microsoft Corporation.

We can use the Sqldependency to get the notification when the data is changed 
in database, but EF doesn’t have the same feature. In this sample, we will 
demonstrate how to automatically update by Sqldependency in Entity Framework.
In this sample, we will demonstrate two ways that use SqlDependency to get the 
change notification to auto update data.
We can regularly detect the changes by this class.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Data.Entity;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CSEFAutoUpdate
{
    public class RegularlyNotificationRegister<TEntity> : IDisposable
    where TEntity : class
    {
        private SqlConnection connection = null;
        private SqlCommand command = null;
        private IQueryable iquery = null;
        private ObjectQuery oquery = null;
        private Int32 interval = -1;

        // Summary:
        //     Occurs when a notification is received for any of the commands associated
        //     with this RegularlyNotificationRegister object.
        public event EventHandler OnChanged;
        SqlDependency dependency = null;
        Timer timer = null;

        /// <summary>
        /// Initializes a new instance of RegularlyNotificationRegister class.
        /// </summary>
        /// <param name="query">an instance of ObjectQuery is used to get connection string and 
        /// command string to register SqlDependency nitification. </param>
        /// <param name="interval">The time interval between invocations of callback, in milliseconds.</param> 
        public RegularlyNotificationRegister(ObjectQuery query, Int32 interval)
        {
            try
            {
                this.oquery = query;
                this.interval = interval;

                QueryExtension.GetSqlCommand(oquery, ref connection, ref command);

                RegisterSqlDependency();
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException("Paramter cannot be null", "query", ex);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Fails to initialize a new instance of RegularlyNotificationRegister class.", ex);
            }
        }

        /// <summary>
        /// Starts the notification of SqlDependency 
        /// </summary>
        /// <param name="context">An instance of dbcontext</param>
        public static void StartMonitor(DbContext context)
        {
            try {
                SqlDependency.Start(context.Database.Connection.ConnectionString);
            } 
            catch (Exception ex) {
                throw new System.Exception("Fails to Start the SqlDependency in the RegularlyNotificationRegister class", ex);
            }
        }

        /// <summary>
        /// Stops the notification of SqlDependency 
        /// </summary>
        /// <param name="context">An instance of dbcontext</param>
        public static void StopMonitor(DbContext context)
        {
            try {
                SqlDependency.Stop(context.Database.Connection.ConnectionString);
            } 
            catch (Exception ex) {
                throw new System.Exception("Fails to Stop the SqlDependency in the RegularlyNotificationRegister class", ex);
            }
        }

        /// <summary>
        /// Initializes a new instance of RegularlyNotificationRegister class.
        /// </summary>
        /// <param name="context">an instance of DbContext is used to get an ObjectQuery object</param>
        /// <param name="query">an instance of IQueryable is used to get ObjectQuery object, and then get  
        /// connection string and command string to register SqlDependency nitification. </param>
        /// <param name="interval">The time interval between invocations of callback, in milliseconds.</param>
        public RegularlyNotificationRegister(DbContext context, IQueryable query, Int32 interval)
        {
            try
            {
                this.iquery = query;
                this.interval = interval;

                oquery = QueryExtension.GetObjectQuery<TEntity>(context, iquery);

                QueryExtension.GetSqlCommand(oquery, ref connection, ref command);

                RegisterSqlDependency();
            }
            catch (ArgumentException ex)
            {
                if (ex.ParamName == "context")
                {
                    throw new ArgumentException("Paramter cannot be null", "context", ex);
                }
                else
                {
                    throw new ArgumentException("Paramter cannot be null", "query", ex);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Fails to initialize a new instance of RegularlyNotificationRegister class.", ex);
            }
        }

        private void RegisterSqlDependency()
        {
            if (connection == null || command == null)
            {
                throw new ArgumentException("command and connection cannot be null");
            }

            // Make sure the command object does not already have
            // a notification object associated with it.
            command.Notification = null;

            // Create and bind the SqlDependency object
            // to the command object.
            dependency = new SqlDependency(command);
            Console.WriteLine("Id of sqldependency:{0}", dependency.Id);

            RegisterSqlCommand();

            timer = new Timer(CheckChange, null, 0, interval);
            timer.Change(0, interval);
        }

        private void RegisterSqlCommand()
        {
            if (connection != null && command != null)
            {
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        private void CheckChange(object state)
        {
            if (dependency != null && dependency.HasChanges)
            {
                if (OnChanged != null)
                {
                    OnChanged(this, null);
                }
            }
        }

        /// <summary>
        /// Releases all the resources by the RegularlyNotificationRegister.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(Boolean disposed)
        {
            if (disposed)
            {
                if (command != null) {
                    command.Dispose();
                    command = null;
                }

                if (connection != null) {
                    connection.Dispose();
                    command = null;
                }

                if (timer != null) {
                    timer.Change(Timeout.Infinite, Timeout.Infinite);
                    timer.Dispose();
                    timer = null;
                }

                iquery = null;
                oquery = null;
                dependency = null;
            }
        }

        /// <summary>
        /// The SqlConnection is got from the Query.
        /// </summary>
        public SqlConnection Connection
        { get { return connection; } }

        /// <summary>
        /// The SqlCommand is got from the Query.
        /// </summary>
        public SqlCommand Command
        { get { return command; } }

        /// <summary>
        /// The ObjectQuery is got from the Query.
        /// </summary>
        public ObjectQuery Oquery
        { get { return oquery; } }
    }
}
