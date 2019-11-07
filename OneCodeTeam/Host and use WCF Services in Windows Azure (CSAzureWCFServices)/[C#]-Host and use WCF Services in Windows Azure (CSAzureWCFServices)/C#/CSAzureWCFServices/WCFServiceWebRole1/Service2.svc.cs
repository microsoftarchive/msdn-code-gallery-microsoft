/****************************** Module Header ******************************\
* Module Name:  Service2.svc.cs
* Project:	    CSAzureWCFServices
* Copyright (c) Microsoft Corporation.
* 
* This class implements WCFService.IContract interface.   Methods talks to all 
* instances of workrole1 to return the instance name and communication channel. 
* These data are returned to the client together with the instance name/communication 
* channel of the current web role instance.
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
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;
using WCFContract;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace WCFServiceWebRole1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service2" 
    // in code, svc and config file together.
    // implement the wcf contract WCFContract.IContract in a different way
    public class Service2 : WCFContract.IContract
    {

        // Return the current web role's name and instance id;
        // Plus, this web role instance talks to a work role instance via internal endpoint, and get 
        // the work role's instance data.
        public string GetRoleInfo()
        {
            RoleInstance currentRoleInstance = RoleEnvironment.CurrentRoleInstance;
            string roleName = currentRoleInstance.Role.Name;
            string roleInstanceID = currentRoleInstance.Id;
            string thisWR = string.Format("You are talking to the workroles via role {0}, instance ID {1}\n.",
                roleName, roleInstanceID);

            // Contact the workrole and get its info
            string workRoleInfo = string.Empty;
            System.Text.StringBuilder sb = new StringBuilder();

            var roles = RoleEnvironment.Roles["WorkerRole1"];            
            foreach (var instance in roles.Instances)
            {
                RoleInstanceEndpoint WorkRoleInternalEndPoint = instance.InstanceEndpoints["Internal"];
                NetTcpBinding binding = new NetTcpBinding(SecurityMode.None, false);
                EndpointAddress myEndpoint = new EndpointAddress(String.Format("net.tcp://{0}/Internal", 
                    WorkRoleInternalEndPoint.IPEndpoint));

                ChannelFactory<IContract> myChanFac = new ChannelFactory<WCFContract.IContract>(binding, myEndpoint);
                WCFContract.IContract myClient = myChanFac.CreateChannel();
                sb.Append(myClient.GetRoleInfo() + "\n");

            }
            workRoleInfo = sb.ToString() ;
           
            return (thisWR + "\n" + workRoleInfo);
           
        }

        // Return the current communication channel between the external client and this web role instance;
        // Plus, return the internal channel between this web role instance and all work role instances.
        public string GetCommunicationChannel()
        {
            string thisWebRoleChannel=string.Format("You are talking to the workroles via {0}.", 
                OperationContext.Current.Channel.LocalAddress.Uri.ToString());

            // Contact the workrole and get the channel info
            string workRoleChannel = string.Empty;
            System.Text.StringBuilder sb = new StringBuilder();

            var roles = RoleEnvironment.Roles["WorkerRole1"];           
            foreach (var instance in roles.Instances)
            {
                RoleInstanceEndpoint workRoleInternalEndPoint = instance.InstanceEndpoints["Internal"];
                NetTcpBinding binding = new NetTcpBinding(SecurityMode.None, false);
                EndpointAddress myEndpoint = new EndpointAddress(String.Format("net.tcp://{0}/Internal",
                    workRoleInternalEndPoint.IPEndpoint));

                ChannelFactory<IContract> myChanFac = new ChannelFactory<WCFContract.IContract>(binding, myEndpoint);
                WCFContract.IContract myClient = myChanFac.CreateChannel();
                sb.Append(myClient.GetCommunicationChannel() + "\n");

            }
            workRoleChannel = sb.ToString();

            return (thisWebRoleChannel + "\n" + workRoleChannel);
        }
    }
}
