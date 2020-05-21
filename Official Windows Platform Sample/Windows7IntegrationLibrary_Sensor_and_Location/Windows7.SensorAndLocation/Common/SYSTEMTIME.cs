// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Globalization;
using System.Runtime.InteropServices;
using WORD = System.UInt16;



namespace Windows7.Sensors.Internal
{
    /// <summary>
    /// The SYSTEMTIME structure represents a date and time using individual members for 
    /// the month, day, year, weekday, hour, minute, second, and millisecond.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct SYSTEMTIME
    {
        public WORD wYear;
        public WORD wMonth;
        public WORD wDayOfWeek;
        public WORD wDay;
        public WORD wHour;
        public WORD wMinute;
        public WORD wSecond;
        public WORD wMillisecond;

        /// <summary>
        /// Initializes the SYSTEMTIME to the specified DateTime.
        /// </summary>
        public SYSTEMTIME(DateTime value)
        {
            wYear = (WORD)value.Year;
            wMonth = (WORD)value.Month;
            wDayOfWeek = (WORD)value.DayOfWeek;
            wDay = (WORD)value.Day;
            wHour = (WORD)value.Hour;
            wMinute = (WORD)value.Minute;
            wSecond = (WORD)value.Second;
            wMillisecond = (WORD)value.Millisecond;
        }

        /// <summary>
        /// Gets the <see cref="DateTime"/> representation of this object.
        /// </summary>
        public DateTime DateTime
        {
            get { return new DateTime(wYear, wMonth, wDay, wHour, wMinute, wSecond, wMillisecond); }
        }

        public static implicit operator DateTime(SYSTEMTIME systemTime)
        {
            return systemTime.DateTime;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture.NumberFormat, "{0:D2}/{1:D2}/{2:D4}, {3:D2}:{4:D2}:{5:D2}.{6}", wMonth, wDay, wYear, wHour, wMinute, wSecond, wMillisecond);
        }
    }
}