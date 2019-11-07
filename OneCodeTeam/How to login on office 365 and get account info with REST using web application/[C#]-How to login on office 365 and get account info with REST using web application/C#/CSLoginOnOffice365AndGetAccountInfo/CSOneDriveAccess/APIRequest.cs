using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CSOneDriveAccess
{
    public class APIRequest
    {
        public WebRequest Request { get; set; }

        public APIRequest(string url, HTTPMethod method, byte[] data = null)
        {
            Creator(url, method, data);
        }

        public APIRequest(string url, HTTPMethod method, string data = null)
        {
            byte[] bytes = null;
            if (!string.IsNullOrEmpty(data))
            {
                bytes = Encoding.UTF8.GetBytes(data);
            }
            Creator(url, method, bytes);
        }

        private void Creator(string url, HTTPMethod method, byte[] data = null)
        {
            this.Request = WebRequest.CreateHttp(url);
            Request.Method = method.ToString();

            switch (method)
            {
                case HTTPMethod.Post:
                    this.Request.ContentType = "application/x-www-form-urlencoded";
                    break;
            }

            if (data != null)
            {
                Request.ContentLength = data.Length;
                Stream dataStream = Request.GetRequestStream();
                dataStream.Write(data, 0, data.Length);
                dataStream.Close();
            }
        }

        public async Task<string> GetResponseToStringAsync()
        {
            WebResponse response = await Request.GetResponseAsync();
            Stream dataStream = response.GetResponseStream();

            StreamReader sr = new StreamReader(dataStream, Encoding.UTF8);

            return await sr.ReadToEndAsync();
        }

        public async Task<byte[]> GetResponseTobytesAsync()
        {
            WebResponse response = await Request.GetResponseAsync();

            using (Stream dataStream = response.GetResponseStream())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    dataStream.CopyTo(ms);

                    return ms.ToArray();
                }
            }
        }
    }
}
