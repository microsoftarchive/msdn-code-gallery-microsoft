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
    /// The result of a scan on a single file.
    /// </summary>
    public interface IScanResult
    {
        /// <summary>
        /// Gets the full path of the file that was scanned.
        /// </summary>
        string FilePath { get; }

        /// <summary>
        /// Gets the scan hits found in the file.
        /// </summary>
        IEnumerable<IScanHit> Results { get; }

        /// <summary>
        /// Gets a boolean value indicating whether the scan was performed.
        /// </summary>
        bool Scanned { get; }

        /// <summary>
        /// Gets a boolean value indicating whether the scan had zero hits.
        /// </summary>
        bool Passed { get; }
    }
}
