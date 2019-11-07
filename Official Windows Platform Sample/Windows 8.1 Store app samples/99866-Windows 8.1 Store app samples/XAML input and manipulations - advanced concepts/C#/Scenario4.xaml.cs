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
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
using Windows.UI;

namespace AdvancedManipulations
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario4 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        //private variables
        private Dictionary<uint, Point?> contacts;
        const double STROKETHICKNESS = 2.5;
        private Canvas canvasElement;

        public Scenario4()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            contacts = new Dictionary<uint, Point?>(2);
        }


        private void Canvas_PointerPressed_1(object sender, PointerRoutedEventArgs e)
        {
            Canvas canvas = (sender as Canvas);
            if (canvas == null) return;
            canvasElement = canvas;

            Windows.UI.Input.PointerPoint pt = e.GetCurrentPoint(canvas);
            contacts[pt.PointerId] = pt.Position;
            e.Handled = true;
        }

        private void Canvas_PointerReleased_1(object sender, PointerRoutedEventArgs e)
        {
            uint ptrId = e.GetCurrentPoint(sender as FrameworkElement).PointerId;

            if (contacts.ContainsKey(ptrId))
            {
                contacts[ptrId] = null;
                contacts.Remove(ptrId);
            }
            flashText.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            e.Handled = true;
        }

        private void Canvas_PointerMoved_1(object sender, PointerRoutedEventArgs e)
        {
            Canvas canvas = (sender as Canvas);
            if (canvas == null) return;
            Windows.UI.Input.PointerPoint pt = e.GetCurrentPoint(canvas);
            uint ptrId = pt.PointerId;

            if (contacts.ContainsKey(ptrId) && contacts[ptrId].HasValue)
            {
                Point currentContact = pt.Position;
                Point previousContact = contacts[ptrId].Value;
                if (Distance(currentContact, previousContact) > 4)
                {
                    Line l = new Line()
                    {
                        X1 = previousContact.X,
                        Y1 = previousContact.Y,
                        X2 = currentContact.X,
                        Y2 = currentContact.Y,
                        StrokeThickness = STROKETHICKNESS,
                        Stroke = new SolidColorBrush(Colors.Red),
                        StrokeEndLineCap = PenLineCap.Round
                    };

                    contacts[ptrId] = currentContact;
                    canvas.Children.Add(l);
                }
            }
        }

        private double Distance(Point currentContact, Point previousContact)
        {
            return Math.Sqrt(Math.Pow(currentContact.X - previousContact.X, 2) + Math.Pow(currentContact.Y - previousContact.Y, 2));
        }

        private void Canvas_Holding_1(object sender, HoldingRoutedEventArgs e)
        {
            //Allow drawing
            var thisCanvas = sender as Canvas;
            thisCanvas.CancelDirectManipulations();
            Point ptPos = e.GetPosition(thisCanvas);
            ptPos.Y -= 40;
            ptPos.X += 40;

            flashText.SetValue(Canvas.LeftProperty, ptPos.X);
            flashText.SetValue(Canvas.TopProperty, ptPos.Y);
            flashText.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

    }
}
