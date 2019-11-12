////////////////////////////////////////////////////////////////////////////////////////////////////
//
//   Copyright (c) 2012 Microsoft Corporation.  All Rights Reserved.
//
//   Module Name:
//      ClassifyFunctions_TransparentProxyCallouts.cpp
//
//   Abstract:
//      This module contains WFP Classify functions for transparently proxying connections.
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
//          TransparentProxy       -       Function demonstates use of 
//
//   Private Functions:
//
//   Public Functions:
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

#include "Framework_WFPSamplerCalloutDriver.h"            /// .
#include "ClassifyFunctions_TransparentProxyCallouts.tmh" /// $(OBJ_PATH)\$(O)\

#if(NTDDI_VERSION >= NTDDI_WIN8)

/**
 @private_function="PrvCloneAuthorizedNBLAndInject"
 
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
NTSTATUS PrvCloneNBLAndInject(_Inout_ CLASSIFY_DATA** ppClassifyData,
                              _Inout_ INJECTION_DATA** ppInjectionData)
{
#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> PrvCloneNBLAndInject()\n");

#endif /// DBG
   
   NT_ASSERT(ppClassifyData);
   NT_ASSERT(ppInjectionData);
   NT_ASSERT(*ppClassifyData);
   NT_ASSERT(*ppInjectionData);

   NTSTATUS                           status          = STATUS_SUCCESS;
   FWPS_INCOMING_VALUES*              pClassifyValues = (FWPS_INCOMING_VALUES*)(*ppClassifyData)->pClassifyValues;
   FWPS_INCOMING_METADATA_VALUES*     pMetadata       = (FWPS_INCOMING_METADATA_VALUES*)(*ppClassifyData)->pMetadataValues;
   COMPARTMENT_ID                     compartmentID   = UNSPECIFIED_COMPARTMENT_ID;
   UINT64                             endpointHandle  = 0;
   FWP_VALUE*                         pAddressValue   = 0;
   NET_BUFFER_LIST*                   pNetBufferList  = 0;
   TRANSPARENT_PROXY_COMPLETION_DATA* pCompletionData = 0;
   FWPS_TRANSPORT_SEND_PARAMS*        pSendParams     = 0;
   BYTE*                              pRemoteAddress  = 0;

#pragma warning(push)
#pragma warning(disable: 6014) /// pCompletionData & pSendArgs will be freed in completionFn using TransparentProxyCompletionDataDestroy

   HLPR_NEW(pCompletionData,
            TRANSPARENT_PROXY_COMPLETION_DATA,
            WFPSAMPLER_CALLOUT_DRIVER_TAG);
   HLPR_BAIL_ON_ALLOC_FAILURE(pCompletionData,
                              status);

   HLPR_NEW(pSendParams,
            FWPS_TRANSPORT_SEND_PARAMS,
            WFPSAMPLER_CALLOUT_DRIVER_TAG);
   HLPR_BAIL_ON_ALLOC_FAILURE(pSendParams,
                              status);

#pragma warning(pop)

   KeInitializeSpinLock(&(pCompletionData->spinLock));

   pCompletionData->performedInline = FALSE;
   pCompletionData->pClassifyData   = *ppClassifyData;
   pCompletionData->pInjectionData  = *ppInjectionData;
   pCompletionData->pSendParams     = pSendParams;

   /// Responsibility for freeing this memory has been transferred to the pCompletionData
   *ppClassifyData = 0;

   *ppInjectionData = 0;

   pSendParams = 0;

   if(FWPS_IS_METADATA_FIELD_PRESENT(pMetadata,
                                     FWPS_METADATA_FIELD_COMPARTMENT_ID))
      compartmentID = (COMPARTMENT_ID)pMetadata->compartmentId;

   if(FWPS_IS_METADATA_FIELD_PRESENT(pMetadata,
                                     FWPS_METADATA_FIELD_TRANSPORT_ENDPOINT_HANDLE))
      endpointHandle = pMetadata->transportEndpointHandle;

   /// NBL offset is at the Transport Header, and no IP Header is present yet...
   status = FwpsAllocateCloneNetBufferList((NET_BUFFER_LIST*)pCompletionData->pClassifyData->pPacket,
                                           g_pNDISPoolData->nblPoolHandle,
                                           g_pNDISPoolData->nbPoolHandle,
                                           0,
                                           &pNetBufferList);
   if(status != STATUS_SUCCESS)
   {
      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_ERROR_LEVEL,
                 " !!!! PrvCloneNBLAndInject : FwpsAllocateCloneNetBufferList() [status: %#x]\n",
                 status);

      HLPR_BAIL;
   }

   pAddressValue = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                             &FWPM_CONDITION_IP_REMOTE_ADDRESS);
   if(pAddressValue)
   {
      if(pCompletionData->pInjectionData->addressFamily == AF_INET)
      {
         UINT32 tempAddress = htonl(pAddressValue->uint32);

#pragma warning(push)
#pragma warning(disable: 6014) /// pRemoteAddress will be freed in completionFn using TransparentProxyCompletionDataDestroy

         HLPR_NEW_ARRAY(pRemoteAddress,
                        BYTE,
                        IPV4_ADDRESS_SIZE,
                        WFPSAMPLER_CALLOUT_DRIVER_TAG);
         HLPR_BAIL_ON_ALLOC_FAILURE(pRemoteAddress,
                                    status);

#pragma warning(pop)

         RtlCopyMemory(pRemoteAddress,
                       &tempAddress,
                       IPV4_ADDRESS_SIZE);
      }
      else
      {

#pragma warning(push)
#pragma warning(disable: 6014) /// pRemoteAddress will be freed in completionFn using TransparentProxyCompletionDataDestroy

         HLPR_NEW_ARRAY(pRemoteAddress,
                        BYTE,
                        IPV6_ADDRESS_SIZE,
                        WFPSAMPLER_CALLOUT_DRIVER_TAG);
         HLPR_BAIL_ON_ALLOC_FAILURE(pRemoteAddress,
                                    status);

#pragma warning(pop)

         RtlCopyMemory(pRemoteAddress,
                       pAddressValue->byteArray16->byteArray16,
                       IPV6_ADDRESS_SIZE);

         if(FWPS_IS_METADATA_FIELD_PRESENT(pMetadata,
                                           FWPS_METADATA_FIELD_REMOTE_SCOPE_ID))
            pCompletionData->pSendParams->remoteScopeId = pMetadata->remoteScopeId;
      }

      pCompletionData->pSendParams->remoteAddress = pRemoteAddress;
   }

   pCompletionData->refCount = KrnlHlprNBLGetRequiredRefCount(pNetBufferList);

   status = FwpsInjectTransportSendAsync(pCompletionData->pInjectionData->injectionHandle,
                                         pCompletionData->pInjectionData->injectionContext,
                                         endpointHandle,
                                         0,
                                         pCompletionData->pSendParams,
                                         pCompletionData->pInjectionData->addressFamily,
                                         compartmentID,
                                         pNetBufferList,
                                         CompleteTransparentProxy,
                                         pCompletionData);

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
         TransparentProxyCompletionDataDestroy(&pCompletionData,
                                               TRUE);
   }

#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- PrvCloneNBLAndInject() [status: %#x]\n",
              status);

#endif /// DBG
   
   return status;
}

/**
 @private_function="PerformTransparentProxyPendConnect"
 
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
NTSTATUS PerformTransparentProxyPendConnect(_Inout_ CLASSIFY_DATA** ppClassifyData,
                                            _Inout_ PEND_DATA** ppPendData,
                                            _Inout_ INJECTION_DATA** ppInjectionData)
{
#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> PerformTransparentProxyPendConnect()\n");

#endif /// DBG
   
   NT_ASSERT(ppClassifyData);
   NT_ASSERT(ppPendData);
   NT_ASSERT(ppInjectionData);
   NT_ASSERT(*ppClassifyData);
   NT_ASSERT(*ppPendData);
   NT_ASSERT(*ppInjectionData);

   NTSTATUS                             status                = STATUS_SUCCESS;
   const FWPS_INCOMING_METADATA_VALUES* pMetadata             = (*ppClassifyData)->pMetadataValues;
   HANDLE                               redirectRecordsHandle = 0;
   BYTE*                                pRedirectRecords      = 0;
   const SIZE_T                         BUFFER_SIZE           = 2048;
   SIZE_T                               redirectRecordsSize   = 0;

   if(FWPS_IS_METADATA_FIELD_PRESENT(pMetadata,
                                     FWPS_METADATA_FIELD_REDIRECT_RECORD_HANDLE))
      redirectRecordsHandle = pMetadata->redirectRecords;

   if(redirectRecordsHandle)
   {
      status = FwpsQueryConnectionSioFormatRedirectRecords(redirectRecordsHandle,
                                                           pRedirectRecords,
                                                           BUFFER_SIZE,
                                                           &redirectRecordsSize);
      if(status != STATUS_SUCCESS)
      {
         DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                    DPFLTR_ERROR_LEVEL,
                    " !!!! PerformTransparentProxyPendConnect : FwpsRedirectHandleCreate() [status: %#x]\n",
                    status);

         HLPR_BAIL;
      }

      /// At this point, we need to send the Redirect Records to the proxy service
   }

#pragma warning(push)
#pragma warning(disable: 6001) /// *ppPendData initialized prior to call to this function

   /// Completes the Pend
   KrnlHlprPendDataDestroy(ppPendData);

#pragma warning(pop)

   status = PrvCloneNBLAndInject(ppClassifyData,
                                 ppInjectionData);
   if(status != STATUS_SUCCESS)
      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_ERROR_LEVEL,
                 " !!!! PerformTransparentProxyPendConnect : PrvCloneNBLAndInject() [status: %#x]\n",
                 status);

   HLPR_BAIL_LABEL:

   if(status != STATUS_SUCCESS)
   {
#pragma warning(push)
#pragma warning(disable: 6001) /// *ppClassifyData & *ppPendData initialized prior to call to this function

      KrnlHlprClassifyDataDestroyLocalCopy(ppClassifyData);

      if(*ppPendData)
         KrnlHlprPendDataDestroy(ppPendData);

#pragma warning(pop)
   }

#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- PerformPendAuthorization() [status: %#x]\n",
              status);

#endif /// DBG
   
   return status;
};

/**
 @private_function="TransparentProxyPendConnectDeferredProcedureCall"
 
   Purpose:                                                                                     <br>
                                                                                                <br>
   Notes:                                                                                       <br>
                                                                                                <br>
   MSDN_Ref: HTTP://MSDN.Microsoft.com/En-US/Library/Windows/Hardware/FF542972.aspx             <br>
*/
_IRQL_requires_(DISPATCH_LEVEL)
_IRQL_requires_same_
_Function_class_(KDEFERRED_ROUTINE)
VOID TransparentProxyPendConnectDeferredProcedureCall(_In_ KDPC* pDPC,
                                                      _In_opt_ PVOID pContext,
                                                      _In_opt_ PVOID pArg1,
                                                      _In_opt_ PVOID pArg2)
{
#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> TransparentProxyPendConnectDeferredProcedureCall()\n");

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
      NTSTATUS status = STATUS_SUCCESS;

      status = PerformTransparentProxyPendConnect(&(pDPCData->pClassifyData),
                                                  &(pDPCData->pPendData),
                                                  (INJECTION_DATA**)&(pDPCData->pContext));
      if(status != STATUS_SUCCESS)
      {
         if(pDPCData->pClassifyData)
            KrnlHlprClassifyDataDestroyLocalCopy(&(pDPCData->pClassifyData));

         if(pDPCData->pContext)
            KrnlHlprInjectionDataDestroy((INJECTION_DATA**)&(pDPCData->pContext));

         DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                    DPFLTR_ERROR_LEVEL,
                    " !!!! TransparentProxyPendConnectDeferredProcedureCall() [status: %#x]\n",
                    status);
      }

      KrnlHlprDPCDataDestroy(&pDPCData);
   }

#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- TransparentProxyPendConnectDeferredProcedureCall()\n");

#endif /// DBG
   
   return;
}


/**
 @private_function="TransparentProxyPendConnectWorkItemRoutine"
 
   Purpose:                                                                                     <br>
                                                                                                <br>
   Notes:                                                                                       <br>
                                                                                                <br>
   MSDN_Ref:                                                                                    <br>
*/
_IRQL_requires_(PASSIVE_LEVEL)
_IRQL_requires_same_
_Function_class_(IO_WORKITEM_ROUTINE)
VOID TransparentProxyPendConnectWorkItemRoutine(_In_ PDEVICE_OBJECT pDeviceObject,
                                                _In_opt_ PVOID pContext)
{
#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> TransparentProxyPendConnectWorkItemRoutine()\n");

#endif /// DBG
   
   UNREFERENCED_PARAMETER(pDeviceObject);

   NT_ASSERT(pContext);
   NT_ASSERT(((WORKITEM_DATA*)pContext)->pClassifyData);
   NT_ASSERT(((WORKITEM_DATA*)pContext)->pInjectionData);

   WORKITEM_DATA* pWorkItemData = (WORKITEM_DATA*)pContext;

   if(pWorkItemData)
   {
      NTSTATUS status = STATUS_SUCCESS;

      status = PerformTransparentProxyPendConnect(&(pWorkItemData->pClassifyData),
                                                  &(pWorkItemData->pPendData),
                                                  (INJECTION_DATA**)&(pWorkItemData->pContext));
      if(status != STATUS_SUCCESS)
      {
         if(pWorkItemData->pClassifyData)
            KrnlHlprClassifyDataDestroyLocalCopy(&(pWorkItemData->pClassifyData));

         if(pWorkItemData->pContext)
            KrnlHlprInjectionDataDestroy((INJECTION_DATA**)&(pWorkItemData->pContext));

         DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                    DPFLTR_ERROR_LEVEL,
                    " !!!! TransparentProxyPendConnectWorkItemRoutine() [status: %#x]\n",
                    status);
      }

      KrnlHlprWorkItemDataDestroy(&pWorkItemData);
   }

#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- TransparentProxyPendConnectWorkItemRoutine()\n");

#endif /// DBG
   
   return;
}


/**
 @private_function="TriggerTransparentProxyPendConnectOutOfBand"
 
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
NTSTATUS TriggerTransparentProxyPendConnectOutOfBand(_In_ const FWPS_INCOMING_VALUES* pClassifyValues,
                                                     _In_ const FWPS_INCOMING_METADATA_VALUES* pMetadata,
                                                     _Inout_opt_ VOID* pLayerData,
                                                     _In_opt_ const VOID* pClassifyContext,
                                                     _In_ const FWPS_FILTER* pFilter,
                                                     _In_ UINT64 flowContext,
                                                     _In_ FWPS_CLASSIFY_OUT* pClassifyOut,
                                                     _Inout_ INJECTION_DATA* pInjectionData,
                                                     _Inout_ PEND_DATA* pPendData,
                                                     _In_ PC_PROXY_DATA* pPCData)
{
#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> TriggerTransparentProxyPendConnectOutOfBand()\n");

#endif /// DBG

   NT_ASSERT(pClassifyValues);
   NT_ASSERT(pMetadata);
   NT_ASSERT(pFilter);
   NT_ASSERT(pClassifyOut);
   NT_ASSERT(pInjectionData);
   NT_ASSERT(pPendData);
   NT_ASSERT(pPCData);
 
   NTSTATUS       status        = STATUS_SUCCESS;
   CLASSIFY_DATA* pClassifyData = 0;

#pragma warning(push)
#pragma warning(disable: 6014) /// pInjectionData will be freed in completionFn using PendAuthorizationCompletionDataDestroy

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

   if(pPCData->useWorkItems)
      status = KrnlHlprWorkItemQueue(g_pWDMDevice,
                                     TransparentProxyPendConnectWorkItemRoutine,
                                     pClassifyData,
                                     pPendData,
                                     (VOID*)pInjectionData);
   else if(pPCData->useThreadedDPC)
      status = KrnlHlprThreadedDPCQueue(TransparentProxyPendConnectDeferredProcedureCall,
                                        pClassifyData,
                                        pPendData,
                                        pInjectionData);
   else
      status = KrnlHlprDPCQueue(TransparentProxyPendConnectDeferredProcedureCall,
                                pClassifyData,
                                pPendData,
                                pInjectionData);

   HLPR_BAIL_ON_FAILURE(status);

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
              " <--- TriggerTransparentProxyPendConnectOutOfBand() [status: %#x]\n",
              status);

#endif /// DBG
   
   return status;
}

/**
 @classify_function="ClassifyTransparentProxyPendConnect"
 
   Purpose:                                                                                     <br>
                                                                                                <br>
   Notes:                                                                                       <br>
                                                                                                <br>
   MSDN_Ref:                                                                                    <br>
*/
_IRQL_requires_min_(PASSIVE_LEVEL)
_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_requires_same_
VOID ClassifyTransparentProxyPendConnect(_In_ const FWPS_INCOMING_VALUES* pClassifyValues,
                                         _In_ const FWPS_INCOMING_METADATA_VALUES* pMetadata,
                                         _Inout_opt_ VOID* pNetBufferList,
                                         _In_opt_ const VOID* pClassifyContext,
                                         _In_ const FWPS_FILTER2* pFilter,
                                         _In_ UINT64 flowContext,
                                         _Inout_ FWPS_CLASSIFY_OUT* pClassifyOut)
{
#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> ClassifyTransparentProxyPendConnect()\n");

#endif /// DBG
   
   NT_ASSERT(pClassifyValues);
   NT_ASSERT(pMetadata);
   NT_ASSERT(pNetBufferList);
   NT_ASSERT(pFilter);
   NT_ASSERT(pClassifyOut);
   NT_ASSERT(pClassifyValues->layerId == FWPS_LAYER_ALE_AUTH_CONNECT_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_ALE_AUTH_CONNECT_V6);
   NT_ASSERT(pFilter->providerContext);
   NT_ASSERT(pFilter->providerContext->type == FWPM_GENERAL_CONTEXT);
   NT_ASSERT(pFilter->providerContext->dataBuffer);
   NT_ASSERT(pFilter->providerContext->dataBuffer->size == sizeof(PC_PROXY_DATA));
   NT_ASSERT(pFilter->providerContext->dataBuffer->data);

   /// AUTH_CONNECT's completeOperation will trigger a Reauthorization
   if(KrnlHlprFwpsIncomingValueConditionFlagsAreSet(pClassifyValues,
                                                    FWP_CONDITION_FLAG_IS_REAUTHORIZE))
   {
      if(pClassifyOut->rights & FWPS_RIGHT_ACTION_WRITE)
         pClassifyOut->actionType = FWP_ACTION_PERMIT;
   }
   else
   {
      BOOLEAN actionSet = FALSE;

      if(pClassifyOut->rights & FWPS_RIGHT_ACTION_WRITE)
      {
         NTSTATUS        status         = STATUS_SUCCESS;
         INJECTION_DATA* pInjectionData = 0;
         PEND_DATA*      pPendData      = 0;

#pragma warning(push)
#pragma warning(disable: 6014) /// pInjectionData  & pPendData will be freed in completionFn using TransparentProxyCompletionDataDestroy

         status = KrnlHlprInjectionDataCreate(&pInjectionData,
                                              pClassifyValues,
                                              pMetadata,
                                              (NET_BUFFER_LIST*)pNetBufferList);
         HLPR_BAIL_ON_FAILURE(status);

         status = KrnlHlprPendDataCreate(&pPendData,
                                         pMetadata,
                                         (NET_BUFFER_LIST*)pNetBufferList,
                                         pFilter);
         HLPR_BAIL_ON_FAILURE(status);

#pragma warning(pop)

         status = TriggerTransparentProxyPendConnectOutOfBand(pClassifyValues,
                                                              pMetadata,
                                                              pNetBufferList,
                                                              pClassifyContext,
                                                              pFilter,
                                                              flowContext,
                                                              pClassifyOut,
                                                              pInjectionData,
                                                              pPendData,
                                                              (PC_PROXY_DATA*)pFilter->providerContext->dataBuffer);

         HLPR_BAIL_LABEL:

         if(status != STATUS_SUCCESS)
         {
            DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                       DPFLTR_ERROR_LEVEL,
                       " !!!! ClassifyTransparentProxyPendConnect() [status: %#x]\n",
                       status);

            if(pInjectionData)
               KrnlHlprInjectionDataDestroy(&pInjectionData);

            if(pPendData)
               KrnlHlprPendDataDestroy(&pPendData);
         }

         if(!actionSet)
         {
            pClassifyOut->actionType  = FWP_ACTION_BLOCK;
            pClassifyOut->flags      |= FWPS_CLASSIFY_OUT_FLAG_ABSORB;
         }
      }
   }

#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- ClassifyTransparentProxyPendConnect()\n");

#endif /// DBG
   
   return;
}

#endif /// (NTDDI_VERSION >= NTDDI_WIN8)
