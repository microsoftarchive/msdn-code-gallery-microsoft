/****************************** Module Header ******************************\
* Module Name:  IContract.cs
* Project:	    CSAzureWCFServices
* Copyright (c) Microsoft Corporation.
* 
* Service Contract.
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


namespace WCFContract
{
    [ServiceContract]
    public interface IContract
    {
        // This operating returns the server information, including the role's name and instance id
        [OperationContract]
        string GetRoleInfo();

        // This operation returns the information of the channel being used between the client & the server.
        [OperationContract]
        string GetCommunicationChannel();
        
    }
   
}
