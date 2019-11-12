// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;
using System;

namespace BdcSampleCSharp
{
	public partial class CustomerDataContext
	{
        public CustomerDataContext() :
            base(global::BdcSampleCSharp.Settings.Default.NORTHWNDConnectionString, mappingSource)
        {
            OnCreated();
        }
	}
}
