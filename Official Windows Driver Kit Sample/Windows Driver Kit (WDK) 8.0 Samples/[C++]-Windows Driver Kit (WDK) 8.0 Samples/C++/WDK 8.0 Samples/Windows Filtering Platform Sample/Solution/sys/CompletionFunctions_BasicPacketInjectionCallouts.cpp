////////////////////////////////////////////////////////////////////////////////////////////////////
//
//   Copyright (c) 2012 Microsoft Corporation.  All Rights Reserved.
//
//   Module Name:
//      CompletionFunctions_BasicPacketInjectionCallouts.cpp
//
//   Abstract:
//      This module contains WFP Completion functions for injecting packets back into the data path 
//         using the clone / block / inject method.
//
//   Naming Convention:
//
//      <Module><Scenario>
//  
//      i.e.
//       CompleteBasicPacketInjection
//
//       <Module>
//          Complete                           - Function is an FWPS_INJECT_COMPLETE function.
//       <Scenario>
//          BasicPacketInjection               - Function demonstrates the clone / block / inject 
//                                               model.
//
//      <Object><Action>
//
//      i.e.
//       BasicPacketInjectionCompletionDataDestroy
//
//       <Object>
//        {
//          BasicPacketInjectionCompletionData - pertains to BASIC_PACKET_INJECTION_COMPLETION_DATA.
//        }
//       <Action>
//          Destroy                            - Cleans up and frees all memory in the object.
//
//   Private Functions:
//      BasicPacketInjectionCompletionDataDestroy(),
//
//   Public Functions:
//      CompleteBasicPacketInjection(),
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

#include "Framework_WFPSamplerCalloutDriver.h"                  /// .
#include "CompletionFunctions_BasicPacketInjectionCallouts.tmh" /// $(OBJ_PATH)\$(O)\

#if DBG

INJECTION_COUNTERS g_bpiTotalCompletions = {0};

/**
 @function="BasicPacketInjectionCountersDecrement"
 
   Purpose:  Deccrement the appropriate counters based on the injection handle.                 <br>
                                                                                                <br>
   Notes:                                                                                       <br>
                                                                                                <br>
   MSDN_Ref: HTTP://MSDN.Microsoft.com/En-US/Library/Desktop/MS683581.aspx                      <br>
*/
VOID BasicPacketInjectionCountersDecrement(_In_ HANDLE injectionHandle,
                                           _Inout_ INJECTION_COUNTERS* pCounters)
{
   if(injectionHandle == g_IPv4InboundNetworkInjectionHandle)
      InterlockedDecrement64((LONG64*)&(pCounters->inboundNetwork_IPv4));
   else if(injectionHandle == g_IPv6InboundNetworkInjectionHandle)
      InterlockedDecrement64((LONG64*)&(pCounters->inboundNetwork_IPv6));
   else if(injectionHandle == g_IPv4OutboundNetworkInjectionHandle)
      InterlockedDecrement64((LONG64*)&(pCounters->outboundNetwork_IPv4));
   else if(injectionHandle == g_IPv6OutboundNetworkInjectionHandle)
      InterlockedDecrement64((LONG64*)&(pCounters->outboundNetwork_IPv6));
   else if(injectionHandle == g_IPv4InboundForwardInjectionHandle)
      InterlockedDecrement64((LONG64*)&(pCounters->inboundForward_IPv4));
   else if(injectionHandle == g_IPv6InboundForwardInjectionHandle)
      InterlockedDecrement64((LONG64*)&(pCounters->inboundForward_IPv6));
   else if(injectionHandle == g_IPv4OutboundForwardInjectionHandle)
      InterlockedDecrement64((LONG64*)&(pCounters->outboundForward_IPv4));
   else if(injectionHandle == g_IPv6OutboundForwardInjectionHandle)
      InterlockedDecrement64((LONG64*)&(pCounters->outboundForward_IPv6));
   else if(injectionHandle == g_IPv4InboundTransportInjectionHandle)
      InterlockedDecrement64((LONG64*)&(pCounters->inboundTransport_IPv4));
   else if(injectionHandle == g_IPv6InboundTransportInjectionHandle)
      InterlockedDecrement64((LONG64*)&(pCounters->inboundTransport_IPv6));
   else if(injectionHandle == g_IPv4OutboundTransportInjectionHandle)
      InterlockedDecrement64((LONG64*)&(pCounters->outboundTransport_IPv4));
   else if(injectionHandle == g_IPv6OutboundTransportInjectionHandle)
      InterlockedDecrement64((LONG64*)&(pCounters->outboundTransport_IPv6));

#if(NTDDI_VERSION >= NTDDI_WIN8)

   else if(injectionHandle == g_IPv4InboundMACInjectionHandle)
      InterlockedDecrement64((LONG64*)&(pCounters->inboundMAC_IPv4));
   else if(injectionHandle == g_IPv6InboundMACInjectionHandle)
      InterlockedDecrement64((LONG64*)&(pCounters->inboundMAC_IPv6));
   else if(injectionHandle == g_InboundMACInjectionHandle)
      InterlockedDecrement64((LONG64*)&(pCounters->inboundMAC_Unknown));
   else if(injectionHandle == g_IPv4OutboundMACInjectionHandle)
      InterlockedDecrement64((LONG64*)&(pCounters->outboundMAC_IPv4));
   else if(injectionHandle == g_IPv6OutboundMACInjectionHandle)
      InterlockedDecrement64((LONG64*)&(pCounters->outboundMAC_IPv6));
   else if(injectionHandle == g_OutboundMACInjectionHandle)
      InterlockedDecrement64((LONG64*)&(pCounters->outboundMAC_Unknown));
   else if(injectionHandle == g_IPv4IngressVSwitchEthernetInjectionHandle)
      InterlockedDecrement64((LONG64*)&(pCounters->ingressVSwitch_IPv4));
   else if(injectionHandle == g_IPv6IngressVSwitchEthernetInjectionHandle)
      InterlockedDecrement64((LONG64*)&(pCounters->ingressVSwitch_IPv6));
   else if(injectionHandle == g_IngressVSwitchEthernetInjectionHandle)
      InterlockedDecrement64((LONG64*)&(pCounters->ingressVSwitch_Unknown));
   else if(injectionHandle == g_IPv4EgressVSwitchEthernetInjectionHandle)
      InterlockedDecrement64((LONG64*)&(pCounters->egressVSwitch_IPv4));
   else if(injectionHandle == g_IPv6EgressVSwitchEthernetInjectionHandle)
      InterlockedDecrement64((LONG64*)&(pCounters->egressVSwitch_IPv6));
   else if(injectionHandle == g_EgressVSwitchEthernetInjectionHandle)
      InterlockedDecrement64((LONG64*)&(pCounters->egressVSwitch_Unknown));

#endif /// (NTDDI_VERSION >= NTDDI_WIN8)

   return;
}

#endif /// DBG

/**
 @private_function="BasicPacketInjectionCompletionDataDestroy"
 
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
VOID BasicPacketInjectionCompletionDataDestroy(_Inout_ BASIC_PACKET_INJECTION_COMPLETION_DATA** ppCompletionData,
                                               _In_ BOOLEAN override)                                             /* FALSE */
{
#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> BasicPacketInjectionCompletionDataDestroy()\n");

#endif /// DBG
   
   NT_ASSERT(ppCompletionData);
   NT_ASSERT(*ppCompletionData);

   BASIC_PACKET_INJECTION_COMPLETION_DATA* pCompletionData = *ppCompletionData;
   KIRQL                                   originalIRQL    = PASSIVE_LEVEL;
   INT32                                   refCount        = -1;

   refCount = InterlockedDecrement((LONG*)&(pCompletionData->refCount));
   if(refCount == 0 ||
      override)
   {
      KeAcquireSpinLock(&(pCompletionData->spinLock),
                        &originalIRQL);

      pCompletionData->refCount = -1;

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

      HLPR_DELETE(*ppCompletionData,
                  WFPSAMPLER_CALLOUT_DRIVER_TAG);
   }

#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- BasicPacketInjectionCompletionDataDestroy()\n");

#endif /// DBG

   return;
}

/**
 @completion_function="CompleteBasicPacketInjection"
 
   Purpose: Cleanup injection objects and memory.                                               <br>
                                                                                                <br>
   Notes:                                                                                       <br>
                                                                                                <br>
   MSDN_Ref: HTTP://MSDN.Microsoft.com/En-Us/Library/Windows/Hardware/FF545018.aspx             <br>
*/
_IRQL_requires_min_(PASSIVE_LEVEL)
_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_requires_same_
VOID CompleteBasicPacketInjection(_In_ VOID* pContext,
                                  _Inout_ NET_BUFFER_LIST* pNetBufferList,
                                  _In_ BOOLEAN dispatchLevel)
{
#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> CompleteBasicPacketInjection()\n");

#endif /// DBG

   UNREFERENCED_PARAMETER(dispatchLevel);

   NT_ASSERT(pContext);
   NT_ASSERT(pNetBufferList);
   NT_ASSERT(NT_SUCCESS(pNetBufferList->Status));
   NT_ASSERT(((BASIC_PACKET_INJECTION_COMPLETION_DATA*)pContext)->pInjectionData);

   if(pNetBufferList->Status != STATUS_SUCCESS)
      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_ERROR_LEVEL,
                 " !!!! CompleteBasicPacketInjection() [status: %#x]\n",
                 pNetBufferList->Status);

   FwpsFreeCloneNetBufferList(pNetBufferList,
                              0);
#if DBG

   BASIC_PACKET_INJECTION_COMPLETION_DATA* pCompletionData = (BASIC_PACKET_INJECTION_COMPLETION_DATA*)pContext;

   BasicPacketInjectionCountersIncrement(pCompletionData->pInjectionData->injectionHandle,
                                      &g_bpiTotalCompletions);

   BasicPacketInjectionCountersDecrement(pCompletionData->pInjectionData->injectionHandle,
                                         &g_bpiOutstandingNBLClones);

#endif // DBG

   BasicPacketInjectionCompletionDataDestroy((BASIC_PACKET_INJECTION_COMPLETION_DATA**)&pContext);

#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- CompleteBasicPacketInjection()\n");

#endif /// DBG

   return;
}
