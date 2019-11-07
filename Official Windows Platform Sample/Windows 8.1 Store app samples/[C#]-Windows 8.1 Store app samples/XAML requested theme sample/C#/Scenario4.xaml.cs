using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace RequestedTheme
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario4 : Page
    {

        public Scenario4()
        {
            this.InitializeComponent();
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RevertRequestedTheme(panel);
        }

        private void RevertRequestedTheme(FrameworkElement fe)
        {
            if (fe.RequestedTheme == ElementTheme.Default)
            {
                //The FrameworkElement doesn't have a RequestedTheme set, 
                //so we will need to ask to the Application what theme is using.
                if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
                {
                    fe.RequestedTheme = ElementTheme.Light;
                }
                else
                {
                    fe.RequestedTheme = ElementTheme.Dark; 
                }
            }
            else if (fe.RequestedTheme == ElementTheme.Dark)
            {
                fe.RequestedTheme = ElementTheme.Light;
            }
            else
            {
                fe.RequestedTheme = ElementTheme.Dark;
            }
            CurrentThemeTxtBlock.Text = "Current theme is " + fe.RequestedTheme.ToString() + ".";
        }
    }
}
