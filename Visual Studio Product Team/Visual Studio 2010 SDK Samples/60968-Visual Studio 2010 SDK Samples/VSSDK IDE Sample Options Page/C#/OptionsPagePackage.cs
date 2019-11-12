/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.Samples.VisualStudio.IDE.OptionsPage
{

    /// <summary>
    /// This class implements Visual studio package that is registered within Visual Studio IDE.
    /// The VSPackage class uses a number of registration attribute to specify integration parameters.
    /// Implementation of IVsUserSettings within the package allows to persist a VSPackage configuration
    /// </summary>
    /// <remarks>
    /// <para>A description of the different attributes used here is given below:</para>
    /// <para>ProvideOptionPageAttribute: This registers a given class for persisting part 
    /// or all of VSPackage's state through the Visual Studio settings mechanism. 
    /// </para>
    /// <para>ProvideProfileAttribute: This registers that a specific independent class implements 
    /// a specific instance of Visual Studio settings functionality for the VSPackage, 
    /// allowing it to save and retrieve VSPackage state information.
    /// </para>
    /// </remarks>
    
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideOptionPageAttribute(typeof(OptionsPageGeneral),"My Options Page (C#)","General", 101, 106, true)]
    [ProvideProfileAttribute(typeof(OptionsPageGeneral), "My Options Page (C#)", "General Options", 101, 106, true, DescriptionResourceID = 101)]
    [ProvideOptionPageAttribute(typeof(OptionsPageCustom), "My Options Page (C#)", "Custom", 101, 107, true)]
    [InstalledProductRegistration("My Options Page (C#)", "My Options Page (C#) Sample", "1.0")]
    [Guid(GuidStrings.GuidPackage)]
    public class OptionsPagePackageCS : Package
    {
        /// <summary>
        /// Initialization of the package.
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();
        }
    }
}
