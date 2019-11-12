/****************************** Module Header ******************************\
Module Name:  UnpivotRow.cs
Project:      CSEFPivotOperation
Copyright (c) Microsoft Corporation.

This sample demonstrates how to implement the Pivot and Unpivot operation in 
Entity Framework.
This file includes the definition of UnpivotRow class that stores the Unpivot 
result.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;

namespace CSEFPivotOperation
{
    /// <summary>
    /// Store the row of the Unpivot result.
    /// </summary>
    /// <typeparam name="TypeId">type of ObjectId</typeparam>
    /// <typeparam name="TypeAttr">type of Attribute</typeparam>
    /// <typeparam name="TypeValue">type of Value</typeparam>
    public class UnpivotRow<TypeId, TypeAttr, TypeValue>
    {
        public TypeId ObjectId { get; set; }
        public TypeAttr Attribute { get; set; }
        public TypeValue Value { get; set; }
    }
}
