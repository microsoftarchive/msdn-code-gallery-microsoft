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

namespace SILVERLIGHT_AccessListItems
{
    /// <summary>
    /// This Silverlight application queries all the items in the Tasks list in the
    /// current SharePoint site and displays them in a text box control. It assumes 
    /// there is a Tasks list.
    /// </summary>
    /// <remarks>
    /// To use this application, compile it then upload the XAP file to a SharePoint 
    /// library such as Site Assets. Add a Silverlight Web Part to a page in the UI and
    /// configure it to display the SILVERLIGHT_AccessListItems.xap file in the SharePoint
    /// library.
    /// </remarks>
    public partial class MainPage : UserControl
    {
        //Internal objects
        Web currentWebSite;
        List tasksList;
        ListItemCollection taskItems;

        public MainPage()
        {
            InitializeComponent();
        }

        private void showTaskListItemsButton_Click(object sender, RoutedEventArgs e)
        {
            //Start but getting the current SharePoint site
            ClientContext context = ClientContext.Current;
            currentWebSite = context.Web;
            context.Load(currentWebSite);
            //Get the Tasks list
            tasksList = context.Web.Lists.GetByTitle("Tasks");
            //Query the Task list for all items. We can use a blank CamlQuery object for this
            CamlQuery query = new CamlQuery();
            taskItems = tasksList.GetItems(query);
            context.Load(taskItems);
            //Run the query asynchronously
            context.ExecuteQueryAsync(onQuerySucceeded, onQueryFailed);
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
            MessageBox.Show("Request Failed: " + args.Message);
        }

        private void DisplayInfo()
        {
            //We add text to the displayTextBlock to show Task items
            displayTextBlock.Text = "Items in the Tasks List: " + taskItems.Count.ToString();
            foreach (ListItem currentItem in taskItems)
            {
                displayTextBlock.Text += "\n\tItem ID: " + currentItem.Id.ToString();
                displayTextBlock.Text += "\n\tItem Title: " + currentItem["Title"];
            }
        }

        private delegate void UpdateUIMethod();
    }
}
