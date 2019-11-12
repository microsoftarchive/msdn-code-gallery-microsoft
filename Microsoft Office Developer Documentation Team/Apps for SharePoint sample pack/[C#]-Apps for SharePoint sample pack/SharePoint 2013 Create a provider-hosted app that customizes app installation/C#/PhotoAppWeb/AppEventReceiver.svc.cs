using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;
using System.Web.Hosting;

namespace PhotoAppWeb {
  public class AppEventReceiver : IRemoteEventService {

    public SPRemoteEventResult ProcessEvent(SPRemoteEventProperties properties) {

      SPRemoteEventResult result = new SPRemoteEventResult();

      // create client context with S2S access token required to call back into SharePoint host environment
      using (ClientContext clientContext = TokenHelper.CreateAppEventClientContext(properties, false)) {
        // obtain CSOM object for host web
        Web hostWeb = clientContext.Web;

        // handle app installed event
        if (properties.EventType == SPRemoteEventType.AppInstalled) {

          // check to see if Picture library named Photos already exists
          ListCollection allLists = hostWeb.Lists;
          IEnumerable<List> foundLists = clientContext.LoadQuery(allLists.Where(list => list.Title == "Photos"));
          clientContext.ExecuteQuery();
          List photoLibrary = foundLists.FirstOrDefault();

          if (photoLibrary == null) {
            // create Picture library named Photos if it does not already exist
            ListCreationInformation photoLibraryInfo = new ListCreationInformation();
            photoLibraryInfo.Title = "Photos";
            photoLibraryInfo.Description = "A picture for photos";
            photoLibraryInfo.QuickLaunchOption = QuickLaunchOptions.On;
            photoLibraryInfo.TemplateType = (int)ListTemplateType.PictureLibrary;
            photoLibraryInfo.Url = "Photos";
            photoLibrary = hostWeb.Lists.Add(photoLibraryInfo);
            clientContext.ExecuteQuery();
          }

          // obtain path to Photos folder in ASP.NET application
          // note you cannot use Server.Mappath because there is no support for 
          // standard ASP.NET objects in svc web service entry points
          string photoFolder = HostingEnvironment.ApplicationPhysicalPath + @"Photos\";

          // enmuerate through each file in Photos folder
          foreach (string filePath in System.IO.Directory.GetFiles(photoFolder)) {
            // upload each photo file to Photos picture library in host web
            byte[] photo = System.IO.File.ReadAllBytes(filePath);
            FileCreationInformation photo1Info = new FileCreationInformation();
            photo1Info.Content = photo;
            photo1Info.Overwrite = true;
            photo1Info.Url = System.IO.Path.GetFileName(filePath);
            File photo1 = photoLibrary.RootFolder.Files.Add(photo1Info);
          }
          // commit changes to upload photos
          clientContext.ExecuteQuery();
        }


        // handle app uninstalling event
        if (properties.EventType == SPRemoteEventType.AppUninstalling) {

          // check to see if Picture library named Photos already exists
          ListCollection allLists = hostWeb.Lists;
          IEnumerable<List> foundLists = clientContext.LoadQuery(allLists.Where(list => list.Title == "Photos"));
          clientContext.ExecuteQuery();
          List photoLibrary = foundLists.FirstOrDefault();

          if (photoLibrary != null) {
            // delete Photos library of it exists
            photoLibrary.DeleteObject();
            clientContext.ExecuteQuery();
          }
        }
      }
      return result;
    }

    // this method is never used with app lifecycle events
    public void ProcessOneWayEvent(SPRemoteEventProperties properties) { }
  }
}
