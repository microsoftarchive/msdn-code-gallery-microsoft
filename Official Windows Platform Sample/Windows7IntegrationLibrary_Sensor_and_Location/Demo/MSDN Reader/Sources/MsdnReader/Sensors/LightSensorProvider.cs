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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Windows7.Sensors.Sensors.Light;
using Windows7.Sensors;


namespace MsdnReader
{
    internal delegate void BeginAnimationDelegate(DependencyProperty dp, AnimationTimeline timeline);

    /// <summary>
    /// LightSensorProvider provides the implementation to convert
    /// the Sensor API managed wrapper into a Windows Presentation Framework 
    /// Animatable dependency object.
    /// </summary>
    /// <remarks>
    /// The ambient light sensor provider uses the Sensors wrapper to retrieve
    /// a collection of ambient light sensors and provides the composited value for WPF applications.
    /// 
    /// OnInitialize() is called to enumerate any existing sensors attached to the system. Any new 
    /// sensors are added to the Sensors property. When a new sensor is found (via an event from 
    /// the SensorManager object), event handlers are subscribed to the sensor objecs to be notified
    /// when the sensor value has changed or the sensor has been removed.
    ///
    /// When an individual sensor reports a new data event, the lux light property is retrieved. This
    /// value is normalized across all sensors to create a composite light property. Then the provider
    /// uses time and value thresholding to ensure that the frequency of ambient light value changes
    /// does not adversly affect the application.
    /// </remarks>
    public class LightSensorProvider : Animatable
    {
        #region Private Member Variables
        /// <summary>
        /// The timer to delay ambient light sensor updates.
        /// </summary>
        private DispatcherTimer _updateTimer;

        /// <summary>
        /// A dictionary matching sensor lux values to the sensor id they originated from.
        /// </summary>
        private Dictionary<Guid, double> _values;

        /// <summary>
        /// The observable collection of sensors that are available on the system.
        /// </summary>
        private ICollection<AmbientLightSensor> _sensors;

        /// <summary>
        /// The most recent composite sensor lux value used for thresholding sensor input.
        /// </summary>
        private double _lastValue;

        /// <summary>
        /// The most recent time the composite sensor value was updated.
        /// </summary>
        private DateTime _lastUpdateTime = DateTime.MinValue;

        #region Private Static Constants
        /// <summary>
        /// The timespan to animate the property value when a new value is received.
        /// </summary>
        private static readonly TimeSpan AnimationTime = TimeSpan.FromSeconds(0.5);

        /// <summary>
        /// The threshold value as a perceived lux 0 -> 1 value to trigger light changes
        /// </summary>
        private static readonly double DefaultChangeThreshold = 0.05;

        /// <summary>
        /// The default ambient light level of an indoor room, in lux.
        /// </summary>
        private const double DefaultNormalIndoorAmbientLightLevelInLux = 350.0;

        /// <summary>
        /// The amount of time this object will wait by default to update large changes
        /// in the detected ambient light value. Increase this value to make fewer updates,
        /// reduce this value in order to make more updates.
        /// </summary>
        private static readonly TimeSpan DefaultMinimumUpdateDelayTime = TimeSpan.FromSeconds(0.5);

        /// <summary>
        /// The amount of time this object will wait by default to update small changes
        /// in the detected ambient light value. If small changes do not significantly impact the 
        /// application this can safely be made larger.
        /// </summary>
        private static readonly TimeSpan DefaultMaximumUpdateDelayTime = TimeSpan.FromSeconds(4);

        /// <summary>
        /// The maximum reported value of the sensor, in lux.
        /// </summary>
        public const double MaximumSensorReportValue = 1000;
        #endregion
        #endregion

        #region Constructors
        /// <summary>
        /// Construct a new LightSensorProvider.
        /// </summary>
        public LightSensorProvider()
            : base()
        {
            // Construct a new mapping of sensor identifiers to the most recent sensor value.
            // This is used to construct the composite sensor value for all ambient light sensor
            // values available on the system (when there is more than once sensor attached).
            _values = new Dictionary<Guid, double>();

            MinimumUpdateDelayTime = DefaultMinimumUpdateDelayTime;
            MaximumUpdateDelayTime = DefaultMaximumUpdateDelayTime;

            // Initialize the update timer on the main thread's dispatcher. This enables us to update
            // the sensor value after a specific timeout has expired.
            _updateTimer = new DispatcherTimer(DispatcherPriority.Background);
            _updateTimer.Tick += new EventHandler(UpdateTimerCallbackHandler);

            // Initialize the sensor collection from the existing set of sensors on the sytsem.
            Initialize();
        }
        #endregion

        /// <summary>
        /// The minimum delay time to update the sensor value. This property is available to reduce
        /// a high processor load due to constantly updating values.
        /// </summary>
        public TimeSpan MinimumUpdateDelayTime
        {
            get;
            set;
        }

        /// <summary>
        /// The minimum delay time to update the sensor value. This property is available to reduce
        /// a high processor load due to constantly updating values.
        /// </summary>
        public TimeSpan MaximumUpdateDelayTime
        {
            get;
            set;
        }

        #region Initialize Functionality
        /// <summary>
        /// Attach to the sensor manager objects and initialize the sensor connection
        /// </summary>
        private void Initialize()
        {
            try
            {
                SensorManager.SensorEnter += OnSensorEnter;
                // get a collection of ambient light sensors from the sensor manager.
                AmbientLightSensor[] sensorCollection = SensorManager.GetSensorsByType<AmbientLightSensor>();
                AddSensorCollection(sensorCollection);
            }
            catch (COMException ex)
            {
                // failed to interop to the sensor API. The most likely reason is the sensor
                // API isn't supported on this operating system and or sku.
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Calculates the composite sensor value from the values available in the _values dictionary.
        /// </summary>
        private double CalculateCompositeSensorValue()
        {
            double result = 0.0;

            // If there are no sensor values in the array, then no sensors are attached to the system
            // or the data is not available. In this case, return the default metadata for the value property.
            if (_values.Count == 0)
            {
                result = (double)LuxValueProperty.DefaultMetadata.DefaultValue;
            }
            else
            {
                // get the median value from the current sensor array. This helps to return
                // a more uniform result when one or more sensor is being covered or in a shadow.
                result = GetMedianValue(_values.Values);
            }
            return result;
        }

        /// <summary>
        /// Gets the median value from an <see cref="ICollection{Double}"/>.
        /// </summary>
        /// <param name="iCollection">The collection of values</param>
        /// <returns>The median value of the collection</returns>
        private static double GetMedianValue(ICollection<double> iCollection)
        {
            if (iCollection.Count > 0)
            {
                double[] values = new double[iCollection.Count];
                iCollection.CopyTo(values, 0);
                Array.Sort<double>(values);

                return (values[values.Length / 2]);
            }
            return 0;
        }

        /// <summary>
        /// Animates the specified dependency property using the specified timeline on the DispatcherObject's dispatch thread.
        /// </summary>
        private void AnimateDependencyPropertyValue(DependencyProperty property, AnimationTimeline timeline)
        {
            // Ensure that the dependency object animation is fired on the correct thread,
            // and marshal this call to the dispatcher thread for this object if it is not.
            if (Dispatcher == Dispatcher.CurrentDispatcher)
            {
                // the current thread is this object's dispatcher thread, so go ahead and execute.
                BeginAnimation(property, timeline);
            }
            else
            {
                // the current thread is not the object's dispatcher thread, so invoke to the dispatch thread.
                Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new BeginAnimationDelegate(BeginAnimation),
                    property, new object[] { timeline });
            }
        }

        #region Delayed Update Helper Functions
        /// <summary>
        /// Determines if the provided value in lux exceeds the default update threshold.
        /// </summary>
        private bool DoesLuxValueExceedThreshold(double luxValue)
        {
            // if the last value is zero it hasn't been initialized yet, and therefore always reset.
            if (_lastValue == 0)
            {
                return true;
            }

            // lux values are not linear when correlated to human perception, they tend to the
            // following chart:
            //  0     -> 10     Dark
            //  10    -> 300    Dim Indoor
            //  300   -> 800    Normal Indoor
            //  800   -> 10000  Bright Indoor
            //  10000 -> 30000  Overcast Outdoor
            //  30000 -> 100000 Direct sunlight
            //
            // Since the threshold value is based on triggering changes in the user interface,
            // we're going to convert our lux values into a threshold that is based on human 
            // perception. To do this, we're going to use a log10 function.
            double newValue = Math.Log10(luxValue) / Math.Log10(MaximumSensorReportValue);
            double lastValue = Math.Log10(_lastValue) / Math.Log10(MaximumSensorReportValue);

            // return the absolute distance compared to the default thresholding value
            return (Math.Abs(newValue - lastValue) > DefaultChangeThreshold);
        }

        /// <summary>
        /// Determines if the time from the last update has exceeded the default delay to
        /// ensure that the ambient light value doesn't change too frequently.
        /// </summary>
        private bool HasUpdateTimeDelayElapsed(TimeSpan delay)
        {
            return (_lastUpdateTime.Add(delay) < DateTime.UtcNow);
        }

        /// <summary>
        /// Attempt to wait a specified amount of time before updating the current ambient light
        /// value, if another update is not pending.
        /// </summary>
        private void WaitForUpdateTimeDelayAndRetry(TimeSpan delay)
        {
            lock (_updateTimer)
            {
                // if an update is already pending we'll want to trigger that value instead of a new one
                // the timer interval is really the priority as to when the value is getting updated,
                // and can only be adjusted toward higher priorities (a higher priority refresh
                // trumps lower priority refresh, however a lower priority refresh can't trump a higher refresh).
                if (!_updateTimer.IsEnabled || _updateTimer.Interval > delay)
                {
                    _updateTimer.Stop();
                    _updateTimer.Interval = delay;
                    _updateTimer.Start();
                }
            }
        }

        /// <summary>
        /// Update timer callback to set the current sensor value.
        /// </summary>
        private void UpdateTimerCallbackHandler(object sender, EventArgs e)
        {
            lock (_updateTimer)
            {
                if (_updateTimer.IsEnabled)
                {
                    // stop the existing timer, then update the current value
                    _updateTimer.Stop();
                    ActivateSensorValueHandler(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Calculates the current composite sensor value for the external exposed property
        /// and then activates the value on the dependency property.
        /// </summary>
        private void ActivateSensorValueHandler(object sender, EventArgs e)
        {
            // this callback is in charge of updating the active value based on the 
            // current composite sensor device. It doesn't use any threshold values
            // or timer delays since we actually want to update the value and fire
            // an event (this function is called as a delayed callback from UpdateCompositeSensorValue).
            double compositeLuxValue = CalculateCompositeSensorValue();
            ActivateSensorValue(compositeLuxValue);
        }

        /// <summary>
        /// Activates the provided composite sensor value on the dependency property.
        /// </summary>
        private void ActivateSensorValue(double compositeSensorValueInLux)
        {
            // update the most recently updated values to be correct
            _lastUpdateTime = DateTime.UtcNow;
            _lastValue = compositeSensorValueInLux;

            // animate the composite lux dependency property value for external binding
            DoubleAnimation animation = new DoubleAnimation(compositeSensorValueInLux, AnimationTime);
            animation.Freeze();

            AnimateDependencyPropertyValue(LuxValueProperty, animation);
        }
        #endregion

        /// <summary>
        /// Updates the external composite sensor value to the current value
        /// </summary>
        private void UpdateCompositeSensorValue()
        {
            // calculate the new composite sensor value (since we're assuming it has just changed)
            double compositeLuxValue = CalculateCompositeSensorValue();

            // determine if the current threshold is exceeded by the new value by comparing 
            // the newly calculated value with the last cached value. If it is below a certain
            // threshold, there is no need to update the dependency property since the program impact
            // won't be worth the performance drawbacks of executing ambient light changes.
            if (DoesLuxValueExceedThreshold(compositeLuxValue))
            {
                // Determine if a specific amount of time has changed since the last ambient
                // light change has occurred. This time delay allows for the system not to
                // constantly be modifying the ambient light value, instead it will periodically
                // change the lighting value.
                if (HasUpdateTimeDelayElapsed(MinimumUpdateDelayTime))
                {
                    // Stop any pending update since we are immediately updating the light value
                    lock (_updateTimer)
                    {
                        _updateTimer.Stop();
                    }

                    ActivateSensorValue(compositeLuxValue);
                }
                else
                {
                    // wait a minimal amount of time since we've passed a threshold boundary
                    WaitForUpdateTimeDelayAndRetry(MinimumUpdateDelayTime);
                }
            }
            else
            {
                // wait the maximum delay time since we haven't had a significant ambient light change
                WaitForUpdateTimeDelayAndRetry(MaximumUpdateDelayTime);
            }
        }

        /// <summary>
        /// Sets the specified sensor's lux value and updates the composite value.
        /// </summary>
        private void SetSensorLightLuxValue(Guid sensorId, double value)
        {
            // set the mapping of sensor ids to values to include the new value.
            _values[sensorId] = value;

            // update the composite lighting value and fire a value changed event if
            // the new sensor value exceeds the thresholding in UpdateCompositeSensorValue
            UpdateCompositeSensorValue();
        }
        #endregion
                
        #region ISensorEvents Members
        /// <summary>
        /// Fired when a sensor object's state is changed.
        /// </summary>
        void OnStateChanged(Sensor sensor, SensorState state)
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
        /// Fired when sensor data is updated.
        /// </summary>
        void OnDataUpdated(Sensor sensor, SensorDataReport newData)
        {
            AmbientLightSensorDataReport alsReport = (AmbientLightSensorDataReport)newData;
            System.Diagnostics.Debug.WriteLine("Data event timestamp: " + alsReport.Timestamp.ToString());

            // get the illuminance property value
            double lightLux = alsReport.IlluminanceLux;

            // Get the sensor id so we can associate the sensor lux value
            // with a specific sensor id in our collection
            Guid sensorId = sensor.SensorID;
            if (sensorId != Guid.Empty)
            {
                // Set the sensor light value
                SetSensorLightLuxValue(sensorId, lightLux);
            }
        }

        /// <summary>
        /// Fired when a sensor is removed.
        /// </summary>
        void OnLeave(Sensor leavingSensor, Guid sensorId)
        {
            // remove the sensor from the list of active sensor values
            if (_values.ContainsKey(sensorId))
            {
                _values.Remove(sensorId);
            }

            if (leavingSensor is AmbientLightSensor && Sensors.Remove((AmbientLightSensor) leavingSensor))
            {
                leavingSensor.DataUpdated -= OnDataUpdated;
                leavingSensor.StateChanged -= OnStateChanged;
                leavingSensor.SensorLeave -= OnLeave;
            }

            // re-calculate the composite sensor value now that the sensor
            // has been removed from the system.
            UpdateCompositeSensorValue();
        }

        #endregion

        #region ISensorManagerEvents Members

        /// <summary>
        /// Fired when a sensor is attached.
        /// </summary>
        void OnSensorEnter(Sensor sensor, SensorState state)
        {
            AmbientLightSensor als = sensor as AmbientLightSensor;
            // check that the sensor is an ambient light sensor
            if (als != null)
                AddSensor(als);
        }

        #endregion

        #region Freezable Abstract Class Implementation
        protected override Freezable CreateInstanceCore()
        {
            // Freeze() is not supported on this object, it inherits from Animatable
            // in order to apply an animation on the Value property. Since this is used
            // from one thread, supporting this functionality isn't needed.
            throw new NotSupportedException();
        }
        #endregion

        /// <summary>
        /// Gets the collection of ambient light sensors that are currently active.
        /// </summary>
        protected ICollection<AmbientLightSensor> Sensors
        {
            get
            {
                if (_sensors == null)
                {
                    // Create a sensor collection to keep track of active sensors.
                    _sensors = new Collection<AmbientLightSensor>();
                }
                return _sensors;
            }
        }

        /// <summary>
        /// Add all sensors from the specified collection as sensor sources for
        /// ambient light. All sensors from this collection are assumed to be
        /// ambient light sensors.
        /// </summary>
        private void AddSensorCollection(AmbientLightSensor[] sensorCollection)
        {
            foreach (AmbientLightSensor sensor in sensorCollection)
            {
                AddSensor(sensor);
            }
        }

        /// <summary>
        /// Adds an individual sensor as a sensor source for ambient light. This
        /// sensor is assumed to be an ambient light sensor.
        /// </summary>
        /// <param name="sensor"></param>
        private void AddSensor(AmbientLightSensor sensor)
        {
            if (sensor != null)
            {
                System.Diagnostics.Debug.WriteLine("AddSensor: " + sensor.FriendlyName);
                sensor.DataUpdated += new SensorDataUpdatedEventHandler(OnDataUpdated);
                sensor.SensorLeave += new SensorLeaveEventHandler(OnLeave);
                Sensors.Add(sensor);

                if (Sensors.Contains(sensor))
                {
                    // get the current sensor state
                    switch (sensor.State)
                    {
                        case SensorState.Ready:
                            RequestSensorDataReport(sensor);
                            break;

                        case SensorState.AccessDenied:
                            RequestSensorAccess(sensor);
                            break;

                        default:
                            // sensor device state is currently not actionable.
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Query the sensor for any sensor data it has, then update the sensor
        /// data by using the OnDataUpdated callback.
        /// </summary>
        private void RequestSensorDataReport(AmbientLightSensor sensor)
        {
            if (sensor != null)
            {
                // If the sensor is ready, we can query for the current sensor value
                AmbientLightSensorDataReport data = null;
                try
                {
                    data = (AmbientLightSensorDataReport) sensor.GetDataReport();
                }
                catch (COMException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }

                if (data != null)
                {
                    // Initialize the sensor data value using the callback
                    OnDataUpdated(sensor, data);
                }
            }
        }

        /// <summary>
        /// Surface a sensor dialog to request access to the sensor device. A user may
        /// restrict access to specific sensors due to security and privacy concerns they may have,
        /// depending on what data the sensor provides.
        /// </summary>
        private void RequestSensorAccess(Sensor sensor)
        {
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
            }
        }

        #region LuxValue Dependency Property
        /// <summary>
        /// The LuxValue Dependency Property
        /// </summary>
        private static readonly DependencyProperty LuxValueProperty =
            DependencyProperty.Register(
                "LuxValue", typeof(double), typeof(LightSensorProvider),
                    new UIPropertyMetadata(DefaultNormalIndoorAmbientLightLevelInLux, null, null, false));

        /// <summary>
        /// Gets the current ambient sensor light value, in lux
        /// </summary>
        public double LuxValue
        {
            get { return (double)GetValue(LuxValueProperty); }
        }
        #endregion
    }
}