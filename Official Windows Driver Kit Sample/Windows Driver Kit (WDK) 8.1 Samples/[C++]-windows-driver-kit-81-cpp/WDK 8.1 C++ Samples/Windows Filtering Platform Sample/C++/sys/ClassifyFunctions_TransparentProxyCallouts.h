////////////////////////////////////////////////////////////////////////////////////////////////////
//
//   Copyright (c) 2012 Microsoft Corporation.  All Rights Reserved.
//
//   Module Name:
//      ClassifyFunctions_TransparentProxyCallouts.h
//
//   Abstract:
//      This module contains prototypes for WFP Classify functions for transparently proxying 
//         connections.
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

#ifndef CLASSIFY_TRANSPARENT_PROXY_H
#define CLASSIFY_TRANSPARENT_PROXY_H

#if(NTDDI_VERSION >= NTDDI_WIN8)

_IRQL_requires_(DISPATCH_LEVEL)
_IRQL_requires_same_
_Function_class_(KDEFERRED_ROUTINE)
VOID TransparentProxyPendConnectDeferredProcedureCall(_In_ KDPC* pDPC,
                                                      _In_opt_ PVOID pContext,
                                                      _In_opt_ PVOID pArg1,
                                                      _In_opt_ PVOID pArg2);

_IRQL_requires_min_(PASSIVE_LEVEL)
_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_requires_same_
VOID ClassifyTransparentProxyPendConnect(_In_ const FWPS_INCOMING_VALUES* pClassifyValues,
                                         _In_ const FWPS_INCOMING_METADATA_VALUES* pMetadata,
                                         _Inout_opt_ VOID* pLayerData,
                                         _In_opt_ const VOID* pClassifyContext,
                                         _In_ const FWPS_FILTER* pFilter,
                                         _In_ UINT64 flowContext,
                                         _Inout_ FWPS_CLASSIFY_OUT* pClassifyOut);

#endif /// (NTDDI_VERSION >= NTDDI_WIN8)

#endif /// CLASSIFY_TRANSPARENT_PROXY_H
