//***************************************************************************
//
//    Copyright (c) Microsoft Corporation. All rights reserved.
//    This code is licensed under the Visual Studio SDK license terms.
//    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
//    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
//    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
//    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//***************************************************************************


using System;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace ToDoGlyphFactory
{
    /// <summary>
    /// This class implements IGlyphFactory, which provides the visual
    /// element that will appear in the glyph margin.
    /// </summary>
    internal class ToDoGlyphFactory : IGlyphFactory
    {
        #region Private Members
        const double _glyphSize = 16.0;
        #endregion

        #region IGlyphFactory members

        /// <summary>
        /// This method is responsible for returning a UIElement to be drawn in the
        /// glyph margin.
        /// </summary>
        /// <param name="line">The line we are generating the glyph for.</param>
        /// <param name="tag">A GlyphTag.  We will just be making sure this is of
        /// the correct type (ToDoTag).</param>
        /// <returns>The UIElement we want drawn in the GlyphMargin.</returns>
        public UIElement GenerateGlyph(IWpfTextViewLine line, IGlyphTag tag)
        {
            // Ensure we can draw a glyph for this marker.
            if (tag == null || !(tag is ToDoTag))
            {
                return null;
            }

            // Draw a green circle with a red border
            System.Windows.Shapes.Ellipse ellipse = new Ellipse();
            ellipse.Fill = Brushes.Green;
            ellipse.StrokeThickness = 1;
            ellipse.Stroke = Brushes.Red;
            ellipse.Height = _glyphSize;
            ellipse.Width = _glyphSize;

            return ellipse;
        }
        #endregion
    }
}
