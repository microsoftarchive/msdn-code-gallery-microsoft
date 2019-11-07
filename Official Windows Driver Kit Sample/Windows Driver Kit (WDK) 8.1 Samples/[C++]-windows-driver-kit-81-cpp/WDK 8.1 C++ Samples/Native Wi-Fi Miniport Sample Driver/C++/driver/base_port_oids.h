/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    base_port_oids.h

Abstract:
    Contains the defines for the OID functionality for the base port class
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/

#pragma once


/**
 * Returns the amount of space required for the IE's to be copied
 * as part of EnumBSS buffer
 *
 * \param pStaBSSEntry      The BSS entry to copy
 * \return Number of bytes required to copy the IE blob 
 * \sa StaCopyEnummBSSIEBuffer
 */
ULONG
BasePortGetEnumBSSIELength(
    _In_  PMP_BSS_ENTRY           BSSEntry
    );

/**
 * Copies the IE blob from the BSS_ENTRY into the OS buffer for
 * EnumBSS
 * 
 * \param pStaBSSEntry      The BSS entry to copy
 * \param pDestBuffer       Buffer to copy the IE information into
 * \param DestinationLength Available length in buffer
 * \param BytesWritten      Number of bytes copied
 * \param BytesNeeded       Number of bytes need to bopy the buffer
 * \return NDIS_STATUS from the operation
 * \sa StaGetEnummBSSIELength
 */
NDIS_STATUS
BasePortCopyEnumBSSIEBuffer(
    _In_ PMP_BSS_ENTRY       pStaBSSEntry,
    _Out_writes_bytes_opt_(DestinationLength) PUCHAR               pDestBuffer,
    _In_ ULONG                DestinationLength,
    _Out_ PULONG               pBytesWritten,
    _Out_ PULONG               pBytesNeeded
    );

