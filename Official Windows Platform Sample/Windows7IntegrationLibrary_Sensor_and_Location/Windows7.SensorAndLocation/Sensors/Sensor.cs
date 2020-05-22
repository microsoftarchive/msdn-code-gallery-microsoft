// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows7.Sensors.Internal;
using System.Runtime.InteropServices;



namespace Windows7.Sensors
{
    #region Helper types

    /// <summary>
    /// Structure containing the property ID (key) and value.
    /// </summary>
    public struct DataFieldInfo : IEquatable<DataFieldInfo>
    {
        private PropertyKey _propKey;
        private object _value;

        /// <summary>
        /// Constructs the structure.
        /// </summary>
        /// <param name="propKey">Property ID (key).</param>
        /// <param name="value">Property value. Type must be valid for the ID.</param>
        public DataFieldInfo(PropertyKey propKey, object value)
        {
            _propKey = propKey;
            _value = value;
        }

        /// <summary>
        /// Returns the property key.
        /// </summary>
        public PropertyKey Key
        {
            get { return _propKey; }
        }

        /// <summary>
        /// Returns property's value.
        /// </summary>
        public object Value
        {
            get { return _value; }
        }

        public override int GetHashCode()
        {
            int valHashCode = _value != null ? _value.GetHashCode() : 0;
            return _propKey.GetHashCode() ^ valHashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (!(obj is DataFieldInfo))
                return false;

            DataFieldInfo other = (DataFieldInfo)obj;
            return _value.Equals(other._value) && _propKey.Equals(other._propKey);
        }



        #region IEquatable<DataFieldInfo> Members

        public bool Equals(DataFieldInfo other)
        {
            return _value.Equals(other._value) && _propKey.Equals(other._propKey);
        }

        #endregion

        public static bool operator ==(DataFieldInfo a, DataFieldInfo b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(DataFieldInfo a, DataFieldInfo b)
        {
            return !a.Equals(b);
        }
    }

    #endregion
    
    /// <summary>
    /// Represents a sensor. Exposes events and properties. Derived types may provide more events and properties. 
    /// </summary>
    public abstract class Sensor : ISensorEvents
    {
        #region Private fields

        private ISensor _iSensor;
        private static Dictionary<Sensor, bool> _instances = new Dictionary<Sensor, bool>();
        private ISensorDataReport _lastReport;

        #endregion
                
        #region Events

        public event SensorLeaveEventHandler SensorLeave;
        public event SensorDataUpdatedEventHandler DataUpdated;
        public event SensorEventHandler EventReceived;
        public event SensorStateChangedEventHandler StateChanged;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs the sensor instance.
        /// </summary>
        protected Sensor()
        {
            lock (_instances)
            {
                _instances.Add(this, true);
            }
        }

        #endregion

        #region Internal properties

        internal ISensor InnerObject
        {
            get { return _iSensor; }
        }

        #endregion

        #region Public properties
        
        /// <summary>
        /// Returns the sensor instance GUID.
        /// </summary>
        public Guid SensorID
        {
            get
            {
                Guid id;
                _iSensor.GetID(out id);
                return id;
            }
        }

        /// <summary>
        /// Returns the sensor category GUID.
        /// </summary>
        public Guid CategoryID
        {
            get
            {
                Guid id;
                _iSensor.GetCategory(out id);
                return id;
            }
        }

        /// <summary>
        /// Returns the sensor type GUID.
        /// </summary>
        public Guid TypeID
        {
            get
            {
                Guid id;
                _iSensor.GetType(out id);
                return id;
            }
        }

        /// <summary>
        /// Returns the sensor's friendly name.
        /// </summary>
        public string FriendlyName
        {
            get
            {
                string name = null;
                _iSensor.GetFriendlyName(out name);
                return name;
            }
        }

        /// <summary>
        /// Returns the sensor's current state.
        /// </summary>
        public SensorState State
        {
            get
            {
                SensorState state;
                _iSensor.GetState(out state);
                return state;
            }
        }

        /// <summary>
        /// Gets or sets the report interval.
        /// </summary>
        public uint ReportInterval
        {
            get
            {
                return (uint)GetProperty(SensorPropertyKeys.SENSOR_PROPERTY_CURRENT_REPORT_INTERVAL);
            }

            set
            {
                SetProperties(new DataFieldInfo[] { new DataFieldInfo(SensorPropertyKeys.SENSOR_PROPERTY_CURRENT_REPORT_INTERVAL, value) });
            }
        }

        /// <summary>
        /// Gets the minimal report interval.
        /// </summary>
        public uint MinReportInterval
        {
            get
            {
                return (uint)GetProperty(SensorPropertyKeys.SENSOR_PROPERTY_MIN_REPORT_INTERVAL);
            }
        }

        /// <summary>
        /// Gets the manufactorer of the sensor.
        /// </summary>
        public string SensorManufacturer
        {
            get
            {
                return (string)GetProperty(SensorPropertyKeys.SENSOR_PROPERTY_MANUFACTURER);
            }
        }

        /// <summary>
        /// Gets the sensor model.
        /// </summary>
        public string SensorModel
        {
            get
            {
                return (string)GetProperty(SensorPropertyKeys.SENSOR_PROPERTY_MODEL);
            }
        }

        /// <summary>
        /// Gets the sensor's serial number.
        /// </summary>
        public string SensorSerialNumber
        {
            get
            {
                return (string)GetProperty(SensorPropertyKeys.SENSOR_PROPERTY_SERIAL_NUMBER);
            }
        }

        /// <summary>
        /// Gets the sensor's description.
        /// </summary>
        public string SensorDescription
        {
            get
            {
                return (string)GetProperty(SensorPropertyKeys.SENSOR_PROPERTY_DESCRIPTION);
            }
        }

        /// <summary>
        /// Gets the sensor's connection type.
        /// </summary>
        public uint SensorConnectionType
        {
            get
            {
                return (uint)GetProperty(SensorPropertyKeys.SENSOR_PROPERTY_CONNECTION_TYPE);
            }
        }

        /// <summary>
        /// Gets or sets the sensor's change sensitivity
        /// </summary>
        public uint ChangeSensitivity
        {
            get
            {
                return (uint)GetProperty(SensorPropertyKeys.SENSOR_PROPERTY_CHANGE_SENSITIVITY);
            }
            set
            {
                SetProperties(new DataFieldInfo[] { new DataFieldInfo(SensorPropertyKeys.SENSOR_PROPERTY_CHANGE_SENSITIVITY, value) });
            }
        }

        /// <summary>
        /// Gets the sensor device path
        /// </summary>
        public string SensorDevicePath
        {
            get
            {
                return (string)GetProperty(SensorPropertyKeys.SENSOR_PROPERTY_DEVICE_ID);
            }
        }

        /// <summary>
        /// Gets sensor accuracy values.
        /// </summary>
        public /*PropertyKey[] */ int SensorAccuracy
        {
            get
            {
                //IPortableDeviceKeyCollection collection = (IPortableDeviceKeyCollection) GetProperty(SensorPropertyKeys.SENSOR_PROPERTY_ACCURACY);
                
                //uint nItems = 0;
                //collection.GetCount(out nItems);
                //PropertyKey[] dataFields = new PropertyKey[nItems];

                //for (uint i = 0; i < nItems; i++)
                //{
                //    PropertyKey propKey;
                //    collection.GetAt(i, out propKey);
                //    dataFields[i] = propKey;
                //}

                //return dataFields;

                return (int)GetProperty(SensorPropertyKeys.SENSOR_PROPERTY_ACCURACY);
            }
        }

        /// <summary>
        /// Gets sensor resolution values.
        /// </summary>
        public /*PropertyKey[] */ int SensorResolution
        {
            get
            {
                //IPortableDeviceKeyCollection collection = (IPortableDeviceKeyCollection)GetProperty(SensorPropertyKeys.SENSOR_PROPERTY_RESOLUTION);

                //uint nItems = 0;
                //collection.GetCount(out nItems);
                //PropertyKey[] dataFields = new PropertyKey[nItems];

                //for (uint i = 0; i < nItems; i++)
                //{
                //    PropertyKey propKey;
                //    collection.GetAt(i, out propKey);
                //    dataFields[i] = propKey;
                //}

                //return dataFields;

                return (int)GetProperty(SensorPropertyKeys.SENSOR_PROPERTY_RESOLUTION);
            }
        }

        /// <summary>
        /// Gets an array representing the light response curve.
        /// </summary>
        public /*uint[]*/ int LightResponseCurve
        {
            get
            {
                return /*(uint[])*/ (int) GetProperty(SensorPropertyKeys.SENSOR_PROPERTY_LIGHT_RESPONSE_CURVE);
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Synchronously retrieves a data report from the sensor.
        /// </summary>
        /// <returns>Data report.</returns>
        public SensorDataReport GetDataReport()
        {
            ISensorDataReport iReport = null;
            _iSensor.GetData(out iReport);
            return GetDataReport(iReport, _iSensor);
        }

        /// <summary>
        /// Retrieves the last data report asynchronously received from the sensor. Otherwise, returns null.
        /// </summary>
        /// <returns>Data report or null.</returns>
        public SensorDataReport GetLastAsyncDataReport()
        {
            ISensorDataReport lastRep = _lastReport;
            if (lastRep == null)
                return null;
            return GetDataReport(lastRep, _iSensor);
        }
                
        /// <summary>
        /// Returns the details of supported properties.
        /// </summary>
        /// <returns>Array of supported data fields.</returns>
        public PropertyKey[] GetSupportedDataFields()
        {
            IPortableDeviceKeyCollection keyCollection;
            _iSensor.GetSupportedDataFields(out keyCollection);

            uint nItems = 0;
            keyCollection.GetCount(out nItems);
            PropertyKey[] dataFields = new PropertyKey[nItems];

            for (uint i = 0; i < nItems; i++)
            {
                PropertyKey propKey;
                keyCollection.GetAt(i, out propKey);
                dataFields[i] = propKey;
            }

            return dataFields;
        }

        /// <summary>
        /// Returns whether the sensor supports the given data field.
        /// </summary>
        /// <param name="field">Property ID to check.</param>
        /// <returns>true if supported. false otherwise.</returns>
        public bool SupportsDataField(PropertyKey field)
        {
            bool supported = false;
            _iSensor.SupportsDataField(field, out supported);
            return supported;
        }

        /// <summary>
        /// Returns whether the sensor supports the given event.
        /// </summary>
        /// <param name="eventGuid">The event GUID.</param>
        /// <returns>true if supported. false otherwise.</returns>
        public bool SupportsEvent(Guid eventGuid)
        {
            bool supported = false;
            _iSensor.SupportsEvent(eventGuid, out supported);
            return supported;
        }

        /// <summary>
        /// Retrieves a property value by property key.
        /// </summary>
        /// <param name="propKey">Property key.</param>
        /// <returns>Property's value.</returns>
        public object GetProperty(PropertyKey propKey)
        {
            PROPVARIANT propVariant = new PROPVARIANT();
            try
            {
                _iSensor.GetProperty(ref propKey, out propVariant);
                return propVariant.Value;
            }
            finally
            {
                propVariant.Clear();
            }
        }

        /// <summary>
        /// Retrieves a property value by property inex.
        /// Assumes the GUID component in the property key takes the sensor's type GUID.
        /// </summary>
        /// <param name="propKey">Property index.</param>
        /// <returns>Property's value.</returns>
        public object GetProperty(int propIdx)
        {
            PropertyKey propKey = PropertyKey.Create(this.TypeID, propIdx);
            return GetProperty(propKey);
        }

        /// <summary>
        /// Retrives the values of multiple properties.
        /// </summary>
        /// <param name="propKeys">Properties to retrieve.</param>
        /// <returns>A dictionary containing the property keys and values.</returns>
        public IDictionary<PropertyKey, object> GetProperties(PropertyKey[] propKeys)
        {
            if (propKeys == null || propKeys.Length == 0)
                throw new ArgumentNullException("propKeys", "Property keys array must not be null or empty.");

            IPortableDeviceKeyCollection keyCollection = new PortableDeviceKeyCollection();
            try
            {
                IPortableDeviceValues valuesCollection;

                for (int i = 0; i < propKeys.Length; i++)
                {
                    PropertyKey propKey = propKeys[i];
                    keyCollection.Add(ref propKey);
                }

                _iSensor.GetProperties(keyCollection, out valuesCollection);

                Dictionary<PropertyKey, object> data = new Dictionary<PropertyKey, object>();

                if (valuesCollection == null)
                    return data;

                uint count = 0;
                valuesCollection.GetCount(ref count);

                for (uint i = 0; i < count; i++)
                {
                    PropertyKey propKey = new PropertyKey();
                    PROPVARIANT propVal = new PROPVARIANT();
                    valuesCollection.GetAt(i, ref propKey, out propVal);

                    try
                    {
                        data.Add(propKey, propVal.Value);
                    }
                    finally
                    {
                        propVal.Clear();
                    }
                }

                return data;
            }
            finally
            {
                Marshal.ReleaseComObject(keyCollection);
            }
        }

        /// <summary>
        /// Retrives the values of multiple properties.
        /// </summary>
        /// <param name="propKeys">Properties to retrieve.</param>
        /// <returns>A dictionary containing the property keys and values.</returns>
        public IDictionary<PropertyKey, object> GetAllProperties()
        {
            IPortableDeviceValues valuesCollection;

            _iSensor.GetProperties(null, out valuesCollection);

            Dictionary<PropertyKey, object> data = new Dictionary<PropertyKey, object>();

            if (valuesCollection == null)
                return data;

            uint count = 0;
            valuesCollection.GetCount(ref count);

            for (uint i = 0; i < count; i++)
            {
                PropertyKey propKey = new PropertyKey();
                PROPVARIANT propVal = new PROPVARIANT();
                valuesCollection.GetAt(i, ref propKey, out propVal);

                try
                {
                    data.Add(propKey, propVal.Value);
                }
                finally
                {
                    propVal.Clear();
                }
            }

            return data;
        }

        /// <summary>
        /// Retrives the values of multiple properties by their indices.
        /// Assues that the GUID component of property keys is the sensor's type GUID.
        /// </summary>
        /// <param name="propIndices">Indices of properties to retrieve.</param>
        /// <returns>An array containing the property values.</returns>
        /// <remarks>
        /// If the values of some properties could not be retrieved, then the returned array will contain null values in the corresponding positions.
        /// </remarks>
        public object[] GetProperties(params int[] propIndices)
        {
            if (propIndices == null || propIndices.Length == 0)
                throw new ArgumentNullException("propIndices", "Property keys array must not be null or empty.");

            IPortableDeviceKeyCollection keyCollection = new PortableDeviceKeyCollection();
            try
            {
                IPortableDeviceValues valuesCollection;
                Dictionary<PropertyKey, int> propKeyToIdx = new Dictionary<PropertyKey, int>();

                for (int i = 0; i < propIndices.Length; i++)
                {
                    PropertyKey propKey = PropertyKey.Create(this.TypeID, propIndices[i]);
                    keyCollection.Add(ref propKey);
                    propKeyToIdx.Add(propKey, i);
                }

                object[] data = new object[propIndices.Length];
                _iSensor.GetProperties(keyCollection, out valuesCollection);

                if (valuesCollection == null)
                    return data;

                uint count = 0;
                valuesCollection.GetCount(ref count);             

                for (uint i = 0; i < count; i++)
                {
                    PropertyKey propKey = new PropertyKey();
                    PROPVARIANT propVal = new PROPVARIANT();
                    valuesCollection.GetAt(i, ref propKey, out propVal);

                    try
                    {
                        int idx = propKeyToIdx[propKey];
                        data[idx] = propVal.Value;
                    }
                    finally
                    {
                        propVal.Clear();
                    }
                }

                return data;
            }
            finally
            {
                Marshal.ReleaseComObject(keyCollection);
            }
        }
        
        /// <summary>
        /// Sets the values of multiple properties.
        /// </summary>
        /// <param name="data">The keys and values of properties to set.</param>
        /// <returns>The new values of the properties. Actual values may not match requested values.</returns>
        public IDictionary<PropertyKey, object> SetProperties(DataFieldInfo[] data)
        {        
            if (data == null || data.Length == 0)
                throw new ArgumentNullException("data", "Data field array must not be null or empty.");

            IPortableDeviceValues pdv = new PortableDeviceValues();

            for (int i = 0; i < data.Length; i++)
            {
                PropertyKey propKey = data[i].Key;
                object value = data[i].Value;
                if (value == null)
                    throw new ArgumentNullException("data", String.Format("Data contains a null value at index {0}", i));

                if (value is string)
                    pdv.SetStringValue(ref propKey, (string)value);
                else if (value is uint)
                    pdv.SetUnsignedIntegerValue(ref propKey, (uint)value);
                else if (value is int)
                    pdv.SetSignedIntegerValue(ref propKey, (int)value);
                else if (value is ulong)
                    pdv.SetUnsignedLargeIntegerValue(ref propKey, (ulong)value);
                else if (value is long)
                    pdv.SetSignedLargeIntegerValue(ref propKey, (long)value);
                else if (value is float || value is double)
                    pdv.SetFloatValue(ref propKey, (float)value);
                else if (value is bool)
                    pdv.SetBoolValue(ref propKey, ((bool)value) ? 1 : 0);
                else if (value is Guid)
                {
                    Guid guid = (Guid) value;
                    pdv.SetGuidValue(ref propKey, ref guid);
                }
                else if (value is byte[])
                {
                    byte[] buffer = (byte[])value;
                    pdv.SetBufferValue(ref propKey, buffer, (uint)buffer.Length);
                }
                else
                {
                    pdv.SetIUnknownValue(ref propKey, value);
                }
            }
            
            IPortableDeviceValues pdv2 = null;
            _iSensor.SetProperties(pdv, out pdv2);

            Dictionary<PropertyKey, object> results = new Dictionary<PropertyKey, object>();

            if (pdv2 == null)
                return results;

            uint count = 0;
            pdv2.GetCount(ref count);

            for (uint i = 0; i < count; i++)
            {
                PropertyKey propKey = new PropertyKey();
                PROPVARIANT propVal = new PROPVARIANT();
                try
                {
                    pdv2.GetAt(i, ref propKey, out propVal);
                    results.Add(propKey, propVal.Value);
                }
                finally
                {
                    propVal.Clear();
                }
            }

            return results;
        }

        /// <summary>
        /// Registers for the specified events by GUIDs.
        /// </summary>
        /// <param name="eventGuids">Events to register.</param>
        public void SetEventInterest(Guid[] eventGuids)
        {
            if (eventGuids == null || eventGuids.Length == 0)
                throw new ArgumentNullException("eventGuids", "Event GUIDs array must no be null or empty.");

            _iSensor.SetEventInterest(eventGuids, (uint)eventGuids.Length);
        }

        /// <summary>
        /// Retrieves the currently registeredevents.
        /// </summary>
        /// <returns></returns>
        public Guid[] GetEventInterest()
        {
            Guid[] events = null;
            uint count = 0;

            _iSensor.GetEventInterest(out events, out count);
            Debug.Assert(events.Length == count);
            return events;
        }

        #endregion

        #region Internal methods
                
        /// <summary>
        /// Creates a data report instance appropirate for the sensor.
        /// </summary>
        /// <param name="report">Underlying data report COM object.</param>
        /// <param name="sensor">Sensor COM object.</param>
        /// <returns></returns>
        internal SensorDataReport GetDataReport(ISensorDataReport report, ISensor sensor)
        {
            SensorDataReport sdr = CreateReport();
            sdr.InitializeReport(report, this);
            return sdr;
        }

        /// <summary>
        /// Allows calling <see cref="Sensor.Initialize"/> internally without making it public.
        /// Sets event sink.
        /// </summary>
        /// <param name="iSensor">Sensor COM object.</param>
        internal void InitializeSensor(ISensor iSensor)
        {
            _iSensor = iSensor;
            _iSensor.SetEventSink(this);
            Initialize();
        }
        
        #endregion

        #region Protected methods

        /// <summary>
        /// When overriden by derived types, provides a point where initializations which require access to the sensor can be performed.
        /// </summary>
        protected abstract void Initialize();

        /// <summary>
        /// When overriden by derived types, creates a data report object appropriate for the sensor.
        /// </summary>
        /// <returns></returns>
        protected abstract SensorDataReport CreateReport();

        #endregion

        #region Internal event handlers Members

        /// <summary>
        /// Handler of the state changed event. Fires the <see cref="StateChanged"/> event.
        /// </summary>
        /// <param name="sensor">COM sensor object for which this event was fired.</param>
        /// <param name="state">Sensor's new state.</param>
        void ISensorEvents.OnStateChanged(ISensor sensor, SensorState state)
        {
            if (StateChanged != null)
                StateChanged(this, state);
        }

        /// <summary>
        /// Handler of the data updated event. Fires the <see cref="DataUpdated"/> event.
        /// Creates an appropriate data report object.
        /// </summary>
        /// <param name="sensor">COM sensor object for which this event was fired.</param>
        /// <param name="newData">COM data report object.</param>
        void ISensorEvents.OnDataUpdated(ISensor sensor, ISensorDataReport newData)
        {
            _lastReport = newData;
            if (DataUpdated != null)
            {
                SensorDataReport sdr = GetDataReport(newData, sensor);
                DataUpdated(this, sdr);
            }
        }

        /// <summary>
        /// Handler for any event. Fires the <see cref="EventReceived"/> event.
        /// </summary>
        /// <param name="sensor">COM sensor object for which this event was fired.</param>
        /// <param name="eventID">Event GUID.</param>
        /// <param name="newData">COM data report object.</param>
        void ISensorEvents.OnEvent(ISensor sensor, Guid eventID, ISensorDataReport newData)
        {
            if (EventReceived != null)
            {
                SensorDataReport sdr = GetDataReport(newData, sensor);
                EventReceived(this, eventID, sdr);
            }
        }

        /// <summary>
        /// Handler of an event indicating cesation of sensor availability.
        /// </summary>
        /// <param name="sensorID">Sensor's instance GUID.</param>
        void ISensorEvents.OnLeave(Guid sensorID)
        {          
            _iSensor = null;
            _lastReport = null;
            
            if (SensorLeave != null)
                SensorLeave(this, sensorID);

            lock (_instances)
            {
                _instances.Remove(this);
            }
        }

        #endregion
    }
}