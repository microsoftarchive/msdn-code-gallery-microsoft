/****************************** Module Header ******************************\
 * Module Name:  IService.cs
 * Project:      CSWindowsStoreAppAccessSQLServer
 * Copyright (c) Microsoft Corporation.
 * 
 * This is service interface 
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.Data;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace AccessSQLService
{
    [ServiceContract]
    public interface IService
    {
        // Query data
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]      
        DataSet querySql(out bool queryParam);
    }
}
