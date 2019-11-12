using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Shared_Library
{
    public static class ServerApi
    {
        public struct ContactBinding
        {
            public string RemoteId;
            public string GivenName;
            public string FamilyName;
            public string Email;
            public string CodeName;
        }

        public struct TileData
        {
            public string Title;
            public List<IRandomAccessStream> Images;
        }

        public static string GetAuthTokenFromWebService() 
        {
            return "authtoken";
        }
        public static string GetUserIdFromWebService()
        { 
            return "userid";
        }

        public static List<ContactBinding> GetContactsFromWebServiceAsync()
        {
            List<ContactBinding> output = new List<ContactBinding>();

            // For this sample, we get the contacts from a local file
            XDocument xmldoc = XDocument.Load("RemoteContactStore.xml");

            if (xmldoc != null)
            {
                var contactElements = xmldoc.Descendants("Contact");

                foreach (var el in contactElements)
                {
                    ContactBinding contactBinding = new ContactBinding();
                    contactBinding.RemoteId = el.Element("RemoteId") != null ? el.Element("RemoteId").Value : null;


                    contactBinding.GivenName = el.Element("GivenName") != null ? el.Element("GivenName").Value : null;
                    contactBinding.FamilyName = el.Element("FamilyName") != null ? el.Element("FamilyName").Value : null;
                    contactBinding.Email = el.Element("Email") != null ? el.Element("Email").Value : null;
                    contactBinding.CodeName = el.Element("CodeName") != null ? el.Element("CodeName").Value : null;

                    output.Add(contactBinding);
                }
            }

            return output;
        }

        async public static Task<TileData> GetTileDataFromWebServiceAsync(string remoteId)
        {
            // For this sample, we get the tile data from a local file
            XDocument xmldoc = XDocument.Load("RemoteContactTileData_" + remoteId + ".xml");
            XElement el = xmldoc.Element("TileData");

            TileData tileData = new TileData();

            if (xmldoc != null)
            {
                tileData.Title = el.Element("Title") != null ? el.Element("Title").Value : null;
                tileData.Images = new List<IRandomAccessStream>();
                foreach(XElement imageElement in el.Descendants("ImageUri"))
                {
                    string imageUri = imageElement.Value;
                    StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(imageUri));
                    IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
                    tileData.Images.Add(stream);
                }
            }

            return tileData;
        }
    }
}
