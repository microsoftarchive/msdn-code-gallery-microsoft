// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using Windows7.Location.Internal;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Windows7.Location
{
    /// <summary>
    /// A base class for event providers, e.g. <see cref="LatLongLocationProvider"/> and <see cref="CivicAddressLocationProvider"/>.
    /// </summary>
    public abstract class LocationProvider : ILocationEvents
    {
        #region Private fields

        private static ILocation _location = new COMLocation();
        private Guid _reportGuid;

        #endregion
        
        #region Events

        /// <summary>
        /// Event notifying about location changes.
        /// </summary>
        public event LocationChangedEventHandler LocationChanged;
        
        /// <summary>
        /// Event notifying about provider status changes.
        /// </summary>
        public event LocationProviderStatusChangedEventHandler StatusChanged;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs the LocationProvider instance with the specified minimal report interval.
        /// </summary>
        protected LocationProvider(uint minReportInterval)
        {
            Type t = GetType();
            object[] attrs = t.GetCustomAttributes(typeof(LocationProviderDescriptionAttribute), true);

            if (attrs == null || attrs.Length == 0)
                throw new NotSupportedException(String.Format("Report {0} does not have the LocationReportDescription attribute", t.Name));

            LocationProviderDescriptionAttribute attr = (LocationProviderDescriptionAttribute)attrs[0];
            _reportGuid = attr.ReportTypeGuid;

            _location.RegisterForReport(this, ref _reportGuid, minReportInterval);
        }

        #endregion

        #region Public methods and properties

        /// <summary>
        /// Synchronously returns a location report. Cast it to the appropriate derived type.
        /// </summary>
        /// <returns>Report wrapper.</returns>
        public LocationReport GetReport()
        {
            ILocationReport iLocReport = null;
            _location.GetReport(ref _reportGuid, out iLocReport);
            return CreateReport(iLocReport);
        }

        /// <summary>
        /// Returns the current provider status.
        /// </summary>
        public ReportStatus ReportStatus
        {
            get
            {
                ReportStatus status;
                int hr = _location.GetReportStatus(ref _reportGuid, out status);
                if (hr != 0)
                    throw new COMException("Error getting report status.", hr);

                return status;
            }
        }

        /// <summary>
        /// Gets or sets the report interval, in milliseconds.
        /// </summary>
        /// <remarks>
        /// You can only get/set the report interval after registering for the <see cref="LocationProvider.LocationChanged"/> event.
        /// </remarks>
        public uint ReportInterval
        {
            get
            {
                uint reportInterval = 0;
                _location.GetReportInterval(ref _reportGuid, out reportInterval);
                return reportInterval;
            }
            set { _location.SetReportInterval(ref _reportGuid, value); }
        }

        /// <summary>
        /// Gets desired report accuracy.  
        /// The desired accuracy is a global setting that affects all providers.
        /// </summary>
        public static DesiredAccuracy GetDesiredAccuracy(Guid reportType)
        {
            DesiredAccuracy da = DesiredAccuracy.Default;
            if (_location != null)
            {
                _location.GetDesiredAccuracy(ref reportType, out da);
            }
            return da;
        }

        /// <summary>
        /// Sets desired report accuracy.  
        /// The desired accuracy is a global setting that affects all providers.
        /// </summary>
        public static void SetDesiredAccuracy(Guid reportType, DesiredAccuracy accuracy)
        {
            if (_location != null)
            {
                _location.SetDesiredAccuracy(ref reportType, accuracy);
            }
        }

        /// <summary>
        /// Requests the user permission to access the given report types/location providers.
        /// </summary>
        /// <param name="hWndParent">HWND handle to a window that can act as a parent to the permissions dialog box.</param>
        /// <param name="modal">Speficifies whether the window should be modal.</param>
        /// <param name="providers">The report types for which to request permission.</param>
        public static void RequestPermissions(IntPtr hWndParent, bool modal, params LocationProvider[] providers)
        {
            if (providers == null || providers.Length == 0)
                throw new ArgumentNullException("providers", "Reports must not be null or empty.");

            Guid[] guids = new Guid[providers.Length];
            for (int i = 0; i < providers.Length; i++)
                guids[i] = providers[i].ReportGuid;

            RequestPermissions(hWndParent, modal, guids);
        }

        /// Requests the user permission to access the given report types.
        /// </summary>
        /// <param name="hWndParent">HWND handle to a window that can act as a parent to the permissions dialog box.</param>
        /// <param name="modal">Speficifies whether the window should be modal.</param>
        /// <param name="reportGuids">The report types for which to request permission.</param>
        public static void RequestPermissions(IntPtr hWndParent, bool modal, params Guid[] reportGuids)
        {
            if (reportGuids == null || reportGuids.Length == 0)
                throw new ArgumentNullException("reportGuids", "Reports GUIDs must not be null or empty.");

            int hr = _location.RequestPermissions(hWndParent, reportGuids, (uint) reportGuids.Length, modal ? 1 : 0);
            if (hr != 0)
                throw new COMException("Error requesting permissions.", hr);
        }

        #endregion

        #region Protected methods

        protected abstract LocationReport CreateReport();

        #endregion

        #region Internal event handlers

        int ILocationEvents.OnLocationChanged(ref Guid reportType, ILocationReport pLocationReport)
        {
            if (LocationChanged != null && reportType.Equals(_reportGuid))
            {
                LocationReport report = CreateReport(pLocationReport);
                LocationChanged(this, report);
            }

            return 0;
        }

        int ILocationEvents.OnStatusChanged(ref Guid reportType, ReportStatus newStatus)
        {
            if (StatusChanged != null && reportType.Equals(_reportGuid))
                StatusChanged(this, newStatus);

            return 0;
        }

        #endregion

        #region Internal methods and properties

        /// <summary>
        /// Returns the report GUID (information provided by the attribute).
        /// </summary>
        internal Guid ReportGuid
        {
            get { return _reportGuid; }
        }

        /// <summary>
        /// Creates a wrapper report instance for the given report COM object.
        /// </summary>
        /// <param name="iLocReport">Report COM object.</param>
        /// <returns>Report wrapper.</returns>
        private LocationReport CreateReport(ILocationReport iLocReport)
        {
            LocationReport locReport = CreateReport();
            locReport.InitializeReport(iLocReport);
            return locReport;
        }

        #endregion

    }
}