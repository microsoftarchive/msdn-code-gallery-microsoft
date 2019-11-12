/****************************** Module Header ******************************\
Module Name:  JsonHelp.cs
Project:      CSRESTfulWCFService
Copyright (c) Microsoft Corporation.
	 
JsonHelp class to Serialize/DeSerialize json data
	 
This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx.
All other rights reserved.
	 
THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.Web.Script.Serialization;

namespace CSRESTfulWCFServiceASPNETClient.Utility
{
    /// <summary>
    /// JSon help class
    /// </summary>
    internal class JsonHelp
    {
        private static JavaScriptSerializer jsonSerialize;

        #region Methods

        /// <summary>
        /// Serialize object to Json
        /// </summary>
        /// <typeparam name="T">Object like: User</typeparam>
        /// <param name="objList">Object list like: List<User></param>
        /// <returns>Return a json data</returns>
        internal static string JsonSerialize<T>(T objList)
        {
            jsonSerialize = new JavaScriptSerializer();

            return jsonSerialize.Serialize(objList);
        }

        /// <summary>
        /// DeSerialize json to an object
        /// </summary>
        /// <typeparam name="T">Object like: User</typeparam>
        /// <param name="strJson">Json string</param>
        /// <returns>Return an object</returns>
        internal static T JsonDeserialize<T>(string strJson)
        {
            jsonSerialize = new JavaScriptSerializer();

            return jsonSerialize.Deserialize<T>(strJson);
        }

        #endregion
    }
}