/****************************** Module Header ******************************\
* Module Name: WebRole.cs
* Project:     IdentityChange
* Copyright (c) Microsoft Corporation.
* 
* Most of customers test their applications to connect to cloud entities like storage,
* SQL Azure, AppFabric services via compute emulator environment. 
* If the machine that is executing the code is behind proxy that does not allow 
* traffic from non-authenticated users, their connections fail. 
* One of the workaround is to change the application identity. This cannot be done 
* manually for Azure scenario since the app pool is created by Azure when it is 
* actually running the service. This sample has code that changes AppPool identity 
* programmatically with configured Domain user/Password.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.Web.Administration;
using System.DirectoryServices;

namespace IdentityChange
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            // Name of the site. Default name Azure gives to website is "Web". If this is changed, 
            // you would need to assign the name of the site to siteName variable. This can be 
            // obtained from ServiceDefinition.def file.
            var siteName = "Web";

            // Please change the domain\user to domain account that you would like to configure 
            // for AppPool to run under
            var userName = @"Domain\user";  

            // Password of the above specified domain user
            var password = "********"; //***This must be changed 

            // This variable is used to iterate through list of Application pools
            var metabasePath = "IIS://localhost/W3SVC/AppPools";

            // This variable is to get the name of AppPool that is created by Azure for current Azure service
            var appPoolName = "";

            
            using (ServerManager serverManager = new ServerManager())
            {
                //Get the name of the appPool that is created by Azure
                appPoolName = serverManager.Sites[RoleEnvironment.CurrentRoleInstance.Id + "_" + siteName].Applications.First().ApplicationPoolName;
            }

            // Get list of appPools at specified metabasePath location
            using (DirectoryEntry appPools = new DirectoryEntry(metabasePath))
            {
                // From the list of appPools, Search and get the appPool that is created by Azure 
                using (DirectoryEntry azureAppPool = appPools.Children.Find(appPoolName, "IIsApplicationPool"))
                {
                    if (azureAppPool != null)
                    {

                        // Set the AppPoolIdentityType to 3. This is equalient to MD_APPPOOL_IDENTITY_TYPE_SPECIFICUSER -  
                        // The application pool runs as a specified user account.
                        // Refer to:
                        // http://www.microsoft.com/technet/prodtechnol/WindowsServer2003/Library/IIS/e3a60d16-1f4d-44a4-9866-5aded450956f.mspx?mfr=true, 
                        // http://learn.iis.net/page.aspx/624/application-pool-identities/ 
                        // for more info on AppPoolIdentityType
                        azureAppPool.InvokeSet("AppPoolIdentityType", new Object[] { 3 });
                        
                        // Configure username for the AppPool with above specified username                      
                        azureAppPool.InvokeSet("WAMUserName", new Object[] { userName });
                        
                        // Configure password for the AppPool with above specified password                      
                        azureAppPool.InvokeSet("WAMUserPass", new Object[] { password });
                        
                        // Write above settings to IIS metabase
                        azureAppPool.Invoke("SetInfo", null);
                        
                        // Commit the above configuration changes that are written to metabase
                        azureAppPool.CommitChanges();
                    }

                }
                
                return base.OnStart();
            }
        }
    }
}