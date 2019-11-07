////////////////////////////////////////////////////////////////////////////////////////////////////
//
//   Copyright (c) 2012 Microsoft Corporation.  All Rights Reserved.
//
//   Module Name:
//      Scenarios_Proxy.cpp
//
//   Abstract:
//      This module contains functions which implements the PROXY scenarios.
//
//   Naming Convention:
//
//      <Scope><Object><Action><Modifier>
//      <Scope><Object><Action>
//  
//      i.e.
//
//       <Scope>
//          {
//                          - Function is likely visible to other modules.
//            Prv           - Function is private to this module.
//          }
//       <Object>
//          {
//            ScenarioProxy - Function pertains to all of the Proxy Scenarios.
//            RPC           - Function is and RPC entry point.
//          }
//       <Action>
//          {
//            Add           - Function adds objects.
//            Remove        - Function removes objects.
//            Invoke        - Function implements the scenario based on parameters passed from the 
//                               commandline interface (WFPSampler.exe).
//          }
//       <Modifier>
//          {
//            FwpmObjects   - Function acts on WFP objects.
//            ScenarioProxy - Function pertains to all of the Proxy Scenarios.
//          }
//
//   Private Functions:
//      PrvScenarioProxyAddFwpmObjects(),
//      PrvScenarioProxyDeleteFwpmObjects(),
//      ScenarioProxyAdd(),
//      ScenarioProxyRemove(),
//
//   Public Functions:
//      RPCInvokeScenarioProxy(),
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

#include "Framework_WFPSamplerService.h" /// .

/**
 @private_function="PrvScenarioProxyDeleteFwpmObjects"
 
   Purpose:  Function that disables the SCENARIO_PROXY scenarios.                               <br>
                                                                                                <br>
   Notes:    Scenario removes the filters using specified filtering conditions at the specified 
             layer.  The associated callout and provider contexts are removed as well.          <br>
                                                                                                <br>
   MSDN_Ref:                                                                                    <br>
*/
_Success_(return == NO_ERROR)
UINT32 PrvScenarioProxyDeleteFwpmObjects(_In_ const FWPM_FILTER* pFilter)
{
   ASSERT(pFilter);

   UINT32                              status                      = NO_ERROR;
   HANDLE                              engineHandle                = 0;
   HANDLE                              enumHandle                  = 0;
   UINT32                              entryCount                  = 0;
   FWPM_FILTER**                       ppFilters                   = 0;
   FWPM_FILTER_ENUM_TEMPLATE           filterEnumTemplate          = {0};
   FWPM_PROVIDER_CONTEXT_ENUM_TEMPLATE providerContextEnumTemplate = {0};
   GUID                                calloutKey                  = {0};

#if(NTDDI_VERSION >= NTDDI_WIN7)

   if(pFilter->layerKey == FWPM_LAYER_ALE_CONNECT_REDIRECT_V4 ||
      pFilter->layerKey == FWPM_LAYER_ALE_CONNECT_REDIRECT_V6 ||
      pFilter->layerKey == FWPM_LAYER_ALE_BIND_REDIRECT_V4 ||
      pFilter->layerKey == FWPM_LAYER_ALE_BIND_REDIRECT_V6)
      calloutKey = WFPSAMPLER_CALLOUT_PROXY_BY_ALE_REDIRECT;
   else

#endif /// (NTDDI_VERSION >= NTDDI_WIN7)

   calloutKey          = WFPSAMPLER_CALLOUT_PROXY_BY_INJECTION;
   calloutKey.Data4[7] = HlprFwpmLayerGetIDByKey(&(pFilter->layerKey));                          /// Uniquely identifies the callout used

   providerContextEnumTemplate.providerContextType = FWPM_GENERAL_CONTEXT;

   filterEnumTemplate.providerKey             = (GUID*)&WFPSAMPLER_PROVIDER;
   filterEnumTemplate.layerKey                = pFilter->layerKey;
   filterEnumTemplate.enumType                = FWP_FILTER_ENUM_FULLY_CONTAINED;
   filterEnumTemplate.flags                   = FWP_FILTER_ENUM_FLAG_INCLUDE_BOOTTIME |
                                                FWP_FILTER_ENUM_FLAG_INCLUDE_DISABLED;
   filterEnumTemplate.numFilterConditions     = pFilter->numFilterConditions;
   filterEnumTemplate.filterCondition         = pFilter->filterCondition;
   filterEnumTemplate.providerContextTemplate = &providerContextEnumTemplate;
   filterEnumTemplate.actionMask              = FWP_ACTION_FLAG_CALLOUT;
   filterEnumTemplate.calloutKey              = &calloutKey;

   status = HlprFwpmEngineOpen(&engineHandle);
   HLPR_BAIL_ON_FAILURE(status);

   status = HlprFwpmFilterCreateEnumHandle(engineHandle,
                                           &filterEnumTemplate,
                                           &enumHandle);
   HLPR_BAIL_ON_FAILURE(status);

   status = HlprFwpmFilterEnum(engineHandle,
                               enumHandle,
                               0xFFFFFFFF,
                               &ppFilters,
                               &entryCount);
   HLPR_BAIL_ON_FAILURE(status);

   if(ppFilters)
   {
      for(UINT32 filterIndex = 0;
          filterIndex < entryCount;
          filterIndex++)
      {
         HlprFwpmFilterDeleteByKey(engineHandle,
                                   &(ppFilters[filterIndex]->filterKey));

         HlprFwpmCalloutDeleteByKey(engineHandle,
                                    &(ppFilters[filterIndex]->action.calloutKey));

         if(ppFilters[filterIndex]->flags & FWPM_FILTER_FLAG_HAS_PROVIDER_CONTEXT)
            HlprFwpmProviderContextDeleteByKey(engineHandle,
                                               &(ppFilters[filterIndex]->providerContextKey));
      }

      FwpmFreeMemory((void**)&ppFilters);
   }

   HLPR_BAIL_LABEL:

   if(engineHandle)
   {
      if(enumHandle)
         HlprFwpmFilterDestroyEnumHandle(engineHandle,
                                         &enumHandle);

      HlprFwpmEngineClose(&engineHandle);
   }

   return status;
}

/**
 @private_function="PrvScenarioProxyAddFwpmObjects"
 
   Purpose:  Function that enables the SCENARIO_PROXY scenarios.                                <br>
                                                                                                <br>
   Notes:    Scenario adds a filter using specified filtering conditions to the specified layer. 
             This filter is associated with WFPSampler's default sublayer and provider.  The 
             appropriate callout and provider context are added and associated with the filter. <br>
                                                                                                <br>
   MSDN_Ref:                                                                                    <br>
*/
_Success_(return == NO_ERROR)
UINT32 PrvScenarioProxyAddFwpmObjects(_In_ const FWPM_FILTER* pFilter,
                                      _In_ const PC_PROXY_DATA* pPCProxyData)
{
   ASSERT(pFilter);
   ASSERT(pPCProxyData);

   UINT32                status          = NO_ERROR;
   HANDLE                engineHandle    = 0;
   FWP_BYTE_BLOB         byteBlob        = {0};
   FWPM_PROVIDER_CONTEXT providerContext = {0};
   FWPM_CALLOUT          callout         = {0};
   FWPM_FILTER           filter          = {0};

   RtlCopyMemory(&filter,
                 pFilter,
                 sizeof(FWPM_FILTER));

   status = HlprGUIDPopulate(&(providerContext.providerContextKey));
   HLPR_BAIL_ON_FAILURE(status);

   providerContext.displayData.name        = L"WFPSampler's Proxy ProviderContext";
   providerContext.displayData.description = L"Instructs the driver where to proxy the socket or connection";      
   providerContext.providerKey             = (GUID*)&WFPSAMPLER_PROVIDER;
   providerContext.type                    = FWPM_GENERAL_CONTEXT;
   providerContext.dataBuffer              = &byteBlob;
   providerContext.dataBuffer->size        = sizeof(PC_PROXY_DATA);
   providerContext.dataBuffer->data        = (UINT8*)pPCProxyData;

#if(NTDDI_VERSION >= NTDDI_WIN7)

   if(pFilter->layerKey == FWPM_LAYER_ALE_CONNECT_REDIRECT_V4 ||
      pFilter->layerKey == FWPM_LAYER_ALE_CONNECT_REDIRECT_V6 ||
      pFilter->layerKey == FWPM_LAYER_ALE_BIND_REDIRECT_V4 ||
      pFilter->layerKey == FWPM_LAYER_ALE_BIND_REDIRECT_V6)
      callout.calloutKey = WFPSAMPLER_CALLOUT_PROXY_BY_ALE_REDIRECT;
   else

#endif /// (NTDDI_VERSION >= NTDDI_WIN7)

   callout.calloutKey              = WFPSAMPLER_CALLOUT_PROXY_BY_INJECTION;
   callout.calloutKey.Data4[7]     = HlprFwpmLayerGetIDByKey(&(filter.layerKey));                /// Uniquely identifies the callout used
   callout.displayData.name        = L"WFPSampler's Proxy Callout";
   callout.displayData.description = L"Proxies the socket or connection to the designated destination";
   callout.flags                   = FWPM_CALLOUT_FLAG_USES_PROVIDER_CONTEXT;
   callout.providerKey             = (GUID*)&WFPSAMPLER_PROVIDER;
   callout.applicableLayer         = filter.layerKey;

   status = HlprGUIDPopulate(&(filter.filterKey));
   HLPR_BAIL_ON_FAILURE(status);

   filter.flags             |= FWPM_FILTER_FLAG_HAS_PROVIDER_CONTEXT;
   filter.providerKey        = (GUID*)&WFPSAMPLER_PROVIDER;
   filter.subLayerKey        = WFPSAMPLER_SUBLAYER;
   filter.weight.type        = FWP_UINT8;
   filter.weight.uint8       = 0xF;
   filter.action.type        = FWP_ACTION_CALLOUT_UNKNOWN;
   filter.action.calloutKey  = callout.calloutKey;
   filter.providerContextKey = providerContext.providerContextKey;

   if(filter.flags & FWPM_FILTER_FLAG_BOOTTIME ||
      filter.flags & FWPM_FILTER_FLAG_PERSISTENT)
   {
      providerContext.flags |= FWPM_PROVIDER_CONTEXT_FLAG_PERSISTENT;

      callout.flags |= FWPM_CALLOUT_FLAG_PERSISTENT;
   }

   status = HlprFwpmEngineOpen(&engineHandle);
   HLPR_BAIL_ON_FAILURE(status);

   status = HlprFwpmTransactionBegin(engineHandle);
   HLPR_BAIL_ON_FAILURE(status);

   status = HlprFwpmProviderContextAdd(engineHandle,
                                       &providerContext);
   HLPR_BAIL_ON_FAILURE(status);

   status = HlprFwpmCalloutAdd(engineHandle,
                               &callout);
   HLPR_BAIL_ON_FAILURE(status);

   status = HlprFwpmFilterAdd(engineHandle,
                              &filter);
   HLPR_BAIL_ON_FAILURE(status);

   status = HlprFwpmTransactionCommit(engineHandle);
   HLPR_BAIL_ON_FAILURE(status);

   HLPR_BAIL_LABEL:

   if(engineHandle)
   {
      if(status != NO_ERROR)
         HlprFwpmTransactionAbort(engineHandle);

      HlprFwpmEngineClose(&engineHandle);
   }

   return status;
}

/**
 @scenario_function="ScenarioProxyRemove"
 
   Purpose:  Function that removes corresponding objects for a previously added SCENARIO_PROXY. <br>
                                                                                                <br>
   Notes:                                                                                       <br>
                                                                                                <br>
   MSDN_Ref:                                                                                    <br>
*/
_Success_(return == NO_ERROR)
UINT32 ScenarioProxyRemove(_In_ const FWPM_FILTER* pFilter)
{
   ASSERT(pFilter);

   return PrvScenarioProxyDeleteFwpmObjects(pFilter);
}

/**
 @scenario_function="ScenarioProxyAdd"
 
   Purpose:  Scenario which will proxy either a connection or a socket to a new endpoint.       <br>
                                                                                                <br>
   Notes:    Adds a filter which references one of the WFPSAMPLER_CALLOUT_PROXY callouts for 
             the provided layer.                                                                <br>
                                                                                                <br>
   MSDN_Ref:                                                                                    <br>
*/
_Success_(return == NO_ERROR)
UINT32 ScenarioProxyAdd(_In_ const FWPM_FILTER* pFilter,
                        _In_ const PC_PROXY_DATA* pPCProxyData)
{
   ASSERT(pFilter);
   ASSERT(pPCProxyData);

   return PrvScenarioProxyAddFwpmObjects(pFilter,
                                         pPCProxyData);
}

/**
 @rpc_function="RPCInvokeScenarioProxy"
 
   Purpose:  RPC exposed function used to dipatch the scenario routines for SCENARIO_PROXY.     <br>
                                                                                                <br>
   Notes:                                                                                       <br>
                                                                                                <br>
   MSDN_Ref:                                                                                    <br>
*/
/* [fault_status][comm_status] */
error_status_t RPCInvokeScenarioProxy(/* [in] */ handle_t rpcBindingHandle,
                                      /* [in] */ WFPSAMPLER_SCENARIO scenario,
                                      /* [in] */ FWPM_CHANGE_TYPE changeType,
                                      /* [ref][in] */ __RPC__in const FWPM_FILTER0* pFilter,
                                      /* [unique][in] */ __RPC__in_opt const PC_PROXY_DATA* pPCProxyData)
{
   UNREFERENCED_PARAMETER(rpcBindingHandle);
   UNREFERENCED_PARAMETER(scenario);

   ASSERT(pFilter);
   ASSERT(scenario == SCENARIO_PROXY);
   ASSERT(changeType < FWPM_CHANGE_TYPE_MAX);

   UINT32 status = NO_ERROR;

   if(changeType == FWPM_CHANGE_ADD)
   {
      if(pPCProxyData)
         status = ScenarioProxyAdd(pFilter,
                                   pPCProxyData);
      else
      {
         status = ERROR_INVALID_PARAMETER;

         HlprLogError(L"RPCInvokeScenarioProxy() [status: %#x][pPCProxyData: %#x]",
                      status,
                      pPCProxyData);
      }
   }
   else
      status = ScenarioProxyRemove(pFilter);

   return status;
}
