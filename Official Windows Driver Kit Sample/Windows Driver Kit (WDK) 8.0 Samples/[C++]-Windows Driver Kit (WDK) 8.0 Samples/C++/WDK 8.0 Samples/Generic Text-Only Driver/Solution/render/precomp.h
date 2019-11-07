
//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//  ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//  THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//  PARTICULAR PURPOSE.
//
//  Copyright  1997-2003  Microsoft Corporation.  All Rights Reserved.
//
//  FILE:	precomp.H
//    
//  PURPOSE:	Optimize compilation of headers that don't change often.
//

#pragma once

// Necessary for compiling under VC.
// but disabled in the builds for IA 64
//#define WINVER          0x0500
//#define _WIN32_WINNT    0x0500


// Required header files that shouldn't change often.

#include <STDDEF.H>
#include <STDLIB.H>
#include <OBJBASE.H>
#include <STDARG.H>
#include <STDIO.H>
#include <WINDEF.H>
#include <WINERROR.H>
#include <WINBASE.H>
#include <WINGDI.H>
#include <WINDDI.H>
#include <TCHAR.H>
#include <EXCPT.H>
#include <ASSERT.H>
#include <PRINTOEM.H>
