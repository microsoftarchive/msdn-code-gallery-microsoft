/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    base_port_recv.c

Abstract:
    Implements the receive functionality for the base port class
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/

#include "precomp.h"
#include "port_defs.h"
#include "base_port.h"
#include "base_port_intf.h"
#include "vnic_intf.h"

#if DOT11_TRACE_ENABLED
#include "base_port_recv.tmh"
#endif

NDIS_STATUS 
BasePortReceiveEventHandler(
    _In_  PMP_PORT                Port,
    _In_  PMP_RX_MSDU             PacketList,
    _In_  ULONG                   ReceiveFlags
    )
{
    UNREFERENCED_PARAMETER(Port);
    UNREFERENCED_PARAMETER(PacketList);
    UNREFERENCED_PARAMETER(ReceiveFlags);

    return NDIS_STATUS_SUCCESS;
}

VOID 
BasePortReturnEventHandler(
    _In_  PMP_PORT                Port,
    _In_  PMP_RX_MSDU             PacketList,
    _In_  ULONG                   ReturnFlags
    )
{
    UNREFERENCED_PARAMETER(Port);
    UNREFERENCED_PARAMETER(PacketList);
    UNREFERENCED_PARAMETER(ReturnFlags);
}


NDIS_STATUS
BasePortTranslateRxPacketsToRxNBLs(
    _In_  PMP_PORT                Port,
    _In_  PMP_RX_MSDU             PacketList,
    _Out_ PNET_BUFFER_LIST*       NetBufferLists
    )
{
    PNET_BUFFER_LIST            outputNetBufferList = NULL;
    PNET_BUFFER_LIST            currentNetBufferList, prevNetBufferList = NULL;
    USHORT                      fragmentIndex = 0;
    PMP_RX_MSDU                 currentPacket = PacketList, nextPacket;
    PMP_RX_MPDU                 currentFragment;
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_EXTSTA_RECV_CONTEXT  osRecvContext;  // This is same for ExtAP & ExtSTA
    PMDL                        currentMdl, prevMdl = NULL, mdlChain = NULL;
    ULONG                       totalLength = 0;
    
    *NetBufferLists = NULL;

    // Convert each PACKET and FRAGMENT to OS structures
    while (currentPacket != NULL)
    {
        nextPacket = MP_RX_MSDU_NEXT_MSDU(currentPacket);
        
        // Go through the FRAGMENTs in this PACKET & create an MDL chain
        mdlChain = NULL;
        totalLength = 0;
        
        for (fragmentIndex = 0; 
             fragmentIndex < MP_RX_MSDU_MPDU_COUNT(currentPacket);
             fragmentIndex++)
        {
            currentFragment = MP_RX_MSDU_MPDU_AT(currentPacket, fragmentIndex);
            totalLength += MP_RX_MPDU_LENGTH(currentFragment);
            
            // Populate the MDL with Fragment contents
            currentMdl = NdisAllocateMdl(Port->MiniportAdapterHandle,
                                MP_RX_MPDU_DATA(currentFragment),
                                MP_RX_MPDU_LENGTH(currentFragment)
                                );
            if (currentMdl == NULL)
            {
                MpTrace(COMP_RECV, DBG_SERIOUS, ("Failed to allocate MDL for a MP_RX_MPDU data\n"));
                ndisStatus = NDIS_STATUS_RESOURCES;
                break;
            }

            // Add this to the MDL chain
            if (mdlChain == NULL)
            {
                // Add this to the head
                mdlChain = currentMdl;
            }
            else
            {
                NDIS_MDL_LINKAGE(prevMdl) = currentMdl;
            }
            prevMdl = currentMdl;
        }
        
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            while (mdlChain != NULL)
            {
                currentMdl = mdlChain;
                mdlChain = NDIS_MDL_LINKAGE(currentMdl);
                
                // MDL was allocated
                NdisFreeMdl(currentMdl);
            }            
            break;
        }

        // Allocate the NET_BUFFER_LIST and the NET_BUFFER
        currentNetBufferList = NdisAllocateNetBufferAndNetBufferList(
                                    Port->RxNetBufferListPool,
                                    0,
                                    0,
                                    mdlChain,
                                    0,
                                    totalLength
                                    );
        if (currentNetBufferList == NULL)
        {
            MpTrace(COMP_SEND, DBG_SERIOUS, ("Failed to allocate NET_BUFFER_LIST for MP_RX_MSDU  \n"));        
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }

        // Identify the origin of this packet
        currentNetBufferList->SourceHandle = Port->MiniportAdapterHandle;
        
        // Populate the RX_PACKET
        MP_NBL_WRAPPED_RX_MSDU(currentNetBufferList) = currentPacket;
        MP_NBL_SOURCE_PORT(currentNetBufferList) = Port;
        
        if (outputNetBufferList == NULL)
        {
            outputNetBufferList = currentNetBufferList;
        }
        else
        {
            NET_BUFFER_LIST_NEXT_NBL(prevNetBufferList) = currentNetBufferList;
        }
        
        // The Next PACKET's NBL would be added after the current PACKET's NBL
        prevNetBufferList = currentNetBufferList;

        osRecvContext = MP_RX_MSDU_RECV_CONTEXT(currentPacket);

        MP_ASSIGN_NDIS_OBJECT_HEADER(osRecvContext->Header, 
             NDIS_OBJECT_TYPE_DEFAULT,
             DOT11_EXTSTA_RECV_CONTEXT_REVISION_1,
             sizeof(DOT11_EXTSTA_RECV_CONTEXT));
        
        MP_SET_RECEIVE_CONTEXT(currentNetBufferList, osRecvContext);        
        
        currentPacket = nextPacket;
    }


    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        if (outputNetBufferList != NULL)
        {
            BasePortFreeTranslatedRxNBLs(Port, outputNetBufferList);
            outputNetBufferList = NULL;
        }
    }

    *NetBufferLists = outputNetBufferList;

    return ndisStatus;
}

// Used on PortReturn to free the RX_PACKETs 
// allocated via BasePortTranslateRxPacketsToRxNBLs 
VOID
BasePortFreeTranslatedRxNBLs(
    _In_  PMP_PORT                Port,
    _Inout_ PNET_BUFFER_LIST        NetBufferLists
    )
{
    PNET_BUFFER_LIST            currentNetBufferList, nextNetBufferList;
    PMDL                        currentMdl, nextMdl;

    UNREFERENCED_PARAMETER(Port);

    currentNetBufferList = NetBufferLists;
    while (currentNetBufferList != NULL)
    {
        nextNetBufferList = NET_BUFFER_LIST_NEXT_NBL(currentNetBufferList);
        NET_BUFFER_LIST_NEXT_NBL(currentNetBufferList) = NULL;

        currentMdl = NET_BUFFER_CURRENT_MDL(NET_BUFFER_LIST_FIRST_NB(currentNetBufferList));
        while (currentMdl != NULL)
        {
            nextMdl = NDIS_MDL_LINKAGE(currentMdl);

            // Free the MDL
            NdisFreeMdl(currentMdl);

            currentMdl = nextMdl;
        }

        // Free the NET_BUFFER and NET_BUFFER_LIST
        NdisFreeNetBufferList(currentNetBufferList);
        
        currentNetBufferList = nextNetBufferList;
    }
}


BOOLEAN
BasePortFilterFragment(
    _In_  PMP_PORT                Port,
    _In_  PMP_RX_MSDU             Packet
    )
{
    PUCHAR      pktbuf = MP_RX_MPDU_DATA(MP_RX_MSDU_MPDU_AT(Packet, 0));
    UCHAR       type = (pktbuf[0] & 0x0f) >> 2;
    BOOLEAN     bAddrMatch = MP_COMPARE_MAC_ADDRESS(pktbuf + 4, VNic11QueryMACAddress(PORT_GET_VNIC(Port)));
    BOOLEAN     bIsBroadcast = DOT11_IS_BROADCAST(&pktbuf[4]);
    BOOLEAN     bIsMulticast = DOT11_IS_MULTICAST(&pktbuf[4]);

    switch(type)
    {
    case DOT11_FRAME_TYPE_MANAGEMENT:
        if (bAddrMatch && (Port->PacketFilter & NDIS_PACKET_TYPE_802_11_DIRECTED_MGMT))
            return TRUE;
        
        if (bIsBroadcast && (Port->PacketFilter & NDIS_PACKET_TYPE_802_11_BROADCAST_MGMT))
            return TRUE;

        if (bIsMulticast && (Port->PacketFilter & (NDIS_PACKET_TYPE_802_11_MULTICAST_MGMT | 
                                                           NDIS_PACKET_TYPE_802_11_ALL_MULTICAST_MGMT)))
            return TRUE;

        if (Port->PacketFilter & NDIS_PACKET_TYPE_802_11_PROMISCUOUS_MGMT)
            return TRUE;

        break;
        
    case DOT11_FRAME_TYPE_CONTROL:
        if (bAddrMatch && (Port->PacketFilter & NDIS_PACKET_TYPE_802_11_DIRECTED_CTRL))
            return TRUE;

        if (bIsBroadcast && (Port->PacketFilter & NDIS_PACKET_TYPE_802_11_BROADCAST_CTRL))
            return TRUE;

        if (Port->PacketFilter & NDIS_PACKET_TYPE_802_11_PROMISCUOUS_CTRL)
            return TRUE;

        //
        // Note: no multicast control frame.
        //

        break;
        
    case DOT11_FRAME_TYPE_DATA:
        if (bAddrMatch && (Port->PacketFilter & NDIS_PACKET_TYPE_DIRECTED))
            return TRUE;

        if (bIsBroadcast && (Port->PacketFilter & NDIS_PACKET_TYPE_BROADCAST))
            return TRUE;

        if (bIsMulticast&& (Port->PacketFilter & (NDIS_PACKET_TYPE_MULTICAST | 
                                                          NDIS_PACKET_TYPE_ALL_MULTICAST)))
            return TRUE;

        if (Port->PacketFilter & (NDIS_PACKET_TYPE_PROMISCUOUS | 
                                          NDIS_PACKET_TYPE_802_11_RAW_DATA))
            return TRUE;

        break;
        
    default:
        //
        // Reserved packet should always be filtered
        //
        return FALSE;
    }

    return FALSE;
}


VOID
BasePortIndicateReceivePackets(
    _In_  PMP_PORT                Port,
    _In_  PMP_RX_MSDU             PacketList,
    _In_  ULONG                   ReceiveFlags
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PMP_RX_MSDU                 currentPacket = PacketList, nextPacket;
    PMP_RX_MSDU                 packetListToIndicate = NULL, prevPacketToIndicate = NULL;
    PMP_RX_MSDU                 packetListToReturn = NULL, prevPacketToReturn = NULL;
    PNET_BUFFER_LIST            nblChainToIndicate;
    ULONG                       indicateCount = 0;
    ULONG                       returnCount = 0;

    //
    // Currently the lower layer only indicates single MSDUs. We cannot handle
    // receiving more than one MSDU since we break the chain that the HW provides
    // and that is not acceptable in the RESOURCES case
    // 
    MPASSERT(MP_RX_MSDU_NEXT_MSDU(currentPacket) == NULL);
    
    
    // Process each of the packets internally
    while (currentPacket != NULL)
    {
        nextPacket = MP_RX_MSDU_NEXT_MSDU(currentPacket);

        MP_RX_MSDU_NEXT_MSDU(currentPacket) = NULL;
       
        do
        {
            //
            // Pass the packet to the port to determine if this 
            // packet should be indicated up to the OS or not
            //
            ndisStatus = Port11NotifyReceive(Port, currentPacket, ReceiveFlags);            
            if (ndisStatus == NDIS_STATUS_SUCCESS)
            {
                // We only pass 1 packet at a time to the port,
                // lets look at whether we should indicate this packet up to
                // the OS or not
                ndisStatus = MP_RX_MSDU_STATUS(currentPacket);
            }

            if (ndisStatus != NDIS_STATUS_SUCCESS)
            {
                // This packet should not be indicated up
                break;
            }

            // Perform filtering of the packet to determine if this need to go up to the OS
            if (BasePortFilterFragment(Port, currentPacket) == FALSE)
            {
                // Drop these packets
                ndisStatus = NDIS_STATUS_NOT_ACCEPTED;                

                // Since we have already given this packet to the port, let it undo 
                // anything it may have done before
                Port11NotifyReturn(Port, 
                    currentPacket, 
                    NDIS_TEST_RECEIVE_AT_DISPATCH_LEVEL(ReceiveFlags) ? NDIS_RETURN_FLAGS_DISPATCH_LEVEL 
                        : 0
                    );
                break;
            }

        } while (FALSE);

        if (ndisStatus == NDIS_STATUS_SUCCESS)
        {
            // This packet gets indicated to the OS
            if (packetListToIndicate == NULL)
            {
                packetListToIndicate = currentPacket;
            }
            else
            {
                MP_RX_MSDU_NEXT_MSDU(prevPacketToIndicate) = currentPacket;
            }
            indicateCount++;
            prevPacketToIndicate = currentPacket;

            //
            // From this point on the packets must be completed
            //
        }
        else
        {
            // This (failed) packet gets returned back to the HW
            if (packetListToReturn == NULL)
            {
                packetListToReturn = currentPacket;
            }
            else
            {
                MP_RX_MSDU_NEXT_MSDU(prevPacketToReturn) = currentPacket;
            }
            returnCount++;
            prevPacketToReturn = currentPacket;
        }

        // Next packet
        currentPacket = nextPacket;
    }

    // Convert the packets we want to indicate into
    if (packetListToIndicate != NULL)
    {
        ndisStatus = BasePortTranslateRxPacketsToRxNBLs(Port, 
                        packetListToIndicate, 
                        &nblChainToIndicate
                        );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_SEND, DBG_SERIOUS, ("Failed to allocate NET_BUFFER_LIST chain for MP_RX_MSDU   chain\n"));

            //
            // Notify port that these packets have been returned
            //
            Port11NotifyReturn(Port, 
                packetListToIndicate, 
                NDIS_TEST_RECEIVE_AT_DISPATCH_LEVEL(ReceiveFlags) ? NDIS_RETURN_FLAGS_DISPATCH_LEVEL 
                    : 0
                );

            //
            // Prepend these "failed" MSDUs to the list that we wont indicate to the OS. These
            // would get freed below. The order doesnt matter here
            //
            currentPacket = packetListToIndicate;
            while (currentPacket != NULL)
            {
                nextPacket = MP_RX_MSDU_NEXT_MSDU(currentPacket);
                MP_RX_MSDU_NEXT_MSDU(currentPacket) = packetListToReturn;
                packetListToReturn = currentPacket;

                currentPacket = nextPacket;                
                returnCount++;
            }
            
        }
        else
        {
            if (NDIS_TEST_RECEIVE_CAN_PEND(ReceiveFlags))
            {
                //
                // Increment the counter for the number of packets we have submitted 
                // to the OS. This would block the port from pausing, etc
                //
                PORT_ADD_PNP_REFCOUNT(Port, indicateCount);
            }
        
            // Indicate these to the OS
            NdisMIndicateReceiveNetBufferLists(
                Port->MiniportAdapterHandle,
                nblChainToIndicate,
                Port->PortNumber,
                indicateCount,
                ReceiveFlags
                );

            if (NDIS_TEST_RECEIVE_CANNOT_PEND(ReceiveFlags))
            {
                // We wont get a return for these. Free the NBLs
                BasePortFreeTranslatedRxNBLs(Port, nblChainToIndicate);

                // Notify the ports about the return
                Port11NotifyReturn(Port, 
                    packetListToIndicate, 
                    NDIS_TEST_RECEIVE_AT_DISPATCH_LEVEL(ReceiveFlags) ? NDIS_RETURN_FLAGS_DISPATCH_LEVEL 
                        : 0
                    );
            }
        }

    }
    
    // Return all the non-used packets back to the HW (if we are permitted to call
    // return)
    if ((packetListToReturn != NULL) && (NDIS_TEST_RECEIVE_CAN_PEND(ReceiveFlags)))
    {
        VNic11ReturnPackets(Port->VNic, 
            packetListToReturn, 
            NDIS_TEST_RECEIVE_AT_DISPATCH_LEVEL(ReceiveFlags) ? NDIS_RETURN_FLAGS_DISPATCH_LEVEL : 0
            );
    }

}


VOID
BasePortHandleReturnNetBufferLists(
    _In_  PMP_PORT                Port,
    _In_  PNET_BUFFER_LIST        NetBufferLists,
    _In_  ULONG                   ReturnFlags
    )
{
    PNET_BUFFER_LIST    currentNetBufferList;    
    PMP_RX_MSDU         packetListToReturn = NULL, currentPacket;

    // Create a RX_PACKET chain to return to the next
    currentNetBufferList = NetBufferLists;
    while (currentNetBufferList != NULL)
    {
        currentPacket = MP_NBL_WRAPPED_RX_MSDU(currentNetBufferList);

        // We return out of order, but it does not matter
        if (packetListToReturn == NULL)
        {
            packetListToReturn = currentPacket;
            
            //
            // On an indicate we may not have cleared the next pointers
            // To ensure that we are only going to return one at a time, clear it now
            //
            MP_RX_MSDU_NEXT_MSDU(currentPacket) = NULL;
        }
        else
        {
            MP_RX_MSDU_NEXT_MSDU(currentPacket) = packetListToReturn;
            packetListToReturn = currentPacket;
        }

        //
        // Decrement the counter for the number of packets we have submitted 
        // to the OS. This would enable us to unblock the port
        //
        PORT_DECREMENT_PNP_REFCOUNT(Port);

        currentNetBufferList = NET_BUFFER_LIST_NEXT_NBL(currentNetBufferList);
    }

    // Free the NET_BUFFER_LIST chain
    BasePortFreeTranslatedRxNBLs(Port, NetBufferLists);

    //
    // Inform the port about the packets that have been returned
    //
    Port11NotifyReturn(Port, packetListToReturn, ReturnFlags);

    // Return the packets back to the driver
    VNic11ReturnPackets(Port->VNic, 
        packetListToReturn, 
        ReturnFlags
        );


}



