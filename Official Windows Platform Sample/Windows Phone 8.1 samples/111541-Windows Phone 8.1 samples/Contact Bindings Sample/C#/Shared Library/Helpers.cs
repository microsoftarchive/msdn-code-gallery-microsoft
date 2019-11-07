using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Shared_Library
{
    public static class Helpers
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

        async public static Task<IInputStream> MakeStreamFromUrlAsync(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }

            WebRequest myHttpWebRequest = WebRequest.Create(url);
            WebResponse myHttpWebResponse = (WebResponse)await myHttpWebRequest.GetResponseAsync();
            Stream receiveStream = myHttpWebResponse.GetResponseStream();

            return receiveStream.AsInputStream();
        }

        async public static Task<IInputStream> MakeStreamFromUrlAsync(Uri url)
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

        async public static Task<IInputStream> MakeStreamFromLocalPathAsync(Uri localPath)
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

        public static async Task ParallelForEach<TSource>(IEnumerable<TSource> collection, Func<TSource, Task> work, uint maxTasksRunningInParallel = 6)
        {
            List<Task> inprogressTasks = new List<Task>();
            foreach(TSource item in collection)
            {
                // limit the number of simultaneous tasks
                if (inprogressTasks.Count >= maxTasksRunningInParallel)
                {
                    Task completed = await Task.WhenAny(inprogressTasks);
                    inprogressTasks.Remove(completed);
                }
                inprogressTasks.Add(work(item));
            }

            // wait for all the tasks to complete
            if (inprogressTasks.Count > 0)
            {
                await Task.WhenAll(inprogressTasks);
                inprogressTasks.Clear();
            } 
        }
    }
}
