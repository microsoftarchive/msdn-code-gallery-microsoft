// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

// logfont.cs
// compile with: /target:module
using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public class LOGFONT 
{ 
    public const int LF_FACESIZE = 32;
    public int lfHeight; 
    public int lfWidth; 
    public int lfEscapement; 
    public int lfOrientation; 
    public int lfWeight; 
    public byte lfItalic; 
    public byte lfUnderline; 
    public byte lfStrikeOut; 
    public byte lfCharSet; 
    public byte lfOutPrecision; 
    public byte lfClipPrecision; 
    public byte lfQuality; 
    public byte lfPitchAndFamily;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst=LF_FACESIZE)]
    public string lfFaceName; 
}

