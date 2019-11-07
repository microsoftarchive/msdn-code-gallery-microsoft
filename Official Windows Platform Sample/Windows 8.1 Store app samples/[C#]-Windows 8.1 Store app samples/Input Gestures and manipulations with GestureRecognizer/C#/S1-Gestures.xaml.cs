//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using SDKTemplate;
using System;
using Windows.UI.Input;
using Windows.UI;

namespace GestureRecognizer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario1
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario1()
        {
            this.InitializeComponent();
            Windows.UI.Input.GestureRecognizer gr1 = new Windows.UI.Input.GestureRecognizer();
            Windows.UI.Input.GestureRecognizer gr2 = new Windows.UI.Input.GestureRecognizer();
            GestureInputProcessor ShapeInput1 = new GestureInputProcessor(gr1, MyRect1);
            GestureInputProcessor ShapeInput2 = new GestureInputProcessor(gr2, MyRect2);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }

    class GestureInputProcessor
    {
        Windows.UI.Input.GestureRecognizer gestureRecognizer;
        Windows.UI.Xaml.UIElement element;
        string FillColor = "fill1";
        string StrokeColor = "stroke1";

        public GestureInputProcessor(Windows.UI.Input.GestureRecognizer gr, Windows.UI.Xaml.UIElement target)
        {
            this.gestureRecognizer = gr;
            this.element = target;

            this.gestureRecognizer.GestureSettings =
                Windows.UI.Input.GestureSettings.Tap |
                Windows.UI.Input.GestureSettings.Hold | //hold must be set in order to recognize the press & hold gesture
                Windows.UI.Input.GestureSettings.RightTap;

            // Set up pointer event handlers. These receive input events that are used by the gesture recognizer.
            this.element.PointerCanceled += OnPointerCanceled;
            this.element.PointerPressed += OnPointerPressed;
            this.element.PointerReleased += OnPointerReleased;
            this.element.PointerMoved += OnPointerMoved;

            // Set up event handlers to respond to gesture recognizer output
            this.gestureRecognizer.Tapped += OnTapped;
            this.gestureRecognizer.RightTapped += OnRightTapped;
        }

        void OnPointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs args)
        {
            // Route teh events to the gesture recognizer
            this.gestureRecognizer.ProcessDownEvent(args.GetCurrentPoint(this.element));
            // Set the pointer capture to the element being interacted with
            this.element.CapturePointer(args.Pointer);
            // Mark the event handled to prevent execution of default handlers
            args.Handled = true;
        }

        void OnPointerCanceled(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs args)
        {
            this.gestureRecognizer.CompleteGesture();
            args.Handled = true;
        }

        void OnPointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs args)
        {
            this.gestureRecognizer.ProcessUpEvent(args.GetCurrentPoint(this.element));
            args.Handled = true;
        }

        void OnPointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs args)
        {
            this.gestureRecognizer.ProcessMoveEvents(args.GetIntermediatePoints(this.element));
        }

        void OnTapped(object sender, TappedEventArgs e)
        {
            if (this.element is Shape) { UpdateFillColor(this.element as Shape); }
        }

        void OnRightTapped(object sender, RightTappedEventArgs e)
        {
            if (this.element is Shape) { UpdateStrokeColor(this.element as Shape); }
        }

        void UpdateFillColor(Shape shape)
        {
            if (this.FillColor == "fill1")
            {
                shape.Fill = new SolidColorBrush(Colors.Yellow);
                this.FillColor = "fill2";
            }
            else
            {
                shape.Fill = new SolidColorBrush(Colors.Aqua);
                this.FillColor = "fill1";
            }
        }

        void UpdateStrokeColor(Shape shape)
        {
            if (StrokeColor == "stroke1")
            {
                shape.Stroke = new SolidColorBrush(Colors.Red);
                StrokeColor = "stroke2";
            }
            else
            {
                shape.Stroke = new SolidColorBrush(Colors.Purple);
                StrokeColor = "stroke1";
            }
        }
    }

}
