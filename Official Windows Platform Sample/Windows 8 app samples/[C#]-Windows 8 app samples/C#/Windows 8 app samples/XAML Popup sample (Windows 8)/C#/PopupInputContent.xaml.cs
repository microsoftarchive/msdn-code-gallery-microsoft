using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace XAMLPopup
{
    public sealed partial class PopupInputContent : UserControl
    {
        public PopupInputContent()
        {
            this.InitializeComponent();
        }

        // Handles the Click event of the 'Save' button simulating a save and close
        private void SimulateSaveClicked(object sender, RoutedEventArgs e)
        {
            // in this example we assume the parent of the UserControl is a Popup
            Popup p = this.Parent as Popup;
            p.IsOpen = false; // close the Popup
        }
    }
}
