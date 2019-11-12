//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Data;
using System.Collections.Generic;
using Windows.UI;

namespace AppBarControl
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario3 : Windows.UI.Xaml.Controls.Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        StackPanel leftPanel;
        StackPanel rightPanel;
        List<UIElement> leftItems;
        List<UIElement> rightItems;
        public Scenario3()
        {
            this.InitializeComponent();
            leftItems = new List<UIElement>();
            rightItems = new List<UIElement>();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Find the stack panels that host our AppBarButtons within the AppBar
            leftPanel = rootPage.FindName("LeftPanel") as StackPanel;
            rightPanel = rootPage.FindName("RightPanel") as StackPanel;

            CopyButtons(leftPanel, leftItems);
            CopyButtons(rightPanel, rightItems);

            // Remove existing AppBarButtons
            leftPanel.Children.Clear();
            rightPanel.Children.Clear();

            // Create the AppBarToggle button for the 'Shuffle' command
            AppBarToggleButton shuffle = new AppBarToggleButton();
            shuffle.Label = "Shuffle";
            shuffle.Icon = new SymbolIcon(Symbol.Shuffle);

            rightPanel.Children.Add(shuffle);

            // Create the AppBarButton for the 'Sun' command
            AppBarButton sun = new AppBarButton();
            sun.Label = "Sun";

            // This button will use the FontIcon class for its icon which allows us to choose
            // any glyph from any FontFamily
            FontIcon sunIcon = new FontIcon();
            sunIcon.FontFamily = new Windows.UI.Xaml.Media.FontFamily("Wingdings");
            sunIcon.FontSize = 30.0;
            sunIcon.Glyph = "\u0052";
            sun.Icon = sunIcon;

            rightPanel.Children.Add(sun);

            // Create the AppBarButton for the 'Triangle' command
            AppBarButton triangle = new AppBarButton();
            triangle.Label = "Triangle";

            // This button will use the PathIcon class for its icon which allows us to 
            // use vector data to represent the icon
            PathIcon trianglePathIcon = new PathIcon();
            PathGeometry g = new PathGeometry();
            g.FillRule = FillRule.Nonzero;

            // Just create a simple triange shape
            PathFigure f = new PathFigure();
            f.IsFilled = true;
            f.IsClosed = true;
            f.StartPoint = new Windows.Foundation.Point(20.0, 5.0);
            LineSegment s1 = new LineSegment();
            s1.Point = new Windows.Foundation.Point(30.0, 30.0);
            LineSegment s2 = new LineSegment();
            s2.Point = new Windows.Foundation.Point(10.0, 30.0);
            LineSegment s3 = new LineSegment();
            s3.Point = new Windows.Foundation.Point(20.0, 5.0);
            f.Segments.Add(s1);
            f.Segments.Add(s2);
            f.Segments.Add(s3);
            g.Figures.Add(f);

            trianglePathIcon.Data = g;

            triangle.Icon = trianglePathIcon;

            rightPanel.Children.Add(triangle);

            // Create the AppBarButton for the 'Smiley' command
            AppBarButton smiley = new AppBarButton();
            smiley.Label = "Smiley";
            smiley.Icon = new BitmapIcon { UriSource = new Uri("ms-appx:/Assets/smiley.png") };

            rightPanel.Children.Add(smiley);


        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            leftPanel.Children.Clear();
            rightPanel.Children.Clear();

            RestoreButtons(leftPanel, leftItems);
            RestoreButtons(rightPanel, rightItems);
        }

        private void CopyButtons(StackPanel panel, List<UIElement> list)
        {
            foreach (UIElement element in panel.Children)
            {
                list.Add(element);
            }
        }

        private void RestoreButtons(StackPanel panel, List<UIElement> list)
        {
            foreach (UIElement element in list)
            {
                panel.Children.Add(element);
            }
        }
    }
}
