using Expression.Blend.SampleData.SampleDataSource;
using Windows.UI.Xaml.Controls;

namespace ListViewInteraction
{
    public sealed partial class Scenario2ItemViewer : UserControl
    {
        public Scenario2ItemViewer()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// This method shows the Title of the data item as well as a placholder
        /// for the image. We set the opacity of other UIElements to zero
        /// so that stale data is not visible to the end user.  Note that we use
        /// Border's background color as the image placeholder.
        /// </summary>
        /// <param name="item"></param>
        public void ShowTitle(Item item)
        {
            _item = item;
            subtitleTextBlock.Opacity = 0;
            image.Opacity = 0;

            titleTextBlock.Text = _item.Title;
            titleTextBlock.Opacity = 1; 
        }

        /// <summary>
        /// Visualize category information by updating the correct TextBlock and 
        /// setting Opacity to 1.
        /// </summary>
        public void ShowSubtitle()
        {
            subtitleTextBlock.Text = _item.Subtitle;
            subtitleTextBlock.Opacity = 1;
        }

        /// <summary>
        /// Visualize the Image associated with the data item by updating the Image 
        /// object and setting Opacity to 1.
        /// </summary>
        public void ShowImage()
        {
            image.Source = _item.Image;
            image.Opacity = 1;
        }

        /// <summary>
        /// Drop all refrences to the data item
        /// </summary>
        public void ClearData()
        {
            _item = null;
            titleTextBlock.ClearValue(TextBlock.TextProperty);
            subtitleTextBlock.ClearValue(TextBlock.TextProperty);
            image.ClearValue(Image.SourceProperty);
        }

        private Item _item;

    }
}
