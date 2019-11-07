////////////////////////////////////////////////////////////////////////////////////////////////////
//
//   Copyright (c) 2012 Microsoft Corporation.  All Rights Reserved.
//
//   Module Name:
//      SubscriptionFunctions_BFEState.h
//
//   Abstract:
//      This module contains protptyes for WFP callback functions for changes in the BFE state.
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

#ifndef SUBSCRIBE_BFE_STATE_H
#define SUBSCRIBE_BFE_STATE_H

_IRQL_requires_(PASSIVE_LEVEL)
_IRQL_requires_same_
VOID SubscriptionBFEStateChangeCallback(_Inout_ VOID* pContext,
                                        _In_ FWPM_SERVICE_STATE bfeState);

#endif /// SUBSCRIBE_BFE_STATE_H