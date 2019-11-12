/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

_WdfVersionBuild_

Module Name: 

    device.h

Abstract:

    This module contains platform specific constants for DMA

Environment:

    kernel-mode only

Revision History:

--*/

//
// These are DMA specific values
// TODO: Change these values if necessary
//
#define UART_DMA_MAX_TRANSFER_LENGTH_BYTES 4096
#define UART_DMA_MAX_FRAGMENTS 1
#define UART_DMA_ADDRESS_HIGH_PART 0
#define UART_DMA_ADDRESS_LOW_PART 0
