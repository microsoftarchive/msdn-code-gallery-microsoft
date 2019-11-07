////////////////////////////////////////////////////////////////////////////////////////////////////
//
//   Copyright (c) 2012 Microsoft Corporation.  All Rights Reserved.
//
//   Module Name:
//      ClassifyFunctions_ProxyCallouts.cpp
//
//   Abstract:
//      This module contains WFP Classify functions for proxying connections and sockets.
//
//   Naming Convention:
//
//      <Module><Scenario>
//  
//      i.e.
//
//       ClassifyProxyByALERedirect
//
//       <Module>
//          Classify               -       Function is an FWPS_CALLOUT_CLASSIFY_FN
//          Perform                -       Function executes the desired scenario.
//          Prv                    -       Function is a private helper to this module.
//          Trigger                -
//       <Scenario>
//          ProxyByALERedirect     -       Function demonstates use of 
//                                            FWPM_LAYER_ALE_CONNECT_REDIRECT_V{4/6} and
//                                            FWPM_LAYER_ALE_BIND_REDIRECT_V{4/6} for proxying.
//                                            (For use in Win7+)
//
//   Private Functions:
//      PerformProxyInjectionAtOutboundTransport(),
//      PerformProxyInjectionAtInboundTransport(),
//      PerformProxySocketRedirection(),
//      PerformProxyConnectRedirection(),
//      ProxyByALERedirectWorkItemRoutine(),
//      ProxyUsingInjectionMethodWorkItemRoutine(),
//      TriggerProxyByALERedirectInline(),
//      TriggerProxyByALERedirectOutOfBand(),
//      TriggerProxyInjectionInline(),
//      TriggerProxyInjectionOutOfBand(),
//
//   Public Functions:
//      ClassifyProxyByALERedirect(),
//      ClassifyProxyByInjection(),
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

#include "Framework_WFPSamplerCalloutDriver.h" /// .
#include "ClassifyFunctions_ProxyCallouts.tmh" /// $(OBJ_PATH)\$(O)\

/**
 @private_function="PerformProxyInjectionAtOutboundTransport"
 
   Purpose:                                                                                     <br>
                                                                                                <br>
   Notes:                                                                                       <br>
                                                                                                <br>
   MSDN_Ref:                                                                                    <br>
*/
_IRQL_requires_min_(PASSIVE_LEVEL)
_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_requires_same_
_Check_return_
_Success_(return == STATUS_SUCCESS)
NTSTATUS PerformProxyInjectionAtOutboundTransport(_In_ CLASSIFY_DATA** ppClassifyData,
                                                  _In_ INJECTION_DATA** ppInjectionData,
                                                  _In_ PC_PROXY_DATA* pProxyData,
                                                  _In_ BOOLEAN isInline = FALSE)
{
#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> PerformProxyInjectionAtOutboundTransport()\n");

#endif /// DBG
   
   NT_ASSERT(ppClassifyData);
   NT_ASSERT(ppInjectionData);
   NT_ASSERT(pProxyData);
   NT_ASSERT(*ppClassifyData);
   NT_ASSERT(*ppInjectionData);

   NTSTATUS                       status           = STATUS_SUCCESS;
   FWPS_INCOMING_VALUES*          pClassifyValues  = (FWPS_INCOMING_VALUES*)(*ppClassifyData)->pClassifyValues;
   FWPS_INCOMING_METADATA_VALUES* pMetadataValues  = (FWPS_INCOMING_METADATA_VALUES*)(*ppClassifyData)->pMetadataValues;
   HANDLE                         injectionHandle  = (*ppInjectionData)->injectionHandle;
   HANDLE                         injectionContext = (*ppInjectionData)->injectionContext;
   UINT64                         endpointHandle   = 0;
   FWPS_TRANSPORT_SEND_PARAMS*    pSendArgs        = 0;
   ADDRESS_FAMILY                 addressFamily    = (*ppInjectionData)->addressFamily;
   COMPARTMENT_ID                 compartmentId    = DEFAULT_COMPARTMENT_ID;
   NET_BUFFER_LIST*               pNetBufferList   = 0;
   FWP_VALUE*                     pRemoteAddress   = 0;
   PROXY_COMPLETION_DATA*         pCompletionData  = 0;
   FWP_VALUE*                     pProtocol        = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                                                               &FWPM_CONDITION_IP_PROTOCOL);

   HLPR_BAIL_ON_NULL_POINTER_WITH_STATUS(pProtocol,
                                         status);

#pragma warning(push)
#pragma warning(disable: 6014) /// pCompletionData & pSendArgs will be freed in completionFn using BasicPacketInjectionCompletionDataDestroy

   HLPR_NEW(pCompletionData,
            PROXY_COMPLETION_DATA,
            WFPSAMPLER_CALLOUT_DRIVER_TAG);
   HLPR_BAIL_ON_ALLOC_FAILURE(pCompletionData,
                              status);

   HLPR_NEW(pSendArgs,
            FWPS_TRANSPORT_SEND_PARAMS,
            WFPSAMPLER_CALLOUT_DRIVER_TAG);
   HLPR_BAIL_ON_ALLOC_FAILURE(pSendArgs,
                              status);

#pragma warning(pop)

   pCompletionData->performedInline = isInline;
   pCompletionData->pClassifyData  = *ppClassifyData;
   pCompletionData->pInjectionData = *ppInjectionData;
   pCompletionData->pProxyData     = pProxyData;
   pCompletionData->pSendParams    = pSendArgs;

   if(FWPS_IS_METADATA_FIELD_PRESENT(pMetadataValues,
                                     FWPS_METADATA_FIELD_TRANSPORT_ENDPOINT_HANDLE))
      endpointHandle = pMetadataValues->transportEndpointHandle;

   if(FWPS_IS_METADATA_FIELD_PRESENT(pMetadataValues,
                                     FWPS_METADATA_FIELD_COMPARTMENT_ID))
      compartmentId = (COMPARTMENT_ID)pMetadataValues->compartmentId;

   pRemoteAddress = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                              &FWPM_CONDITION_IP_REMOTE_ADDRESS);
   HLPR_BAIL_ON_NULL_POINTER_WITH_STATUS(pRemoteAddress,
                                         status);

   if(addressFamily == AF_INET6 &&
      pProxyData->ipVersion == IPV6)
   {
      HLPR_NEW_ARRAY(pSendArgs->remoteAddress,
                     BYTE,
                     IPV6_ADDRESS_SIZE,
                     WFPSAMPLER_CALLOUT_DRIVER_TAG);
      HLPR_BAIL_ON_ALLOC_FAILURE(pSendArgs->remoteAddress,
                                 status);

      RtlCopyMemory(pSendArgs->remoteAddress,
                    pRemoteAddress->byteArray16->byteArray16,
                    IPV6_ADDRESS_SIZE);

      if(FWPS_IS_METADATA_FIELD_PRESENT(pMetadataValues,
                                        FWPS_METADATA_FIELD_REMOTE_SCOPE_ID))
         pSendArgs->remoteScopeId = pMetadataValues->remoteScopeId;
   }
   else if(addressFamily == AF_INET &&
           pProxyData->ipVersion == IPV4)
   {
      UINT32 remoteAddress = htonl(pRemoteAddress->uint32);

      HLPR_NEW_ARRAY(pSendArgs->remoteAddress,
                     BYTE,
                     IPV4_ADDRESS_SIZE,
                     WFPSAMPLER_CALLOUT_DRIVER_TAG);
      HLPR_BAIL_ON_ALLOC_FAILURE(pSendArgs->remoteAddress,
                                 status);

      RtlCopyMemory(pSendArgs->remoteAddress,
                    &remoteAddress,
                    IPV4_ADDRESS_SIZE);
   }

   status = FwpsAllocateCloneNetBufferList((NET_BUFFER_LIST*)(*ppClassifyData)->pPacket,
                                           g_pNDISPoolData->nblPoolHandle,
                                           g_pNDISPoolData->nbPoolHandle,
                                           0,
                                           &pNetBufferList);
   if(status != STATUS_SUCCESS)
   {
      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_ERROR_LEVEL,
                 " !!!! PerformProxyInjectionAtOutboundTransport : FwpsAllocateCloneNetBufferList() [status: %#x]\n",
                 status);

      HLPR_BAIL;
   }

   if(pProxyData->flags & PCPDF_PROXY_REMOTE_ADDRESS)
   {
      BYTE*  pIPAddress  = pProxyData->ipVersion == IPV6 ?
                           (BYTE*)(pProxyData->proxyRemoteAddress.pIPv6) :
                           (BYTE*)(pProxyData->proxyRemoteAddress.pIPv4);
      UINT32 addressSize = pProxyData->ipVersion == IPV6 ?
                           IPV6_ADDRESS_SIZE :
                           IPV4_ADDRESS_SIZE;

      RtlCopyMemory(pSendArgs->remoteAddress,
                    pIPAddress,
                    addressSize);
   }

   if(pProxyData->flags & PCPDF_PROXY_LOCAL_PORT)
   {
      FWP_VALUE localPort;

      RtlZeroMemory(&localPort,
                    sizeof(FWP_VALUE));

      localPort.type   = FWP_UINT16;
      localPort.uint16 = pProxyData->proxyLocalPort;

      if(pProtocol->uint8 == IPPROTO_ICMP)
      {
         status = KrnlHlprICMPv4HeaderModifyType(&localPort,
                                                 pNetBufferList);
         HLPR_BAIL_ON_FAILURE(status);
      }
      else if(pProtocol->uint8 == IPPROTO_TCP)
      {
         status = KrnlHlprTCPHeaderModifySourcePort(&localPort,
                                                    pNetBufferList);
         HLPR_BAIL_ON_FAILURE(status);
      }
      else if(pProtocol->uint8 == IPPROTO_UDP)
      {
         status = KrnlHlprUDPHeaderModifySourcePort(&localPort,
                                                    pNetBufferList);
         HLPR_BAIL_ON_FAILURE(status);
      }
      else if(pProtocol->uint8 == IPPROTO_ICMPV6)
      {
         status = KrnlHlprICMPv6HeaderModifyType(&localPort,
                                                 pNetBufferList);
         HLPR_BAIL_ON_FAILURE(status);
      }
   }

   if(pProxyData->flags & PCPDF_PROXY_REMOTE_PORT)
   {
      FWP_VALUE remotePort;

      RtlZeroMemory(&remotePort,
                    sizeof(FWP_VALUE));

      remotePort.type   = FWP_UINT16;
      remotePort.uint16 = pProxyData->proxyRemotePort;

      if(pProtocol->uint8 == IPPROTO_ICMP)
      {
         status = KrnlHlprICMPv4HeaderModifyType(&remotePort,
                                                 pNetBufferList);
         HLPR_BAIL_ON_FAILURE(status);
      }
      else if(pProtocol->uint8 == IPPROTO_TCP)
      {
         status = KrnlHlprTCPHeaderModifyDestinationPort(&remotePort,
                                                         pNetBufferList);
         HLPR_BAIL_ON_FAILURE(status);
      }
      else if(pProtocol->uint8 == IPPROTO_UDP)
      {
         status = KrnlHlprUDPHeaderModifyDestinationPort(&remotePort,
                                                         pNetBufferList);
         HLPR_BAIL_ON_FAILURE(status);
      }
      else if(pProtocol->uint8 == IPPROTO_ICMPV6)
      {
         status = KrnlHlprICMPv6HeaderModifyType(&remotePort,
                                                 pNetBufferList);
         HLPR_BAIL_ON_FAILURE(status);
      }
   }

   if(pProxyData->flags & PCPDF_PROXY_LOCAL_ADDRESS ||
      pProxyData->flags & PCPDF_PROXY_LOCAL_PORT ||
      pProxyData->flags & PCPDF_PROXY_REMOTE_PORT)
   {
      BYTE*       pSourceAddress     = 0;
      WSACMSGHDR* pControlData       = 0;
      UINT32      controlDataLength  = 0;
      FWP_VALUE*  pInterfaceIndex    = 0;
      FWP_VALUE*  pSubInterfaceIndex = 0;
      IF_INDEX    interfaceIndex     = 0;
      IF_INDEX    subInterfaceIndex  = 0;
      UINT32      tempAddress        = 0;

      if(pProxyData->flags & PCPDF_PROXY_LOCAL_ADDRESS)
         pSourceAddress = pProxyData->ipVersion == IPV6 ?
                          (BYTE*)(pProxyData->proxyLocalAddress.pIPv6) :
                          (BYTE*)(pProxyData->proxyLocalAddress.pIPv4);
      else
      {
         FWP_VALUE* pLocalAddress = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                                              &FWPM_CONDITION_IP_LOCAL_ADDRESS);

         HLPR_BAIL_ON_NULL_POINTER_WITH_STATUS(pLocalAddress,
                                               status);

         if(pLocalAddress->type == FWP_UINT32)
         {
            tempAddress = htonl(pLocalAddress->uint32);

            pSourceAddress = (BYTE*)&tempAddress;
         }
         else
            pSourceAddress = pLocalAddress->byteArray16->byteArray16;
      }

      pInterfaceIndex = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                                  &FWPM_CONDITION_INTERFACE_INDEX);
      if(pInterfaceIndex &&
         pInterfaceIndex->type == FWP_UINT32)
         interfaceIndex = (IF_INDEX)pInterfaceIndex->uint32;

      pSubInterfaceIndex = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                                     &FWPM_CONDITION_SUB_INTERFACE_INDEX);
      if(pSubInterfaceIndex &&
         pSubInterfaceIndex->type == FWP_UINT32)
         subInterfaceIndex = (IF_INDEX)pSubInterfaceIndex->uint32;            

      if(FWPS_IS_METADATA_FIELD_PRESENT(pMetadataValues,
                                        FWPS_METADATA_FIELD_TRANSPORT_CONTROL_DATA))
      {
         pControlData = pMetadataValues->controlData;

         controlDataLength = pMetadataValues->controlDataLength;
      }

      status = FwpsConstructIpHeaderForTransportPacket(pNetBufferList,
                                                       0,
                                                       addressFamily,
                                                       pSourceAddress,
                                                       pSendArgs->remoteAddress,
                                                       (IPPROTO)pProtocol->uint8,
                                                       endpointHandle,
                                                       pControlData,
                                                       controlDataLength,
                                                       0,
                                                       0,
                                                       interfaceIndex,
                                                       subInterfaceIndex);
      if(status != STATUS_SUCCESS)
      {
         DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                    DPFLTR_ERROR_LEVEL,
                    " !!!! PerformProxyInjectionAtOutboundTransport : FwpsConstructIpHeaderForTransportPacket() [status: %#x]\n",
                    status);

         HLPR_BAIL;
      }

   }

   status = FwpsInjectTransportSendAsync(injectionHandle,
                                         injectionContext,
                                         endpointHandle,
                                         0,
                                         pSendArgs,
                                         addressFamily,
                                         compartmentId,
                                         pNetBufferList,
                                         CompleteProxyInjection,
                                         pCompletionData);
   if(status != STATUS_SUCCESS)
   {
      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_ERROR_LEVEL,
                 " !!!! PerformProxyInjectionAtOutboundTransport : FwpsInjectTransportSendAsync() [status: %#x]\n",
                 status);

      HLPR_BAIL;
   }

   HLPR_BAIL_LABEL:

   if(status != STATUS_SUCCESS)
   {
      if(pNetBufferList)
      {
         FwpsFreeCloneNetBufferList(pNetBufferList,
                                    0);

         pNetBufferList = 0;
      }

      if(pCompletionData)
         ProxyCompletionDataDestroy(&pCompletionData);
   }

#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- PerformProxyInjectionAtOutboundTransport() [status: %#x]\n",
              status);

#endif /// DBG
   
   return status;
}

/**
 @private_function="PerformProxyInjectionAtInboundTransport"
 
   Purpose:                                                                                     <br>
                                                                                                <br>
   Notes:                                                                                       <br>
                                                                                                <br>
   MSDN_Ref:                                                                                    <br>
*/
_IRQL_requires_min_(PASSIVE_LEVEL)
_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_requires_same_
_Check_return_
_Success_(return == STATUS_SUCCESS)
NTSTATUS PerformProxyInjectionAtInboundTransport(_In_ CLASSIFY_DATA** ppClassifyData,
                                                 _In_ INJECTION_DATA** ppInjectionData,
                                                 _In_ PC_PROXY_DATA* pProxyData,
                                                 _In_ BOOLEAN isInline = FALSE)
{
#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> PerformProxyInjectionAtInboundTransport()\n");

#endif /// DBG
   
   NT_ASSERT(ppClassifyData);
   NT_ASSERT(ppInjectionData);
   NT_ASSERT(pProxyData);
   NT_ASSERT(*ppClassifyData);
   NT_ASSERT(*ppInjectionData);

   NTSTATUS                       status             = STATUS_SUCCESS;
   FWPS_INCOMING_VALUES*          pClassifyValues    = (FWPS_INCOMING_VALUES*)(*ppClassifyData)->pClassifyValues;
   FWPS_INCOMING_METADATA_VALUES* pMetadataValues    = (FWPS_INCOMING_METADATA_VALUES*)(*ppClassifyData)->pMetadataValues;
   HANDLE                         injectionHandle    = (*ppInjectionData)->injectionHandle;
   HANDLE                         injectionContext   = (*ppInjectionData)->injectionContext;
   ADDRESS_FAMILY                 addressFamily      = (*ppInjectionData)->addressFamily;
   COMPARTMENT_ID                 compartmentId      = DEFAULT_COMPARTMENT_ID;
   NET_BUFFER_LIST*               pNetBufferList     = 0;
   IF_INDEX                       interfaceIndex     = 0;
   IF_INDEX                       subInterfaceIndex  = 0;
   PROXY_COMPLETION_DATA*         pCompletionData    = 0;
   UINT32                         bytesRetreated     = 0;
   FWP_VALUE*                     pInterfaceIndex    = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                                                                 &FWPM_CONDITION_INTERFACE_INDEX);
   FWP_VALUE*                     pSubInterfaceIndex = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                                                                 &FWPM_CONDITION_SUB_INTERFACE_INDEX);
   FWP_VALUE*                     pProtocol          = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                                                                 &FWPM_CONDITION_IP_PROTOCOL);

   HLPR_BAIL_ON_NULL_POINTER_WITH_STATUS(pProtocol,
                                         status);

#pragma warning(push)
#pragma warning(disable: 6014) /// pCompletionData will be freed in completionFn using BasicPacketInjectionCompletionDataDestroy

   HLPR_NEW(pCompletionData,
            PROXY_COMPLETION_DATA,
            WFPSAMPLER_CALLOUT_DRIVER_TAG);
   HLPR_BAIL_ON_ALLOC_FAILURE(pCompletionData,
                              status);

#pragma warning(pop)

   if(FWPS_IS_METADATA_FIELD_PRESENT(pMetadataValues,
                                     FWPS_METADATA_FIELD_COMPARTMENT_ID))
      compartmentId = (COMPARTMENT_ID)pMetadataValues->compartmentId;

   if(pInterfaceIndex &&
      pInterfaceIndex->type == FWP_UINT32)
      interfaceIndex = (IF_INDEX)pInterfaceIndex->uint32;

   if(pSubInterfaceIndex &&
      pSubInterfaceIndex->type == FWP_UINT32)
      subInterfaceIndex = (IF_INDEX)pSubInterfaceIndex->uint32;

   /// Retreat so we clone the entire NET_BUFFER_LIST
   status = NdisRetreatNetBufferDataStart(NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)(*ppClassifyData)->pPacket),
                                          bytesRetreated,
                                          0,
                                          0);
   if(status != STATUS_SUCCESS)
   {
      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_ERROR_LEVEL,
                 " !!!! PerformBasicPacketInjectionAtInboundTransport : NdisRetreatNetBufferDataStart() [status: %#x]\n",
                 status);

      HLPR_BAIL;
   }

   status = FwpsAllocateCloneNetBufferList((NET_BUFFER_LIST*)(*ppClassifyData)->pPacket,
                                           g_pNDISPoolData->nblPoolHandle,
                                           g_pNDISPoolData->nbPoolHandle,
                                           0,
                                           &pNetBufferList);

   /// Advance so we are back at the expected position in the NET_BUFFER_LIST
   NdisAdvanceNetBufferDataStart(NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)(*ppClassifyData)->pPacket),
                                 bytesRetreated,
                                 FALSE,
                                 0);

   if(status != STATUS_SUCCESS)
   {
      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_ERROR_LEVEL,
                 " !!!! PerformBasicPacketInjectionAtInboundTransport : FwpsAllocateCloneNetBufferList() [status: %#x]\n",
                 status);

      HLPR_BAIL;
   }

   if(pProxyData->flags & PCPDF_PROXY_REMOTE_ADDRESS)
   {
      FWP_VALUE* pRemoteAddress = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                                            &FWPM_CONDITION_IP_REMOTE_ADDRESS);

      HLPR_BAIL_ON_NULL_POINTER_WITH_STATUS(pRemoteAddress,
                                            status);
/*
      BYTE*  pIPAddress  = pProxyData->ipVersion == IPV6 ?
                           pProxyData->pProxyRemoteIPv6Address :
                           pProxyData->pProxyRemoteIPv4Address;
      UINT32 addressSize = pProxyData->ipVersion == IPV6 ?
                           IPV6_ADDRESS_SIZE :
                           IPV4_ADDRESS_SIZE;
*/
   }

   if(pProxyData->flags & PCPDF_PROXY_LOCAL_PORT)
   {
      FWP_VALUE localPort;

      RtlZeroMemory(&localPort,
                    sizeof(FWP_VALUE));

      localPort.type   = FWP_UINT16;
      localPort.uint16 = pProxyData->proxyLocalPort;

      if(pProtocol->uint8 == IPPROTO_ICMP)
      {
         status = KrnlHlprICMPv4HeaderModifyType(&localPort,
                                                 pNetBufferList);
         HLPR_BAIL_ON_FAILURE(status);
      }
      else if(pProtocol->uint8 == IPPROTO_TCP)
      {
         status = KrnlHlprTCPHeaderModifyDestinationPort(&localPort,
                                                         pNetBufferList);
         HLPR_BAIL_ON_FAILURE(status);
      }
      else if(pProtocol->uint8 == IPPROTO_UDP)
      {
         status = KrnlHlprUDPHeaderModifyDestinationPort(&localPort,
                                                         pNetBufferList);
         HLPR_BAIL_ON_FAILURE(status);
      }
      else if(pProtocol->uint8 == IPPROTO_ICMPV6)
      {
         status = KrnlHlprICMPv6HeaderModifyType(&localPort,
                                                 pNetBufferList);
         HLPR_BAIL_ON_FAILURE(status);
      }
   }

   if(pProxyData->flags & PCPDF_PROXY_REMOTE_PORT)
   {
      FWP_VALUE remotePort;

      RtlZeroMemory(&remotePort,
                    sizeof(FWP_VALUE));

      remotePort.type   = FWP_UINT16;
      remotePort.uint16 = pProxyData->proxyRemotePort;

      if(pProtocol->uint8 == IPPROTO_ICMP)
      {
         status = KrnlHlprICMPv4HeaderModifyType(&remotePort,
                                                 pNetBufferList);
         HLPR_BAIL_ON_FAILURE(status);
      }
      else if(pProtocol->uint8 == IPPROTO_TCP)
      {
         status = KrnlHlprTCPHeaderModifySourcePort(&remotePort,
                                                    pNetBufferList);
         HLPR_BAIL_ON_FAILURE(status);
      }
      else if(pProtocol->uint8 == IPPROTO_UDP)
      {
         status = KrnlHlprUDPHeaderModifySourcePort(&remotePort,
                                                    pNetBufferList);
         HLPR_BAIL_ON_FAILURE(status);
      }
      else if(pProtocol->uint8 == IPPROTO_ICMPV6)
      {
         status = KrnlHlprICMPv6HeaderModifyType(&remotePort,
                                                 pNetBufferList);
         HLPR_BAIL_ON_FAILURE(status);
      }
   }

   if(pProxyData->flags & PCPDF_PROXY_LOCAL_ADDRESS)
   {
   }

   pCompletionData->performedInline = isInline;
   pCompletionData->pClassifyData   = *ppClassifyData;
   pCompletionData->pInjectionData  = *ppInjectionData;
   pCompletionData->pProxyData      = pProxyData;

   status = FwpsInjectTransportReceiveAsync(injectionHandle,
                                            injectionContext,
                                            0,
                                            0,
                                            addressFamily,
                                            compartmentId,
                                            interfaceIndex,
                                            subInterfaceIndex,
                                            pNetBufferList,
                                            CompleteProxyInjection,
                                            pCompletionData);
   if(status != STATUS_SUCCESS)
   {
      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_ERROR_LEVEL,
                 " !!!! PerformBasicPacketInjectionAtInboundTransport : FwpsInjectTransportReceiveAsync() [status: %#x]\n",
                 status);

      HLPR_BAIL;
   }

   HLPR_BAIL_LABEL:

   if(status != STATUS_SUCCESS)
   {
      if(pNetBufferList)
      {
         FwpsFreeCloneNetBufferList(pNetBufferList,
                                    0);

         pNetBufferList = 0;
      }

      if(pCompletionData)
         ProxyCompletionDataDestroy(&pCompletionData);
   }

#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- PerformProxyInjectionAtInboundTransport() [status: %#x]\n",
              status);

#endif /// DBG
   
   return status;
}

/**
 @private_function="ProxyUsingInjectionMethodDeferredProcedureCall"
 
   Purpose:  Invokes the appropriate private injection routine to perform the injection at 
             DISPATCH_LEVEL.                                                                    <br>
                                                                                                <br>
   Notes:                                                                                       <br>
                                                                                                <br>
   MSDN_Ref: HTTP://MSDN.Microsoft.com/En-US/Library/Windows/Hardware/FF542972.aspx             <br>
*/
_IRQL_requires_(DISPATCH_LEVEL)
_IRQL_requires_same_
_Function_class_(KDEFERRED_ROUTINE)
VOID ProxyUsingInjectionMethodDeferredProcedureCall(_In_ KDPC* pDPC,
                                                    _In_opt_ PVOID pContext,
                                                    _In_opt_ PVOID pArg1,
                                                    _In_opt_ PVOID pArg2)
{
#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> BasicPacketInjectionDeferredProcedureCall()\n");

#endif /// DBG
   
   UNREFERENCED_PARAMETER(pDPC);
   UNREFERENCED_PARAMETER(pContext);
   UNREFERENCED_PARAMETER(pArg2);

   NT_ASSERT(pDPC);
   NT_ASSERT(pArg1);
   NT_ASSERT(((DPC_DATA*)pArg1)->pClassifyData);
   NT_ASSERT(((DPC_DATA*)pArg1)->pInjectionData);

   DPC_DATA* pDPCData = (DPC_DATA*)pArg1;

   if(pDPCData)
   {
      NTSTATUS              status          = STATUS_SUCCESS;
      FWPS_INCOMING_VALUES* pClassifyValues = 0;

      pClassifyValues = (FWPS_INCOMING_VALUES*)pDPCData->pClassifyData->pClassifyValues;

      if(pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_TRANSPORT_V4 ||
         pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_TRANSPORT_V6 ||
         (pDPCData->pInjectionData->direction == FWP_DIRECTION_OUTBOUND &&
         (pClassifyValues->layerId == FWPS_LAYER_DATAGRAM_DATA_V4 ||
         pClassifyValues->layerId == FWPS_LAYER_DATAGRAM_DATA_V6

#if(NTDDI_VERSION >= NTDDI_WIN7)

         ||
         pClassifyValues->layerId == FWPS_LAYER_STREAM_PACKET_V4 ||
         pClassifyValues->layerId == FWPS_LAYER_STREAM_PACKET_V6

#endif // (NTDDI_VERSION >= NTDDI_WIN7)

         )))
         status = PerformProxyInjectionAtOutboundTransport(&(pDPCData->pClassifyData),
                                                           &(pDPCData->pInjectionData),
                                                           (PC_PROXY_DATA*)pDPCData->pInjectionData->pContext);
      else if(pClassifyValues->layerId == FWPS_LAYER_INBOUND_TRANSPORT_V4 ||
              pClassifyValues->layerId == FWPS_LAYER_INBOUND_TRANSPORT_V6 ||
              (pDPCData->pInjectionData->direction == FWP_DIRECTION_INBOUND &&
              (pClassifyValues->layerId == FWPS_LAYER_DATAGRAM_DATA_V4 ||
              pClassifyValues->layerId == FWPS_LAYER_DATAGRAM_DATA_V6

#if(NTDDI_VERSION >= NTDDI_WIN7)

              ||
              pClassifyValues->layerId == FWPS_LAYER_STREAM_PACKET_V4 ||
              pClassifyValues->layerId == FWPS_LAYER_STREAM_PACKET_V6

#endif // (NTDDI_VERSION >= NTDDI_WIN7)

             )))
         status = PerformProxyInjectionAtInboundTransport(&(pDPCData->pClassifyData),
                                                          &(pDPCData->pInjectionData),
                                                          (PC_PROXY_DATA*)pDPCData->pInjectionData->pContext);

      if(status != STATUS_SUCCESS)
         DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                    DPFLTR_ERROR_LEVEL,
                    " !!!! ProxyUsingInjectionMethodDeferredProcedureCall() [status: %#x]\n",
                    status);

      KrnlHlprDPCDataDestroy(&pDPCData);
   }

#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- ProxyUsingInjectionMethodDeferredProcedureCall()\n");

#endif /// DBG
   
   return;
}

/**
 @private_function="ProxyUsingInjectionMethodWorkItemRoutine"
 
   Purpose:                                                                                     <br>
                                                                                                <br>
   Notes:                                                                                       <br>
                                                                                                <br>
   MSDN_Ref:                                                                                    <br>
*/
_IRQL_requires_(PASSIVE_LEVEL)
_IRQL_requires_same_
_Function_class_(IO_WORKITEM_ROUTINE)
VOID ProxyUsingInjectionMethodWorkItemRoutine(_In_ PDEVICE_OBJECT pDeviceObject,
                                              _In_opt_ PVOID pContext)
{
#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> ProxyUsingInjectionMethodWorkItemRoutine()\n");

#endif /// DBG
   
   UNREFERENCED_PARAMETER(pDeviceObject);

   NT_ASSERT(pContext);
   NT_ASSERT(((WORKITEM_DATA*)pContext)->pClassifyData);
   NT_ASSERT(((WORKITEM_DATA*)pContext)->pInjectionData);

   WORKITEM_DATA* pWorkItemData = (WORKITEM_DATA*)pContext;

   if(pWorkItemData)
   {
      NTSTATUS              status          = STATUS_SUCCESS;
      FWPS_INCOMING_VALUES* pClassifyValues = 0;

      pClassifyValues = (FWPS_INCOMING_VALUES*)pWorkItemData->pClassifyData->pClassifyValues;

      if(pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_TRANSPORT_V4 ||
         pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_TRANSPORT_V6 ||
         (pWorkItemData->pInjectionData->direction == FWP_DIRECTION_OUTBOUND &&
         (pClassifyValues->layerId == FWPS_LAYER_DATAGRAM_DATA_V4 ||
         pClassifyValues->layerId == FWPS_LAYER_DATAGRAM_DATA_V6

#if(NTDDI_VERSION >= NTDDI_WIN7)

         ||
         pClassifyValues->layerId == FWPS_LAYER_STREAM_PACKET_V4 ||
         pClassifyValues->layerId == FWPS_LAYER_STREAM_PACKET_V6

#endif // (NTDDI_VERSION >= NTDDI_WIN7)

         )))
         status = PerformProxyInjectionAtOutboundTransport(&(pWorkItemData->pClassifyData),
                                                           &(pWorkItemData->pInjectionData),
                                                           (PC_PROXY_DATA*)pWorkItemData->pInjectionData->pContext);
      else if(pClassifyValues->layerId == FWPS_LAYER_INBOUND_TRANSPORT_V4 ||
              pClassifyValues->layerId == FWPS_LAYER_INBOUND_TRANSPORT_V6 ||
              (pWorkItemData->pInjectionData->direction == FWP_DIRECTION_INBOUND &&
              (pClassifyValues->layerId == FWPS_LAYER_DATAGRAM_DATA_V4 ||
              pClassifyValues->layerId == FWPS_LAYER_DATAGRAM_DATA_V6

#if(NTDDI_VERSION >= NTDDI_WIN7)

              ||
              pClassifyValues->layerId == FWPS_LAYER_STREAM_PACKET_V4 ||
              pClassifyValues->layerId == FWPS_LAYER_STREAM_PACKET_V6

#endif // (NTDDI_VERSION >= NTDDI_WIN7)

             )))
         status = PerformProxyInjectionAtInboundTransport(&(pWorkItemData->pClassifyData),
                                                          &(pWorkItemData->pInjectionData),
                                                          (PC_PROXY_DATA*)pWorkItemData->pInjectionData->pContext);

      if(status != STATUS_SUCCESS)
         DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                    DPFLTR_ERROR_LEVEL,
                    " !!!! ProxyUsingInjectionMethodWorkItemRoutine : PerformProxyInjectionAt*() [status: %#x]\n",
                    status);

      KrnlHlprWorkItemDataDestroy(&pWorkItemData);
   }

#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- ProxyUsingInjectionMethodWorkItemRoutine()\n");

#endif /// DBG
   
   return;
}

/**
 @private_function="TriggerProxyInjectionInline"
 
   Purpose:                                                                                     <br>
                                                                                                <br>
   Notes:                                                                                       <br>
                                                                                                <br>
   MSDN_Ref:                                                                                    <br>
*/
_IRQL_requires_min_(PASSIVE_LEVEL)
_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_requires_same_
_Success_(return == STATUS_SUCCESS)
NTSTATUS TriggerProxyInjectionInline(_In_ const FWPS_INCOMING_VALUES* pClassifyValues,
                                     _In_ const FWPS_INCOMING_METADATA_VALUES* pMetadata,
                                     _Inout_ VOID* pNetBufferList,
                                     _In_opt_ const VOID* pClassifyContext,
                                     _In_ const FWPS_FILTER* pFilter,
                                     _In_ UINT64 flowContext,
                                     _Inout_ FWPS_CLASSIFY_OUT* pClassifyOut,
                                     _In_ INJECTION_DATA** ppInjectionData,
                                     _In_ PC_PROXY_DATA* pProxyData)
{
#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> TriggerProxyInjectionInline()\n");

#endif /// DBG
   
   NT_ASSERT(pClassifyValues);
   NT_ASSERT(pMetadata);
   NT_ASSERT(pNetBufferList);
   NT_ASSERT(pFilter);
   NT_ASSERT(pClassifyOut);
   NT_ASSERT(ppInjectionData);
   NT_ASSERT(*ppInjectionData);
   NT_ASSERT(pProxyData);

#if(NTDDI_VERSION >= NTDDI_WIN7)

   NT_ASSERT(pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_TRANSPORT_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_TRANSPORT_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_DATAGRAM_DATA_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_DATAGRAM_DATA_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_STREAM_PACKET_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_STREAM_PACKET_V6);

#else

   NT_ASSERT(pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_TRANSPORT_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_TRANSPORT_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_DATAGRAM_DATA_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_DATAGRAM_DATA_V6);

#endif // (NTDDI_VERSION >= NTDDI_WIN7)

   NTSTATUS      status       = STATUS_SUCCESS;
   CLASSIFY_DATA* pClassifyData = 0;

#pragma warning(push)
#pragma warning(disable: 6014) /// pClassifyData will be freed in completionFn using ProxyCompletionDataDestroy

   HLPR_NEW(pClassifyData,
            CLASSIFY_DATA,
            WFPSAMPLER_CALLOUT_DRIVER_TAG);
   HLPR_BAIL_ON_ALLOC_FAILURE(pClassifyData,
                              status);

#pragma warning(pop)

   pClassifyData->pClassifyValues  = pClassifyValues;
   pClassifyData->pMetadataValues  = pMetadata;
   pClassifyData->pPacket          = pNetBufferList;
   pClassifyData->pClassifyContext = pClassifyContext;
   pClassifyData->pFilter          = pFilter;
   pClassifyData->flowContext      = flowContext;
   pClassifyData->pClassifyOut     = pClassifyOut;

   status = PerformProxyInjectionAtOutboundTransport(&pClassifyData,
                                                     ppInjectionData,
                                                     pProxyData,
                                                     TRUE);

   HLPR_BAIL_LABEL:

#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- TriggerProxyInjectionInline() [status: %#x]\n",
              status);

#endif /// DBG
   
   return status;
}

/**
 @private_function="TriggerProxyInjectionOutOfBand"
 
   Purpose:                                                                                     <br>
                                                                                                <br>
   Notes:                                                                                       <br>
                                                                                                <br>
   MSDN_Ref:                                                                                    <br>
*/
_IRQL_requires_min_(PASSIVE_LEVEL)
_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_requires_same_
_Success_(return == STATUS_SUCCESS)
NTSTATUS TriggerProxyInjectionOutOfBand(_In_ const FWPS_INCOMING_VALUES* pClassifyValues,
                                        _In_ const FWPS_INCOMING_METADATA_VALUES* pMetadata,
                                        _Inout_ VOID* pNetBufferList,
                                        _In_opt_ const VOID* pClassifyContext,
                                        _In_ const FWPS_FILTER* pFilter,
                                        _In_ UINT64 flowContext,
                                        _In_ FWPS_CLASSIFY_OUT* pClassifyOut,
                                        _In_ INJECTION_DATA* pInjectionData,
                                        _In_ PC_PROXY_DATA* pPCData)
{
#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> TriggerProxyInjectionOutOfBand()\n");

#endif /// DBG
   
   NT_ASSERT(pClassifyValues);
   NT_ASSERT(pMetadata);
   NT_ASSERT(pNetBufferList);
   NT_ASSERT(pFilter);
   NT_ASSERT(pClassifyOut);
   NT_ASSERT(pInjectionData);
   NT_ASSERT(pPCData);

   NTSTATUS       status        = STATUS_SUCCESS;
   CLASSIFY_DATA* pClassifyData = 0;

#pragma warning(push)
#pragma warning(disable: 6014) /// pClassifyData will be freed in completionFn using ProxyInjectionCompletionDataDestroy

   status = KrnlHlprClassifyDataCreateLocalCopy(&pClassifyData,
                                                pClassifyValues,
                                                pMetadata,
                                                pNetBufferList,
                                                pClassifyContext,
                                                pFilter,
                                                flowContext,
                                                pClassifyOut);
   HLPR_BAIL_ON_FAILURE(status);

#pragma warning(pop)

   if(pPCData->useWorkItems)
      status = KrnlHlprWorkItemQueue(g_pWDMDevice,
                                     ProxyUsingInjectionMethodWorkItemRoutine,
                                     pClassifyData,
                                     pInjectionData,
                                     (VOID*)pPCData);
   else if(pPCData->useThreadedDPC)
      status = KrnlHlprThreadedDPCQueue(ProxyUsingInjectionMethodDeferredProcedureCall,
                                        pClassifyData,
                                        pInjectionData,
                                        0);
   else
      status = KrnlHlprDPCQueue(ProxyUsingInjectionMethodDeferredProcedureCall,
                                pClassifyData,
                                pInjectionData,
                                0);

   HLPR_BAIL_LABEL:

   if(status != STATUS_SUCCESS)
   {
      if(pClassifyData)
         KrnlHlprClassifyDataDestroyLocalCopy(&pClassifyData);
   }

#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- TriggerProxyInjectionOutOfBand() [status: %#x]\n",
              status);

#endif /// DBG
   
   return status;
}

#if(NTDDI_VERSION >= NTDDI_WIN7)

/**
 @private_function="PerformProxySocketRedirection"
 
   Purpose:                                                                                     <br>
                                                                                                <br>
   Notes:                                                                                       <br>
                                                                                                <br>
   MSDN_Ref:                                                                                    <br>
*/
_IRQL_requires_min_(PASSIVE_LEVEL)
_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_requires_same_
_Check_return_
_Success_(return == STATUS_SUCCESS)
NTSTATUS PerformProxySocketRedirection(_In_ CLASSIFY_DATA** ppClassifyData,
                                       _Inout_ REDIRECT_DATA** ppRedirectData)
{
#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> PerformProxySocketRedirection()\n");

#endif /// DBG
   
   NT_ASSERT(ppClassifyData);
   NT_ASSERT(ppRedirectData);
   NT_ASSERT((*ppRedirectData)->pWritableLayerData);
   NT_ASSERT((*ppRedirectData)->pProxyData);

   NTSTATUS              status          = STATUS_SUCCESS;
   FWPS_BIND_REQUEST*    pBindRequest    = (FWPS_BIND_REQUEST*)(*ppRedirectData)->pWritableLayerData;
   FWPS_INCOMING_VALUES* pClassifyValues = (FWPS_INCOMING_VALUES*)(*ppClassifyData)->pClassifyValues;
   FWP_VALUE*            pProtocolValue  = 0;
   UINT8                 ipProtocol      = 0;
   
   pProtocolValue = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                              &FWPM_CONDITION_IP_PROTOCOL);
   if(pProtocolValue)
      ipProtocol = pProtocolValue->uint8;

   if((*ppRedirectData)->pProxyData->flags & PCPDF_PROXY_LOCAL_ADDRESS)
      INETADDR_SET_ADDRESS((PSOCKADDR)&(pBindRequest->localAddressAndPort),
                           (*ppRedirectData)->pProxyData->proxyLocalAddress.pBytes);

   if((*ppRedirectData)->pProxyData->flags & PCPDF_PROXY_LOCAL_PORT)
      INETADDR_SET_PORT((PSOCKADDR)&(pBindRequest->localAddressAndPort),
                        (*ppRedirectData)->pProxyData->proxyLocalPort);

   if(ipProtocol == IPPROTO_TCP)
      pBindRequest->portReservationToken = (*ppRedirectData)->pProxyData->tcpPortReservationToken;
   else if(ipProtocol == IPPROTO_UDP)
      pBindRequest->portReservationToken = (*ppRedirectData)->pProxyData->udpPortReservationToken;

   (*ppRedirectData)->pClassifyOut->actionType = FWP_ACTION_PERMIT;

#pragma warning(push)
#pragma warning(disable: 6001) /// *ppRedirectData has already been initialized in previous call to KrnlHlprRedirectDataCreate

   /// This will apply the modified data and cleanup the classify handle
   KrnlHlprRedirectDataDestroy(ppRedirectData);

#pragma warning(pop)


#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- PerformProxySocketRedirection() [status: %#x]\n",
              status);

#endif /// DBG
   
   return status;
}

/**
 @private_function="PerformProxyConnectRedirection"
 
   Purpose:                                                                                     <br>
                                                                                                <br>
   Notes:                                                                                       <br>
                                                                                                <br>
   MSDN_Ref:                                                                                    <br>
*/
_IRQL_requires_min_(PASSIVE_LEVEL)
_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_requires_same_
_Check_return_
_Success_(return == STATUS_SUCCESS)
NTSTATUS PerformProxyConnectRedirection(_In_ CLASSIFY_DATA** ppClassifyData,
                                        _Inout_ REDIRECT_DATA** ppRedirectData)
{
#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> PerformProxyConnectRedirection()\n");

#endif /// DBG
   
   NT_ASSERT(ppClassifyData);
   NT_ASSERT(ppRedirectData);
   NT_ASSERT(*ppClassifyData);
   NT_ASSERT(*ppRedirectData);

   NTSTATUS              status           = STATUS_SUCCESS;
   FWPS_CONNECT_REQUEST* pConnectRequest  = (FWPS_CONNECT_REQUEST*)(*ppRedirectData)->pWritableLayerData;
   UINT32                actionType       = FWP_ACTION_PERMIT;
   FWPS_INCOMING_VALUES* pClassifyValues  = (FWPS_INCOMING_VALUES*)(*ppClassifyData)->pClassifyValues;
   FWP_VALUE*            pProtocolValue   = 0;
   UINT8                 ipProtocol       = 0;

#if(NTDDI_VERSION >= NTDDI_WIN8)

   SOCKADDR_STORAGE*     pSockAddrStorage = 0;

   if((*ppRedirectData)->redirectHandle)
      pConnectRequest->localRedirectHandle = (*ppRedirectData)->redirectHandle;

   HLPR_NEW_ARRAY(pSockAddrStorage,
                  SOCKADDR_STORAGE,
                  2,
                  WFPSAMPLER_CALLOUT_DRIVER_TAG);
   HLPR_BAIL_ON_ALLOC_FAILURE(pSockAddrStorage,
                              status);

   /// Pass original remote destination values to query them in user mode
   RtlCopyMemory(&(pSockAddrStorage[0]),
                 &(pConnectRequest->remoteAddressAndPort),
                 sizeof(SOCKADDR_STORAGE));

   RtlCopyMemory(&(pSockAddrStorage[1]),
                 &(pConnectRequest->localAddressAndPort),
                 sizeof(SOCKADDR_STORAGE));

   /// WFP will take ownership of this memory and free it when the flow / redirection terminates
   pConnectRequest->localRedirectContext     = pSockAddrStorage;
   pConnectRequest->localRedirectContextSize = sizeof(SOCKADDR_STORAGE) * 2;

#endif /// (NTDDI_VERSION >= NTDDI_WIN8)

   pProtocolValue = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                              &FWPM_CONDITION_IP_PROTOCOL);
   if(pProtocolValue)
      ipProtocol = pProtocolValue->uint8;

   /// For non-TCP, this setting will not be enforced being that local redirection of this tuple is only 
   /// available during bind time. and ideally redirection should be performed using ALE_BIND_REDIRECT instead.
   if((*ppRedirectData)->pProxyData->flags & PCPDF_PROXY_LOCAL_ADDRESS)
      INETADDR_SET_ADDRESS((PSOCKADDR)&(pConnectRequest->localAddressAndPort),
                           (*ppRedirectData)->pProxyData->proxyLocalAddress.pBytes);

   /// For non-TCP, this setting will not be enforced being that local redirection of this tuple is only 
   /// available during bind time. and ideally redirection should be performed using ALE_BIND_REDIRECT instead.
   if((*ppRedirectData)->pProxyData->flags & PCPDF_PROXY_LOCAL_PORT)
      INETADDR_SET_PORT((PSOCKADDR)&(pConnectRequest->localAddressAndPort),
                        (*ppRedirectData)->pProxyData->proxyLocalPort);

   if((*ppRedirectData)->pProxyData->flags & PCPDF_PROXY_REMOTE_ADDRESS)
   {
      if((*ppRedirectData)->pProxyData->proxyToRemoteService)
         INETADDR_SET_ADDRESS((PSOCKADDR)&(pConnectRequest->remoteAddressAndPort),
                              (*ppRedirectData)->pProxyData->proxyRemoteAddress.pBytes);
      else
      {
         /// Ensure we don't need to worry about crossing any of the TCP/IP stack's zones
         if(INETADDR_ISANY((PSOCKADDR)&(pConnectRequest->localAddressAndPort)))
            INETADDR_SETLOOPBACK((PSOCKADDR)&(pConnectRequest->remoteAddressAndPort));
         else
            INETADDR_SET_ADDRESS((PSOCKADDR)&(pConnectRequest->remoteAddressAndPort),
                                 INETADDR_ADDRESS((PSOCKADDR)&(pConnectRequest->localAddressAndPort)));
      }
   }

   if((*ppRedirectData)->pProxyData->flags & PCPDF_PROXY_REMOTE_PORT)
      INETADDR_SET_PORT((PSOCKADDR)&(pConnectRequest->remoteAddressAndPort),
                        (*ppRedirectData)->pProxyData->proxyRemotePort);

   if(ipProtocol == IPPROTO_TCP)
      pConnectRequest->portReservationToken = (*ppRedirectData)->pProxyData->tcpPortReservationToken;
   else if(ipProtocol == IPPROTO_UDP)
      pConnectRequest->portReservationToken = (*ppRedirectData)->pProxyData->udpPortReservationToken;

   if((*ppRedirectData)->pProxyData->targetProcessID)
      pConnectRequest->localRedirectTargetPID = (*ppRedirectData)->pProxyData->targetProcessID;

#if(NTDDI_VERSION >= NTDDI_WIN8)

   HLPR_BAIL_LABEL:

   if(status != STATUS_SUCCESS)
   {
      actionType = FWP_ACTION_BLOCK;

      HLPR_DELETE_ARRAY(pSockAddrStorage,
                        WFPSAMPLER_CALLOUT_DRIVER_TAG);
   }

#endif /// (NTDDI_VERSION >= NTDDI_WIN8)

#pragma warning(push)
#pragma warning(disable: 6001) /// *ppRedirectData has already been initialized in previous call to KrnlHlprRedirectDataCreate

   (*ppRedirectData)->pClassifyOut->actionType = actionType;

   /// This will apply the modified data and cleanup the classify handle
   KrnlHlprRedirectDataDestroy(ppRedirectData);

#pragma warning(pop)

#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- PerformProxyConnectRedirection() [status:%#x]\n",
              status);

#endif /// DBG
   
   return status;
}

/**
 @private_function="ProxyByALERedirectDeferredProcedureCall"
 
   Purpose:  Invokes the appropriate private redirection routine to at DISPATCH_LEVEL.          <br>
                                                                                                <br>
   Notes:                                                                                       <br>
                                                                                                <br>
   MSDN_Ref: HTTP://MSDN.Microsoft.com/En-US/Library/Windows/Hardware/FF542972.aspx             <br>
*/
_IRQL_requires_(DISPATCH_LEVEL)
_IRQL_requires_same_
_Function_class_(KDEFERRED_ROUTINE)
VOID ProxyByALERedirectDeferredProcedureCall(_In_ KDPC* pDPC,
                                             _In_opt_ PVOID pContext,
                                             _In_opt_ PVOID pArg1,
                                             _In_opt_ PVOID pArg2)
{
#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> ProxyByALERedirectDeferredProcedureCall()\n");

#endif /// DBG

   UNREFERENCED_PARAMETER(pDPC);
   UNREFERENCED_PARAMETER(pContext);
   UNREFERENCED_PARAMETER(pArg2);

   NT_ASSERT(pDPC);
   NT_ASSERT(pArg1);
   NT_ASSERT(((DPC_DATA*)pArg1)->pClassifyData);
   NT_ASSERT(((DPC_DATA*)pArg1)->pInjectionData);

   DPC_DATA* pDPCData = (DPC_DATA*)pArg1;

   if(pDPCData)
   {
      NTSTATUS              status          = STATUS_SUCCESS;
      FWPS_INCOMING_VALUES* pClassifyValues = (FWPS_INCOMING_VALUES*)pDPCData->pClassifyData->pClassifyValues;
   
      if(pClassifyValues->layerId == FWPS_LAYER_ALE_CONNECT_REDIRECT_V4 ||
         pClassifyValues->layerId == FWPS_LAYER_ALE_CONNECT_REDIRECT_V6)
         status = PerformProxyConnectRedirection(&(pDPCData->pClassifyData),
                                                 &(pDPCData->pRedirectData));
      else if(pClassifyValues->layerId == FWPS_LAYER_ALE_BIND_REDIRECT_V4 ||
              pClassifyValues->layerId == FWPS_LAYER_ALE_BIND_REDIRECT_V6)
         status = PerformProxySocketRedirection(&(pDPCData->pClassifyData),
                                                &(pDPCData->pRedirectData));
   
      if(status != STATUS_SUCCESS)
         DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                    DPFLTR_ERROR_LEVEL,
                    " !!!! ProxyByALERedirectDeferredProcedureCall() [status: %#x]\n",
                    status);

      KrnlHlprDPCDataDestroy(&pDPCData);
   }

#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- ProxyByALERedirectDeferredProcedureCall()\n");

#endif /// DBG

   return;
}

/**
 @private_function="ProxyByALERedirectWorkItemRoutine"
 
   Purpose:                                                                                     <br>
                                                                                                <br>
   Notes:                                                                                       <br>
                                                                                                <br>
   MSDN_Ref:                                                                                    <br>
*/
_IRQL_requires_(PASSIVE_LEVEL)
_IRQL_requires_same_
_Function_class_(IO_WORKITEM_ROUTINE)
VOID ProxyByALERedirectWorkItemRoutine(_In_ PDEVICE_OBJECT pDeviceObject,
                                       _In_opt_ PVOID pContext)
{
#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> ProxyByALERedirectWorkItemRoutine()\n");

#endif /// DBG
   
   UNREFERENCED_PARAMETER(pDeviceObject);

   NT_ASSERT(pContext);
   NT_ASSERT((WORKITEM_DATA*)pContext);
   NT_ASSERT(((WORKITEM_DATA*)pContext)->pClassifyData);
   NT_ASSERT(((WORKITEM_DATA*)pContext)->pRedirectData);

   WORKITEM_DATA* pWorkItemData = (WORKITEM_DATA*)pContext;

   if(pWorkItemData)
   {
      NTSTATUS              status          = STATUS_SUCCESS;
      FWPS_INCOMING_VALUES* pClassifyValues = (FWPS_INCOMING_VALUES*)pWorkItemData->pClassifyData->pClassifyValues;

      if(pClassifyValues->layerId == FWPS_LAYER_ALE_CONNECT_REDIRECT_V4 ||
         pClassifyValues->layerId == FWPS_LAYER_ALE_CONNECT_REDIRECT_V6)
         status = PerformProxyConnectRedirection(&(pWorkItemData->pClassifyData),
                                                 &(pWorkItemData->pRedirectData));
      else if(pClassifyValues->layerId == FWPS_LAYER_ALE_BIND_REDIRECT_V4 ||
              pClassifyValues->layerId == FWPS_LAYER_ALE_BIND_REDIRECT_V6)
         status = PerformProxySocketRedirection(&(pWorkItemData->pClassifyData),
                                                &(pWorkItemData->pRedirectData));

      if(status != STATUS_SUCCESS)
         DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                    DPFLTR_ERROR_LEVEL,
                    " !!!! ProxyByALERedirectWorkItemRoutine() [status: %#x]\n",
                    status);

      KrnlHlprWorkItemDataDestroy(&pWorkItemData);
   }

#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- ProxyUsingInjectionMethodWorkItemRoutine()\n");

#endif /// DBG
   
   return;
}

/**
 @private_function="TriggerProxyByALERedirectInline"
 
   Purpose:                                                                                     <br>
                                                                                                <br>
   Notes:                                                                                       <br>
                                                                                                <br>
   MSDN_Ref:                                                                                    <br>
*/
_IRQL_requires_min_(PASSIVE_LEVEL)
_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_requires_same_
_Success_(return == STATUS_SUCCESS)
NTSTATUS TriggerProxyByALERedirectInline(_In_ const FWPS_INCOMING_VALUES* pClassifyValues,
                                         _In_ const FWPS_INCOMING_METADATA_VALUES* pMetadata,
                                         _Inout_ VOID* pLayerData,
                                         _In_opt_ const VOID* pClassifyContext,
                                         _In_ const FWPS_FILTER* pFilter,
                                         _In_ UINT64 flowContext,
                                         _Inout_ FWPS_CLASSIFY_OUT* pClassifyOut,
                                         _Inout_ REDIRECT_DATA** ppRedirectData)
{
#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> TriggerProxyByALERedirectInline()\n");

#endif /// DBG
   
   NT_ASSERT(pClassifyValues);
   NT_ASSERT(pMetadata);
   NT_ASSERT(pLayerData);
   NT_ASSERT(pFilter);
   NT_ASSERT(pClassifyOut);
   NT_ASSERT(ppRedirectData);
   NT_ASSERT(*ppRedirectData);

   NTSTATUS       status        = STATUS_SUCCESS;
   CLASSIFY_DATA* pClassifyData = 0;

   HLPR_NEW(pClassifyData,
            CLASSIFY_DATA,
            WFPSAMPLER_CALLOUT_DRIVER_TAG);
   HLPR_BAIL_ON_ALLOC_FAILURE(pClassifyData,
                              status);

   pClassifyData->pClassifyValues  = pClassifyValues;
   pClassifyData->pMetadataValues  = pMetadata;
   pClassifyData->pPacket          = pLayerData;
   pClassifyData->pClassifyContext = pClassifyContext;
   pClassifyData->pFilter          = pFilter;
   pClassifyData->flowContext      = flowContext;
   pClassifyData->pClassifyOut     = pClassifyOut;

   (*ppRedirectData)->pClassifyOut = pClassifyOut;

   if(pClassifyValues->layerId == FWPS_LAYER_ALE_CONNECT_REDIRECT_V4 ||
      pClassifyValues->layerId == FWPS_LAYER_ALE_CONNECT_REDIRECT_V6)
      status = PerformProxyConnectRedirection(&pClassifyData,
                                              ppRedirectData);
   else if(pClassifyValues->layerId == FWPS_LAYER_ALE_BIND_REDIRECT_V4 ||
           pClassifyValues->layerId == FWPS_LAYER_ALE_BIND_REDIRECT_V6)
      status = PerformProxySocketRedirection(&pClassifyData,
                                             ppRedirectData);

   HLPR_BAIL_LABEL:

   HLPR_DELETE(pClassifyData,
               WFPSAMPLER_CALLOUT_DRIVER_TAG);

#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- TriggerProxyByALERedirectInline() [status: %#x]\n",
              status);

#endif /// DBG
   
   return status;
}

/**
 @private_function="TriggerProxyByALERedirectOutOfBand"
 
   Purpose:                                                                                     <br>
                                                                                                <br>
   Notes:                                                                                       <br>
                                                                                                <br>
   MSDN_Ref:                                                                                    <br>
*/
_IRQL_requires_min_(PASSIVE_LEVEL)
_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_requires_same_
_Check_return_
_Success_(return == STATUS_SUCCESS)
NTSTATUS TriggerProxyByALERedirectOutOfBand(_In_ const FWPS_INCOMING_VALUES* pClassifyValues,
                                            _In_ const FWPS_INCOMING_METADATA_VALUES* pMetadata,
                                            _Inout_ VOID* pLayerData,
                                            _In_opt_ const VOID* pClassifyContext,
                                            _In_ const FWPS_FILTER* pFilter,
                                            _In_ UINT64 flowContext,
                                            _In_ FWPS_CLASSIFY_OUT* pClassifyOut,
                                            _Inout_ REDIRECT_DATA* pRedirectData)
{
#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> TriggerProxyByALERedirectOutOfBand()\n");

#endif /// DBG
   
   NT_ASSERT(pClassifyValues);
   NT_ASSERT(pMetadata);
   NT_ASSERT(pLayerData);
   NT_ASSERT(pFilter);
   NT_ASSERT(pClassifyOut);
   NT_ASSERT(pRedirectData);

   NTSTATUS       status        = STATUS_SUCCESS;
   CLASSIFY_DATA* pClassifyData = 0;

#pragma warning(push)
#pragma warning(disable: 6014) /// pClassifyData will be freed in completionFn using ProxyInjectionCompletionDataDestroy

   status = KrnlHlprClassifyDataCreateLocalCopy(&pClassifyData,
                                                pClassifyValues,
                                                pMetadata,
                                                pLayerData,
                                                pClassifyContext,
                                                pFilter,
                                                flowContext,
                                                pClassifyOut);
   HLPR_BAIL_ON_FAILURE(status);

#pragma warning(pop)

   status = FwpsPendClassify(pRedirectData->classifyHandle,
                             pFilter->filterId,
                             0,
                             pClassifyOut);
   if(status != STATUS_SUCCESS)
   {
      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_ERROR_LEVEL,
                 " !!!! TriggerProxyByALERedirectOutOfBand : FwpsPendClassify() [status: %#x]\n",
                 status);

      HLPR_BAIL;
   }

   pRedirectData->isPended     = TRUE;
   pRedirectData->pClassifyOut = pClassifyData->pClassifyOut;

   if(pRedirectData->pProxyData->useWorkItems)
      status = KrnlHlprWorkItemQueue(g_pWDMDevice,
                                     ProxyByALERedirectWorkItemRoutine,
                                     pClassifyData,
                                     pRedirectData,
                                     0);
   else if(pRedirectData->pProxyData->useThreadedDPC)
      status = KrnlHlprThreadedDPCQueue(ProxyByALERedirectDeferredProcedureCall,
                                        pClassifyData,
                                        pRedirectData,
                                        0);      
   else
      status = KrnlHlprDPCQueue(ProxyByALERedirectDeferredProcedureCall,
                                pClassifyData,
                                pRedirectData,
                                0);

   pClassifyOut->actionType = FWP_ACTION_BLOCK;

   HLPR_BAIL_LABEL:

   if(status != STATUS_SUCCESS)
   {
      if(pClassifyData)
         KrnlHlprClassifyDataDestroyLocalCopy(&pClassifyData);
   }

#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- TriggerProxyByALERedirectOutOfBand() [status: %#x]\n",
              status);

#endif /// DBG
   
   return status;
}

/**
 @classify_function="ClassifyProxyByALERedirect"
 
   Purpose:                                                                                     <br>
                                                                                                <br>
   Notes:    Applies to the following layers:                                                   <br>
                FWPM_LAYER_ALE_REDIRECT_BIND_V4                                                 <br>
                FWPM_LAYER_ALE_REDIRECT_BIND_V6                                                 <br>
                FWPM_LAYER_ALE_REDIRECT_CONNECT_V4                                              <br>
                FWPM_LAYER_ALE_REDIRECT_CONNECT_V6                                              <br>
                                                                                                <br>
             Microsoft recommends using FWPM_LAYER_STREAM_V{4/6} rather than proxying network 
             data to a local service.  Doing so will make for a better ecosystem, however if you 
             feel you must proxy, then it is advised to use 
             FWPM_LAYER_ALE_REDIRECT_CONNECT_V{4/6}, and have the proxy service call the 
             REDIRECT_RECORD IOCTLs so multiple proxies can coexst without losing data on the 
             origin of the data.                                                                <br>
                                                                                                <br>
   MSDN_Ref: HTTP://MSDN.Microsoft.com/En-US/Library/Windows/Hardware/FF551229.aspx             <br>
             HTTP://MSDN.Microsoft.com/En-US/Library/Windows/Hardware/FF544893.aspx             <br>
             HTTP://MSDN.Microsoft.com/En-US/Library/Windows/Hardware/FF571005.aspx             <br>
*/
_IRQL_requires_min_(PASSIVE_LEVEL)
_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_requires_same_
VOID ClassifyProxyByALERedirect(_In_ const FWPS_INCOMING_VALUES* pClassifyValues,
                                _In_ const FWPS_INCOMING_METADATA_VALUES* pMetadata,
                                _Inout_opt_ VOID* pLayerData,
                                _In_opt_ const VOID* pClassifyContext,
                                _In_ const FWPS_FILTER* pFilter,
                                _In_ UINT64 flowContext,
                                _Inout_ FWPS_CLASSIFY_OUT* pClassifyOut)
{
#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> ClassifyProxyByALERedirect()\n");

#endif /// DBG
   
   NT_ASSERT(pClassifyValues);
   NT_ASSERT(pMetadata);
   NT_ASSERT(pLayerData);
   NT_ASSERT(pClassifyContext);
   NT_ASSERT(pFilter);
   NT_ASSERT(pClassifyOut);
   NT_ASSERT(pClassifyValues->layerId == FWPS_LAYER_ALE_CONNECT_REDIRECT_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_ALE_CONNECT_REDIRECT_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_ALE_BIND_REDIRECT_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_ALE_BIND_REDIRECT_V6);
   NT_ASSERT(pFilter->providerContext);
   NT_ASSERT(pFilter->providerContext->type == FWPM_GENERAL_CONTEXT);
   NT_ASSERT(pFilter->providerContext->dataBuffer);
   NT_ASSERT(pFilter->providerContext->dataBuffer->size == sizeof(PC_PROXY_DATA));
   NT_ASSERT(pFilter->providerContext->dataBuffer->data);

   if(pLayerData &&
      pClassifyContext)
   {
      NTSTATUS status         = STATUS_SUCCESS;
      UINT32   conditionIndex = FWPS_FIELD_ALE_CONNECT_REDIRECT_V4_FLAGS;
      BOOLEAN  redirectSocket = FALSE;

      if(pClassifyValues->layerId == FWPS_LAYER_ALE_BIND_REDIRECT_V4 ||
         pClassifyValues->layerId == FWPS_LAYER_ALE_BIND_REDIRECT_V6)
      {
         conditionIndex = FWPS_FIELD_ALE_BIND_REDIRECT_V4_FLAGS;

         redirectSocket = TRUE;
      }

      if(pClassifyOut->rights & FWPS_RIGHT_ACTION_WRITE &&
         !(pClassifyValues->incomingValue[conditionIndex].value.uint32 & FWP_CONDITION_FLAG_IS_REAUTHORIZE))
      {
         REDIRECT_DATA* pRedirectData = 0;

#pragma warning(push)
#pragma warning(disable: 6014) /// pRedirectData will be freed in completionFn using ProxyInjectionCompletionDataDestroy

         status = KrnlHlprRedirectDataCreate(&pRedirectData,
                                             pClassifyContext,
                                             pFilter,
                                             pClassifyOut);
         HLPR_BAIL_ON_FAILURE(status);

#pragma warning(pop)

         /// Prevent infinite redirection
         if(redirectSocket)
         {
            UINT32 timesRedirected = 0;

            for(FWPS_BIND_REQUEST* pBindRequest = ((FWPS_BIND_REQUEST*)(pRedirectData->pWritableLayerData))->previousVersion;
                pBindRequest;
                pBindRequest = pBindRequest->previousVersion)
            {
               if(pBindRequest->modifierFilterId == pFilter->filterId)
                  timesRedirected++;

               /// Don't redirect the same socket more than 3 times
               if(timesRedirected > 3)
               {
                  status = STATUS_TOO_MANY_COMMANDS;

                  HLPR_BAIL;
               }
            }
         }
         else
         {
            UINT32 timesRedirected = 0;

            for(FWPS_CONNECT_REQUEST* pConnectRequest = ((FWPS_CONNECT_REQUEST*)(pRedirectData->pWritableLayerData))->previousVersion;
                pConnectRequest;
                pConnectRequest = pConnectRequest->previousVersion)
            {
               if(pConnectRequest->modifierFilterId == pFilter->filterId)
                  timesRedirected++;

               /// Don't redirect the same connection more than 3 times
               if(timesRedirected > 3)
               {
                  status = STATUS_TOO_MANY_COMMANDS;

                  HLPR_BAIL;
               }
            }
         }

         if(pRedirectData->pProxyData->performInline)
            status = TriggerProxyByALERedirectInline(pClassifyValues,
                                                     pMetadata,
                                                     pLayerData,
                                                     pClassifyContext,
                                                     pFilter,
                                                     flowContext,
                                                     pClassifyOut,
                                                     &pRedirectData);
         else
            status = TriggerProxyByALERedirectOutOfBand(pClassifyValues,
                                                        pMetadata,
                                                        pLayerData,
                                                        pClassifyContext,
                                                        pFilter,
                                                        flowContext,
                                                        pClassifyOut,
                                                        pRedirectData);

         HLPR_BAIL_LABEL:

         if(status != STATUS_SUCCESS)
         {
            DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                       DPFLTR_ERROR_LEVEL,
                       " !!!! ClassifyProxyByALERedirect() [status: %#x]\n",
                       status);

            if(pRedirectData)
               KrnlHlprRedirectDataDestroy(&pRedirectData);

            pClassifyOut->actionType = FWP_ACTION_BLOCK;
         }
      }
   }

#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- ClassifyProxyByALERedirect()\n");

#endif /// DBG
   
   return;
}

/**
 @classify_function="ClassifyProxyByInjection"
 
   Purpose:                                                                                     <br>
                                                                                                <br>
   Notes:                                                                                       <br>
                                                                                                <br>
   MSDN_Ref:                                                                                    <br>
*/
_IRQL_requires_min_(PASSIVE_LEVEL)
_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_requires_same_
VOID ClassifyProxyByInjection(_In_ const FWPS_INCOMING_VALUES* pClassifyValues,
                              _In_ const FWPS_INCOMING_METADATA_VALUES* pMetadata,
                              _Inout_opt_ VOID* pNetBufferList,
                              _In_opt_ const VOID* pClassifyContext,
                              _In_ const FWPS_FILTER* pFilter,
                              _In_ UINT64 flowContext,
                              _Inout_ FWPS_CLASSIFY_OUT* pClassifyOut)
{
#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> ClassifyProxyByInjection()\n");

#endif /// DBG
   
   NT_ASSERT(pClassifyValues);
   NT_ASSERT(pMetadata);
   NT_ASSERT(pNetBufferList);
   NT_ASSERT(pFilter);
   NT_ASSERT(pClassifyOut);
   NT_ASSERT(pClassifyValues->layerId == FWPS_LAYER_INBOUND_TRANSPORT_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_INBOUND_TRANSPORT_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_TRANSPORT_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_TRANSPORT_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_DATAGRAM_DATA_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_DATAGRAM_DATA_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_STREAM_PACKET_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_STREAM_PACKET_V6);
   NT_ASSERT(pFilter->providerContext);
   NT_ASSERT(pFilter->providerContext->type == FWPM_GENERAL_CONTEXT);
   NT_ASSERT(pFilter->providerContext->dataBuffer);
   NT_ASSERT(pFilter->providerContext->dataBuffer->data);
   NT_ASSERT(pFilter->providerContext->dataBuffer->size == sizeof(PC_PROXY_DATA));

   if(pNetBufferList)
   {
      NTSTATUS        status         = STATUS_SUCCESS;
      INJECTION_DATA* pInjectionData = 0;

      pClassifyOut->actionType = FWP_ACTION_CONTINUE;

#pragma warning(push)
#pragma warning(disable: 6014) /// pInjectionData will be freed in completionFn using ProxyInjectionCompletionDataDestroy

      status = KrnlHlprInjectionDataCreate(&pInjectionData,
                                           pClassifyValues,
                                           pMetadata,
                                           (NET_BUFFER_LIST*)pNetBufferList);
      HLPR_BAIL_ON_FAILURE(status);

#pragma warning(pop)

      if(pInjectionData->injectionState != FWPS_PACKET_INJECTED_BY_SELF &&
         pInjectionData->injectionState != FWPS_PACKET_PREVIOUSLY_INJECTED_BY_SELF)
      {
         PC_PROXY_DATA* pProxyData = (PPC_PROXY_DATA)pFilter->providerContext->dataBuffer->data;

         pClassifyOut->actionType  = FWP_ACTION_BLOCK;
         pClassifyOut->flags      |= FWPS_CLASSIFY_OUT_FLAG_ABSORB;

         if(pProxyData->performInline)
            status = TriggerProxyInjectionInline(pClassifyValues,
                                                 pMetadata,
                                                 pNetBufferList,
                                                 pClassifyContext,
                                                 pFilter,
                                                 flowContext,
                                                 pClassifyOut,
                                                 &pInjectionData,
                                                 pProxyData);
         else
            status = TriggerProxyInjectionOutOfBand(pClassifyValues,
                                                    pMetadata,
                                                    pNetBufferList,
                                                    pClassifyContext,
                                                    pFilter,
                                                    flowContext,
                                                    pClassifyOut,
                                                    pInjectionData,
                                                    pProxyData);
      }
      else
         DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                    DPFLTR_INFO_LEVEL,
                    "   -- ClassifyProxyByInjection() Injection previously performed.\n");

      HLPR_BAIL_LABEL:

      if(status != STATUS_SUCCESS)
      {
         DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                    DPFLTR_ERROR_LEVEL,
                    " !!!! ClassifyProxyByInjection() [status: %#x]\n",
                    status);

         if(pInjectionData)
            KrnlHlprInjectionDataDestroy(&pInjectionData);
      }
   }

#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- ClassifyProxyByInjection()\n");

#endif /// DBG
   
   return;
}

#else

/**
 @classify_function="ClassifyProxyByALE"
 
   Purpose:  Stub function for downlevel OS's trying to use uplevel functionality.              <br>
                                                                                                <br>
   Notes:                                                                                       <br>
                                                                                                <br>
   MSDN_Ref: HTTP://MSDN.Microsoft.com/En-US/Library/Windows/Hardware/FF551229.aspx             <br>
             HTTP://MSDN.Microsoft.com/En-US/Library/Windows/Hardware/FF544890.aspx             <br>
*/
_IRQL_requires_min_(PASSIVE_LEVEL)
_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_requires_same_
VOID ClassifyProxyByALERedirect(_In_ const FWPS_INCOMING_VALUES* pClassifyValues,
                                _In_ const FWPS_INCOMING_METADATA_VALUES* pMetadata,
                                _Inout_opt_ VOID* pLayerData,
                                _In_ const FWPS_FILTER* pFilter,
                                _In_ UINT64 flowContext,
                                _Inout_ FWPS_CLASSIFY_OUT* pClassifyOut)
{
#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> ClassifyProxyByALERedirect()\n");

#endif /// DBG
   
   NT_ASSERT(pClassifyValues);
   NT_ASSERT(pMetadata);
   NT_ASSERT(pLayerData);
   NT_ASSERT(pFilter);
   NT_ASSERT(pClassifyOut);

   UNREFERENCED_PARAMETER(pClassifyValues);
   UNREFERENCED_PARAMETER(pMetadata);
   UNREFERENCED_PARAMETER(pLayerData);
   UNREFERENCED_PARAMETER(pFilter);
   UNREFERENCED_PARAMETER(flowContext);
   UNREFERENCED_PARAMETER(pClassifyOut);

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_ERROR_LEVEL,
              " !!!! ClassifyProxyByALERedirect() : Method not supported prior to Windows 7 [status: %#x]\n",
              (UINT32)STATUS_UNSUCCESSFUL);

#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- ClassifyProxyByALERedirect()\n");

#endif /// DBG
   
   return;
};

/**
 @classify_function="ClassifyProxyByInjection"
 
   Purpose:                                                                                     <br>
                                                                                                <br>
   Notes:                                                                                       <br>
                                                                                                <br>
   MSDN_Ref:                                                                                    <br>
*/
_IRQL_requires_min_(PASSIVE_LEVEL)
_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_requires_same_
VOID ClassifyProxyByInjection(_In_ const FWPS_INCOMING_VALUES* pClassifyValues,
                              _In_ const FWPS_INCOMING_METADATA_VALUES* pMetadata,
                              _Inout_opt_ VOID* pNetBufferList,
                              _In_ const FWPS_FILTER* pFilter,
                              _In_ UINT64 flowContext,
                              _Inout_ FWPS_CLASSIFY_OUT* pClassifyOut)
{
#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> ClassifyProxyByInjection()\n");

#endif /// DBG
   
   NT_ASSERT(pClassifyValues);
   NT_ASSERT(pMetadata);
   NT_ASSERT(pNetBufferList);
   NT_ASSERT(pFilter);
   NT_ASSERT(pClassifyOut);
   NT_ASSERT(pClassifyValues->layerId == FWPS_LAYER_INBOUND_TRANSPORT_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_INBOUND_TRANSPORT_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_TRANSPORT_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_TRANSPORT_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_DATAGRAM_DATA_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_DATAGRAM_DATA_V6);
   NT_ASSERT(pFilter->providerContext);
   NT_ASSERT(pFilter->providerContext->type == FWPM_GENERAL_CONTEXT);
   NT_ASSERT(pFilter->providerContext->dataBuffer);
   NT_ASSERT(pFilter->providerContext->dataBuffer->data);
   NT_ASSERT(pFilter->providerContext->dataBuffer->size == sizeof(PC_PROXY_DATA));

   if(pNetBufferList)
   {
      NTSTATUS        status         = STATUS_SUCCESS;
      INJECTION_DATA* pInjectionData = 0;

      pClassifyOut->actionType = FWP_ACTION_CONTINUE;

#pragma warning(push)
#pragma warning(disable: 6014) /// pInjectionData will be freed in completionFn using ProxyInjectionCompletionDataDestroy

      status = KrnlHlprInjectionDataCreate(&pInjectionData,
                                           pClassifyValues,
                                           pMetadata,
                                           (NET_BUFFER_LIST*)pNetBufferList);
      HLPR_BAIL_ON_FAILURE(status);

#pragma warning(pop)

      if(pInjectionData->injectionState != FWPS_PACKET_INJECTED_BY_SELF &&
         pInjectionData->injectionState != FWPS_PACKET_PREVIOUSLY_INJECTED_BY_SELF)
      {
         PC_PROXY_DATA* pProxyData = (PPC_PROXY_DATA)pFilter->providerContext->dataBuffer->data;

         pClassifyOut->actionType  = FWP_ACTION_BLOCK;
         pClassifyOut->flags      |= FWPS_CLASSIFY_OUT_FLAG_ABSORB;

         if(pProxyData->performInline)
            status = TriggerProxyInjectionInline(pClassifyValues,
                                                 pMetadata,
                                                 pNetBufferList,
                                                 0,
                                                 pFilter,
                                                 flowContext,
                                                 pClassifyOut,
                                                 &pInjectionData,
                                                 pProxyData);
         else
            status = TriggerProxyInjectionOutOfBand(pClassifyValues,
                                                    pMetadata,
                                                    pNetBufferList,
                                                    0,
                                                    pFilter,
                                                    flowContext,
                                                    pClassifyOut,
                                                    pInjectionData,
                                                    pProxyData);
      }
      else
         DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                    DPFLTR_INFO_LEVEL,
                    "   -- ClassifyProxyByInjection() Injection previously performed.\n");

      HLPR_BAIL_LABEL:

      if(status != STATUS_SUCCESS)
      {
         DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                    DPFLTR_ERROR_LEVEL,
                    " !!!! ClassifyProxyByInjection() [status: %#x]\n",
                    status);

         if(pInjectionData)
            KrnlHlprInjectionDataDestroy(&pInjectionData);
      }
   }

#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- ClassifyProxyByInjection()\n");


#endif /// DBG
   
   return;
}

#endif /// (NTDDI_VERSION >= NTDDI_WIN7)
