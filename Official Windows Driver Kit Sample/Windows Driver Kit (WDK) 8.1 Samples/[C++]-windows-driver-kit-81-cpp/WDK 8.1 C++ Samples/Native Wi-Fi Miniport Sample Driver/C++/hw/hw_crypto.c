/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    hw_crypto.c

Abstract:
    Implements the cipher/encryption functionality for the HW layer
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#include "precomp.h"
#include "hw_crypto.h"

#if DOT11_TRACE_ENABLED
#include "hw_crypto.tmh"
#endif


#define ROL32(A,n)      ((A << n) | ((A >> (32 - n)) & ((1UL << n) - 1)))
#define ROR32(A,n)      ROL32(A, (32 - n))

__inline 
VOID
HwMICBlock(
    PULONG  L,
    PULONG  R
    )
{
    *R ^= ROL32(*L, 17);
    *L += *R;
    *R ^= ((*L & 0xff00ff00) >> 8) | ((*L & 0x00ff00ff) << 8);
    *L += *R;
    *R ^= ROL32(*L, 3);
    *L += *R;
    *R ^= ROR32(*L, 2);
    *L += *R;
}

NDIS_STATUS
HwCalculateTxMIC(
    _In_  PNET_BUFFER     NetBuffer,
    _In_  UCHAR           Priority,
    _In_reads_bytes_(8)  PUCHAR          MICKey,
    _Out_writes_bytes_(HW_MIC_LENGTH) PUCHAR          MICData
    )
{
    ULONG                   L, R;
    ULONG                   length;
    PMDL                    mdl;
    ULONG                   offset;
    PUCHAR                  bytePtr;
    ULONG                   index;
    ULONG                   count;
    ULONG                   dataSize;
    ULONG                   data;
    PUCHAR                  SA;
    PUCHAR                  DA;
    ULONG                   headerSize;
    PDOT11_DATA_LONG_HEADER dataHeader;

    // 
    // Note: only the first MDL is guaranteed to be mapped to system virtual space.
    //

    //
    // Find data header.
    //
    mdl = NET_BUFFER_CURRENT_MDL(NetBuffer);
    dataHeader = Add2Ptr(mdl->MappedSystemVa, NET_BUFFER_CURRENT_MDL_OFFSET(NetBuffer));

    //
    // Find SA, DA and header size.
    //
    if (dataHeader->FrameControl.ToDS)
    {
        DA = dataHeader->Address3;
        if (dataHeader->FrameControl.FromDS) 
        {
            headerSize = sizeof(DOT11_DATA_LONG_HEADER);
            SA = dataHeader->Address4;
        } 
        else 
        {
            headerSize = sizeof(DOT11_DATA_SHORT_HEADER);
            SA = dataHeader->Address2;
        }
    } 
    else 
    {
        headerSize = sizeof(DOT11_DATA_SHORT_HEADER);
        DA = dataHeader->Address1;
        if (dataHeader->FrameControl.FromDS) 
        {
            SA = dataHeader->Address3;
        } 
        else 
        {
            SA = dataHeader->Address2;
        }
    }

    //
    // Find the size of data the MIC is calculated on.
    //
    dataSize = NET_BUFFER_DATA_LENGTH(NetBuffer) - headerSize;
    length = 16 + dataSize + 5;                     // MSDU + 5 bytes pad (one 0x5a, four 0s)
    if (length & 0x03)
    {
        length += 4 - (length & 0x03);              // extra 0-3 bytes of 0s pad
    }
    length >>= 2;

    //
    // Find the starting offset for actual data (payload).
    // Caller guarantees that the header is in one single MDL.
    //
    offset = NET_BUFFER_CURRENT_MDL_OFFSET(NetBuffer) + headerSize;
    ASSERT(offset <= mdl->ByteCount);
    bytePtr = Add2Ptr(mdl->MappedSystemVa, offset);

    //
    // Initial L and R value from MIC key.
    //
    L = *((ULONG UNALIGNED *)MICKey);
    R = *((ULONG UNALIGNED *)(MICKey + 4));

    //
    // Loop to calculate the MIC
    //
    for (index = 0; index < length - 1; index++)
    {
        switch (index)
        {
            case 0:
                data = *((ULONG UNALIGNED *)DA);
                break;

            case 1:
                data = *((USHORT UNALIGNED *)SA);
                data <<= 16;
                data |= *((USHORT UNALIGNED *)(DA + 4));
                break;

            case 2:
                data = *((ULONG UNALIGNED *)(SA + 2));
                break;

            case 3:
                data = Priority;
                break;

            default:
                //
                // Data
                //
                if (offset + 4 <= mdl->ByteCount && (index - 4) * 4 + 4 <= dataSize)
                {
                    //
                    // We can get the whole ULONG in contiguous virtual memory.
                    //
                    data = *((ULONG UNALIGNED *)bytePtr);
                    bytePtr += 4;
                    offset += 4;
                }
                else 
                {
                    //
                    // We can't get the whole ULONG in contiguous virtual memory.
                    // Get one byte at a time. 
                    //
                    count = 0;
                    data = 0;
                    while (count < 4)
                    {
                        ASSERT((index - 4) * 4 + count <= dataSize);
                        if ((index - 4) * 4 + count == dataSize)
                        {
                            //
                            // We are done reading. Append 0x5a at the end.
                            //
                            data |= 0x5a << (count * 8);
                            break;
                        }
                        else if (offset < mdl->ByteCount)
                        {
                            //
                            // We can get the next byte without going to next MDL
                            //
                            data |= ((ULONG)(*bytePtr)) << (count * 8);
                            bytePtr++;
                            offset++;
                            count++;
                        }
                        else 
                        {
                            //
                            // We are at the end of a MDL. Follow the link to
                            // traverse to the next MDL.
                            //
                            mdl = mdl->Next;
                            ASSERT(mdl);

                            //
                            // Map the new MDL to system virtual space.
                            //
                            bytePtr = NdisBufferVirtualAddressSafe(mdl, NormalPagePriority);
                            if (!bytePtr)
                            {
                                return NDIS_STATUS_RESOURCES;
                            }

                            offset = 0;
                        }
                    }
                }
        }
    
        L ^= data;
        HwMICBlock(&L, &R); 
    }

    HwMICBlock(&L, &R); 
    *((ULONG UNALIGNED *)MICData) = L;
    *((ULONG UNALIGNED *)(MICData + 4)) = R;

    return NDIS_STATUS_SUCCESS;
}



NDIS_STATUS
HwCalculateRxMIC(
    _In_  PHW_RX_MSDU             Msdu,
    _In_  UCHAR                   Priority,
    _In_reads_bytes_(8)  PUCHAR                  MICKey,
    _Out_writes_bytes_(HW_MIC_LENGTH) PUCHAR                  MICData
    )
{
    ULONG                   L, R;
    ULONG                   length;
    ULONG                   offset;
    PUCHAR                  bytePtr;
    ULONG                   index;
    ULONG                   count;
    ULONG                   dataSize;
    ULONG                   data;
    PUCHAR                  SA;
    PUCHAR                  DA;
    ULONG                   headerSize;
    PDOT11_DATA_LONG_HEADER dataHeader;
    PHW_RX_MPDU             mpdu;
    USHORT                  mpduIndex = 0;

    //
    // Find data header.
    //
    mpdu = Msdu->MpduList[0];
    dataHeader = (PDOT11_DATA_LONG_HEADER)mpdu->DataStart;

    //
    // Find SA, DA and header size.
    //
    if (dataHeader->FrameControl.ToDS)
    {
        DA = dataHeader->Address3;
        if (dataHeader->FrameControl.FromDS) 
        {
            headerSize = sizeof(DOT11_DATA_LONG_HEADER);
            SA = dataHeader->Address4;
        } 
        else 
        {
            headerSize = sizeof(DOT11_DATA_SHORT_HEADER);
            SA = dataHeader->Address2;
        }
    } 
    else 
    {
        headerSize = sizeof(DOT11_DATA_SHORT_HEADER);
        DA = dataHeader->Address1;
        if (dataHeader->FrameControl.FromDS) 
        {
            SA = dataHeader->Address3;
        } 
        else 
        {
            SA = dataHeader->Address2;
        }
    }

    //
    // Find the size of data the MIC is calculated on.
    //
    dataSize = Msdu->DataLength - headerSize;
    length = 16 + dataSize + 5;                     // MSDU + 5 bytes pad (one 0x5a, four 0s)
    if (length & 0x03)
    {
        length += 4 - (length & 0x03);              // extra 0-3 bytes of 0s pad
    }
    length >>= 2;

    //
    // Find the starting offset for actual data (payload).
    // Caller guarantees that the header is in one single MDL.
    //
    offset = headerSize;
    MPASSERT(offset <= mpdu->DataLength);
    bytePtr = mpdu->DataStart + offset;

    //
    // Initial L and R value from MIC key.
    //
    L = *((ULONG UNALIGNED *)MICKey);
    R = *((ULONG UNALIGNED *)(MICKey + 4));

    //
    // Loop to calculate the MIC
    //
    for (index = 0; index < length - 1; index++)
    {
        switch (index)
        {
            case 0:
                data = *((ULONG UNALIGNED *)DA);
                break;

            case 1:
                data = *((USHORT UNALIGNED *)SA);
                data <<= 16;
                data |= *((USHORT UNALIGNED *)(DA + 4));
                break;

            case 2:
                data = *((ULONG UNALIGNED *)(SA + 2));
                break;

            case 3:
                data = Priority;
                break;

            default:
                //
                // Data
                //
                if (offset + 4 <= mpdu->DataLength && (index - 4) * 4 + 4 <= dataSize)
                {
                    //
                    // We can get the whole ULONG in contiguous virtual memory.
                    //
                    data = *((ULONG UNALIGNED *)bytePtr);
                    bytePtr += 4;
                    offset += 4;
                }
                else 
                {
                    //
                    // We can't get the whole ULONG in contiguous virtual memory.
                    // Get one byte at a time. 
                    //
                    count = 0;
                    data = 0;
                    while (count < 4)
                    {
                        ASSERT((index - 4) * 4 + count <= dataSize);
                        if ((index - 4) * 4 + count == dataSize)
                        {
                            //
                            // We are done reading. Append 0x5a at the end.
                            //
                            data |= 0x5a << (count * 8);
                            break;
                        }
                        else if (offset < mpdu->DataLength)
                        {
                            //
                            // We can get the next byte without going to next MPDU
                            //
                            data |= ((ULONG)(*bytePtr)) << (count * 8);
                            bytePtr++;
                            offset++;
                            count++;
                        }
                        else
                        {
                            //
                            // We are at the end of a MPDU. Get the next
                            // MPDU
                            //
                            mpduIndex++;
                            if (mpduIndex < Msdu->MpduCount)
                            {
                                mpdu = Msdu->MpduList[mpduIndex];

                                bytePtr = mpdu->DataStart;
                                offset = 0;
                            }
                            else
                            {
                                // Failed
                                MPASSERT(FALSE);
                                break;
                            }
                        }
                    }
                }
        }
    
        L ^= data;
        HwMICBlock(&L, &R); 
    }

    HwMICBlock(&L, &R); 
    *((ULONG UNALIGNED *)MICData) = L;
    *((ULONG UNALIGNED *)(MICData + 4)) = R;

    return NDIS_STATUS_SUCCESS;
}
