//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;

namespace simpleInk
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario1 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        SDKTemplate.MainPage rootPage = SDKTemplate.MainPage.Current;

        Windows.UI.Input.Inking.InkManager inkManager = null;
        Windows.UI.Input.Inking.InkDrawingAttributes drawingAttributes = null;
        Windows.UI.Input.Inking.InkDrawingAttributes lassoAttributes = null;
        Rendering.XamlInkRenderer renderer = null;

        // Stores the id of the 'active' pointer, -1 if none. We are allowing only one 'active' pointer
        // at a time. This variable is set by InkingArea_PointerPressed/SelectionRect_PointerPressed and
        // it is reset by InkingArea_PointerReleased/SelectionRect_PointerReleased.
        int pointerId = -1;

        public Scenario1()
        {
            this.InitializeComponent();

            rootPage.NotifyUser(" ", SDKTemplate.NotifyType.StatusMessage);

            // Initialize drawing attributes. These are used in inking mode.
            drawingAttributes = new Windows.UI.Input.Inking.InkDrawingAttributes();
            drawingAttributes.Color = Windows.UI.Colors.Red;
            double penSize = 2 + 2*PenThickness.SelectedIndex;
            drawingAttributes.Size = new Windows.Foundation.Size(penSize, penSize);
            drawingAttributes.IgnorePressure = true;
            drawingAttributes.FitToCurve = true;

            // Initialize lasso attributes. These are used in selection mode.
            lassoAttributes = new Windows.UI.Input.Inking.InkDrawingAttributes();
            lassoAttributes.Color = Windows.UI.Colors.Goldenrod;
            lassoAttributes.PenTip = Windows.UI.Input.Inking.PenTipShape.Circle;
            lassoAttributes.Size = new Windows.Foundation.Size(0.5f, 0.5f);

            // Create the InkManager and set the drawing attributes
            inkManager = new Windows.UI.Input.Inking.InkManager();
            inkManager.SetDefaultDrawingAttributes(drawingAttributes);

            renderer = new Rendering.XamlInkRenderer(InkingArea);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached. The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
        }

        /// <summary>
        /// Invoked immediately after this page is unloaded and is no longer the current
        //     source of a parent Frame.
        /// </summary>
        /// <param name="e">Event data that describes the navigation that has unloaded this page.</param>
        protected override void OnNavigatedFrom(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
        }

        void AnchorSelection()
        {
            // Resize SelecionRect and remove and disable manipulation
            SelectionRect.Height = 0;
            SelectionRect.Width = 0;
            SelectionRect.ManipulationMode = Windows.UI.Xaml.Input.ManipulationModes.None;
        }

        
        void InkingArea_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            // Make sure no pointer is already inking (we allow only one 'active' pointer at a time)
            if (pointerId == -1)
            {
                var pointerPoint = e.GetCurrentPoint(InkingArea);

                rootPage.NotifyUser(" ", SDKTemplate.NotifyType.StatusMessage);

                // Determine ink manipulation mode
                switch (Helpers.GetPointerEventType(e))
                {
                    case Helpers.PointerEventType.Erase:
                        rootPage.NotifyUser("Erase mode: draw a line across the ink you want to erase.", SDKTemplate.NotifyType.StatusMessage);
                        inkManager.Mode = Windows.UI.Input.Inking.InkManipulationMode.Erasing;
                        break;
                    case Helpers.PointerEventType.Ink:
                        inkManager.Mode = Windows.UI.Input.Inking.InkManipulationMode.Inking;
                        renderer.EnterLiveRendering(pointerPoint, drawingAttributes);
                        break;
                    case Helpers.PointerEventType.Select:
                        rootPage.NotifyUser("Select mode: draw a contour around the ink you want to select.", SDKTemplate.NotifyType.StatusMessage);
                        inkManager.Mode = Windows.UI.Input.Inking.InkManipulationMode.Selecting;
                        renderer.EnterLiveRendering(pointerPoint, lassoAttributes);
                        break;
                    default:
                        return; // pointer is neither inking nor erasing nor selecting: do nothing
                }

                // Clear selection
                inkManager.ClearSelection();
                renderer.UpdateSelection();
                AnchorSelection();

                inkManager.ProcessPointerDown(pointerPoint);              

                pointerId = (int)pointerPoint.PointerId; // save pointer id so that no other pointer can ink until this one is released
            }
        }

        void InkingArea_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var pointerPoint = e.GetCurrentPoint(InkingArea);

            if (pointerId == (int)pointerPoint.PointerId)
            {
                switch (inkManager.Mode)
                {
                    case Windows.UI.Input.Inking.InkManipulationMode.Erasing:
                        // Check if something has been erased.
                        // In erase mode InkManager::ProcessPointerUpdate returns an invalidate
                        // rectangle: if it is not degenerate something has been erased
                        // In erase mode we don't bother processing intermediate points
                        var invalidateRect = (Windows.Foundation.Rect)inkManager.ProcessPointerUpdate(e.GetCurrentPoint(InkingArea));
                        if (invalidateRect.Height != 0 && invalidateRect.Width != 0)
                        {
                            // We don't know what has been erased so we clear the render
                            // and add back all the ink saved in the ink manager
                            renderer.Clear();
                            renderer.AddInk(inkManager.GetStrokes());
                        }
                        break;
                    case Windows.UI.Input.Inking.InkManipulationMode.Inking:
                    case Windows.UI.Input.Inking.InkManipulationMode.Selecting:
                        // Process intermediate points
                        var intermediatePoints = e.GetIntermediatePoints(InkingArea);
                        for (int i = intermediatePoints.Count - 1; i >= 0; i--)
                        {
                            inkManager.ProcessPointerUpdate(intermediatePoints[i]);
                        }

                        // Live rendering
                        renderer.UpdateLiveRender(pointerPoint);
                        break;
                }
            }
        }

        void InkingArea_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var pointerPoint = e.GetCurrentPoint(InkingArea);

            if (pointerId == (int)pointerPoint.PointerId)
            {
                pointerId = -1; // reset pointerId so that other pointers may enter live rendering mode

                var rect = inkManager.ProcessPointerUp(pointerPoint);
                switch (inkManager.Mode)
                {
                    case Windows.UI.Input.Inking.InkManipulationMode.Inking:
                        renderer.ExitLiveRendering(pointerPoint);
                        renderer.AddInk(inkManager.GetStrokes()[inkManager.GetStrokes().Count - 1]); // Add last stroke that was created to the Bezier render
                        break;

                    case Windows.UI.Input.Inking.InkManipulationMode.Selecting:
                        renderer.ExitLiveRendering(pointerPoint);

                        if (inkManager.AnySelected())
                        {
                            // Something has been selected

                            // Notify the renderer to update the selection
                            renderer.UpdateSelection();

                            // Resize SelectionRect and register event handlers to move the selection
                            Windows.UI.Xaml.Controls.Canvas.SetLeft(SelectionRect, rect.Left);
                            Windows.UI.Xaml.Controls.Canvas.SetTop(SelectionRect, rect.Top);
                            SelectionRect.Width = rect.Width;
                            SelectionRect.Height = rect.Height;
                            SelectionRect.ManipulationMode = Windows.UI.Xaml.Input.ManipulationModes.TranslateX | Windows.UI.Xaml.Input.ManipulationModes.TranslateY;
                        }
                        break;
                }

                rootPage.NotifyUser(" ", SDKTemplate.NotifyType.StatusMessage);
            }
        }

        void InkingArea_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            // More complicated logics can be created, but for this simple example we pretend that
            // the pointer is released when it leaves the inking area
            InkingArea_PointerReleased(sender, e);
        }

        
        
        void SelectionRect_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            // Get current position of selection's bounding box
            var curPos = new Windows.Foundation.Point(
                Windows.UI.Xaml.Controls.Canvas.GetLeft(SelectionRect),
                Windows.UI.Xaml.Controls.Canvas.GetTop(SelectionRect)
            );

            // Compute new position, making sure that the bounding box does not go outside the InkingArea
            var newPos = new Windows.Foundation.Point(
                Math.Max(0, Math.Min(curPos.X + e.Delta.Translation.X, InkingArea.ActualWidth - SelectionRect.Width)),
                Math.Max(0, Math.Min(curPos.Y + e.Delta.Translation.Y, InkingArea.ActualHeight - SelectionRect.Height))
            );

            // Compute the actual translation to pass to InkManager.MoveSelected
            var translation = new Windows.Foundation.Point(
                newPos.X - curPos.X,
                newPos.Y - curPos.Y
            );

            if (Math.Abs(translation.X) > 0 || Math.Abs(translation.Y) > 0)
            {
                // Move the selection's bounding box
                Windows.UI.Xaml.Controls.Canvas.SetLeft(SelectionRect, newPos.X);
                Windows.UI.Xaml.Controls.Canvas.SetTop(SelectionRect, newPos.Y);

                // Move selected ink
                inkManager.MoveSelected(translation);

                // Re-render everything
                renderer.Clear();
                renderer.AddInk(inkManager.GetStrokes());
                renderer.UpdateSelection();
            }

            e.Handled = true;
        }

        
        
        void OnClear(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            rootPage.NotifyUser(" ", SDKTemplate.NotifyType.StatusMessage);

            pointerId = -1;

            if (inkManager.AnySelected())
            {
                inkManager.DeleteSelected();

                // We don't know what has been erased so we clear the render
                // and add back all the ink saved in the ink manager
                renderer.Clear();
                renderer.AddInk(inkManager.GetStrokes());

                // There is no selection - disable movement
                AnchorSelection();
            }
            else
            {
                inkManager.DeleteAll();
                renderer.Clear();
            }
        }

        async void OnLoad(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker();
            openPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".gif");
            Windows.Storage.StorageFile file = await openPicker.PickSingleFileAsync();
            if (null != file)
            {
                using (var stream = await file.OpenSequentialReadAsync())
                {
                    await inkManager.LoadAsync(stream);
                }

                renderer.Clear();
                renderer.AddInk(inkManager.GetStrokes());

                // There is no selection - disable movement
                AnchorSelection();

                rootPage.NotifyUser(inkManager.GetStrokes().Count + " strokes loaded!", SDKTemplate.NotifyType.StatusMessage);
            }
        }

        async void OnSave(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // We don't want to save an empty file
            if (inkManager.GetStrokes().Count > 0)
            {
                var savePicker = new Windows.Storage.Pickers.FileSavePicker();
                savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
                savePicker.FileTypeChoices.Add("Gif with embedded ISF", new System.Collections.Generic.List<string> { ".gif" });

                Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
                if (null != file)
                {
                    using (var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
                    {
                        await inkManager.SaveAsync(stream);

                        rootPage.NotifyUser(inkManager.GetStrokes().Count + " strokes saved!", SDKTemplate.NotifyType.StatusMessage);
                    }
                }
            }
            else
            {
                rootPage.NotifyUser("There is no ink to save.", SDKTemplate.NotifyType.ErrorMessage);
            }
        }

        void OnPenColorChanged(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // Use button's background to set new pen's color
            var btnSender = sender as Windows.UI.Xaml.Controls.Button;
            var brush = btnSender.Background as Windows.UI.Xaml.Media.SolidColorBrush;

            drawingAttributes.Color = brush.Color;
            inkManager.SetDefaultDrawingAttributes(drawingAttributes);
        }

        void OnPenThicknessChanged(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (PenThickness != null)
            {
                double penSize = 2 + 2 * PenThickness.SelectedIndex;
                drawingAttributes.Size = new Windows.Foundation.Size(penSize, penSize);
                inkManager.SetDefaultDrawingAttributes(drawingAttributes);
            }
        }

        void OnCopy(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (inkManager.AnySelected())
            {
                inkManager.CopySelectedToClipboard();
            }
            else
            {
                rootPage.NotifyUser("Must first select something to copy.", SDKTemplate.NotifyType.ErrorMessage);
            }
        }

        void OnCut(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (inkManager.AnySelected())
            {
                inkManager.CopySelectedToClipboard();
                inkManager.DeleteSelected();

                // There is no selection - disable movement
                AnchorSelection();

                // We don't know what has been erased so we clear the render and add back all the ink saved in the ink manager
                renderer.Clear();
                renderer.AddInk(inkManager.GetStrokes());
            }
            else
            {
                rootPage.NotifyUser("Must first select something to cut.", SDKTemplate.NotifyType.ErrorMessage);
            }
        }

        void OnPaste(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (inkManager.CanPasteFromClipboard())
            {
                inkManager.PasteFromClipboard(new Windows.Foundation.Point(10, 10));

                // We don't know what has been added so we clear the render and add back all the ink saved in the ink manager
                renderer.Clear();
                renderer.AddInk(inkManager.GetStrokes());
                renderer.UpdateSelection();
            }
            else
            {
                rootPage.NotifyUser("The clipboard does not contain ink compatible objects. Cannot paste from clipboard.", SDKTemplate.NotifyType.ErrorMessage);
            }
        }

        async void OnRecognize(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (inkManager.GetStrokes().Count > 0)
            {
                // the following call to RecognizeAsync may fail for various reasons, most notably if another recognition is in progress
                try
                {
                    var recognitionResults = await inkManager.RecognizeAsync(inkManager.AnySelected() ? Windows.UI.Input.Inking.InkRecognitionTarget.Selected : Windows.UI.Input.Inking.InkRecognitionTarget.All);

                    // Save recognition results to inkManager
                    inkManager.UpdateRecognitionResults(recognitionResults);

                    // Display recognition result
                    String str = "Recognition result:";
                    foreach (var r in recognitionResults)
                    {
                        str += " " + r.GetTextCandidates()[0];
                    }
                    rootPage.NotifyUser(str, SDKTemplate.NotifyType.StatusMessage);
                }
                catch (System.Exception se)
                {
                    rootPage.NotifyUser("Recognize error " + se.HResult, SDKTemplate.NotifyType.ErrorMessage);
                }
            }
            else
            {
                rootPage.NotifyUser("Must first write something.", SDKTemplate.NotifyType.ErrorMessage);
            }
        }

        
    }
}
