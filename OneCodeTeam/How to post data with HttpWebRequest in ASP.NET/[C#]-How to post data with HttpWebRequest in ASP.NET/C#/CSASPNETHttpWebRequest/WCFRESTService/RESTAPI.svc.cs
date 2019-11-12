/***************************** Module Header ******************************\
* Module Name:	RESTAPI.svc.cs
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
using System.Text;

namespace WCFRESTService
{
    
    public class RESTAPI : IRESTAPI
    {
        public string GetUserNameByID(string id)
        {     
            try 
	        {
                int userID = int.Parse(id);
                if (userID > 0 && userID < 5)
                {
                    var userDataList = (new UserDataList()).getUserDataList();
                    var query = from data in userDataList
                                where data.ID == id
                                select data.UserName;
                    return query.FirstOrDefault();
                }
                else
                {
                    return "Can't find the user with id: " + id;
                }        
	        }
	        catch 
	        {
                throw new Exception(string.Format("Can't convert {0} to int", id));
	        }
        }

        public UserData GetUserNameByPostID(UserData rData)
        {
            try
            {
                int userID = int.Parse(rData.ID);
                if (userID > 0 && userID < 5)
                {
                    var userDataList = (new UserDataList()).getUserDataList();
                    var query = from data in userDataList
                                where data.ID == rData.ID
                                select data;
                    return new UserData { UserName = query.FirstOrDefault().UserName };
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                throw new Exception(string.Format("Can't convert {0} to int", rData.ID));
            }


        }
    }
}
