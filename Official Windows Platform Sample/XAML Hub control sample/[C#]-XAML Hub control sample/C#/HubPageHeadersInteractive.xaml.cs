using System;
using SDKTemplate;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace HubControl
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HubPageHeadersInteractive : Page
    {
        List<string> images;
        List<string> states;
        List<Widget> widgets;
        MainPage rootPage = MainPage.Current;

        public HubPageHeadersInteractive()
        {
            this.InitializeComponent();

            images = new List<string>();
            widgets = new List<Widget>();
            states = new List<string>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PopulateImages();
            PopulateWidgets();
            PopulateStates();

            cvs1.Source = images;
            cvs2.Source = widgets;
            cvs3.Source = states;

            // Make sure we land on section 2 at start
            SampleHub.DefaultSectionIndex = 2;
        }

        private void PopulateImages()
        {
            images.Add("Assets/circle_image1.jpg");
            images.Add("Assets/circle_image3.jpg");
            images.Add("Assets/circle_image2.jpg");
            images.Add("Assets/circle_image1.jpg");
            images.Add("Assets/circle_image3.jpg");
            images.Add("Assets/circle_image2.jpg");
        }

        private void PopulateWidgets()
        {
            widgets.Add(new Widget { Name = "Fire Hydrant", Color = "Red", Image = "Assets/circle_list1.jpg" });
            widgets.Add(new Widget { Name = "Fire Hydrant", Color = "Yellow", Image = "Assets/circle_list2.jpg" });
            widgets.Add(new Widget { Name = "Pothole Cover", Color = "Gray", Image = "Assets/circle_list3.jpg" });
            widgets.Add(new Widget { Name = "Sprinkler", Color = "Gray", Image = "Assets/circle_list4.jpg" });
            widgets.Add(new Widget { Name = "Electrical Charger", Color = "Yellow", Image = "Assets/circle_list5.jpg" });
            widgets.Add(new Widget { Name = "Valve", Color = "Red", Image = "Assets/circle_list6.jpg" });
            widgets.Add(new Widget { Name = "Fire Hydrant", Color = "Red", Image = "Assets/circle_list1.jpg" });
            widgets.Add(new Widget { Name = "Fire Hydrant", Color = "Yellow", Image = "Assets/circle_list2.jpg" });
            widgets.Add(new Widget { Name = "Pothole Cover", Color = "Gray", Image = "Assets/circle_list3.jpg" });
        }

        private void PopulateStates()
        {
            states.Add("");
            states.Add("Alabama");
            states.Add("Alaska");
            states.Add("Arizona");
            states.Add("Arkansas");
            states.Add("California");
            states.Add("Colorado");
            states.Add("Connecticut");
            states.Add("Districk of Columbia");
            states.Add("Delaware");
            states.Add("Florida");
            states.Add("Georgia");
            states.Add("Hawaii");
            states.Add("Idaho");
            states.Add("Illinois");
            states.Add("Indiana");
            states.Add("Iowa");
            states.Add("Kansas");
            states.Add("Kentucky");
            states.Add("Louisiana");
            states.Add("Maine");
            states.Add("Maryland");
            states.Add("Massachusetts");
            states.Add("Michigan");
            states.Add(">Minnesota");
            states.Add("Mississippi");
            states.Add("Missouri");
            states.Add("Montana");
            states.Add("Nebraska");
            states.Add("Nevada");
            states.Add("New Hampshire");
            states.Add("New Jersey");
            states.Add("New Mexico");
            states.Add("New York");
            states.Add("North Carolina");
            states.Add("North Dakota");
            states.Add("Ohio");
            states.Add("Oklahoma");
            states.Add("Oregon");
            states.Add("Pennsylvania");
            states.Add("Puerto Rico");
            states.Add("Rhode Island");
            states.Add("South Carolina");
            states.Add("South Dakota");
            states.Add("Tennessee");
            states.Add("Texas");
            states.Add("Utah");
            states.Add("Vermont");
            states.Add("Virginia");
            states.Add("Washington");
            states.Add("West Virginia");
            states.Add("Wisconsin");
            states.Add("Wyoming");
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            rootPage.Frame.GoBack();
        }

        private void Hub_SectionHeaderClick(object sender, HubSectionHeaderClickEventArgs e)
        {
            SuspensionManager.SessionState["SelectedSection"] = e.Section.Name;
            switch (e.Section.Name)
            {
                case "Images":
                    rootPage.Frame.Navigate(typeof(ImagesPage), rootPage);
                    break;
                case "Videos":
                    rootPage.Frame.Navigate(typeof(VideosPage), rootPage);
                    break;
                default:
                    break;
            }
        }

    }
}
