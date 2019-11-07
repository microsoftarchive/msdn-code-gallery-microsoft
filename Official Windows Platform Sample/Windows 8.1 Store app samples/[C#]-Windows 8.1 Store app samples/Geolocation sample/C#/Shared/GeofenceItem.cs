//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;

namespace Microsoft.Samples.Devices.Geolocation
{
    // GeofenceItem implements IEquatable to allow
    // removal of objects in the collection
    public class GeofenceItem : IEquatable<GeofenceItem>
    {
        private Geofence geofence;
        private string id;

        public GeofenceItem(Geofence geofence)
        {
            this.geofence = geofence;
            this.id = geofence.Id;
        }

        public bool Equals(GeofenceItem other)
        {
            bool isEqual = false;
            if (Id == other.Id)
            {
                isEqual = true;
            }

            return isEqual;
        }

        public Windows.Devices.Geolocation.Geofencing.Geofence Geofence
        {
            get
            {
                return geofence;
            }
        }

        public string Id
        {
            get
            {
                return id;
            }
        }

        public double Latitude
        {
            get
            {
                Geocircle circle = geofence.Geoshape as Geocircle;
                return circle.Center.Latitude;
            }
        }

        public double Longitude
        {
            get
            {
                Geocircle circle = geofence.Geoshape as Geocircle;
                return circle.Center.Longitude;
            }
        }

        public double Radius
        {
            get
            {
                Geocircle circle = geofence.Geoshape as Geocircle;
                return circle.Radius;
            }
        }

        public bool SingleUse
        {
            get
            {
                return geofence.SingleUse;
            }
        }

        public MonitoredGeofenceStates MonitoredStates
        {
            get
            {
                return geofence.MonitoredStates;
            }
        }

        public TimeSpan DwellTime
        {
            get
            {
                return geofence.DwellTime;
            }
        }

        public DateTimeOffset StartTime
        {
            get
            {
                return geofence.StartTime;
            }
        }

        public TimeSpan Duration
        {
            get
            {
                return geofence.Duration;
            }
        }
    }
}
