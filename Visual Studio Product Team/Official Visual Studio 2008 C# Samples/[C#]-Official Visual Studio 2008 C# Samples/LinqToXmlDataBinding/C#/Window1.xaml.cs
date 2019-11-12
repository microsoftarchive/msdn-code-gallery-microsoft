using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.ComponentModel;
using System.Xml.Linq;

namespace LinqToXmlDataBinding {
    public partial class Window1 : Window {
        private Brush previousBrush;

        public Window1() {
            this.InitializeComponent();
        }

        /// <summary>
        /// Save MyFavorites list on closing.
        /// </summary>
        protected override void OnClosing(CancelEventArgs args) {
            XElement myFavorites = (XElement)((ObjectDataProvider)Resources["MyFavoritesList"]).Data;
            myFavorites.Save(@"..\..\data\MyFavorites.xml");
        }

        /// <summary>
        /// Play button event handler
        /// </summary>
        void OnPlay(object sender, EventArgs e) {
            videoImage.Visibility = Visibility.Hidden;
            mediaElement.Play();
        }

        /// <summary>
        /// Stop button event handler
        /// </summary>
        void OnStop(object sender, EventArgs e) {
            mediaElement.Stop();
            videoImage.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Add button event handler, adds currently selected video to MyFavorites
        /// </summary>
        void OnAdd(object sender, EventArgs e) {
            XElement itemsList = (XElement)((ObjectDataProvider)Resources["MyFavoritesList"]).Data;
            itemsList.Add(videoListBox1.SelectedItem as XElement);
        }

        /// <summary>
        /// Delete button event handler, delets currently selected video from MyFavorites
        /// </summary>
        void OnDelete(object sender, EventArgs e) {
            XElement selectedItem = (XElement)videoListBox2.SelectedItem;
            if (selectedItem != null) {
                if (selectedItem.PreviousNode != null)
                    this.videoListBox2.SelectedItem = selectedItem.PreviousNode;
                else if (selectedItem.NextNode != null)
                    this.videoListBox2.SelectedItem = selectedItem.NextNode;
                selectedItem.Remove();
            }
        }

        /// <summary>
        /// Searchbox event handler, Search videos by user specifed input
        /// </summary>
        private void OnKeyUp(object sender, KeyEventArgs e) {
            if (e.Key.Equals(Key.Enter)) {
                ObjectDataProvider objectDataProvider = Resources["MsnVideosList"] as ObjectDataProvider;
                objectDataProvider.MethodParameters[0] = @"http://soapbox.msn.com/rss.aspx?searchTerm=" + searchBox.Text;
                objectDataProvider.Refresh();
            }
        }

        /// <summary>
        /// Event handlers for search options listed on the first page, simply update the static resource
        /// "MsnVideosList" with the new argument and refresh it.
        /// </summary>
        private void OnMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            string content = (string)((Label)sender).Content;
            ObjectDataProvider objectDataProvider = Resources["MsnVideosList"] as ObjectDataProvider;

            switch (content) {
                case "Most Viewed":
                    objectDataProvider.MethodParameters[0] = @"http://soapbox.msn.com/rss.aspx?listId=MostPopular";
                    objectDataProvider.Refresh();
                    break;
                case "Most Recent":
                    objectDataProvider.MethodParameters[0] = @"http://soapbox.msn.com/rss.aspx?listId=MostRecent";
                    objectDataProvider.Refresh();
                    break;
                case "Top Favorites":
                    objectDataProvider.MethodParameters[0] = @"http://soapbox.msn.com/rss.aspx?listId=TopFavorites";
                    objectDataProvider.Refresh();
                    break;
                case "Top Rated":
                    objectDataProvider.MethodParameters[0] = @"http://soapbox.msn.com/rss.aspx?listId=TopRated";
                    objectDataProvider.Refresh();
                    break;
                case "My Favorites":
                    XElement msn = (XElement)objectDataProvider.Data;
                    XElement favorites = (XElement)((ObjectDataProvider)Resources["MyFavoritesList"]).Data;
                    msn.ReplaceAll(favorites.Elements("item"));
                    break;
            }
        }

        /// <summary>
        /// Change the color or the search links as the mouse enters and leaves to indicate
        /// that they are clickable
        /// </summary>
        private void OnMouseEnter(object sender, System.Windows.Input.MouseEventArgs e) {
            Label myLabel = sender as Label;
            previousBrush = myLabel.Foreground;
            myLabel.Foreground = Brushes.Blue;
        }

        private void OnMouseLeave(object sender, System.Windows.Input.MouseEventArgs e) {
            Label myLabel = sender as Label;
            myLabel.Foreground = previousBrush;
        }
    }
}