// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Windows7.Sensors.Internal;
using Windows7.Sensors.Sensors;



namespace Windows7.Sensors.Internal
{
    #region Helper types

    /// <summary>
    /// Data associated with a sensor type GUID.
    /// </summary>
    internal struct SensorTypeData
    {
        private Type _sensorType;
        private SensorDescriptionAttribute _sda;

        public SensorTypeData(Type sensorClassType, SensorDescriptionAttribute sda)
        {
            _sensorType = sensorClassType;
            _sda = sda;
        }

        public Type SensorType
        {
            get { return _sensorType; }
        }


        public SensorDescriptionAttribute Attr
        {
            get { return _sda; }
        }
    }
        
    internal class SensorManagerEventSink : ISensorManagerEvents
    {
        #region ISensorManagerEventsInternal Members

        public void OnSensorEnter(ISensor pSensor, SensorState state)
        {
            Windows7.Sensors.SensorManager.OnSensorEnter(pSensor, state);
        }

        #endregion
    }

    #endregion
}

namespace Windows7.Sensors
{
    /// <summary>
    /// Provides enumratation of sensors by <see cref="Sensor"/>-derived type or category, type or instance GUIDs.
    /// Exposes an event notifying about availability of new sensors.
    /// </summary>
    /// <remarks>
    /// This is a singleton class. Access the instance via the <see cref="SensorManager.Instance"/> property.
    /// </remarks>
    public static class SensorManager
    {
        #region Static fields
        
        /// <summary>
        /// Sensor type GUID -> .NET Type + report type
        /// </summary>
        private static Dictionary<Guid, SensorTypeData> _guidToSensorDescr = new Dictionary<Guid, SensorTypeData>();
        
        /// <summary>
        /// .NET type -> type GUID.
        /// </summary>      
        private static Dictionary<Type, Guid> _sensorTypeToGuid = new Dictionary<Type, Guid>();
        private static ISensorManager _sensorMgr;   
        private static SensorManagerEventSink _eventSink;
                
        #endregion

        #region Events

        /// <summary>
        /// Event notifying about sensors becoming available.
        /// </summary>
        public static event SensorEnterEventHandler SensorEnter;

        #endregion

        #region Constructors

        static SensorManager()
        {
            BuildSensorTypeMap();
            _sensorMgr = new Windows7.Sensors.Internal.SensorManager();
            _eventSink = new SensorManagerEventSink();
            Thread.MemoryBarrier();
            _sensorMgr.SetEventSink(_eventSink);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Interrogates assemblies currently loaded into the AppDomain for classes deriving from <see cref="Sensor"/>.
        /// Builds data structures which map those types to sensor type GUIDs. 
        /// </summary>
        private static void BuildSensorTypeMap()
        {
            // TODO: Handle new assembly loads
            Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly asm in loadedAssemblies)
            {
                Type[] exportedTypes = asm.GetExportedTypes();
                foreach (Type t in exportedTypes)
                {
                    if (t.IsSubclassOf(typeof(Sensor)) && t.IsPublic && !t.IsAbstract && !t.IsGenericType)
                    {
                        object[] attrs = t.GetCustomAttributes(typeof(SensorDescriptionAttribute), true);
                        if (attrs != null && attrs.Length > 0)
                        {
                            SensorDescriptionAttribute sda = (SensorDescriptionAttribute)attrs[0];
                            SensorTypeData stm = new SensorTypeData(t, sda);
                            // TODO: Handle multiple types with same GUIDs
                            _guidToSensorDescr.Add(sda.SensorTypeGuid, stm);
                            _sensorTypeToGuid.Add(t, sda.SensorTypeGuid);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// A handler for the underlying COM object's OnSensorEnter event, which notifies about new sensor's availability.
        /// Fires the <see cref="SensorAttached"/> event.
        /// </summary>
        /// <param name="pSensor">The newly added sensor.</param>
        /// <param name="state">The current state of the added sensor.</param>
        internal static void OnSensorEnter(ISensor pSensor, SensorState state)
        {
            if (SensorEnter != null)
            {
                Sensor sensor = GetSensorWrapperInstance(pSensor);
                SensorEnter(sensor, state);
            }
        }

        /// <summary>
        /// Returns an instance of a sensor wrapper appropritate for the given sensor COM interface.
        /// If no appropriate sensor wrapper type could be found, the object created will be of the base-class type <see cref="Sensor"/>.
        /// </summary>
        /// <param name="pSensor">The underlying sensor COM interface.</param>
        /// <returns>A wrapper instance.</returns>
        private static Sensor GetSensorWrapperInstance(ISensor pSensor)
        {
            Guid sensorTypeGuid;
            pSensor.GetType(out sensorTypeGuid);
            
            SensorTypeData stm;
            Type sensorClassType = sensorClassType = _guidToSensorDescr.TryGetValue(sensorTypeGuid, out stm) ? stm.SensorType : typeof(UnknownSensor);
                        
            Sensor sensor = (Sensor)Activator.CreateInstance(sensorClassType);
            sensor.InitializeSensor(pSensor);
            return sensor;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Retrieves a list of sensors by the category GUID.
        /// </summary>
        /// <param name="sensorCategory">The category GUID. You can find a list of GUIDs in <see cref="SensorWrapper.SensorCategories"/> class.</param>
        /// <returns>A list of sensor wrappers. You should cast them to derived types, if available. If no sensors are available, an empty array is returned.</returns>
        public static Sensor[] GetSensorsByCategory(Guid sensorCategory)
        {
            ISensorCollection sensorCollection = null;
            _sensorMgr.GetSensorsByCategory(sensorCategory, out sensorCollection);
            if (sensorCollection != null)
            {
                uint sensorCount = 0;
                sensorCollection.GetCount(out sensorCount);
                Sensor[] foundSensors = new Sensor[sensorCount];

                for (uint i = 0; i < sensorCount; i++)
                {
                    ISensor iSensor = null;
                    sensorCollection.GetAt(i, out iSensor);
                    Sensor sensor = GetSensorWrapperInstance(iSensor);
                    foundSensors[i] = sensor;
                }

                return foundSensors;
            }

            return new Sensor[0];
        }

        /// <summary>
        /// Retrieves a list of sensors by the type GUID.
        /// </summary>
        /// <param name="sensorCategory">The type GUID. You can find a list of GUIDs in <see cref="SensorWrapper.SensorTypes"/> class.</param>
        /// <returns>A list of sensor wrappers. You should cast them to derived types, if available.</returns>
        public static Sensor[] GetSensorsByType(Guid sensorType)
        {
            ISensorCollection sensorCollection = null;
            _sensorMgr.GetSensorsByType(sensorType, out sensorCollection);

            if (sensorCollection != null)
            {
                uint sensorCount = 0;
                sensorCollection.GetCount(out sensorCount);
                Sensor[] foundSensors = new Sensor[sensorCount];

                for (uint i = 0; i < sensorCount; i++)
                {
                    ISensor iSensor = null;
                    sensorCollection.GetAt(i, out iSensor);
                    Sensor sensor = GetSensorWrapperInstance(iSensor);
                    foundSensors[i] = sensor;
                }

                return foundSensors;
            }

            return new Sensor[0];
        }

        /// <summary>
        /// Retrieves a list of all sensors.
        /// </summary>
        /// <returns>A list of sensors wrappers. You should cast them to derived types, if available.</returns>
        public static Sensor[] GetAllSensors()
        {
            return GetSensorsByCategory(SensorCategories.All);
        }
        
        /// <summary>
        /// Retrieves a list of sensors by the wrapper type (classes which derive from <see cref="Sensor"/>.
        /// This overload is the most type-safe to work with sensors.
        /// </summary>
        /// <typeparam name="T">The type of the wrapper.</typeparam>
        /// <returns>A list of sensor wrappers. If no sensors are available, an empty array is returned.</returns>
        public static T[] GetSensorsByType<T>() where T : Sensor, new()
        {
            Guid sensorGuid;
            if (!_sensorTypeToGuid.TryGetValue(typeof(T), out sensorGuid))
                return null;

            Sensor[] sensors = GetSensorsByType(sensorGuid);
            T[] castSensors = new T[sensors.Length];

            for (int i = 0; i < sensors.Length; i++)
            {
                castSensors[i] = (T) sensors[i];
            }

            return castSensors;
        }
        /// <summary>
        /// Retries a sensor by its unique instance ID GUID.
        /// </summary>
        /// <param name="sensorID">Sensor's instance GUID.</param>
        /// <returns>The sensor, if exists. null otherwise.</returns>
        public static Sensor GetSensorByID(Guid sensorID)
        {
            ISensor iSensor = null;
            _sensorMgr.GetSensorByID(sensorID, out iSensor);
            if (iSensor == null)
                return null;
            return GetSensorWrapperInstance(iSensor);
        }

        /// <summary>
        /// Opens a system dialog box to request user permission to access sensor data.
        /// </summary>
        /// <param name="parentWindow">HWND handle to a window that can act as a parent to the permissions dialog box.</param>
        /// <param name="modal">Speficifies whether the window should be modal.</param>
        /// <param name="sensors">The sensors for which to request permission.</param>      
        public static void RequestPermission(IntPtr parentWindow, bool modal, params Sensor[] sensors)
        {
            if (sensors == null || sensors.Length == 0)
                throw new ArgumentNullException("sensors", "Sensors collection must not be null or empty.");

            ISensorCollection sensorCollection = new SensorCollection();

            foreach (Sensor sensor in sensors)
            {
                sensorCollection.Add((ISensor)sensor.InnerObject);
            }

            _sensorMgr.RequestPermissions(parentWindow, sensorCollection, modal);
        }

        #endregion
    }
}