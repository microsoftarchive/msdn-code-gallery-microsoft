// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="ThumbnailedImageDateComparer.cs">
//   Copyright (c) 2012 - 2014 Microsoft Corporation. All rights reserved.
// </copyright>
// <summary>
//   Use of this sample source code is subject to the terms of the Microsoft license
//   agreement under which you licensed this sample source code and is provided AS-IS.
//   If you did not accept the terms of the license agreement, you are not authorized
//   to use this sample source code. For the terms of the license, please see the
//   license agreement between you and Microsoft.<br/><br/>
//   To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604.
// </summary>
// ----------------------------------------------------------------------------

namespace Microsoft.Phone.Controls
{
    using System.Collections.Generic;

    /// <summary>
    /// Compares two <see cref="IThumbnailedImage"/> instances by <see cref="IThumbnailedImage.DateTaken"/>.
    /// </summary>
    /// <remarks>
    /// This code is taken from the Code Samples for Windows Phone (http://code.msdn.microsoft.com/wpapps).
    /// </remarks>
    public class ThumbnailedImageDateComparer : IComparer<IThumbnailedImage>
    {
        /// <summary>
        /// Compare two IThumbnailedImage instances by DateTaken
        /// </summary>
        /// <param name="x">First IThumbnailedImage to examine</param>
        /// <param name="y">IThumbnailedImage to compare to the first IThumbnailedImage</param>
        /// <returns></returns>
        public int Compare(IThumbnailedImage x, IThumbnailedImage y)
        {
            return x.DateTaken.CompareTo(y.DateTaken);
        }
    }
}
