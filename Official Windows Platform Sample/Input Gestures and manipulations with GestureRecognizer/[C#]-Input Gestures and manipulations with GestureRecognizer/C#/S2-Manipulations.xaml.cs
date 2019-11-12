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
using Windows.Foundation;

namespace GestureRecognizer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario2
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        Windows.UI.Input.GestureRecognizer gr2;
        ManipulationInputProcessor ShapeInput2;

        public Scenario2()
        {
            this.InitializeComponent();
            this.gr2 = new Windows.UI.Input.GestureRecognizer();
            this.ShapeInput2 = new ManipulationInputProcessor(gr2, ManipulateMe, Scenario2Output);
        }

        void Scenario2Reset(object sender, RoutedEventArgs e)
        {
            ManipulateMe.RenderTransform = null;
            this.ShapeInput2.InitializeTransforms();
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

    class ManipulationInputProcessor
    {
        Windows.UI.Input.GestureRecognizer gestureRecognizer;
        Windows.UI.Xaml.UIElement element;
        Windows.UI.Xaml.UIElement reference;
        string FillColor = "fill1";
        string StrokeColor = "stroke1";
        TransformGroup cumulativeTransform;
        MatrixTransform previousTransform;
        CompositeTransform deltaTransform;

        public ManipulationInputProcessor(Windows.UI.Input.GestureRecognizer gr, Windows.UI.Xaml.UIElement target, Windows.UI.Xaml.UIElement referenceframe)
        {
            this.gestureRecognizer = gr;
            this.element = target;
            this.reference = referenceframe;

            this.gestureRecognizer.GestureSettings =
                Windows.UI.Input.GestureSettings.Tap |
                Windows.UI.Input.GestureSettings.Hold | //hold must be set in order to recognize the press & hold gesture
                Windows.UI.Input.GestureSettings.RightTap |
                Windows.UI.Input.GestureSettings.ManipulationTranslateX |
                Windows.UI.Input.GestureSettings.ManipulationTranslateY |
                Windows.UI.Input.GestureSettings.ManipulationRotate |
                Windows.UI.Input.GestureSettings.ManipulationScale |
                Windows.UI.Input.GestureSettings.ManipulationTranslateInertia |
                Windows.UI.Input.GestureSettings.ManipulationRotateInertia |
                Windows.UI.Input.GestureSettings.ManipulationMultipleFingerPanning | //reduces zoom jitter when panning with multiple fingers
                Windows.UI.Input.GestureSettings.ManipulationScaleInertia;

            // Set up pointer event handlers. These receive input events that are used by the gesture recognizer.
            this.element.PointerCanceled += OnPointerCanceled;
            this.element.PointerPressed += OnPointerPressed;
            this.element.PointerReleased += OnPointerReleased;
            this.element.PointerMoved += OnPointerMoved;

            // Set up event handlers to respond to gesture recognizer output
            this.gestureRecognizer.Tapped += OnTapped;
            this.gestureRecognizer.RightTapped += OnRightTapped;
            this.gestureRecognizer.ManipulationStarted += OnManipulationStarted;
            this.gestureRecognizer.ManipulationUpdated += OnManipulationUpdated;
            this.gestureRecognizer.ManipulationCompleted += OnManipulationCompleted;
            InitializeTransforms();
        }
        void OnPointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs args)
        {
            // Route the events to the gesture recognizer
            // The points are in the reference frame of the canvas that contains the rectangle element.
            this.gestureRecognizer.ProcessDownEvent(args.GetCurrentPoint(this.reference));
            // Set the pointer capture to the element being interacted with
            this.element.CapturePointer(args.Pointer);
            // Mark the event handled to prevent execution of default handlers
            args.Handled = true;
        }
        void OnPointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs args)
        {
            this.gestureRecognizer.ProcessUpEvent(args.GetCurrentPoint(this.reference));
            args.Handled = true;
        }
        void OnPointerCanceled(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs args)
        {
            this.gestureRecognizer.CompleteGesture();
            args.Handled = true;
        }
        void OnPointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs args)
        {
            this.gestureRecognizer.ProcessMoveEvents(args.GetIntermediatePoints(this.reference));
            args.Handled = true;
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

        public void InitializeTransforms()
        {
            this.cumulativeTransform = new TransformGroup();
            this.deltaTransform = new CompositeTransform();
            this.previousTransform = new MatrixTransform() { Matrix = Matrix.Identity };

            this.cumulativeTransform.Children.Add(previousTransform);
            this.cumulativeTransform.Children.Add(deltaTransform);

            this.element.RenderTransform = this.cumulativeTransform;
        }
        void OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
        }
        void OnManipulationUpdated(object sender, ManipulationUpdatedEventArgs e)
        {
            this.previousTransform.Matrix = this.cumulativeTransform.Value;

            Point center = new Point(e.Position.X, e.Position.Y);

            this.deltaTransform.CenterX = center.X;
            this.deltaTransform.CenterY = center.Y;

            this.deltaTransform.Rotation = e.Delta.Rotation;
            this.deltaTransform.ScaleX = deltaTransform.ScaleY = e.Delta.Scale;
            this.deltaTransform.TranslateX = e.Delta.Translation.X;
            this.deltaTransform.TranslateY = e.Delta.Translation.Y;
        }
        void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
        }
    }
}
