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

        public Scenario3()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
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

            ((CommandBar)BottomAppBar).PrimaryCommands.Insert(2, triangle);
        }
    }
}
