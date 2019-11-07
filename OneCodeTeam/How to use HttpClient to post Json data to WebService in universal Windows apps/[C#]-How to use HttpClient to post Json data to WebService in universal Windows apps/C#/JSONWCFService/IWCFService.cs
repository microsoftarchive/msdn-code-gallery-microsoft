/****************************** Module Header ******************************\
 * Module Name:  IWCFService.cs
 * Project:      JSONWCFService
 * Copyright (c) Microsoft Corporation.
 * 
 * This is Interface of WCF Service
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
using System.ServiceModel;
using System.ServiceModel.Web;

namespace JSONWCFService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IWCFService
    {

        [OperationContract]
        [WebInvoke(UriTemplate = "GetData", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        string GetDataUsingDataContract(string Name, string Age);
    }

}
