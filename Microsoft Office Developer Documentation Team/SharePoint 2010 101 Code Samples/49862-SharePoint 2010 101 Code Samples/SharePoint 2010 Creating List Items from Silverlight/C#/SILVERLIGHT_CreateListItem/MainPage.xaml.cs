using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.SharePoint.Client;

namespace SILVERLIGHT_CreateListItem
{
    /// <summary>
    /// This Silverlight application creates a new item in the Annoucements list
    /// in the current SharePoint site. 
    /// </summary>
    /// <remarks>
    /// To use this application, compile it then upload the XAP file to a SharePoint 
    /// library such as Site Assets. Add a Silverlight Web Part to a page in the UI and
    /// configure it to display the SILVERLIGHT_CreateListItem.xap file in the SharePoint
    /// library.
    /// </remarks>
    public partial class MainPage : UserControl
    {
        private List annoucementsList;

        public MainPage()
        {
            InitializeComponent();
        }

        private void createAnnouncementButton_Click(object sender, RoutedEventArgs e)
        {
            //Only create the item if there is a title
            if (titleTextbox.Text != String.Empty)
            {
                //Get the current context
                ClientContext context = ClientContext.Current;
                //Get the Announcements list and add a new item
                annoucementsList = context.Web.Lists.GetByTitle("Announcements");
                ListItem newItem = annoucementsList.AddItem(new ListItemCreationInformation());
                //Set the new item's properties
                newItem["Title"] = titleTextbox.Text;
                newItem["Body"] = bodyTextbox.Text + "\n\tThis announcement was created by Silverlight code!";
                newItem.Update();
                //Load the list
                context.Load(annoucementsList, list => list.Title);
                //Execute the query to create the new item
                context.ExecuteQueryAsync(onQuerySucceeded, onQueryFailed);
            }
        }

        private void onQuerySucceeded(object sender, ClientRequestSucceededEventArgs args)
        {
            //The query succeeded but this method does not run in the UI thread.
            //We must use the Dispatcher to begin another method in the UI thread.
            UpdateUIMethod updateUI = DisplayInfo;
            this.Dispatcher.BeginInvoke(updateUI);
        }

        private void onQueryFailed(object sender, ClientRequestFailedEventArgs args)
        {
            //The query failed, display the reason
            resultLabel.Content = "Request Failed: " + args.Message;
        }

        private void DisplayInfo()
        {
            //The item was successfully created so let the user know
            resultLabel.Content = "New item successfully created";
        }

        private delegate void UpdateUIMethod();
    }
}
