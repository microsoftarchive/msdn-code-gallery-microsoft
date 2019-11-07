/*++

Copyright (C) Microsoft Corporation, All Rights Reserved

Module Name:

	Internal.h

Abstract:

	This module contains the local type definitions for the UMDF Skeleton
	driver sample.

Environment:

	Windows User-Mode Driver Framework (WUDF)

--*/

#pragma once

#ifndef ARRAY_SIZE
#define ARRAY_SIZE(x) (sizeof(x) / sizeof(x[0]))
#endif

#include "windows.h"

_Analysis_mode_(_Analysis_code_type_user_driver_);  // Macro letting the compiler know this is not a kernel driver (this will help surpress needless warnings)

#include <devioctl.h>

//
// Include the WUDF Headers
//

#include "wudfddi.h"

//
// Use specstrings for in/out annotation of function parameters.
//

#include "specstrings.h"

//
// Get limits on common data types (ULONG_MAX for example)
//

#include "limits.h"

//
// Headers for hid specific defines
//

#include "hidusage.h"
extern "C" 
{
#include "hidsdi.h"
}
#include "hidclass.h"

//
// Include the header shared between the drivers and the test applications.
//

#include "public.h"

//
// Include the header shared between the drivers and the test applications.
//

#include "WUDFOsrUsbPublic.h"

//
// Forward definitions of classes in the other header files.
//

typedef class CMyDriver *PCMyDriver;
typedef class CMyDevice *PCMyDevice;
typedef class CMyQueue  *PCMyQueue;

typedef class CMyControlQueue   *PCMyControlQueue;
typedef class CMyReadWriteQueue *PCMyReadWriteQueue;

//
// Define the tracing flags.
//
// TODO: Choose a different trace control GUID
//

#define WPP_CONTROL_GUIDS												\
	WPP_DEFINE_CONTROL_GUID(											\
		WudfFx2HidTraceGuid, (5E27A0B4,FD7A,43c0,B1B8,721D52AE6B84),	\
																		\
		WPP_DEFINE_BIT(MYDRIVER_ALL_INFO)								\
		WPP_DEFINE_BIT(TEST_TRACE_DRIVER)								\
		WPP_DEFINE_BIT(TEST_TRACE_DEVICE)								\
		WPP_DEFINE_BIT(TEST_TRACE_QUEUE)								\
		)							 

#define WPP_FLAG_LEVEL_LOGGER(flag, level)								\
	WPP_LEVEL_LOGGER(flag)

#define WPP_FLAG_LEVEL_ENABLED(flag, level)								\
	(WPP_LEVEL_ENABLED(flag) &&											\
	 WPP_CONTROL(WPP_BIT_ ## flag).Level >= level)

#define WPP_LEVEL_FLAGS_LOGGER(lvl,flags) \
		   WPP_LEVEL_LOGGER(flags)
			   
#define WPP_LEVEL_FLAGS_ENABLED(lvl, flags) \
		   (WPP_LEVEL_ENABLED(flags) && WPP_CONTROL(WPP_BIT_ ## flags).Level >= lvl)

//
// This comment block is scanned by the trace preprocessor to define our
// Trace function.
//
// begin_wpp config
// FUNC Trace{FLAG=MYDRIVER_ALL_INFO}(LEVEL, MSG, ...);
// FUNC TraceEvents(LEVEL, FLAGS, MSG, ...);
// end_wpp
//

//
// Driver specific #defines
//
// TODO: Change these values to be appropriate for your driver.
//

#define MYDRIVER_TRACING_ID		L"Microsoft\\UMDF\\Fx2Hid"
#define MYDRIVER_CLASS_ID		{0xc759cfb5, 0x6ea1, 0x4ad7, {0xab, 0x76, 0x83, 0x16, 0x9a, 0xa, 0xa0, 0x1c}}

//
// Include the type specific headers.
//

#include "comsup.h"
#include "driver.h"
#include "device.h"
#include "queue.h"
#include "ControlQueue.h"
#include "ReadWriteQueue.h"
#include "list.h"

__forceinline 
#ifdef _PREFAST_
__declspec(noreturn)
#endif
VOID
WdfTestNoReturn(
	VOID
	)
{
	// do nothing.
}

#define WUDF_TEST_DRIVER_ASSERT(p)	\
{									\
	if ( !(p) )						\
	{								\
		DebugBreak();				\
		WdfTestNoReturn();			\
	}								\
}

#define SAFE_RELEASE(p)	 {if ((p)) { (p)->Release(); (p) = NULL; }}
