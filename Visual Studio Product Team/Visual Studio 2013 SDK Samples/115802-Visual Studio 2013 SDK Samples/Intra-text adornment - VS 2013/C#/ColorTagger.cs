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
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;

namespace IntraTextAdornmentSample
{
    /// <summary>
    /// Determines which spans of text likely refer to color values.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is a data-only component. The tagging system is a good fit for presenting data-about-text.
    /// The <see cref="ColorAdornmentTagger"/> takes color tags produced by this tagger and creates corresponding UI for this data.
    /// </para>
    /// <para>
    /// This class is a sample usage of the <see cref="RegexTagger"/> utility base class.
    /// </para>
    /// </remarks>
    internal sealed class ColorTagger : RegexTagger<ColorTag>
    {
        internal ColorTagger(ITextBuffer buffer)
            : base(buffer, new[] { new Regex(@"\b[\dA-F]{6}\b", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase) })
        {
        }

        protected override ColorTag TryCreateTagForMatch(Match match)
        {
            Color color = ParseColor(match.ToString());

            Debug.Assert(match.Length == 6);

            return new ColorTag(color);
        }

        private static Color ParseColor(string hexColor)
        {
            int number;

            if (!Int32.TryParse(hexColor, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out number))
            {
                Debug.Fail("unable to parse " + hexColor);
                return Colors.Transparent;
            }

            byte r = (byte)(number >> 16);
            byte g = (byte)(number >> 8);
            byte b = (byte)(number >> 0);

            return Color.FromRgb(r, g, b);
        }
    }
}
