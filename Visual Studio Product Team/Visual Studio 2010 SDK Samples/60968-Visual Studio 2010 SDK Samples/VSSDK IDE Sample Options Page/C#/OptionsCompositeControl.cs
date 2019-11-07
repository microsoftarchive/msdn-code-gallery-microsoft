/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.Samples.VisualStudio.IDE.OptionsPage
{
	/// <summary>
	/// This class implements UI for the Custom options page.
    /// It uses OptionsPageCustom object as a data objects.
	/// </summary>
	public class OptionsCompositeControl : System.Windows.Forms.UserControl
	{
        #region Fields

        private PictureBox pictureBox;
        private OpenFileDialog openImageFileDialog;
        private Button buttonChooseImage;
        private Button buttonClearImage;
        private OptionsPageCustom customOptionsPage; 

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Explicitly defined default constructor.
        /// Initializes new instance of OptionsCompositeControl class.
        /// </summary>
        public OptionsCompositeControl()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        #endregion

        #region Methods

        #region IDisposable implementation
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if( openImageFileDialog != null )
                {
                    openImageFileDialog.Dispose();
                    openImageFileDialog = null;
                }
                if (components != null)
                {
                    components.Dispose();
                }
                GC.SuppressFinalize(this);
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.openImageFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.buttonChooseImage = new System.Windows.Forms.Button();
            this.buttonClearImage = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(16, 16);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(264, 120);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            // 
            // buttonChooseImage
            // 
            this.buttonChooseImage.Location = new System.Drawing.Point(16, 152);
            this.buttonChooseImage.Name = "buttonChooseImage";
            this.buttonChooseImage.Size = new System.Drawing.Size(112, 23);
            this.buttonChooseImage.TabIndex = 1;
            this.buttonChooseImage.Text = Resources.ChooseImageButtonText;
            this.buttonChooseImage.Click += new System.EventHandler(this.OnChooseImage);
            // 
            // buttonClearImage
            // 
            this.buttonClearImage.Location = new System.Drawing.Point(160, 152);
            this.buttonClearImage.Name = "buttonClearImage";
            this.buttonClearImage.Size = new System.Drawing.Size(96, 23);
            this.buttonClearImage.TabIndex = 2;
            this.buttonClearImage.Text = global::Microsoft.Samples.VisualStudio.IDE.OptionsPage.Resources.ButtonClearImageText;
            this.buttonClearImage.Click += new System.EventHandler(this.OnClearImage);
            // 
            // OptionsCompositeControl
            // 
            this.AllowDrop = true;
            this.Controls.Add(this.buttonClearImage);
            this.Controls.Add(this.buttonChooseImage);
            this.Controls.Add(this.pictureBox);
            this.Name = "OptionsCompositeControl";
            this.Size = new System.Drawing.Size(292, 195);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion
        /// <summary>
        /// Handles the ChooseImage event. 
        /// </summary>
        /// <param name="sender">The reference to contained object.</param>
        /// <param name="e">The event arguments.</param>
        private void OnChooseImage(object sender, System.EventArgs e)
        {
            openImageFileDialog = new OpenFileDialog();

            if ((openImageFileDialog != null) && (DialogResult.OK == openImageFileDialog.ShowDialog()))
            {
                if (customOptionsPage != null)
                {
                    customOptionsPage.CustomBitmap = openImageFileDialog.FileName;
                }
                RefreshImage();
            }
        }
        /// <summary>
        /// Handles the ClearImage event. 
        /// </summary>
        /// <param name="sender">The reference to contained object.</param>
        /// <param name="e">The event arguments.</param>
        private void OnClearImage(object sender, System.EventArgs e)
        {
            if (customOptionsPage != null)
            {
                customOptionsPage.CustomBitmap = null;
            }
            RefreshImage();
        }
        /// <summary>
        /// Refresh PictureBox Image data.
        /// </summary>
        /// <remarks>Image was reloaded from the file, specified by CustomBitmap (full path to the file).</remarks>
        private void RefreshImage()
        {
            if (customOptionsPage == null)
            {
                return;
            }

            string fileName = customOptionsPage.CustomBitmap;
            if (fileName != null && fileName.Length != 0)
            {
                // avoid to use Image.FromFile() method for loading image to exclude file locks
                using (FileStream lStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    pictureBox.Image = Image.FromStream(lStream);
                }
            }
            else
            {
                pictureBox.Image = null;
            }
        }
        /// <summary>
        /// Gets or Sets the reference to the underlying OptionsPage object.
        /// </summary>
        public OptionsPageCustom OptionsPage
        {
            get
            {
                return customOptionsPage;
            }
            set
            {
                customOptionsPage = value;
                RefreshImage();
            }
        } 
        #endregion
	}
}
