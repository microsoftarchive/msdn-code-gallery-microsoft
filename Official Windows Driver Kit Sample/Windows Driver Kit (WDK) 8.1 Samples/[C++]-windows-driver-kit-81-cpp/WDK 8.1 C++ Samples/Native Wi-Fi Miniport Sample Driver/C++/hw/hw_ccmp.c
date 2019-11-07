/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    Hw_ccmp.c

Abstract:
    Hw layer software-based CCMP encryption and decryption 
    functions
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    08-01-2005    Created

Notes:

--*/
#include "precomp.h"
#include <packon.h>

typedef struct CCMP_NONCE {
    UCHAR Priority: 4;
    UCHAR Rsvd: 4;
    DOT11_MAC_ADDRESS Address2;
    UCHAR PN[6];
} CCMP_NONCE, * PCCMP_NONCE;

typedef struct DOT11_AAD_ALL_FIELDS {
    DOT11_FRAME_CTRL        FrameControl;
    DOT11_MAC_ADDRESS       Address1;
    DOT11_MAC_ADDRESS       Address2;
    DOT11_MAC_ADDRESS       Address3;
    DOT11_SEQUENCE_CONTROL  SequenceControl;

    union {
        struct {
            DOT11_MAC_ADDRESS A4;
            DOT11_QOS_CONTROL A4QoSControl;
        };

        DOT11_QOS_CONTROL A3QoSControl;
    };
} DOT11_AAD_ALL_FIELDS, *PDOT11_AAD_ALL_FIELDS;

#include <packoff.h>

#define CCMP_NONCE_SIZE         13
#define CCMP_BSIZE              16
#define CCMP_KEY_SIZE           16
#define CCMP_MIC_SIZE           8
#define CCMP_HEADER_SIZE        8



NDIS_STATUS
HwDecryptCCMP(
    _In_  BCRYPT_KEY_HANDLE       hCNGKey,
    _Inout_updates_bytes_(BufferLength)  PUCHAR                  DataBuffer,
    _In_  ULONG                   BufferLength
    )
{
    NTSTATUS                    status = STATUS_SUCCESS;
    ULONG                       headerSize = 0;
    PDOT11_DATA_LONG_HEADER     MacHdr = NULL;
    CCMP_NONCE                  nonce = {0};
    DOT11_AAD_ALL_FIELDS        aad = {0};
    USHORT                      aadLength = 0;
    BCRYPT_AUTHENTICATED_CIPHER_MODE_INFO authCipherInfo = {0};
    ULONG                       cbResult = 0;

    //
    // NOTE: this function does not handle data frame with QoS field.
    //
    MPASSERT(hCNGKey != NULL);

    MacHdr = (PDOT11_DATA_LONG_HEADER)DataBuffer;
    
    if (MacHdr->FrameControl.FromDS && MacHdr->FrameControl.ToDS)
    {
        headerSize = sizeof(DOT11_DATA_LONG_HEADER);
        aadLength = FIELD_OFFSET(DOT11_AAD_ALL_FIELDS, A4QoSControl);
    }
    else
    {
        headerSize = sizeof(DOT11_DATA_SHORT_HEADER);
        aadLength = FIELD_OFFSET(DOT11_AAD_ALL_FIELDS, A3QoSControl);
    }

    //
    // Data must contain MAC header, 8-byte CCMP header, at least 1-byte of encrypted PDU, followed by
    // 8-byte MIC.
    //
    if (BufferLength <= headerSize + CCMP_HEADER_SIZE + CCMP_MIC_SIZE) 
    {
        return NDIS_STATUS_BUFFER_TOO_SHORT;
    }

    //
    // Validate CCMP header. Reserved field (3rd byte) must be 0.
    // In 4th byte of CCMP header, ExtIV must be 1 and rsvd must be 0
    //
    DataBuffer += headerSize;
    BufferLength -= headerSize;
    if (DataBuffer[2] != 0)
    {
        return NDIS_STATUS_FAILURE;
    }

    if ((DataBuffer[3] & 0x20) == 0)
    {
        return NDIS_STATUS_INVALID_PACKET;
    }

    //
    // Construct Nonce and AAD
    //
    nonce.Priority = 0;
    nonce.Rsvd = 0;
    nonce.PN[0] = DataBuffer[7];
    nonce.PN[1] = DataBuffer[6];
    nonce.PN[2] = DataBuffer[5];
    nonce.PN[3] = DataBuffer[4];
    nonce.PN[4] = DataBuffer[1];
    nonce.PN[5] = DataBuffer[0];
    NdisMoveMemory(nonce.Address2, MacHdr->Address2, sizeof(DOT11_MAC_ADDRESS));

    aad.FrameControl = MacHdr->FrameControl;
    // This is an intentional overflow since AAD has the same structure as MAC 
    // header
#pragma warning(push)    
#pragma warning(suppress:26000)
    NdisMoveMemory((char *)(&aad)+FIELD_OFFSET(DOT11_AAD_ALL_FIELDS, Address1), &(MacHdr->Address1), headerSize - FIELD_OFFSET(DOT11_MAC_HEADER, Address1));
#pragma warning(pop)

    aad.FrameControl.Subtype = 0;
    aad.FrameControl.Retry = 0;
    aad.FrameControl.PwrMgt = 0;
    aad.FrameControl.MoreData = 0;
    aad.SequenceControl.SequenceNumber = 0;

    DataBuffer += CCMP_HEADER_SIZE;
    BufferLength -= CCMP_HEADER_SIZE + CCMP_MIC_SIZE; // excluding MIC as well

    //
    // Set up the CNG structure
    //
    BCRYPT_INIT_AUTH_MODE_INFO(authCipherInfo);
    authCipherInfo.pbAuthData = (UCHAR*)&aad;
    authCipherInfo.cbAuthData = aadLength;
    authCipherInfo.pbTag = Add2Ptr(DataBuffer, BufferLength);
    authCipherInfo.cbTag = CCMP_MIC_SIZE;
    authCipherInfo.pbNonce = (PUCHAR)&nonce;
    authCipherInfo.cbNonce = CCMP_NONCE_SIZE;

    status = BCryptDecrypt(
        hCNGKey,
        DataBuffer,
        BufferLength,
        (PVOID)&authCipherInfo,
        NULL,
        0,
        DataBuffer,
        BufferLength,
        &cbResult,
        0
        );
    if (!NT_SUCCESS(status))
    {
        return NDIS_STATUS_FAILURE;
    }
    
    MPASSERT(cbResult == BufferLength);

    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
HwEncryptCCMP(
    _In_  PHW                     Hw,
    _In_  BCRYPT_KEY_HANDLE       hCNGKey,
    _Inout_ PNET_BUFFER           NetBuffer
    )
{
    NTSTATUS                    status = STATUS_SUCCESS;
    ULONG                       headerSize = 0;
    PDOT11_DATA_LONG_HEADER     MacHdr = NULL;
    CCMP_NONCE                  nonce = {0};
    DOT11_AAD_ALL_FIELDS        aad = {0};
    USHORT                      aadLength = 0;
    PUCHAR                      data = NULL;
    ULONG                       size = 0;
    PMDL                        mdl = NULL;
    PMDL                        dataMdl = NULL;
    ULONG                       offset = 0;
    ULONG                       dataMdlOffset = 0;
    BCRYPT_AUTHENTICATED_CIPHER_MODE_INFO   authCipherInfo = {0};
    ULONG                       cbResult = 0;
    PUCHAR                      contiguousDataBuffer = NULL;
    
    //
    // This function assumes the following:
    // 1. 802.11 header and the CCMP header are in the first MDL.
    // 2. Last MDL has exactly 8 bytes which will be used to store MIC.
    // 3. Only the first MDL is guaranteed to be mapped to system virtual space.
    // 4. The DataBuffer frame does not contain QoS field.
    //
    MPASSERT(hCNGKey != NULL);
    
    //
    // Find 802.11 dataBuffer header.
    //
    mdl = NET_BUFFER_CURRENT_MDL(NetBuffer);
    MacHdr = Add2Ptr(mdl->MappedSystemVa, NET_BUFFER_CURRENT_MDL_OFFSET(NetBuffer));
    ASSERT(MacHdr->FrameControl.WEP);

    //
    // Find CCMP header.
    //
    if (MacHdr->FrameControl.FromDS && MacHdr->FrameControl.ToDS)
    {
        headerSize = sizeof(DOT11_DATA_LONG_HEADER);
        aadLength = FIELD_OFFSET(DOT11_AAD_ALL_FIELDS, A4QoSControl);
    }
    else
    {
        headerSize = sizeof(DOT11_DATA_SHORT_HEADER);
        aadLength = FIELD_OFFSET(DOT11_AAD_ALL_FIELDS, A3QoSControl);
    }

    data = Add2Ptr(MacHdr, headerSize);

    //
    // Check the data length. It has to be bigger than MAC header, 8 bytes CCMP header and 8 bytes MIC.
    //
    size = NET_BUFFER_DATA_LENGTH(NetBuffer);
    if (size <= headerSize + CCMP_HEADER_SIZE + CCMP_MIC_SIZE)
    {
        return NDIS_STATUS_BUFFER_TOO_SHORT;
    }

    //
    // Construct Nonce and AAD
    //
    nonce.Priority = 0;
    nonce.Rsvd = 0;
    nonce.PN[0] = data[7];
    nonce.PN[1] = data[6];
    nonce.PN[2] = data[5];
    nonce.PN[3] = data[4];
    nonce.PN[4] = data[1];
    nonce.PN[5] = data[0];
    NdisMoveMemory(nonce.Address2, MacHdr->Address2, sizeof(DOT11_MAC_ADDRESS));

    aad.FrameControl = MacHdr->FrameControl;
    // This is an intentional overflow since AAD has the same structure as MAC 
    // header
    //
    // Copy all the addresses into the aad
    //
    #pragma prefast(push)
    #pragma prefast(suppress:26000, "buffer sizes are defined and checked already")
    NdisMoveMemory((char *)(&aad)+FIELD_OFFSET(DOT11_AAD_ALL_FIELDS, Address1), (char*)(&(MacHdr->Address1)), headerSize - FIELD_OFFSET(DOT11_MAC_HEADER, Address1));
    #pragma prefast(pop)
    
    // note: this also copies 8 more bytes beyond the three address fields, all the way to the end of the header    

    aad.FrameControl.Subtype = 0;
    aad.FrameControl.Retry = 0;
    aad.FrameControl.PwrMgt = 0;
    aad.FrameControl.MoreData = 0;
    aad.SequenceControl.SequenceNumber = 0;

    size -= headerSize + CCMP_HEADER_SIZE + CCMP_MIC_SIZE;

    //
    // Find the start of actual DataBuffer.
    //
    offset = NET_BUFFER_CURRENT_MDL_OFFSET(NetBuffer) + headerSize + 8;
    ASSERT(offset <= mdl->ByteCount);
    if (offset == mdl->ByteCount)
    {
        mdl = mdl->Next;
        ASSERT(mdl);
        if (!mdl)
        {
            return NDIS_STATUS_FAILURE;
        }
        
        offset = 0;
    }

    // Save the start of the data buffer since we will fill that in later
    dataMdl = mdl;    
    dataMdlOffset = offset;

    //
    // The crypto API do not allow CCM across multiple buffers. We 
    // will copy the data into a contiguous buffer & use that
    //
    contiguousDataBuffer = NdisAllocateFromNPagedLookasideList(&Hw->CryptoState.SoftEncryptLookaside);
    if (contiguousDataBuffer == NULL)
    {
        return NDIS_STATUS_RESOURCES;
    }
    
    //
    // Copy the data buffer into a contiguous buffer
    //
    data = contiguousDataBuffer;
    while (mdl->Next)
    {
        if (!NdisBufferVirtualAddressSafe(mdl, NormalPagePriority))
        {
            NdisFreeToNPagedLookasideList(&Hw->CryptoState.SoftEncryptLookaside, contiguousDataBuffer);
            return NDIS_STATUS_RESOURCES;
        }
                            
        NdisMoveMemory(data, Add2Ptr(mdl->MappedSystemVa, offset), mdl->ByteCount - offset);
        data += (mdl->ByteCount - offset);
        
        mdl = mdl->Next;
        offset = 0;
    }        

    // The MDL that we ended up at is the micMDL

    //
    // Set up the CNG structure
    //
    BCRYPT_INIT_AUTH_MODE_INFO(authCipherInfo);
    authCipherInfo.pbAuthData = (PUCHAR)&aad;
    authCipherInfo.cbAuthData = aadLength;
    authCipherInfo.cbTag = CCMP_MIC_SIZE;
    authCipherInfo.pbTag = mdl->MappedSystemVa; // We are at the MIC MDL
    authCipherInfo.pbNonce = (PUCHAR)&nonce;
    authCipherInfo.cbNonce = CCMP_NONCE_SIZE;

    //
    // Encrypt the payload 
    //
    status = BCryptEncrypt(
        hCNGKey,
        contiguousDataBuffer,
        size,
        (PVOID)&authCipherInfo,
        NULL,
        0,
        contiguousDataBuffer,
        size,
        &cbResult,
        0
        );
    if (!NT_SUCCESS(status))
    {
        NdisFreeToNPagedLookasideList(&Hw->CryptoState.SoftEncryptLookaside, contiguousDataBuffer);
        return NDIS_STATUS_FAILURE;        
    }
    MPASSERT(cbResult == size);

    // Copy the data from the buffer back into the original mdl
    mdl = dataMdl;
    offset = dataMdlOffset;

    data = contiguousDataBuffer;
    
    //
    // Copy the encrypted output from the contiguous buffer
    // into the NET_BUFFER_LIST MDL chain
    //
    while (mdl->Next)
    {
        NdisMoveMemory(Add2Ptr(mdl->MappedSystemVa, offset), data, mdl->ByteCount - offset);
        data += (mdl->ByteCount - offset);
        
        mdl = mdl->Next;
        offset = 0;
    }    

    NdisFreeToNPagedLookasideList(&Hw->CryptoState.SoftEncryptLookaside, contiguousDataBuffer);

    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
HwInitializeCrypto(
    _In_  PHW                     Hw
    )
{
    NTSTATUS                    ntStatus = STATUS_SUCCESS;
    BCRYPT_ALG_HANDLE           algoHandle = NULL;
    ULONG                       keyObjLen = 0;
    ULONG                       temp = 0;
    ULONG                       ndisVersion;

    do
    {
        Hw->CryptoState.AlgoHandle= NULL;
        Hw->CryptoState.KeyObjLen = 0;

        //
        // On Vista gold (RTM), the BCRYPT_CHAIN_MODE_CCM is not supported.
        // We cannot support WPA2 adhoc there
        //
        ndisVersion = NdisGetVersion();
        if (ndisVersion < MP_NDIS_VERSION_VISTASP1_SRV2008)
        {
            // Succeed silently
            MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("WPA2 Adhoc CNG algorithm/chain mode not supported in this OS"));
            break;
        }
    
        // Lookaside list for MP_TX_MSDU  
        NdisInitializeNPagedLookasideList(
            &Hw->CryptoState.SoftEncryptLookaside,
            NULL,
            NULL,
            0,
            MAX_TX_RX_PACKET_SIZE,
            HW_MEMORY_TAG,
            0
            );

        ntStatus = BCryptOpenAlgorithmProvider(
                        &algoHandle,
                        BCRYPT_AES_ALGORITHM,
                        MS_PRIMITIVE_PROVIDER, 
                        BCRYPT_PROV_DISPATCH 
                        );
        if (!NT_SUCCESS(ntStatus))
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to open CNG algorithm provider. Error = %d\n", ntStatus));
            break;
        }

        ntStatus = BCryptSetProperty(
                        algoHandle,
                        BCRYPT_CHAINING_MODE,
                        (PUCHAR)BCRYPT_CHAIN_MODE_CCM,
                        (ULONG)(sizeof(WCHAR)*(wcslen(BCRYPT_CHAIN_MODE_CCM)+1)),
                        0
                        );
        if (!NT_SUCCESS(ntStatus))
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to set CNG chaining mode property. Error = %d\n", ntStatus));
            break;
        }

        // Determine the object length
        ntStatus = BCryptGetProperty(
            algoHandle,
            BCRYPT_OBJECT_LENGTH,
            (PUCHAR)&keyObjLen,
            sizeof(ULONG),
            &temp,
            0
            );
        if (!NT_SUCCESS(ntStatus))
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to determine CNG key length. Error = %d\n", ntStatus));
            break;
        }

        Hw->CryptoState.AlgoHandle = algoHandle;
        Hw->CryptoState.KeyObjLen = keyObjLen;

        algoHandle = NULL;
    } while (FALSE);

    if (!NT_SUCCESS(ntStatus))
    {
        if (algoHandle) 
        {
            (VOID)BCryptCloseAlgorithmProvider(algoHandle, 0);
        }

        NdisDeleteNPagedLookasideList(&Hw->CryptoState.SoftEncryptLookaside);

        return NDIS_STATUS_FAILURE;
    }

    return NDIS_STATUS_SUCCESS;
}

VOID
HwTerminateCrypto(
    _In_  PHW                     Hw
    )
{
    if (Hw->CryptoState.AlgoHandle)
    {
        (VOID)BCryptCloseAlgorithmProvider(Hw->CryptoState.AlgoHandle, 0);
        Hw->CryptoState.AlgoHandle = NULL;

        NdisDeleteNPagedLookasideList(&Hw->CryptoState.SoftEncryptLookaside);
    }
}

