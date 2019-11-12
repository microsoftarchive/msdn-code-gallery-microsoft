//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SwapChainPanel
{
    /// <summary>
    /// A control that displays drawing options for a drawing surface when bound to an data model.
    /// </summary>
    public sealed partial class PaletteControl : UserControl
    {
        public PaletteControl()
        {
            this.InitializeComponent();
        }

        private void MoreColorsPath_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            // Update the data model when a new color is added to the active color palette
            if (sender is Windows.UI.Xaml.Shapes.Path &&
                ((Windows.UI.Xaml.Shapes.Path)sender).Fill is SolidColorBrush &&
                DataContext is Model.DrawingAttributes)
            {
                var drawingContext = (Model.DrawingAttributes)DataContext;
                var path = (Windows.UI.Xaml.Shapes.Path)sender;
                var color = ((SolidColorBrush)path.Fill).Color;

                drawingContext.ActivePaletteColors.Add(color);
                drawingContext.BrushColor = color;
                PaletteListBox.SelectedItem = color;
            }

            MoreColorsPopup.IsOpen = false;
        }
    }
}
