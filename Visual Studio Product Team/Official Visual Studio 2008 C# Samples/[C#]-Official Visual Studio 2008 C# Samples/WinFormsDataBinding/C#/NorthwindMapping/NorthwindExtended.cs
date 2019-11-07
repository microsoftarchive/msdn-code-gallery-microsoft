//Copyright (C) Microsoft Corporation.  All rights reserved.
namespace NorthwindMapping
{
    using System;

    public partial class Employee
    {
        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0} {1} {2}", this._TitleOfCourtesy, this._FirstName, this._LastName);
        }
    }
}
