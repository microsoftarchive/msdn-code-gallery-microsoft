using System;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CSGetScreenResolution
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// btnShow click event
        /// Get the screen resolution and show the information in the textblock.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShow_Click(object sender, RoutedEventArgs e)
        {
            // Clean the TextBlock
            this.txtInfo.Text = string.Empty;
            //Add the screen resolution information to the textblock.
            if (App.ScreenResolutionSize != Size.Empty)
            {
                var fullSize = App.ScreenResolutionSize;
                this.txtInfo.Text = string.Format("The screen resolution is: {0}x{1}", fullSize.Width, fullSize.Height);
            }
            var windowSize = ScreenResolutionHelper.GetScreenResolutionInfo();
            //Add the application window's resolution information to the textblock.
            if (windowSize != null)
            {        
                this.txtInfo.Text += (string.IsNullOrEmpty(this.txtInfo.Text) ? string.Empty : Environment.NewLine) 
                    + string.Format("The application window's resolution is: {0}x{1}", windowSize.Width, windowSize.Height);
            }
        }
    }
    public class ScreenResolutionHelper
    {
        /// <summary>
        /// Get screen resolution.
        /// If you want to get the resolution on every page in your solution, you need to call this method from app.xaml.cs and save the data as a global variable.
        /// If you have more than one computer monitor, you can only get the main monitor's screen resolution.
        /// </summary>
        /// <returns></returns>
        public static Size GetScreenResolutionInfo()
        {
            var applicationView = ApplicationView.GetForCurrentView();
            var displayInformation = DisplayInformation.GetForCurrentView();
            var bounds = applicationView.VisibleBounds;
            var scale = displayInformation.RawPixelsPerViewPixel;
            var size = new Size(bounds.Width * scale, bounds.Height * scale);
            return size;
        }
    }

}
