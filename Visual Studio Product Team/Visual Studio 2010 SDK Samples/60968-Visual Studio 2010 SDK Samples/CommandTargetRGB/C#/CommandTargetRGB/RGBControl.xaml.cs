using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace Microsoft.CommandTargetRGB
{
    public enum RGBControlColor
    {
        Red,
        Green,
        Blue,
    }

    /// <summary>
    /// Interaction logic for RGBControl.xaml
    /// </summary>
    public partial class RGBControl : UserControl
    {
        private ToolBarTray tray = null;

        public RGBControl()
        {
            InitializeComponent();
        }

        public static DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(RGBControlColor), typeof(RGBControl), new FrameworkPropertyMetadata(RGBControlColor.Red));

        public RGBControlColor Color
        {
            get
            {
                return ((RGBControlColor)(base.GetValue(RGBControl.ColorProperty)));
            }
            set
            {
                base.SetValue(RGBControl.ColorProperty, value);
            }
        }

        // Allow the tool window to create the toolbar tray.  Set its style and
        // add it to the grid.
        public void SetTray(ToolBarTray t)
        {
            tray = t;
            tray.Style = FindResource("ToolBarTrayStyle") as Style;
            grid.Children.Add(tray);
        }
    }
}