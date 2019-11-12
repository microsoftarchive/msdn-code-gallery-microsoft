////////////////////////////////////////////////////////////////////////////////////////////////////
//
//   Copyright (c) 2012 Microsoft Corporation.  All Rights Reserved.
//
//   Module Name:
//      CompletionFunctions_TransparentProxyCallouts.h
//
//   Abstract:
//      This module contains prototypes for WFP Completion functions for transparently proxying 
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

#ifndef COMPLETION_TRANSPARENT_PROXY_H
#define COMPLETION_TRANSPARENT_PROXY_H

#if(NTDDI_VERSION >= NTDDI_WIN8)

typedef struct TRANSPARENT_PROXY_COMPLETION_DATA_
{
   KSPIN_LOCK                  spinLock;
   UINT32                      refCount;
   BOOLEAN                     performedInline;
   CLASSIFY_DATA*              pClassifyData;
   INJECTION_DATA*             pInjectionData;
   FWPS_TRANSPORT_SEND_PARAMS* pSendParams;
}TRANSPARENT_PROXY_COMPLETION_DATA, *PTRANSPARENT_PROXY_COMPLETION_DATA;

_At_(*ppCompletionData, _Pre_ _Notnull_)
_At_(*ppCompletionData, _Post_ _Null_ __drv_freesMem(Pool))
_IRQL_requires_min_(PASSIVE_LEVEL)
_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_requires_same_
_Success_(*ppCompletionData == 0)
VOID TransparentProxyCompletionDataDestroy(_Inout_ TRANSPARENT_PROXY_COMPLETION_DATA** ppCompletionData,
                                           _In_ BOOLEAN override = FALSE);

_IRQL_requires_min_(PASSIVE_LEVEL)
_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_requires_same_
VOID CompleteTransparentProxy(_In_ VOID* pContext,
                              _Inout_ NET_BUFFER_LIST* pNetBufferList,
                              _In_ BOOLEAN dispatchLevel);


#endif /// (NTDDI_VERSION >= NTDDI_WIN8)

#endif /// COMPLETION_TRANSPARENT_PROXY_H
