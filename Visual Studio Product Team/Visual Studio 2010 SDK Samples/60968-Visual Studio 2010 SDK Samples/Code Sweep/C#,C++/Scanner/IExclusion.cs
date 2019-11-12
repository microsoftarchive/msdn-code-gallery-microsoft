/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Samples.VisualStudio.CodeSweep.Scanner
{
    /// <summary>
    /// An exclusion for a specific term, used to suppress a hit on the term if it appears in a
    /// particular context.
    /// </summary>
    public interface IExclusion
    {
        /// <summary>
        /// Gets the case-insensitive text of the exclusion.
        /// </summary>
        string Text { get; }

        /// <summary>
        /// Gets the search term to which this exclusion applies.
        /// </summary>
        ISearchTerm Term { get; }
    }
}
