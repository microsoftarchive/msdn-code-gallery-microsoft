#pragma once

#include <windows.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <strsafe.h>

//
// Define KDEXT_64BIT to make all wdbgexts APIs recognize 64 bit addresses
// It is recommended for extensions to use 64 bit headers from wdbgexts so
// the extensions could support 64 bit targets.
//
#define KDEXT_64BIT
#include <wdbgexts.h>

#pragma warning(disable:4201) // nonstandard extension used : nameless struct
#pragma warning(disable:4100) // nonstandard extension used : nameless struct

#include "mpdbg.h"
#include "copied.h"

#define BAIL() goto exitPt;

#define BAIL_ON_HR_ERROR(hr)   \
    if (FAILED(hr))         \
    {                       \
        goto exitPt;        \
    }                       \
    
#define WIN32_FROM_HRESULT(hr)           \
    (SUCCEEDED(hr) ? ERROR_SUCCESS :    \
        (HRESULT_FACILITY(hr) == FACILITY_WIN32 ? HRESULT_CODE(hr) : (hr)))

#pragma hdrstop

