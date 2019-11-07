//***************************************************************************
// Copyright © 2010 Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//***************************************************************************
using System;
using System.Collections.Generic;

namespace PkgDefLanguage
{
    public enum PkgDefLanguageTokens
    {
        Error
       ,Comment
       ,Key
       ,ValueName
       ,ValueString
       ,ValueExpandString
       ,ValueMultiString
       ,ValueQWord
       ,ValueDWord
       ,ValueBinary
       ,ValueType
       ,Token
       ,Guid
    }

    public static class PkgDefTokenStrings
    {
        public const string HexSpecifier = "0x";
        public const string StringExpandSzPrefix = "e:\"";  // include trailing quote when testing
        public const string HexDWordBinaryPrefix = "hex(b):";
        public const string HexExpandSzBinaryPrefix = "hex(2):";
        public const string HexMultiSzPrefix1 = "hex(7):";
        public const string HexMultiSzPrefix2 = "hex(m):";
        public const string HexBinaryPrefix = "hex:";
        public const string BinaryDWordPrefix = "dword:";
        public const string BinaryQWordPrefix = "qword:";
    }
}
