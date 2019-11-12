////////////////////////////////////////////////////////////////////////////////////////////////////
//
//   Copyright (c) 2012 Microsoft Corporation.  All Rights Reserved.
//
//   Module Name:
//      Scenarios_BasicPacketExamination.cpp
//
//   Abstract:
//      This module contains functions which prepares and sends data for the 
//         BASIC_PACKET_EXAMINATION scenario implementation.
//
//   Naming Convention:
//
//      <Scope><Object><Action><Modifier>
//  
//      i.e.
//
//       <Scope>
//          {
//                                           - Function is likely visible to other modules.
//          }
//       <Object>
//          {
//            BasicPacketExaminationScenario - Function pertains to the Basic Packet Examination.
//          }
//       <Action>
//          {
//            Execute                        - Function packages data and invokes RPC to the 
//                                                WFPSampler service.
//            Log                            - Function writes to the console.
//          }
//       <Modifier>
//          {
//            Help                           - Function provides context sensitive help for the 
//                                                scenario.
//          }
//
//   Private Functions:
//
//   Public Functions:
//      BasicPacketExaminationScenarioExecute(),
//      BasicPacketExaminationScenarioLogHelp(),
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

#include "Framework_WFPSampler.h" /// .

/**
 @scenario_function="BasicPacketExaminationScenarioExecute"

   Purpose:  Gather and package data neccessary to setup the BASIC_PACKET_EXAMINATION scenario, 
             then invoke RPC to implement the scenario in the WFPSampler service.               <br>
                                                                                                <br>
   Notes:                                                                                       <br>
                                                                                                <br>
   MSDN_Ref:                                                                                    <br>
*/
_Success_(return == NO_ERROR)
UINT32 BasicPacketExaminationScenarioExecute(_In_reads_(stringCount) PCWSTR* ppCLPStrings,
                                             _In_ UINT32 stringCount)
{
   ASSERT(ppCLPStrings);
   ASSERT(stringCount);

   UINT32       status         = NO_ERROR;
   BOOLEAN      removeScenario = FALSE;
   FWPM_FILTER* pFilter        = 0;

   status = HlprFwpmFilterCreate(&pFilter);
   HLPR_BAIL_ON_FAILURE(status);

   pFilter->displayData.name = L"WFPSampler's Basic Packet Examination Filter";

   HlprCommandLineParseForScenarioRemoval(ppCLPStrings,
                                          stringCount,
                                          &removeScenario);

   status = HlprCommandLineParseForFilterInfo(ppCLPStrings,
                                              stringCount,
                                              pFilter,
                                              removeScenario);
   HLPR_BAIL_ON_FAILURE(status);

   status = RPCInvokeScenarioBasicPacketExamination(wfpSamplerBindingHandle,
                                                    SCENARIO_BASIC_PACKET_EXAMINATION,
                                                    removeScenario ? FWPM_CHANGE_DELETE : FWPM_CHANGE_ADD,
                                                    pFilter);
   if(status != NO_ERROR)
      HlprLogError(L"BasicPacketExaminationScenarioExecute : RpcInvokeScenarioBasicPacketExamination() [status: %#x]",
                   status);
   else
      HlprLogInfo(L"BasicPacketExaminationScenarioExecute : RpcInvokeScenarioBasicPacketExamination() [status: %#x]",
                  status);

   HLPR_BAIL_LABEL:

   if(pFilter)
      HlprFwpmFilterDestroy(&pFilter);

   return status;
}

/**
 @public_function="BasicPacketExaminationScenarioLogHelp"
 
   Purpose:  Log usage information for the BASIC_PACKET_EXAMINATION scenario to the console.    <br>
                                                                                                <br>
   Notes:                                                                                       <br>
                                                                                                <br>
   MSDN_Ref:                                                                                    <br>
*/
VOID BasicPacketExaminationScenarioLogHelp()
{
   wprintf(L"\n\t\t -s     \t BASIC_PACKET_EXAMINATION");
   wprintf(L"\n\t\t -?     \t Receive usage information.");
   wprintf(L"\n\t\t -l     \t Specify the layer to perform the filtering. [Required]");
   wprintf(L"\n\t\t -r     \t Remove the scenario objects.");
   wprintf(L"\n\t\t -v     \t Make the filter volatile (non-persistent). [Optional]");
   wprintf(L"\n\t\t -b     \t Makes the objects available during boot time. [Optional]");
   wprintf(L"\n\t\t -ipla  \t Specify the IP_LOCAL_ADDRESS /");
   wprintf(L"\n\t\t        \t    IP_SOURCE_ADDRESS to filter. [Optional]");
   wprintf(L"\n\t\t -ipra  \t Specify the IP_REMOTE_ADDRESS /");
   wprintf(L"\n\t\t        \t    IP_DESTINATION_ADDRESS to filter. [Optional]");
   wprintf(L"\n\t\t -ipp   \t Specify the IP_PROTOCOL to filter. [Optional]");
   wprintf(L"\n\t\t -iplp  \t Specify the IP_LOCAL_PORT to filter. [Optional]");
   wprintf(L"\n\t\t -icmpt \t Specify the ICMP_TYPE to filter. [Optional]");
   wprintf(L"\n\t\t -iprp  \t Specify the IP_REMOTE_PORT to filter. [Optional]");
   wprintf(L"\n\t\t -icmpc \t Specify the ICMP_CODE to filter. [Optional]");
   wprintf(L"\n");
   wprintf(L"\n\t i.e.");
   wprintf(L"\n\t\t  WFPSampler.Exe -s BASIC_PACKET_EXAMINATION -l FWPM_LAYER_INBOUND_TRANSPORT_V4 -ipla 1.0.0.1 -ipra 1.0.0.254 -ipp TCP -v");
   wprintf(L"\n\t\t  WFPSampler.Exe -s BASIC_PACKET_EXAMINATION -l FWPM_LAYER_INBOUND_TRANSPORT_V4 -ipla 1.0.0.1 -ipra 1.0.0.254 -ipp TCP -v -r");
   wprintf(L"\n");

   return;
}
