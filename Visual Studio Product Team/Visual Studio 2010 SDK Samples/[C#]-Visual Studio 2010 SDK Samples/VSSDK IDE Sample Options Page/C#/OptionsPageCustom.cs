/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;

namespace Microsoft.Samples.VisualStudio.IDE.OptionsPage
{
	/// <summary>
    /// Extends a standard dialog functionality for implementing ToolsOptions pages, 
    /// with support for the Visual Studio automation model, Windows Forms, and state 
    /// persistence through the Visual Studio settings mechanism.
	/// </summary>
	[Guid(GuidStrings.GuidPageCustom)]
	public class OptionsPageCustom : DialogPage
    {
        #region Fields
        // The path to the image file which must be shown.
        private string selectedImagePath = String.Empty;
        #endregion Fields

        #region Properties
        /// <summary>
        /// Gets the window an instance of DialogPage that it uses as its user interface.
        /// </summary>
		/// <devdoc>
		/// The window this dialog page will use for its UI.
		/// This window handle must be constant, so if you are
		/// returning a Windows Forms control you must make sure
		/// it does not recreate its handle.  If the window object
		/// implements IComponent it will be sited by the 
		/// dialog page so it can get access to global services.
		/// </devdoc>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected override IWin32Window Window
		{
			get
			{
				OptionsCompositeControl optionsControl = new OptionsCompositeControl();
                optionsControl.Location = new Point(0, 0);
                optionsControl.OptionsPage = this;
                return optionsControl;
			}
		}
		/// <summary>
		/// Gets or sets the path to the image file.
		/// </summary>
        /// <remarks>The property that needs to be persisted.</remarks>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public string CustomBitmap
		{
			get 
			{
				return selectedImagePath;
			}
			set
			{
				selectedImagePath = value;
			}
        }
        #endregion Properties
    }
}
