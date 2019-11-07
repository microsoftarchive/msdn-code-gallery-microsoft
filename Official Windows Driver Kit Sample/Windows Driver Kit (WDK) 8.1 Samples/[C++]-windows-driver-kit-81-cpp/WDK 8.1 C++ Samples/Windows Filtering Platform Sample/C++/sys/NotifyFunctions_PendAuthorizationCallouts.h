////////////////////////////////////////////////////////////////////////////////////////////////////
//
//   Copyright (c) 2012 Microsoft Corporation.  All Rights Reserved.
//
//   Module Name:
//      NotifyFunctions_PendAuthorizationCallouts.h
//
//   Abstract:
//      This module contains prototypes of WFP Notify functions for the pen authorization callouts.
//
//   Naming Convention:
//
//      <Module><Scenario>
//  
//      i.e.
//
//       NotifyProxyByALERedirectNotification
//
//       <Module>
//          Notify                            -     Function is located in sys\NotifyFunctions\
//       <Scenario>
//          PendAuthorizationNotification     -     Function demonstates use of notifications for 
//                                                     callouts pending authorization requests.
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

#ifndef NOTIFY_PEND_AUTHORIZATION_NOTIFICATION_H
#define NOTIFY_PEND_AUTHORIZATION_NOTIFICATION_H

_IRQL_requires_min_(PASSIVE_LEVEL)
_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_requires_same_
_Success_(return == STATUS_SUCCESS)
NTSTATUS NotifyPendAuthorizationNotification(_In_ FWPS_CALLOUT_NOTIFY_TYPE notificationType,
                                             _In_ const GUID* pFilterKey,
                                             _Inout_ FWPS_FILTER* pFilter);

#endif /// NOTIFY_PEND_AUTHORIZATION_NOTIFICATION_H