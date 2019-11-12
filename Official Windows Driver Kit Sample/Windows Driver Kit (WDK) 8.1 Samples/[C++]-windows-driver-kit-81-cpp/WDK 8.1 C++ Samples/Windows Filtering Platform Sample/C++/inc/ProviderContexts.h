///////////////////////////////////////////////////////////////////////////////
//
//   Copyright (c) 2012 Microsoft Corporation.  All Rights Reserved.
//
//   Module Name:
//      ProviderContexts.h
//
//   Abstract:
//      This module contains global definitions of FWPM_PROVIDER_CONTEXT Data for the WFPSampler project
//
//   Author:
//      Dusty Harper      (DHarper)
//
//   Revision History:
//
//      [ Month ][Day] [Year] - [Revision]-[ Comments ]
//      May       01,   2010  -     1.0   -  Creation
//
///////////////////////////////////////////////////////////////////////////////

#ifndef WFP_SAMPLER_PROVIDER_CONTEXT_H
#define WFP_SAMPLER_PROVIDER_CONTEXT_H

/// ProviderContext ProxyData Flags
#define PCPDF_PROXY_LOCAL_ADDRESS  0x01
#define PCPDF_PROXY_LOCAL_PORT     0x02
#define PCPDF_PROXY_REMOTE_ADDRESS 0x04
#define PCPDF_PROXY_REMOTE_PORT    0x08

///ProviderContext PacketModificationData Flags
#define PCPMDF_MODIFY_MAC_HEADER                     0x10
#define PCPMDF_MODIFY_MAC_HEADER_SOURCE_ADDRESS      0x01
#define PCPMDF_MODIFY_MAC_HEADER_DESTINATION_ADDRESS 0x02

#define PCPMDF_MODIFY_IP_HEADER                     0x20
#define PCPMDF_MODIFY_IP_HEADER_SOURCE_ADDRESS      0x01
#define PCPMDF_MODIFY_IP_HEADER_DESTINATION_ADDRESS 0x02

#define PCPMDF_MODIFY_TRANSPORT_HEADER                  0x40
#define PCPMDF_MODIFY_TRANSPORT_HEADER_SOURCE_PORT      0x01
#define PCPMDF_MODIFY_TRANSPORT_HEADER_DESTINATION_PORT 0x02

#define PCPMDF_MODIFY_TRANSPORT_HEADER_ICMP_TYPE PCPMDF_MODIFY_TRANSPORT_HEADER_SOURCE_PORT
#define PCPMDF_MODIFY_TRANSPORT_HEADER_ICMP_CODE PCPMDF_MODIFY_TRANSPORT_HEADER_DESTINATION_PORT

typedef struct MAC_DATA_
{
   UINT32 flags;
   BYTE   pReserved[4];
   BYTE   pSourceMACAddress[8];
   BYTE   pDestinationMACAddress[8];
}MAC_DATA;

typedef struct IP_DATA_
{
   UINT32 flags;
   BYTE   pReserved[3];
   UINT8  ipVersion;
   union
   {
      BYTE pIPv4[4];                       /// Network Byte Order
      BYTE pIPv6[16];
      BYTE pBytes[16];
   }sourceAddress;
   union
   {
      BYTE pIPv4[4];                       /// Network Byte Order
      BYTE pIPv6[16];
      BYTE pBytes[16];
   }destinationAddress;
}IP_DATA;

typedef struct TRANSPORT_DATA_
{
   UINT32 flags;
   BYTE   pReserved[3];
   UINT8  protocol;
   union
   {
      UINT8  icmpType;
      UINT16 sourcePort;                   /// Network Byte Order
   };
   union
   {
      UINT8  icmpCode;
      UINT16 destinationPort;              /// Network Byte Order
   };
   BYTE pPadding[4];
} TRANSPORT_DATA;

typedef struct PC_BASIC_ACTION_DATA_
{
   UINT8 percentBlock;
   UINT8 percentPermit;
   UINT8 percentContinue;
   BYTE  pReserved[5];
} PC_BASIC_ACTION_DATA, *PPC_BASIC_ACTION_DATA;

typedef struct PC_BASIC_PACKET_INJECTION_DATA_
{
   BOOLEAN performInline;  /// Inline vs. Out of Band
   BOOLEAN useWorkItems;   /// Work Items vs. Deferred Procedure Calls
   BOOLEAN useThreadedDPC; /// Threaded DPCs vs Deferred Procedure Calls
   BYTE    pReserved[5];
} PC_BASIC_PACKET_INJECTION_DATA, *PPC_BASIC_PACKET_INJECTION_DATA;

typedef struct PC_BASIC_PACKET_MODIFICATION_DATA_
{
   UINT32         flags;
   BOOLEAN        performInline;  /// Inline vs. Out of Band
   BOOLEAN        useWorkItems;   /// Work Items vs. Deferred Procedure Calls
   BOOLEAN        useThreadedDPC; /// Threaded DPCs vs Deferred Procedure Calls
   BYTE           pReserved[1];
   MAC_DATA       macData;
   IP_DATA        ipData;
   TRANSPORT_DATA transportData;
}PC_BASIC_PACKET_MODIFICATION_DATA;

typedef struct PC_BASIC_STREAM_INJECTION_DATA_
{
   BOOLEAN performInline;  /// Inline vs. Out of Band
   BOOLEAN useWorkItems;   /// Work Items vs. Deferred Procedure Calls
   BOOLEAN useThreadedDPC; /// Threaded DPCs vs Deferred Procedure Calls
   BYTE    pReserved[5];
} PC_BASIC_STREAM_INJECTION_DATA, *PPC_BASIC_STREAM_INJECTION_DATA;

typedef struct PC_PEND_AUTHORIZATION_DATA_
{
   UINT32  flags;
   UINT32  finalAction;
   UINT32  delay;
   BOOLEAN useWorkItems;                   /// Work Items vs. Deferred Procedure Calls
   BOOLEAN useThreadedDPC;                 /// Threaded DPCs vs Deferred Procedure Calls
   BYTE    pReserved[2];
}PC_PEND_AUTHORIZATION_DATA, *PPC_PEND_AUTHORIZATION_DATA;

typedef struct PC_PROXY_DATA_
{
   UINT32  flags;
   BOOLEAN performInline;                  /// Inline vs. Out of Band
   BOOLEAN useWorkItems;                   /// Work Items vs. Deferred Procedure Calls
   BOOLEAN useThreadedDPC;                 /// Threaded DPCs vs Deferred Procedure Calls
   BOOLEAN proxyToRemoteService;           /// Local vs. Remote Service
   BYTE    pReserved[7];
   UINT8   ipVersion;
   union
   {
      BYTE pIPv4[4];                       /// Network Byte Order
      BYTE pIPv6[16];
      BYTE pBytes[16];
   }proxyLocalAddress;
   union
   {
      BYTE pIPv4[4];                       /// Network Byte Order
      BYTE pIPv6[16];
      BYTE pBytes[16];
   }proxyRemoteAddress;
   UINT32  localScopeId;
   UINT32  remoteScopeId;
   UINT16  proxyLocalPort;                 /// Network Byte Order
   UINT16  proxyRemotePort;                /// Network Byte Order
   UINT32  targetProcessID;
   UINT64  tcpPortReservationToken;
   UINT64  udpPortReservationToken;

} PC_PROXY_DATA, *PPC_PROXY_DATA;

#endif /// WFP_SAMPLER_PROVIDER_CONTEXT_H
