/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

#pragma once

// Windows Platform headers and control defines
#define STRICT
#ifndef _WIN32_WINNT
#define _WIN32_WINNT 0x0500 // Visual Studio requires Windows 2000 or better
#endif
#define NOMINMAX // Windows Platform min and max macros cause problems for the Standard C++ Library
#define WIN32_LEAN_AND_MEAN	// Exclude rarely-used stuff from the Windows Platform headers

// ATL headers and control defines
#define _ATL_APARTMENT_THREADED

#include <atlbase.h>
#include <atlcom.h>
#include <atlstr.h>

// Visual Studio Platform headers
#include <proffserv.h>

// VSL headers
#define VSLASSERT _ASSERTE
#define VSLASSERTEX(exp, szMsg) _ASSERT_BASE(exp, szMsg)
#define VSLTRACE ATLTRACE

#include <VSLPackage.h>
#include <VSLCommandTarget.h>
#include <VSLShortNameDefines.h>

using namespace VSL;

