/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    data_glb_defs.h

Abstract:
    TX/RX data related defines common to the complete driver

Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/

#pragma once

typedef struct _MP_TX_MSDU      MP_TX_MSDU, *PMP_TX_MSDU;
typedef struct _MP_TX_MPDU      MP_TX_MPDU, *PMP_TX_MPDU;
typedef struct _MP_RX_MSDU      MP_RX_MSDU, *PMP_RX_MSDU;
typedef struct _MP_RX_MPDU      MP_RX_MPDU, *PMP_RX_MPDU;

typedef struct _HW_TX_MSDU      HW_TX_MSDU, *PHW_TX_MSDU;
typedef struct _HW_TX_MPDU      HW_TX_MPDU, *PHW_TX_MPDU;
typedef struct _HW_RX_MSDU      HW_RX_MSDU, *PHW_RX_MSDU;
typedef struct _HW_RX_MPDU      HW_RX_MPDU, *PHW_RX_MPDU;

/** We dont do TX fragmentation. So we only expect there to be
 * a single TX fragment
 */
#define MP_TX_FRAGMENTS_MAX_COUNT       1

/**
 * Number of RX fragments we can expect correspondings to the 
 * MAX number by the spec
 */
#define MP_RX_FRAGMENTS_MAX_COUNT       DOT11_MAX_NUM_OF_FRAGMENTS

// Macro to access the TX_MSDU & TX_MPDU fields
#define MP_TX_MSDU_NEXT_MSDU(_Msdu)     ((_Msdu)->Next)
#define MP_TX_MSDU_STATUS(_Msdu)        ((_Msdu)->Status)
#define MP_TX_MSDU_WRAPPED_NBL(_Msdu)   ((_Msdu)->NetBufferList)
#define MP_TX_MSDU_MPDU_AT(_Msdu, _i)   ((_Msdu)->MpduList[_i])
#define MP_TX_MSDU_MPDU_COUNT(_Msdu)    ((_Msdu)->MpduCount)
#define MP_TX_MSDU_SEND_CONTEXT(_Msdu)  (&(_Msdu)->SendContext)
#define MP_TX_MSDU_MAC_CONTEXT(_Msdu)   ((_Msdu)->MacContext)
#define MP_TX_MSDU_IS_INTERNAL(_Msdu)   (MP_TX_MPDU_IS_INTERNAL(MP_TX_MSDU_MPDU_AT(_Msdu, 0)))
#define MP_TX_MSDU_IS_PRIVATE(_Msdu)    ((_Msdu)->HardwarePrivate ? TRUE : FALSE)

#define MP_TX_MPDU_WRAPPED_NB(_Mpdu)    ((_Mpdu)->NetBuffer)
#define MP_TX_MPDU_MSDU(_Mpdu)          ((_Mpdu)->Msdu)
#define MP_TX_MPDU_IS_INTERNAL(_Mpdu)   (MP_TX_MPDU_WRAPPED_NB(_Mpdu) == NULL ? TRUE : FALSE)

// Returns the total length of the data in the FRAGMENT
#define MP_TX_MPDU_LENGTH(_Mpdu)        \
    (MP_TX_MPDU_IS_INTERNAL(_Mpdu) ? _Mpdu->InternalSendLength : NET_BUFFER_DATA_LENGTH(MP_TX_MPDU_WRAPPED_NB(_Mpdu)))

// Returns a pointer to a buffer containing X bytes in it. If the desired number of bytes is not
// contiguous, this returns a NULL
#define MP_TX_MPDU_DATA(_Mpdu, _BytesNeeded)    \
    (MP_TX_MPDU_IS_INTERNAL(_Mpdu) ? _Mpdu->InternalSendDataStart : Dot11GetNetBufferData(MP_TX_MPDU_WRAPPED_NB(_Mpdu), _BytesNeeded))
    

// Macros for getting the PACKET from the NBL
#define MP_NBL_WRAPPED_TX_MSDU(_NBL)    (*((PMP_TX_MSDU*)&(NET_BUFFER_LIST_MINIPORT_RESERVED(_NBL)[0])))
#define MP_NB_WRAPPED_TX_MPDU(_NB)      (*((PMP_TX_MPDU*)&(NET_BUFFER_MINIPORT_RESERVED(_NB)[0])))


/*
 * This corresponds to the TX packet unit that the OS hands to the miniport
 * This is a TX MSDU / NET_BUFFER_LIST
 */
typedef struct _MP_TX_MSDU  
{
    /** Use for queuing the packets */
    QUEUE_ENTRY                 QueueEntry;

    /** When sending/completing multiple this is the next pointer */
    PMP_TX_MSDU                 Next;
    
    /** NET_BUFFER_LIST submitted by the OS. This can be NULL for internal sends */
    PNET_BUFFER_LIST            NetBufferList;
    
    PMP_TX_MPDU                 MpduList[MP_TX_FRAGMENTS_MAX_COUNT];
    USHORT                      MpduCount;

    BOOLEAN                     HardwarePrivate;

    /** Send context from the OS */
    DOT11_EXTSTA_SEND_CONTEXT   SendContext;

    // Start of data from the TX completion
    NDIS_STATUS                 Status;
    // End of data from the TX completion

    /** The MAC context that sent this packet */
    PHW_MAC_CONTEXT             MacContext;

    /** Structure used to maintain hardware's TX_MSDU data */
    PHW_TX_MSDU                 HwMsdu;
    
}MP_TX_MSDU, *PMP_TX_MSDU;


/*
 * This corresponds to the TX fragment that the miniport puts in the air.
 * This is a TX MPDU / NET_BUFFER
 */
typedef struct _MP_TX_MPDU
{
    /** MP_TX_MSDU   corresponding to this fragment */
    PMP_TX_MSDU                 Msdu;

    /** NET_BUFFER submitted by the OS. This can be NULL for internal sends in which case
     * the InternalSend values should be non-zero */
    PNET_BUFFER                 NetBuffer;

    /** For internal sends the NET_BUFFER field would be empty and this is a 
     * pointer to the data that needs to be sent. This would point to
     * some location withing the internal send buffer */
    PUCHAR                      InternalSendDataStart;
    USHORT                      InternalSendLength;

    /** This holds the memory for the internal send data. The send data 
     * would be at a backfill within this buffer
     */
    PUCHAR                      InternalSendBuffer;

    /** Structure used to maintain hardware's TX_MPDU data */
    PHW_TX_MPDU                 HwMpdu;
    
}MP_TX_MPDU, *PMP_TX_MPDU;




// Macro to access the RX_PACKET & RX_FRAGMENT fields
#define MP_RX_MSDU_NEXT_MSDU(_Msdu)     ((_Msdu)->Next)
#define MP_RX_MSDU_FLAGS(_Msdu)         ((_Msdu)->Flags)
#define MP_RX_MSDU_MPDU_AT(_Msdu, _i)   ((_Msdu)->MpduList[_i])
#define MP_RX_MSDU_MPDU_COUNT(_Msdu)    ((_Msdu)->MpduCount)
#define MP_RX_MSDU_STATUS(_Msdu)        ((_Msdu)->Status)
#define MP_RX_MSDU_RECV_CONTEXT(_Msdu)  (&((_Msdu)->RecvContext))

#define MP_RX_MPDU_DATA(_Mpdu)          ((_Mpdu)->Data)
#define MP_RX_MPDU_LENGTH(_Mpdu)        ((_Mpdu)->DataLength)


// Macros for getting the PACKET from the NBL
#define MP_NBL_WRAPPED_RX_MSDU(_NBL)    (*((PMP_RX_MSDU*)&(NET_BUFFER_LIST_MINIPORT_RESERVED(_NBL)[0])))
#define MP_NB_WRAPPED_RX_MPDU(_NB)      (*((PMP_RX_MPDU*)&(NET_BUFFER_MINIPORT_RESERVED(_NB)[0])))

// Macros for getting the PORT from the NBL
#define MP_NBL_SOURCE_PORT(_NBL)         (*((PMP_PORT*)&(NET_BUFFER_LIST_MINIPORT_RESERVED(_NBL)[1])))

/** Flags for the RX PACKET */
#define MP_RX_MSDU_FLAG_ENCRYPTED      0x0001

/*
 * This corresponds to the RX packet unit that the miniport hands to the OS
 * This is a RX MSDU / NET_BUFFER_LIST. 
 */
typedef struct _MP_RX_MSDU  
{
    /** When receiving/returning multiple this is the next pointer */
    PMP_RX_MSDU                 Next;

    PMP_RX_MPDU                 MpduList[MP_RX_FRAGMENTS_MAX_COUNT];
    USHORT                      MpduCount;

    /** The receive context for this FRAGMENT */
    DOT11_EXTSTA_RECV_CONTEXT   RecvContext;

    /** Internally used status field */
    NDIS_STATUS                 Status;

    /** The channel this MSDU was received on */
    UCHAR                       Channel;
    
    /** Link quality value for this MSDU */
    UCHAR                       LinkQuality;

    USHORT                      Flags;
    /** Structure used to maintain hardware's RX_MSDU data */
    PHW_RX_MSDU                 HwMsdu;
    
}MP_RX_MSDU, *PMP_RX_MSDU;


/*
 * This corresponds to the RX fragment that the miniport gets from the air.
 * This is a RX MPDU / NET_BUFFER
 */
typedef struct _MP_RX_MPDU    
{
    /** MP_RX_MSDU   corresponding to this fragment */
    PMP_RX_MSDU                 Msdu;

    /** Actual data for the fragment */
    PVOID                       Data;

    /** Length of the data */
    ULONG                       DataLength;

    /** Structure used to maintain hardware's RX_MPDU data */
    PHW_RX_MPDU                 HwMpdu;
    
}MP_RX_MPDU, *PMP_RX_MPDU;

//
// General TX/RX constants
//
#define MAX_SEND_MSDU_TO_PROCESS                5


#define NUM_RECV_PACKETS_TO_PROCESS_DEFAULT     10
#define NUM_RECV_PACKETS_TO_PROCESS_MIN         1
#define NUM_RECV_PACKETS_TO_PROCESS_MAX         50

