'
'   Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
'   Use of this sample source code is subject to the terms of the Microsoft license 
'   agreement under which you licensed this sample source code and is provided AS-IS.
'   If you did not accept the terms of the license agreement, you are not authorized 
'   to use this sample source code.  For the terms of the license, please see the 
'   license agreement between you and Microsoft.
'  
'   To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
'  
'
Imports Microsoft.Devices.Sensors
Imports Microsoft.Devices
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Imports Matrix = Microsoft.Xna.Framework.Matrix


Partial Public Class MainPage
    Inherits PhoneApplicationPage
    Private motion As Motion
    Private cam As PhotoCamera

    Private textBlocks As List(Of TextBlock)
    Private points As List(Of Vector3)
    Private pointOnScreen As System.Windows.Point

    Private viewport As Viewport
    Private _projection As Matrix
    Private view As Matrix
    Private attitude As Matrix

    ' Constructor
    Public Sub New()
        InitializeComponent()

        ' Initialize the list of TextBlock and Vector3 objects.
        textBlocks = New List(Of TextBlock)()
        points = New List(Of Vector3)()
    End Sub

    Public Sub InitializeViewport()
        ' Initialize the viewport and matrixes for 3d projection.
        viewport = New Viewport(0, 0, CInt(Fix(Me.ActualWidth)), CInt(Fix(Me.ActualHeight)))
        Dim aspect As Single = viewport.AspectRatio
        _projection = Matrix.CreatePerspectiveFieldOfView(1, aspect, 1, 12)
        view = Matrix.CreateLookAt(New Vector3(0, 0, 1), Vector3.Zero, Vector3.Up)
    End Sub

    Protected Overrides Sub OnNavigatedTo(ByVal e As System.Windows.Navigation.NavigationEventArgs)
        ' Initialize the camera and set the video brush source.
        cam = New Microsoft.Devices.PhotoCamera()
        viewfinderBrush.SetSource(cam)

        If Not motion.IsSupported Then
            MessageBox.Show("the Motion API is not supported on this device.")
            Return
        End If

        ' If the Motion object is null, initialize it and add a CurrentValueChanged
        ' event handler.
        If motion Is Nothing Then
            motion = New Motion()
            motion.TimeBetweenUpdates = TimeSpan.FromMilliseconds(20)
            AddHandler motion.CurrentValueChanged, AddressOf motion_CurrentValueChanged
        End If

        ' Try to start the Motion API.
        Try
            motion.Start()
        Catch e1 As Exception
            MessageBox.Show("unable to start the Motion API.")
        End Try

        ' Hook up the event handler for when the user taps the screen.
        AddHandler MouseLeftButtonUp, AddressOf MainPage_MouseLeftButtonUp

        AddDirectionPoints()

        MyBase.OnNavigatedTo(e)
    End Sub

    Private Sub MainPage_MouseLeftButtonUp(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
        ' If the Canvas containing the TextBox is visible, ignore
        ' this event.
        If TextBoxCanvas.Visibility = Visibility.Visible Then
            Return
        End If

        ' Save the location where the user touched the screen.
        pointOnScreen = e.GetPosition(LayoutRoot)

        ' Save the device attitude when the user touched the screen.
        attitude = motion.CurrentValue.Attitude.RotationMatrix

        ' Make the Canvas containing the TextBox visible and
        ' give the TextBox focus.
        TextBoxCanvas.Visibility = Visibility.Visible
        NameTextBox.Focus()
    End Sub



    Private Sub motion_CurrentValueChanged(ByVal sender As Object, ByVal e As SensorReadingEventArgs(Of MotionReading))
        ' This event arrives on a background thread. Use BeginInvoke
        ' to call a method on the UI thread.
        Dispatcher.BeginInvoke(Sub() CurrentValueChanged(e.SensorReading))
    End Sub



    Private Sub CurrentValueChanged(ByVal reading As MotionReading)
        If viewport.Width = 0 Then
            InitializeViewport()
        End If


        ' Get the RotationMatrix from the MotionReading.
        ' Rotate it 90 degrees around the X axis to put it in xna coordinate system.
        Dim attitude As Matrix = Matrix.CreateRotationX(MathHelper.PiOver2) * reading.Attitude.RotationMatrix

        ' Loop through the points in the list
        For i = 0 To points.Count - 1
            ' Create a World matrix for the point.
            Dim world As Matrix = Matrix.CreateWorld(points(i), New Vector3(0, 0, 1), New Vector3(0, 1, 0))

            ' Use Viewport.Project to project the point from 3D space into screen coordinates.
            Dim projected As Vector3 = viewport.Project(Vector3.Zero, _projection, view, world * attitude)


            If projected.Z > 1 OrElse projected.Z < 0 Then
                ' If the point is outside of this range, it is behind the camera.
                ' So hide the TextBlock for this point.
                textBlocks(i).Visibility = Visibility.Collapsed
            Else
                ' Otherwise, show the TextBlock
                textBlocks(i).Visibility = Visibility.Visible

                ' Create a TranslateTransform to position the TextBlock.
                ' Offset by half of the TextBlock's RenderSize to center it on the point.
                Dim tt As New TranslateTransform()
                tt.X = projected.X - (textBlocks(i).RenderSize.Width \ 2)
                tt.Y = projected.Y - (textBlocks(i).RenderSize.Height \ 2)
                textBlocks(i).RenderTransform = tt
            End If
        Next i
    End Sub


    Private Sub AddPoint(ByVal point As Vector3, ByVal name As String)
        ' Create a new TextBlock. Set the Canvas.ZIndexProperty to make sure
        ' it appears above the camera rectangle.
        Dim textblock As New TextBlock()
        textblock.Text = name
        textblock.FontSize = 124
        textblock.SetValue(Canvas.ZIndexProperty, 2)
        textblock.Visibility = Visibility.Collapsed

        ' Add the TextBlock to the LayoutRoot container.
        LayoutRoot.Children.Add(textblock)

        ' Add the TextBlock and the point to the List collections.
        textBlocks.Add(textblock)
        points.Add(point)


    End Sub

    Private Sub AddDirectionPoints()
        AddPoint(New Vector3(0, 0, -10), "front")
        AddPoint(New Vector3(0, 0, 10), "back")
        AddPoint(New Vector3(10, 0, 0), "right")
        AddPoint(New Vector3(-10, 0, 0), "left")
        AddPoint(New Vector3(0, 10, 0), "top")
        AddPoint(New Vector3(0, -10, 0), "bottom")
    End Sub
    '        
    '  Private Sub NameTextBox_LostFocus(ByVal sender As Object, ByVal e As RoutedEventArgs)
    '  When the TextBox loses focus. Hide the Canvas containing it.
    '  TextBoxCanvas.Visibility = Visibility.Collapsed
    '  End Sub
    '         
    Private Sub NameTextBox_KeyUp(ByVal sender As Object, ByVal e As KeyEventArgs)
        ' If the key is not the Enter key, don't do anything.
        If e.Key <> Key.Enter Then
            Return
        End If

        ' When the TextBox loses focus. Hide the Canvas containing it.
        TextBoxCanvas.Visibility = Visibility.Collapsed

        ' If any of the objects we need are not present, exit the event handler.
        If NameTextBox.Text = "" OrElse pointOnScreen = Nothing OrElse motion Is Nothing Then
            Return
        End If

        ' Translate the point before projecting it.
        Dim p As System.Windows.Point = pointOnScreen
        p.X = LayoutRoot.RenderSize.Width - p.X
        p.Y = LayoutRoot.RenderSize.Height - p.Y
        p.X *= 0.5
        p.Y *= 0.5


        ' Use the attitude Matrix saved in the OnMouseLeftButtonUp handler.
        ' Rotate it 90 degrees around the X axis to put it in xna coordinate system.
        attitude = Matrix.CreateRotationX(MathHelper.PiOver2) * attitude

        ' Use Viewport.Unproject to translate the point on the screen to 3D space.
        Dim unprojected As Vector3 = viewport.Unproject(New Vector3(CSng(p.X), CSng(p.Y), -0.9F), _projection, view, attitude)
        unprojected.Normalize()
        unprojected *= -10

        ' Call the helper method to add this point
        AddPoint(unprojected, NameTextBox.Text)

        ' Clear the TextBox
        NameTextBox.Text = ""
    End Sub

    Protected Overrides Sub OnNavigatedFrom(ByVal e As System.Windows.Navigation.NavigationEventArgs)
        ' Dispose camera to minimize power consumption and to expedite shutdown.
        cam.Dispose()
    End Sub
End Class
