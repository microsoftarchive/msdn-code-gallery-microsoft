////////////////////////////////////////////////////////////////////////////////////////////////////
//
//   Copyright (c) 2012 Microsoft Corporation.  All Rights Reserved.
//
//   Module Name:
//      CompletionFunctions_TransparentProxyCallouts.cpp
//
//   Abstract:
//      This module contains WFP Completion functions for transparently proxying connections.
//
//   Naming Convention:
//
//      <Module><Scenario>
//  
//      i.e.
//       CompleteTransparentProxy
//
//       <Module>
//          Complete                        - Function is an FWPS_INJECT_COMPLETE function.
//       <Scenario>
//          TransparentProxy                - Function demonstrates the.
//
//      <Object><Action>
//
//      i.e.
//       TransparentProxyCompletionDataDestroy
//
//       <Object>
//        {
//          TransparentProxyCompletionData  - pertains to TRANSPARENT_PROXY_COMPLETION_DATA.
//        }
//       <Action>
//          Destroy                         - Cleans up and frees all memory in the object.
//
//   Private Functions:
//
//   Public Functions:
//      CompleteTransparentProxy(),
//      TransparentProxyCompletionDataDestroy(),
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

#include "Framework_WFPSamplerCalloutDriver.h"               /// .
#include "CompletionFunctions_TransparentProxyCallouts.tmh" /// $(OBJ_PATH)\$(O)\

#if(NTDDI_VERSION >= NTDDI_WIN8)

/**
 @private_function="TransparentProxyCompletionDataDestroy"
 
   Purpose:                                                                                     <br>
                                                                                                <br>
   Notes:                                                                                       <br>
                                                                                                <br>
   MSDN_Ref:                                                                                    <br>
*/
_At_(*ppCompletionData, _Pre_ _Notnull_)
_At_(*ppCompletionData, _Post_ _Null_ __drv_freesMem(Pool))
_IRQL_requires_min_(PASSIVE_LEVEL)
_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_requires_same_
_Success_(*ppCompletionData == 0)
VOID TransparentProxyCompletionDataDestroy(_Inout_ TRANSPARENT_PROXY_COMPLETION_DATA** ppCompletionData,
                                           _In_ BOOLEAN override)                                        /* FALSE */
{
#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> TransparentProxyCompletionDataDestroy()\n");

#endif /// DBG
   
   NT_ASSERT(ppCompletionData);
   NT_ASSERT(*ppCompletionData);

   TRANSPARENT_PROXY_COMPLETION_DATA* pCompletionData = *ppCompletionData;
   KIRQL                              originalIRQL    = PASSIVE_LEVEL;

   KeAcquireSpinLock(&(pCompletionData->spinLock),
                     &originalIRQL);

   pCompletionData->refCount--;

   if(pCompletionData->pClassifyData)
   {
      if(!(pCompletionData->performedInline))
         KrnlHlprClassifyDataDestroyLocalCopy(&(pCompletionData->pClassifyData));
      else
      {
         HLPR_DELETE(pCompletionData->pClassifyData,
                     WFPSAMPLER_CALLOUT_DRIVER_TAG);
      }
   }

   if(pCompletionData->pInjectionData)
      KrnlHlprInjectionDataDestroy(&(pCompletionData->pInjectionData));

   if(pCompletionData->pSendParams)
   {
      HLPR_DELETE_ARRAY(pCompletionData->pSendParams->remoteAddress,
                        WFPSAMPLER_CALLOUT_DRIVER_TAG);

      HLPR_DELETE(pCompletionData->pSendParams,
                  WFPSAMPLER_CALLOUT_DRIVER_TAG);
   }

   KeReleaseSpinLock(&(pCompletionData->spinLock),
                     originalIRQL);

   if(pCompletionData->refCount == 0 ||
      override)
   {
      HLPR_DELETE(*ppCompletionData,
                  WFPSAMPLER_CALLOUT_DRIVER_TAG);
   }

#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- PendAuthorizationCompletionDataDestroy()\n");

#endif /// DBG
   
   return;
}

/**
 @completion_function="CompleteTransparentProxy"
 
   Purpose:                                                                                     <br>
                                                                                                <br>
   Notes:                                                                                       <br>
                                                                                                <br>
   MSDN_Ref: HTTP://MSDN.Microsoft.com/En-Us/Library/Windows/Hardware/FF545018.aspx             <br>
*/
_IRQL_requires_min_(PASSIVE_LEVEL)
_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_requires_same_
VOID CompleteTransparentProxy(_In_ VOID* pContext,
                              _Inout_ NET_BUFFER_LIST* pNetBufferList,
                              _In_ BOOLEAN dispatchLevel)
{
#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> CompleteTransparentProxy()\n");

#endif /// DBG
   
   UNREFERENCED_PARAMETER(dispatchLevel);

   NT_ASSERT(pContext);
   NT_ASSERT(pNetBufferList);
   NT_ASSERT(NT_SUCCESS(pNetBufferList->Status));

   if(pNetBufferList->Status != STATUS_SUCCESS)
      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_ERROR_LEVEL,
                 " !!!! CompleteTransparentProxy() [status: %#x]\n",
                 pNetBufferList->Status);

   FwpsFreeCloneNetBufferList(pNetBufferList,
                              0);

   TransparentProxyCompletionDataDestroy((TRANSPARENT_PROXY_COMPLETION_DATA**)&pContext);

#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- CompleteTransparentProxy()\n");

#endif /// DBG
   
   return;
}

#endif /// (NTDDI_VERSION >= NTDDI_WIN8)
