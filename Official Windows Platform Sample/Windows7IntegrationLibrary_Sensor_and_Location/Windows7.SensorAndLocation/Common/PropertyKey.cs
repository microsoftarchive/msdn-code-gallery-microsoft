// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Runtime.InteropServices;

namespace Windows7.Sensors
{
    /// <summary>
    /// Specifies the FMTID/PID identifier that programmatically identifies a property.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 0x4)]
    public struct PropertyKey : IEquatable<PropertyKey>
    {
        /// <summary>
        /// A unique GUID for the property.
        /// </summary>
        public Guid fmtid;
        
        /// <summary>
        /// A property identifier (PID). Any value greater than or equal to 2 is acceptable.
        /// </summary>
        public Int32 pid;

        /// <summary>
        /// Create a new property key from the FMTID (GUID) and PID identifiers.
        /// </summary>
        public static PropertyKey Create(Guid guid, int id)
        {
            PropertyKey key = new PropertyKey();
            key.fmtid = guid;
            key.pid = id;

            return key;
        }

        /// <summary>
        /// Create a new property key from the FMTID (string representation of GUID) and PID identifiers.
        /// </summary>
        public static PropertyKey Create(string guid, int id)
        {
            PropertyKey key = new PropertyKey();
            key.fmtid = new Guid( guid );
            key.pid = id;

            return key;
        }

        /// <summary>
        /// Returns the hash code of the object. This is vital for performance of value types.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return fmtid.GetHashCode() ^ pid;
        }

        /// <summary>
        /// Returns whether this object is equal to another. This is vital for performance of value types.
        /// </summary>
        /// <param name="obj">The object to compare against.</param>
        /// <returns>Equality result.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (!(obj is PropertyKey))
                return false;

            PropertyKey other = (PropertyKey)obj;
            return other.fmtid.Equals(fmtid) && other.pid == pid;
        }

        #region IEquatable<PropertyKey> Members

        /// <summary>
        /// Returns whether this object is equal to another. This is vital for performance of value types.
        /// </summary>
        /// <param name="other">The object to compare against.</param>
        /// <returns>Equality result.</returns>
        public bool Equals(PropertyKey other)
        {
            return other.fmtid.Equals(fmtid) && other.pid == pid;
        }

        #endregion

        /// <summary>
        /// Implements the == (equality) operator.
        /// </summary>
        /// <param name="a">Object a.</param>
        /// <param name="b">Object b.</param>
        /// <returns>true if object a equals object b. false otherwise.</returns>
        public static bool operator==(PropertyKey a, PropertyKey b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Implements the != (inequality) operator.
        /// </summary>
        /// <param name="a">Object a.</param>
        /// <param name="b">Object b.</param>
        /// <returns>true if object a does not equal object b. false otherwise.</returns>
        public static bool operator !=(PropertyKey a, PropertyKey b)
        {
            return !a.Equals(b);
        }
    }
}