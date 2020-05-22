// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using Windows7.Location.Internal;

namespace Windows7.Location
{
    /// <summary>
    /// A base class for default event providers, e.g. <see cref="DefaultLatLongLocationProvider"/> and <see cref="DefaultCivicAddressLocationProvider"/>.
    /// </summary>
    public abstract class DefaultLocationProvider
    {
        #region Private fields

        private static IDefaultLocation _defaultLocation = new COMDefaultLocation();
        private Guid _reportGuid;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs the DefaultLocationProvider instance.
        /// </summary>
        protected DefaultLocationProvider()
        {
            Type t = GetType();
            object[] attrs = t.GetCustomAttributes(typeof(LocationProviderDescriptionAttribute), true);

            if (attrs == null || attrs.Length == 0)
                throw new NotSupportedException(String.Format("Report {0} does not have the LocationReportDescription attribute", t.Name));

            LocationProviderDescriptionAttribute attr = (LocationProviderDescriptionAttribute)attrs[0];
            _reportGuid = attr.ReportTypeGuid;
        }

        #endregion

        #region Public methods and properties

        /// <summary>
        /// Synchronously returns a location report. Cast it to the appropriate derived type.
        /// </summary>
        /// <returns>Report wrapper.</returns>
        public LocationReport GetReport()
        {
            ILocationReport iLocReport = _defaultLocation.GetReport(ref _reportGuid);
            return CreateReport(iLocReport);
        }

        public void SetReport(LocationReport report)
        {
            _defaultLocation.SetReport(ref _reportGuid, report.InnerObject);
        }

        #endregion

        #region Protected methods

        protected abstract LocationReport CreateReport();

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