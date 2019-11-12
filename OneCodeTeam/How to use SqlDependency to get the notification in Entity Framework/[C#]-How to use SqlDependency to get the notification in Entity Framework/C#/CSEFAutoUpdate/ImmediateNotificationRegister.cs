/****************************** Module Header ******************************\
Module Name:  ChangeNotificationRegister.cs
Project:      CSEFAutoUpdate
Copyright (c) Microsoft Corporation.

We can use the Sqldependency to get the notification when the data is changed 
in database, but EF doesn’t have the same feature. In this sample, we will 
demonstrate how to automatically update by Sqldependency in Entity Framework.
In this sample, we will demonstrate two ways that use SqlDependency to get the 
change notification to auto update data.
We can get the notification immediately by this class when the data changed.

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

namespace CSEFAutoUpdate
{
    public class ImmediateNotificationRegister<TEntity> : IDisposable
        where TEntity : class
    {
        private SqlConnection connection = null;
        private SqlCommand command = null;
        private IQueryable iquery = null;
        private ObjectQuery oquery = null;

        // Summary:
        //     Occurs when a notification is received for any of the commands associated
        //     with this ImmediateNotificationRegister object.
        public event EventHandler OnChanged;
        private SqlDependency dependency = null;

        /// <summary>
        /// Initializes a new instance of ImmediateNotificationRegister class.
        /// </summary>
        /// <param name="query">an instance of ObjectQuery is used to get connection string and 
        /// command string to register SqlDependency nitification. </param>
        public ImmediateNotificationRegister(ObjectQuery query)
        {
            try
            {
                this.oquery = query;

                QueryExtension.GetSqlCommand(oquery, ref connection, ref command);

                RegisterSqlDependency();
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException("Paramter cannot be null", "query",ex);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Fails to initialize a new instance of ImmediateNotificationRegister class.", ex);
            }
        }

        /// <summary>
        /// Initializes a new instance of ImmediateNotificationRegister class.
        /// </summary>
        /// <param name="context">an instance of DbContext is used to get an ObjectQuery object</param>
        /// <param name="query">an instance of IQueryable is used to get ObjectQuery object, and then get  
        /// connection string and command string to register SqlDependency nitification. </param>
        public ImmediateNotificationRegister(DbContext context, IQueryable query)
        {
            try
            {
                this.iquery = query;

                // Get the ObjectQuery directly or convert the DbQuery to ObjectQuery.
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
                    "Fails to initialize a new instance of ImmediateNotificationRegister class.", ex);
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
            } catch (Exception ex) {
                throw new System.Exception("Fails to Start the SqlDependency in the ImmediateNotificationRegister class", ex);
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
            } catch (Exception ex) {
                throw new System.Exception("Fails to Stop the SqlDependency in the ImmediateNotificationRegister class", ex);
            }
        }

        private void RegisterSqlDependency()
        {
            if (command == null || connection == null)
            {
                throw new ArgumentException("command and connection cannot be null");
            }

            // Make sure the command object does not already have
            // a notification object associated with it.
            command.Notification = null;

            // Create and bind the SqlDependency object to the command object.
            dependency = new SqlDependency(command);
            dependency.OnChange += new OnChangeEventHandler(DependencyOnChange);

            // After register SqlDependency, the SqlCommand must be executed, or we can't 
            // get the notification.
            RegisterSqlCommand();
        }

        private void DependencyOnChange(object sender, SqlNotificationEventArgs e)
        {
            // Move the original SqlDependency event handler.
            SqlDependency dependency = (SqlDependency)sender;
            dependency.OnChange -= DependencyOnChange;

            if (OnChanged != null)
            {
                OnChanged(this, null);
            }

            // We re-register the SqlDependency.
            RegisterSqlDependency();
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

        /// <summary>
        /// Releases all the resources by the ImmediateNotificationRegister.
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
                    connection = null;
                }

                OnChanged = null;
                iquery = null;
                dependency.OnChange -= DependencyOnChange;
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
