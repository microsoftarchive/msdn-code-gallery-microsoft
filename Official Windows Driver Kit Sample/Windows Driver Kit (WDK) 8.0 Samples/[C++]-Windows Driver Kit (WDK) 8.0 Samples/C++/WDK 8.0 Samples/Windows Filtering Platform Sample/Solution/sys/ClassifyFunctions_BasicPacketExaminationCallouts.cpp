////////////////////////////////////////////////////////////////////////////////////////////////////
//
//   Copyright (c) 2012 Microsoft Corporation.  All Rights Reserved.
//
//   Module Name:
//      ClassifyFunctions_BasicPacketExaminationCallouts.cpp
//
//   Abstract:
//      This module contains WFP Classify functions for examining indicated NET_BUFFER_LISTS.
//
//   Naming Convention:
//
//      <Module><Scenario>
//  
//      i.e.
//
//       ClassifyBasicPacketExamination
//
//       <Module>
//          Classify               - Function is an FWPS_CALLOUT_CLASSIFY_FN.
//       <Scenario>
//          BasicPacketExamination - Function demonstates examining classified packets.
//
//   Private Functions:
//
//   Public Functions:
//      ClassifyBasicPacketExamination(),
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
#include "ClassifyFunctions_BasicPacketExaminationCallouts.tmh" /// $(OBJ_PATH)\$(O)\

#define MAX_STRING_SIZE 1800

/**
 @private_function="LogEthernetIIHeader"
 
   Purpose:  Logs the Ethernet II Header into a more easily readable format.                    <br>
                                                                                                <br>
   Notes:    Uses ETW Tracing for the logging, which is not ideal in a real world scenario.     <br>
                                                                                                <br>
   MSDN_Ref: HTTP://MSDN.Microsoft.com/En-US/Library/FF562859.aspx                              <br>
*/
VOID LogEthernetIIHeader(_In_ ETHERNET_II_HEADER* pEthernetIIHeader,
                         _In_ UINT32 vlanID = 0) 
{
#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> LogEthernetIIHeader()\n");

#endif /// DBG

   NT_ASSERT(pEthernetIIHeader);

   NTSTATUS status  = STATUS_SUCCESS;
   PSTR     pString = 0;

   HLPR_NEW_ARRAY(pString,
                  CHAR,
                  MAX_STRING_SIZE,
                  WFPSAMPLER_CALLOUT_DRIVER_TAG);
   HLPR_BAIL_ON_ALLOC_FAILURE(pString,
                              status);

   if(vlanID)
      status = RtlStringCchPrintfA(pString,
                                   MAX_STRING_SIZE,
                                   " 0                   1                   2\n"
                                   " 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3\n"
                                   "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n"
                                   "|                                               |\n"
                                   "+            Destination MAC Address            +\n"
                                   "|                                               |\n"
                                   "|               %02x:%02x:%02x:%02x:%02x:%02x              |\n"
                                   "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n"
                                   "|                                               |\n"
                                   "+               Source MAC Address              +\n"
                                   "|                                               |\n"
                                   "|               %02x:%02x:%02x:%02x:%02x:%02x              |\n"
                                   "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n"
                                   "|                    VLAN ID                    >"
                                   "|                                               |\n"
                                   "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n"
                                   "|VLAN ID (cont.)|              Type             |\n"
                                   "|               |                               |\n"
                                   "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n",
                                   pEthernetIIHeader->pDestinationAddress[0],
                                   pEthernetIIHeader->pDestinationAddress[1],
                                   pEthernetIIHeader->pDestinationAddress[2],
                                   pEthernetIIHeader->pDestinationAddress[3],
                                   pEthernetIIHeader->pDestinationAddress[4],
                                   pEthernetIIHeader->pDestinationAddress[5],
                                   pEthernetIIHeader->pSourceAddress[0],
                                   pEthernetIIHeader->pSourceAddress[1],
                                   pEthernetIIHeader->pSourceAddress[2],
                                   pEthernetIIHeader->pSourceAddress[3],
                                   pEthernetIIHeader->pSourceAddress[4],
                                   pEthernetIIHeader->pSourceAddress[5]);
   else
      status = RtlStringCchPrintfA(pString,
                                   MAX_STRING_SIZE,
                                   " 0                   1                   2\n"
                                   " 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3\n"
                                   "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n"
                                   "|                                               |\n"
                                   "+            Destination MAC Address            +\n"
                                   "|                                               |\n"
                                   "|               %02x:%02x:%02x:%02x:%02x:%02x              |\n"
                                   "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n"
                                   "|                                               |\n"
                                   "+               Source MAC Address              +\n"
                                   "|                                               |\n"
                                   "|               %02x:%02x:%02x:%02x:%02x:%02x              |\n"
                                   "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n"
                                   "|              Type             |    Data...    |\n"
                                   "|              %04x             |               |\n"
                                   "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n",
                                   pEthernetIIHeader->pDestinationAddress[0],
                                   pEthernetIIHeader->pDestinationAddress[1],
                                   pEthernetIIHeader->pDestinationAddress[2],
                                   pEthernetIIHeader->pDestinationAddress[3],
                                   pEthernetIIHeader->pDestinationAddress[4],
                                   pEthernetIIHeader->pDestinationAddress[5],
                                   pEthernetIIHeader->pSourceAddress[0],
                                   pEthernetIIHeader->pSourceAddress[1],
                                   pEthernetIIHeader->pSourceAddress[2],
                                   pEthernetIIHeader->pSourceAddress[3],
                                   pEthernetIIHeader->pSourceAddress[4],
                                   pEthernetIIHeader->pSourceAddress[5],
                                   pEthernetIIHeader->type);

   if(status == STATUS_SUCCESS)
      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_INFO_LEVEL,
                 "Ethernet II Header:\n%s",
                 pString);

   HLPR_BAIL_LABEL:

   HLPR_DELETE_ARRAY(pString,
                     WFPSAMPLER_CALLOUT_DRIVER_TAG);

#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- LogEthernetIIHeader [status: %#x]\n",
              status);

#else

   /// Used to get around preFast Warning 28931
   UNREFERENCED_PARAMETER(status);

#endif /// DBG

   return;
}

/**
 @private_function="LogEthernetSNAPHeader"
 
   Purpose:  Logs the MAC Ethernet SNAP Header into a more easily readable format.              <br>
                                                                                                <br>
   Notes:    Uses ETW Tracing for the logging, which is not ideal in a real world scenario.     <br>
                                                                                                <br>
   MSDN_Ref: HTTP://MSDN.Microsoft.com/En-US/Library/FF562859.aspx                              <br>
*/
VOID LogEthernetSNAPHeader(_In_ ETHERNET_SNAP_HEADER* pEthernetSNAPHeader)
{
#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> LogEthernetSNAPHeader()\n");

#endif /// DBG

   NT_ASSERT(pEthernetSNAPHeader);

   NTSTATUS status  = STATUS_SUCCESS;
   PSTR     pString = 0;

   HLPR_NEW_ARRAY(pString,
                  CHAR,
                  MAX_STRING_SIZE,
                  WFPSAMPLER_CALLOUT_DRIVER_TAG);
   HLPR_BAIL_ON_ALLOC_FAILURE(pString,
                              status);

   status = RtlStringCchPrintfA(pString,
                                MAX_STRING_SIZE,
                                " 0                   1                   2\n"
                                " 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3\n"
                                "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n"
                                "|                                               |\n"
                                "+            Destination MAC Address            +\n"
                                "|                                               |\n"
                                "|               %02x:%02x:%02x:%02x:%02x:%02x              |\n"
                                "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n"
                                "|                                               |\n"
                                "+               Source MAC Address              +\n"
                                "|                                               |\n"
                                "|               %02x:%02x:%02x:%02x:%02x:%02x              |\n"
                                "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n"
                                "|            Length             |      DSAP     |\n"
                                "|              %04x             |       %02x      |\n"
                                "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n"
                                "|      SSAP     | Control Byte  |    OUI ...    >\n"
                                "|       %02x      |       %02x      |       %02x      |\n"
                                "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n"
                                "<           OUI (cont.)         |    Type ...   >\n"
                                "<              %02x%02x             |       %04x      >\n"
                                "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n"
                                "<  Type (cont.) |            Data ...           |\n"
                                "<               |                               |\n"
                                "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n",
                                pEthernetSNAPHeader->pDestinationAddress[0],
                                pEthernetSNAPHeader->pDestinationAddress[1],
                                pEthernetSNAPHeader->pDestinationAddress[2],
                                pEthernetSNAPHeader->pDestinationAddress[3],
                                pEthernetSNAPHeader->pDestinationAddress[4],
                                pEthernetSNAPHeader->pDestinationAddress[5],
                                pEthernetSNAPHeader->pSourceAddress[0],
                                pEthernetSNAPHeader->pSourceAddress[1],
                                pEthernetSNAPHeader->pSourceAddress[2],
                                pEthernetSNAPHeader->pSourceAddress[3],
                                pEthernetSNAPHeader->pSourceAddress[4],
                                pEthernetSNAPHeader->pSourceAddress[5],
                                pEthernetSNAPHeader->length,
                                pEthernetSNAPHeader->destinationSAP,
                                pEthernetSNAPHeader->sourceSAP,
                                pEthernetSNAPHeader->controlByte,
                                pEthernetSNAPHeader->pOUI[0],
                                pEthernetSNAPHeader->pOUI[1],
                                pEthernetSNAPHeader->pOUI[2],
                                pEthernetSNAPHeader->type);
   if(status == STATUS_SUCCESS)
      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_INFO_LEVEL,
                 "Ethernet SNAP Header:\n%s",
                 pString);

   HLPR_BAIL_LABEL:

   HLPR_DELETE_ARRAY(pString,
                     WFPSAMPLER_CALLOUT_DRIVER_TAG);

#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- LogEthernetSNAPHeader() [status: %#x]\n",
              status);

#else

   /// Used to get around preFast Warning 28931
   UNREFERENCED_PARAMETER(status);

#endif /// DBG

   return;
}

/**
 @private_function="LogIPv4Header"
 
   Purpose:  Logs the IPv4 Header into a more easily readable format.                           <br>
                                                                                                <br>
   Notes:    Uses ETW Tracing for the logging, which is not ideal in a real world scenario.     <br>
                                                                                                <br>
   MSDN_Ref: HTTP://MSDN.Microsoft.com/En-US/Library/FF562859.aspx                              <br>
*/
VOID LogIPv4Header(_In_ IP_HEADER_V4* pIPv4Header)
{
#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> LogIPv4Header()\n");

#endif /// DBG

   NT_ASSERT(pIPv4Header);

   NTSTATUS status  = STATUS_SUCCESS;
   PSTR     pString = 0;

   HLPR_NEW_ARRAY(pString,
                  CHAR,
                  MAX_STRING_SIZE,
                  WFPSAMPLER_CALLOUT_DRIVER_TAG);
   HLPR_BAIL_ON_ALLOC_FAILURE(pString,
                              status);

   status = RtlStringCchPrintfA(pString,
                                MAX_STRING_SIZE,
                                " 0                   1                   2                   3\n"
                                " 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1\n"
                                "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n"
                                "|Version|  IHL  |Type of Service|         Total Length          |\n"
                                "|   %01x   |   %01x  |       %02x      |       %02x      |\n"
                                "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n"
                                "|        Identification         |Flags|     Fragment Offset     |\n"
                                "|              %04x             |              %04x             |\n"
                                "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n"
                                "|  Time to Live |    Protocol     |        Header Checksum        |\n"
                                "|       %02x      |       %02x      |              %04x             |\n"
                                "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n"
                                "|                        Source Address                         |\n"
                                "|                           %08x                           |\n"
                                "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n"
                                "|                      Destination Address                      |\n"
                                "|                           %08x                           |\n"
                                "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n"
                                "|                    Options                    |    Padding    |\n"
                                "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n",
                                pIPv4Header->version,
                                pIPv4Header->headerLength,
                                pIPv4Header->typeOfService,
                                pIPv4Header->totalLength,
                                ntohs(pIPv4Header->identification),
                                ntohs(pIPv4Header->flagsAndFragmentOffset),
                                pIPv4Header->timeToLive,
                                pIPv4Header->protocol,
                                ntohs(pIPv4Header->checksum),
                                ntohl((*((UINT32*)pIPv4Header->pSourceAddress))),
                                ntohl((*((UINT32*)pIPv4Header->pDestinationAddress))));
   if(status == STATUS_SUCCESS)
      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_INFO_LEVEL,
                 "IPv4 Header:\n%s",
                 pString);

   HLPR_BAIL_LABEL:

   HLPR_DELETE_ARRAY(pString,
                     WFPSAMPLER_CALLOUT_DRIVER_TAG);

#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- LogIPv4Header() [status: %#x]\n",
              status);

#else

   /// Used to get around preFast Warning 28931
   UNREFERENCED_PARAMETER(status);

#endif /// DBG

   return;
}

/**
 @private_function="LogIPv6Header"
 
   Purpose:  Logs the IPv6 Header into a more easily readable format.                           <br>
                                                                                                <br>
   Notes:    Uses ETW Tracing for the logging, which is not ideal in a real world scenario.     <br>
                                                                                                <br>
   MSDN_Ref: HTTP://MSDN.Microsoft.com/En-US/Library/FF562859.aspx                              <br>
*/
VOID LogIPv6Header(_In_ IP_HEADER_V6* pIPv6Header)
{
#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> LogIPv6Header()\n");

#endif /// DBG

   NT_ASSERT(pIPv6Header);

   NTSTATUS status  = STATUS_SUCCESS;
   PSTR     pString = 0;

   HLPR_NEW_ARRAY(pString,
                  CHAR,
                  MAX_STRING_SIZE,
                  WFPSAMPLER_CALLOUT_DRIVER_TAG);
   HLPR_BAIL_ON_ALLOC_FAILURE(pString,
                              status);

   status = RtlStringCchPrintfA(pString,
                                MAX_STRING_SIZE,
                                " 0                   1                   2                   3\n"
                                " 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1\n"
                                "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n"
                                "|Version| Traffic Class |              Flow Label               |\n"
                                "|   %01x   |                        %07x                        |\n"
                                "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n"
                                "|        Payload Length         |  Next Header  |   Hop Limit   |\n"
                                "|              %04x             |       %02x      |       %02x      |\n"
                                "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n"
                                "|                                                               |\n"
                                "+                                                               +\n"
                                "|                                                               |\n"
                                "+                        Source Address                         +\n"
                                "|                                                               |\n"
                                "+                                                               +\n"
                                "|                                                               |\n"
                                "|            %02x:%02x:%02x:%02x:%02x:%02x:%02x:%02x:%02x:%02x:%02x:%02x:%02x:%02x:%02x:%02x            |\n"
                                "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n"
                                "|                                                               |\n"
                                "+                                                               +\n"
                                "|                                                               |\n"
                                "+                      Destination Address                      +\n"
                                "|                                                               |\n"
                                "+                                                               +\n"
                                "|                                                               |\n"
                                "|            %02x:%02x:%02x:%02x:%02x:%02x:%02x:%02x:%02x:%02x:%02x:%02x:%02x:%02x:%02x:%02x            |\n"
                                "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n",
                                pIPv6Header->version.value,
                                ntohl((UINT32)pIPv6Header->pVersionTrafficClassAndFlowLabel) & 0x0FFFFFFF,
                                ntohs(pIPv6Header->payloadLength),
                                pIPv6Header->nextHeader,
                                pIPv6Header->hopLimit,
                                pIPv6Header->pSourceAddress[0],
                                pIPv6Header->pSourceAddress[1],
                                pIPv6Header->pSourceAddress[2],
                                pIPv6Header->pSourceAddress[3],
                                pIPv6Header->pSourceAddress[4],
                                pIPv6Header->pSourceAddress[5],
                                pIPv6Header->pSourceAddress[6],
                                pIPv6Header->pSourceAddress[7],
                                pIPv6Header->pSourceAddress[8],
                                pIPv6Header->pSourceAddress[9],
                                pIPv6Header->pSourceAddress[10],
                                pIPv6Header->pSourceAddress[11],
                                pIPv6Header->pSourceAddress[12],
                                pIPv6Header->pSourceAddress[13],
                                pIPv6Header->pSourceAddress[14],
                                pIPv6Header->pSourceAddress[15],
                                pIPv6Header->pDestinationAddress[0],
                                pIPv6Header->pDestinationAddress[1],
                                pIPv6Header->pDestinationAddress[2],
                                pIPv6Header->pDestinationAddress[3],
                                pIPv6Header->pDestinationAddress[4],
                                pIPv6Header->pDestinationAddress[5],
                                pIPv6Header->pDestinationAddress[6],
                                pIPv6Header->pDestinationAddress[7],
                                pIPv6Header->pDestinationAddress[8],
                                pIPv6Header->pDestinationAddress[9],
                                pIPv6Header->pDestinationAddress[10],
                                pIPv6Header->pDestinationAddress[11],
                                pIPv6Header->pDestinationAddress[12],
                                pIPv6Header->pDestinationAddress[13],
                                pIPv6Header->pDestinationAddress[14],
                                pIPv6Header->pDestinationAddress[15]);
   if(status == STATUS_SUCCESS)
      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_INFO_LEVEL,
                 "IPv6 Header:\n%s",
                 pString);

   HLPR_BAIL_LABEL:

   HLPR_DELETE_ARRAY(pString,
                     WFPSAMPLER_CALLOUT_DRIVER_TAG);

#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- LogIPv6Header [status: %#x]\n",
              status);
#else

   /// Used to get around preFast Warning 28931
   UNREFERENCED_PARAMETER(status);

#endif /// DBG

   return;
}

/**
 @private_function="LogIPHeader"
 
   Purpose:  Proxy IP Header logging to appropriate logging function base on IP version.        <br>
                                                                                                <br>
   Notes:                                                                                       <br>
                                                                                                <br>
   MSDN_Ref:                                                                                    <br>
*/
VOID LogIPHeader(_In_ VOID* pIPHeader,
                 _In_ UINT8 ipVersion)
{
   NT_ASSERT(pIPHeader);

   if(ipVersion == IPV4)
      LogIPv4Header((IP_HEADER_V4*)pIPHeader);
   else
      LogIPv6Header((IP_HEADER_V6*)pIPHeader);

   return;
}

/**
 @private_function="LogICMPv4Header"
 
   Purpose:  Logs the ICMPv4 Header into a more easily readable format.                         <br>
                                                                                                <br>
   Notes:    Uses ETW Tracing for the logging, which is not ideal in a real world scenario.     <br>
                                                                                                <br>
   MSDN_Ref: HTTP://MSDN.Microsoft.com/En-US/Library/FF562859.aspx                              <br>
*/
VOID LogICMPv4Header(_In_ ICMP_HEADER_V4* pICMPv4Header)
{
#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> LogICMPv4Header()\n");

#endif /// DBG

   NT_ASSERT(pICMPv4Header);

   NTSTATUS status  = STATUS_SUCCESS;
   PSTR     pString = 0;

   HLPR_NEW_ARRAY(pString,
                  CHAR,
                  MAX_STRING_SIZE,
                  WFPSAMPLER_CALLOUT_DRIVER_TAG);
   HLPR_BAIL_ON_ALLOC_FAILURE(pString,
                              status);

   status = RtlStringCchPrintfA(pString,
                                MAX_STRING_SIZE,
                                " 0                   1                   2                   3\n"
                                " 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1\n"
                                "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n"
                                "|     Type      |     Code      |           Checksum            |\n"
                                "|       %02x      |       %02x      |              %04x             |\n"
                                "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n"
                                "|              Variable (Dependent on Type / Code)              |\n"
                                "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n",
                                pICMPv4Header->type,
                                pICMPv4Header->code,
                                ntohs(pICMPv4Header->checksum));
   if(status == STATUS_SUCCESS)
      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_INFO_LEVEL,
                 "ICMPv4 Header:\n%s",
                 pString);

   HLPR_BAIL_LABEL:

   HLPR_DELETE_ARRAY(pString,
                     WFPSAMPLER_CALLOUT_DRIVER_TAG);

#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- LogICMPv4Header [status: %#x]\n",
              status);

#else

   /// Used to get around preFast Warning 28931
   UNREFERENCED_PARAMETER(status);

#endif /// DBG

   return;
}

/**
 @private_function="LogICMPv6Header"
 
   Purpose:  Logs the ICMPv6 Header into a more easily readable format.                         <br>
                                                                                                <br>
   Notes:    Uses ETW Tracing for the logging, which is not ideal in a real world scenario.     <br>
                                                                                                <br>
   MSDN_Ref: HTTP://MSDN.Microsoft.com/En-US/Library/FF562859.aspx                              <br>
*/
VOID LogICMPv6Header(_In_ ICMP_HEADER_V6* pICMPv6Header)
{
#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> LogICMPv6Header()\n");

#endif /// DBG

   NT_ASSERT(pICMPv6Header);

   NTSTATUS status  = STATUS_SUCCESS;
   PSTR     pString = 0;

   HLPR_NEW_ARRAY(pString,
                  CHAR,
                  MAX_STRING_SIZE,
                  WFPSAMPLER_CALLOUT_DRIVER_TAG);
   HLPR_BAIL_ON_ALLOC_FAILURE(pString,
                              status);

   status = RtlStringCchPrintfA(pString,
                                MAX_STRING_SIZE,
                                " 0                   1                   2                   3\n"
                                " 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1\n"
                                "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n"
                                "|     Type      |     Code      |           Checksum            |\n"
                                "|       %02x      |       %02x      |              %04x             |\n"
                                "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n"
                                "|              Variable (Dependent on Type / Code)              |\n"
                                "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n",
                                pICMPv6Header->type,
                                pICMPv6Header->code,
                                ntohs(pICMPv6Header->checksum));
   if(status == STATUS_SUCCESS)
      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_INFO_LEVEL,
                 "ICMPv6 Header:\n%s",
                 pString);

   HLPR_BAIL_LABEL:

   HLPR_DELETE_ARRAY(pString,
                     WFPSAMPLER_CALLOUT_DRIVER_TAG);

#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- LogICMPv6Header() [status: %#x]\n",
              status);

#else

   /// Used to get around preFast Warning 28931
   UNREFERENCED_PARAMETER(status);

#endif /// DBG

   return;
}

/**
 @private_function="LogTCPHeader"
 
   Purpose:  Logs the TCP Header into a more easily readable format.                            <br>
                                                                                                <br>
   Notes:    Uses ETW Tracing for the logging, which is not ideal in a real world scenario.     <br>
                                                                                                <br>
   MSDN_Ref: HTTP://MSDN.Microsoft.com/En-US/Library/FF562859.aspx                              <br>
*/
VOID LogTCPHeader(_In_ TCP_HEADER* pTCPHeader)
{
#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> LogTCPHeader()\n");

#endif /// DBG

   NT_ASSERT(pTCPHeader);

   NTSTATUS status  = STATUS_SUCCESS;
   PSTR     pString = 0;

   HLPR_NEW_ARRAY(pString,
                  CHAR,
                  MAX_STRING_SIZE,
                  WFPSAMPLER_CALLOUT_DRIVER_TAG);
   HLPR_BAIL_ON_ALLOC_FAILURE(pString,
                              status);

   status = RtlStringCchPrintfA(pString,
                                MAX_STRING_SIZE,
                                " 0                   1                   2                   3\n"
                                " 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1\n"
                                "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n"
                                "|          Source Port          |       Destination Port        |\n"
                                "|              %04x             |              %04x             |\n"
                                "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n"
                                "|                        Sequence Number                        |\n"
                                "|                            %08x                           |"
                                "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n"
                                "|                     Acknowledgment Number                     |\n"
                                "|                            %08x                           |"
                                "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n"
                                "|Offset |Rsvd |N|C|E|U|A|P|R|S|F|            Window             |\n"
                                "|      %01x     |       %01x        |              %04x             |\n"
                                "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n"
                                "|           Checksum            |        Urgent Pointer         |\n"
                                "|              %04x             |              %04x             |\n"
                                "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n"
                                "|                    Options                    |    Padding    |\n"
                                "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n",
                                ntohs(pTCPHeader->sourcePort),
                                ntohs(pTCPHeader->destinationPort),
                                ntohl(pTCPHeader->sequenceNumber),
                                ntohl(pTCPHeader->acknowledgementNumber),
                                pTCPHeader->dataOffsetReservedAndNS,
                                pTCPHeader->controlBits,
                                ntohs(pTCPHeader->window),
                                ntohs(pTCPHeader->checksum),
                                ntohs(pTCPHeader->urgentPointer));
   if(status == STATUS_SUCCESS)
      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_INFO_LEVEL,
                 "TCP Header:\n%s",
                 pString);

   HLPR_BAIL_LABEL:

   HLPR_DELETE_ARRAY(pString,
                     WFPSAMPLER_CALLOUT_DRIVER_TAG);

#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- LogTCPHeader() [status: %#x]\n",
              status);

#else

   /// Used to get around preFast Warning 28931
   UNREFERENCED_PARAMETER(status);

#endif /// DBG

   return;
}

/**
 @private_function="LogUDPHeader"
 
   Purpose:  Logs the UDP Header into a more easily readable format.                            <br>
                                                                                                <br>
   Notes:    Uses ETW Tracing for the logging, which is not ideal in a real world scenario.     <br>
                                                                                                <br>
   MSDN_Ref: HTTP://MSDN.Microsoft.com/En-US/Library/FF562859.aspx                              <br>
*/
VOID LogUDPHeader(_In_ UDP_HEADER* pUDPHeader)
{
#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> LogUDPHeader()\n");

#endif /// DBG

   NT_ASSERT(pUDPHeader);

   NTSTATUS status  = STATUS_SUCCESS;
   PSTR     pString = 0;

   HLPR_NEW_ARRAY(pString,
                  CHAR,
                  MAX_STRING_SIZE,
                  WFPSAMPLER_CALLOUT_DRIVER_TAG);
   HLPR_BAIL_ON_ALLOC_FAILURE(pString,
                              status);

   status = RtlStringCchPrintfA(pString,
                                MAX_STRING_SIZE,
                                " 0                   1                   2                   3\n"
                                " 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1\n"
                                "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n"
                                "|          Source Port          |       Destination Port        |\n"
                                "|              %04x             |              %04x             |\n"
                                "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n"
                                "|            Length             |           Checksum            |\n"
                                "|              %04x             |              %04x             |\n"
                                "+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n",
                                ntohs(pUDPHeader->sourcePort),
                                ntohs(pUDPHeader->destinationPort),
                                ntohs(pUDPHeader->length),
                                ntohs(pUDPHeader->checksum));
   if(status == STATUS_SUCCESS)
      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_INFO_LEVEL,
                 "UDP Header:\n%s",
                 pString);

   HLPR_BAIL_LABEL:

   HLPR_DELETE_ARRAY(pString,
                     WFPSAMPLER_CALLOUT_DRIVER_TAG);

#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- LogUDPHeader() [status: %#x]\n",
              status);

#else

   /// Used to get around preFast Warning 28931
   UNREFERENCED_PARAMETER(status);

#endif /// DBG

   return;
}

/**
 @private_function="LogTransportHeader"
 
   Purpose:  Proxy Transport Header logging to appropriate logging function base on IP protocol.<br>
                                                                                                <br>
   Notes:                                                                                       <br>
                                                                                                <br>
   MSDN_Ref:                                                                                    <br>
*/
VOID LogTransportHeader(_In_ VOID* pTransportHeader,
                        _In_ UINT8 ipProtocol)
{
   NT_ASSERT(pTransportHeader);

   switch(ipProtocol)
   {
      case ICMPV4:
      {
         LogICMPv4Header((ICMP_HEADER_V4*)pTransportHeader);

         break;
      }
      case TCP:
      {
         LogTCPHeader((TCP_HEADER*)pTransportHeader);

         break;
      }
      case UDP:
      {
         LogUDPHeader((UDP_HEADER*)pTransportHeader);

         break;
      }
      case ICMPV6:
      {
         LogICMPv6Header((ICMP_HEADER_V6*)pTransportHeader);

         break;
      }
   }

   return;
}

#if(NTDDI_VERSION >= NTDDI_WIN8)

/**
 @private_function="PerformBasicPacketExaminationAtInboundMACFrame"
 
   Purpose:  Examines and logs the contents of the MAC Header and Ip and Transport Headers if 
             available.                                                                         <br>
                                                                                                <br>
   Notes:    Applies to the following forwarding layers:                                        <br>
                FWPM_LAYER_INBOUND_MAC_FRAME_ETHERNET                                           <br>
                FWPM_LAYER_INBOUND_MAC_FRAME_NATIVE                                             <br>
                                                                                                <br>
   MSDN_Ref: HTTP://MSDN.Microsoft.com/En-US/Library/FF560703.aspx                              <br>
             HTTP://MSDN.Microsoft.com/En-US/Library/FF564527.aspx                              <br>
             HTTP://MSDN.Microsoft.com/En-US/Library/FF562631.aspx                              <br>
*/
VOID PerformBasicPacketExaminationAtInboundMACFrame(_In_ CLASSIFY_DATA* pClassifyData)
{
#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> PerformBasicPacketExaminationAtInboundMACFrame()\n");

#endif /// DBG

   NT_ASSERT(pClassifyData);

   NTSTATUS                       status          = STATUS_SUCCESS;
   FWPS_INCOMING_VALUES*          pClassifyValues = (FWPS_INCOMING_VALUES*)pClassifyData->pClassifyValues;
   FWPS_INCOMING_METADATA_VALUES* pMetadata       = (FWPS_INCOMING_METADATA_VALUES*)pClassifyData->pMetadataValues;
   UINT32                         bytesRetreated  = 0;
   UINT32                         bytesAdvanced   = 0;
   UINT32                         macHeaderSize   = 0;
   UINT32                         ipHeaderSize    = 0;
   UINT16                         etherType       = 0;
   UINT8                          ipProtocol      = 0;
   PVOID                          pContiguousData = 0;
   NET_BUFFER*                    pNetBuffer      = 0;

   if(FWPS_IS_L2_METADATA_FIELD_PRESENT(pMetadata,
                                        FWPS_L2_METADATA_FIELD_ETHERNET_MAC_HEADER_SIZE))
      macHeaderSize = pMetadata->ethernetMacHeaderSize;

   if(pClassifyValues->layerId == FWPS_LAYER_INBOUND_MAC_FRAME_ETHERNET)
   {
      FWP_VALUE* pDestinationAddressValue = 0;
      FWP_VALUE* pSourceAddressValue      = 0;
      FWP_VALUE* pEtherTypeValue          = 0;
      FWP_VALUE* pVLANIDValue             = 0;

      pSourceAddressValue = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                                      &FWPM_CONDITION_MAC_REMOTE_ADDRESS);
      HLPR_BAIL_ON_NULL_POINTER(pSourceAddressValue);

      pDestinationAddressValue = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                                           &FWPM_CONDITION_MAC_LOCAL_ADDRESS);
      HLPR_BAIL_ON_NULL_POINTER(pDestinationAddressValue);

      pEtherTypeValue = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                                  &FWPM_CONDITION_ETHER_TYPE);
      HLPR_BAIL_ON_NULL_POINTER(pEtherTypeValue);

      pVLANIDValue = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                                  &FWPM_CONDITION_VLAN_ID);
      HLPR_BAIL_ON_NULL_POINTER(pVLANIDValue);

      /// Initial offset is at the IP Header, so retreat the size of the MAC Header ...
      status = NdisRetreatNetBufferDataStart(NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket),
                                             bytesRetreated,
                                             0,
                                             0);
      if(status != STATUS_SUCCESS)
      {
         DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                    DPFLTR_ERROR_LEVEL,
                    " !!!! PerformBasicPacketExaminationAtInboundMACFrame: NdisRetreatNetBufferDataStart() [status: %#x]\n",
                    status);

         HLPR_BAIL;
      }
      else
         bytesRetreated = macHeaderSize;

      pNetBuffer = NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket);

      pContiguousData = NdisGetDataBuffer(pNetBuffer,
                                          NET_BUFFER_DATA_LENGTH(pNetBuffer),
                                          0,
                                          1,
                                          0);
      if(!pContiguousData)
      {
         status = STATUS_UNSUCCESSFUL;

         DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                    DPFLTR_ERROR_LEVEL,
                    " !!!! PerformBasicPacketExaminationAtInboundMACFrame : NdisGetDataBuffer() [status: %#x]\n",
                    status);

         HLPR_BAIL;
      }

      if(macHeaderSize == sizeof(ETHERNET_SNAP_HEADER))
      {
         ETHERNET_SNAP_HEADER* pEthernetSNAPHeader = (ETHERNET_SNAP_HEADER*)pNetBuffer;

         NT_ASSERT(pEthernetSNAPHeader->type == pEtherTypeValue->uint8);

         LogEthernetSNAPHeader(pEthernetSNAPHeader);
      }
      else
      {
         ETHERNET_II_HEADER* pEthernetIIHeader = (ETHERNET_II_HEADER*)pNetBuffer;

         NT_ASSERT(RtlCompareMemory(pEthernetIIHeader->pDestinationAddress,
                                    pDestinationAddressValue->byteArray6->byteArray6,
                                    ETHERNET_ADDRESS_SIZE) == ETHERNET_ADDRESS_SIZE);
         NT_ASSERT(RtlCompareMemory(pEthernetIIHeader->pSourceAddress,
                                    pSourceAddressValue->byteArray6->byteArray6,
                                    ETHERNET_ADDRESS_SIZE) == ETHERNET_ADDRESS_SIZE);
         NT_ASSERT(pEthernetIIHeader->type == pEtherTypeValue->uint8);

         LogEthernetIIHeader(pEthernetIIHeader);
      }

      etherType = pEtherTypeValue->uint16;
   }
   else
   {
      /// Initial offset is at the MAC Header ...
      pNetBuffer = NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket);

      pContiguousData = NdisGetDataBuffer(pNetBuffer,
                                          NET_BUFFER_DATA_LENGTH(pNetBuffer),
                                          0,
                                          1,
                                          0);
      if(!pContiguousData)
      {
         status = STATUS_UNSUCCESSFUL;

         DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                    DPFLTR_ERROR_LEVEL,
                    " !!!! PerformBasicPacketExaminationAtInboundMACFrame : NdisGetDataBuffer() [status: %#x]\n",
                    status);

         HLPR_BAIL;
      }

      if(macHeaderSize == sizeof(ETHERNET_SNAP_HEADER))
      {
         LogEthernetSNAPHeader((ETHERNET_SNAP_HEADER*)pNetBuffer);

         etherType = ((ETHERNET_SNAP_HEADER*)pNetBuffer)->type;
      }
      else
      {
         LogEthernetIIHeader((ETHERNET_II_HEADER*)pNetBuffer);

         etherType = ((ETHERNET_II_HEADER*)pNetBuffer)->type;
      }
   }

   if(bytesRetreated)
   {
      /// ... advance the offset back to the original position.
      NdisAdvanceNetBufferDataStart(NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket),
                                    bytesRetreated,
                                    FALSE,
                                    0);

      bytesRetreated -= macHeaderSize;
   }
   else
   {
      /// ... advance the offset to the IP Header.
      NdisAdvanceNetBufferDataStart(NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket),
                                    macHeaderSize,
                                    FALSE,
                                    0);

      bytesAdvanced += macHeaderSize;
   }

   pNetBuffer = NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket);

   pContiguousData = NdisGetDataBuffer(pNetBuffer,
                                       NET_BUFFER_DATA_LENGTH(pNetBuffer),
                                       0,
                                       1,
                                       0);
   if(!pContiguousData)
   {
      status = STATUS_UNSUCCESSFUL;

      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_ERROR_LEVEL,
                 " !!!! PerformBasicPacketExaminationAtInboundMACFrame : NdisGetDataBuffer() [status: %#x]\n",
                 status);

      HLPR_BAIL;
   }

   if(etherType == NDIS_ETH_TYPE_IPV4)
   {
      IP_HEADER_V4* pIPv4Header = (IP_HEADER_V4*)pNetBuffer;

      ipProtocol = pIPv4Header->protocol;

      ipHeaderSize = pIPv4Header->headerLength * 4;

      LogIPv4Header(pIPv4Header);
   }
   else if(etherType == NDIS_ETH_TYPE_IPV6)
   {
      IP_HEADER_V6* pIPv6Header = (IP_HEADER_V6*)pNetBuffer;

      ipProtocol = pIPv6Header->nextHeader;

      ipHeaderSize = sizeof(IP_HEADER_V6);

      LogIPv6Header(pIPv6Header);
   }
   else
      HLPR_BAIL;

   if(ipProtocol)
   {
      /// ... advance the offset to the Transport Header.
      NdisAdvanceNetBufferDataStart(NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket),
                                    ipHeaderSize,
                                    FALSE,
                                    0);

      bytesAdvanced += ipHeaderSize;

      LogTransportHeader(pNetBuffer,
                         ipProtocol);
   }

   HLPR_BAIL_LABEL:

   if(bytesRetreated)
   {
      /// ... advance the offset back to the original position.
      NdisAdvanceNetBufferDataStart(NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket),
                                    bytesRetreated,
                                    FALSE,
                                    0);
   }

   if(bytesAdvanced)
   {
      /// ... retreat the offset back to the original position.
      status = NdisRetreatNetBufferDataStart(NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket),
                                             bytesRetreated,
                                             0,
                                             0);
      if(status != STATUS_SUCCESS)
         DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                    DPFLTR_ERROR_LEVEL,
                    " !!!! PerformBasicPacketExaminationAtInboundMACFrame: NdisRetreatNetBufferDataStart() [status: %#x]\n",
                    status);
   }

#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- PerformBasicPacketExaminationAtInboundMACFrame()\n");

#endif /// DBG

   return;
}

/**
 @private_function="PerformBasicPacketExaminationAtOutboundMACFrame"
 
   Purpose:  Examines and logs the contents of the MAC Header and Ip and Transport Headers if 
             available.                                                                         <br>
                                                                                                <br>
   Notes:    Applies to the following forwarding layers:                                        <br>
                FWPM_LAYER_OUTBOUND_MAC_FRAME_ETHERNET                                          <br>
                FWPM_LAYER_OUTBOUND_MAC_FRAME_NATIVE                                            <br>
                                                                                                <br>
   MSDN_Ref: HTTP://MSDN.Microsoft.com/En-US/Library/FF560703.aspx                              <br>
             HTTP://MSDN.Microsoft.com/En-US/Library/FF564527.aspx                              <br>
             HTTP://MSDN.Microsoft.com/En-US/Library/FF562631.aspx                              <br>
*/
VOID PerformBasicPacketExaminationAtOutboundMACFrame(_In_ CLASSIFY_DATA* pClassifyData)
{
#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> PerformBasicPacketExaminationAtOutboundMACFrame()\n");

#endif /// DBG

   NT_ASSERT(pClassifyData);

   NTSTATUS                       status          = STATUS_SUCCESS;
   FWPS_INCOMING_VALUES*          pClassifyValues = (FWPS_INCOMING_VALUES*)pClassifyData->pClassifyValues;
   FWPS_INCOMING_METADATA_VALUES* pMetadata       = (FWPS_INCOMING_METADATA_VALUES*)pClassifyData->pMetadataValues;
   UINT32                         bytesAdvanced   = 0;
   UINT32                         macHeaderSize   = 0;
   UINT32                         ipHeaderSize    = 0;
   UINT16                         etherType       = 0;
   UINT8                          ipProtocol      = 0;
   PVOID                          pContiguousData = 0;
   NET_BUFFER*                    pNetBuffer      = 0;

   if(FWPS_IS_L2_METADATA_FIELD_PRESENT(pMetadata,
                                        FWPS_L2_METADATA_FIELD_ETHERNET_MAC_HEADER_SIZE))
      macHeaderSize = pMetadata->ethernetMacHeaderSize;

   if(pClassifyValues->layerId == FWPS_LAYER_INBOUND_MAC_FRAME_ETHERNET)
   {
      FWP_VALUE* pDestinationAddressValue = 0;
      FWP_VALUE* pSourceAddressValue      = 0;
      FWP_VALUE* pEtherTypeValue          = 0;
      FWP_VALUE* pVLANIDValue             = 0;

      pSourceAddressValue = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                                      &FWPM_CONDITION_MAC_REMOTE_ADDRESS);
      HLPR_BAIL_ON_NULL_POINTER(pSourceAddressValue);

      pDestinationAddressValue = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                                           &FWPM_CONDITION_MAC_LOCAL_ADDRESS);
      HLPR_BAIL_ON_NULL_POINTER(pDestinationAddressValue);

      pEtherTypeValue = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                                  &FWPM_CONDITION_ETHER_TYPE);
      HLPR_BAIL_ON_NULL_POINTER(pEtherTypeValue);

      pVLANIDValue = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                                  &FWPM_CONDITION_VLAN_ID);
      HLPR_BAIL_ON_NULL_POINTER(pVLANIDValue);

      /// Initial offset is at the MAC Header ...
      pNetBuffer = NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket);

      pContiguousData = NdisGetDataBuffer(pNetBuffer,
                                          NET_BUFFER_DATA_LENGTH(pNetBuffer),
                                          0,
                                          1,
                                          0);
      if(!pContiguousData)
      {
         status = STATUS_UNSUCCESSFUL;

         DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                    DPFLTR_ERROR_LEVEL,
                    " !!!! PerformBasicPacketExaminationAtOutboundMACFrame : NdisGetDataBuffer() [status: %#x]\n",
                    status);

         HLPR_BAIL;
      }

      if(macHeaderSize == sizeof(ETHERNET_SNAP_HEADER))
      {
         ETHERNET_SNAP_HEADER* pEthernetSNAPHeader = (ETHERNET_SNAP_HEADER*)pNetBuffer;

         NT_ASSERT(pEthernetSNAPHeader->type == pEtherTypeValue->uint8);

         LogEthernetSNAPHeader(pEthernetSNAPHeader);
      }
      else
      {
         ETHERNET_II_HEADER* pEthernetIIHeader = (ETHERNET_II_HEADER*)pNetBuffer;

         NT_ASSERT(RtlCompareMemory(pEthernetIIHeader->pDestinationAddress,
                                    pDestinationAddressValue->byteArray6->byteArray6,
                                    ETHERNET_ADDRESS_SIZE) == ETHERNET_ADDRESS_SIZE);
         NT_ASSERT(RtlCompareMemory(pEthernetIIHeader->pSourceAddress,
                                    pSourceAddressValue->byteArray6->byteArray6,
                                    ETHERNET_ADDRESS_SIZE) == ETHERNET_ADDRESS_SIZE);
         NT_ASSERT(pEthernetIIHeader->type == pEtherTypeValue->uint8);

         LogEthernetIIHeader(pEthernetIIHeader,
                             pVLANIDValue->type == FWP_UINT32 ? pVLANIDValue->uint32: 0);
      }

      etherType = pEtherTypeValue->uint16;
   }
   else
   {
      /// Initial offset is at the MAC Header ...
      pNetBuffer = NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket);

      pContiguousData = NdisGetDataBuffer(pNetBuffer,
                                          NET_BUFFER_DATA_LENGTH(pNetBuffer),
                                          0,
                                          1,
                                          0);
      if(!pContiguousData)
      {
         status = STATUS_UNSUCCESSFUL;

         DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                    DPFLTR_ERROR_LEVEL,
                    " !!!! PerformBasicPacketExaminationAtOutboundMACFrame : NdisGetDataBuffer() [status: %#x]\n",
                    status);

         HLPR_BAIL;
      }

      if(macHeaderSize == sizeof(ETHERNET_SNAP_HEADER))
      {
         LogEthernetSNAPHeader((ETHERNET_SNAP_HEADER*)pNetBuffer);

         etherType = ((ETHERNET_SNAP_HEADER*)pNetBuffer)->type;
      }
      else
      {
         LogEthernetIIHeader((ETHERNET_II_HEADER*)pNetBuffer);

         etherType = ((ETHERNET_II_HEADER*)pNetBuffer)->type;
      }
   }

   /// ... advance the offset to the IP Header.
   NdisAdvanceNetBufferDataStart(NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket),
                                 macHeaderSize,
                                 FALSE,
                                 0);

   bytesAdvanced += macHeaderSize;

   pNetBuffer = NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket);

   pContiguousData = NdisGetDataBuffer(pNetBuffer,
                                       NET_BUFFER_DATA_LENGTH(pNetBuffer),
                                       0,
                                       1,
                                       0);
   if(!pContiguousData)
   {
      status = STATUS_UNSUCCESSFUL;

      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_ERROR_LEVEL,
                 " !!!! PerformBasicPacketExaminationAtOutboundMACFrame : NdisGetDataBuffer() [status: %#x]\n",
                 status);

      HLPR_BAIL;
   }

   if(etherType == NDIS_ETH_TYPE_IPV4)
   {
      IP_HEADER_V4* pIPv4Header = (IP_HEADER_V4*)pNetBuffer;

      ipProtocol = pIPv4Header->protocol;

      ipHeaderSize = pIPv4Header->headerLength * 4;

      LogIPv4Header(pIPv4Header);
   }
   else if(etherType == NDIS_ETH_TYPE_IPV6)
   {
      IP_HEADER_V6* pIPv6Header = (IP_HEADER_V6*)pNetBuffer;

      ipProtocol = pIPv6Header->nextHeader;

      ipHeaderSize = sizeof(IP_HEADER_V6);

      LogIPv6Header(pIPv6Header);
   }
   else
      HLPR_BAIL;

   if(ipProtocol)
   {
      /// ... advance the offset to the Transport Header.
      NdisAdvanceNetBufferDataStart(NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket),
                                    ipHeaderSize,
                                    FALSE,
                                    0);

      bytesAdvanced += ipHeaderSize;

      LogTransportHeader(pNetBuffer,
                         ipProtocol);
   }

   HLPR_BAIL_LABEL:

   if(bytesAdvanced)
   {
      /// ... retreat the offset back to the original position.
      status = NdisRetreatNetBufferDataStart(NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket),
                                             bytesAdvanced,
                                             0,
                                             0);
      if(status != STATUS_SUCCESS)
         DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                    DPFLTR_ERROR_LEVEL,
                    " !!!! PerformBasicPacketExaminationAtOutboundMACFrame: NdisRetreatNetBufferDataStart() [status: %#x]\n",
                    status);
   }

#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- PerformBasicPacketExaminationAtOutboundMACFrame()\n");

#endif /// DBG

   return;
}

/**
 @private_function="PerformBasicPacketExaminationAtVSwitchTransport"
 
   Purpose:  Examines and logs the contents of the IP Header and Transport Headers if available.<br>
                                                                                                <br>
   Notes:    Applies to the following forwarding layers:                                        <br>
                FWPM_LAYER_INGRESS_VSWITCH_ETHERNET                                             <br>
                FWPM_LAYER_EGRESS_VSWITCH_ETHERNET                                              <br>
                                                                                                <br>
   MSDN_Ref: HTTP://MSDN.Microsoft.com/En-US/Library/FF560703.aspx                              <br>
             HTTP://MSDN.Microsoft.com/En-US/Library/FF564527.aspx                              <br>
             HTTP://MSDN.Microsoft.com/En-US/Library/FF562631.aspx                              <br>
*/
VOID PerformBasicPacketExaminationAtVSwitchEthernet(_In_ CLASSIFY_DATA* pClassifyData)
{
#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> PerformBasicPacketExaminationAtVSwitchEthernet()\n");

#endif /// DBG

   NT_ASSERT(pClassifyData);

   NTSTATUS                       status                   = STATUS_SUCCESS;
   FWPS_INCOMING_VALUES*          pClassifyValues          = (FWPS_INCOMING_VALUES*)pClassifyData->pClassifyValues;
   FWPS_INCOMING_METADATA_VALUES* pMetadata                = (FWPS_INCOMING_METADATA_VALUES*)pClassifyData->pMetadataValues;
   UINT32                         bytesAdvanced            = 0;
   UINT32                         macHeaderSize            = 0;
   UINT32                         ipHeaderSize             = 0;
   UINT16                         etherType                = 0;
   UINT8                          ipProtocol               = 0;
   PVOID                          pContiguousData          = 0;
   NET_BUFFER*                    pNetBuffer               = 0;
   FWP_VALUE*                     pDestinationAddressValue = 0;
   FWP_VALUE*                     pSourceAddressValue      = 0;
   FWP_VALUE*                     pEtherTypeValue          = 0;
   FWP_VALUE*                     pVLANIDValue             = 0;

   if(FWPS_IS_L2_METADATA_FIELD_PRESENT(pMetadata,
                                        FWPS_L2_METADATA_FIELD_ETHERNET_MAC_HEADER_SIZE))
      macHeaderSize = pMetadata->ethernetMacHeaderSize;

   pSourceAddressValue = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                                   &FWPM_CONDITION_MAC_SOURCE_ADDRESS);
   HLPR_BAIL_ON_NULL_POINTER(pSourceAddressValue);

   pDestinationAddressValue = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                                        &FWPM_CONDITION_MAC_DESTINATION_ADDRESS);
   HLPR_BAIL_ON_NULL_POINTER(pDestinationAddressValue);

   pEtherTypeValue = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                               &FWPM_CONDITION_ETHER_TYPE);
   HLPR_BAIL_ON_NULL_POINTER(pEtherTypeValue);

   pVLANIDValue = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                               &FWPM_CONDITION_VLAN_ID);
   HLPR_BAIL_ON_NULL_POINTER(pVLANIDValue);

   /// Initial offset is at the MAC Header ...
   pNetBuffer = NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket);

   pContiguousData = NdisGetDataBuffer(pNetBuffer,
                                       NET_BUFFER_DATA_LENGTH(pNetBuffer),
                                       0,
                                       1,
                                       0);
   if(!pContiguousData)
   {
      status = STATUS_UNSUCCESSFUL;

      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_ERROR_LEVEL,
                 " !!!! PerformBasicPacketExaminationAtVSwitchEthernet : NdisGetDataBuffer() [status: %#x]\n",
                 status);

      HLPR_BAIL;
   }

   if(macHeaderSize == sizeof(ETHERNET_SNAP_HEADER))
   {
      ETHERNET_SNAP_HEADER* pEthernetSNAPHeader = (ETHERNET_SNAP_HEADER*)pNetBuffer;

      NT_ASSERT(pEthernetSNAPHeader->type == pEtherTypeValue->uint8);

      LogEthernetSNAPHeader(pEthernetSNAPHeader);
   }
   else
   {
      ETHERNET_II_HEADER* pEthernetIIHeader = (ETHERNET_II_HEADER*)pNetBuffer;

      NT_ASSERT(RtlCompareMemory(pEthernetIIHeader->pDestinationAddress,
                                 pDestinationAddressValue->byteArray6->byteArray6,
                                 ETHERNET_ADDRESS_SIZE) == ETHERNET_ADDRESS_SIZE);
      NT_ASSERT(RtlCompareMemory(pEthernetIIHeader->pSourceAddress,
                                 pSourceAddressValue->byteArray6->byteArray6,
                                 ETHERNET_ADDRESS_SIZE) == ETHERNET_ADDRESS_SIZE);
      NT_ASSERT(pEthernetIIHeader->type == pEtherTypeValue->uint8);

      LogEthernetIIHeader(pEthernetIIHeader,
                          pVLANIDValue->type == FWP_UINT32 ? pVLANIDValue->uint32: 0);
   }

   etherType = pEtherTypeValue->uint16;

   /// ... advance the offset to the IP Header ...
   NdisAdvanceNetBufferDataStart(NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket),
                                 macHeaderSize,
                                 FALSE,
                                 0);

   bytesAdvanced += macHeaderSize;

   pNetBuffer = NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket);

   pContiguousData = NdisGetDataBuffer(pNetBuffer,
                                       NET_BUFFER_DATA_LENGTH(pNetBuffer),
                                       0,
                                       1,
                                       0);
   if(!pContiguousData)
   {
      status = STATUS_UNSUCCESSFUL;

      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_ERROR_LEVEL,
                 " !!!! PerformBasicPacketExaminationAtVSwitchEthernet : NdisGetDataBuffer() [status: %#x]\n",
                 status);

      HLPR_BAIL;
   }

   if(etherType == NDIS_ETH_TYPE_IPV4)
   {
      IP_HEADER_V4* pIPv4Header = (IP_HEADER_V4*)pNetBuffer;

      ipProtocol = pIPv4Header->protocol;

      ipHeaderSize = pIPv4Header->headerLength * 4;

      LogIPv4Header(pIPv4Header);
   }
   else if(etherType == NDIS_ETH_TYPE_IPV6)
   {
      IP_HEADER_V6* pIPv6Header = (IP_HEADER_V6*)pNetBuffer;

      ipProtocol = pIPv6Header->nextHeader;

      ipHeaderSize = sizeof(IP_HEADER_V6);

      LogIPv6Header(pIPv6Header);
   }
   else
      HLPR_BAIL;

   if(ipProtocol)
   {
      /// ... advance the offset to the Transport Header.
      NdisAdvanceNetBufferDataStart(NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket),
                                    ipHeaderSize,
                                    FALSE,
                                    0);

      bytesAdvanced += ipHeaderSize;

      LogTransportHeader(pNetBuffer,
                         ipProtocol);
   }

   HLPR_BAIL_LABEL:

   if(bytesAdvanced)
   {
      /// ... retreat the offset back to the original position.
      status = NdisRetreatNetBufferDataStart(NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket),
                                             bytesAdvanced,
                                             0,
                                             0);
      if(status != STATUS_SUCCESS)
         DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                    DPFLTR_ERROR_LEVEL,
                    " !!!! PerformBasicPacketExaminationAtVSwitchEthernet: NdisRetreatNetBufferDataStart() [status: %#x]\n",
                    status);
   }

#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- PerformBasicPacketExaminationAtVSwitchEthernet()\n");

#endif /// DBG

   return;
}

/**
 @private_function="PerformBasicPacketExaminationAtVSwitchTransport"
 
   Purpose:  Examines and logs the contents of the IP Header and Transport Headers if available.<br>
                                                                                                <br>
   Notes:    Applies to the following forwarding layers:                                        <br>
                FWPM_LAYER_INGRESS_VSWITCH_TRANSPORT_V4                                         <br>
                FWPM_LAYER_INGRESS_VSWITCH_TRANSPORT_V6                                         <br>
                FWPM_LAYER_EGRESS_VSWITCH_TRANSPORT_V4                                          <br>
                FWPM_LAYER_EGRESS_VSWITCH_TRANSPORT_V6                                          <br>
                                                                                                <br>
   MSDN_Ref: HTTP://MSDN.Microsoft.com/En-US/Library/FF560703.aspx                              <br>
             HTTP://MSDN.Microsoft.com/En-US/Library/FF564527.aspx                              <br>
             HTTP://MSDN.Microsoft.com/En-US/Library/FF562631.aspx                              <br>
*/
VOID PerformBasicPacketExaminationAtVSwitchTransport(_In_ CLASSIFY_DATA* pClassifyData)
{
#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> PerformBasicPacketExaminationAtVSwitchTransport()\n");

#endif /// DBG

   NT_ASSERT(pClassifyData);

   NTSTATUS                       status                   = STATUS_SUCCESS;
   FWPS_INCOMING_VALUES*          pClassifyValues          = (FWPS_INCOMING_VALUES*)pClassifyData->pClassifyValues;
   FWPS_INCOMING_METADATA_VALUES* pMetadata                = (FWPS_INCOMING_METADATA_VALUES*)pClassifyData->pMetadataValues;
   UINT32                         ipHeaderSize             = 0;
   UINT32                         bytesAdvanced            = 0;
   PVOID                          pContiguousData          = 0;
   NET_BUFFER*                    pNetBuffer               = 0;
   FWP_VALUE*                     pProtocolValue           = 0;
   FWP_VALUE*                     pSourceAddressValue      = 0;
   FWP_VALUE*                     pDestinationAddressValue = 0;
   FWP_VALUE*                     pSourcePortValue         = 0;
   FWP_VALUE*                     pDestinationPortValue    = 0;
   FWP_VALUE*                     pICMPTypeValue           = 0;
   FWP_VALUE*                     pICMPCodeValue           = 0;

   pProtocolValue = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                              &FWPM_CONDITION_IP_PROTOCOL);
   HLPR_BAIL_ON_NULL_POINTER(pProtocolValue);

   pSourceAddressValue = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                                   &FWPM_CONDITION_IP_SOURCE_ADDRESS);
   HLPR_BAIL_ON_NULL_POINTER(pSourceAddressValue);

   pDestinationAddressValue = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                                        &FWPM_CONDITION_IP_DESTINATION_ADDRESS);
   HLPR_BAIL_ON_NULL_POINTER(pDestinationAddressValue);

   pSourcePortValue = pICMPCodeValue = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                                                 &FWPM_CONDITION_IP_SOURCE_PORT);
   HLPR_BAIL_ON_NULL_POINTER(pSourcePortValue);

   pDestinationPortValue = pICMPTypeValue =  KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                                                       &FWPM_CONDITION_IP_DESTINATION_PORT);
   HLPR_BAIL_ON_NULL_POINTER(pDestinationPortValue);

   if(FWPS_IS_METADATA_FIELD_PRESENT(pMetadata,
                                     FWPS_METADATA_FIELD_IP_HEADER_SIZE))
      ipHeaderSize = pMetadata->ipHeaderSize;

   /// Initial offset is at the IP Header ...
   pNetBuffer = NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket);

   pContiguousData = NdisGetDataBuffer(pNetBuffer,
                                       NET_BUFFER_DATA_LENGTH(pNetBuffer),
                                       0,
                                       1,
                                       0);
   if(!pContiguousData)
   {
      status = STATUS_UNSUCCESSFUL;

      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_ERROR_LEVEL,
                 " !!!! PerformBasicPacketExaminationAtVSwitchTransport : NdisGetDataBuffer() [status: %#x]\n",
                 status);

      HLPR_BAIL;
   }

   /// Validate that what is indicated in the classify is what is present in packet's IP Header
   if(KrnlHlprFwpmLayerIsIPv4(pClassifyValues->layerId))
   {
      IP_HEADER_V4* pIPv4Header = (IP_HEADER_V4*)pContiguousData;

      NT_ASSERT(pIPv4Header->version == IPV4);
      NT_ASSERT(((UINT32)(pIPv4Header->headerLength * 4)) == ipHeaderSize);
      NT_ASSERT(pIPv4Header->protocol == pProtocolValue->uint8);
      NT_ASSERT(ntohl(*((UINT32*)pIPv4Header->pSourceAddress)) == pSourceAddressValue->uint32);
      NT_ASSERT(ntohl(*((UINT32*)pIPv4Header->pDestinationAddress)) == pDestinationAddressValue->uint32);

      LogIPv4Header(pIPv4Header);
   }
   else
   {
      IP_HEADER_V6* pIPv6Header = (IP_HEADER_V6*)pContiguousData;

      NT_ASSERT(sizeof(IP_HEADER_V6) == ipHeaderSize);
      NT_ASSERT(pIPv6Header->version.value == IPV6);
      NT_ASSERT(pIPv6Header->nextHeader == pProtocolValue->uint8);
      NT_ASSERT(RtlCompareMemory(pIPv6Header->pSourceAddress,
                                 pSourceAddressValue->byteArray16->byteArray16,
                                 IPV6_ADDRESS_SIZE) == IPV6_ADDRESS_SIZE);
      NT_ASSERT(RtlCompareMemory(pIPv6Header->pDestinationAddress,
                                 pDestinationAddressValue->byteArray16->byteArray16,
                                 IPV6_ADDRESS_SIZE) == IPV6_ADDRESS_SIZE);

      LogIPv6Header(pIPv6Header);
   }

   /// ... advance the offset to the Transport Header.
   NdisAdvanceNetBufferDataStart(NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket),
                                 ipHeaderSize,
                                 FALSE,
                                 0);

   bytesAdvanced += ipHeaderSize;

   pNetBuffer = NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket);

   pContiguousData = NdisGetDataBuffer(pNetBuffer,
                                       NET_BUFFER_DATA_LENGTH(pNetBuffer),
                                       0,
                                       1,
                                       0);
   if(!pContiguousData)
   {
      status = STATUS_UNSUCCESSFUL;

      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_ERROR_LEVEL,
                 " !!!! PerformBasicPacketExaminationAtVSwitchTransport : NdisGetDataBuffer() [status: %#x]\n",
                 status);

      HLPR_BAIL;
   }

   switch(pProtocolValue->uint8)
   {
      case ICMPV4:
      {
         ICMP_HEADER_V4* pICMPv4Header = (ICMP_HEADER_V4*)pContiguousData;

         NT_ASSERT(pICMPv4Header->type == pICMPTypeValue->uint16);
         NT_ASSERT(pICMPv4Header->code == pICMPCodeValue->uint16);

         LogICMPv4Header(pICMPv4Header);

         break;
      }
      case ICMPV6:
      {
         ICMP_HEADER_V6* pICMPv6Header = (ICMP_HEADER_V6*)pContiguousData;

         NT_ASSERT(pICMPv6Header->type == pICMPTypeValue->uint16);
         NT_ASSERT(pICMPv6Header->code == pICMPCodeValue->uint16);

         LogICMPv6Header(pICMPv6Header);

         break;
      }
      case TCP:
      {
         TCP_HEADER* pTCPHeader = (TCP_HEADER*)pContiguousData;

         NT_ASSERT(ntohs(pTCPHeader->sourcePort) == pSourcePortValue->uint16 );
         NT_ASSERT(ntohs(pTCPHeader->destinationPort) == pDestinationPortValue->uint16);

         LogTCPHeader(pTCPHeader);

         break;
      }
      case UDP:
      {
         UDP_HEADER* pUDPHeader = (UDP_HEADER*)pContiguousData;

         NT_ASSERT(ntohs(pUDPHeader->sourcePort) == pSourcePortValue->uint16 );
         NT_ASSERT(ntohs(pUDPHeader->destinationPort) == pDestinationPortValue->uint16);

         LogUDPHeader(pUDPHeader);

         break;
      }
   }

   /// ... advance the offset to the original position.
   NdisAdvanceNetBufferDataStart(NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket),
                                 bytesAdvanced,
                                 FALSE,
                                 0);

   bytesAdvanced -= ipHeaderSize;

   HLPR_BAIL_LABEL:

   if(bytesAdvanced)
   {
      /// ... retreat the offset to the original position.
      status = NdisRetreatNetBufferDataStart(NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket),
                                             bytesAdvanced,
                                             0,
                                             0);
      if(status != STATUS_SUCCESS)
         DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                    DPFLTR_ERROR_LEVEL,
                    " !!!! PerformBasicPacketExaminationAtInboundMACFrame: NdisRetreatNetBufferDataStart() [status: %#x]\n",
                    status);
   }

#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- PerformBasicPacketExaminationAtVSwitchTransport()\n");

#endif /// DBG

   return;
}

#endif /// (NTDDI_VERSION >= NTDDI_WIN8)

/**
 @private_function="PerformBasicPacketExaminationAtInboundNetwork"
 
   Purpose:  Examines and logs the contents of the IP Header and Transport Header if available. <br>
                                                                                                <br>
   Notes:    Applies to the following forwarding layers:                                        <br>
                FWPM_LAYER_INBOUND_IPPACKET_V{4/6}                                              <br>
                                                                                                <br>
   MSDN_Ref: HTTP://MSDN.Microsoft.com/En-US/Library/FF560703.aspx                              <br>
             HTTP://MSDN.Microsoft.com/En-US/Library/FF564527.aspx                              <br>
             HTTP://MSDN.Microsoft.com/En-US/Library/FF562631.aspx                              <br>
*/
VOID PerformBasicPacketExaminationAtInboundNetwork(_In_ CLASSIFY_DATA* pClassifyData)
{
#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> PerformBasicPacketExaminationAtInboundNetwork()\n");

#endif /// DBG

   NT_ASSERT(pClassifyData);

   NTSTATUS                       status                   = STATUS_SUCCESS;
   FWPS_INCOMING_VALUES*          pClassifyValues          = (FWPS_INCOMING_VALUES*)pClassifyData->pClassifyValues;
   FWPS_INCOMING_METADATA_VALUES* pMetadata                = (FWPS_INCOMING_METADATA_VALUES*)pClassifyData->pMetadataValues;
   UINT32                         ipHeaderSize             = 0;
   UINT32                         bytesRetreated           = 0;
   PVOID                          pContiguousData          = 0;
   NET_BUFFER*                    pNetBuffer               = 0;
   FWP_VALUE*                     pProtocolValue           = 0;
   FWP_VALUE*                     pSourceAddressValue      = 0;
   FWP_VALUE*                     pDestinationAddressValue = 0;

   pProtocolValue = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                              &FWPM_CONDITION_IP_PROTOCOL);
   HLPR_BAIL_ON_NULL_POINTER(pProtocolValue);

   pSourceAddressValue = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                                   &FWPM_CONDITION_IP_REMOTE_ADDRESS);
   HLPR_BAIL_ON_NULL_POINTER(pSourceAddressValue);

   pDestinationAddressValue = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                                        &FWPM_CONDITION_IP_LOCAL_ADDRESS);
   HLPR_BAIL_ON_NULL_POINTER(pDestinationAddressValue);

   if(FWPS_IS_METADATA_FIELD_PRESENT(pMetadata,
                                     FWPS_METADATA_FIELD_IP_HEADER_SIZE))
      ipHeaderSize = pMetadata->ipHeaderSize;

   /// Initial offset is at the Transport Header, so retreat the size of the IP Header ...
   status = NdisRetreatNetBufferDataStart(NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket),
                                          ipHeaderSize,
                                          0,
                                          0);
   if(status != STATUS_SUCCESS)
   {
      bytesRetreated = 0;

      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_ERROR_LEVEL,
                 " !!!! PerformBasicPacketExaminationAtInboundNetwork: NdisRetreatNetBufferDataStart() [status: %#x]\n",
                 status);

      HLPR_BAIL;
   }
   else
      bytesRetreated = ipHeaderSize;

   pNetBuffer = NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket);

   pContiguousData = NdisGetDataBuffer(pNetBuffer,
                                       NET_BUFFER_DATA_LENGTH(pNetBuffer),
                                       0,
                                       1,
                                       0);
   if(!pContiguousData)
   {
      status = STATUS_UNSUCCESSFUL;

      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_ERROR_LEVEL,
                 " !!!! PerformBasicPacketExaminationAtInboundNetwork : NdisGetDataBuffer() [status: %#x]\n",
                 status);

      HLPR_BAIL;
   }

   /// Validate that what is indicated in the classify is what is present in packet's IP Header
   if(KrnlHlprFwpmLayerIsIPv4(pClassifyValues->layerId))
   {
      IP_HEADER_V4* pIPv4Header = (IP_HEADER_V4*)pContiguousData;

      NT_ASSERT(pIPv4Header->version == IPV4);
      NT_ASSERT(((UINT32)(pIPv4Header->headerLength * 4)) == ipHeaderSize);
      NT_ASSERT(pIPv4Header->protocol == pProtocolValue->uint8);
      NT_ASSERT(ntohl(*((UINT32*)pIPv4Header->pSourceAddress)) == pSourceAddressValue->uint32);
      NT_ASSERT(ntohl(*((UINT32*)pIPv4Header->pDestinationAddress)) == pDestinationAddressValue->uint32);

      LogIPv4Header(pIPv4Header);
   }
   else
   {
      IP_HEADER_V6* pIPv6Header = (IP_HEADER_V6*)pContiguousData;

      NT_ASSERT(sizeof(IP_HEADER_V6) == ipHeaderSize);
      NT_ASSERT(pIPv6Header->version.value == IPV6);
      NT_ASSERT(pIPv6Header->nextHeader == pProtocolValue->uint8);
      NT_ASSERT(RtlCompareMemory(pIPv6Header->pSourceAddress,
                                 pSourceAddressValue->byteArray16->byteArray16,
                                 IPV6_ADDRESS_SIZE) == IPV6_ADDRESS_SIZE);
      NT_ASSERT(RtlCompareMemory(pIPv6Header->pDestinationAddress,
                                 pDestinationAddressValue->byteArray16->byteArray16,
                                 IPV6_ADDRESS_SIZE) == IPV6_ADDRESS_SIZE);

      LogIPv6Header(pIPv6Header);
   }

   /// ... and advance the offset back to the original position.
   NdisAdvanceNetBufferDataStart(NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket),
                                 bytesRetreated,
                                 FALSE,
                                 0);

   bytesRetreated -= ipHeaderSize;

   pNetBuffer = NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket);

   pContiguousData = NdisGetDataBuffer(pNetBuffer,
                                       NET_BUFFER_DATA_LENGTH(pNetBuffer),
                                       0,
                                       1,
                                       0);
   if(!pContiguousData)
   {
      status = STATUS_UNSUCCESSFUL;

      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_ERROR_LEVEL,
                 " !!!! PerformBasicPacketExaminationAtInboundNetwork : NdisGetDataBuffer() [status: %#x]\n",
                 status);

      HLPR_BAIL;
   }

   LogTransportHeader(pClassifyData->pPacket,
                      pProtocolValue->uint8);

   HLPR_BAIL_LABEL:

   if(bytesRetreated)
   {
      /// ... and advance the offset back to the original position.
      NdisAdvanceNetBufferDataStart(NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket),
                                    bytesRetreated,
                                    FALSE,
                                    0);
   }

#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- PerformBasicPacketExaminationAtInboundNetwork()\n");

#endif /// DBG

   return;
}

/**
 @private_function="PerformBasicPacketExaminationAtOutboundNetwork"
 
   Purpose:  Examines and logs the contents of the IP Header and Transport Header if available. <br>
                                                                                                <br>
   Notes:    Applies to the following forwarding layers:                                        <br>
                FWPM_LAYER_OUTBOUND_IPPACKET_V{4/6}                                             <br>
                                                                                                <br>
   MSDN_Ref: HTTP://MSDN.Microsoft.com/En-US/Library/FF560703.aspx                              <br>
             HTTP://MSDN.Microsoft.com/En-US/Library/FF564527.aspx                              <br>
             HTTP://MSDN.Microsoft.com/En-US/Library/FF562631.aspx                              <br>
*/
VOID PerformBasicPacketExaminationAtOutboundNetwork(_In_ CLASSIFY_DATA* pClassifyData)
{
#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> PerformBasicPacketExaminationAtOutboundNetwork()\n");

#endif /// DBG

   NT_ASSERT(pClassifyData);

   NTSTATUS                       status                   = STATUS_SUCCESS;
   FWPS_INCOMING_VALUES*          pClassifyValues          = (FWPS_INCOMING_VALUES*)pClassifyData->pClassifyValues;
   FWPS_INCOMING_METADATA_VALUES* pMetadata                = (FWPS_INCOMING_METADATA_VALUES*)pClassifyData->pMetadataValues;
   UINT32                         ipHeaderSize             = 0;
   UINT32                         bytesAdvanced            = 0;
   PVOID                          pContiguousData          = 0;
   NET_BUFFER*                    pNetBuffer               = 0;
   FWP_VALUE*                     pProtocolValue           = 0;
   FWP_VALUE*                     pSourceAddressValue      = 0;
   FWP_VALUE*                     pDestinationAddressValue = 0;

   pProtocolValue = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                              &FWPM_CONDITION_IP_PROTOCOL);
   HLPR_BAIL_ON_NULL_POINTER(pProtocolValue);

   pSourceAddressValue = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                                   &FWPM_CONDITION_IP_LOCAL_ADDRESS);
   HLPR_BAIL_ON_NULL_POINTER(pSourceAddressValue);

   pDestinationAddressValue = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                                        &FWPM_CONDITION_IP_REMOTE_ADDRESS);
   HLPR_BAIL_ON_NULL_POINTER(pDestinationAddressValue);

   if(FWPS_IS_METADATA_FIELD_PRESENT(pMetadata,
                                     FWPS_METADATA_FIELD_IP_HEADER_SIZE))
      ipHeaderSize = pMetadata->ipHeaderSize;

   /// Initial offset is at the IP Header ...
   pNetBuffer = NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket);

   pContiguousData = NdisGetDataBuffer(pNetBuffer,
                                       NET_BUFFER_DATA_LENGTH(pNetBuffer),
                                       0,
                                       1,
                                       0);
   if(!pContiguousData)
   {
      status = STATUS_UNSUCCESSFUL;

      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_ERROR_LEVEL,
                 " !!!! PerformBasicPacketExaminationAtOutboundNetwork : NdisGetDataBuffer() [status: %#x]\n",
                 status);

      HLPR_BAIL;
   }

   /// Validate that what is indicated in the classify is what is present in packet's IP Header
   if(KrnlHlprFwpmLayerIsIPv4(pClassifyValues->layerId))
   {
      IP_HEADER_V4* pIPv4Header = (IP_HEADER_V4*)pContiguousData;

      NT_ASSERT(pIPv4Header->version == IPV4);
      NT_ASSERT(((UINT32)(pIPv4Header->headerLength * 4)) == ipHeaderSize);
      NT_ASSERT(pIPv4Header->protocol == pProtocolValue->uint8);
      NT_ASSERT(ntohl(*((UINT32*)pIPv4Header->pSourceAddress)) == pSourceAddressValue->uint32);
      NT_ASSERT(ntohl(*((UINT32*)pIPv4Header->pDestinationAddress)) == pDestinationAddressValue->uint32);

      LogIPv4Header(pIPv4Header);
   }
   else
   {
      IP_HEADER_V6* pIPv6Header = (IP_HEADER_V6*)pContiguousData;

      NT_ASSERT(sizeof(IP_HEADER_V6) == ipHeaderSize);
      NT_ASSERT(pIPv6Header->version.value == IPV6);
      NT_ASSERT(pIPv6Header->nextHeader == pProtocolValue->uint8);
      NT_ASSERT(RtlCompareMemory(pIPv6Header->pSourceAddress,
                                 pSourceAddressValue->byteArray16->byteArray16,
                                 IPV6_ADDRESS_SIZE) == IPV6_ADDRESS_SIZE);
      NT_ASSERT(RtlCompareMemory(pIPv6Header->pDestinationAddress,
                                 pDestinationAddressValue->byteArray16->byteArray16,
                                 IPV6_ADDRESS_SIZE) == IPV6_ADDRESS_SIZE);

      LogIPv6Header(pIPv6Header);
   }

   bytesAdvanced = ipHeaderSize;

   /// ... advance the offset to the Transport Header ...
   NdisAdvanceNetBufferDataStart(NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket),
                                 bytesAdvanced,
                                 FALSE,
                                 0);

   pNetBuffer = NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket);

   pContiguousData = NdisGetDataBuffer(pNetBuffer,
                                       NET_BUFFER_DATA_LENGTH(pNetBuffer),
                                       0,
                                       1,
                                       0);
   if(!pContiguousData)
   {
      status = STATUS_UNSUCCESSFUL;

      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_ERROR_LEVEL,
                 " !!!! PerformBasicPacketExaminationAtOutboundNetwork : NdisGetDataBuffer() [status: %#x]\n",
                 status);

      HLPR_BAIL;
   }

   LogTransportHeader(pClassifyData->pPacket,
                      pProtocolValue->uint8);

   HLPR_BAIL_LABEL:

   if(bytesAdvanced)
   {
      /// ... and retreat the offset back to the original position.
      status = NdisRetreatNetBufferDataStart(NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket),
                                             bytesAdvanced,
                                             0,
                                             0);
      if(status != STATUS_SUCCESS)
         DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                    DPFLTR_ERROR_LEVEL,
                    " !!!! PerformBasicPacketExaminationAtOutboundNetwork : NdisRetreatNetBufferDataStart() [status: %#x]\n",
                    status);
   }

#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- PerformBasicPacketExaminationAtOutboundNetwork()\n");

#endif /// DBG

   return;
}

/**
 @private_function="PerformBasicPacketExaminationAtForward"
 
   Purpose:  Examines and logs the contents of the IP Header and Transport Header if available. <br>
                                                                                                <br>
   Notes:    Applies to the following forwarding layers:                                        <br>
                FWPM_LAYER_IPFORWARD_V{4/6}                                                     <br>
                                                                                                <br>
   MSDN_Ref: HTTP://MSDN.Microsoft.com/En-US/Library/FF560703.aspx                              <br>
             HTTP://MSDN.Microsoft.com/En-US/Library/FF564527.aspx                              <br>
             HTTP://MSDN.Microsoft.com/En-US/Library/FF562631.aspx                              <br>
*/
VOID PerformBasicPacketExaminationAtForward(_In_ CLASSIFY_DATA* pClassifyData)
{
#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> PerformBasicPacketExaminationAtForward()\n");

#endif /// DBG

   NT_ASSERT(pClassifyData);

   NTSTATUS                       status                   = STATUS_SUCCESS;
   FWPS_INCOMING_VALUES*          pClassifyValues          = (FWPS_INCOMING_VALUES*)pClassifyData->pClassifyValues;
   FWPS_INCOMING_METADATA_VALUES* pMetadata                = (FWPS_INCOMING_METADATA_VALUES*)pClassifyData->pMetadataValues;
   UINT32                         ipHeaderSize             = 0;
   UINT32                         bytesAdvanced            = 0;
   PVOID                          pContiguousData          = 0;
   NET_BUFFER*                    pNetBuffer               = 0;
   FWP_VALUE*                     pProtocolValue           = 0;
   FWP_VALUE*                     pSourceAddressValue      = 0;
   FWP_VALUE*                     pDestinationAddressValue = 0;

   pProtocolValue = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                              &FWPM_CONDITION_IP_PROTOCOL);
   HLPR_BAIL_ON_NULL_POINTER(pProtocolValue);

   pSourceAddressValue = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                                   &FWPM_CONDITION_IP_SOURCE_ADDRESS);
   HLPR_BAIL_ON_NULL_POINTER(pSourceAddressValue);

   pDestinationAddressValue = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                                        &FWPM_CONDITION_IP_DESTINATION_ADDRESS);
   HLPR_BAIL_ON_NULL_POINTER(pDestinationAddressValue);

   if(FWPS_IS_METADATA_FIELD_PRESENT(pMetadata,
                                     FWPS_METADATA_FIELD_IP_HEADER_SIZE))
      ipHeaderSize = pMetadata->ipHeaderSize;

   /// Initial offset is at the IP Header ...
   pNetBuffer = NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket);

   pContiguousData = NdisGetDataBuffer(pNetBuffer,
                                       NET_BUFFER_DATA_LENGTH(pNetBuffer),
                                       0,
                                       1,
                                       0);
   if(!pContiguousData)
   {
      status = STATUS_UNSUCCESSFUL;

      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_ERROR_LEVEL,
                 " !!!! PerformBasicPacketExaminationAtForward : NdisGetDataBuffer() [status: %#x]\n",
                 status);

      HLPR_BAIL;
   }

   /// Validate that what is indicated in the classify is what is present in packet's IP Header
   if(KrnlHlprFwpmLayerIsIPv4(pClassifyValues->layerId))
   {
      IP_HEADER_V4* pIPv4Header = (IP_HEADER_V4*)pContiguousData;

      NT_ASSERT(pIPv4Header->version == IPV4);
      NT_ASSERT(((UINT32)(pIPv4Header->headerLength * 4)) == ipHeaderSize);
      NT_ASSERT(pIPv4Header->protocol == pProtocolValue->uint8);
      NT_ASSERT(ntohl(*((UINT32*)pIPv4Header->pSourceAddress)) == pSourceAddressValue->uint32);
      NT_ASSERT(ntohl(*((UINT32*)pIPv4Header->pDestinationAddress)) == pDestinationAddressValue->uint32);

      LogIPv4Header(pIPv4Header);
   }
   else
   {
      IP_HEADER_V6* pIPv6Header = (IP_HEADER_V6*)pContiguousData;

      NT_ASSERT(sizeof(IP_HEADER_V6) == ipHeaderSize);
      NT_ASSERT(pIPv6Header->version.value == IPV6);
      NT_ASSERT(pIPv6Header->nextHeader == pProtocolValue->uint8);
      NT_ASSERT(RtlCompareMemory(pIPv6Header->pSourceAddress,
                                 pSourceAddressValue->byteArray16->byteArray16,
                                 IPV6_ADDRESS_SIZE) == IPV6_ADDRESS_SIZE);
      NT_ASSERT(RtlCompareMemory(pIPv6Header->pDestinationAddress,
                                 pDestinationAddressValue->byteArray16->byteArray16,
                                 IPV6_ADDRESS_SIZE) == IPV6_ADDRESS_SIZE);

      LogIPv6Header(pIPv6Header);
   }

   bytesAdvanced = ipHeaderSize;

   /// ... advance the offset to the Transport Header ...
   NdisAdvanceNetBufferDataStart(NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket),
                                 bytesAdvanced,
                                 FALSE,
                                 0);

   pNetBuffer = NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket);

   pContiguousData = NdisGetDataBuffer(pNetBuffer,
                                       NET_BUFFER_DATA_LENGTH(pNetBuffer),
                                       0,
                                       1,
                                       0);
   if(!pContiguousData)
   {
      status = STATUS_UNSUCCESSFUL;

      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_ERROR_LEVEL,
                 " !!!! PerformBasicPacketExaminationAtForward : NdisGetDataBuffer() [status: %#x]\n",
                 status);

      HLPR_BAIL;
   }

   LogTransportHeader(pClassifyData->pPacket,
                      pProtocolValue->uint8);

   HLPR_BAIL_LABEL:

   if(bytesAdvanced)
   {
      /// ... and retreat the offset back to the original position.
      status = NdisRetreatNetBufferDataStart(NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket),
                                             bytesAdvanced,
                                             0,
                                             0);
      if(status != STATUS_SUCCESS)
         DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                    DPFLTR_ERROR_LEVEL,
                    " !!!! PerformBasicPacketExaminationAtForward : NdisRetreatNetBufferDataStart() [status: %#x]\n",
                    status);
   }

#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- PerformBasicPacketExaminationAtForward()\n");

#endif /// DBG

   return;
}

/**
 @private_function="PerformBasicPacketExaminationAtInboundTransport"
 
   Purpose:  Examines and logs the contents of the IP Header and Transport Header if available. <br>
                                                                                                <br>
   Notes:    Applies to the following forwarding layers:                                        <br>
                FWPM_LAYER_INBOUND_TRANSPORT_V{4/6}                                             <br>
                FWPM_LAYER_INBOUND_ICMP_ERROR_V{4/6}                                            <br>
                FWPM_LAYER_DATAGRAM_DATA_V{4/6}        (Inbound only)                           <br>
                FWPM_LAYER_ALE_AUTH_RECV_ACCEPT_V{4/6} (Inbound only)                           <br>
                FWPM_LAYER_ALE_AUTH_RECV_ACCEPT_V{4/6} (Inbound only)                           <br>
                FWPM_LAYER_ALE_AUTH_CONNECT_V{4/6}     (Inbound, reauthorization only)          <br>
                FWPM_LAYER_ALE_FLOW_ESTABLISHED_V{4/6} (Inbound, non-TCP only)                  <br>
                FWPM_LAYER_STREAM_PACKET_V{4/6}        (Inbound only)                           <br>
                                                                                                <br>
   MSDN_Ref: HTTP://MSDN.Microsoft.com/En-US/Library/FF560703.aspx                              <br>
             HTTP://MSDN.Microsoft.com/En-US/Library/FF564527.aspx                              <br>
             HTTP://MSDN.Microsoft.com/En-US/Library/FF562631.aspx                              <br>
*/
VOID PerformBasicPacketExaminationAtInboundTransport(_In_ CLASSIFY_DATA* pClassifyData)
{
#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> PerformBasicPacketExaminationAtInboundTransport()\n");

#endif /// DBG

   NT_ASSERT(pClassifyData);

   NTSTATUS                       status                   = STATUS_SUCCESS;
   FWPS_INCOMING_VALUES*          pClassifyValues          = (FWPS_INCOMING_VALUES*)pClassifyData->pClassifyValues;
   FWPS_INCOMING_METADATA_VALUES* pMetadata                = (FWPS_INCOMING_METADATA_VALUES*)pClassifyData->pMetadataValues;
   UINT32                         ipHeaderSize             = 0;
   UINT32                         transportHeaderSize      = 0;
   UINT32                         bytesRetreated           = 0;
   UINT8                          protocol                 = 0;
   PVOID                          pContiguousData          = 0;
   NET_BUFFER*                    pNetBuffer               = 0;
   FWP_VALUE*                     pProtocolValue           = 0;
   FWP_VALUE*                     pSourceAddressValue      = 0;
   FWP_VALUE*                     pDestinationAddressValue = 0;
   FWP_VALUE*                     pSourcePortValue         = 0;
   FWP_VALUE*                     pDestinationPortValue    = 0;
   FWP_VALUE*                     pICMPTypeValue           = 0;
   FWP_VALUE*                     pICMPCodeValue           = 0;

   if(pClassifyValues->layerId == FWPS_LAYER_INBOUND_ICMP_ERROR_V4 ||
      pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_ICMP_ERROR_V4)
      protocol = ICMPV4;
   else if(pClassifyValues->layerId == FWPS_LAYER_INBOUND_ICMP_ERROR_V6 ||
           pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_ICMP_ERROR_V6)
      protocol = ICMPV6;

#if(NTDDI_VERSION >= NTDDI_WIN7)

   else if(pClassifyValues->layerId == FWPS_LAYER_STREAM_PACKET_V4 ||
           pClassifyValues->layerId == FWPS_LAYER_STREAM_PACKET_V6)
      protocol = TCP;

#endif /// (NTDDI_VERSION >= NTDDI_WIN7)

   else
   {
      pProtocolValue = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                                 &FWPM_CONDITION_IP_PROTOCOL);
      HLPR_BAIL_ON_NULL_POINTER(pProtocolValue);

      protocol = pProtocolValue->uint8;
   }

   pSourceAddressValue = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                                   &FWPM_CONDITION_IP_REMOTE_ADDRESS);
   HLPR_BAIL_ON_NULL_POINTER(pSourceAddressValue);

   pDestinationAddressValue = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                                        &FWPM_CONDITION_IP_LOCAL_ADDRESS);
   HLPR_BAIL_ON_NULL_POINTER(pDestinationAddressValue);

   pSourcePortValue = pICMPCodeValue = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                                                 &FWPM_CONDITION_IP_REMOTE_PORT);
   HLPR_BAIL_ON_NULL_POINTER(pSourcePortValue);

   pDestinationPortValue = pICMPTypeValue =  KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                                                       &FWPM_CONDITION_IP_LOCAL_PORT);
   HLPR_BAIL_ON_NULL_POINTER(pDestinationPortValue);

   if(FWPS_IS_METADATA_FIELD_PRESENT(pMetadata,
                                     FWPS_METADATA_FIELD_IP_HEADER_SIZE))
      ipHeaderSize = pMetadata->ipHeaderSize;

   if(FWPS_IS_METADATA_FIELD_PRESENT(pMetadata,
                                     FWPS_METADATA_FIELD_TRANSPORT_HEADER_SIZE))
      transportHeaderSize = pMetadata->transportHeaderSize;

   if(protocol == ICMPV4 ||
      protocol == ICMPV6)
   {
      /// Initial offset is at the ICMP Header, so retreat the size of the IP Header ...
      status = NdisRetreatNetBufferDataStart(NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket),
                                             ipHeaderSize,
                                             0,
                                             0);
   }
   else
   {
      /// Initial offset is at the Data, so retreat the size of the IP and Transport Headers ...
      status = NdisRetreatNetBufferDataStart(NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket),
                                             ipHeaderSize + transportHeaderSize,
                                             0,
                                             0);
   }

   if(status != STATUS_SUCCESS)
   {
      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_ERROR_LEVEL,
                 " !!!! PerformBasicPacketExaminationAtInboundTransport: NdisRetreatNetBufferDataStart() [status: %#x]\n",
                 status);

      HLPR_BAIL;
   }
   else
   {
      if(protocol == ICMPV4 ||
         protocol == ICMPV6)
         bytesRetreated = ipHeaderSize;
      else
         bytesRetreated = ipHeaderSize + transportHeaderSize;
   }

   pNetBuffer = NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket);

   pContiguousData = NdisGetDataBuffer(pNetBuffer,
                                       NET_BUFFER_DATA_LENGTH(pNetBuffer),
                                       0,
                                       1,
                                       0);
   if(!pContiguousData)
   {
      status = STATUS_UNSUCCESSFUL;

      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_ERROR_LEVEL,
                 " !!!! PerformBasicPacketExaminationAtInboundTransport : NdisGetDataBuffer() [status: %#x]\n",
                 status);

      HLPR_BAIL;
   }

   /// Validate that what is indicated in the classify is what is present in packet's IP Header
   if(KrnlHlprFwpmLayerIsIPv4(pClassifyValues->layerId))
   {
      IP_HEADER_V4* pIPv4Header = (IP_HEADER_V4*)pContiguousData;

      NT_ASSERT(pIPv4Header->version == IPV4);
      NT_ASSERT(((UINT32)(pIPv4Header->headerLength * 4)) == ipHeaderSize);
      NT_ASSERT(pIPv4Header->protocol == pProtocolValue->uint8);
      NT_ASSERT(ntohl(*((UINT32*)pIPv4Header->pSourceAddress)) == pSourceAddressValue->uint32);
      NT_ASSERT(ntohl(*((UINT32*)pIPv4Header->pDestinationAddress)) == pDestinationAddressValue->uint32);

      LogIPv4Header(pIPv4Header);
   }
   else
   {
      IP_HEADER_V6* pIPv6Header = (IP_HEADER_V6*)pContiguousData;

      NT_ASSERT(sizeof(IP_HEADER_V6) == ipHeaderSize);
      NT_ASSERT(pIPv6Header->version.value == IPV6);
      NT_ASSERT(pIPv6Header->nextHeader == pProtocolValue->uint8);
      NT_ASSERT(RtlCompareMemory(pIPv6Header->pSourceAddress,
                                 pSourceAddressValue->byteArray16->byteArray16,
                                 IPV6_ADDRESS_SIZE) == IPV6_ADDRESS_SIZE);
      NT_ASSERT(RtlCompareMemory(pIPv6Header->pDestinationAddress,
                                 pDestinationAddressValue->byteArray16->byteArray16,
                                 IPV6_ADDRESS_SIZE) == IPV6_ADDRESS_SIZE);

      LogIPv6Header(pIPv6Header);
   }

   /// and advance the offset to the Transport Header.
   NdisAdvanceNetBufferDataStart(NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket),
                                 ipHeaderSize,
                                 FALSE,
                                 0);

   bytesRetreated -= ipHeaderSize;

   pNetBuffer = NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket);

   pContiguousData = NdisGetDataBuffer(pNetBuffer,
                                       NET_BUFFER_DATA_LENGTH(pNetBuffer),
                                       0,
                                       1,
                                       0);
   if(!pContiguousData)
   {
      status = STATUS_UNSUCCESSFUL;

      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_ERROR_LEVEL,
                 " !!!! PerformBasicPacketExaminationAtInboundTransport : NdisGetDataBuffer() [status: %#x]\n",
                 status);

      HLPR_BAIL;
   }

   switch(protocol)
   {
      case ICMPV4:
      {
         ICMP_HEADER_V4* pICMPv4Header = (ICMP_HEADER_V4*)pContiguousData;

         NT_ASSERT(pICMPv4Header->type == pICMPTypeValue->uint16);
         NT_ASSERT(pICMPv4Header->code == pICMPCodeValue->uint16);

         LogICMPv4Header(pICMPv4Header);

         break;
      }
      case ICMPV6:
      {
         ICMP_HEADER_V6* pICMPv6Header = (ICMP_HEADER_V6*)pContiguousData;

         NT_ASSERT(pICMPv6Header->type == pICMPTypeValue->uint16);
         NT_ASSERT(pICMPv6Header->code == pICMPCodeValue->uint16);

         LogICMPv6Header(pICMPv6Header);

         break;
      }
      case TCP:
      {
         TCP_HEADER* pTCPHeader = (TCP_HEADER*)pContiguousData;

         NT_ASSERT(ntohs(pTCPHeader->sourcePort) == pSourcePortValue->uint16 );
         NT_ASSERT(ntohs(pTCPHeader->destinationPort) == pDestinationPortValue->uint16);

         LogTCPHeader(pTCPHeader);

         break;
      }
      case UDP:
      {
         UDP_HEADER* pUDPHeader = (UDP_HEADER*)pContiguousData;

         NT_ASSERT(ntohs(pUDPHeader->sourcePort) == pSourcePortValue->uint16 );
         NT_ASSERT(ntohs(pUDPHeader->destinationPort) == pDestinationPortValue->uint16);

         LogUDPHeader(pUDPHeader);

         break;
      }
   }

   /// ... advance the offset to the original position.
   NdisAdvanceNetBufferDataStart(NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket),
                                 bytesRetreated,
                                 FALSE,
                                 0);

   bytesRetreated -= transportHeaderSize;

   HLPR_BAIL_LABEL:

   if(bytesRetreated)
   {
      /// ... and advance the offset back to the original position.
      NdisAdvanceNetBufferDataStart(NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket),
                                    bytesRetreated,
                                    FALSE,
                                    0);
   }

#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- PerformBasicPacketExaminationAtInboundTransport()\n");

#endif /// DBG

   return;
}

/**
 @private_function="PerformBasicPacketExaminationAtOutboundTransport"
 
   Purpose:  Examines and logs the contents of the IP Header and Transport Header if available. <br>
                                                                                                <br>
   Notes:    Applies to the following forwarding layers:                                        <br>
                FWPM_LAYER_OUTBOUND_TRANSPORT_V{4/6}                                            <br>
                FWPM_LAYER_OUTBOUND_ICMP_ERROR_V{4/6}                                           <br>
                FWPM_LAYER_DATAGRAM_DATA_V{4/6}        (Outbound only)                          <br>
                FWPM_LAYER_ALE_AUTH_RECV_ACCEPT_V{4/6} (Outbound only)                          <br>
                FWPM_LAYER_ALE_AUTH_RECV_ACCEPT_V{4/6} (Outbound only)                          <br>
                FWPM_LAYER_ALE_AUTH_CONNECT_V{4/6}     (Outbound reauthorization only)          <br>
                FWPM_LAYER_ALE_FLOW_ESTABLISHED_V{4/6} (Outbound, non-TCP only)                 <br>
                FWPM_LAYER_STREAM_PACKET_V{4/6}        (Outbound only)                          <br>
                                                                                                <br>
   MSDN_Ref: HTTP://MSDN.Microsoft.com/En-US/Library/FF560703.aspx                              <br>
             HTTP://MSDN.Microsoft.com/En-US/Library/FF564527.aspx                              <br>
             HTTP://MSDN.Microsoft.com/En-US/Library/FF562631.aspx                              <br>
*/
VOID PerformBasicPacketExaminationAtOutboundTransport(_In_ CLASSIFY_DATA* pClassifyData)
{
#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> PerformBasicPacketExaminationAtOutboundTransport()\n");

#endif /// DBG

   NT_ASSERT(pClassifyData);

   NTSTATUS              status                = STATUS_SUCCESS;
   FWPS_INCOMING_VALUES* pClassifyValues       = (FWPS_INCOMING_VALUES*)pClassifyData->pClassifyValues;
   PVOID                 pContiguousData       = 0;
   NET_BUFFER*           pNetBuffer            = 0;
   FWP_VALUE*            pProtocolValue        = 0;
   FWP_VALUE*            pSourcePortValue      = 0;
   FWP_VALUE*            pDestinationPortValue = 0;
   FWP_VALUE*            pICMPTypeValue        = 0;
   FWP_VALUE*            pICMPCodeValue        = 0;

   pProtocolValue = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                              &FWPM_CONDITION_IP_PROTOCOL);
   HLPR_BAIL_ON_NULL_POINTER(pProtocolValue);

   pSourcePortValue = pICMPCodeValue = KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                                                 &FWPM_CONDITION_IP_LOCAL_PORT);
   HLPR_BAIL_ON_NULL_POINTER(pSourcePortValue);

   pDestinationPortValue = pICMPTypeValue =  KrnlHlprFwpValueGetFromFwpsIncomingValues(pClassifyValues,
                                                                                       &FWPM_CONDITION_IP_REMOTE_PORT);
   HLPR_BAIL_ON_NULL_POINTER(pDestinationPortValue);

   /// Initial offset is at the Transport Header ...
   pNetBuffer = NET_BUFFER_LIST_FIRST_NB((NET_BUFFER_LIST*)pClassifyData->pPacket);

   pContiguousData = NdisGetDataBuffer(pNetBuffer,
                                       NET_BUFFER_DATA_LENGTH(pNetBuffer),
                                       0,
                                       1,
                                       0);
   if(!pContiguousData)
   {
      status = STATUS_UNSUCCESSFUL;

      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_ERROR_LEVEL,
                 " !!!! PerformBasicPacketExaminationAtOutboundTransport : NdisGetDataBuffer() [status: %#x]\n",
                 status);

      HLPR_BAIL;
   }

   switch(pProtocolValue->uint8)
   {
      case ICMPV4:
      {
         ICMP_HEADER_V4* pICMPv4Header = (ICMP_HEADER_V4*)pContiguousData;

         NT_ASSERT(pICMPv4Header->type == pICMPTypeValue->uint16);
         NT_ASSERT(pICMPv4Header->code == pICMPCodeValue->uint16);

         LogICMPv4Header(pICMPv4Header);

         break;
      }
      case ICMPV6:
      {
         ICMP_HEADER_V6* pICMPv6Header = (ICMP_HEADER_V6*)pContiguousData;

         NT_ASSERT(pICMPv6Header->type == pICMPTypeValue->uint16);
         NT_ASSERT(pICMPv6Header->code == pICMPCodeValue->uint16);

         LogICMPv6Header(pICMPv6Header);

         break;
      }
      case TCP:
      {
         TCP_HEADER* pTCPHeader = (TCP_HEADER*)pContiguousData;

         NT_ASSERT(ntohs(pTCPHeader->sourcePort) == pSourcePortValue->uint16);
         NT_ASSERT(ntohs(pTCPHeader->destinationPort) == pDestinationPortValue->uint16);

         LogTCPHeader(pTCPHeader);

         break;
      }
      case UDP:
      {
         UDP_HEADER* pUDPHeader = (UDP_HEADER*)pContiguousData;

         NT_ASSERT(ntohs(pUDPHeader->sourcePort) == pSourcePortValue->uint16);
         NT_ASSERT(ntohs(pUDPHeader->destinationPort) == pDestinationPortValue->uint16);

         LogUDPHeader(pUDPHeader);

         break;
      }
   }

   HLPR_BAIL_LABEL:

#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- PerformBasicPacketExaminationAtOutboundTransport()\n");

#endif /// DBG

   return;
}

#if(NTDDI_VERSION >= NTDDI_WIN7)

/**
 @classify_function="ClassifyBasicPacketExamination"
 
   Purpose:  Examines the packet and returns FWP_ACTION_CONTINUE.                               <br>
                                                                                                <br>
   Notes:    Applies to the following layers:                                                   <br>
                FWPS_LAYER_INBOUND_IPPACKET_V{4/6}                                              <br>
                FWPS_LAYER_OUTBOUND_IPPACKET_V{4/6}                                             <br>
                FWPS_LAYER_IPFORWARD_V{4/6}                                                     <br>
                FWPS_LAYER_INBOUND_TRANSPORT_V{4/6}                                             <br>
                FWPS_LAYER_OUTBOUND_TRANSPORT_V{4/6}                                            <br>
                FWPS_LAYER_DATAGRAM_DATA_V{4/6}                                                 <br>
                FWPS_LAYER_INBOUND_ICMP_ERROR_V{4/6}                                            <br>
                FWPS_LAYER_OUTBOUND_ICMP_ERROR_V{4/6}                                           <br>
                FWPS_LAYER_ALE_AUTH_CONNECT_V{4/6}                                              <br>
                FWPS_LAYER_ALE_FLOW_ESTABLISHED_V{4/6}                                          <br>
                FWPS_LAYER_STREAM_PACKET_V{4/6}                                                 <br>
                FWPS_LAYER_INBOUND_MAC_FRAME_ETHERNET                                           <br>
                FWPS_LAYER_OUTBOUND_MAC_FRAME_ETHERNET                                          <br>
                FWPS_LAYER_INBOUND_MAC_FRAME_NATIVE                                             <br>
                FWPS_LAYER_OUTBOUND_MAC_FRAME_NATIVE                                            <br>
                FWPS_LAYER_INGRESS_VSWITCH_ETHERNET                                             <br>
                FWPS_LAYER_EGRESS_VSWITCH_ETHERNET                                              <br>
                FWPS_LAYER_INGRESS_VSWITCH_TRANSPORT_V{4/6}                                     <br>
                FWPS_LAYER_EGRESS_VSWITCH_TRANSPORT_V{4/6}                                      <br>
                                                                                                <br>
   MSDN_Ref: HTTP://MSDN.Microsoft.com/En-US/Library/FF544893.aspx                              <br>
*/
_IRQL_requires_min_(PASSIVE_LEVEL)
_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_requires_same_
VOID ClassifyBasicPacketExamination(_In_ const FWPS_INCOMING_VALUES0* pClassifyValues,
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
              " ---> ClassifyBasicPacketExamination()\n");

#endif /// DBG

   NT_ASSERT(pClassifyValues);
   NT_ASSERT(pMetadata);
   NT_ASSERT(pFilter);
   NT_ASSERT(pClassifyOut);

#if(NTDDI_VERSION >= NTDDI_WIN8)

   NT_ASSERT(pClassifyValues->layerId == FWPS_LAYER_INBOUND_IPPACKET_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_INBOUND_IPPACKET_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_IPPACKET_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_IPPACKET_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_IPFORWARD_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_IPFORWARD_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_INBOUND_TRANSPORT_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_INBOUND_TRANSPORT_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_TRANSPORT_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_TRANSPORT_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_DATAGRAM_DATA_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_DATAGRAM_DATA_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_INBOUND_ICMP_ERROR_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_INBOUND_ICMP_ERROR_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_ICMP_ERROR_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_ICMP_ERROR_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_ALE_AUTH_RECV_ACCEPT_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_ALE_AUTH_RECV_ACCEPT_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_ALE_AUTH_CONNECT_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_ALE_AUTH_CONNECT_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_ALE_FLOW_ESTABLISHED_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_ALE_FLOW_ESTABLISHED_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_STREAM_PACKET_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_STREAM_PACKET_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_INBOUND_MAC_FRAME_ETHERNET ||
             pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_MAC_FRAME_ETHERNET ||
             pClassifyValues->layerId == FWPS_LAYER_INBOUND_MAC_FRAME_NATIVE ||
             pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_MAC_FRAME_NATIVE ||
             pClassifyValues->layerId == FWPS_LAYER_INGRESS_VSWITCH_ETHERNET ||
             pClassifyValues->layerId == FWPS_LAYER_EGRESS_VSWITCH_ETHERNET ||
             pClassifyValues->layerId == FWPS_LAYER_INGRESS_VSWITCH_TRANSPORT_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_INGRESS_VSWITCH_TRANSPORT_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_EGRESS_VSWITCH_TRANSPORT_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_EGRESS_VSWITCH_TRANSPORT_V6);

#else

   NT_ASSERT(pClassifyValues->layerId == FWPS_LAYER_INBOUND_IPPACKET_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_INBOUND_IPPACKET_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_IPPACKET_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_IPPACKET_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_IPFORWARD_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_IPFORWARD_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_INBOUND_TRANSPORT_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_INBOUND_TRANSPORT_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_TRANSPORT_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_TRANSPORT_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_DATAGRAM_DATA_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_DATAGRAM_DATA_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_INBOUND_ICMP_ERROR_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_INBOUND_ICMP_ERROR_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_ICMP_ERROR_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_ICMP_ERROR_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_ALE_AUTH_RECV_ACCEPT_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_ALE_AUTH_RECV_ACCEPT_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_ALE_AUTH_CONNECT_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_ALE_AUTH_CONNECT_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_ALE_FLOW_ESTABLISHED_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_ALE_FLOW_ESTABLISHED_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_STREAM_PACKET_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_STREAM_PACKET_V6);

#endif /// (NTDDI_VERSION >= NTDDI_WIN8)

   NTSTATUS       status        = STATUS_SUCCESS;
   CLASSIFY_DATA* pClassifyData = 0;
   FWP_DIRECTION  direction     = FWP_DIRECTION_MAX;

   HLPR_NEW(pClassifyData,
            CLASSIFY_DATA,
            WFPSAMPLER_CALLOUT_DRIVER_TAG);
   HLPR_BAIL_ON_ALLOC_FAILURE(pClassifyData,
                              status);

   if(FWPS_IS_METADATA_FIELD_PRESENT(pMetadata,
                                     FWPS_METADATA_FIELD_PACKET_DIRECTION))
      direction = pMetadata->packetDirection;

   pClassifyData->pClassifyValues  = pClassifyValues;
   pClassifyData->pMetadataValues  = pMetadata;
   pClassifyData->pPacket          = pNetBufferList;
   pClassifyData->pClassifyContext = pClassifyContext;
   pClassifyData->pFilter          = pFilter;
   pClassifyData->flowContext      = flowContext;
   pClassifyData->pClassifyOut     = pClassifyOut;

   if(pClassifyValues->layerId == FWPS_LAYER_INBOUND_IPPACKET_V4 ||
      pClassifyValues->layerId == FWPS_LAYER_INBOUND_IPPACKET_V6)
      PerformBasicPacketExaminationAtInboundNetwork(pClassifyData);
   else if(pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_IPPACKET_V4 ||
           pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_IPPACKET_V6)
      PerformBasicPacketExaminationAtOutboundNetwork(pClassifyData);
   else if(pClassifyValues->layerId == FWPS_LAYER_IPFORWARD_V4 ||
           pClassifyValues->layerId == FWPS_LAYER_IPFORWARD_V6)
      PerformBasicPacketExaminationAtForward(pClassifyData);
   else if(pClassifyValues->layerId == FWPS_LAYER_INBOUND_TRANSPORT_V4 ||
           pClassifyValues->layerId == FWPS_LAYER_INBOUND_TRANSPORT_V6 ||
           pClassifyValues->layerId == FWPS_LAYER_INBOUND_ICMP_ERROR_V4 ||
           pClassifyValues->layerId == FWPS_LAYER_INBOUND_ICMP_ERROR_V6 ||
           (direction == FWP_DIRECTION_INBOUND &&
           (pClassifyValues->layerId == FWPS_LAYER_DATAGRAM_DATA_V4 ||
           pClassifyValues->layerId == FWPS_LAYER_DATAGRAM_DATA_V6 ||
           pClassifyValues->layerId == FWPS_LAYER_ALE_AUTH_RECV_ACCEPT_V4 ||
           pClassifyValues->layerId == FWPS_LAYER_ALE_AUTH_RECV_ACCEPT_V6 ||
           pClassifyValues->layerId == FWPS_LAYER_ALE_AUTH_CONNECT_V4 ||     /// Policy Change Reauthorization
           pClassifyValues->layerId == FWPS_LAYER_ALE_AUTH_CONNECT_V6 ||     /// Policy Change Reauthorization
           pClassifyValues->layerId == FWPS_LAYER_ALE_FLOW_ESTABLISHED_V4 ||
           pClassifyValues->layerId == FWPS_LAYER_ALE_FLOW_ESTABLISHED_V6)))
      PerformBasicPacketExaminationAtInboundTransport(pClassifyData);
   else if(pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_TRANSPORT_V4 ||
           pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_TRANSPORT_V6 ||
           pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_ICMP_ERROR_V4 ||
           pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_ICMP_ERROR_V6 ||
           (direction == FWP_DIRECTION_OUTBOUND &&
           (pClassifyValues->layerId == FWPS_LAYER_DATAGRAM_DATA_V4 ||
           pClassifyValues->layerId == FWPS_LAYER_DATAGRAM_DATA_V6 ||
           pClassifyValues->layerId == FWPS_LAYER_ALE_AUTH_RECV_ACCEPT_V4 || /// Policy Change Reauthorization
           pClassifyValues->layerId == FWPS_LAYER_ALE_AUTH_RECV_ACCEPT_V6 || /// Policy Change Reauthorization
           pClassifyValues->layerId == FWPS_LAYER_ALE_AUTH_CONNECT_V4 ||
           pClassifyValues->layerId == FWPS_LAYER_ALE_AUTH_CONNECT_V6 ||
           pClassifyValues->layerId == FWPS_LAYER_ALE_FLOW_ESTABLISHED_V4 ||
           pClassifyValues->layerId == FWPS_LAYER_ALE_FLOW_ESTABLISHED_V6)))
      PerformBasicPacketExaminationAtOutboundTransport(pClassifyData);
   else if(direction == FWP_DIRECTION_INBOUND &&
           (pClassifyValues->layerId == FWPS_LAYER_STREAM_PACKET_V4 ||
           pClassifyValues->layerId == FWPS_LAYER_STREAM_PACKET_V6))
      PerformBasicPacketExaminationAtInboundTransport(pClassifyData);
   else if(direction == FWP_DIRECTION_OUTBOUND &&
           (pClassifyValues->layerId == FWPS_LAYER_STREAM_PACKET_V4 ||
           pClassifyValues->layerId == FWPS_LAYER_STREAM_PACKET_V6))
      PerformBasicPacketExaminationAtOutboundTransport(pClassifyData);

#if(NTDDI_VERSION >= NTDDI_WIN8)

   else if(pClassifyValues->layerId == FWPS_LAYER_INBOUND_MAC_FRAME_ETHERNET ||
           pClassifyValues->layerId == FWPS_LAYER_INBOUND_MAC_FRAME_NATIVE)
      PerformBasicPacketExaminationAtInboundMACFrame(pClassifyData);
   else if(pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_MAC_FRAME_ETHERNET ||
           pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_MAC_FRAME_NATIVE)
      PerformBasicPacketExaminationAtOutboundMACFrame(pClassifyData);
   else if(pClassifyValues->layerId == FWPS_LAYER_INGRESS_VSWITCH_ETHERNET ||
           pClassifyValues->layerId == FWPS_LAYER_EGRESS_VSWITCH_ETHERNET)
      PerformBasicPacketExaminationAtVSwitchEthernet(pClassifyData);
   else if(pClassifyValues->layerId == FWPS_LAYER_INGRESS_VSWITCH_TRANSPORT_V4 ||
           pClassifyValues->layerId == FWPS_LAYER_INGRESS_VSWITCH_TRANSPORT_V6 ||
           pClassifyValues->layerId == FWPS_LAYER_EGRESS_VSWITCH_TRANSPORT_V4 ||
           pClassifyValues->layerId == FWPS_LAYER_EGRESS_VSWITCH_TRANSPORT_V6)
      PerformBasicPacketExaminationAtVSwitchTransport(pClassifyData);

#endif // (NTDDI_VERSION >= NTDDI_WIN8)

   HLPR_BAIL_LABEL:

   HLPR_DELETE(pClassifyData,
               WFPSAMPLER_CALLOUT_DRIVER_TAG);

   if(pClassifyOut->rights & FWPS_RIGHT_ACTION_WRITE)
      pClassifyOut->actionType = FWP_ACTION_CONTINUE;

#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- ClassifyBasicPacketExamination() [status: %#x]\n",
              status);

#else

   UNREFERENCED_PARAMETER(status);

#endif /// DBG

   return;
}

#else

/**
 @classify_function="ClassifyBasicPacketExamination"
 
   Purpose:  Examines the packet and returns FWP_ACTION_CONTINUE.                               <br>
                                                                                                <br>
   Notes:    Applies to the following layers:                                                   <br>
                FWPS_LAYER_INBOUND_IPPACKET_V{4/6}                                              <br>
                FWPS_LAYER_OUTBOUND_IPPACKET_V{4/6}                                             <br>
                FWPS_LAYER_IPFORWARD_V{4/6}                                                     <br>
                FWPS_LAYER_INBOUND_TRANSPORT_V{4/6}                                             <br>
                FWPS_LAYER_OUTBOUND_TRANSPORT_V{4/6}                                            <br>
                FWPS_LAYER_DATAGRAM_DATA_V{4/6}                                                 <br>
                FWPS_LAYER_INBOUND_ICMP_ERROR_V{4/6}                                            <br>
                FWPS_LAYER_OUTBOUND_ICMP_ERROR_V{4/6}                                           <br>
                FWPS_LAYER_ALE_AUTH_CONNECT_V{4/6}                                              <br>
                FWPS_LAYER_ALE_FLOW_ESTABLISHED_V{4/6}                                          <br>
                                                                                                <br>
   MSDN_Ref: HTTP://MSDN.Microsoft.com/En-US/Library/FF544890.aspx                              <br>
*/
_IRQL_requires_min_(PASSIVE_LEVEL)
_IRQL_requires_max_(DISPATCH_LEVEL)
_IRQL_requires_same_
VOID ClassifyBasicPacketExamination(_In_ const FWPS_INCOMING_VALUES* pClassifyValues,
                                    _In_ const FWPS_INCOMING_METADATA_VALUES* pMetadata,
                                    _Inout_opt_ VOID* pNetBufferList,
                                    _In_ const FWPS_FILTER* pFilter,
                                    _In_ UINT64 flowContext,
                                    _Inout_ FWPS_CLASSIFY_OUT* pClassifyOut)
{
#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> ClassifyBasicPacketExamination()\n");

#endif /// DBG

   NT_ASSERT(pClassifyValues);
   NT_ASSERT(pMetadata);
   NT_ASSERT(pFilter);
   NT_ASSERT(pClassifyOut);
   NT_ASSERT(pClassifyValues->layerId == FWPS_LAYER_INBOUND_IPPACKET_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_INBOUND_IPPACKET_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_IPPACKET_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_IPPACKET_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_IPFORWARD_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_IPFORWARD_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_INBOUND_TRANSPORT_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_INBOUND_TRANSPORT_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_TRANSPORT_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_TRANSPORT_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_DATAGRAM_DATA_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_DATAGRAM_DATA_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_INBOUND_ICMP_ERROR_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_INBOUND_ICMP_ERROR_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_ICMP_ERROR_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_ICMP_ERROR_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_ALE_AUTH_RECV_ACCEPT_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_ALE_AUTH_RECV_ACCEPT_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_ALE_AUTH_CONNECT_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_ALE_AUTH_CONNECT_V6 ||
             pClassifyValues->layerId == FWPS_LAYER_ALE_FLOW_ESTABLISHED_V4 ||
             pClassifyValues->layerId == FWPS_LAYER_ALE_FLOW_ESTABLISHED_V6);

   NTSTATUS       status        = STATUS_SUCCESS;
   CLASSIFY_DATA* pClassifyData = 0;
   FWP_DIRECTION  direction     = FWP_DIRECTION_MAX;

   HLPR_NEW(pClassifyData,
            CLASSIFY_DATA,
            WFPSAMPLER_CALLOUT_DRIVER_TAG);
   HLPR_BAIL_ON_ALLOC_FAILURE(pClassifyData,
                              status);

   if(FWPS_IS_METADATA_FIELD_PRESENT(pMetadata,
                                     FWPS_METADATA_FIELD_PACKET_DIRECTION))
      direction = pMetadata->packetDirection;

   pClassifyData->pClassifyValues = pClassifyValues;
   pClassifyData->pMetadataValues = pMetadata;
   pClassifyData->pPacket         = pNetBufferList;
   pClassifyData->pFilter         = pFilter;
   pClassifyData->flowContext     = flowContext;
   pClassifyData->pClassifyOut    = pClassifyOut;

   if(pClassifyValues->layerId == FWPS_LAYER_INBOUND_IPPACKET_V4 ||
      pClassifyValues->layerId == FWPS_LAYER_INBOUND_IPPACKET_V6)
      PerformBasicPacketExaminationAtInboundNetwork(pClassifyData);
   else if(pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_IPPACKET_V4 ||
           pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_IPPACKET_V6)
      PerformBasicPacketExaminationAtOutboundNetwork(pClassifyData);
   else if(pClassifyValues->layerId == FWPS_LAYER_IPFORWARD_V4 ||
           pClassifyValues->layerId == FWPS_LAYER_IPFORWARD_V6)
      PerformBasicPacketExaminationAtForward(pClassifyData);
   else if(pClassifyValues->layerId == FWPS_LAYER_INBOUND_TRANSPORT_V4 ||
           pClassifyValues->layerId == FWPS_LAYER_INBOUND_TRANSPORT_V6 ||
           pClassifyValues->layerId == FWPS_LAYER_INBOUND_ICMP_ERROR_V4 ||
           pClassifyValues->layerId == FWPS_LAYER_INBOUND_ICMP_ERROR_V6 ||
           (direction == FWP_DIRECTION_INBOUND &&
           (pClassifyValues->layerId == FWPS_LAYER_DATAGRAM_DATA_V4 ||
           pClassifyValues->layerId == FWPS_LAYER_DATAGRAM_DATA_V6 ||
           pClassifyValues->layerId == FWPS_LAYER_ALE_AUTH_RECV_ACCEPT_V4 ||
           pClassifyValues->layerId == FWPS_LAYER_ALE_AUTH_RECV_ACCEPT_V6 ||
           pClassifyValues->layerId == FWPS_LAYER_ALE_AUTH_CONNECT_V4 ||     /// Policy Change Reauthorization
           pClassifyValues->layerId == FWPS_LAYER_ALE_AUTH_CONNECT_V6 ||     /// Policy Change Reauthorization
           pClassifyValues->layerId == FWPS_LAYER_ALE_FLOW_ESTABLISHED_V4 ||
           pClassifyValues->layerId == FWPS_LAYER_ALE_FLOW_ESTABLISHED_V6)))
      PerformBasicPacketExaminationAtInboundTransport(pClassifyData);
   else if(pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_TRANSPORT_V4 ||
           pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_TRANSPORT_V6 ||
           pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_ICMP_ERROR_V4 ||
           pClassifyValues->layerId == FWPS_LAYER_OUTBOUND_ICMP_ERROR_V6 ||
           (direction == FWP_DIRECTION_OUTBOUND &&
           (pClassifyValues->layerId == FWPS_LAYER_DATAGRAM_DATA_V4 ||
           pClassifyValues->layerId == FWPS_LAYER_DATAGRAM_DATA_V6 ||
           pClassifyValues->layerId == FWPS_LAYER_ALE_AUTH_RECV_ACCEPT_V4 || /// Policy Change Reauthorization
           pClassifyValues->layerId == FWPS_LAYER_ALE_AUTH_RECV_ACCEPT_V6 || /// Policy Change Reauthorization
           pClassifyValues->layerId == FWPS_LAYER_ALE_AUTH_CONNECT_V4 ||
           pClassifyValues->layerId == FWPS_LAYER_ALE_AUTH_CONNECT_V6 ||
           pClassifyValues->layerId == FWPS_LAYER_ALE_FLOW_ESTABLISHED_V4 ||
           pClassifyValues->layerId == FWPS_LAYER_ALE_FLOW_ESTABLISHED_V6)))
      PerformBasicPacketExaminationAtOutboundTransport(pClassifyData);

   HLPR_BAIL_LABEL:

   HLPR_DELETE(pClassifyData,
               WFPSAMPLER_CALLOUT_DRIVER_TAG);

   if(pClassifyOut->rights & FWPS_RIGHT_ACTION_WRITE)
      pClassifyOut->actionType = FWP_ACTION_CONTINUE;

#if DBG

   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- ClassifyBasicPacketExamination() [status: %#x]\n",
              status);

#endif /// DBG

   return;
}

#endif // (NTDDI_VERSION >= NTDDI_WIN7)
