using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SpecialAgent
{
    static class BingApi
    {

        public struct Album
        {
            public string id;
            public string title;
            public bool authRequired;
        }

        public struct Photo
        {
            public string id;
            public DateTimeOffset timestamp;
            public string title;
            public string smallThumbnailUrl;
            public string largeThumbnailUrl;
            public string fullSizeUrl;
        }

        public static async Task<string> GetVersionStampAsync(string albumId)
        {
            string url = GetAlbumItemsUrl(albumId, 0, 1);
            WebRequest request = WebRequest.Create(url);
            Stream stream = (await request.GetResponseAsync()).GetResponseStream();
            XDocument doc = XDocument.Load(stream);
            return doc.Element("images").Element("image").Element("fullstartdate").Value;
        }

        public static List<Album> GetAlbums()
        {
            return new List<Album>(Albums);
        }

        public static async Task<List<Photo>> GetAllPhotosMetadataAsync(string albumId)
        {
            List<Photo> photos = new List<Photo>();

            // for better performance the Web Requests should be sent in parallel
            int countFetched = 0;
            while (countFetched < PhotoItemsTotalCount)
            {
                int nextFetch = Math.Min(PhotoItemsMaxPerRequest, PhotoItemsTotalCount - countFetched);

                WebRequest request = WebRequest.Create(GetAlbumItemsUrl(albumId, countFetched, nextFetch));
                Stream stream = (await request.GetResponseAsync()).GetResponseStream();
                XDocument doc = null;

                try
                {
                    doc = XDocument.Load(stream);
                }
                catch (Exception)
                {
                }

                if (null == doc)
                {
                    // bail out if response is empty
                    break;
                }                

                int currentFetch = 0;
                // Iterate through the image elements
                foreach (XElement image in doc.Descendants("image"))
                {
                    string startdate = image.Element("startdate").Value;
                    string fullstartdate = image.Element("fullstartdate").Value;
                    string copyright = image.Element("copyright").Value;
                    string urlBase = image.Element("urlBase").Value;

                    Photo photo = new Photo();
                    photo.id = albumId + startdate; // must be unique
                    photo.timestamp = DateTimeOffset.ParseExact(fullstartdate, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
                    photo.title = copyright;
                    photo.smallThumbnailUrl = BingImagePrefix + urlBase + SmallThumbnailResolution + BingImageSuffix;
                    photo.largeThumbnailUrl = BingImagePrefix + urlBase + LargeThumbnailResolution + BingImageSuffix;
                    photo.fullSizeUrl = BingImagePrefix + urlBase + FullSizeResolution + BingImageSuffix;

                    if (!photos.Exists(x => x.id == photo.id))
                    {
                        photos.Add(photo);
                        ++currentFetch;
                    } 
                    

                }

                countFetched += currentFetch;

                if (currentFetch == 0)
                {
                    break;
                }

            }

            return photos;
        }


        private static string GetAlbumItemsUrl(string albumId, int startIndex, int count)
        {
            return string.Format(BingApiFormat, startIndex, count, albumId);
        }

        private static Album[] Albums = new Album[]{ 
            new Album() { id = "en-US", title = "United States", authRequired = true},
            new Album() { id = "fr-FR", title = "France", authRequired = false},
            new Album() { id = "de-DE", title = "Denmark", authRequired = false},
            new Album() { id = "ro-RO", title = "Romania", authRequired = false},
            new Album() { id = "fr-CA", title = "Canada (french)", authRequired = false}
        };

        private const string BingApiFormat = "http://www.bing.com/HPImageArchive.aspx?format=xml&idx={0}&n={1}&mkt={2}";
        private const string BingImagePrefix = "http://www.bing.com";
        private const string BingImageSuffix = ".jpg";

        private const string DefaultResolution = "_1366x768";
        private const string SmallThumbnailResolution = "_320x240";
        private const string LargeThumbnailResolution = "_1280x720";
        private const string FullSizeResolution = "_1920x1080";
        private const int PhotoItemsMaxPerRequest = 8;
        private const int PhotoItemsTotalCount = 40;
    }
}
