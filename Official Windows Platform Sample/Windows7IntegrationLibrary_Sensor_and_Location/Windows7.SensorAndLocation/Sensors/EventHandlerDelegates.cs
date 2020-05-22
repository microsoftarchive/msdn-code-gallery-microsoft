// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;



namespace Windows7.Sensors
{
    /// <summary>
    /// Handler for sensor attachment/availability events.
    /// </summary>
    /// <param name="sensor">Sensor for which this event applies.</param>
    /// <param name="state">New state of the sensor.</param>
    public delegate void SensorEnterEventHandler(Sensor sensor, SensorState state);
    
    /// <summary>
    /// Handler of sensor detachment/cessation of availability events.
    /// </summary>
    /// <param name="sensor">Sensor for which this event applies.</param>
    /// <param name="sensorID">Sensor instance ID.</param>
    /// <remarks>
    /// Do not use any sensor property.
    /// </remarks>
    public delegate void SensorLeaveEventHandler(Sensor sensor, Guid sensorID);
    
    /// <summary>
    /// Handler of sensor state changes events.
    /// </summary>
    /// <param name="sensor">Sensor for which this event applies.</param>
    /// <param name="state">New state of the sensor.</param>
    public delegate void SensorStateChangedEventHandler(Sensor sensor, SensorState state);
    
    /// <summary>
    /// Handler of new data event.
    /// </summary>
    /// <param name="sensor">Sensor for which this event applies.</param>
    /// <param name="dataReport">Data report for the sensor. You should cast it to a appropriate derived type.</param>
    public delegate void SensorDataUpdatedEventHandler(Sensor sensor, SensorDataReport dataReport);
    
    /// <summary>
    /// Handler for a generic/custom event.
    /// </summary>
    /// <param name="sensor">Sensor for which this event applies.</param>
    /// <param name="eventID">Event GUID.</param>
    /// <param name="dataReport">Data report for the sensor. You should cast it to a appropriate derived type.</param>
    public delegate void SensorEventHandler(Sensor sensor, Guid eventID, SensorDataReport dataReport);  
}