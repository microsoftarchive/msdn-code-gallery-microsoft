//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************


namespace Rendering
{
    class XamlInkRenderer
    {
        public static Windows.UI.Xaml.Shapes.Path CreateBezierPath(Windows.UI.Input.Inking.InkStroke stroke)
        {
            // Create Bezier geometries using information provided by the stroke's segments
            var figure = new Windows.UI.Xaml.Media.PathFigure();
            var segments = stroke.GetRenderingSegments().GetEnumerator();
            segments.MoveNext();
            // First segment is degenerate and corresponds to initial position
            figure.StartPoint = segments.Current.Position;
            // Now loop through all remaining segments
            while (segments.MoveNext())
            {
                var bs = new Windows.UI.Xaml.Media.BezierSegment();
                bs.Point1 = segments.Current.BezierControlPoint1;
                bs.Point2 = segments.Current.BezierControlPoint2;
                bs.Point3 = segments.Current.Position;
                figure.Segments.Add(bs);
            }

            // Create and initialize the data structures necessary to render the figure
            var geometry = new Windows.UI.Xaml.Media.PathGeometry();
            geometry.Figures.Add(figure);
            var path = new Windows.UI.Xaml.Shapes.Path();
            path.Data = geometry;

            // Set the stroke's graphical properties, which are controlled by the Path object
            path.Stroke = new Windows.UI.Xaml.Media.SolidColorBrush(stroke.DrawingAttributes.Color);
            path.StrokeThickness = stroke.DrawingAttributes.Size.Width;
            path.StrokeLineJoin = Windows.UI.Xaml.Media.PenLineJoin.Round;
            path.StrokeStartLineCap = Windows.UI.Xaml.Media.PenLineCap.Round;

            return path;
        }

        // Rendering in WPF
        // WPF uses a retained rendering mode. In order to be rendered, Segments must be assigned to a Figure, the Figure to a 
        // Geometry, the Geometry to a Path (_livePaths) and the Path to a UIElement (_panel). The Path controls the graphical 
        // properties (stroke color, thickness, etc.) of all the geometries it contains.
        // We don't need to keep track of the intermediate objects but we do need to save the paths so that we can release them
        // when they are no longer needed.

        readonly Windows.UI.Xaml.Controls.Panel render = null; // UIElement that displays the render

        // Live render
        // We use dictionaries because several pointers may be contributing to the live render at a time.
        // The dictionaries are indexed by the pointer ID. A new entry is added to the dictionaries by EnterLiveRenderingMode,
        // it is updated by UpdateLiveRender, and it is removed from the dictionaries by ExitLiveRendering.
        // In principle we don't need a dictionary for the polylines as we could walk our way down from the
        // corresponding path, however it is convenient to have one.
        readonly System.Collections.Generic.Dictionary<uint, Windows.UI.Xaml.Shapes.Path> livePaths = null;
        readonly System.Collections.Generic.Dictionary<uint, Windows.UI.Xaml.Media.PolyLineSegment> liveStrokes = null;

        // Bezier render
        readonly System.Collections.Generic.Dictionary<Windows.UI.Input.Inking.InkStroke, Windows.UI.Xaml.Shapes.Path> bezierPaths = null;
        readonly System.Collections.Generic.Dictionary<Windows.UI.Input.Inking.InkStroke, Windows.UI.Xaml.Shapes.Path> selectionPaths = null;

        public XamlInkRenderer(Windows.UI.Xaml.Controls.Panel panel)
        {
            if (null == panel)
            {
                throw new System.ArgumentException("Argument cannot be null");
            }

            render = panel;
            livePaths = new System.Collections.Generic.Dictionary<uint, Windows.UI.Xaml.Shapes.Path>();
            liveStrokes = new System.Collections.Generic.Dictionary<uint, Windows.UI.Xaml.Media.PolyLineSegment>();
            bezierPaths = new System.Collections.Generic.Dictionary<Windows.UI.Input.Inking.InkStroke, Windows.UI.Xaml.Shapes.Path>();
            selectionPaths = new System.Collections.Generic.Dictionary<Windows.UI.Input.Inking.InkStroke, Windows.UI.Xaml.Shapes.Path>();
        }

        public void Clear()
        {
            // Remove all paths from the render so they are not rendered any more.
            // We don't assume that we have full ownership of the render so we only remove the paths that we own
            // instead of clearing it.
            foreach (var path in bezierPaths.Values)
            {
                render.Children.Remove(path);
            }
            foreach (var path in selectionPaths.Values)
            {
                render.Children.Remove(path);
            }
            foreach (var path in livePaths.Values)
            {
                render.Children.Remove(path);
            }

            // Clear dictionaries
            bezierPaths.Clear();
            selectionPaths.Clear();
            livePaths.Clear();
            liveStrokes.Clear();
        }

        #region Live render

        // throws ArgumentException if pointerPoint.pointerId is already in live rendering mode
        public void EnterLiveRendering(Windows.UI.Input.PointerPoint pointerPoint, Windows.UI.Input.Inking.InkDrawingAttributes drawingAttributes)
        {
            uint pointerId = pointerPoint.PointerId;

            // Create and initialize the data structures necessary to render a polyline in XAML.
            var stroke = new Windows.UI.Xaml.Media.PolyLineSegment();
            stroke.Points.Add(pointerPoint.Position);
            var figure = new Windows.UI.Xaml.Media.PathFigure();
            figure.StartPoint = pointerPoint.Position;
            figure.Segments.Add(stroke);
            var geometry = new Windows.UI.Xaml.Media.PathGeometry();
            geometry.Figures.Add(figure);
            var path = new Windows.UI.Xaml.Shapes.Path();
            path.Data = geometry;

            // Set the stroke's graphical properties, which are controlled by the Path object
            path.Stroke = new Windows.UI.Xaml.Media.SolidColorBrush(drawingAttributes.Color);
            path.StrokeThickness = drawingAttributes.Size.Width;
            path.StrokeLineJoin = Windows.UI.Xaml.Media.PenLineJoin.Round;
            path.StrokeStartLineCap = Windows.UI.Xaml.Media.PenLineCap.Round;

            // Update dictionaries
            liveStrokes.Add(pointerId, stroke); // throws ArgumentException if pointerId is already in the dictionary
            livePaths.Add(pointerId, path);     // throws ArgumentException if pointerId is already in the dictionary

            // Add path to render so that it is rendered (on top of all the elements with same ZIndex).
            // We want the live render to be on top of the Bezier render, so we set the ZIndex of the elements of the
            // live render to 2 and that of the elements of the Bezier render to 1.
            render.Children.Add(path);
            Windows.UI.Xaml.Controls.Canvas.SetZIndex(path, 2);
        }

        public void UpdateLiveRender(Windows.UI.Input.PointerPoint pointerPoint)
        {
            uint pointerId = pointerPoint.PointerId;

            try
            {
                liveStrokes[pointerId].Points.Add(pointerPoint.Position); // throws KeyNotFoundException if pointerId is not in the dictionary
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                // this pointer is not in live rendering mode - ignore it
            }
        }

        public void ExitLiveRendering(Windows.UI.Input.PointerPoint pointerPoint)
        {
            uint pointerId = pointerPoint.PointerId;

            try
            {
                // Remove path from the render so it is not rendered any more
                render.Children.Remove(livePaths[pointerId]); // throws KeyNotFoundException if stroke is already in the dictionary

                // Update dictionaries
                liveStrokes.Remove(pointerId);
                livePaths.Remove(pointerId);
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                // this pointer is not in live rendering mode - ignore it
            }
        }

        #endregion

        #region Bezier render

        public void AddInk(Windows.UI.Input.Inking.InkStroke stroke)
        {
            try
            {
                var path = CreateBezierPath(stroke);

                // Update dictionary
                bezierPaths.Add(stroke, path); // throws ArgumentException if stroke is already in the dictionary

                // Add path to render so that it is rendered (on top of all the elements with same ZIndex).
                // We want the live render to be on top of the Bezier render, so we set the ZIndex of the elements of the
                // live render to 2 and that of the elements of the Bezier render to 1.
                render.Children.Add(path);
                Windows.UI.Xaml.Controls.Canvas.SetZIndex(path, 1);
            }
            catch (System.ArgumentException)
            {
                // ink is already present - ignore it
            }
        }

        public void AddInk(System.Collections.Generic.IEnumerable<Windows.UI.Input.Inking.InkStroke> strokes)
        {
            foreach (var stroke in strokes)
            {
                AddInk(stroke);
            }
        }

        public void UpdateSelection()
        {
            foreach (var sp in bezierPaths)
            {
                Windows.UI.Xaml.Shapes.Path selectionPath = null;
                bool selectionPathExists = selectionPaths.TryGetValue(sp.Key, out selectionPath);

                if (sp.Key.Selected && !selectionPathExists)
                {
                    // If stroke is selected, we render its contour with the stroke's color and we fill it with white.
                    // To do so, we increase the original thickness of the stroke's path and we add an additional path
                    // of the color of the background on top of it.

                    // Increase stroke's width
                    sp.Value.StrokeThickness = sp.Key.DrawingAttributes.Size.Width + 2;

                    // Create additional stroke and store it in _selectionPaths so that we will be able to remove it later on.
                    selectionPath = CreateBezierPath(sp.Key);
                    selectionPath.Stroke = render.Background;
                    selectionPath.StrokeThickness = sp.Key.DrawingAttributes.Size.Width;
                    selectionPaths.Add(sp.Key, selectionPath);

                    // Add path to render so that it is rendered (on top of all the elements with same ZIndex).
                    // We want the live render to be on top of the Bezier render, so we set the ZIndex of the elements of the
                    // live render to 1 and that of the elements of the Bezier render to 2.
                    render.Children.Add(selectionPath);
                    Windows.UI.Xaml.Controls.Canvas.SetZIndex(selectionPath, 2);
                }
                else if (selectionPathExists)
                {
                    // If stroke is not selected, reset its path's width and remove its selection path
                    sp.Value.StrokeThickness = sp.Key.DrawingAttributes.Size.Width;
                    selectionPaths.Remove(sp.Key);
                    render.Children.Remove(selectionPath);
                }
            }
        }

        #endregion
    }
}
