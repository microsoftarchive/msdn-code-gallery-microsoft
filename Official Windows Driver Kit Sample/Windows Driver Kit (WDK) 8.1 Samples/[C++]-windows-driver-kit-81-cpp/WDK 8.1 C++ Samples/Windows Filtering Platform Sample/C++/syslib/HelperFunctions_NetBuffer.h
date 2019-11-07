////////////////////////////////////////////////////////////////////////////////////////////////////
//
//   Copyright (c) 2012 Microsoft Corporation.  All Rights Reserved.
//
//   Module Name:
//      HelperFunctions_NetBuffer.h
//
//   Abstract:
//      This module contains prototypes for kernel helper functions that assist with NET_BUFFER and
//         NET_BUFFER_LIST.
//
//   Author:
//      Dusty Harper      (DHarper)
//
//   Revision History:
//
//      [ Month ][Day] [Year] - [Revision]-[ Comments ]
//      May       01,   2010  -     1.0   -  Creation
//
////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef HELPERFUNCTIONS_NET_BUFFER_H
#define HELPERFUNCTIONS_NET_BUFFER_H

_IRQL_requires_min_(PASSIVE_LEVEL)
_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_requires_same_
UINT32 KrnlHlprNBLGetRequiredRefCount(_In_ const NET_BUFFER_LIST* pNBL,
                                      _In_ BOOLEAN isChained = FALSE);

#endif /// HELPERFUNCTIONS_NET_BUFFER_H