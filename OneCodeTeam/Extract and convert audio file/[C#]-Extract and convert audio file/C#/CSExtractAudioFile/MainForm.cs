/********************************** Module Header **********************************\
* Module Name:  MainForm.cs
* Project:      CSExtractAudioFile
* Copyright (c) Microsoft Corporation.
*
* The sample demonstrates how to extract and convert audio file formats, which 
* include wav, mp3 and mp4 files.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
* EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
* MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***********************************************************************************/

using System;
using System.IO;
using System.Security.Permissions;
using System.Windows.Forms;


namespace CSExtractAudioFile
{
    partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            this.cmbOutputAudioType.DataSource = Enum.GetValues(typeof(OutputAudioType));
            this.tbOutputDirectory.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        }

        /// <summary>
        /// Open the music file using the OpenFileDialog object.
        /// </summary>
        private void btnChooseSourceFile_Click(object sender, EventArgs e)
        {
            if (this.openAudioFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.player.URL = openAudioFileDialog.FileName;
                this.player.Ctlcontrols.play();

                this.btnChooseSourceFile.Text = openAudioFileDialog.FileName;
            }
        }

        /// <summary>
        /// Set the startpoint and the endpoint of the video clip.
        /// </summary>
        private void btnSetBeginEndPoints_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.player.URL))
            {
                if (btnSetBeginEndPoints.Tag.Equals("SetStartPoint"))
                {
                    // Save the startpoint.
                    // player.Ctlcontrols.currentPosition contains the current 
                    // position in the media item in seconds from the beginning.
                    this.tbStartpoint.Text = (player.Ctlcontrols.currentPosition * 1000).ToString("0");
                    this.tbEndpoint.Text = "";

                    this.btnSetBeginEndPoints.Text = "Set End Point";
                    this.btnSetBeginEndPoints.Tag = "SetEndPoint";
                }
                else if (btnSetBeginEndPoints.Tag.Equals("SetEndPoint"))
                {
                    // Check if the startpoint is in front of the endpoint.
                    int endPoint = (int)(player.Ctlcontrols.currentPosition * 1000);
                    if (endPoint <= int.Parse(this.tbStartpoint.Text))
                    {
                        MessageBox.Show("Audio endpoint is overlapped. Please reset the endpoint.");
                    }
                    else
                    {
                        // Save the endpoint.
                        this.tbEndpoint.Text = endPoint.ToString();

                        this.btnSetBeginEndPoints.Text = "Set Start Point";
                        this.btnSetBeginEndPoints.Tag = "SetStartPoint";
                    }
                }
            }
        }

        /// <summary>
        /// Extract the video clip.
        /// </summary>
        private void btnExtract_Click(object sender, EventArgs e)
        {
            try
            {
                // Check parameters ...
                string sourceAudioFile = this.player.URL;
                if (string.IsNullOrEmpty(sourceAudioFile))
                {
                    throw new ArgumentException("Please choose the source audio file");
                }
                string outputDirectory = this.tbOutputDirectory.Text;
                if (string.IsNullOrEmpty(outputDirectory))
                {
                    throw new ArgumentException("Please specify the output directory");
                }
                string startpoint = this.tbStartpoint.Text;
                if (string.IsNullOrEmpty(tbStartpoint.Text))
                {
                    throw new ArgumentException("Please specify the startpoint of the audio clip");
                }
                string endpoint = this.tbEndpoint.Text;
                if (string.IsNullOrEmpty(endpoint))
                {
                    throw new ArgumentException("Please specify the endpoint of the audio clip");
                }

                // Extract the audio file.
                OutputAudioType outputType = (OutputAudioType)this.cmbOutputAudioType.SelectedValue;
                string outputFileName = ExpressionEncoderWrapper.ExtractAudio(
                    sourceAudioFile, outputDirectory, outputType, 
                    Double.Parse(startpoint), Double.Parse(endpoint));

                MessageBox.Show("Audio file is successfully extracted: " + outputFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        /// <summary>
        /// Select the output directory
        /// </summary>
        private void btnChooseOutputDirectory_Click(object sender, EventArgs e)
        {
            if (this.outputFolderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                this.tbOutputDirectory.Text = outputFolderBrowserDialog.SelectedPath;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Releases Windows Media Player resources.
            this.player.close();
        }
    }
}