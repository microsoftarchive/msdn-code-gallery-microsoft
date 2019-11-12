/****************************** Module Header ******************************\
* Module Name: Downloader.cs
* Project:     CSASPNETResumeDownload2012
* Copyright (c) Microsoft Corporation
*
* This project demonstrates how to implement resume download feature in Asp.net.
* As we know, due to network interruptions, downloading a file is a problem when 
* the size of the file is large, we need to support resume download if the connection
* broken.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace CSASPNETResumeDownload2012
{
    public class Downloader
    {
        public static void DownloadFile(HttpContext httpContext, string filePath)
        {
            if (!IsFileExists(filePath))
            {
                httpContext.Response.StatusCode = 404;
                return;
            }

            FileInfo fileInfo = new FileInfo(filePath);

            if (fileInfo.Length > Int32.MaxValue)
            {
                httpContext.Response.StatusCode = 413;
                return;
            }

            // Get the response header information by the http request.
            HttpResponseHeader responseHeader = GetResponseHeader(httpContext.Request, fileInfo);

            if (responseHeader == null)
            {
                return;
            }

            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            try
            {
                SendDownloadFile(httpContext.Response, responseHeader, fileStream);
            }
            catch (HttpException ex)
            {
                httpContext.Response.StatusCode = ex.GetHttpCode();
            }
            finally
            {
                fileStream.Close();
            }
        }

        /// <summary>
        /// Check whether the file exists.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static bool IsFileExists(string filePath)
        {
            bool fileExists = false;

            if (!string.IsNullOrEmpty(filePath))
            {
                if (File.Exists(filePath))
                {
                    fileExists = true;
                }
            }

            return fileExists;
        }

        /// <summary>
        /// Get the response header by the http request.
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        private static HttpResponseHeader GetResponseHeader(HttpRequest httpRequest, FileInfo fileInfo)
        {
            if (httpRequest == null)
            {
                return null;
            }

            if (fileInfo == null)
            {
                return null;
            }

            long startPosition = 0;
            string contentRange = "";

            string fileName = fileInfo.Name;
            long fileLength = fileInfo.Length;
            string lastUpdateTimeStr = fileInfo.LastWriteTimeUtc.ToString();

            string eTag = HttpUtility.UrlEncode(fileName, Encoding.UTF8) + " " + lastUpdateTimeStr;
            string contentDisposition = "attachment;filename=" + HttpUtility.UrlEncode(fileName, Encoding.UTF8).Replace("+", "%20");

            if (httpRequest.Headers["Range"] != null)
            {
                string[] range = httpRequest.Headers["Range"].Split(new char[] { '=', '-' });
                startPosition = Convert.ToInt64(range[1]);
                if (startPosition < 0 || startPosition >= fileLength)
                {
                    return null;
                }
            }

            if (httpRequest.Headers["If-Range"] != null)
            {
                if (httpRequest.Headers["If-Range"].Replace("\"", "") != eTag)
                {
                    startPosition = 0;
                }
            }

            string contentLength = (fileLength - startPosition).ToString();

            if (startPosition > 0)
            {
                contentRange = string.Format(" bytes {0}-{1}/{2}", startPosition, fileLength - 1, fileLength);
            }

            HttpResponseHeader responseHeader = new HttpResponseHeader();

            responseHeader.AcceptRanges = "bytes";
            responseHeader.Connection = "Keep-Alive";
            responseHeader.ContentDisposition = contentDisposition;
            responseHeader.ContentEncoding = Encoding.UTF8;
            responseHeader.ContentLength = contentLength;
            responseHeader.ContentRange = contentRange;
            responseHeader.ContentType = "application/octet-stream";
            responseHeader.Etag = eTag;
            responseHeader.LastModified = lastUpdateTimeStr;

            return responseHeader;
        }

        /// <summary>
        /// Send the download file to the client.
        /// </summary>
        /// <param name="httpResponse"></param>
        /// <param name="responseHeader"></param>
        /// <param name="fileStream"></param>
        private static void SendDownloadFile(HttpResponse httpResponse, HttpResponseHeader responseHeader, Stream fileStream)
        {
            if (httpResponse == null || responseHeader == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(responseHeader.ContentRange))
            {
                httpResponse.StatusCode = 206;

                // Set the start position of the reading files.
                string[] range = responseHeader.ContentRange.Split(new char[] { ' ', '=', '-' });
                fileStream.Position = Convert.ToInt64(range[2]);
            }
            httpResponse.Clear();
            httpResponse.Buffer = false;
            httpResponse.AppendHeader("Accept-Ranges", responseHeader.AcceptRanges);
            httpResponse.AppendHeader("Connection", responseHeader.Connection);
            httpResponse.AppendHeader("Content-Disposition", responseHeader.ContentDisposition);
            httpResponse.ContentEncoding = responseHeader.ContentEncoding;
            httpResponse.AppendHeader("Content-Length", responseHeader.ContentLength);
            if (!string.IsNullOrEmpty(responseHeader.ContentRange))
            {
                httpResponse.AppendHeader("Content-Range", responseHeader.ContentRange);
            }
            httpResponse.ContentType = responseHeader.ContentType;
            httpResponse.AppendHeader("Etag", "\"" + responseHeader.Etag + "\"");
            httpResponse.AppendHeader("Last-Modified", responseHeader.LastModified);

            Byte[] buffer = new Byte[10240];
            long fileLength = Convert.ToInt64(responseHeader.ContentLength);

            // Send file to client.
            while (fileLength > 0)
            {
                if (httpResponse.IsClientConnected)
                {
                    int length = fileStream.Read(buffer, 0, 10240);

                    httpResponse.OutputStream.Write(buffer, 0, length);

                    httpResponse.Flush();

                    fileLength = fileLength - length;
                }
                else
                {
                    fileLength = -1;
                }
            }
        }
    }

    /// <summary>
    /// Respresent the HttpResponse header information.
    /// </summary>
    class HttpResponseHeader
    {
        public string AcceptRanges { get; set; }
        public string Connection { get; set; }
        public string ContentDisposition { get; set; }
        public Encoding ContentEncoding { get; set; }
        public string ContentLength { get; set; }
        public string ContentRange { get; set; }
        public string ContentType { get; set; }
        public string Etag { get; set; }
        public string LastModified { get; set; }
    }
}