using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;

namespace GENERAL_ContentTypesInCode.Features.ContentTypeFeature
{
    /// <summary>
    /// Content Types are often created declaratively. In SharePoint 2010 you can also create
    /// them in code. If you do this, you get extra functionality. For example, you get more
    /// control over upgrades. In this example we'll add a new Site Column and a custom 
    /// Content Type in Feature Receiver code.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("348df9fc-a8bb-4e52-a6be-dde7294695be")]
    public class ContentTypeFeatureEventReceiver : SPFeatureReceiver
    {
        //This GUID uniquely idenitifies the custom field
        public static readonly Guid MyFieldId = new Guid("B9EE12F5-B540-4F11-A21B-68A524014C45");
        //This is the XML used to create the field
        public static readonly string MyFieldDefXml =
             "<Field ID=\"{B9EE12F5-B540-4F11-A21B-68A524014C43}\"" +
             " Name=\"ContosoProductName\" StaticName=\"ContosoProductName\"" +
             " Type=\"Text\" DisplayName=\"Contoso Product Name\"" +
             " Group=\"Product Columns\" DisplaceOnUpgrade=\"TRUE\" />";

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            //Get references to the site and web, ensuring correct disposal
            using (SPSite site = (SPSite)properties.Feature.Parent)
            {
                using (SPWeb web = site.RootWeb)
                {
                    //Check if the custom field already exists.
                    if (web.AvailableFields.Contains(MyFieldId) == false)
                    {
                        //Create the new field
                        web.Fields.AddFieldAsXml(MyFieldDefXml);
                        web.Update();
                    }
                    //Check if the content type already exists
                    SPContentType myContentType = web.ContentTypes["Product Announcement Content Type"];
                    if (myContentType == null)
                    {
                        //Our content type will be based on the Annoucement content type
                        SPContentType announcementContentType = web.AvailableContentTypes[SPBuiltInContentTypeId.Announcement];

                        //Create the new content type
                        myContentType = new SPContentType(announcementContentType, web.ContentTypes, "Product Announcement Content Type");
                        
                        //Add the custom field to it
                        SPFieldLink newFieldLink = new SPFieldLink(web.AvailableFields["Contoso Product Name"]);
                        myContentType.FieldLinks.Add(newFieldLink);
                        
                        //Add the new content type to the site
                        web.ContentTypes.Add(myContentType);
                        web.Update();

                        //Add it to the Announcements list
                        SPList annoucementsList = web.Lists["Announcements"];
                        annoucementsList.ContentTypesEnabled = true;
                        annoucementsList.ContentTypes.Add(myContentType);
                        annoucementsList.Update();
                    }
                }
            }
        }


        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            //Get references to the site and web, ensuring correct disposal
            using (SPSite site = (SPSite)properties.Feature.Parent)
            {
                using (SPWeb web = site.RootWeb)
                {
                    //get the custom content type 
                    SPContentType myContentType = web.ContentTypes["Product Announcement Content Type"];
                    if (myContentType != null)
                    {
                        //Remove it from the Announcements list
                        SPList annoucementsList = web.Lists["Announcements"];
                        annoucementsList.ContentTypesEnabled = true;
                        SPContentTypeId ctID = annoucementsList.ContentTypes.BestMatch(myContentType.Id);
                        annoucementsList.ContentTypes.Delete(ctID);
                        annoucementsList.Update();

                        //Remove it from the site
                        web.ContentTypes.Delete(myContentType.Id);
                        web.Update();
                    }
                    try {
                        //Remove the field
                        web.Fields.Delete("ContosoProductName");
                        web.Update();
                    }
                    catch
                    {
                        //Field was not in the collection
                    }

                }
            }
        }

    }
}
