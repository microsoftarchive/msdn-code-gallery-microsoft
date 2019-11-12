//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

//
// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once
#include "targetver.h"

// Exclude rarely-used stuff from Windows headers
#define WIN32_LEAN_AND_MEAN

// Better type saftey for Shell IDLists 
// For more info refer to http://msdn.microsoft.com/en-us/library/bb773321(VS.85).aspx
#define STRICT_TYPED_ITEMIDS

// Don't use min and max macros, we'll use std functions instead
#define NOMINMAX

// Windows Headers
#include <Windows.h>
#include <UIAnimation.h>
#include <commctrl.h>

// Shell
#include <ShellAPI.h>
#include <ShlObj.h>
#include <StructuredQuery.h>
#include <PropKey.h>

// Graphics
#include <D2d1.h>
#include <DWrite.h>
#include <WinCodec.h>

// Ribbon
#include <UIRibbon.h>
#include <UIRibbonPropertyHelpers.h>

// C RunTime Header Files
#include <assert.h>
#include <tchar.h>

// Standrd library declarations
#include <algorithm>
#include <fstream>
#include <queue>
#include <sstream>
#include <stack>
#include <string>
#include <vector>

// Commonly used headers
#include "ComPtr.h"
#include "SharedObject.h"
#include "ComHelpers.h"

#include "AnimationUtility.h"
#include "ImageEditorInterface.h"
#include "Direct2DUtility.h"
#include "ImageOperation.h"
#include "Window.h"
#include "WindowMessageHandler.h"
#include "WindowFactory.h"

// Useful macros
#ifndef HINST_THISCOMPONENT
EXTERN_C IMAGE_DOS_HEADER __ImageBase;
#define HINST_THISCOMPONENT ((HINSTANCE)&__ImageBase)
#endif

#ifndef IDC_PEN
#define IDC_PEN MAKEINTRESOURCE(32631)
#endif

// Common constants
const double PI = 3.14159265358979323846;

// Use the correct version of the common control library based on the currently selected CPU architecture
// This is needed in order to use TaskDialog, since TaskDialog requires version 6.0 of Comctl32.dll
#if defined _M_IX86
#pragma comment(linker, "/manifestdependency:\"type='win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='x86' publicKeyToken='6595b64144ccf1df' language='*'\"")
#elif defined _M_X64
#pragma comment(linker, "/manifestdependency:\"type='win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='amd64' publicKeyToken='6595b64144ccf1df' language='*'\"")
#else
#pragma comment(linker, "/manifestdependency:\"type='win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='*' publicKeyToken='6595b64144ccf1df' language='*'\"")
#endif

