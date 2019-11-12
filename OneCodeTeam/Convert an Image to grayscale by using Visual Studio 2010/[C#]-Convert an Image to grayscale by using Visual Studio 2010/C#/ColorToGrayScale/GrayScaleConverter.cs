using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace ColorToGrayScale
{
    /// <summary>
    /// This class represents an example of converting a colored image to a gray scale.
    /// </summary>
    public partial class GrayScaleConverterForm : Form
    {
        private PictureBox PctSourceImage; // Variable to store source colored image.
        private PictureBox PctOutputImage; // Variable to store output gray scale image.
        private string SourcePath; // Variable to store source image file path.
        private Bitmap OutputBitmap; // Bitmap handle to gray scale image.

        public GrayScaleConverterForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Load event of the windows form.
        /// </summary>

        private void GrayScaleConverterForm_Load(object sender, EventArgs e)
        {
            // Dynamically adding source image PictureBox control in the form.
            PctSourceImage = new PictureBox();
            this.Controls.Add(PctSourceImage);
            PctSourceImage.Location = new Point(0, 0);
            PctSourceImage.Size = new Size(this.Width / 2 - 1, this.Height);
            PctSourceImage.SizeMode = PictureBoxSizeMode.StretchImage;

            // Dynamically adding output image PictureBox control in the form.
            PctOutputImage = new PictureBox();
            this.Controls.Add(PctOutputImage);
            PctOutputImage.Location = new Point(this.Width / 2 + 1, 0);
            PctOutputImage.Size = new Size(this.Width / 2 - 1, this.Height);
            PctOutputImage.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        /// <summary>
        /// Paint event of the windows form.
        /// </summary>

        private void GrayScaleConverterForm_Paint(object sender, PaintEventArgs e)
        {
            // Drawing a vertical separator between source and output images.
            Pen pen = new Pen(Color.FromArgb(255, 0, 150, 0));
            e.Graphics.DrawLine(pen, this.Width / 2, 0, this.Width / 2, this.Height);
            e.Graphics.DrawLine(pen, this.Width / 2 - 1, 0, this.Width / 2 - 1, this.Height);
            e.Graphics.DrawLine(pen, this.Width / 2 + 1, 0, this.Width / 2 + 1, this.Height);
        }

        /// <summary>
        /// This function initializes OpenFileDialog box to choose the source image file.
        /// </summary>

        private void mnuChooseFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog chooseFile = new OpenFileDialog();
            chooseFile.ShowDialog();
            if (chooseFile.FileName != "")
            {
                PctSourceImage.Image = null;
                PctSourceImage.ImageLocation = chooseFile.FileName;
                SourcePath = chooseFile.FileName;
            }
        }

        /// <summary>
        /// This function converts a colored image to gray scale.
        /// </summary>
        /// <param name="originalBitmap">
        /// Bitmap handle to the source colored image.
        /// </param>
        /// <returns>
        /// Bitmap handle of the converted gray scale image.
        /// </returns>

        private static Bitmap ConvertToGrayScaleImage(Bitmap originalBitmap)
        {
            // A blank bitmap is created having same size as original bitmap image.
            Bitmap GrayScaleBitmap = new Bitmap(originalBitmap.Width, originalBitmap.Height);

            // Initializing a graphics object from the new image bitmap.
            Graphics graphics = Graphics.FromImage(GrayScaleBitmap);

            // Creating the Grayscale ColorMatrix whose values are determined by
            // calculating the luminosity of a color, which is a weighted average of the
            // RGB color components. The average is weighted according to the sensitivity
            // of the human eye to each color component. The weights used here are as
            // given by the NTSC (North America Television Standards Committee)
            // and are widely accepted.
            ColorMatrix colorMatrix = new ColorMatrix(new float[][]
            {
                new float[] { 0.299f, 0.299f, 0.299f, 0, 0 }, 
                new float[] { 0.587f, 0.587f, 0.587f, 0, 0 }, 
                new float[] { 0.114f, 0.114f, 0.114f, 0, 0 }, 
                new float[] { 0,      0,      0,      1, 0 }, 
                new float[] { 0,      0,      0,      0, 1 } 
            });

            // Creating image attributes.
            ImageAttributes attributes = new ImageAttributes();

            // Setting the color matrix attribute.
            attributes.SetColorMatrix(colorMatrix);

            // Drawing the original bitmap image on the new bitmap image using the
            // Grayscale color matrix.
            graphics.DrawImage(originalBitmap, new Rectangle(0, 0, originalBitmap.Width,
                originalBitmap.Height), 0, 0, originalBitmap.Width,
                originalBitmap.Height, GraphicsUnit.Pixel, attributes);

            // Disposing the Graphics object.
            graphics.Dispose();
            return GrayScaleBitmap;
        }

        /// <summary>
        /// Click event handler for 'mnuConvert' toolstrip menu item which calls
        /// 'ConvertToGrayscaleImage' function.
        /// </summary>

        private void mnuConvert_Click(object sender, EventArgs e)
        {
            if (PctSourceImage.Image == null)
            {
                MessageBox.Show("Please choose a source image file.",
                    "Gray Scale Converter", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
            else
            {
                try
                {
                    OutputBitmap = ConvertToGrayScaleImage(
                        new Bitmap(PctSourceImage.Image));
                    PctOutputImage.Image = ConvertToGrayScaleImage(
                        new Bitmap(PctSourceImage.Image));
                    MessageBox.Show("The color image has been successfully converted " +
                        "to gray scale.", "Gray Scale Converter", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (FileNotFoundException fileNotFoundException)
                {
                    MessageBox.Show("Error encountered : " +
                        fileNotFoundException.Message, "Gray Scale Converter",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (ExternalException externalException)
                {
                    MessageBox.Show("Error encountered : " + externalException.Message,
                        "Gray Scale Converter", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                catch (IndexOutOfRangeException indexOutOfRangeException)
                {
                    MessageBox.Show("Error encountered : " +
                        indexOutOfRangeException.Message, "Gray Scale Converter",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Error encountered : " + exception.Message,
                        "Gray Scale Converter", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Click event handler for 'mnuSaveOutput' toolstrip menu item which saves
        /// the converted grey scale image to a local directory.
        /// </summary>

        private void mnuSaveOutput_Click(object sender, EventArgs e)
        {
            string sourceImageExtension = null;

            if (PctOutputImage.Image == null)
            {
                MessageBox.Show("Cannot find gray scale image in the output area.",
                    "Gray Scale Converter", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
            else
            {
                sourceImageExtension = SourcePath.Split('.')[1];
                SaveFileDialog saveImageFile = new SaveFileDialog();
                saveImageFile.Filter = sourceImageExtension.ToUpper() + " Image (*." +
                    sourceImageExtension + ")|*." + sourceImageExtension;
                saveImageFile.ShowDialog();

                if (saveImageFile.FileName != "")
                {
                    OutputBitmap.Save(saveImageFile.FileName);
                    if (OutputBitmap != null)
                    {
                        OutputBitmap.Dispose();
                    }
                    Process.Start(saveImageFile.FileName);
                }
            }
        }

        /// <summary>
        /// Click event handler for 'mnuReset' toolstrip menu item which resets/removes
        /// source and output images from the respective PictureBox.
        /// </summary>

        private void mnuReset_Click(object sender, EventArgs e)
        {
            PctOutputImage.Image = null;
            PctSourceImage.Image = null;
            MessageBox.Show("Image areas have been reset sucessfully.",
                "Gray Scale Converter", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
