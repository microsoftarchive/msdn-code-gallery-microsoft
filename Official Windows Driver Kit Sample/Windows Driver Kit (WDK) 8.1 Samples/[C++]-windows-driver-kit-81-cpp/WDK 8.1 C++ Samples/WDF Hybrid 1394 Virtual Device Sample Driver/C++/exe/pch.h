/*++

Copyright (c) 1998  Microsoft Corporation

Module Name: 

    pch.h
--*/

//
// Disable warnings for nonstandard extension used : nameless struct/union.
// This warning is generated from WinIoctl.h and not from this code.
//

#if _MSC_VER >= 1200

#pragma warning(push)

#endif

#pragma warning(disable:4201) // nonstandard extension used : nameless struct/union

#include <driverspecs.h>
_Analysis_mode_(_Analysis_code_type_user_driver_);
#include <windows.h>
#include <setupapi.h>
#include <stdio.h>
#include <stdlib.h>
#include <winioctl.h>
#include <ntdd1394.h>
#include <initguid.h>
#include "wdf_common.h"
#include "wdf_vdev_api.h"

#include "debug.h"
#include "util.h"
#include "async.h"
#include "isoch.h"
#include "1394.h"
#include "resource.h"
#include "local.h"
#define STRSAFE_NO_DEPRECATE
#include <strsafe.h>
#include <intsafe.h>


#if _MSC_VER >= 1200

#pragma warning(pop)

#else

#endif
