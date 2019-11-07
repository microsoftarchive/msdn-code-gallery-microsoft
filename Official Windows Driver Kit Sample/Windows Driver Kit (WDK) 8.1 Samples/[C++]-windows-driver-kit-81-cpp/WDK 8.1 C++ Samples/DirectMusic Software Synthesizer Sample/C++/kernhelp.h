/*
    Copyright (c) 1998-2000 Microsoft Corporation.  All rights reserved.
*/

#ifndef _KernHelp_
#define _KernHelp_

#include <evntrace.h>

//
// Implement this macro for debug tracing
//
#define _DbgPrintF(lvl, args)    ((void) 0)

#define DEBUGLVL_BLAB      TRACE_LEVEL_VERBOSE
#define DEBUGLVL_VERBOSE   TRACE_LEVEL_VERBOSE
#define DEBUGLVL_TERSE     TRACE_LEVEL_INFORMATION
#define DEBUGLVL_ERROR     TRACE_LEVEL_ERROR

#define DEBUGLVL_WARNING   TRACE_LEVEL_WARNING
#define DEBUGLVL_INFO      TRACE_LEVEL_INFORMATION

// Use kernel mutex to implement critical section
//
typedef KMUTEX CRITICAL_SECTION;
typedef CRITICAL_SECTION *LPCRITICAL_SECTION;

VOID InitializeCriticalSection(LPCRITICAL_SECTION);
_Acquires_lock_(*CritSect) VOID EnterCriticalSection(LPCRITICAL_SECTION CritSect);
_Releases_lock_(*CritSect) VOID LeaveCriticalSection(LPCRITICAL_SECTION CritSect);
VOID DeleteCriticalSection(LPCRITICAL_SECTION);

// We have very little registry work to do, so just encapsulate the
// entire process
//
int GetRegValueDword(_In_ LPTSTR RegPath,_In_ LPTSTR ValueName,PULONG Value);

ULONG GetTheCurrentTime();

PVOID KernHelpGetSysAddrForMdl(PMDL pMdl);


#ifndef _NEW_DELETE_OPERATORS_
#define _NEW_DELETE_OPERATORS_

/*****************************************************************************
 * operator new()
 *****************************************************************************
 * Overload new to allocate from PagedPool, with our pooltag.
 */
inline void* __cdecl operator new
(
    size_t    iSize
)
{
    //  Replace 'ySkD' with a tag appropriate to your product.
    PVOID result = ExAllocatePoolWithTag(PagedPool, iSize, 'ySkD');

    if (result)
    {
        RtlZeroMemory(result, iSize);
    }
#if DBG
    else
    {
        _DbgPrintF(DEBUGLVL_TERSE, ("Couldn't allocate paged pool: %I64d bytes", iSize));
    }
#endif // DBG
    return result;
}

/*****************************************************************************
 * operator new
 *****************************************************************************
 * Overload new to allocate with our pooltag.
 * Allocates from PagedPool or NonPagedPool, as specified.
 */
inline PVOID operator new
(
    size_t    iSize,
    _When_((poolType & NonPagedPoolMustSucceed) != 0,
       __drv_reportError("Must succeed pool allocations are forbidden. "
             "Allocation failures cause a system crash"))
    POOL_TYPE poolType
)
{
    //  Replace 'ySkD' with a tag appropriate to your product.
    PVOID result = ExAllocatePoolWithTag(poolType, iSize, 'ySkD');
    if (result)
    {
        RtlZeroMemory(result, iSize);
    }
#if DBG
    else
    {
        _DbgPrintF(DEBUGLVL_TERSE, ("Couldn't allocate poolType(%d): %I64d bytes", (ULONG)poolType, iSize));
    }
#endif // DBG

    return result;
}

/*****************************************************************************
 * operator new()
 *****************************************************************************
 * Overload new to allocate with a specified allocation tag.
 * Allocates from PagedPool or NonPagedPool, as specified.
 */
inline PVOID operator new
(
    size_t      iSize,
    _When_((poolType & NonPagedPoolMustSucceed) != 0,
       __drv_reportError("Must succeed pool allocations are forbidden. "
             "Allocation failures cause a system crash"))
    POOL_TYPE   poolType,
    ULONG       tag
)
{
    PVOID result = ExAllocatePoolWithTag(poolType, iSize, tag);

    if (result)
    {
        RtlZeroMemory(result,iSize);
    }
#if DBG
    else
    {
        _DbgPrintF(DEBUGLVL_TERSE, ("Couldn't allocate tagged poolType(%d): %I64d bytes",
            (ULONG)poolType, iSize));
    }
#endif // DBG

    return result;
}

/*****************************************************************************
 * operator delete()
 *****************************************************************************
 * Delete function.
 */
inline void __cdecl operator delete
(
    PVOID pVoid
)
{
    ExFreePool(pVoid);
}


#endif //!_NEW_DELETE_OPERATORS_

// Debug trace facility
//
#define DM_DEBUG_CRITICAL       DEBUGLVL_ERROR   // Used to include critical messages
#define DM_DEBUG_NON_CRITICAL   DEBUGLVL_TERSE   // Used to include level 1 plus important non-critical messages
#define DM_DEBUG_STATUS         DEBUGLVL_VERBOSE // Used to include level 1 and level 2 plus status\state messages
#define DM_DEBUG_FUNC_FLOW      DEBUGLVL_BLAB    // Used to include level 1, level 2 and level 3 plus function flow messages
#define DM_DEBUG_ALL            DEBUGLVL_BLAB    // Used to include all debug messages

#if DBG
#define Trace
#define Trace0(lvl, fstr) \
    _DbgPrintF(lvl, (fstr))
#define Trace1(lvl, fstr, arg1) \
    _DbgPrintF(lvl, (fstr, arg1))
#define Trace2(lvl, fstr, arg1, arg2) \
    _DbgPrintF(lvl, (fstr, arg1, arg2))
#define Trace3(lvl, fstr, arg1, arg2, arg3) \
    _DbgPrintF(lvl, (fstr, arg1, arg2, arg3))
#define Trace4(lvl, fstr, arg1, arg2, arg3, arg4) \
    _DbgPrintF(lvl, (fstr, arg1, arg2, arg3, arg4))
#else
#define Trace
#define Trace0
#define Trace1
#define Trace2
#define Trace3
#define Trace4
#endif

#define assert ASSERT

// Paramter validation unused
//
#define V_INAME(x)
#define V_BUFPTR_READ(p,cb)


#endif // _KernHelp_

