' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.Drawing
Imports System.Drawing.Imaging
Public Class MainForm

#Region "Form Level Variables and Constants"

    ' The original image that is maintained in order to revert.
    Private originalImage As Image = Nothing
    ' The current image that it being edited and updated.
    Private currentImage As Image = Nothing
    ' The image that is stored to undo the most recent change.
    Private undoImage As Image = Nothing
    ' The value used to determine the current zoom factor.
    Private zoomFactor As Double = 1
    ' The width of the image on the screen in pixels.
    Private screenImageWidth As Integer = 0
    ' The height of the image on the screen in pixels.
    Private screenImageHeight As Integer = 0
    ' Current X value of the mouse over the image.
    Private X As Integer = 0
    ' Current Y value of the mouse over the image.
    Private Y As Integer = 0

    ' The percentage of the size of the current image to make the thumbnail.
    Const thumbnailFactor As Double = 0.15

#End Region

#Region "GUI Methods"

    ''' <summary>
    ''' Updates the X and Y coordinates when the mouse moves over MainImage
    ''' </summary>
    Private Sub MainImage_MouseMove(ByVal sender As Object, _
        ByVal e As System.Windows.Forms.MouseEventArgs) Handles MainImage.MouseMove

        ' Determine bounds.
        Dim left As Integer = CInt((MainImage.Width - screenImageWidth) / 2)
        Dim top As Integer = CInt((MainImage.Height - screenImageHeight) / 2)
        Dim right As Integer = left + screenImageWidth
        Dim bottom As Integer = top + screenImageHeight

        ' Determine location of the mouse relative to the image.
        If e.X < left Then
            X = 0
            XLabel.Text = "X: 0"
        ElseIf e.X > right Then
            X = CInt(screenImageWidth / zoomFactor)
            XLabel.Text = "X: " & X.ToString()
        Else
            X = CInt((e.X - left) / zoomFactor)
            XLabel.Text = "X: " + X.ToString()
        End If

        If e.Y < top Then
            Y = 0
            YLabel.Text = "Y: 0"
        ElseIf e.Y > bottom Then
            Y = CInt(screenImageHeight / zoomFactor)
            YLabel.Text = "Y: " & Y.ToString()
        Else
            Y = CInt((e.Y - top) / zoomFactor)
            YLabel.Text = "Y: " + Y.ToString()
        End If

    End Sub

    ''' <summary>
    ''' Exit the application
    ''' </summary> 
    Private Sub ExitApplication_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Application.Exit()
    End Sub

#End Region

#Region "Zoom Methods"

    ''' <summary>
    ''' Zooms to 25%
    ''' </summary>
    Private Sub Zoom25_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Zoom25.Click
        Zoom(0.25)
    End Sub

    ''' <summary>
    ''' Zooms to 50%
    ''' </summary>
    Private Sub Zoom50_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Zoom50.Click
        Zoom(0.5)
    End Sub

    ''' <summary>
    ''' Zooms to 100%
    ''' </summary>
    Private Sub Zoom100_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Zoom100.Click
        Zoom(1)
    End Sub

    ''' <summary>
    ''' Zooms to 150%
    ''' </summary>
    Private Sub Zoom150_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Zoom150.Click
        Zoom(1.5)
    End Sub

    ''' <summary>
    ''' Zooms to 200%
    ''' </summary>
    Private Sub Zoom200_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Zoom200.Click
        Zoom(2)
    End Sub

    ''' <summary>
    ''' Updates the zoom menu to have the appropriate option selected
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UpdateZoomMenu()

        Zoom25.Checked = False
        Zoom50.Checked = False
        Zoom100.Checked = False
        Zoom150.Checked = False
        Zoom200.Checked = False

        Select Case zoomFactor
            Case 0.25
                Zoom25.Checked = True
                Exit Select
            Case 0.5
                Zoom50.Checked = True
                Exit Select
            Case 1
                Zoom100.Checked = True
                Exit Select
            Case 1.5
                Zoom150.Checked = True
                Exit Select
            Case 2
                Zoom200.Checked = True
                Exit Select
        End Select

    End Sub

    ''' <summary>
    ''' Zooms the image to the designated factor of the current image.
    ''' </summary>
    Private Sub Zoom(ByVal factor As Double)

        ' Save the factor in global variable.
        zoomFactor = factor

        ' Get the resized image.
        Dim sourceBitmap As New Bitmap(currentImage)
        Dim destBitmap As New Bitmap(CInt(sourceBitmap.Width * factor), _
            CInt(sourceBitmap.Height * factor))
        Dim destGraphic As Graphics = Graphics.FromImage(destBitmap)

        destGraphic.DrawImage(sourceBitmap, 0, 0, destBitmap.Width + 1, _
            destBitmap.Height + 1)

        ' Save the size of the image on the screen in globals.
        screenImageWidth = destBitmap.Width
        screenImageHeight = destBitmap.Height

        MainImage.Image = destBitmap

        ' Update the zoom label on the form.
        ZoomLabel.Text = "Zoom: " & zoomFactor * 100 & "%"

        ' Update the zoom menu selection.
        UpdateZoomMenu()

    End Sub

#End Region

#Region "Image Drawing, Converting, and Updating Methods"

    ''' <summary>
    ''' Draws an image using the designated ColorMatrix
    ''' </summary>
    Private Function DrawAdjustedImage(ByVal cMatrix As ColorMatrix) As Boolean

        ' Update undo image and menu.
        undoImage = CType(currentImage.Clone(), Image)
        Undo.Enabled = True
        Try
            Dim bmp As New Bitmap(currentImage)
            Dim rc As New Rectangle(0, 0, _
                currentImage.Width, currentImage.Height)
            Dim graphicsObject As Graphics = _
                Graphics.FromImage(currentImage)

            ' associate the ColorMatrix object with an ImageAttributes object
            Dim imgattr As New ImageAttributes()
            imgattr.SetColorMatrix(cMatrix)

            'apply the ColorMatrix
            graphicsObject.DrawImage(bmp, rc, 0, 0, currentImage.Width, _
                currentImage.Height, GraphicsUnit.Pixel, imgattr)

            graphicsObject.Dispose()

            ' Apply the current zoom to the image.
            Zoom(zoomFactor)

            Return True
        Catch
            Return False
        End Try

    End Function

    ''' <summary>
    ''' Resizes an image to a percentage of the current size.
    ''' </summary>
    Private Sub ResizeImage(ByVal percent As Double)

        ' Update undo image and menu.
        undoImage = CType(currentImage.Clone(), Image)
        Undo.Enabled = True

        ' Resize the image.
        Dim sourceBitmap As New Bitmap(currentImage)
        Dim destBitmap As New Bitmap(CInt(sourceBitmap.Width * percent / 100), CInt(sourceBitmap.Height * percent / 100))
        Dim destGraphic As Graphics = Graphics.FromImage(destBitmap)
        destGraphic.DrawImage(sourceBitmap, 0, 0, destBitmap.Width + 1, destBitmap.Height + 1)
        currentImage = destBitmap

        ' Zoom to the current zoom settings.
        Zoom(zoomFactor)

        ' Update the width and height labels on the form.
        UpdateWidthandHeight()

    End Sub

    ''' <summary>
    ''' Negates the current image
    ''' </summary>
    Private Sub DrawNegativeImage()

        ' Create the color matrix for a negative image.
        Dim cMatrix As ColorMatrix = New ColorMatrix(New Single()() _
                           {New Single() {-1, 0, 0, 0, 0}, _
                            New Single() {0, -1, 0, 0, 0}, _
                            New Single() {0, 0, -1, 0, 0}, _
                            New Single() {0, 0, 0, 1, 0}, _
                            New Single() {0, 0, 0, 0, 1}})

        ' Draw the image using the color matrix.
        DrawAdjustedImage(cMatrix)

    End Sub

    ''' <summary>
    ''' Converts the current image to greyscale
    ''' </summary>
    Public Function ConvertToGrayScale() As Boolean

        ' Create the color matrix for a grayscale image.
        Dim cMatrix As ColorMatrix = New ColorMatrix(New Single()() _
                               {New Single() {0.299, 0.299, 0.299, 0, 0}, _
                                New Single() {0.587, 0.587, 0.587, 0, 0}, _
                                New Single() {0.114, 0.114, 0.114, 0, 0}, _
                                New Single() {0, 0, 0, 1, 0}, _
                                New Single() {0, 0, 0, 0, 1}})

        ' Draw the image using the color matrix.
        DrawAdjustedImage(cMatrix)

    End Function

    ''' <summary>
    ''' Negates the current image
    ''' </summary>
    Private Sub Negative_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Negative.Click
        DrawNegativeImage()
    End Sub

    ''' <summary>
    ''' Converts the current image to greyscale
    ''' </summary>
    Private Sub Grayscale_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Grayscale.Click
        ConvertToGrayScale()
    End Sub

    ''' <summary>
    ''' Resizes the image
    ''' </summary>
    Private Sub ResizeButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ResizeButton.Click

        Dim ValidationMsg As String = _
            txtPercent.Tag.ToString & " must be a positive integer"

        ' Validate the input in the textbox.
        With txtPercent

            ValidationMsg &= "."

            If .Text.Trim = "" Then
                RejectTextBox(ValidationMsg, txtPercent)
                Exit Sub
            End If

            If Not IsNumeric(.Text.Trim) Then
                RejectTextBox(ValidationMsg, txtPercent)
                Exit Sub
            Else
                Dim integerValue As Integer
                Try
                    integerValue = CInt(.Text.Trim)
                    If integerValue <= 0 Then
                        RejectTextBox(ValidationMsg, txtPercent)
                        Exit Sub
                    End If
                Catch Exp As Exception
                    RejectTextBox(ValidationMsg, txtPercent)
                    Exit Sub
                End Try
            End If
        End With

        ' Call the ResizeImage Method.
        ResizeImage(CDbl(txtPercent.Text))

    End Sub

    ''' <summary>
    ''' Converts crop values into values the fit on the current image.
    ''' </summary>
    Private Sub TranslateCropValues()

        If CInt(LeftInput.Text) > currentImage.Width Then
            LeftInput.Text = CStr(currentImage.Width)
        End If

        If CInt(TopInput.Text) > currentImage.Height Then
            TopInput.Text = CStr(currentImage.Height)
        End If

        If CInt(LeftInput.Text) + CInt(WidthInput.Text) > currentImage.Width Then
            WidthInput.Text = CStr(currentImage.Width - CInt(LeftInput.Text))
        End If

        If CInt(TopInput.Text) + CInt(HeightInput.Text) > currentImage.Height Then
            HeightInput.Text = CStr(currentImage.Height - CInt(TopInput.Text))
        End If

    End Sub

    ''' <summary>
    ''' Crops the image according to crop values
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub CropButton_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles CropButton.Click

        If IsValidCroppingInput() Then
            undoImage = CType(currentImage.Clone(), Image)
            Undo.Enabled = True

            TranslateCropValues()

            Dim recSource As New Rectangle( _
                CInt(LeftInput.Text), CInt(TopInput.Text), _
                CInt(WidthInput.Text), CInt(HeightInput.Text))

            Dim bmpCropped As New Bitmap( _
                CInt(WidthInput.Text), CInt(HeightInput.Text))

            Dim grBitmap As Graphics = Graphics.FromImage(bmpCropped)

            grBitmap.DrawImage(currentImage, 0, 0, _
                recSource, GraphicsUnit.Pixel)

            currentImage = bmpCropped
            Zoom(zoomFactor)
        End If

    End Sub

    ''' <summary>
    ''' Determines if the cropping values are valid
    ''' </summary>
    Private Function IsValidCroppingInput() As Boolean

        ' Iterate through the textbox's
        For Each txt As TextBox In New TextBox() _
            {TopInput, LeftInput, WidthInput, HeightInput}
            Dim ValidationMsg As String = _
                txt.Tag.ToString & " must be a positive integer"
            With txt
                If (txt Is LeftInput) Or (txt Is TopInput) Then
                    ValidationMsg &= " or zero."
                Else
                    ValidationMsg &= "."
                End If

                ' If there is no input.
                If .Text.Trim = "" Then
                    RejectTextBox(ValidationMsg, txt)
                    Return False
                End If

                ' If there is non numeric input.
                If Not IsNumeric(.Text.Trim) Then
                    RejectTextBox(ValidationMsg, txt)
                    Return False
                Else
                    Dim integerValue As Integer
                    ' Determine if the input is positive.
                    Try
                        integerValue = CInt(.Text.Trim)
                        If (txt Is LeftInput) Or (txt Is TopInput) Then
                            If integerValue < 0 Then
                                RejectTextBox(ValidationMsg, txt)
                                Return False
                            End If
                        ElseIf integerValue <= 0 Then
                            RejectTextBox(ValidationMsg, txt)
                            Return False
                        End If
                    Catch Exp As Exception
                        RejectTextBox(ValidationMsg, txt)
                        Return False
                    End Try
                End If
            End With
        Next

        Return True

    End Function

    Private Sub RejectTextBox(ByVal message As String, ByVal txt As TextBox)
        MessageBox.Show(message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
        txt.SelectAll()
        txt.Focus()
    End Sub

    ''' <summary>
    ''' Draws a box to show where cropping would occur.
    ''' </summary>
    Private Sub TestButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TestButton.Click

        If IsValidCroppingInput() Then

            ' Redraw the image to remove previous crop box.
            MainImage.Refresh()

            Dim left As Integer = CInt((MainImage.Width - screenImageWidth) / 2)
            Dim top As Integer = CInt((MainImage.Height - screenImageHeight) / 2)

            TranslateCropValues()

            ' Draw a red rectangle to show where the image will be cropped.
            Dim recCropBox As New Rectangle(CInt(CDbl(LeftInput.Text) * zoomFactor + left - 1), _
                CInt(CDbl(TopInput.Text) * zoomFactor + top - 1), CInt(CDbl(WidthInput.Text) * zoomFactor), _
                    CInt(CDbl(HeightInput.Text) * zoomFactor))
            MainImage.CreateGraphics.DrawRectangle(Pens.Red, recCropBox)
        End If

    End Sub


    ''' <summary>
    ''' Updates the width and height labels on the form.
    ''' </summary>
    Private Sub UpdateWidthandHeight()

        WidthLabel.Text = "Width: " & currentImage.Width
        HeightLabel.Text = "Height: " & currentImage.Height

    End Sub

#End Region

#Region "Rotate and Flip Methods"

    ''' <summary>
    ''' Rotates or Flips the current image.
    ''' </summary>
    Private Sub RotateFlip(ByVal degrees As Integer)

        ' Update undo image and menu.
        undoImage = CType(currentImage.Clone(), Image)
        Undo.Enabled = True
        Select Case degrees
            Case 0
                currentImage.RotateFlip(RotateFlipType.RotateNoneFlipX)
            Case 1
                currentImage.RotateFlip(RotateFlipType.RotateNoneFlipY)
            Case 90
                currentImage.RotateFlip(RotateFlipType.Rotate90FlipNone)
                Exit Select
            Case 180
                currentImage.RotateFlip(RotateFlipType.Rotate180FlipNone)
                Exit Select
            Case 270
                currentImage.RotateFlip(RotateFlipType.Rotate270FlipNone)
            Case Else
                Exit Select
        End Select

        ' Display current image with zoom settings.
        Zoom(zoomFactor)

    End Sub

    ''' <summary>
    ''' Rotates the current image 90 degrees. 
    ''' </summary>
    Private Sub Rotate90_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Rotate90.Click
        RotateFlip(90)
    End Sub

    ''' <summary>
    ''' Rotates the image 180 degrees
    ''' </summary>
    Private Sub Rotate180_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Rotate180.Click
        RotateFlip(180)
    End Sub

    ''' <summary>
    ''' Rotates the image 270 degrees
    ''' </summary>
    Private Sub Rotate270_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Rotate270.Click
        RotateFlip(270)
    End Sub

    ''' <summary>
    ''' Flip the current image over the y-axis
    ''' </summary>
    Private Sub HorizontalFlip_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HorizontalFlip.Click
        RotateFlip(0)
        Zoom(zoomFactor)
    End Sub

    ''' <summary>
    ''' Flips the current image across the x axis.
    ''' </summary>
    Private Sub VerticalFlip_Click(ByVal sender As System.Object, _
     ByVal e As System.EventArgs) Handles VerticalFlip.Click
        RotateFlip(1)
        Zoom(zoomFactor)
    End Sub

#End Region

#Region "Revert and Undo Methods"

    ''' <summary>
    ''' Restores the original image to its on-disk representation
    ''' </summary>
    Private Sub Revert_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Revert.Click
        undoImage = CType(currentImage.Clone(), Image)
        currentImage = CType(originalImage.Clone(), Image)
        Zoom(zoomFactor)
    End Sub

    ''' <summary>
    ''' Restores the image with the undo image from the last change.
    ''' </summary>
    Private Sub Undo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Undo.Click
        currentImage = CType(undoImage.Clone(), Image)
        Zoom(zoomFactor)

        ' Update the undo menu to disabled.
        Undo.Enabled = False
    End Sub

#End Region

#Region "Open and Save Methods"

    ''' <summary>
    ''' Opens an image from a file and updates the form.
    ''' </summary>
    Private Sub OpenImage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenImage.Click

        If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            ' Open Image
            originalImage = Image.FromFile(OpenFileDialog1.FileName)
            currentImage = CType(originalImage.Clone(), Image)

            ' Determine appropriate zoom for initial view of image.
            If currentImage.Width / 2 > MainImage.Width Or _
             currentImage.Height / 2 > MainImage.Height Then
                Zoom(0.25)
            ElseIf currentImage.Width > MainImage.Width Or _
             currentImage.Height > MainImage.Height Then
                Zoom(0.5)
            ElseIf currentImage.Width * 2 < MainImage.Width _
             And currentImage.Height * 2 < MainImage.Height Then
                Zoom(2)
            ElseIf currentImage.Width * 2 > MainImage.Width _
             And currentImage.Height * 2 > MainImage.Height Then
                Zoom(2)
            ElseIf currentImage.Width * 1.5 < MainImage.Width _
             And currentImage.Height * 2 < MainImage.Height Then
                Zoom(1.5)
            Else
                Zoom(1)
            End If

            ' Update the form for a new image.
            UpdateWidthandHeight()
            Undo.Enabled = False
            ImageToolStripMenuItem.Visible = True
            EditMenu.Visible = True
            SaveThumbnailAs.Enabled = True
            SaveImageAs.Enabled = True
            Resizing.Enabled = True
            Cropping.Enabled = True
            ImageInfo.Enabled = True
            MainImage.Enabled = True

        End If

    End Sub

    ''' <summary>
    ''' Saves the image to the file chosen.
    ''' </summary>
    Private Sub SaveImageAs_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveImageAs.Click
        If SaveFileDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
            currentImage.Save(SaveFileDialog1.FileName, GetImageFormat())
        End If
    End Sub

    ''' <summary>
    ''' Saves the current image as a thumbnail
    ''' </summary>
    Private Sub SaveThumbnailAs_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveThumbnailAs.Click

        Dim sourceBitmap As New Bitmap(currentImage)
        Dim destBitmap As New Bitmap( _
         CInt(sourceBitmap.Width * thumbnailFactor), _
          CInt(sourceBitmap.Height * thumbnailFactor))

        Dim destGraphic As Graphics = Graphics.FromImage(destBitmap)

        destGraphic.DrawImage(sourceBitmap, 0, 0, _
         destBitmap.Width + 1, destBitmap.Height + 1)

        If SaveFileDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
            destBitmap.Save(SaveFileDialog1.FileName, GetImageFormat())
        End If

    End Sub

    ''' <summary>
    ''' Get the image format from the save dilaog box.
    ''' </summary>
    Private Function GetImageFormat() As ImageFormat
        Select Case SaveFileDialog1.FilterIndex
            Case 1
                Return ImageFormat.Bmp
            Case 2
                Return ImageFormat.Jpeg
            Case 3
                Return ImageFormat.Gif
            Case Else
                Return ImageFormat.Tiff
        End Select
    End Function

    Private Sub ExitApplication_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitApplication.Click
        Me.Close()
    End Sub

#End Region

End Class
