'****************************** Module Header ******************************\
'Module Name:    Form1.vb
'Project:        VBColorToGrayScale2010
'Copyright (c) Microsoft Corporation

'The project illustrates how to convert a colored image to gray scale.

'This source is subject to the Microsoft Public License.
'See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
'All other rights reserved.

'*****************************************************************************/

Imports System.Drawing.Imaging

Public Class Form1

    Private PctSourceImage As PictureBox
    Private PctOutputImage As PictureBox
    Private SourcePath As String
    Private OutputBitmap As Bitmap

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub

    Private Sub GrayScaleConverterForm_Paint(ByVal sender As Object, ByVal e As PaintEventArgs) Handles MyBase.Paint
        ' Drawing a vertical separator between source and output images.
        Dim pen As New Pen(Color.FromArgb(255, 0, 150, 0))
        e.Graphics.DrawLine(pen, CInt(Me.Width / 2), 0, CInt(Me.Width / 2), Me.Height)
        e.Graphics.DrawLine(pen, CInt(Me.Width / 2 - 1), 0, CInt(Me.Width / 2 - 1), Me.Height)
        e.Graphics.DrawLine(pen, CInt(Me.Width / 2 + 1), 0, CInt(Me.Width / 2 + 1), Me.Height)
    End Sub

    Private Sub GrayScaleConverterForm_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        ' Dynamically adding source image PictureBox control in the form.
        PctSourceImage = New PictureBox()
        Me.Controls.Add(PctSourceImage)
        PctSourceImage.Location = New Point(0, 0)
        PctSourceImage.Size = New Size(Me.Width / 2 - 1, Me.Height)
        PctSourceImage.SizeMode = PictureBoxSizeMode.StretchImage

        ' Dynamically adding output image PictureBox control in the form.
        PctOutputImage = New PictureBox()
        Me.Controls.Add(PctOutputImage)
        PctOutputImage.Location = New Point(Me.Width / 2 + 1, 0)
        PctOutputImage.Size = New Size(Me.Width / 2 - 1, Me.Height)
        PctOutputImage.SizeMode = PictureBoxSizeMode.StretchImage
    End Sub

    Private Sub mnuChooseFile_Click(ByVal sender As Object, ByVal e As EventArgs) Handles mnuChooseFile.Click
        Dim chooseFile As New OpenFileDialog()
        chooseFile.ShowDialog()
        If chooseFile.FileName <> "" Then
            PctSourceImage.Image = Nothing
            PctSourceImage.ImageLocation = chooseFile.FileName
            SourcePath = chooseFile.FileName
        End If
    End Sub


    Private Shared Function ConvertToGrayScaleImage(ByVal originalBitmap As Bitmap) As Bitmap
        ' A blank bitmap is created having same size as original bitmap image.
        Dim GrayScaleBitmap As New Bitmap(originalBitmap.Width, originalBitmap.Height)

        ' Initializing a graphics object from the new image bitmap.
        Dim graphics__1 As Graphics = Graphics.FromImage(GrayScaleBitmap)

        ' Creating the Grayscale ColorMatrix whose values are determined by
        ' calculating the luminosity of a color, which is a weighted average of the
        ' RGB color components. The average is weighted according to the sensitivity
        ' of the human eye to each color component. The weights used here are as
        ' given by the NTSC (North America Television Standards Committee)
        ' and are widely accepted.
        Dim colorMatrix As New ColorMatrix(New Single()() {New Single() {0.299F, 0.299F, 0.299F, 0, 0}, New Single() {0.587F, 0.587F, 0.587F, 0, 0}, New Single() {0.114F, 0.114F, 0.114F, 0, 0}, New Single() {0, 0, 0, 1, 0}, New Single() {0, 0, 0, 0, 1}})

        ' Creating image attributes.
        Dim attributes As New ImageAttributes()

        ' Setting the color matrix attribute.
        attributes.SetColorMatrix(colorMatrix)

        ' Drawing the original bitmap image on the new bitmap image using the
        ' Grayscale color matrix.
        graphics__1.DrawImage(originalBitmap, New Rectangle(0, 0, originalBitmap.Width, originalBitmap.Height), 0, 0, originalBitmap.Width, originalBitmap.Height, _
         GraphicsUnit.Pixel, attributes)

        ' Disposing the Graphics object.
        graphics__1.Dispose()
        Return GrayScaleBitmap
    End Function


    Private Sub mnuConvert_Click(ByVal sender As Object, ByVal e As EventArgs) Handles mnuConvert.Click
        If PctSourceImage.Image Is Nothing Then
            MessageBox.Show("Please choose a source image file.", "Gray Scale Converter", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        Else
            Try
                OutputBitmap = ConvertToGrayScaleImage(New Bitmap(PctSourceImage.Image))
                PctOutputImage.Image = ConvertToGrayScaleImage(New Bitmap(PctSourceImage.Image))
                MessageBox.Show("The color image has been successfully converted " + "to gray scale.", "Gray Scale Converter", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch fileNotFoundException As IO.FileNotFoundException
                MessageBox.Show("Error encountered : " + fileNotFoundException.Message, "Gray Scale Converter", MessageBoxButtons.OK, MessageBoxIcon.[Error])
            Catch externalException As Runtime.InteropServices.ExternalException
                MessageBox.Show("Error encountered : " + externalException.Message, "Gray Scale Converter", MessageBoxButtons.OK, MessageBoxIcon.[Error])
            Catch indexOutOfRangeException As IndexOutOfRangeException
                MessageBox.Show("Error encountered : " + indexOutOfRangeException.Message, "Gray Scale Converter", MessageBoxButtons.OK, MessageBoxIcon.[Error])
            Catch exception As Exception
                MessageBox.Show("Error encountered : " + exception.Message, "Gray Scale Converter", MessageBoxButtons.OK, MessageBoxIcon.[Error])
            End Try
        End If
    End Sub

    Private Sub mnuSaveOutput_Click(ByVal sender As Object, ByVal e As EventArgs) Handles mnuSaveOutput.Click
        Dim sourceImageExtension As String = Nothing

        If PctOutputImage.Image Is Nothing Then
            MessageBox.Show("Cannot find gray scale image in the output area.", "Gray Scale Converter", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        Else
            sourceImageExtension = SourcePath.Split("."c)(1)
            Dim saveImageFile As New SaveFileDialog()
            saveImageFile.Filter = Convert.ToString((Convert.ToString(sourceImageExtension.ToUpper() + " Image (*.") & sourceImageExtension) + ")|*.") & sourceImageExtension
            saveImageFile.ShowDialog()

            If saveImageFile.FileName <> "" Then
                OutputBitmap.Save(saveImageFile.FileName)
                If OutputBitmap IsNot Nothing Then
                    OutputBitmap.Dispose()
                End If
                Process.Start(saveImageFile.FileName)
            End If
        End If
    End Sub

    Private Sub mnuReset_Click(ByVal sender As Object, ByVal e As EventArgs) Handles mnuReset.Click
        PctOutputImage.Image = Nothing
        PctSourceImage.Image = Nothing
        MessageBox.Show("Image areas have been reset sucessfully.", "Gray Scale Converter", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub


End Class
