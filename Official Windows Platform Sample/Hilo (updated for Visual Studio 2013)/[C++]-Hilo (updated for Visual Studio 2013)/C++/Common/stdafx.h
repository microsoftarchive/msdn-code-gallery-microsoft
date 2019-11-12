//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once

#include "targetver.h"

#define WIN32_LEAN_AND_MEAN  // Exclude rarely-used stuff from Windows headers

// Windows Headers
#include <Windows.h>
#include <WinBase.h>
#include <Unknwn.h>
#include <wincodecsdk.h>

// Shell
#include <ShlObj.h>

// DirectX
#include <D2d1.h>
#include <DWrite.h>
#include <WinCodec.h>

// C RunTime Header Files
#include <assert.h>

// Standard library declarations
#include <algorithm>
#include <string>
#include <cctype>

// Commonly used headers
#include "ComPtr.h"
#include "SharedObject.h"
#include "ComHelpers.h"
#include "Window.h"
#include "WindowMessageHandler.h"
#include "WindowMessageHandler.h"
#include "WindowLayout.h"
#include "WindowFactory.h"
#include "Direct2DUtility.h"
#include "AnimationUtility.h"

// Usefule macros
#ifndef HINST_THISCOMPONENT
EXTERN_C IMAGE_DOS_HEADER __ImageBase;
#define HINST_THISCOMPONENT ((HINSTANCE)&__ImageBase)
#endif
