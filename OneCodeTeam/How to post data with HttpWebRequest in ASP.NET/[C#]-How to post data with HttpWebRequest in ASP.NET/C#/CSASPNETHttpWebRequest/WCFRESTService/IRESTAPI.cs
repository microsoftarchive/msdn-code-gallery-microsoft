/***************************** Module Header ******************************\
* Module Name:	IRESTAPI.cs
* Project:		WCFRESTService
* Copyright (c) Microsoft Corporation.
* 
* This sample will show you how to create HTTPWebReqeust, and get HTTPWebResponse.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\**************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WCFRESTService
{
    // NOTE: You can use the "Rename" command on the "RefactoPr" menu to change the interface name "IRESTAPI" in both code and config file together.
    [ServiceContract]
    public interface IRESTAPI
    {
        [OperationContract]
        [WebInvoke(Method="GET",
           ResponseFormat=WebMessageFormat.Xml,
           BodyStyle=WebMessageBodyStyle.Wrapped,
           UriTemplate="json/{id}")]
        string GetUserNameByID(string id);

        [OperationContract]
        [WebInvoke(Method = "POST",
           ResponseFormat = WebMessageFormat.Xml,
           RequestFormat=WebMessageFormat.Xml,
           BodyStyle = WebMessageBodyStyle.Bare,
           UriTemplate = "json")]
        UserData GetUserNameByPostID(UserData data);



       



    }
}
