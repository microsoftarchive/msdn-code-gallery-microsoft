/****************************** Module Header ******************************\
Module Name:  IUserService.cs
Project:      CSRESTfulWCFServiceProvider
Copyright (c) Microsoft Corporation.
	 
WCF Service interface to define operations for UserService.svc
	 
This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx.
All other rights reserved.
	 
THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using CSRESTfulWCFServiceProvider.Utilities;

namespace CSRESTfulWCFServiceProvider
{
    /// <summary>
    /// WCF Service interface to define operations
    /// </summary>
    [ServiceContract]
    [DataContractFormat]
    internal interface IUserService
    {
        /// <summary>
        /// Definde operation contract
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "/User/All",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        List<User> GetAllUsers();

        [WebInvoke(Method="POST",
            UriTemplate = "/User/Create",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void CreateUser(User user);

        [WebInvoke(Method = "PUT",
           UriTemplate = "/User/Edit",
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json,
           BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void UpdateUser(User user);

        [WebInvoke(Method = "DELETE",
          UriTemplate = "/User/Delete/{Id}",
          RequestFormat = WebMessageFormat.Json,
          ResponseFormat = WebMessageFormat.Json,
          BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void DeleteUser(string id);
    }
}
