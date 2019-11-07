// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)


using System;
using System.Runtime.InteropServices;
using Outlook = Microsoft.Office.Interop.Outlook;
using Microsoft.Vbe.Interop.Forms;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

// Need to add reference to Microsoft.Vbe.Interop.Forms.
// COM tab: Microsoft Forms 2.0 Object Library (the first one if there are more than one).
namespace UiManagerOutlookAddIn
{

    [ComVisible(true)]
    [Guid("38F28415-204F-479a-B9B2-A386A462F7DE")]
    [ProgId("UiManager.FormRegionConnector")]
    public class FormRegionConnector : Outlook.FormRegionStartup
    {

        #region Fields

        private const String _formRegionName = "SimpleFormRegion";

        #endregion


        #region FormRegionStartup implementation

        // This method is not CLSCompliant because of its Office parameters.
        [CLSCompliant(false)]
        public object GetFormRegionStorage(
            string FormRegionName, object Item, int LCID,
            Outlook.OlFormRegionMode FormRegionMode, 
            Outlook.OlFormRegionSize FormRegionSize)
        {
            Application.DoEvents();
            switch (FormRegionName)
            {
                case _formRegionName:
                    return Properties.Resources.SimpleFormRegionOfs;
                default:
                    return null;
            }
        }

        // This method is not CLSCompliant because of its Office parameter.
        [CLSCompliant(false)]
        public object GetFormRegionIcon(
            string FormRegionName, int LCID, Outlook.OlFormRegionIcon Icon)
        {
            return PictureConverter.ImageToPictureDisp(
                Properties.Resources.espressoCup);
        }

        public object GetFormRegionManifest(string FormRegionName, int LCID)
        {
            return Properties.Resources.SimpleFormRegionXml;
        }

        // This method is not CLSCompliant because of its Office parameter.
        [CLSCompliant(false)]
        public void BeforeFormRegionShow(Outlook.FormRegion FormRegion)
        {
            if (FormRegion != null)
            {
                // Create a new wrapper for the form region controls, 
                // and add it to our collection.
                Globals.ThisAddIn._uiElements.AttachFormRegion(
                    FormRegion.Inspector, new FormRegionControls(FormRegion));
            }
        }

        #endregion

    }
}
