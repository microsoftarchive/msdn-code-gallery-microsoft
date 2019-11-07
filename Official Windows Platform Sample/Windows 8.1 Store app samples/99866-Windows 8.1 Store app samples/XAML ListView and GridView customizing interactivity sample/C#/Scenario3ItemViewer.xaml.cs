using Expression.Blend.SampleData.SampleDataSource;
using System;
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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ListViewInteraction
{
    public sealed partial class Scenario3ItemViewer : UserControl
    {
        public Scenario3ItemViewer()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// This method shows the Title of the data item. We set the opacity of other 
        /// UIElements to zero so that stale data is not visible to the end user.
        /// </summary>
        /// <param name="item"></param>
        public void ShowTitle(Item item)
        {
            _item = item;
            contentTextBlock.Opacity = 0;

            titleTextBlock.Text = _item.Title;
            titleTextBlock.Opacity = 1;
        }

        /// <summary>
        /// Visualize category information by updating the correct TextBlock and 
        /// setting Opacity to 1.
        /// </summary>
        public void ShowContent()
        {
            contentTextBlock.Text = _item.Content;
            contentTextBlock.Opacity = 1;
        }


        /// <summary>
        /// Drop all refrences to the data item
        /// </summary>
        public void ClearData()
        {
            _item = null;
            titleTextBlock.ClearValue(TextBlock.TextProperty);
            contentTextBlock.ClearValue(TextBlock.TextProperty);
        }

        private Item _item;

    }
}
