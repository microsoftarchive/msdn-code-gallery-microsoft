using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace SpecialAgent
{
    class Helpers
    {
        public static void HandleException(Exception e)
        {
                if (Debugger.IsAttached)
                {
                    // An unhandled exception has occurred; break into the debugger
                    Debugger.Break();
                }
                else
                {
                    Logger.Log("Agent", "Operation processing encountered an exception:");
                    Logger.Log("Agent", "*********************************");
                    Logger.Log("Agent", e.Message);
                    Logger.Log("Agent", e.StackTrace);
                    Logger.Log("Agent", e.ToString());
                }
        }

        async public static Task<IInputStream> MakeStreamFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }

            WebRequest myHttpWebRequest = WebRequest.Create(url);
            WebResponse myHttpWebResponse = (WebResponse) await myHttpWebRequest.GetResponseAsync();
            Stream receiveStream = myHttpWebResponse.GetResponseStream();

            return receiveStream.AsInputStream();
        }

        async public static Task<IInputStream> MakeStreamFromUrl(Uri url)
        {
            if (null == url)
            {
                return null;
            }

            WebRequest myHttpWebRequest = WebRequest.Create(url);
            WebResponse myHttpWebResponse = (WebResponse)await myHttpWebRequest.GetResponseAsync();
            Stream receiveStream = myHttpWebResponse.GetResponseStream();

            return receiveStream.AsInputStream();
        }

        async public static Task<IInputStream> MakeStreamFromLocalPath(Uri localPath)
        {
            if (null == localPath)
            {
                return null;
            }

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(localPath); 
            
            RandomAccessStreamReference stream = RandomAccessStreamReference.CreateFromFile(file);
            IRandomAccessStreamWithContentType streamWithType = await stream.OpenReadAsync();
            IInputStream inputStream = streamWithType;

            return inputStream;
        }
    }
}
