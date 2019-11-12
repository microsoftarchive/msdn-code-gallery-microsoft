using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)


namespace WebPartSample
{
	public class ListProperties
	{
        public string ListTitle { get; set; }
        public int ItemCount { get; set; }
        public int FieldCount { get; set; }
        public int FolderCount { get; set; }
        public int ViewCount {get; set; }
        public int WorkFlowCount { get; set; }
	}
}
