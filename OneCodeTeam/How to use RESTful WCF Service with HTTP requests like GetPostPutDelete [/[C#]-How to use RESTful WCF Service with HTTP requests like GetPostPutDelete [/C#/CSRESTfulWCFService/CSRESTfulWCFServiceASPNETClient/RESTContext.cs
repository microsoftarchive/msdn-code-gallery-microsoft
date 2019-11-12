/****************************** Module Header ******************************\
Module Name:  RESTContext.cs
Project:      CSRESTfulWCFService
Copyright (c) Microsoft Corporation.
	 
REST context class to address Get/Post/Delete/Put
	 
This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx.
All other rights reserved.
	 
THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using CSRESTfulWCFServiceASPNETClient.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace CSRESTfulWCFServiceASPNETClient
{
    /// <summary>
    /// REST context class
    /// </summary>
    /// <typeparam name="T">object like: User</typeparam>
    public class RESTContext<T>
    {
        #region Fields

        private readonly string basicUrl;
        private HttpWebRequest httpRequest;
        private HttpWebResponse httpResponse;
        private Stream dataStream;
        private StreamReader streamReader;

        #endregion

        #region Properties

        /// <summary>
        /// Message property
        /// </summary>
        public string StrMessage { get; private set; }

        #endregion

        #region Constructor

        internal RESTContext(string url)
        {
            basicUrl = url + "/{0}/{1}";
        }

        #endregion

        #region Methods

        /// <summary>
        /// Send the request to WCF Service
        /// </summary>
        /// <param name="template">Object template like: User</param>
        /// <param name="action">Action like: Delete</param>
        /// <param name="method">Request method</param>
        /// <param name="t">Object like: User</param>
        private void SendRequest(string template, string action, HttpMethod method, T t)
        {
            string jsonData = JsonHelp.JsonSerialize<T>(t);
            if (string.IsNullOrEmpty(jsonData))
                return;

            byte[] data = UnicodeEncoding.UTF8.GetBytes(jsonData);

            httpRequest = HttpWebRequest.CreateHttp(string.Format(basicUrl, template, action));
            httpRequest.Method = method.ToString();
            httpRequest.ContentType = "application/json";
            httpRequest.ContentLength = data.Length;

            try
            {
                using (dataStream = httpRequest.GetRequestStream())
                {
                    dataStream.Write(data, 0, data.Length);
                }
            }
            catch (WebException we)
            {
                StrMessage = we.Message;
            }
        }

        /// <summary>
        /// Get the response from WCF Service
        /// </summary>
        /// <param name="template">Object template like: User</param>
        /// <param name="action">Action like: Delete</param>
        /// <param name="method">Request method</param>
        /// <returns>Return the result from WCF Service</returns>
        private string GetResponse(string template, string action, HttpMethod method)
        {
            string responseData = string.Empty;

            httpRequest = HttpWebRequest.CreateHttp(string.Format(basicUrl, template, action));
            httpRequest.Method = method.ToString();

            try
            {
                using (httpResponse = httpRequest.GetResponse() as HttpWebResponse)
                {
                    dataStream = httpResponse.GetResponseStream();

                    using (streamReader = new StreamReader(dataStream))
                    {
                        responseData = streamReader.ReadToEnd();
                    }
                }
            }
            catch (WebException we)
            {
                StrMessage = we.Message;
            }
            catch (ProtocolViolationException pve)
            {
                StrMessage = pve.Message;
            }

            return responseData;
        }

        /// <summary>
        /// Get all
        /// </summary>
        /// <returns>Return a object list like: List<User></returns>
        internal List<T> GetAll()
        {
            string data = GetResponse("User", "All", HttpMethod.GET);
            if (string.IsNullOrEmpty(data))
                return null;

            return JsonHelp.JsonDeserialize<List<T>>(data);
        }

        /// <summary>
        /// Create a object like: User
        /// </summary>
        /// <param name="t">Object like User</param>
        internal void Create(T t)
        {
            SendRequest("User", "Create", HttpMethod.POST, t);
        }


        /// <summary>
        /// Update a object like: User
        /// </summary>
        /// <param name="t">Object like User</param>
        internal void Update(T t)
        {
            SendRequest("User", "Edit", HttpMethod.PUT, t);
        }

        /// <summary>
        /// Delete a object like: User
        /// </summary>
        /// <typeparam name="S">Type of object member like: int</typeparam>
        /// <param name="id">Object member like: User's id</param>
        internal void Delete<S>(S id)
        {
            GetResponse("User", string.Format("Delete/{0}", id), HttpMethod.DELETE);
        }

        #endregion
    }

    #region HttpMethod Class

    /// <summary>
    /// Class to simulate an enum
    /// </summary>
    class HttpMethod
    {
        private string method;

        public HttpMethod(string method)
        {
            this.method = method;
        }

        public static readonly HttpMethod GET = new HttpMethod("GET");
        public static readonly HttpMethod POST = new HttpMethod("POST");
        public static readonly HttpMethod PUT = new HttpMethod("PUT");
        public static readonly HttpMethod DELETE = new HttpMethod("DELETE");

        public override string ToString()
        {
            return method;
        }
    }

    #endregion
}