//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

namespace GesturesApp.Manipulations
{
    /// <summary>
    /// Thin wrapper around the <see cref="Windows.UI.Input.GestureRecognizer"/>, routes pointer events received by
    /// the manipulation target to the gesture recognizer.
    /// </summary>
    /// <remarks>
    /// Transformations during manipulations cannot be expressed in the coordinate space of the manipulation target.
    /// Instead they need be expressed with respect to a reference coordinate space, usually an ancestor (in the UI tree)
    /// of the element being manipulated.
    /// </remarks>
    public abstract class InputProcessor
    {
        protected Windows.UI.Input.GestureRecognizer _gestureRecognizer;
        
        // Element being manipulated
        protected Windows.UI.Xaml.FrameworkElement _target;
        public Windows.UI.Xaml.FrameworkElement Target
        {
            get { return _target; }
        }

        // Reference element that contains the coordinate space used for expressing transformations 
        // during manipulation, usually the parent element of Target in the UI tree
        protected Windows.UI.Xaml.Controls.Canvas _reference;
        public Windows.UI.Xaml.FrameworkElement Reference
        {
            get { return _reference; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="element">
        /// Manipulation target.
        /// </param>
        /// <param name="reference">
        /// Element that contains the coordinate space used for expressing transformations
        /// during manipulations, usually the parent element of Target in the UI tree.
        /// </param>
        /// <remarks>
        /// Transformations during manipulations cannot be expressed in the coordinate space of the manipulation target.
        /// Thus <paramref name="element"/> and <paramref name="reference"/> must be different. Usually <paramref name="reference"/>
        /// will be an ancestor of <paramref name="element"/> in the UI tree.
        /// </remarks>
        internal InputProcessor(Windows.UI.Xaml.FrameworkElement element, Windows.UI.Xaml.Controls.Canvas reference)
        {
            this._target = element;
            this._reference = reference;

            // Setup pointer event handlers for the element.
            // They are used to feed the gesture recognizer.    
            this._target.PointerCanceled += OnPointerCanceled;
            this._target.PointerMoved += OnPointerMoved;
            this._target.PointerPressed += OnPointerPressed;
            this._target.PointerReleased += OnPointerReleased;
            this._target.PointerWheelChanged += OnPointerWheelChanged;

            // Create the gesture recognizer
            this._gestureRecognizer = new Windows.UI.Input.GestureRecognizer();
            this._gestureRecognizer.GestureSettings = Windows.UI.Input.GestureSettings.None;
        }

        #region Pointer event handlers

        private void OnPointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs args)
        {
            // Obtain current point in the coordinate system of the reference element
            Windows.UI.Input.PointerPoint currentPoint = args.GetCurrentPoint(this._reference);

            // Route the event to the gesture recognizer
            this._gestureRecognizer.ProcessDownEvent(currentPoint);

            // Capture the pointer associated to this event
            this._target.CapturePointer(args.Pointer);

            // Mark event handled, to prevent execution of default event handlers
            args.Handled = true;
        }

        private void OnPointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs args)
        {
            // Route the events to the gesture recognizer.
            // All intermediate points are passed to the gesture recognizer in
            // the coordinate system of the reference element.
            this._gestureRecognizer.ProcessMoveEvents(args.GetIntermediatePoints(this._reference));

            // Mark event handled, to prevent execution of default event handlers
            args.Handled = true;
        }

        private void OnPointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs args)
        {
            // Obtain current point in the coordinate system of the reference element
            Windows.UI.Input.PointerPoint currentPoint = args.GetCurrentPoint(this._reference);

            // Route the event to the gesture recognizer
            this._gestureRecognizer.ProcessUpEvent(currentPoint);

            // Release pointer capture on the pointer associated to this event
            this._target.ReleasePointerCapture(args.Pointer);

            // Mark event handled, to prevent execution of default event handlers
            args.Handled = true;
        }

        private void OnPointerWheelChanged(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs args)
        {
            // Obtain current point in the coordinate system of the reference element
            Windows.UI.Input.PointerPoint currentPoint = args.GetCurrentPoint(this._reference);

            // Find out whether shift/ctrl buttons are pressed
            bool shift = (args.KeyModifiers & Windows.System.VirtualKeyModifiers.Shift) == Windows.System.VirtualKeyModifiers.Shift;
            bool ctrl = (args.KeyModifiers & Windows.System.VirtualKeyModifiers.Control) == Windows.System.VirtualKeyModifiers.Control;

            // Route the event to the gesture recognizer
            this._gestureRecognizer.ProcessMouseWheelEvent(currentPoint, shift, ctrl);

            // Mark event handled, to prevent execution of default event handlers
            args.Handled = true;
        }

        private void OnPointerCanceled(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs args)
        {
            this._gestureRecognizer.CompleteGesture();

            // Release pointer capture on the pointer associated to this event
            this._target.ReleasePointerCapture(args.Pointer);

            // Mark event handled, to prevent execution of default event handlers
            args.Handled = true;
        }

        #endregion Pointer event handlers
    }
}
