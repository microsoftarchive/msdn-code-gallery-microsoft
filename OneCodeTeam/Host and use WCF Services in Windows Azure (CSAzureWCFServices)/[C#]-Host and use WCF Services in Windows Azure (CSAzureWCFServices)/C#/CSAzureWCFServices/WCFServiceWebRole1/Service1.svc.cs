/****************************** Module Header ******************************\
* Module Name:  Service1.svc.cs
* Project:	    CSAzureWCFServices
* Copyright (c) Microsoft Corporation.
* 
* This class implements WCFService.IContract interface.  Methods directly returns 
* information about the current web role.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using WCFContract;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace WCFServiceWebRole1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, 
    // svc and config file together.
    // Implement the wcf contract WCFContract.IContract
    public class Service1 : WCFContract.IContract
    {
      
        // Return the current web role's name and instance id
        public string GetRoleInfo()
        {
            RoleInstance currentRoleInstance = RoleEnvironment.CurrentRoleInstance;
            string RoleName = currentRoleInstance.Role.Name;
            string RoleInstanceID = currentRoleInstance.Id;
            return (string.Format("You are talking to role {0}, instance ID {1}\n.", RoleName, RoleInstanceID));
        }

        // Return the channel between the client & the server
        public string GetCommunicationChannel()
        {
            return (string.Format("We are talking via {0}.", OperationContext.Current.Channel.LocalAddress.Uri.ToString()));
           
        }
    }
}
