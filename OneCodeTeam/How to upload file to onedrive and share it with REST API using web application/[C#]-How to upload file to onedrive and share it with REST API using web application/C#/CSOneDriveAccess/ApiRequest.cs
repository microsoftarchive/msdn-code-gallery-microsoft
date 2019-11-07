using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSOneDriveAccess
{
    public class ApiRequest
    {
        /// <summary>
        /// Request object
        /// </summary>
        public HttpWebRequest Request { get; set; }

        /// <summary>
        /// Init a ApiRequest object, that contain a HttpWebRequest object.
        /// </summary>
        /// <param name="uri">Request uri</param>
        /// <param name="method">HttpMethod</param>
        /// <param name="data">The requst body</param>
        /// <param name="contentType">Request content type</param>
        public ApiRequest(string uri, HTTPMethod method, byte[] data = null, string contentType = null)
        {
            this.Request = WebRequest.Create(uri) as HttpWebRequest;
            Request.Method = method.ToString();

            if (!string.IsNullOrEmpty(contentType))
            {
                this.Request.ContentType = contentType;
            }

            if (data != null)
            {
                Stream dataStream = Request.GetRequestStream();
                dataStream.Write(data, 0, data.Length);
                dataStream.Close();
            }
            else
            {
                Request.ContentLength = 0;
            }
        }

        /// <summary>
        /// try get response and convert to string
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetResponseStringAsync()
        {
            try
            {
                WebResponse response = await RetryGetResponse(3000, 3);
                Stream dataStream = response.GetResponseStream();

                StreamReader sr = new StreamReader(dataStream, Encoding.UTF8);

                return await sr.ReadToEndAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// try get response and convert to byte[]
        /// </summary>
        /// <returns></returns>
        public async Task<byte[]> GetResponseBytesAsync()
        {
            try
            {
                WebResponse response = await RetryGetResponse(3000, 3);

                using (Stream dataStream = response.GetResponseStream())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        dataStream.CopyTo(ms);

                        return ms.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Retry to get response assign times, if it's still failed, throw out a exception
        /// </summary>
        /// <param name="retryInterval">retry time span</param>
        /// <param name="retryCount">retry time</param>
        /// <returns></returns>
        private async Task<WebResponse> RetryGetResponse(int retryInterval, int retryCount = 3)
        {
            var exceptions = new List<Exception>();

            for (int retry = 0; retry < retryCount; retry++)
            {
                try
                {
                    if (retry > 0)
                    {
                        Thread.Sleep(retryInterval);
                    }
                    return await Request.GetResponseAsync();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            throw new AggregateException(exceptions);
        }
    }
}
