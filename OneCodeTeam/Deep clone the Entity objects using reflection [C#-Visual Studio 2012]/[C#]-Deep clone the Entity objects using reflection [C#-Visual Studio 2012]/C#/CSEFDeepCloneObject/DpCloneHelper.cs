/********************************* Module Header **********************************\
* Module Name:	DpCloneHelper.cs
* Project:		CSEFDeepCloneObject
* Copyright (c) Microsoft Corporation.
* 
* This sample demonstrates how to implement deep clone/duplicate entity objects 
* using serialization and reflection.
* This module defines some extension methods to implement the deep clone.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
* **********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace CSEFDeepCloneObject
{
    public static class DpClonehelper
    {
        /// <summary>
        /// Implement deep clone the object using serialization.
        /// </summary>
        /// <param name="source">Entity Object needs to be cloned </param>
        /// <returns>The Cloned object</returns>
        public static T Clone<T>(this T source) where T : EntityObject
        {
            var ser = new DataContractSerializer(typeof(T));
            using (var stream = new MemoryStream())
            {
                ser.WriteObject(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)ser.ReadObject(stream);
            }
        }

        /// <summary>
        ///  Clear the entity key of the object and all related child objects 
        /// </summary>
        /// <param name="source">Entity Object needs to be cleared</param>
        /// <param name="bCheckHierarchy">
        ///  Determine whether to clear all the child object
        /// </param>
        /// <returns></returns>
        public static EntityObject ClearEntityReference(this EntityObject source,
            bool bCheckHierarchy)
        {
            return source.ClearEntityObject(bCheckHierarchy);
        }

        /// <summary>
        ///  Clear the entity of object and all related child objects 
        /// </summary>
        /// <param name="source">Entity Object needs to be cleared</param>
        /// <param name="bCheckHierarchy">
        ///  Determine whether to clear all the child object
        /// </param>
        /// <returns></returns>
        private static T ClearEntityObject<T>(this  T source, bool bCheckHierarchy)
            where T : class
        {
            // Throw if passed object is null
            if (source == null)
            {
                throw new Exception("Null Object cannot be cloned");
            }

            // Get the Type of passed object 
            Type tObj = source.GetType();

            // Check passed object's entity key Attribute 
            if (tObj.GetProperty("EntityKey") != null)
            {
                tObj.GetProperty("EntityKey").SetValue(source, null, null);
            }

            // bCheckHierarchy is used to check and clear child object releation keys 
            if (!bCheckHierarchy)
            {
                return source;
            }

            // Clearing the Entity's related Child Objects 
            List<PropertyInfo> propertyList = (from a in source.GetType().GetProperties()
                                               where a.PropertyType.Name.Equals
                                               ("ENTITYCOLLECTION`1",
                                               StringComparison.OrdinalIgnoreCase)
                                               select a).ToList();

            // Loop the list of Child Object and Clear the Entity Reference 
            foreach (PropertyInfo prop in propertyList)
            {
                var keys = (IEnumerable)tObj.GetProperty(prop.Name)
                    .GetValue(source, null);

                foreach (object key in keys)
                {
                    // Clearing Entity Reference from Parent Object 
                    var childProp = (from a in key.GetType().GetProperties()
                                     where a.PropertyType.Name.Equals
                                     ("EntityReference`1",
                                     StringComparison.OrdinalIgnoreCase)
                                     select a).SingleOrDefault();

                    if (childProp != null)
                    {
                        childProp.GetValue(key, null).ClearEntityObject(false);
                    }

                    // Recursively clearing the the Entity Reference from Child object 
                    key.ClearEntityObject(true);
                }
            }
            return source;
        }
    }
}
