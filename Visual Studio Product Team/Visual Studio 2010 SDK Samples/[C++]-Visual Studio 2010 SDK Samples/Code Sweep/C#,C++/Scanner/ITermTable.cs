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
    /// A table of search terms.
    /// </summary>
    public interface ITermTable
    {
        /// <summary>
        /// Gets the full path of the file from which this table was loaded.
        /// </summary>
        string SourceFile { get; }

        /// <summary>
        /// Gets the terms defined in this table.
        /// </summary>
        IEnumerable<ISearchTerm> Terms { get; }
    }
}
