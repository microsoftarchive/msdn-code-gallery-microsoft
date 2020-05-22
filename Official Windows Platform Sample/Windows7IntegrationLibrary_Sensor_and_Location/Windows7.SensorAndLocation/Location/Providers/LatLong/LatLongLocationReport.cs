// Copyright (c) Microsoft Corporation.  All rights reserved.

using Windows7.Location.Internal;

namespace Windows7.Location
{   
    /// <summary>
    /// Represents a latitude/longitude/altitude report.
    /// </summary>
    public class LatLongLocationReport : LocationReport
    {
        private ILatLongReport _latLongReport;

        public LatLongLocationReport()
        {

        }

        /// <summary>
        /// Initializes the position report to the specified values.  Use to set the default location.
        /// </summary>
        /// <param name="latitude">The latitude to report.</param>
        /// <param name="longitude">The longitude to report.</param>
        /// <param name="errorRadius">The error radius to report.</param>
        /// <param name="altitude">The altitude to report.</param>
        /// <param name="altitudeError">The altitude error to report.</param>
        public LatLongLocationReport(double latitude, double longitude, double errorRadius, double altitude, double altitudeError)
        {
            DefaultLatLongReport report = new DefaultLatLongReport(latitude, longitude, errorRadius, altitude, altitudeError);
            InitializeReport(report);
        }

        /// <summary>
        /// Initialized the instance.
        /// </summary>
        protected override void Initialize()
        {
            _latLongReport = (ILatLongReport)InnerObject;
        }
        
        /// <summary>
        /// Latitude.
        /// </summary>
        public double Latitude
        {
            get { return _latLongReport.GetLatitude(); }
        }

        /// <summary>
        /// Longitude.
        /// </summary>
        public double Longitude
        {
            get { return _latLongReport.GetLongitude(); }
        }

        /// <summary>
        /// Altitude in meters.
        /// </summary>
        public double Altitude
        {
            get { return _latLongReport.GetAltitude(); }
        }

        /// <summary>
        /// Error radius.
        /// </summary>
        public double ErrorRadius
        {
            get { return _latLongReport.GetErrorRadius(); }
        }

        /// <summary>
        /// Altitude error in meters.
        /// </summary>
        public double AltitudeError
        {
            get { return _latLongReport.GetAltitudeError(); }
        }
    }
}