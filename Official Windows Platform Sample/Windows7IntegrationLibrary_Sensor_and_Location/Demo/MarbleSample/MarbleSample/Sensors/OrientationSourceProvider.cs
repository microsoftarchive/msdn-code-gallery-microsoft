// Copyright (c) Microsoft Corporation.  All rights reserved.

//---------------------------------------------------------------------------
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Windows7.Sensors;
using Windows7.Sensors.Sensors.Motion;

namespace Microsoft.Sensors.Samples
{
    /// <summary>
    /// OrientationSourceProvider provides the implementation to convert
    /// the Sensor COM Interop library into a Windows Presentation Framework 
    /// Animatable dependency object.
    /// </summary>
    /// <remarks>
    /// The orientation sourceprovider uses the Windows 7 Sensors COM interface to retrieve
    /// an acceleromter sensor and provides its vector and a calcuated orientation for Xna applications.
    /// 
    /// Initialize() is called in the constructor to enumerate any existing Accelerometer3d sensors
    /// attached to the system. The first active sensor found will be used to provide data.
    /// When a new sensor is found (via a callback from the sensor managed object), a callback is attached to the sensor 
    /// object to notify the provider when the sensor value has changed or the sensor has been removed.
    /// 
    /// When an individual sensor reports a new data event, the vector values are retrieved. This
    /// value is used to calculate the orientation of the screen by calculating the angle 
    /// around the z-axis, which is the vector pointing into the screen.
    /// </remarks>
    public class OrientationSourceProvider
    {

        /// <summary>
        /// A reference to the sensor object.
        /// </summary>
        private Accelerometer3D _sensor;

        /// <summary>
        /// The orientation around the Z axis, which is the vector pointing into the screen.
        /// </summary>
        public Orientation Orientation
        {
            get;
            private set;
        }

        /// <summary>
        /// The vector coming from the accelerometer which represents the acceleration acting on it.
        /// A null value means that the accelerometer is disconnected.
        /// </summary>
        public Vector3? Vector
        {
            get;
            private set;
        }

        /// <summary>
        /// The orientation (in degrees) around the Z axis, which is the vector pointing into the screen.
        /// </summary>
        public double Angle
        {
            get;
            private set;
        }

        /// <summary>
        /// The type of the sensor used by this provider is Accelerometer3d
        /// </summary>
        public static Guid SensorType
        {
            get { return SensorTypes.Accelerometer3d; }
        }

        /// <summary>
        /// The desired report interval for the accelerometer is 100ms
        /// </summary>
        private const uint DesiredReportInterval = 100;

        /// <summary>
        /// Raised when the screen orientation changed.
        /// </summary>
        public event EventHandler OrientationChanged;

        /// <summary>
        /// Raised when the vector changed.
        /// </summary>
        public event EventHandler VectorChanged;

        /// <summary>
        /// Constructs a new OrientationSourceProvider.
        /// </summary>
        public OrientationSourceProvider()
        {
            Initialize();
        }

        #region Initialize Functionality
        public void Initialize()
        {
            // get a collection of Acceleromter3d sensors from the sensor manager.
            Sensor[] sensorCollection = null;
            try
            {
                //Manager.GetSensorsByType(SensorType, out sensorCollection);
                sensorCollection = SensorManager.GetSensorsByType<Accelerometer3D>();
            }
            catch (COMException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            if (sensorCollection.Length > 0)
            {
                AddSensorCollection(sensorCollection);
            }

            SensorManager.SensorEnter += new SensorEnterEventHandler(OnSensorEnter);
        }

        private void AddSensorCollection(Sensor[] sensorCollection)
        {
            if (sensorCollection.Length > 0)
            {
                Sensor sensor = sensorCollection[0];
                if (sensor != null)
                {
                    // add the sensor to the current collection
                    OnSensorEnter(sensor, SensorState.Initializing);
                }
            }
        }
        #endregion
                
        #region ISensorEvents Members
        /// <summary>
        /// Called when accelerometer sensor's state is changed.
        /// </summary>
        public void OnStateChanged(Sensor sensor, SensorState state)
        {
            // for this class, there is no response to an OnStateChanged event. The code
            // below is provided for clarity in implementing your own sensor state change response
            switch (state)
            {
                case SensorState.Ready:
                    // the sensor has come online and is ready to read and write data
                    break;
                case SensorState.AccessDenied:
                    // the sensor access has been revoked. The application should remove
                    // sensor values since the sensor is no longer available.
                    break;
                case SensorState.Error:
                    // a sensor error has occurred 
                    break;
                case SensorState.Initializing:
                    // the sensor is initializing, data is not available.
                    break;
                case SensorState.NotAvailable:
                    // the sensor is not available.
                    break;

                default:
                    break;
            }

        }

        /// <summary>
        /// Called when an accelerometer sensor's data is updated.
        /// </summary>
        public void OnDataUpdated(Sensor sensor, SensorDataReport newData)
        {
            if (sensor == _sensor)
            {
                Accelerometer3DReport a3dReport = (Accelerometer3DReport) newData;
                Vector3 vector = Vector3.Zero;
                vector.X = a3dReport.AxisX_G;
                vector.Y = a3dReport.AxisY_G;
                vector.Z = a3dReport.AxisZ_G;

                if (vector != Vector3.Zero)
                {
                    this.Vector = vector;
                    if (VectorChanged != null)
                    {
                        VectorChanged(this, EventArgs.Empty);
                    }

                    CalculateOrientation(vector);
                }
                else
                {
                    // The accelerometer is providing no value so 
                    // set Vector to null and reset the orientation
                    this.Vector = null;
                    Orientation? oldOrientation = Orientation;
                    Orientation newOrientation = Orientation.Angle0;
                    Orientation = newOrientation;
                    if (OrientationChanged != null)
                    {
                        OrientationChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// Called when an accelerometer sensor fires an event.
        /// </summary>
        public void OnEvent(Sensor sensor, Guid eventID, SensorDataReport newData) { }


        /// <summary>
        /// Called when an accelerometer sensor is removed.
        /// </summary>
        /// <param name="sensorID"></param>
        public void OnLeave(Sensor sensor, Guid sensorID)
        {
            if (sensor.SensorID == _sensor.SensorID && sensor.SensorID != Guid.Empty)
            {
                _sensor = null;

                // attempt to find another sensor, if possible
                Initialize();
            }
        }

        #endregion

        #region ISensorManagerEvents Members

        /// <summary>
        /// Called when any new sensor is attached to the system.
        /// </summary>
        public void OnSensorEnter(Sensor pSensor, SensorState state)
        {
            // The sensor paramater should not be null
            System.Diagnostics.Debug.Assert(pSensor != null);
            // If we are currently using a sensor we don't want to stop using it and
            // use a new one so we only use a new sensor if we don't have one already.
            if (_sensor == null)
            {
                Guid sensorType = Guid.Empty;
                try
                {
                    sensorType = pSensor.TypeID;
                }
                catch (COMException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
                if (sensorType == SensorType)
                {
                    _sensor = (Accelerometer3D) pSensor;
                    try
                    {
                        pSensor.DataUpdated += new SensorDataUpdatedEventHandler(OnDataUpdated);
                        pSensor.EventReceived += new SensorEventHandler(OnEvent);
                        pSensor.SensorLeave += new SensorLeaveEventHandler(OnLeave);
                        pSensor.StateChanged += new SensorStateChangedEventHandler(OnStateChanged);
                    }
                    catch (COMException ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }
     
                    state = pSensor.State;

                    if (state == SensorState.Ready)
                    {
                        SetupSensorProperties(pSensor);

                        SensorDataReport data = null;
                        try
                        {
                            data = pSensor.GetDataReport();
                        }
                        catch (COMException ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.Message);
                        }
                        if (data != null)
                        {
                            OnDataUpdated(pSensor, data);
                        }
                    }
                    else if (state == SensorState.AccessDenied)
                    {
                        if (RequestSensorAccess(pSensor))
                        {
                            SetupSensorProperties(pSensor);
                        }
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Set the report interval of the sensor to desired values.
        /// </summary>
        /// <param name="sensor">The sensor on which to set the values.</param>
        private void SetupSensorProperties(Sensor sensor)
        {
            Accelerometer3D a3dSensor = (Accelerometer3D)sensor;
            a3dSensor.ReportInterval = DesiredReportInterval;
            if (a3dSensor.ReportInterval != DesiredReportInterval)
            {
                Debug.WriteLine("Error setting report interval. The desired value of " + DesiredReportInterval +
                " could not be set. The current report interval is: " + a3dSensor.ReportInterval);         
            }
        }

        /// <summary>
        /// Surface a sensor dialog to request access to the sensor device. A user may
        /// restrict access to specific sensors due to security and privacy concerns they may have,
        /// depending on what data the sensor provides.
        /// </summary>
        /// <param name="sensor">The sensor to which to request access.</param>
        /// <returns>Whether the sensor is now ready.</returns>
        private bool RequestSensorAccess(Sensor sensor)
        {
            SensorState state = SensorState.AccessDenied;
            if (sensor != null)
            {
                    // if available, the application should get the handle to the 
                    // main window for this application.
                    IntPtr windowHandle = IntPtr.Zero;

                    try
                    {
                        // ask the sensor platform to request access from the user from a modal window
                        // if we are able to get the main window for the current application.
                        SensorManager.RequestPermission(windowHandle, true, sensor);
                    }
                    catch (COMException ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }

                    state = sensor.State;
            }
            
            return state == SensorState.Ready;
        }

        #region Orientation Conversion

        /// <summary>
        /// Convert the accelerometer vector to an angle representing
        /// orientation around the Z axis, which is the vector pointing
        /// into the screen.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        private void CalculateOrientation(Vector3 vector)
        {
            Vector2 vec2 = new Vector2(vector.X, -vector.Y);

            double? angleConversion = Vector2ToAngleConverter.Vector2DToAngle(vec2);

            // Convert the angle around the Z axis to an Orientation for the UI
            if (angleConversion.HasValue)
            {
                Angle = angleConversion.Value;
                Orientation? oldOrientation = Orientation;
                Orientation newOrientation = AngleToDirection(Angle);
                if (!oldOrientation.HasValue || newOrientation != Orientation)
                {
                    Orientation = newOrientation;
                    if (OrientationChanged != null)
                    {
                        OrientationChanged(this, new EventArgs());
                    }
                }
            }
        }

        /// <summary>
        /// Convert an angle in degrees to the appropriate orientation enumeration.
        /// </summary>
        /// <param name="angle">The angle, in degrees</param>
        /// <returns>The orientation</returns>
        private static Orientation AngleToDirection(double angle)
        {
            // Normalize the input value to be between 0 and 360
            angle = angle % 360;
            if (angle < 0)
            {
                angle += 360;
            }

            Orientation orientation = Orientation.Angle0;
            if (angle > 315 || angle <= 45)
            {
                orientation = Orientation.Angle0;
            }
            else if (angle > 45 && angle <= 135)
            {
                orientation = Orientation.Angle90;
            }
            else if (angle > 135 && angle <= 225)
            {
                orientation = Orientation.Angle180;
            }
            else if (angle > 225 && angle <= 315)
            {
                orientation = Orientation.Angle270;
            }
            return orientation;
        }

        #endregion
    }
}