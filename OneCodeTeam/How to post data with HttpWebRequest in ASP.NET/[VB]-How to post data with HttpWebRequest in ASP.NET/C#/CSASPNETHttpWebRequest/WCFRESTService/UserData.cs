/***************************** Module Header ******************************\
* Module Name:	UserData.cs
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
using System.Web;

namespace WCFRESTService
{
     [DataContract(Namespace = "http://msdn.microsoft.com")]
    public class UserData
    {
         [DataMember]
        public string ID { get; set; }

         [DataMember]
        public string UserName { get; set; }

    }

    public class UserDataList
    {
        List<UserData> userDataList;
        public UserDataList()
        { 
            userDataList = new List<UserData>();
            userDataList.Add(new UserData { ID = "1", UserName = "Sam" });
            userDataList.Add(new UserData { ID = "2", UserName = "Lucy" });
            userDataList.Add(new UserData { ID = "3", UserName = "Dandy" });
            userDataList.Add(new UserData { ID = "4", UserName = "Alex" });
        }
        public List<UserData> getUserDataList()
        {
            return userDataList;
        }
    }

}