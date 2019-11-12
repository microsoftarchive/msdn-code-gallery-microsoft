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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using Windows.UI.Xaml;

using Geofencing4SqSampleTasks;

namespace Geofencing4SqSample
{
    /// <summary>
    /// Used to display a geofence
    /// </summary>
    public class GeofenceItem
    {
        private Geofence _geofence;
        private double _latitude;
        private double _longitude;
        private double _radiusInMeters;
        private Venue _venue;
        private IList<Geofence> _geofenceCollection;

        public GeofenceItem(Geofence geofence, IList<Geofence> geofenceCollection) : 
            this(geofence, null, geofenceCollection)
        {
        }

        public GeofenceItem(Geofence geofence, Venue venue, IList<Geofence> geofenceCollection)
        {
            _geofence = geofence;
            var circle = _geofence.Geoshape as Geocircle;
            _latitude = circle.Center.Latitude;
            _longitude = circle.Center.Longitude;
            _radiusInMeters = circle.Radius;
            _venue = venue;
            _geofenceCollection = geofenceCollection;
        }

        public override string ToString() { return _geofence.Id; }

        public Geofence Geofence
        {
            get
            {
                return _geofence;
            }
        }

        public IList<Geofence> GeofenceCollection
        {
            get
            {
                return _geofenceCollection;
            }
        }

        #region Databinding fields

        public string Title
        {
            get
            {
                return (null != _venue) ? _venue.Name : _geofence.Id;
            }
        }

        public string Subtitle1
        {
            get
            {
                return Logger.FormatLatLong(_latitude, _longitude);
            }
        }

        public string Subtitle2
        {
            get
            {
                return _radiusInMeters + " meter radius";
            }
        }

        public string LinkUrl
        {
            get
            {
                return null;
            }
        }

        public Visibility LinkVisibility
        {
            get
            {
                return String.IsNullOrEmpty(LinkUrl) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        #endregion Databinding fields
    }
}
