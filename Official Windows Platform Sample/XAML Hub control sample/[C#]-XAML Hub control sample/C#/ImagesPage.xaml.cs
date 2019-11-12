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
    public sealed partial class ImagesPage : Page
    {
        MainPage rootPage = MainPage.Current;

        List<Widget> widgets;

        public ImagesPage()
        {
            this.InitializeComponent();
            widgets = new List<Widget>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PopulateWidgets();
            ImagesGrid.ItemsSource = widgets;
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
            widgets.Add(new Widget { Name = "Fire Hydrant", Color = "Red", Image = "Assets/circle_list1.jpg" });
            widgets.Add(new Widget { Name = "Fire Hydrant", Color = "Yellow", Image = "Assets/circle_list2.jpg" });
            widgets.Add(new Widget { Name = "Pothole Cover", Color = "Gray", Image = "Assets/circle_list3.jpg" });
            widgets.Add(new Widget { Name = "Sprinkler", Color = "Gray", Image = "Assets/circle_list4.jpg" });
            widgets.Add(new Widget { Name = "Electrical Charger", Color = "Yellow", Image = "Assets/circle_list5.jpg" });
            widgets.Add(new Widget { Name = "Valve", Color = "Red", Image = "Assets/circle_list6.jpg" });
            widgets.Add(new Widget { Name = "Fire Hydrant", Color = "Red", Image = "Assets/circle_list1.jpg" });
            widgets.Add(new Widget { Name = "Fire Hydrant", Color = "Yellow", Image = "Assets/circle_list2.jpg" });
            widgets.Add(new Widget { Name = "Pothole Cover", Color = "Gray", Image = "Assets/circle_list3.jpg" });
            widgets.Add(new Widget { Name = "Fire Hydrant", Color = "Red", Image = "Assets/circle_list1.jpg" });
            widgets.Add(new Widget { Name = "Fire Hydrant", Color = "Yellow", Image = "Assets/circle_list2.jpg" });
            widgets.Add(new Widget { Name = "Pothole Cover", Color = "Gray", Image = "Assets/circle_list3.jpg" });
            widgets.Add(new Widget { Name = "Sprinkler", Color = "Gray", Image = "Assets/circle_list4.jpg" });
            widgets.Add(new Widget { Name = "Electrical Charger", Color = "Yellow", Image = "Assets/circle_list5.jpg" });
            widgets.Add(new Widget { Name = "Valve", Color = "Red", Image = "Assets/circle_list6.jpg" });
            widgets.Add(new Widget { Name = "Fire Hydrant", Color = "Red", Image = "Assets/circle_list1.jpg" });
            widgets.Add(new Widget { Name = "Fire Hydrant", Color = "Yellow", Image = "Assets/circle_list2.jpg" });
            widgets.Add(new Widget { Name = "Pothole Cover", Color = "Gray", Image = "Assets/circle_list3.jpg" });
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

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            rootPage.Frame.GoBack();
        }
    }
}
