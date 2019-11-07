// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)


using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;


namespace UiManagerOutlookAddIn.AddinUtilities
{
    // This interface is not CLS compliant because of its Office property.
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("43189577-8667-4c8f-8167-EBF23CC285CB")]
    [CLSCompliant(false)]
    public interface IRibbonConnector
    {
        Microsoft.Office.Core.IRibbonUI Ribbon
        {
            get;
            set;
        }
    }


    // Regasm won't register an assembly that only contains interfaces.
    // We need to define a COM-createable class in order to get a typelib.
    // We don't want to use this class for anything, because we're
    // implementing the interface in another assembly.
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [CLSCompliant(false)]
    public class RibbonConnectorPlaceholder : IRibbonConnector 
    {
        public Microsoft.Office.Core.IRibbonUI Ribbon
        {
            get { return null; }
            set { }
        }
    }

}
