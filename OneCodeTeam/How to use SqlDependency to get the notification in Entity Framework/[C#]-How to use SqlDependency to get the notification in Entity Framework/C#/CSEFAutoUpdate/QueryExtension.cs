/****************************** Module Header ******************************\
Module Name:  QueryExtension.cs
Project:      CSEFAutoUpdate
Copyright (c) Microsoft Corporation.

We can use the Sqldependency to get the notification when the data is changed 
in database, but EF doesn’t have the same feature. In this sample, we will 
demonstrate how to automatically update by Sqldependency in Entity Framework.
In this sample, we will demonstrate two ways that use SqlDependency to get the 
change notification to auto update data.
This class contains some methods to extend the EF.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;

namespace CSEFAutoUpdate
{
    public static class QueryExtension
    {
        /// <summary>
        /// Return the ObjectQuery directly or convert the DbQuery to ObjectQuery.
        /// </summary>
        public static ObjectQuery GetObjectQuery<TEntity>(DbContext context, IQueryable query)
            where TEntity : class
        {
            if (query is ObjectQuery)
                return query as ObjectQuery;

            if (context == null)
                throw new ArgumentException("Paramter cannot be null", "context");

            // Use the DbContext to create the ObjectContext
            ObjectContext objectContext = ((IObjectContextAdapter)context).ObjectContext;
            // Use the DbSet to create the ObjectSet and get the appropriate provider.
            IQueryable iqueryable = objectContext.CreateObjectSet<TEntity>() as IQueryable;
            IQueryProvider provider = iqueryable.Provider;

            // Use the provider and expression to create the ObjectQuery.
            return provider.CreateQuery(query.Expression) as ObjectQuery;
        }

        /// <summary>
        /// Use ObjectQuery to get SqlConnection and SqlCommand.
        /// </summary>
        public static void GetSqlCommand(ObjectQuery query, ref SqlConnection connection, ref SqlCommand command)
        {
            if (query == null)
                throw new System.ArgumentException("Paramter cannot be null", "query");

            if (connection == null)
            {
                connection = new SqlConnection(QueryExtension.GetConnectionString(query));
            }

            if (command == null)
            {
                command = new SqlCommand(QueryExtension.GetSqlString(query), connection);

                // Add all the paramters used in query.
                foreach (ObjectParameter parameter in query.Parameters)
                {
                    command.Parameters.AddWithValue(parameter.Name, parameter.Value);
                }
            }
        }

        /// <summary>
        /// Use ObjectQuery to get the connection string.
        /// </summary>
        public static String GetConnectionString(ObjectQuery query)
        {
            if (query == null)
            {
                throw new ArgumentException("Paramter cannot be null", "query");
            }

            EntityConnection connection = query.Context.Connection as EntityConnection;
            return connection.StoreConnection.ConnectionString;
        }

        /// <summary>
        /// Use ObjectQuery to get the Sql string.
        /// </summary>
        public static String GetSqlString(ObjectQuery query)
        {
            if (query == null)
            {
                throw new ArgumentException("Paramter cannot be null", "query");
            }

            string s = query.ToTraceString();

            return s;
        }

    }
}
