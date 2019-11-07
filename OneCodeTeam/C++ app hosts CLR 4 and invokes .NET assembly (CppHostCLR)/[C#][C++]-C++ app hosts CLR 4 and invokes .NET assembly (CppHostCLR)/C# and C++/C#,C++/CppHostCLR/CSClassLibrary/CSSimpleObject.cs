/****************************** Module Header ******************************\
 * Module Name:  CSSimpleObject.cs
 * Project:      CSClassLibrary
 * Copyright (c) Microsoft Corporation.
 * 
 * The code sample demonstrates a C# class library that we can use in other 
 * applications. The class library exposes a simple class named CSSimpleObject. 
 * The process of creating the class library is very straight-forward.
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#region Using directives
using System;
using System.Text;
#endregion


namespace CSClassLibrary
{
    public class CSSimpleObject
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CSSimpleObject()
        {
        }

        private float fField = 0F;

        /// <summary>
        /// This is a public Property. It allows you to get and set the value 
        /// of a float field.
        /// </summary>
        public float FloatProperty
        {
            get { return fField; }
            set
            {
                // Fire the event FloatPropertyChanging
                bool cancel = false;
                if (FloatPropertyChanging != null)
                {
                    FloatPropertyChanging(value, out cancel);
                }

                // If the change is not canceled, make the change.
                if (!cancel)
                {
                    fField = value;
                }
            }
        }

        /// <summary>
        /// Returns a String that represents the current Object. Here, we 
        /// return the string form of the float field fField.
        /// </summary>
        /// <returns>the string form of the float field fField.</returns>
        public override string ToString()
        {
            return this.fField.ToString("F2");
        }

        /// <summary>
        /// This is a public static method. It returns the number of 
        /// characters in a string.
        /// </summary>
        /// <param name="str">a string</param>
        /// <returns>the number of characters in the string</returns>
        public static int GetStringLength(string str)
        {
            return str.Length;
        }

        /// <summary>
        /// This is an event. The event is fired when the float property is 
        /// set.
        /// </summary>
        public event PropertyChangingEventHandler FloatPropertyChanging; 
    }


    /// <summary>
    /// Property value changing event handler
    /// </summary>
    /// <param name="NewValue">the new value of the property</param>
    /// <param name="Cancel">
    /// Output whether the change should be cancelled or not.
    /// </param>
    public delegate void PropertyChangingEventHandler(object NewValue, out bool Cancel);
}