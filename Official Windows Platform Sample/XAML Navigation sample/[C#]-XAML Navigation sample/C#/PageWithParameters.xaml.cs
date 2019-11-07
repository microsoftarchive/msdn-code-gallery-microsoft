using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Navigation
{
    //This page is used by the Scenario 2
    public sealed partial class PageWithParameters : Page
    {
        PageWithParametersConfiguration _pageParameters;

        public PageWithParameters()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
			//We are going to cast the property Parameter of NavigationEventArgs object
			//into PageWithParametersConfiguration.
			//PageWithParametersConfiguration contains a set of parameters to pass to the page 			
            _pageParameters = e.Parameter as PageWithParametersConfiguration;
            if (_pageParameters != null)
            {
                MessageText.Text = _pageParameters.Message;
                MessageText.Text += "\nPage ID: " + _pageParameters.Id.ToString();
            }
        }
    }
}
