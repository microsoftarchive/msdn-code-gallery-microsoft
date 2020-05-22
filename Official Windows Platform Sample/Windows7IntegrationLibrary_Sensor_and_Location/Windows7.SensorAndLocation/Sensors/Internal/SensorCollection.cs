// Copyright (c) Microsoft Corporation.  All rights reserved.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using REFSENSOR_ID = System.Guid;
using ULONG = System.UInt32;


namespace Windows7.Sensors.Internal
{
    /// <summary>
    /// A COM interop wrapper for the ISensorCollection interface
    /// </summary>
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("23571E11-E545-4DD8-A337-B89BF44B10DF")]
    internal interface ISensorCollection
    {
        /// <summary>
        /// Get a sensor by index
        /// </summary>
        /// <param name="ulIndex">The index for the sensor to retrieve</param>
        /// <param name="ppSensor">The sensor retrieved</param>
        void GetAt([In] ULONG ulIndex, [Out] out ISensor ppSensor);

        /// <summary>
        /// Get the sensor count for the collection
        /// </summary>
        /// <param name="pCount">The count returned</param>
        void GetCount([Out] out ULONG pCount);

        /// <summary>
        /// Add a sensor to the collection
        /// </summary>
        /// <param name="pSensor">The sensor to add</param>
        void Add([In, MarshalAs(UnmanagedType.IUnknown)] ISensor pSensor);

        /// <summary>
        /// Remove a sensor from the collection
        /// </summary>
        /// <param name="pSensor">The sensor to remove</param>
        void Remove([In] ISensor pSensor);

        /// <summary>
        /// Remove a sensor from the collection
        /// </summary>
        /// <param name="sensorID">Remove sensor by ID</param>
        void RemoveByID([In, MarshalAs(UnmanagedType.LPStruct)] REFSENSOR_ID sensorID);

        /// <summary>
        /// Clear the collection
        /// </summary>
        void Clear();
    }

    /// <summary>
    /// A COM interop wrapper for the SensorCollection class
    /// </summary>
    [ComImport, Guid("79C43ADB-A429-469F-AA39-2F2B74B75937"), ClassInterface(ClassInterfaceType.None), TypeLibType(TypeLibTypeFlags.FCanCreate)]
    internal class SensorCollection : ISensorCollection
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void GetAt([In] ULONG index, [MarshalAs(UnmanagedType.Interface)] out ISensor ppSensor);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void GetCount(out ULONG pCount);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void Add([MarshalAs(UnmanagedType.Interface)] ISensor pSensor);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void Remove([MarshalAs(UnmanagedType.Interface)] ISensor pSensor);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void RemoveByID([In, MarshalAs(UnmanagedType.LPStruct)] REFSENSOR_ID sensorId);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void Clear();
    }
}