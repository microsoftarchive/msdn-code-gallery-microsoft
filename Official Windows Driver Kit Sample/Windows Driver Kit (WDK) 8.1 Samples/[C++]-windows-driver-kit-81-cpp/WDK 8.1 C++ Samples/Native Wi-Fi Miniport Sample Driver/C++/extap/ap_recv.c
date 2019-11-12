/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    ap_recv.c

Abstract:
    AP layer receive processing functions
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    10-08-2007    Created

Notes:

--*/
#include "precomp.h"

// TODO: move the following to a common header file shared with station
#include <packon.h>
typedef struct {
    UCHAR sh_dsap;
    UCHAR sh_ssap;
    UCHAR sh_ctl;
    UCHAR sh_protid[3];
    unsigned short  sh_etype;
} IEEE_8022_LLC_SNAP, *PIEEE_8022_LLC_SNAP;
#include <packoff.h>

#if DOT11_TRACE_ENABLED
#include "ap_recv.tmh"
#endif

/**
 * Check whether ExtAP needs to process a management packet.
 */
BOOLEAN
FORCEINLINE
ApNeedToProcessPacket(
    _In_ PDOT11_MGMT_HEADER MgmtPktHeader,
    _In_ USHORT FragmentSize
    )
{
    BOOLEAN processPkt = FALSE;

    UNREFERENCED_PARAMETER(FragmentSize);
    
    do
    {
        //
        // For now, only process unicast management packets
        //
        if (DOT11_IS_BROADCAST(&MgmtPktHeader->DA) ||
            MgmtPktHeader->FrameControl.Type != DOT11_FRAME_TYPE_MANAGEMENT)
        {
            break;
        }

        //
        // Interested management packets
        //
        switch(MgmtPktHeader->FrameControl.Subtype)
        {
            case DOT11_MGMT_SUBTYPE_AUTHENTICATION:
            case DOT11_MGMT_SUBTYPE_ASSOCIATION_REQUEST:
            case DOT11_MGMT_SUBTYPE_REASSOCIATION_REQUEST:
            case DOT11_MGMT_SUBTYPE_DEAUTHENTICATION:
            case DOT11_MGMT_SUBTYPE_DISASSOCIATION:
                processPkt = TRUE;
                break;
        }
    } while (FALSE);

    return processPkt;
}

/**
 * Processing management packet
 */
NDIS_STATUS
ApProcessPacket(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_MGMT_HEADER MgmtPktHeader,
    _In_ USHORT FragmentSize
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // 
        // Only process packet when AP is started
        // 
        if (ApGetState(ApPort) != AP_STATE_STARTED)
        {
            ndisStatus = NDIS_STATUS_INVALID_STATE;
            break;
        }

        // 
        // Only process management packet
        // 
        if (MgmtPktHeader->FrameControl.Type != DOT11_FRAME_TYPE_MANAGEMENT)
        {
            // Not a failure
            break;
        }
        
        //
        // Validate management packet length
        //
        if (FragmentSize < DOT11_MGMT_HEADER_SIZE)
        {
            MpTrace(COMP_RECV, DBG_LOUD, ("Port(%u): Management packet (size = %u) too short.\n", 
                                AP_GET_PORT_NUMBER(ApPort),
                                FragmentSize));
            ndisStatus = NDIS_STATUS_NOT_ACCEPTED;
            break;
        }

        switch(MgmtPktHeader->FrameControl.Subtype)
        {
            case DOT11_MGMT_SUBTYPE_AUTHENTICATION:
                ApReceiveAuthentication(
                    ApPort,
                    MgmtPktHeader,
                    FragmentSize
                    );
                break;

            case DOT11_MGMT_SUBTYPE_ASSOCIATION_REQUEST:
                ApReceiveAssociationRequest(
                    ApPort,
                    MgmtPktHeader,
                    FragmentSize
                    );
                break;

            case DOT11_MGMT_SUBTYPE_REASSOCIATION_REQUEST:
                ApReceiveReassociationRequest(
                    ApPort,
                    MgmtPktHeader,
                    FragmentSize
                    );
                break;

            case DOT11_MGMT_SUBTYPE_DEAUTHENTICATION:
                ApReceiveDeauthentication(
                    ApPort,
                    MgmtPktHeader,
                    FragmentSize
                    );
                break;

            case DOT11_MGMT_SUBTYPE_DISASSOCIATION:
                ApReceiveDisassociation(
                    ApPort,
                    MgmtPktHeader,
                    FragmentSize
                    );
                break;

            default:
                break;
        }
    } while (FALSE);
    
    return ndisStatus;    
}

/**
 * Validate data packet.
 * Will check encryption in the validation.
 */
NDIS_STATUS
ApValidateDataPacket(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PMP_RX_MPDU Fragment,
    _In_ USHORT FragmentSize
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_PRIVACY_EXEMPTION_LIST privacyExemptionList = AP_GET_ASSOC_MGR(ApPort)->PrivacyExemptionList;
    USHORT etherType;
    USHORT packetType;
    PDOT11_PRIVACY_EXEMPTION privacyExemption;
    ULONG index;
    BOOLEAN isUnicast;
    USHORT sequenceNumber;
    PDOT11_DATA_SHORT_HEADER fragmentHdr = (PDOT11_DATA_SHORT_HEADER)MP_RX_MPDU_DATA(Fragment);
    BOOLEAN checkExcludeUnencrypted = TRUE;

    do
    {
        //
        // We don't check the fragmented data frame that is not the first fragment.
        //
        if (FragmentSize < DOT11_DATA_SHORT_HEADER_SIZE)
        {
            ndisStatus = NDIS_STATUS_NOT_ACCEPTED;
            break;
        }
        
        RtlCopyMemory(
            &sequenceNumber, 
            &fragmentHdr->SequenceControl, 
            2
            );
        
        if ((sequenceNumber & 0x0f) != 0)
        {
            break;
        }
        
        //
        // Data frame must also contain 802.2 LLC and 802.2 SNAP (8 bytes total)
        //
        if (FragmentSize < DOT11_DATA_SHORT_HEADER_SIZE + sizeof(IEEE_8022_LLC_SNAP))
        {
            ndisStatus = NDIS_STATUS_NOT_ACCEPTED;
            break;
        }

        //
        // Go through the privacy exemption list to see if we can accept the data frame.
        //
        if (privacyExemptionList && privacyExemptionList->uNumOfEntries > 0)
        {
            //
            // Find the EtherType and PacketType of the frame
            //

            etherType = ((PIEEE_8022_LLC_SNAP)Add2Ptr(fragmentHdr, DOT11_DATA_SHORT_HEADER_SIZE))->sh_etype;
            isUnicast = (BOOLEAN)DOT11_IS_UNICAST(fragmentHdr->Address1);
            packetType = isUnicast ? DOT11_EXEMPT_UNICAST : DOT11_EXEMPT_MULTICAST;

            //
            // Check the disposition of the frame.
            //

            privacyExemption = privacyExemptionList->PrivacyExemptionEntries;
            for (index = 0; index < privacyExemptionList->uNumOfEntries; index++, privacyExemption++)
            {
                //
                // Skip if EtherType does not match
                //

                if (privacyExemption->usEtherType != etherType)
                {
                    continue;
                }

                //
                // Skip if PacketType does not match
                //

                if (privacyExemption->usExemptionPacketType != packetType &&
                    privacyExemption->usExemptionPacketType != DOT11_EXEMPT_BOTH)
                {
                    continue;
                }

                if (privacyExemption->usExemptionActionType == DOT11_EXEMPT_ALWAYS)
                {
                    //
                    // In this case, we drop the frame if it was originally
                    // encrypted.
                    //
                    if (MP_TEST_FLAG(Fragment->Msdu->Flags, MP_RX_MSDU_FLAG_ENCRYPTED))
                    {
                        ndisStatus = NDIS_STATUS_NOT_ACCEPTED;
                    }

                    //
                    // No need to check exclude unencrypted
                    //
                    checkExcludeUnencrypted = FALSE;
                    
                    break;
                }
                else if (privacyExemption->usExemptionActionType == DOT11_EXEMPT_ON_KEY_MAPPING_KEY_UNAVAILABLE)
                {
                    //
                    // In this case, we reject the frame if it was originally NOT encrypted but 
                    // we have the key mapping key for this frame.
                    //
                    if (!MP_TEST_FLAG(Fragment->Msdu->Flags, MP_RX_MSDU_FLAG_ENCRYPTED) && 
                        isUnicast && 
                        VNic11IsKeyMappingKeyAvailable(AP_GET_VNIC(ApPort), fragmentHdr->Address2)
                        )
                    {
                        ndisStatus = NDIS_STATUS_NOT_ACCEPTED;
                    }

                    //
                    // No need to check exclude unencrypted
                    //
                    checkExcludeUnencrypted = FALSE;
                    
                    break;
                }
                else 
                {
                    //
                    // The privacy exemption does not apply to this frame.
                    //

                    break;
                }
            }
        }

        //
        // If the privacy exemption list does not apply to the frame, check ExcludeUnencrypted.
        // if ExcludeUnencrypted is set and this frame was not oringially an encrypted frame, 
        // dropped it.
        //
        if (checkExcludeUnencrypted)
        {
            if (AP_GET_ASSOC_MGR(ApPort)->ExcludeUnencrypted && !MP_TEST_FLAG(Fragment->Msdu->Flags, MP_RX_MSDU_FLAG_ENCRYPTED))
            {
                ndisStatus = NDIS_STATUS_NOT_ACCEPTED;
            }
        }

    } while (FALSE);   
    
    return ndisStatus;
}


/**
 * Processing authentication
 */
VOID 
ApReceiveAuthentication(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_MGMT_HEADER MgmtPacket,
    _In_ USHORT PacketSize
    )
{
    do
    {
        if (PacketSize < (DOT11_MGMT_HEADER_SIZE + sizeof(DOT11_AUTH_FRAME)))
        {
            MpTrace(COMP_RECV, DBG_LOUD, ("Port(%u): Authentication packet (size = %u) too short.\n", 
                                AP_GET_PORT_NUMBER(ApPort),
                                PacketSize));
            break;
        }

        // 
        // Let association manager process authentication
        //
        AmProcessStaAuthentication(
            AP_GET_ASSOC_MGR(ApPort), 
            MgmtPacket, 
            PacketSize
            );
    } while (FALSE);
}

/**
 * Processing deauthentication
 */
VOID 
ApReceiveDeauthentication(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_MGMT_HEADER MgmtPacket,
    _In_ USHORT PacketSize
    )
{
    do
    {
        if (PacketSize < (DOT11_MGMT_HEADER_SIZE + sizeof(DOT11_DEAUTH_FRAME)))
        {
            MpTrace(COMP_RECV, DBG_LOUD, ("Port(%u): Deauthentication packet (size = %u) too short.\n", 
                                AP_GET_PORT_NUMBER(ApPort),
                                PacketSize));
            break;
        }

        // 
        // Let association manager process deauthentication
        //
        AmProcessStaDeauthentication(
            AP_GET_ASSOC_MGR(ApPort), 
            MgmtPacket, 
            PacketSize
            );
    } while (FALSE);
}

/**
 * Processing association request
 */
VOID 
ApReceiveAssociationRequest(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_MGMT_HEADER MgmtPacket,
    _In_ USHORT PacketSize
    )
{
    do
    {
        if (PacketSize < (DOT11_MGMT_HEADER_SIZE + sizeof(DOT11_ASSOC_REQUEST_FRAME)))
        {
            MpTrace(COMP_RECV, DBG_LOUD, ("Port(%u): Association request packet (size = %u) too short.\n", 
                                AP_GET_PORT_NUMBER(ApPort),
                                PacketSize));
            break;
        }

        // 
        // Let association manager process association request
        //
        AmProcessStaAssociation(
            AP_GET_ASSOC_MGR(ApPort), 
            FALSE,
            MgmtPacket, 
            PacketSize
            );
    } while (FALSE);
}

/**
 * Processing reassociation request
 */
VOID 
ApReceiveReassociationRequest(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_MGMT_HEADER MgmtPacket,
    _In_ USHORT PacketSize
    )
{
    do
    {
        if (PacketSize < (DOT11_MGMT_HEADER_SIZE + sizeof(DOT11_REASSOC_REQUEST_FRAME)))
        {
            MpTrace(COMP_RECV, DBG_LOUD, ("Port(%u): Reassociation request packet (size = %u) too short.\n", 
                                AP_GET_PORT_NUMBER(ApPort),
                                PacketSize));
            break;
        }

        // 
        // Let association manager process reassociation request
        //
        AmProcessStaAssociation(
            AP_GET_ASSOC_MGR(ApPort), 
            TRUE,
            MgmtPacket, 
            PacketSize
            );
    } while (FALSE);
}

/**
 * Processing disassociation
 */
VOID 
ApReceiveDisassociation(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_MGMT_HEADER MgmtPacket,
    _In_ USHORT PacketSize
    )
{
    do
    {
        if (PacketSize < (DOT11_MGMT_HEADER_SIZE + sizeof(DOT11_DISASSOC_FRAME)))
        {
            MpTrace(COMP_RECV, DBG_LOUD, ("Port(%u): Disassociation packet (size = %u) too short.\n", 
                                AP_GET_PORT_NUMBER(ApPort),
                                PacketSize));
            break;
        }

        // 
        // Let association manager process disassociation
        //
        AmProcessStaDisassociation(
            AP_GET_ASSOC_MGR(ApPort), 
            MgmtPacket, 
            PacketSize
            );
    } while (FALSE);
}

NDIS_STATUS
Ap11ReceiveHandler(
    _In_ PMP_PORT Port,
    _In_ PMP_RX_MSDU   PacketList,
    _In_ ULONG ReceiveFlags
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PAP_ASSOC_MGR assocMgr = AP_GET_ASSOC_MGR(MP_GET_AP_PORT(Port));
    PDOT11_MGMT_HEADER mgmtPktHeader;
    PMP_RX_MPDU     rxFragment;
    USHORT fragmentSize;
    PMP_RX_MSDU   currentPacket;
    PMAC_HASH_ENTRY macEntry;
    PAP_STA_ENTRY staEntry;
    DOT11_ASSOCIATION_STATE assocState;
    DOT11_FRAME_CLASS frameClass;
    MP_RW_LOCK_STATE lockState;
    //
    // The following boolean is used to improve the performance w.r.t. lock
    //
    BOOLEAN holdReadLock = FALSE;

    UNREFERENCED_PARAMETER(ReceiveFlags);
    
    // 
    // Reference AP port first
    //
    ApRefPort(MP_GET_AP_PORT(Port));
    
    do
    {
        // 
        // Only process packets when AP is started
        //
        if (ApGetState(MP_GET_AP_PORT(Port)) != AP_STATE_STARTED)
        {
            ndisStatus = NDIS_STATUS_INVALID_STATE;
            break;
        }
        
        // 
        // Process each of the packets internally
        //
        for (currentPacket = PacketList;
             currentPacket != NULL;
             currentPacket = MP_RX_MSDU_NEXT_MSDU(currentPacket)
             )
        {
            // 
            // We only accept 1 fragment
            //
            rxFragment = MP_RX_MSDU_MPDU_AT(currentPacket, 0);
            fragmentSize = (USHORT)MP_RX_MPDU_LENGTH(rxFragment);
            if (fragmentSize < DOT11_MGMT_HEADER_SIZE)
            {
                continue;
            }

            mgmtPktHeader = MP_RX_MPDU_DATA(rxFragment);

            //
            // TODO: ignore packets that are not in this BSSID?
            //
            if (!MP_COMPARE_MAC_ADDRESS(mgmtPktHeader->BSSID, assocMgr->Bssid))
            {
                continue;
            }
            
            //
            // Check whether ExtAP needs to process the packet
            //
            // If ExtAP will process the packet, a write lock will be acquired later.
            // The association state of the send station is validated when the packet
            // is processed.
            //
            // Otherwise, we need to hold a read lock on the 
            // Mac table to check the association state of the sending station.
            // We don't hold a write lock because we can tolerate
            // inconsistency in statistics to achieve better performance.
            //
            if (ApNeedToProcessPacket(
                    mgmtPktHeader, 
                    fragmentSize
                    ))
            {
                //
                // Process the management packet
                //
                
                //
                // 1. Release the read lock if it is held because a write lock may be acquired
                //    in the process of the management packet
                // 2. Process the management packet
                // This improves the performance if we receive a bunch of data packets.
                //
                if (holdReadLock)
                {
                    MP_RELEASE_READ_LOCK(&assocMgr->MacHashTableLock, &lockState);
                    holdReadLock = FALSE;
                }
                
                //
                // Don't pass the packet up if it is not valid
                //
                ndisStatus = ApProcessPacket(
                                MP_GET_AP_PORT(Port),
                                mgmtPktHeader,
                                fragmentSize
                                );
            }
            else
            {
                //
                // Need to know the association state of the sending station
                //
                
                //
                // Acquire read lock if we don't hold it
                //
                if (!holdReadLock)
                {
                    MP_ACQUIRE_READ_LOCK(&assocMgr->MacHashTableLock, &lockState);
                    holdReadLock = TRUE;
                }

                //
                // Lookup the station entry from Mac table if it exists
                //
                macEntry = LookupMacHashTable(
                            &assocMgr->MacHashTable, 
                            &mgmtPktHeader->SA
                            );
                
                // 
                // Get the station association state
                //
                if (macEntry != NULL)
                {
                    staEntry = CONTAINING_RECORD(macEntry, AP_STA_ENTRY, MacHashEntry);
                    assocState = ApGetStaAssocState(staEntry);
                    
                    //
                    // Reset station inactive time.
                    // No need to check the association state.
                    // Ignore the returned original value.
                    //
                    ApResetStaInactiveTime(staEntry);
                }
                else
                {
                    assocState = dot11_assoc_state_unauth_unassoc;
                }
                
                //
                // Get the frame class
                //
                frameClass = Dot11GetFrameClass(&mgmtPktHeader->FrameControl);

                switch(frameClass)
                {
                    case DOT11_FRAME_CLASS_1:
                        //
                        // Class 1 frames are always allowed
                        //
                        break;

                    case DOT11_FRAME_CLASS_2:
                        //
                        // Class 2 frames are allowed if the station is authenticated
                        //
                        if (dot11_assoc_state_unauth_unassoc == assocState)
                        {
                            ndisStatus = NDIS_STATUS_INVALID_STATE;
                        }
                        break;

                    case DOT11_FRAME_CLASS_3:
                        //
                        // Class 3 frames are allowed if the station is associated
                        //
                        if (assocState != dot11_assoc_state_auth_assoc)
                        {
                            ndisStatus = NDIS_STATUS_INVALID_STATE;
                        }
                        break;

                    default:
                        ndisStatus = NDIS_STATUS_INVALID_STATE;
                }

                //
                // Validate data packet
                //
                if (NDIS_STATUS_SUCCESS == ndisStatus && DOT11_FRAME_TYPE_DATA == mgmtPktHeader->FrameControl.Type)
                {
                    ndisStatus = ApValidateDataPacket(
                                    MP_GET_AP_PORT(Port), 
                                    rxFragment, 
                                    fragmentSize
                                    );
                }
            }

            //
            // Mark the status for the current packet and reset ndis status
            //
            MP_TX_MSDU_STATUS(currentPacket) = ndisStatus;
            ndisStatus = NDIS_STATUS_SUCCESS;
        }
             
        //
        // Release the read lock
        //
        if (holdReadLock)
        {
            MP_RELEASE_READ_LOCK(&assocMgr->MacHashTableLock, &lockState);
        }

    } while (FALSE);

    // 
    // Dereference AP port
    //
    ApDerefPort(MP_GET_AP_PORT(Port));

    return ndisStatus;
}
