using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;

namespace BasicControls
{
    sealed partial class PopupPanel
    {
        public PopupPanel()
        {
            InitializeComponent();
        }

        private void ClosePopup(object sender, RoutedEventArgs e)
        {
            Popup hostPopup = this.Parent as Popup;
            hostPopup.IsOpen = false;
        }
    }
}
