//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//  ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//  THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//  PARTICULAR PURPOSE.
//
//  Copyright  1997-2003  Microsoft Corporation.  All Rights Reserved.
//
//  FILE:   TTY.h
//
//  PURPOSE:  Define memory allocation functions.
//

#pragma once

#include <stddef.h>
#include <stdlib.h>
#include <objbase.h>
#include <stdarg.h>
#include <windef.h>
#include <winerror.h>
#include <winbase.h>
#include <wingdi.h>
#include <winddi.h>
#include <tchar.h>
#include <excpt.h>


// Macros for Memory Allocation

#define MemAlloc(size)      ((PVOID) LocalAlloc(LMEM_FIXED, (size)))
#define MemFree(p)          { if (p) LocalFree((HLOCAL) (p)); }


