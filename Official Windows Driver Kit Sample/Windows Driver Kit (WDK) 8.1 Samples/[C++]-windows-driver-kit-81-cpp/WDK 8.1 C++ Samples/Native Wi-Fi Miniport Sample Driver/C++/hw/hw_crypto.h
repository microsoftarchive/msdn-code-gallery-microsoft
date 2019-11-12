/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    hw_crypto.h

Abstract:
    Contains defines used for receive functionality 
    in the HW layer
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#pragma once

// Implementation in hw_ccmp.c
NDIS_STATUS
HwDecryptCCMP(
    _In_  BCRYPT_KEY_HANDLE       hCNGKey,
    _Inout_updates_bytes_(BufferLength)  PUCHAR                  DataBuffer,
    _In_  ULONG                   BufferLength
    );

// Implementation in hw_ccmp.c
NDIS_STATUS
HwEncryptCCMP(
    _In_  PHW                     Hw,
    _In_  BCRYPT_KEY_HANDLE       hCNGKey,
    _Inout_ PNET_BUFFER           NetBuffer
    );
    

// Implementation in hw_ccmp.c
NDIS_STATUS
HwInitializeCrypto(
    _In_  PHW                     Hw
    );
    
// Implementation in hw_ccmp.c
VOID
HwTerminateCrypto(
    _In_  PHW                     Hw
    );
    

NDIS_STATUS
HwCalculateTxMIC(
    _In_  PNET_BUFFER             NetBuffer,
    _In_  UCHAR                   Priority,
    _In_reads_bytes_(8)  PUCHAR                  MICKey,
    _Out_writes_bytes_(HW_MIC_LENGTH) PUCHAR                  MICData
    );

NDIS_STATUS
HwCalculateRxMIC(
    _In_  PHW_RX_MSDU             Msdu,
    _In_  UCHAR                   Priority,
    _In_reads_bytes_(8)  PUCHAR                  MICKey,
    _Out_writes_bytes_(HW_MIC_LENGTH) PUCHAR                  MICData
    );
