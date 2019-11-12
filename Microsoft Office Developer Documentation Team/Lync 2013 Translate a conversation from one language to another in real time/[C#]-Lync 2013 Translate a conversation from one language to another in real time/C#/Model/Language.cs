/*=====================================================================
  This file is part of the Microsoft Unified Communications Code Samples.

  Copyright (C) 2012 Microsoft Corporation.  All rights reserved.

This source code is intended only as a supplement to Microsoft
Development Tools and/or on-line documentation.  See these other
materials for detailed information regarding Microsoft code samples.

THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
PARTICULAR PURPOSE.
=====================================================================*/

using System;
using System.Runtime.Serialization;

namespace Microsoft.Lync.SDK.Samples.ConversationTranslator
{
    /// <summary>
    /// Describes a language available for translation.
    /// </summary>
    [DataContract]
    public class Language
    {
        public static Language DefaultSource = new Language("en", "English");
        public static Language DefaultTarget = new Language("es", "Spanish");

        /// <summary>
        /// Locale of the language. Examples: en, pt.
        /// </summary>
        [DataMember]
        public string Code { get; set; }

        /// <summary>
        /// Description of the language. Examples: English, Portuguese.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Language(string theCode, string theName)
        {
            this.Code = theCode;
            this.Name = theName;
        }

        /// <summary>
        /// Comparison uses the Code property only.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Language)
            {
                return ((Language)obj).Code == this.Code;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Hashcode of the Code property is used.
        /// </summary>
        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }
    }
}
