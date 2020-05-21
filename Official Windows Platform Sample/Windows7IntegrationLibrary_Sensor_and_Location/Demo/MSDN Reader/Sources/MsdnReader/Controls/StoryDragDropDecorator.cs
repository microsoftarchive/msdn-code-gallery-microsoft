// Copyright (c) Microsoft Corporation.  All rights reserved.

//---------------------------------------------------------------------------
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
//---------------------------------------------------------------------------

namespace MsdnReader
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Markup;
    using Microsoft.SceReader;
    using Microsoft.SceReader.Data;
    using System.Globalization;

    /// <summary>
    /// The StoryDragDropDecorator enabled dragging behavior for its content.
    /// </summary>
    public class StoryDragDropDecorator : Decorator
    {
        /// <summary>
        /// The mouse position stored when the left mouse button was pressed.
        /// </summary>
        private Point mouseDownPosition;

        /// <summary>
        /// The last mouse position for mouse move event.
        /// </summary>
        private Point mouseMovePosition;

        /// <summary>
        /// The flag indicating whether or not the left mouse button is pressed.
        /// </summary>
        private bool mouseDown;

        /// <summary>
        /// The adorner element used to display the snapshot of the element while dragging.
        /// </summary>
        private DragDropAdorner adorner;

        /// <summary>
        /// The Storyboard used for drag animation effect.
        /// </summary>
        private Storyboard storyboard;

        /// <summary>
        /// Invoked when an unhandled PreviewMouseLeftButtonDown�routed event reaches the element in its route.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the event data.</param>
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            // MouseButtonDown event may indicate the click on the element or the start of the drag operation.
            // Since the decision about dragging cannot be done here, store enough information to perform drag later on.
            this.mouseDown = true;
            this.mouseDownPosition = e.GetPosition(this);

            base.OnPreviewMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Invoked when an unhandled PreviewMouseMove�routed event reaches the element in its route.
        /// </summary>
        /// <param name="e">The MouseEventArgs that contains the event data.</param>
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            // If the mouse button was pressed, the mouse move starts the drag operation.
            // Create adorner element used to display the snapshot of the element while dragging.
            if (this.mouseDown)
            {
                this.mouseMovePosition = e.GetPosition(this);
                if (this.CanDrag(this.mouseDownPosition, this.mouseMovePosition))
                {
                    if (Mouse.Captured == null)
                    {
                        Mouse.Capture(this);
                    }

                    if (this.adorner == null)
                    {
                        AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                        if (adornerLayer != null)
                        {
                            this.adorner = new DragDropAdorner(this);
                            adornerLayer.Add(this.adorner);
                        }
                    }

                    if (this.adorner != null)
                    {
                        this.adorner.Left = this.mouseMovePosition.X - this.mouseDownPosition.X;
                        this.adorner.Top = this.mouseMovePosition.Y - this.mouseDownPosition.Y;
                    }
                }
            }

            base.OnPreviewMouseMove(e);
        }

        /// <summary>
        /// Invoked when an unhandled PreviewMouseLeftButtonUp�routed event reaches the element in its route.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the event data.</param>
        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            // If the drag operation was initiated, complete it by sending the selected 
            // story to the reading list or canceling the drag operation.
            this.mouseDown = false;
            if (this.adorner != null)
            {
                bool startAnimation = false;

                // If the item was dragged away from the �ReadingList� control, cancel the drag operation. 
                // Otherwise start animation that moves the item to the �ReadingList� control.
                Point mouseUpPosition = e.GetPosition(this);
                if (mouseUpPosition.X >= this.mouseDownPosition.X || mouseUpPosition.Y <= this.mouseDownPosition.Y)
                {
                    // Find the animation target position.
                    Point readingListPosition = new Point();
                    MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                    Control readingListControl = (mainWindow != null) ? mainWindow.ReadingListControl : null;
                    if (readingListControl != null)
                    {
                        Control readingListButton = this.FindElementByName("SidebarButton", readingListControl) as Control;
                        if (readingListButton != null && readingListButton.FindCommonVisualAncestor(this) != null)
                        {
                            GeneralTransform readingListTransform = readingListButton.TransformToVisual(this);
                            if (readingListTransform != null && readingListTransform.TryTransform(new Point(0, 0), out readingListPosition))
                            {
                                readingListPosition.X += readingListButton.ActualWidth / 2;
                                readingListPosition.Y += readingListButton.ActualHeight / 2;
                                startAnimation = true;
                            }
                        }
                        else
                        {
                            GeneralTransform readingListTransform = readingListControl.TransformToVisual(this);
                            if (readingListTransform != null && readingListTransform.TryTransform(new Point(0, 0), out readingListPosition))
                            {
                                startAnimation = true;
                            }
                        }
                    }

                    if (startAnimation)
                    {
                        string strName = "DragDropAdorner" + this.adorner.GetHashCode().ToString(CultureInfo.InvariantCulture);
                        this.adorner.Name = strName;
                        NameScope.SetNameScope(this, new NameScope());
                        RegisterName(strName, this.adorner);

                        Duration duration = new Duration(TimeSpan.FromSeconds(1));

                        DoubleAnimation horizontalAnimation = new DoubleAnimation();
                        horizontalAnimation.From = mouseUpPosition.X - this.mouseDownPosition.X;
                        horizontalAnimation.To = readingListPosition.X;
                        horizontalAnimation.Duration = duration;
                        Storyboard.SetTargetProperty(horizontalAnimation, new PropertyPath("Left"));
                        Storyboard.SetTargetName(horizontalAnimation, strName);

                        DoubleAnimation verticalAnimation = new DoubleAnimation();
                        verticalAnimation.From = mouseUpPosition.Y - this.mouseDownPosition.Y;
                        verticalAnimation.To = readingListPosition.Y;
                        verticalAnimation.Duration = duration;
                        Storyboard.SetTargetProperty(verticalAnimation, new PropertyPath("Top"));
                        Storyboard.SetTargetName(verticalAnimation, strName);

                        DoubleAnimation scaleAnimation = new DoubleAnimation();
                        scaleAnimation.From = 1;
                        scaleAnimation.To = 0;
                        scaleAnimation.Duration = duration;
                        Storyboard.SetTargetProperty(scaleAnimation, new PropertyPath("Scale"));
                        Storyboard.SetTargetName(scaleAnimation, strName);

                        this.storyboard = new Storyboard();
                        this.storyboard.Children.Add(horizontalAnimation);
                        this.storyboard.Children.Add(verticalAnimation);
                        this.storyboard.Children.Add(scaleAnimation);
                        this.storyboard.Completed += new EventHandler(this.Storyboard_Completed);
                        this.storyboard.Begin(this);
                    }
                }

                // If the drag operation was canceled (animation not started), destroy all resources 
                // acquired for item drag effect.
                if (!startAnimation)
                {
                    this.DestroyAdorner();
                }

                // Release mouse capture, if we were in the middle of drag operation, to avoid treating 
                // mouse release as mouse click on buttons that may be inside the item.
                Mouse.Capture(null);
            }

            base.OnPreviewMouseLeftButtonUp(e);
        }

        /// <summary>
        /// Handler for Storyboard.Completed event fired when the dragging animation is finished.
        /// Adds the story to the reading list and destroys all resources used for drag animation.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The EventArgs that contains the event data.</param>
        private void Storyboard_Completed(object sender, EventArgs e)
        {
            Story story = this.DataContext as Story;
            if (story != null)
            {
                if (ServiceProvider.ViewManager.PersistenceCommands.AddStoryToReadingListCommand.CanExecute(story))
                {
                    ServiceProvider.ViewManager.PersistenceCommands.AddStoryToReadingListCommand.Execute(story);
                }
            }

            this.DestroyAdorner();
        }

        /// <summary>
        /// Destroy all resources associated with the adorner used for drag operation.
        /// </summary>
        private void DestroyAdorner()
        {
            if (this.adorner != null)
            {
                if (this.storyboard != null)
                {
                    this.storyboard.Completed -= new EventHandler(this.Storyboard_Completed);
                    this.storyboard = null;

                    INameScope nameScope = NameScope.GetNameScope(this);
                    if (nameScope != null)
                    {
                        UnregisterName(this.adorner.Name);
                        NameScope.SetNameScope(this, null);
                    }
                }

                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null)
                {
                    adornerLayer.Remove(this.adorner);
                }

                this.adorner = null;
            }
        }

        /// <summary>
        /// Search visual tree for a visual maching requested name.
        /// </summary>
        /// <param name="name">Requested name of visual.</param>
        /// <param name="node">Visual node to start search from.</param>
        private Visual FindElementByName(string name, Visual node)
        {
            Visual child = null;
            int count = VisualTreeHelper.GetChildrenCount(node);
            for (int i = 0; i < count; i++)
            {
                child = VisualTreeHelper.GetChild(node, i) as Visual;
                if (child != null)
                {
                    string visualName = child.GetValue(FrameworkElement.NameProperty) as string;

                    if (!String.IsNullOrEmpty(visualName) && String.Compare(name, visualName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        break;
                    }
                    child = FindElementByName(name, child);
                    if (child != null)
                    {
                        break;
                    }
                }
            }

            return child;
        }

        /// <summary>
        /// Determine whether or not dragging operation is enabled given two positions.
        /// </summary>
        /// <param name="start">The starting position of the element.</param>
        /// <param name="end">The current position of the element.</param>
        /// <returns>Whether or not dragging operation is enabled.</returns>
        private bool CanDrag(Point start, Point end)
        {
            bool horz = Math.Abs(start.X - end.X) > SystemParameters.MinimumHorizontalDragDistance;
            bool vert = Math.Abs(start.Y - end.Y) > SystemParameters.MinimumVerticalDragDistance;
            return (horz || vert);
        }

        /// <summary>
        /// The DragDropAdorner class represents the adorner used during drag operation.
        /// </summary>
        private class DragDropAdorner : Adorner
        {
            /// <summary>
            /// DependencyProperty for <see cref="Scale"/> property.
            /// </summary>
            public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(
                "Scale",
                typeof(double),
                typeof(DragDropAdorner),
                new PropertyMetadata(1.0, new PropertyChangedCallback(OnPositionChanged)));

            /// <summary>
            /// DependencyProperty for <see cref="Left"/> property.
            /// </summary>
            public static readonly DependencyProperty LeftProperty = DependencyProperty.Register(
                "Left",
                typeof(double),
                typeof(DragDropAdorner),
                new PropertyMetadata(0.0, new PropertyChangedCallback(OnPositionChanged)));

            /// <summary>
            /// DependencyProperty for <see cref="Top"/> property.
            /// </summary>
            public static readonly DependencyProperty TopProperty = DependencyProperty.Register(
                "Top",
                typeof(double),
                typeof(DragDropAdorner),
                new PropertyMetadata(0.0, new PropertyChangedCallback(OnPositionChanged)));

            /// <summary>
            /// The brush used to draw the content of the adorner.
            /// </summary>
            private VisualBrush brush;

            /// <summary>
            /// The bounding rectangle for the contnet of the adorner.
            /// </summary>
            private Rect rect;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="adornedElement">The element to bind the adorner to.</param>
            public DragDropAdorner(UIElement adornedElement)
                : base(adornedElement)
            {
                this.brush = new VisualBrush(adornedElement);
                this.rect = new Rect(adornedElement.DesiredSize);
            }

            /// <summary>
            /// Gets or sets the scale factor for the element.
            /// </summary>
            public double Scale
            {
                get { return (double)GetValue(ScaleProperty); }
                set { SetValue(ScaleProperty, value); }
            }

            /// <summary>
            /// Gets or sets the left offset value for the element.
            /// </summary>
            public double Left
            {
                get { return (double)GetValue(LeftProperty); }
                set { SetValue(LeftProperty, value); }
            }

            /// <summary>
            /// Gets or sets the top offset value for the element.
            /// </summary>
            public double Top
            {
                get { return (double)GetValue(TopProperty); }
                set { SetValue(TopProperty, value); }
            }

            /// <summary>
            /// Returns a Transform for the adorner, based on the transform that is currently applied to the adorned element.
            /// </summary>
            /// <param name="transform">The transform that is currently applied to the adorned element.</param>
            /// <returns>A transform to apply to the adorner.</returns>
            public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
            {
                GeneralTransformGroup desiredTransform = new GeneralTransformGroup();
                desiredTransform.Children.Add(new ScaleTransform(Scale, Scale));
                desiredTransform.Children.Add(new TranslateTransform(Left, Top));
                desiredTransform.Children.Add(base.GetDesiredTransform(transform));
                return desiredTransform;
            }

            /// <summary>
            /// Renders content of the element.
            /// </summary>
            /// <param name="drawingContext">The drawing instructions for the element.</param>
            protected override void OnRender(DrawingContext drawingContext)
            {
                drawingContext.DrawRectangle(this.brush, null, this.rect);
            }

            /// <summary>
            /// Handler for property value changed notification raised for properties which are affecting the position of the adorner.
            /// </summary>
            /// <param name="element">The source of the property changed event.</param>
            /// <param name="args">Provides data for property changed event.</param>
            private static void OnPositionChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
            {
                Adorner adorner = element as Adorner;
                AdornerLayer layer = adorner.Parent as AdornerLayer;
                if (layer != null)
                {
                    layer.Update(adorner.AdornedElement);
                }
            }
        }
    }
}