/***************************** Module Header ******************************\
* Module Name:	ExcelTableEntity.cs
* Project:		CSAZureBulkImportExportExeclTableStorage
* Copyright (c) Microsoft Corporation.
* 
* This sample shows how to define properties at the run time which will be 
* added to the table when inserting the entities.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\**************************************************************************/

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSAZureBulkImportExportExcelTableStorage
{
    public class ExcelTableEntity  : ITableEntity
    {
        #region ITableEntity properties
        // Summary:
        //     Gets or sets the entity's current ETag. Set this value to '*' in order to
        //     blindly overwrite an entity as part of an update operation.
        public string ETag { get; set; }
        //
        // Summary:
        //     Gets or sets the entity's partition key.
        public string PartitionKey { get; set; }
        //
        // Summary:
        //     Gets or sets the entity's row key.
        public string RowKey { get; set; }
        //
        // Summary:
        //     Gets or sets the entity's time stamp.
        public DateTimeOffset Timestamp { get; set; }
        #endregion

        // Use this Dictionary store table's properties. 
        public IDictionary<string, EntityProperty> properties { get; private set; }

        public ExcelTableEntity()
        {
            properties=new Dictionary<string,EntityProperty>();
        }

        public ExcelTableEntity(string PartitionKey, string RowKey)
        {
            this.PartitionKey = PartitionKey;
            this.RowKey = RowKey;
            properties = new Dictionary<string, EntityProperty>();
        }
	
	    #region ITableEntity implementation
	
	    public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
	    {
		    this.properties = properties;
	    }
 
	    public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
	    {
		    return this.properties;
	    }
	
	    #endregion
	
	    /// <summary>
        /// Convert object value to EntityProperty.
	    /// </summary>
        public EntityProperty ConvertToEntityProperty(string key, object value)
	    {
            if (value == null) return new EntityProperty((string)null);
		    if (value.GetType() == typeof(byte[])) 
                return new EntityProperty((byte[])value);
		    if (value.GetType() == typeof(bool)) 
                return new EntityProperty((bool)value);
		    if (value.GetType() == typeof(DateTimeOffset)) 
                return new EntityProperty((DateTimeOffset)value);
		    if (value.GetType() == typeof(DateTime)) 
                return new EntityProperty((DateTime)value);
		    if (value.GetType() == typeof(double)) 
                return new EntityProperty((double)value);
		    if (value.GetType() == typeof(Guid)) 
                return new EntityProperty((Guid)value);
		    if (value.GetType() == typeof(int)) 
                return new EntityProperty((int)value);
		    if (value.GetType() == typeof(long)) 
                return new EntityProperty((long)value);
		    if (value.GetType() == typeof(string)) 
                return new EntityProperty((string)value);
		    throw new Exception("This value type" + value.GetType() + " for " + key);
            throw new Exception(string.Format("This value type {0} is not supported for {1}",key));
	    }
       
         /// <summary>
         /// Get the edm type, if the type is not a edm type throw a exception.
         /// </summary>
        public Type GetType(EdmType edmType)
	    {
		    switch (edmType)
		    {
			    case EdmType.Binary : 
                    return typeof(byte[]);
			    case EdmType.Boolean : 
                    return typeof(bool);
			    case EdmType.DateTime : 
                    return typeof(DateTime);
			    case EdmType.Double : 
                    return typeof(double);
			    case EdmType.Guid : 
                    return typeof(Guid);
			    case EdmType.Int32 : 
                    return typeof(int);
			    case EdmType.Int64 : 
                    return typeof(long);
			    case EdmType.String : 
                    return typeof(string);
			    default: throw new TypeLoadException(string.Format("not supported edmType:{0}" ,edmType));
		    }
	    }
        }
}






 

 
