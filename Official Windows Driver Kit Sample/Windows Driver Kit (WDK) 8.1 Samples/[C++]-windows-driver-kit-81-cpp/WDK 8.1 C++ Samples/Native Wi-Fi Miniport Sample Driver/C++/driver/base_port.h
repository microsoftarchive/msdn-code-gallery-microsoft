/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    base_port.h

Abstract:
    Contains defines for base port functionality
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/

#pragma once



//
// Macros for send
//
#define     MP_GET_SEND_CONTEXT(_NBL)   ((PDOT11_EXTSTA_SEND_CONTEXT) NET_BUFFER_LIST_INFO(_NBL, MediaSpecificInformation))

// Time the send thread waits for the send trigger event (in milliseconds)
#define     MP_SEND_THREAD_SLEEP_TIME    2000

// Number of counts to sleep waiting for the send token
#define     MP_SEND_TOKEN_WAIT_COUNT   10

//
// Macros for receive
//
#define     MP_SET_RECEIVE_CONTEXT(_NBL, _RecvContext)  (NET_BUFFER_LIST_INFO(_NBL, MediaSpecificInformation) = _RecvContext)

VOID
BasePortFlushQueuedTxPackets(
    _In_  PMP_PORT                Port
    );

KSTART_ROUTINE BasePortDeferredSendThread;

