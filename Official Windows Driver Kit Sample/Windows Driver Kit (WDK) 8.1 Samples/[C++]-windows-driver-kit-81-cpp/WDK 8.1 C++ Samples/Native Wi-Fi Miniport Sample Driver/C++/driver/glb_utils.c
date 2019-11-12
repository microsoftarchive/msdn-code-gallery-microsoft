/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    glb_utils.c

Abstract:
    Implements utility functions used by the whole driver
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#include "precomp.h"
#include "glb_utils.h"
#if DOT11_TRACE_ENABLED
#include "glb_utils.tmh"
#endif

ULONG
FASTCALL
MpInterlockedSetClearBits (
    _Inout_ PULONG Flags,
    _In_ ULONG sFlag,
    _In_ ULONG cFlag
    )
{

    ULONG NewFlags, OldFlags;

    OldFlags = * (volatile ULONG *) Flags;
    NewFlags = (OldFlags | sFlag) & ~cFlag;
    while (NewFlags != OldFlags) {
        NewFlags = InterlockedCompareExchange ((PLONG) Flags, (LONG) NewFlags, (LONG) OldFlags);
        if (NewFlags == OldFlags) {
            break;
        }

        OldFlags = NewFlags;
        NewFlags = (NewFlags | sFlag) & ~cFlag;
    }

    return OldFlags;
}

LONG 
MpInterlockedIncrementWrapped(PLONG pCurrentValue, LONG WrapLimit)
{
    LONG SnapshotValue, OldValue, NewValue;
    
    do
    {
        //
        // Take a snapshot of the current value.
        //
        SnapshotValue = *pCurrentValue;

        //
        // Do on local variable
        //
        NewValue = SnapshotValue + 1;
        if (NewValue >= WrapLimit)        // eg. 4+1 %4 = 1, %5 = 0
            NewValue = 0;

        //
        // Update current value but only if the current value is the
        // same as that obtained in the snapshot
        //
        OldValue = InterlockedCompareExchange(pCurrentValue,
            NewValue,
            SnapshotValue
            );

    } while (OldValue != SnapshotValue);

    // Return the value we updated
    return OldValue;
}

ULONG
MpReadRegistry(
    _In_        PVOID                   StructureStart,
    _In_opt_    NDIS_HANDLE             ConfigurationHandle,
    _In_        PMP_REG_ENTRY           RegKeyList,
    _In_        ULONG                   NumKeys
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PMP_REG_ENTRY               regEntry;
    ULONG                       i, valueRead, numValuesRead = 0;
    PUCHAR                      destination;
    PNDIS_CONFIGURATION_PARAMETER parameter = NULL;

    for (i = 0; i < NumKeys; i++)
    {
        regEntry= &RegKeyList[i];

        //
        // Read the registry value if the hConfigurationHandle is valid.
        //
        destination = ((PUCHAR)StructureStart) + regEntry->StructOffset + regEntry->FieldOffset;

        if (ConfigurationHandle != NULL)
        {

            //
            // Get the configuration value for a specific parameter.  Under NT the
            // parameters are all read in as DWORDs.
            //
            NdisReadConfiguration(&ndisStatus,
                &parameter,
                ConfigurationHandle,
                &regEntry->RegName,
                (enum _NDIS_PARAMETER_TYPE)regEntry->Type
                );

        }
        else
        {
            //
            // Use defaults
            //
            ndisStatus = NDIS_STATUS_FAILURE;            
        }

        if ((ndisStatus == NDIS_STATUS_SUCCESS) && (parameter != NULL))
        {
            if ((regEntry->Type == NdisParameterInteger) || (regEntry->Type == NdisParameterHexInteger))
            {
                if(parameter->ParameterData.IntegerData < regEntry->Min || 
                    parameter->ParameterData.IntegerData > regEntry->Max)
                {
                    MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("A bad value %d read from registry. Reverting to default value %d",
                                                parameter->ParameterData.IntegerData,
                                                regEntry->Default
                                                ));
                    valueRead = regEntry->Default;
                }
                else
                {
                    valueRead = parameter->ParameterData.IntegerData;
                    numValuesRead++;
                }
            }
            else
            {
                // We dont handle any other reads except for ints
                MPASSERT(FALSE);
                continue;
            }
        }
        else
        {
            MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("Unable to read from registry. Reverting to default value: %d\n",
                                        regEntry->Default
                                        ));

            if ((regEntry->Type == NdisParameterInteger) || (regEntry->Type == NdisParameterHexInteger))
            {
                valueRead = regEntry->Default;
            }
            else
            {
                // We dont handle defaults for any type other than ints
                MPASSERT(FALSE);
                continue;
            }
        }

        //
        // Moving the registry values in the adapter data structure
        //
        switch(regEntry->FieldSize)
        {
            case sizeof(UCHAR):
                *((PUCHAR) destination)  = (UCHAR) valueRead;
                break;
            case sizeof(USHORT):
                *((PUSHORT) destination) = (USHORT) valueRead;
                break;
            case sizeof(ULONG):
                *((PULONG) destination)  = (ULONG) valueRead;
                break;
            default:
                MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Bogus field size %d\n", regEntry->FieldSize));
                break;
        }

    }

    return numValuesRead;
}

NDIS_STATUS
Dot11CopyMdlToBuffer(
    _Inout_updates_(_Inexpressible_("Varies")) PNDIS_BUFFER * ppMdlChain,
    ULONG uOffset,
    PVOID pvBuffer,
    ULONG uBytesToCopy,
    PULONG puLastWalkedMdlOffset,
    PULONG puBytesCopied
    )
/*++

Routine Description:

    Copies a maximum of uBytesToCopy bytes of data from an MDL chain
    to a flat buffer.

Arguments:

    ppMdlChain - Pointer to a pointer to a chain of MDLs describing the source
                 data. On successfully copying uBytesToCopy bytes of data, this
                 points to the last walked MDL.

    uOffset - Number of initial bytes to skip in the MDL chain.

    pvBuffer - Pointer to the flat buffer to copy into.

    uBytesToCopy - Number of bytes to copy.

    puLastWalkedMdlOffset - Pointer to a location that will contain the offset
                            into the last walked MDL from where the next send
                            buffer can be retrieved.

    puBytesCopied - Pointer to a location to contain the actual number of bytes
                    copied.

Return Value:

    Success - STATUS_SUCCESS.

    Failure - NT STATUS FAILURE CODE.

--*/
{
    PNDIS_BUFFER pMdl = *ppMdlChain;
    ULONG uMdlOffset = uOffset;
    ULONG uMdlByteCount = 0, uDummy;
    ULONG uNumBytes = uBytesToCopy;
    PUCHAR pucSysVa = NULL;
    ULONG uCopySize = 0;
    PNDIS_BUFFER pLastWalkedMdl = pMdl; // Initialize to pMdl to avoid Prefast warning


    *puBytesCopied = 0;

    if (pMdl == NULL)
    {
        if (uBytesToCopy == 0)
        {
            return NDIS_STATUS_SUCCESS;
        }
        else
        {
            return NDIS_STATUS_FAILURE;
        }
    }

    //
    // Skip the offset bytes in the MDL chain.
    //    
    while (pMdl) {
        NdisQueryBufferSafe(pMdl, NULL, (PUINT)&uMdlByteCount, LowPagePriority);
        if (uMdlOffset >= uMdlByteCount)
        {
            uMdlOffset -= uMdlByteCount;
            pMdl = pMdl->Next;
        }
        else
            break;
    }

    while (pMdl) {

        if (uNumBytes > 0) {
            NdisQueryBufferSafe(pMdl, NULL, (PUINT)&uMdlByteCount, LowPagePriority); 
            if (uMdlByteCount == 0) {
                pLastWalkedMdl = pMdl;
                pMdl = pMdl->Next;
                continue;
            }

            NdisQueryBufferSafe(pMdl, &pucSysVa, (PUINT)&uDummy, LowPagePriority);
            if (!pucSysVa) {
                return (STATUS_INSUFFICIENT_RESOURCES);
            }

            pucSysVa += uMdlOffset;
            uMdlByteCount -= uMdlOffset;
            uMdlOffset = 0;

            //
            // uMdlByteCount can never be zero because at this point its always
            // greater than uMdlOffset.
            //

            uCopySize = (uNumBytes < uMdlByteCount ? uNumBytes: uMdlByteCount);
            NdisMoveMemory(pvBuffer, pucSysVa, uCopySize);
            pvBuffer = (PVOID) ((PUCHAR) pvBuffer + uCopySize);
            uNumBytes -= uCopySize;

            pLastWalkedMdl = pMdl;
            pMdl = pMdl->Next;
        }
        else
            break;
    }

    if (!uNumBytes) {

        if (uCopySize < uMdlByteCount) {
            *ppMdlChain = pLastWalkedMdl;
            *puLastWalkedMdlOffset = uCopySize;
        }
        else {
            pLastWalkedMdl = pLastWalkedMdl->Next;
            *ppMdlChain = pLastWalkedMdl;
            *puLastWalkedMdlOffset = 0;
        }
    }

    *puBytesCopied = uBytesToCopy - uNumBytes;

    return (STATUS_SUCCESS);
}


NDIS_STATUS
Dot11GetDataFromMdlChain(
    _Inout_updates_(_Inexpressible_("Varies"))  PNDIS_BUFFER * ppMdlChain,
    ULONG uOffset,
    ULONG uBytesNeeded,
    PVOID pvStorage,
    PULONG puLastWalkedMdlOffset,
    PUCHAR * ppucReturnBuf
    )
/*++

Routine Description:

    Retrieves data from an MDL chain.
    If the desired data is contiguous, then just returns
    a pointer directly to the data in the buffer chain.
    Otherwise the data is copied to the supplied buffer
    and returns a pointer to the supplied buffer.

Arguments:

    ppMdlChain - Pointer to a pointer to a chain of MDLs describing the source
                 data. On successful return, this points to the last walked MDL.

    uOffset - Number of initial bytes to skip in the MDL chain.

    uBytesNeeded - Number of bytes needed from the specified offset.

    pvStorage - Pointer to the flat buffer of uBytesNeeded size.
                Client of this call should free this buffer only when its
                done using *ppucReturnBuf.

    puLastWalkedMdlOffset - Pointer to a location that will contain the offset
                            into the last walked MDL from where the next send
                            buffer can be retrieved.

    ppucReturnBuf - Pointer to a location that will contain the pointer to
                    the flat buffer. Must not be freed by the client.
Return Value:

    Success - STATUS_SUCCESS.

    Failure - NT STATUS FAILURE CODE.

--*/
{
    NDIS_STATUS ndisStatus = STATUS_SUCCESS;

    PNDIS_BUFFER pMdl = *ppMdlChain;
    ULONG uMdlOffset = uOffset;

    ULONG uMdlByteCount = 0, uDummy;
    PUCHAR pucVa = NULL;
    PUCHAR pucReturnBuf = NULL;
    ULONG uLastWalkedMdlOffset = 0;
    ULONG uBytesCopied = 0;


    //
    // Find which MDL.
    //

    if (!pMdl) {
        ndisStatus = STATUS_BUFFER_TOO_SMALL;
        goto error;
    }

    NdisQueryBufferSafe(pMdl, NULL, (PUINT)&uMdlByteCount, NormalPagePriority);
    while (uMdlOffset >= uMdlByteCount) {
        uMdlOffset -= uMdlByteCount;
        pMdl = pMdl->Next;
        if (!pMdl) {
            ndisStatus = STATUS_BUFFER_TOO_SMALL;
            goto error;
        }
        else {
            NdisQueryBufferSafe(pMdl, NULL, (PUINT)&uMdlByteCount, NormalPagePriority);
        }
    }

    //
    // See if the found MDL contains uMdlOffset + uBytesNeeded bytes of data.
    //
    if (uMdlOffset + uBytesNeeded <= uMdlByteCount) {

        NdisQueryBufferSafe(pMdl, &pucVa, (PUINT)&uDummy, LowPagePriority);
        if (!pucVa) {
            ndisStatus = STATUS_INSUFFICIENT_RESOURCES;
            goto error;
        }
        pucReturnBuf = pucVa + uMdlOffset;

        if (uMdlOffset + uBytesNeeded < uMdlByteCount) {
            uLastWalkedMdlOffset = uMdlOffset + uBytesNeeded;
        }
        else {
            pMdl = pMdl->Next;
            uLastWalkedMdlOffset = 0;
        }
    }
    else {

        if (!pvStorage) {
            ndisStatus = STATUS_BUFFER_OVERFLOW;
            goto error;
        }

        ndisStatus = Dot11CopyMdlToBuffer(
                       &pMdl,
                       uMdlOffset,
                       pvStorage,
                       uBytesNeeded,
                       &uLastWalkedMdlOffset,
                       &uBytesCopied
                       );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
            goto error;

        if (uBytesCopied != uBytesNeeded) {
            ndisStatus = STATUS_BUFFER_TOO_SMALL;
            goto error;
        }
        pucReturnBuf = pvStorage;

    }

    if (ppMdlChain) {
        *ppMdlChain = pMdl;
    }
    if (puLastWalkedMdlOffset) {
        *puLastWalkedMdlOffset = uLastWalkedMdlOffset;
    }
    if (ppucReturnBuf) {
        *ppucReturnBuf = pucReturnBuf;
    }
    return (ndisStatus);

error:

    *puLastWalkedMdlOffset = 0;
    *ppucReturnBuf = NULL;
    return (ndisStatus);
}

NDIS_STATUS
Dot11GetMacHeaderFromNB(
    _In_  PNET_BUFFER                     NetBuffer,
    _In_  PDOT11_MAC_HEADER*              ppDot11MacHeader
    )
{
    *ppDot11MacHeader = Dot11GetNetBufferData(NetBuffer, sizeof(DOT11_MAC_HEADER));
    if (*ppDot11MacHeader == NULL)
    {
        return NDIS_STATUS_RESOURCES;
    }
    
    return NDIS_STATUS_SUCCESS;
}

PVOID
Dot11GetNetBufferData(
    _In_  PNET_BUFFER                     NetBuffer,
    _In_  ULONG                           BytesNeeded
    )
{
    ULONG           mdlDataLength;
    PUCHAR          mdlDataPointer = NULL;

    // If we have already reached the end
    if(NET_BUFFER_CURRENT_MDL(NetBuffer) == NULL)
    {
        return NULL;
    }   

    // Check if we have enough contiguous space for this MDL
    NdisQueryMdl(NET_BUFFER_CURRENT_MDL(NetBuffer), &mdlDataPointer, &mdlDataLength, NormalPagePriority);
    if (mdlDataPointer == NULL)
    {
        return NULL;
    }
    
    if ((mdlDataLength - NET_BUFFER_CURRENT_MDL_OFFSET(NetBuffer)) < BytesNeeded)
    {
        // Not enough space available
        return NULL;
    }

    // Return data = MDL Data start + Offset
    return (mdlDataPointer + NET_BUFFER_CURRENT_MDL_OFFSET(NetBuffer));
}

NDIS_STATUS
Dot11ValidateInfoBlob(
    _In_reads_bytes_(uSizeOfBlob) PUCHAR pucInfoBlob,
    _In_ ULONG uSizeOfBlob
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_INFO_ELEMENT pInfoEleHdr = NULL;
    ULONG uRequiredSize = 0;

    while(uSizeOfBlob) {

        pInfoEleHdr = (PDOT11_INFO_ELEMENT)pucInfoBlob;
        if (uSizeOfBlob < sizeof(DOT11_INFO_ELEMENT)) {
            break;
        }

        uRequiredSize = (ULONG)(pInfoEleHdr->Length) + sizeof(DOT11_INFO_ELEMENT);
        if (uSizeOfBlob < uRequiredSize) {
            break;
        }

        uSizeOfBlob -= uRequiredSize;
        pucInfoBlob += uRequiredSize;
    }

    if (uSizeOfBlob) {
        ndisStatus = NDIS_STATUS_FAILURE;
    }
    return ndisStatus;
}


/**
Mitesh: Update documentation
    Validate the info blob by walking through the information
    element chain.

    This routine return the size of information element blob
    that it walks thru. In case of malformed beacon, caller
 */
/**
 * 
 * 
 * \param pPacketBuffer
 * \param PacketLength
 * \param OffsetOfInfoElemBlob
 * \param pInfoElemBlobSize
 * \return 
 * \sa 
 */

NDIS_STATUS
Dot11GetInfoBlobSize(
    _In_reads_bytes_(PacketLength)  PUCHAR                  pPacketBuffer,
    _In_  ULONG                   PacketLength,
    _In_  ULONG                   OffsetOfInfoElemBlob,
    _Out_ PULONG                  pInfoElemBlobSize
    )
{
    NDIS_STATUS         ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_INFO_ELEMENT pInfoElemHdr = NULL;
    ULONG               RemainingLength = PacketLength - OffsetOfInfoElemBlob;
    ULONG               InfoBlobSize = 0;

    pPacketBuffer += OffsetOfInfoElemBlob;
    
    // Walk through to check nothing is malformed
    while (RemainingLength >= sizeof(DOT11_INFO_ELEMENT))
    {
        // Atleast one more header is present
        pInfoElemHdr = (PDOT11_INFO_ELEMENT)(pPacketBuffer + InfoBlobSize);
        RemainingLength -= sizeof(DOT11_INFO_ELEMENT);
        InfoBlobSize += sizeof(DOT11_INFO_ELEMENT);
        
        if (pInfoElemHdr->Length == 0)
        {
            continue;
        }

        if (RemainingLength < pInfoElemHdr->Length)
        {
            // Incomplete/bad info element
            ndisStatus = NDIS_STATUS_NOT_ACCEPTED;
            break;
        }

        // Consume info element 
        RemainingLength -= pInfoElemHdr->Length;
        InfoBlobSize += pInfoElemHdr->Length;
    }

    *pInfoElemBlobSize = InfoBlobSize;
    return ndisStatus;
}

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
Dot11GetInfoEle(
    _In_reads_bytes_(uSizeOfBlob) PUCHAR pucInfoBlob,
    _In_ ULONG uSizeOfBlob,
    _In_ UCHAR ucInfoId,
    _Out_ PUCHAR pucLength,
    _Outptr_result_bytebuffer_(*pucLength) PVOID * ppvInfoEle
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_INFO_ELEMENT pInfoEleHdr = NULL;
    ULONG uRequiredSize = 0;
    BOOLEAN bFound = FALSE;

    *pucLength = 0;
    *ppvInfoEle = NULL;
    while(uSizeOfBlob) {

        pInfoEleHdr = (PDOT11_INFO_ELEMENT)pucInfoBlob;
        if (uSizeOfBlob < sizeof(DOT11_INFO_ELEMENT)) {
            break;
        }

        uRequiredSize = (ULONG)(pInfoEleHdr->Length) + sizeof(DOT11_INFO_ELEMENT);
        if (uSizeOfBlob < uRequiredSize) {
            break;
        }

        if (pInfoEleHdr->ElementID == ucInfoId) {
            *pucLength = pInfoEleHdr->Length;
            *ppvInfoEle = pucInfoBlob + sizeof(DOT11_INFO_ELEMENT);
            bFound = TRUE;
            break;
        }

        uSizeOfBlob -= uRequiredSize;
        pucInfoBlob += uRequiredSize;
    }

    if (!bFound) {
        ndisStatus = NDIS_STATUS_FAILURE;
    }
    return ndisStatus;
}

NDIS_STATUS
Dot11ValidatePacketInfoBlob(
    _In_ PNDIS_BUFFER pMdlHead,
    _In_ ULONG uOffsetOfInfoEleBlob
    )
/*++

Routine Description:

    Validate the info blob by walking through the information
    element chain.

Arguments:

Return Value:

--*/
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    DOT11_INFO_ELEMENT InfoEleHdr = { 0 };
    PDOT11_INFO_ELEMENT pInfoEleHdr = NULL;
    ULONG uCurrentOffset = uOffsetOfInfoEleBlob;
    UCHAR ucLastByte = 0;
    PUCHAR pucLastByte = NULL;

    while(pMdlHead) {
        pInfoEleHdr = NULL;
        ndisStatus = Dot11GetDataFromMdlChain(
                            &pMdlHead,
                            uCurrentOffset,
                            sizeof(DOT11_INFO_ELEMENT),
                            &InfoEleHdr,
                            &uCurrentOffset,
                            (PVOID)&pInfoEleHdr
                            );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
            goto error;

        MPASSERT (NULL != pInfoEleHdr);

        if (pInfoEleHdr->Length == 0) {
            continue;
        }

        //
        // Try to read the last byte of the information element
        //
        uCurrentOffset += pInfoEleHdr->Length - 1;

        pucLastByte = NULL;
        ndisStatus = Dot11GetDataFromMdlChain(
                            &pMdlHead,
                            uCurrentOffset,
                            1,      // only one byte
                            &ucLastByte,
                            &uCurrentOffset,
                            (PVOID)&pucLastByte
                            );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
            goto error;

        //
        // The buffer should be always contiguous since
        // we read only one bye.
        //
        MPASSERT(pucLastByte != &ucLastByte);
    }

error:
    return ndisStatus;
}

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
Dot11GetInfoEleFromPacket(
    _In_    PNDIS_BUFFER pMdlHead,
    _In_    ULONG uOffsetOfInfoEleBlob,
    _In_    UCHAR ucInfoId,
    _In_    UCHAR ucMaxLength,
    _Out_   PUCHAR pucLength,
    _Inout_ PVOID * ppvInfoEle
    )
/*++

Routine Description:

    This routine copy information element from an MDL chain

Arguments:

    pMdlHead    the MDL chain

    uOffsetOfInfoEleBlob    the offset of the information element blob in the packet

    ucInfoId    the ID of the information element to be look up

    ucMaxLength the size of the buffer pointed by (*ppvInfoEle)

    pucLength   when the function is successfully completed, it will
                hole the size of the informaion element.

    ppvInfoEle  as input, (*ppvInfoEle) specify the pointer to the storage to be used
                in case of the information element buffer is discontiguous
                as output, (*ppvInfoEle) stores the pointer to the returned information blob.
                it could the same input pointer if the buffer is discontiguous. Otherwise,
                it will point to the buffer in the packet.

Return Value:

--*/
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    DOT11_INFO_ELEMENT InfoEleHdr = { 0 };
    PDOT11_INFO_ELEMENT pInfoEleHdr = NULL;
    ULONG uCurrentOffset = uOffsetOfInfoEleBlob;

    ASSERT(ppvInfoEle && *ppvInfoEle);

    while(1) {
        pInfoEleHdr = NULL;
        ndisStatus = Dot11GetDataFromMdlChain(
                            &pMdlHead,
                            uCurrentOffset,
                            sizeof(DOT11_INFO_ELEMENT),
                            &InfoEleHdr,
                            &uCurrentOffset,
                            (PVOID)&pInfoEleHdr
                            );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
            goto error;

        ASSERT (NULL != pInfoEleHdr);

        if (pInfoEleHdr->ElementID == ucInfoId) {
            break;
        }

        //
        // Skip the information content and move the pointer
        // to the header of next information element.
        //
        uCurrentOffset += pInfoEleHdr->Length;
    }

    *pucLength = pInfoEleHdr->Length;
    if (pInfoEleHdr->Length > ucMaxLength) {
        ndisStatus = NDIS_STATUS_BUFFER_TOO_SHORT;
        goto error;
    }

    ndisStatus = Dot11GetDataFromMdlChain(
                            &pMdlHead,
                            uCurrentOffset,
                            pInfoEleHdr->Length,
                            *ppvInfoEle,
                            &uCurrentOffset,
                            (PUCHAR*)ppvInfoEle
                            );
    if (ndisStatus != NDIS_STATUS_SUCCESS)
        goto error;

error:
    return ndisStatus;
}

NDIS_STATUS
Dot11CopySSIDFromPacket(
    _In_ PNDIS_BUFFER pMdlHead,
    _In_ ULONG uOffsetOfInfoEleBlob,
    _Inout_ PDOT11_SSID pDot11SSID
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    UCHAR ucLength = 0;
    PVOID pvStorage = pDot11SSID->ucSSID;

    ndisStatus = Dot11GetInfoEleFromPacket(
                    pMdlHead,
                    uOffsetOfInfoEleBlob,
                    DOT11_INFO_ELEMENT_ID_SSID,
                    sizeof(pDot11SSID->ucSSID),
                    &ucLength,
                    &pvStorage
                    );
    if (ndisStatus != NDIS_STATUS_SUCCESS)
        goto error;

    if (ucLength > 32)
    {
        // Invalid SSID
        ndisStatus = NDIS_STATUS_INVALID_DATA;
        goto error;
    }

    pDot11SSID->uSSIDLength = ucLength;
    if (pvStorage != pDot11SSID->ucSSID) {
        memcpy(pDot11SSID->ucSSID, pvStorage, ucLength);
    }

error:
    return ndisStatus;
}

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
Dot11GetChannelForDSPhy(
    _In_reads_bytes_(uSizeOfBlob) PUCHAR pucInfoBlob,
    _In_ ULONG uSizeOfBlob,
    _Out_ PUCHAR pucChannel
    )
{
    UCHAR ucLength = 0;
    PUCHAR pucParam = NULL;
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    ndisStatus = Dot11GetInfoEle(
                    pucInfoBlob,
                    uSizeOfBlob,
                    DOT11_INFO_ELEMENT_ID_DS_PARAM_SET,
                    &ucLength,
                    &pucParam
                    );
    if (ndisStatus != NDIS_STATUS_SUCCESS)
        goto error;

    if (ucLength != 1) {
        ndisStatus = NDIS_STATUS_INVALID_LENGTH;
        goto error;
    }
    if (NULL == pucParam) {
        ndisStatus = NDIS_STATUS_FAILURE;
        goto error;
    }

    *pucChannel = *pucParam;

error:
    return ndisStatus;
}

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
Dot11CopySSIDFromMemoryBlob(
    _In_reads_bytes_(uSizeOfBlob) PUCHAR pucInfoBlob,
    _In_ ULONG uSizeOfBlob,
    _Out_ PDOT11_SSID pDot11SSID
    )
{
    UCHAR ucLength = 0;
    PUCHAR pucParam = NULL;
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    ndisStatus = Dot11GetInfoEle(
                    pucInfoBlob,
                    uSizeOfBlob,
                    DOT11_INFO_ELEMENT_ID_SSID,
                    &ucLength,
                    &pucParam
                    );
    if ((ndisStatus != NDIS_STATUS_SUCCESS) || (NULL == pucParam))
    {
        ndisStatus = (ndisStatus != NDIS_STATUS_SUCCESS) ? ndisStatus : NDIS_STATUS_FAILURE;
        goto error;
    }

    if (ucLength > DOT11_SSID_MAX_LENGTH) {
        ndisStatus = NDIS_STATUS_INVALID_LENGTH;
        goto error;
    }

    pDot11SSID->uSSIDLength = (ULONG)ucLength;
    RtlCopyMemory(pDot11SSID->ucSSID, pucParam, pDot11SSID->uSSIDLength);

error:
    return ndisStatus;
}

_Success_(return == NDIS_STATUS_SUCCESS)
_At_(*pusBlobSize,
        _In_range_(>=, 2U + ucElementLength))
_At_(*ppucBlob, _Writes_and_advances_ptr_(*pusBlobSize))
NDIS_STATUS
Dot11AttachElement(
    _Inout_ PUCHAR *ppucBlob,
    _Inout_ USHORT *pusBlobSize,
    _In_    UCHAR ucElementId,
    _In_    UCHAR ucElementLength,
    _In_reads_bytes_(ucElementLength)
            PUCHAR pucElementInfo
    )
{
    PUCHAR      pucBlob = *ppucBlob;
    USHORT      usBlobSize = *pusBlobSize;

    if (usBlobSize < (2U + ucElementLength))
    {
        return STATUS_BUFFER_TOO_SMALL;
    }

    *pucBlob = ucElementId;
    pucBlob++; 
    usBlobSize--;
    
    *pucBlob = ucElementLength;
    pucBlob++; 
    usBlobSize--;
    
    memcpy(pucBlob, pucElementInfo, ucElementLength);
    pucBlob = pucBlob + ucElementLength;
    usBlobSize = usBlobSize - ucElementLength;

    *ppucBlob = pucBlob;
    *pusBlobSize = usBlobSize;

    return NDIS_STATUS_SUCCESS;
}

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
Dot11CopyInfoEle(
    _In_reads_bytes_(uSizeOfBlob) PUCHAR pucInfoBlob,
    _In_ ULONG uSizeOfBlob,
    _In_ UCHAR ucInfoId,
    _Out_ PUCHAR pucLength,
    _In_ ULONG uBufSize,
    _Out_ PVOID pvInfoEle
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    UCHAR ucLength = 0;
    PVOID pvTemp = NULL;

    ndisStatus = Dot11GetInfoEle(
            pucInfoBlob,
            uSizeOfBlob,
            ucInfoId,
            &ucLength,
            &pvTemp
            );
    if ((ndisStatus != NDIS_STATUS_SUCCESS) || (NULL == pvTemp))
    {
        ndisStatus = (ndisStatus != NDIS_STATUS_SUCCESS) ? ndisStatus : NDIS_STATUS_FAILURE;
        goto error;
    }

    *pucLength = ucLength;
    if (uBufSize < ucLength) {
        ndisStatus = STATUS_BUFFER_TOO_SMALL;
        goto error;
    }

    RtlCopyMemory(
            pvInfoEle,
            pvTemp,
            ucLength
            );

error:
    return ndisStatus;
}

PUCHAR
Dot11GetBSSID(
    _In_reads_bytes_(fragmentLength) PUCHAR fragmentHeader,
    _In_ ULONG fragmentLength
    )
{
    PUCHAR              pBssId = NULL;
    PDOT11_MAC_HEADER   macHeader;
    BOOLEAN             fBSSIdValid = FALSE;
    USHORT              usBSSIdOffset = 0;

    macHeader = (PDOT11_MAC_HEADER)fragmentHeader;
    MPASSERT(macHeader != NULL);

    switch(macHeader->FrameControl.Type)
    {
    case DOT11_FRAME_TYPE_CONTROL:    
        switch (macHeader->FrameControl.Subtype)
        {
            case DOT11_CTRL_SUBTYPE_RTS:
            case DOT11_CTRL_SUBTYPE_CTS:
            case DOT11_CTRL_SUBTYPE_ACK:
                // no bssid field
                break;
                
            case DOT11_CTRL_SUBTYPE_PS_POLL:
                fBSSIdValid = TRUE;
                usBSSIdOffset = 4; // BSSId is the 1st address field
                break;

            case DOT11_CTRL_SUBTYPE_CFE:
            case DOT11_CTRL_SUBTYPE_CFE_CFA:
                fBSSIdValid = TRUE;
                usBSSIdOffset = 10; // BSSId is the 2nd address field
                break;
        }
        break;
        
    case DOT11_FRAME_TYPE_MANAGEMENT:        
        fBSSIdValid = TRUE;
        usBSSIdOffset = 16; // BSSId is the 3rd address field

        break;
        
    case DOT11_FRAME_TYPE_DATA:
        if (macHeader->FrameControl.ToDS)
        {
            if (macHeader->FrameControl.FromDS)
            {
                // ToDS = 1, FromDS = 1
                // no bssid
            }
            else
            {
                // ToDS = 1, FromDS = 0
                fBSSIdValid = TRUE;
                usBSSIdOffset = 4; // BSSId is the 1st address field
            }
        }
        else
        {
            if (macHeader->FrameControl.FromDS)
            {
                // ToDS = 0, FromDS = 1
                fBSSIdValid = TRUE;
                usBSSIdOffset = 10; // BSSId is the 2nd address field
            }
            else
            {
                // ToDS = 0, FromDS = 0
                fBSSIdValid = TRUE;
                usBSSIdOffset = 16; // BSSId is the 3rd address field
            }
        }        
        break;
        
    default:
        // no bssid to report
        break;
    }

    if (fBSSIdValid && (usBSSIdOffset > fragmentLength - 6))
    {
        return NULL;
    }

    pBssId = fragmentHeader + usBSSIdOffset;
    
    return pBssId;
}

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
Dot11GetWPAIE(
    _In_reads_bytes_(uSizeOfBlob) PUCHAR pucInfoBlob,
    _In_ ULONG uSizeOfBlob,
    _Out_ PUCHAR pucLength,
    _Outptr_result_bytebuffer_(*pucLength) PVOID * ppvInfoEle
    )
{
    UCHAR ucLength = 0;
    PUCHAR pucParam = NULL;
    PUCHAR InfoElemPtr;
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    InfoElemPtr = pucInfoBlob;
    while (TRUE)
    {
        ndisStatus = Dot11GetInfoEle(InfoElemPtr,
                                     uSizeOfBlob - PtrOffset(pucInfoBlob, InfoElemPtr),
                                     DOT11_INFO_ELEMENT_ID_VENDOR_SPECIFIC,
                                     &ucLength,
                                     &pucParam);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
            goto error;

        if (NULL == pucParam) 
        {
            ndisStatus = NDIS_STATUS_FAILURE;
            goto error;
        }

        //
        // WPA IE contains 4 bytes WPA_IE_TAG (0x01f25000) at the very beginning of the IE data. Check for the tag.
        // If we don't find the tag, it's not a WPA IE. Keep searching.
        //
        if (ucLength < 4 || *((ULONG UNALIGNED *)pucParam) != WPA_IE_TAG)
        {
            InfoElemPtr = pucParam + ucLength;
            continue;
        }

        ucLength -= 4;
        pucParam += 4;

        break;
    }

    *ppvInfoEle = pucParam;
    *pucLength = ucLength;

error:
    return ndisStatus;
}


BOOLEAN
Dot11IsHiddenSSID(
    _In_reads_bytes_(SSIDLength) PUCHAR               SSID,
    _In_ ULONG                SSIDLength
    )
{
    ULONG i;
    //
    // We flag this as Hidden SSID if the Length is 0
    // of the SSID only contains 0's
    //
    if (SSIDLength == 0)
    {
        // Zero length is hidden SSID
        return TRUE;
    }

    for (i = 0; i < SSIDLength; i++)
    {
        if (SSID[i] != 0)
        {
            // Non zero SSID value
            return FALSE;
        }
    }

    // All 0's
    return TRUE;
}

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
Dot11GetRateSetFromInfoEle(
    _In_reads_bytes_(InfoElemBlobSize)  PUCHAR InfoElemBlobPtr,
    _In_  ULONG InfoElemBlobSize,
    _In_  BOOLEAN basicRateOnly,
    _Inout_ PDOT11_RATE_SET rateSet
    )
{
    NDIS_STATUS ndisStatus;
    PUCHAR      tmpPtr;
    PUCHAR      rate = rateSet->ucRateSet;
    UCHAR       size;
    UCHAR       remainingLength = DOT11_RATE_SET_MAX_LENGTH;

    ndisStatus = Dot11GetInfoEle(InfoElemBlobPtr,
                                 InfoElemBlobSize,
                                 DOT11_INFO_ELEMENT_ID_SUPPORTED_RATES,
                                 &size,
                                 &tmpPtr);
    if ((ndisStatus != NDIS_STATUS_SUCCESS) || (NULL == tmpPtr))
    {
        ndisStatus = (ndisStatus != NDIS_STATUS_SUCCESS) ? ndisStatus : NDIS_STATUS_FAILURE;
        return ndisStatus;
    }

    while ((size > 0) && (remainingLength > 0))
    {
        if (!basicRateOnly || (*tmpPtr & 0x80)) 
        {
            *rate = *tmpPtr;
            rate++;
            remainingLength--;
        }

        tmpPtr++;
        size--;
    }

    //
    // If size > 0, it means we have only copied a portion of the rates into our buffer
    //

    //
    // 802.11g make copy of extended supported rates as well
    //

    ndisStatus = Dot11GetInfoEle(InfoElemBlobPtr,
                                 InfoElemBlobSize,
                                 DOT11_INFO_ELEMENT_ID_EXTD_SUPPORTED_RATES,
                                 &size,
                                 &tmpPtr);

    if ((ndisStatus == NDIS_STATUS_SUCCESS) && tmpPtr) 
    {
        while ((size > 0) && (remainingLength > 0))
        {
            if (!basicRateOnly || (*tmpPtr & 0x80)) 
            {
                *rate = *tmpPtr;
                rate++;
                remainingLength--;
            }

            tmpPtr++;
            size--;
        }
    }
    //
    // If size > 0, it means we have only copied a portion of the rates into our buffer
    //

    rateSet->uRateSetLength = (ULONG)PtrOffset(rateSet->ucRateSet, rate);
    return NDIS_STATUS_SUCCESS;
}


VOID
Dot11GenerateRandomBSSID(
    _In_reads_bytes_(DOT11_ADDRESS_SIZE)  DOT11_MAC_ADDRESS MACAddr,
    _Out_writes_bytes_(DOT11_ADDRESS_SIZE) DOT11_MAC_ADDRESS BSSID
    )
{
    ULONGLONG   time;

    //
    // Initialize the random BSSID to be the same as MAC address.
    //

    RtlCopyMemory(BSSID, MACAddr, sizeof(DOT11_MAC_ADDRESS));

    //
    // Get the system time in 10 millisecond.
    //

    NdisGetCurrentSystemTime((PLARGE_INTEGER)&time);
    time /= 100000;

    //
    // Randomize the first 4 bytes of BSSID.
    //

    BSSID[0] ^= (UCHAR)(time & 0xff); 
    BSSID[0] &= ~0x01;              // Turn off multicast bit
    BSSID[0] |= 0x02;               // Turn on local bit
    
    time >>= 8;
    BSSID[1] ^= (UCHAR)(time & 0xff); 
    
    time >>= 8;
    BSSID[2] ^= (UCHAR)(time & 0xff); 
    
    time >>= 8;
    BSSID[3] ^= (UCHAR)(time & 0xff);
}


NDIS_STATUS
Dot11ParseRNSIE(
    _In_reads_bytes_(RSNIELength) PUCHAR RSNIEData,
    _In_ ULONG OUIPrefix,
    _In_ UCHAR RSNIELength,
    _Out_ PRSN_IE_INFO RSNIEInfo
    )
{
    NdisZeroMemory(RSNIEInfo, sizeof(RSN_IE_INFO));
    RSNIEInfo->OUIPrefix = OUIPrefix;

    //
    // Get version (only mendatory field)
    //
    if (RSNIELength < 2)
    {
        return NDIS_STATUS_INVALID_DATA;
    }

    RSNIEInfo->Version = *((USHORT UNALIGNED *)RSNIEData);
    RSNIEData += 2;
    RSNIELength -= 2;

    // 
    // Get group cipher 
    //
    if (RSNIELength < 4)
    {
        return (RSNIELength == 0) ? NDIS_STATUS_SUCCESS : NDIS_STATUS_INVALID_DATA;
    }

    RSNIEInfo->GroupCipherCount = 1;
    RSNIEInfo->GroupCipher = RSNIEData;
    RSNIEData += 4;
    RSNIELength -= 4;

    // 
    // Get pairwise cipher count
    //
    if (RSNIELength < 2)
    {
        return (RSNIELength == 0) ? NDIS_STATUS_SUCCESS : NDIS_STATUS_INVALID_DATA;
    }

    RSNIEInfo->PairwiseCipherCount = *((USHORT UNALIGNED *)RSNIEData);
    RSNIEData += 2;
    RSNIELength -= 2;

    //
    // Get pairwise cipher
    //
    if (RSNIELength < RSNIEInfo->PairwiseCipherCount * 4)
    {
        return NDIS_STATUS_INVALID_DATA;
    }

    RSNIEInfo->PairwiseCipher = RSNIEData;
    RSNIEData += RSNIEInfo->PairwiseCipherCount * 4;
    RSNIELength -= (UCHAR)RSNIEInfo->PairwiseCipherCount * 4;

    //
    // Get AKM suite count
    //
    if (RSNIELength < 2)
    {
        return (RSNIELength == 0) ? NDIS_STATUS_SUCCESS : NDIS_STATUS_INVALID_DATA;
    }

    RSNIEInfo->AKMSuiteCount = *((USHORT UNALIGNED *)RSNIEData);
    RSNIEData += 2;
    RSNIELength -= 2;

    //
    // Get AKM suite
    //
    if (RSNIELength < RSNIEInfo->AKMSuiteCount * 4)
    {
        return NDIS_STATUS_INVALID_DATA;
    }

    RSNIEInfo->AKMSuite = RSNIEData;
    RSNIEData += RSNIEInfo->AKMSuiteCount * 4;
    RSNIELength -= (UCHAR)RSNIEInfo->AKMSuiteCount * 4;

    //
    // Get RSN capability
    //

    if (RSNIELength < 2)
    {
        return (RSNIELength == 0) ? NDIS_STATUS_SUCCESS : NDIS_STATUS_INVALID_DATA;
    }

    RSNIEInfo->Capability = *((USHORT UNALIGNED *)RSNIEData);
    RSNIEData += 2;
    RSNIELength -= 2;

    //
    // Get PMKID count
    //

    if (RSNIELength < 2)
    {
        return (RSNIELength == 0) ? NDIS_STATUS_SUCCESS : NDIS_STATUS_INVALID_DATA;
    }

    RSNIEInfo->PMKIDCount = *((USHORT UNALIGNED *)RSNIEData);
    RSNIEData += 2;
    RSNIELength -= 2;

    //
    // Get PMKID
    //
    if (RSNIELength != RSNIEInfo->PMKIDCount * 16)
    {
        return NDIS_STATUS_INVALID_DATA;
    }

    RSNIEInfo->PMKID = RSNIEData;
    return NDIS_STATUS_SUCCESS;
}

DOT11_CIPHER_ALGORITHM
Dot11GetCipherFromRSNOUI(
    _In_ ULONG prefix,
    _In_ ULONG oui
    )
{
    switch (oui - prefix)
    {
        case RSNA_CIPHER_WEP40:
            return DOT11_CIPHER_ALGO_WEP40;

        case RSNA_CIPHER_TKIP:
            return DOT11_CIPHER_ALGO_TKIP;

        case RSNA_CIPHER_CCMP:
            return DOT11_CIPHER_ALGO_CCMP;

        case RSNA_CIPHER_WEP104:
            return DOT11_CIPHER_ALGO_WEP104;

        default:
            return DOT11_CIPHER_ALGO_NONE;
    }
}

ULONG
Dot11GetRSNOUIFromCipher(
    _In_ ULONG prefix,
    _In_ DOT11_CIPHER_ALGORITHM cipher
    )
{
    switch (cipher)
    {
        case DOT11_CIPHER_ALGO_WEP40:
            return prefix + RSNA_CIPHER_WEP40;

        case DOT11_CIPHER_ALGO_TKIP:
            return prefix + RSNA_CIPHER_TKIP;

        case DOT11_CIPHER_ALGO_CCMP:
            return prefix + RSNA_CIPHER_CCMP;

        case DOT11_CIPHER_ALGO_WEP104:
            return prefix + RSNA_CIPHER_WEP104;

        default:
            ASSERT(0);
            return prefix + RSNA_CIPHER_CCMP;
    }
}

DOT11_AUTH_ALGORITHM
Dot11GetAuthAlgoFromRSNOUI(
    _In_ ULONG oui
    )
{
    switch (oui)
    {
        case RSNA_OUI_PREFIX + RSNA_AKM_RSNA:
            return DOT11_AUTH_ALGO_RSNA;

        case RSNA_OUI_PREFIX + RSNA_AKM_RSNA_PSK:
            return DOT11_AUTH_ALGO_RSNA_PSK;

        case WPA_OUI_PREFIX + RSNA_AKM_RSNA:
            return DOT11_AUTH_ALGO_WPA;

        case WPA_OUI_PREFIX + RSNA_AKM_RSNA_PSK:
            return DOT11_AUTH_ALGO_WPA_PSK;

        default:
            return DOT11_AUTH_ALGO_80211_OPEN;
    }
}

ULONG
Dot11GetRSNOUIFromAuthAlgo(
    _In_ DOT11_AUTH_ALGORITHM algo
    )
{
    switch (algo)
    {
        case DOT11_AUTH_ALGO_RSNA:
            return RSNA_OUI_PREFIX + RSNA_AKM_RSNA;

        case DOT11_AUTH_ALGO_RSNA_PSK:
            return RSNA_OUI_PREFIX + RSNA_AKM_RSNA_PSK;

        case DOT11_AUTH_ALGO_WPA:
            return WPA_OUI_PREFIX + RSNA_AKM_RSNA;

        case DOT11_AUTH_ALGO_WPA_PSK:
            return WPA_OUI_PREFIX + RSNA_AKM_RSNA_PSK;

        default:
            ASSERT(0);
            return RSNA_OUI_PREFIX + RSNA_AKM_RSNA;
    }
}

DOT11_CIPHER_ALGORITHM
Dot11GetGroupCipherFromRSNIEInfo(
    _In_ PRSN_IE_INFO RSNIEInfo
    )
{
    if (RSNIEInfo->GroupCipherCount == 1)
    {
        return Dot11GetCipherFromRSNOUI(RSNIEInfo->OUIPrefix, *((ULONG UNALIGNED *)RSNIEInfo->GroupCipher));
    }
    else
    {
        return DOT11_CIPHER_ALGO_NONE;
    }
}

DOT11_CIPHER_ALGORITHM
Dot11GetPairwiseCipherFromRSNIEInfo(
    _In_ PRSN_IE_INFO RSNIEInfo,
    _In_ USHORT index
    )
{
    if (RSNIEInfo->PairwiseCipherCount > index)
    {
        return Dot11GetCipherFromRSNOUI(RSNIEInfo->OUIPrefix, *((ULONG UNALIGNED *)Add2Ptr(RSNIEInfo->PairwiseCipher, index * 4)));
    }
    else
    {
        return DOT11_CIPHER_ALGO_NONE;
    }
}

DOT11_AUTH_ALGORITHM
Dot11GetAKMSuiteFromRSNIEInfo(
    _In_ PRSN_IE_INFO RSNIEInfo,
    _In_ USHORT index
    )
{
    if (RSNIEInfo->AKMSuiteCount > index)
    {
        return Dot11GetAuthAlgoFromRSNOUI(*((ULONG UNALIGNED *)Add2Ptr(RSNIEInfo->AKMSuite, index * 4)));
    }
    else
    {
        return DOT11_CIPHER_ALGO_NONE;
    }
}

NDIS_STATUS
MpCreateThread(
    _In_ PKSTART_ROUTINE StartFunc,
    _In_ PVOID Context,
    _In_ KPRIORITY Priority,
    _Outptr_ PKTHREAD* Thread
    )
{
    HANDLE              ThreadHandle;
    NDIS_STATUS         Status;
    OBJECT_ATTRIBUTES   ObjectAttribs;

    do
    {
        InitializeObjectAttributes(&ObjectAttribs, 
                                   NULL,              // ObjectName
                                   OBJ_KERNEL_HANDLE, 
                                   NULL,              // RootDir 
                                   NULL);             // Security Desc

        Status = PsCreateSystemThread(&ThreadHandle,
                                      THREAD_ALL_ACCESS,
                                      &ObjectAttribs,
                                      NULL,           // ProcessHandle
                                      NULL,           // ClientId
                                      StartFunc,
                                      Context);
        if (!NT_SUCCESS(Status))
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Worker Thread creation failed with 0x%lx\n", Status));
            break;
        }

        Status = ObReferenceObjectByHandle(ThreadHandle,
                                           THREAD_ALL_ACCESS,
                                           *PsThreadType,
                                           KernelMode,
                                           Thread,
                                           NULL);
        MPASSERT(NT_SUCCESS(Status));
       
        
        if (Priority != 0)
        {
            KeSetPriorityThread(*Thread, Priority);
        }

        ZwClose(ThreadHandle);

    }while(FALSE);

    
    return Status;
}

