////////////////////////////////////////////////////////////////////////////////////////////////////
//
//   Copyright (c) 2012 Microsoft Corporation.  All Rights Reserved.
//
//   Module Name:
//      CompletionFunctions_PendAuthorizationCallouts.cpp
//
//   Abstract:
//      This module contains WFP Completion functions for pended authorizations that inject data 
//         back into the data path.
//
//   Naming Convention:
//
//      <Module><Scenario>
//  
//      i.e.
//       CompleteBasicPacketInjection
//
//       <Module>
//          Complete                        - Function is an FWPS_INJECT_COMPLETE function.
//       <Scenario>
//          PendAuthorization               - Function demonstrates the clone / block / inject 
//                                               model for pended items.
//
//      <Object><Action>
//
//      i.e.
//       PendAuthorizationCompletionDataDestroy
//
//       <Object>
//        {
//          PendAuthorizationCompletionData - pertains to PEND_AUTHORIZATION_COMPLETION_DATA.
//        }
//       <Action>
//          Destroy                         - Cleans up and frees all memory in the object.
//
//   Private Functions:
//      PendAuthorizationCompletionDataDestroy(),
//
//   Public Functions:
//      CompletePendAuthorization(),
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
#include "CompletionFunctions_PendAuthorizationCallouts.tmh" /// $(OBJ_PATH)\$(O)\

/**
 @private_function="PendAuthorizationCompletionDataDestroy"
 
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
VOID PendAuthorizationCompletionDataDestroy(_Inout_ PEND_AUTHORIZATION_COMPLETION_DATA** ppCompletionData,
                                            _In_ BOOLEAN override)                                         /* FALSE */
{
#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> PendAuthorizationCompletionDataDestroy()\n");

#endif /// DBG
   
   NT_ASSERT(ppCompletionData);
   NT_ASSERT(*ppCompletionData);

   PEND_AUTHORIZATION_COMPLETION_DATA* pCompletionData = *ppCompletionData;
   KIRQL                               originalIRQL    = PASSIVE_LEVEL;

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
 @completion_function="CompletePendAuthorization"
 
   Purpose:                                                                                     <br>
                                                                                                <br>
   Notes:                                                                                       <br>
                                                                                                <br>
   MSDN_Ref: HTTP://MSDN.Microsoft.com/En-Us/Library/Windows/Hardware/FF545018.aspx             <br>
*/
_IRQL_requires_min_(PASSIVE_LEVEL)
_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_requires_same_
VOID CompletePendAuthorization(_In_ VOID* pContext,
                               _Inout_ NET_BUFFER_LIST* pNetBufferList,
                               _In_ BOOLEAN dispatchLevel)
{
#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> CompletePendAuthorization()\n");

#endif /// DBG
   
   UNREFERENCED_PARAMETER(dispatchLevel);

   NT_ASSERT(pContext);
   NT_ASSERT(pNetBufferList);
   NT_ASSERT(NT_SUCCESS(pNetBufferList->Status));

   if(pNetBufferList->Status != STATUS_SUCCESS)
      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_ERROR_LEVEL,
                 " !!!! CompletePendAuthorization() [status: %#x]\n",
                 pNetBufferList->Status);

   FwpsFreeCloneNetBufferList(pNetBufferList,
                              0);

   PendAuthorizationCompletionDataDestroy((PEND_AUTHORIZATION_COMPLETION_DATA**)&pContext);

#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- CompletePendAuthorization()\n");

#endif /// DBG
   
   return;
}
