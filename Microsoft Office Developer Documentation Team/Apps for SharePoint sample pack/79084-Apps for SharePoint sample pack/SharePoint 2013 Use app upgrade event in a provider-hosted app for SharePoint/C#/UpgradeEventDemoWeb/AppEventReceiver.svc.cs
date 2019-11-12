using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;

namespace UpgradeEventDemoWeb {
  public class AppEventReceiver : IRemoteEventService {
    public SPRemoteEventResult ProcessEvent(SPRemoteEventProperties properties) {

      if (properties.AppEventProperties == null || properties.AppEventProperties.AppWebFullUrl == null) {
        throw new ApplicationException("Error: there is no AppWeb");
      }

      SPRemoteEventResult result = new SPRemoteEventResult();

      if (properties.EventType == SPRemoteEventType.AppUpgraded) {

        // create client context to appWeb
        using (ClientContext cc = TokenHelper.CreateAppEventClientContext(properties, true)) {
          if (cc == null) {
            throw new ApplicationException("Client context to app web could not be created");
          }


          // add a new column to book genre
          Web appWeb = cc.Web;
          List booksList = appWeb.Lists.GetByTitle("Books");
          cc.Load(appWeb);
          cc.Load(booksList);
          cc.ExecuteQuery();

          Field field =
            booksList.Fields.AddFieldAsXml("<Field DisplayName='BookGenre' Type='Text' />",
                                                true,
                                                AddFieldOptions.DefaultValue);
          field.Title = "Book Genre";
          field.Update();

          cc.ExecuteQuery();

          // up[date existing items with vale for new column
          // so that it grabs all list items, regardless of the folder they are in. 
          CamlQuery query = CamlQuery.CreateAllItemsQuery(100);
          ListItemCollection items = booksList.GetItems(query);
          cc.Load(items);
          cc.ExecuteQuery();

          foreach (var book in items) {
            book["BookGenre"] = "Literature";
            book.Update();
          }

          cc.ExecuteQuery();

          ListItemCreationInformation lici = new ListItemCreationInformation();
          ListItem li = booksList.AddItem(lici);
          li["Title"] = "Dr Phil's Guide To Becoming a Much Better You";
          li["BookGenre"] = "Self Help";
          li.Update();

          ListItemCreationInformation lici2 = new ListItemCreationInformation();
          ListItem li2 = booksList.AddItem(lici2);
          li2["Title"] = "Inside SharePoint Apps";
          li2["BookGenre"] = "Technical";
          li2.Update();


          cc.ExecuteQuery();


        }
      }
      
      return result;
    }

    public void ProcessOneWayEvent(SPRemoteEventProperties properties) {
      // This method is not used by app events
    }
  }
}
