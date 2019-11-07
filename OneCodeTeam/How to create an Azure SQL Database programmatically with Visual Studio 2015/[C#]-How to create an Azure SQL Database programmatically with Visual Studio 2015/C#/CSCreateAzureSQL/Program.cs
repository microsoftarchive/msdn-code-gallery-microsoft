/****************************** Module Header ******************************\
* Module Name: Program.cs
* Project:     CSCreateAzureSQL
* Copyright (c) Microsoft Corporation.
* 
* This sample will show how to create a SQL database on Microsoft Azure 
* using C#
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using Microsoft.Azure;
using Microsoft.Azure.Management.Resources;
using Microsoft.Azure.Management.Sql;
using Microsoft.Azure.Management.Sql.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace CreateAzureSQLdatabaseCS
{
    class Program
    {
        private static string azureSubscriptionId = "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX"; /* Azure Subscription ID */

        private static string location = "Data Center Location"; /* Location of server and location for database (eg. Australia East) */
        private static string edition = "Standard"; /* Databse Edition (eg. Standard)*/
        private static string requestedServiceObjectName = "Performance Level"; /* Name of Service Object (eg. S0) */

        private static string resourceGroupName = "Resource Group Name"; /* Name of Resource Group containing SQL Server */
        private static string serverName = "Server Name"; /* Name of SQL Server */
        private static string databaseName = "Database Name"; /* Name of Database */

        private static string domainName = "domain.name.com"; /* Tenant ID or AAD domain */

        /* Authentication variables from Azure Active Directory (AAD) */
        private static string clientId = "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX"; /* Active Directory Client ID */
        private static string clientAppUri = "http://clientappuri"; /* redirect URI */

        static void Main(string[] args)
        {

            var token = GetAccessToken();
            SqlManagementClient client = new SqlManagementClient(new TokenCloudCredentials(azureSubscriptionId, token.AccessToken));

            DatabaseCreateOrUpdateParameters databaseParameters = new DatabaseCreateOrUpdateParameters()
            {
                Location = location,
                Properties = new DatabaseCreateOrUpdateProperties()
                {
                    Edition = edition,
                    RequestedServiceObjectiveName = requestedServiceObjectName,
                }

            };
            var dbResponse = client.Databases.CreateOrUpdate(resourceGroupName, serverName, databaseName, databaseParameters);
            Console.WriteLine("Database {0} created with status code: {1}. Service Objective: {2} ", dbResponse.Database.Name, dbResponse.StatusCode, dbResponse.Database.Properties.ServiceObjective);

        }

        /// <summary>
        /// Prompts for user credentials when first run or if the cached credentials have expired.
        /// </summary>
        /// <returns>The access token from AAD</returns>
        private static AuthenticationResult GetAccessToken()
        {

            AuthenticationContext authContext = new AuthenticationContext("https://login.windows.net/" + domainName);

            AuthenticationResult token = authContext.AcquireToken
                ("https://management.azure.com/"/* the Azure Resource Management endpoint */,
                    clientId, new Uri(clientAppUri),
            PromptBehavior.Auto /* with Auto user will not be prompted if an unexpired token is cached */);

            return token;

        }


    }
}
