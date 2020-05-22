// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Runtime.InteropServices;
using SENSOR_CATEGORY_ID = System.Guid;
using SENSOR_ID = System.Guid;
using SENSOR_TYPE_ID = System.Guid;
using ULONG = System.UInt32;
using VARIANT_BOOL = System.Boolean;



namespace Windows7.Sensors.Internal
{
    /// <summary>
    /// A COM interop wrapper for the ISensor interface.
    /// </summary>
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("5FA08F80-2657-458E-AF75-46F73FA6AC5C")]
    internal interface ISensor
    {
        /// <summary>
        /// Unique ID of sensor within the sensors platform
        /// </summary>
        /// <param name="id">The unique ID to be returned</param>
        void GetID(out SENSOR_ID id);

        /// <summary>
        /// Category of sensor Ex: Location
        /// </summary>
        /// <param name="sensorCategory">The sensor category to be returned</param>
        void GetCategory(out SENSOR_CATEGORY_ID sensorCategory);

        /// <summary>
        /// Specific type of sensor: Ex: IPLocationSensor
        /// </summary>
        /// <param name="sensorType">The sensor Type to be returned</param>
        void GetType(out SENSOR_TYPE_ID sensorType);

        /// <summary>
        /// Human readable name for sensor
        /// </summary>
        /// <param name="friendlyName">The friendly name for the sensor</param>
        void GetFriendlyName([Out, MarshalAs(UnmanagedType.BStr)] out string friendlyName);

        /// <summary>
        /// Sensor metadata: make, model, serial number, etc
        /// </summary>
        /// <param name="key">The PROPERTYKEY for the property to be retrieved</param>
        /// <param name="property">The property returned</param>
        void GetProperty([In] ref PropertyKey key, out PROPVARIANT property);

        /// <summary>
        /// Bulk Sensor metadata query: make, model, serial number, etc
        /// </summary>
        /// <param name="keys">The PROPERTYKEY collection for the properties to be retrieved</param>
        /// <param name="properties">The properties returned</param>
        void GetProperties(
            [In, MarshalAs(UnmanagedType.Interface)] IPortableDeviceKeyCollection keys,
            [Out, MarshalAs(UnmanagedType.Interface)] out IPortableDeviceValues properties);

        /// <summary>
        /// Get the array of SensorDataField objects that describe the individual values that can be reported by the sensor
        /// </summary>
        /// <param name="dataFields">A collection of PROPERTYKEY structures representing the data values reported by the sensor</param>
        void GetSupportedDataFields(
            [Out, MarshalAs(UnmanagedType.Interface)] out IPortableDeviceKeyCollection dataFields);

        /// <summary>
        /// Bulk Sensor metadata set for settable properties
        /// </summary>
        /// <param name="properties">The properties to be set</param>
        /// <param name="results">The PROPERTYKEY/HRESULT pairs indicating success/failure for each property set</param>
        void SetProperties(
            [In, MarshalAs(UnmanagedType.Interface)] IPortableDeviceValues properties,
            [Out, MarshalAs(UnmanagedType.Interface)] out IPortableDeviceValues results);

        /// <summary>
        /// Reports whether or not a sensor can deliver the requested data type
        /// </summary>
        /// <param name="key">The GUID to find matching PROPERTYKEY structures for</param>
        /// <param name="isSupported">A collection of PROPERTYKEY structures representing the data values</param>
        void SupportsDataField(
            [In] PropertyKey key,
            [Out, MarshalAs(UnmanagedType.VariantBool)] out VARIANT_BOOL isSupported);

        /// <summary>
        /// Get the sensor state
        /// </summary>
        /// <param name="state">The SensorState returned</param>
        void GetState([Out, MarshalAs(UnmanagedType.U4)] out SensorState state);

        /// <summary>
        /// Get the most recent ISensorDataReport for the sensor
        /// </summary>
        /// <param name="dataReport">The data report returned</param>
        void GetData([Out, MarshalAs(UnmanagedType.Interface)] out ISensorDataReport dataReport);

        /// <summary>
        /// Reports whether or not a sensor supports the specified event.
        /// </summary>
        /// <param name="eventGuid">The event identifier</param>
        /// <param name="isSupported">true if the event is supported, otherwise false</param>
        void SupportsEvent(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid eventGuid,
            [Out, MarshalAs(UnmanagedType.VariantBool)] out VARIANT_BOOL isSupported);

        void GetEventInterest(
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] out Guid[] pValues, [Out] out ULONG count);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pValues">The array of GUIDs representing sensor events of interest</param>
        /// <param name="count">The number of guids included</param>
        void SetEventInterest([In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] Guid[] pValues, [In] ULONG count);

        /// <summary>
        /// Subscribe to ISensor events
        /// </summary>
        /// <param name="pEvents">An interface pointer to the callback object created for events</param>
        void SetEventSink([In, MarshalAs(UnmanagedType.Interface)] ISensorEvents pEvents);
    }
}