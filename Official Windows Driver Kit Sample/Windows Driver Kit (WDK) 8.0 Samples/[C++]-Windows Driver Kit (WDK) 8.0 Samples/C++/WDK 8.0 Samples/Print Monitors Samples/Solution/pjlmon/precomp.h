/*++

Copyright (c) 1994-2003 Microsoft Corporation
All rights reserved.

Module Name:

    precomp.h

Abstract:

    Precompiled header file

Author:

Revision History:

--*/

#if _MSC_VER >= 1200
#pragma warning(push)
#endif

#pragma warning(disable:4201) // nameless struct/union

#include <windows.h>
#include <winspool.h>
#include <winsplp.h>
#include "spltypes.h"
#include "local.h"
#include "parsepjl.h"
#include <strsafe.h>
#include <intsafe.h>

#if _MSC_VER >= 1200
#pragma warning(pop)
#else
#pragma warning(default:4201)
#endif
