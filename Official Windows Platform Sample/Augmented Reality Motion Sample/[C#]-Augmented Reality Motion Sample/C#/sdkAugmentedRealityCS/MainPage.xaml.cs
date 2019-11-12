/* 
    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Phone.Controls;

using Microsoft.Devices.Sensors;
using Microsoft.Devices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Matrix = Microsoft.Xna.Framework.Matrix;

namespace sdkAugmentedRealityCS
{
    public partial class MainPage : PhoneApplicationPage
    {
        Motion motion;
        PhotoCamera cam;

        List<TextBlock> textBlocks;
        List<Vector3> points;
        System.Windows.Point pointOnScreen;

        Viewport viewport;
        Matrix projection;
        Matrix view;
        Matrix attitude;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Initialize the list of TextBlock and Vector3 objects.
            textBlocks = new List<TextBlock>();
            points = new List<Vector3>();
        }

        public void InitializeViewport()
        {
            // Initialize the viewport and matrixes for 3d projection.
            viewport = new Viewport(0, 0, (int)this.ActualWidth, (int)this.ActualHeight);
            float aspect = viewport.AspectRatio;
            projection = Matrix.CreatePerspectiveFieldOfView(1, aspect, 1, 12);
            view = Matrix.CreateLookAt(new Vector3(0, 0, 1), Vector3.Zero, Vector3.Up);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Initialize the camera and set the video brush source.
            cam = new Microsoft.Devices.PhotoCamera();
            viewfinderBrush.SetSource(cam);

            if (!Motion.IsSupported)
            {
                MessageBox.Show("the Motion API is not supported on this device.");
                return;
            }

            // If the Motion object is null, initialize it and add a CurrentValueChanged
            // event handler.
            if (motion == null)
            {
                motion = new Motion();
                motion.TimeBetweenUpdates = TimeSpan.FromMilliseconds(20);
                motion.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<MotionReading>>(motion_CurrentValueChanged);
            }

            // Try to start the Motion API.
            try
            {
                motion.Start();
            }
            catch (Exception)
            {
                MessageBox.Show("unable to start the Motion API.");
            }

            // Hook up the event handler for when the user taps the screen.
            this.MouseLeftButtonUp += new MouseButtonEventHandler(MainPage_MouseLeftButtonUp);

            AddDirectionPoints();

            base.OnNavigatedTo(e);
        }

        void MainPage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // If the Canvas containing the TextBox is visible, ignore
            // this event.
            if (TextBoxCanvas.Visibility == Visibility.Visible)
            {
                return;
            }

            // Save the location where the user touched the screen.
            pointOnScreen = e.GetPosition(LayoutRoot);

            // Save the device attitude when the user touched the screen.
            attitude = motion.CurrentValue.Attitude.RotationMatrix;

            // Make the Canvas containing the TextBox visible and
            // give the TextBox focus.
            TextBoxCanvas.Visibility = Visibility.Visible;
            NameTextBox.Focus();
        }



        void motion_CurrentValueChanged(object sender, SensorReadingEventArgs<MotionReading> e)
        {
            // This event arrives on a background thread. Use BeginInvoke
            // to call a method on the UI thread.
            Dispatcher.BeginInvoke(() => CurrentValueChanged(e.SensorReading));
        }



        private void CurrentValueChanged(MotionReading reading)
        {
            if (viewport.Width == 0)
            {
                InitializeViewport();
            }


            // Get the RotationMatrix from the MotionReading.
            // Rotate it 90 degrees around the X axis to put it in xna coordinate system.
            Matrix attitude = Matrix.CreateRotationX(MathHelper.PiOver2) * reading.Attitude.RotationMatrix;

            // Loop through the points in the list
            for (int i = 0; i < points.Count; i++)
            {
                // Create a World matrix for the point.
                Matrix world = Matrix.CreateWorld(points[i], new Vector3(0, 0, 1), new Vector3(0, 1, 0));

                // Use Viewport.Project to project the point from 3D space into screen coordinates.
                Vector3 projected = viewport.Project(Vector3.Zero, projection, view, world * attitude);


                if (projected.Z > 1 || projected.Z < 0)
                {
                    // If the point is outside of this range, it is behind the camera.
                    // So hide the TextBlock for this point.
                    textBlocks[i].Visibility = Visibility.Collapsed;
                }
                else
                {
                    // Otherwise, show the TextBlock
                    textBlocks[i].Visibility = Visibility.Visible;

                    // Create a TranslateTransform to position the TextBlock.
                    // Offset by half of the TextBlock's RenderSize to center it on the point.
                    TranslateTransform tt = new TranslateTransform();
                    tt.X = projected.X - (textBlocks[i].RenderSize.Width / 2);
                    tt.Y = projected.Y - (textBlocks[i].RenderSize.Height / 2);
                    textBlocks[i].RenderTransform = tt;
                }
            }
        }


        private void AddPoint(Vector3 point, string name)
        {
            // Create a new TextBlock. Set the Canvas.ZIndexProperty to make sure
            // it appears above the camera rectangle.
            TextBlock textblock = new TextBlock();
            textblock.Text = name;
            textblock.FontSize = 124;
            textblock.SetValue(Canvas.ZIndexProperty, 2);
            textblock.Visibility = Visibility.Collapsed;

            // Add the TextBlock to the LayoutRoot container.
            LayoutRoot.Children.Add(textblock);

            // Add the TextBlock and the point to the List collections.
            textBlocks.Add(textblock);
            points.Add(point);


        }

        private void AddDirectionPoints()
        {
            AddPoint(new Vector3(0, 0, -10), "front");
            AddPoint(new Vector3(0, 0, 10), "back");
            AddPoint(new Vector3(10, 0, 0), "right");
            AddPoint(new Vector3(-10, 0, 0), "left");
            AddPoint(new Vector3(0, 10, 0), "top");
            AddPoint(new Vector3(0, -10, 0), "bottom");
        }
        /*
        private void NameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // When the TextBox loses focus. Hide the Canvas containing it.
            TextBoxCanvas.Visibility = Visibility.Collapsed;
        }
         */
        private void NameTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            // If the key is not the Enter key, don't do anything.
            if (e.Key != Key.Enter)
            {
                return;
            }

            // When the TextBox loses focus. Hide the Canvas containing it.
            TextBoxCanvas.Visibility = Visibility.Collapsed;

            // If any of the objects we need are not present, exit the event handler.
            if (NameTextBox.Text == "" || pointOnScreen == null || motion == null)
            {
                return;
            }

            // Translate the point before projecting it.
            System.Windows.Point p = pointOnScreen;
            p.X = LayoutRoot.RenderSize.Width - p.X;
            p.Y = LayoutRoot.RenderSize.Height - p.Y;
            p.X *= .5;
            p.Y *= .5;


            // Use the attitude Matrix saved in the OnMouseLeftButtonUp handler.
            // Rotate it 90 degrees around the X axis to put it in xna coordinate system.
            attitude = Matrix.CreateRotationX(MathHelper.PiOver2) * attitude;

            // Use Viewport.Unproject to translate the point on the screen to 3D space.
            Vector3 unprojected = viewport.Unproject(new Vector3((float)p.X, (float)p.Y, -.9f), projection, view, attitude);
            unprojected.Normalize();
            unprojected *= -10;

            // Call the helper method to add this point
            AddPoint(unprojected, NameTextBox.Text);

            // Clear the TextBox
            NameTextBox.Text = "";
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Dispose camera to minimize power consumption and to expedite shutdown.
            cam.Dispose();
        }
    }
}
