/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    hw_mac.c

Abstract:
    Implements the MAC functionality for the HW layer
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#include "precomp.h"
#include "hw_mac.h"
#include "hw_send.h"
#include "hw_isr.h"
#include "hw_phy.h"
#include "hw_context.h"
#include "hw_main.h"

#if DOT11_TRACE_ENABLED
#include "hw_mac.tmh"
#endif

#define     HW_DEFAULT_PROBE_DELAY                      0
#define     HW_DEFAULT_ACTIVE_SCAN_CHANNEL_PARK_TIME    60
#define     HW_DEFAULT_PASSIVE_SCAN_CHANNEL_PARK_TIME   130

#define HW_TRANSITION_SCAN_STEP(_Scan_Step)    \
    (_Scan_Step = ((_Scan_Step + 1) % ScanStepMax))

NDIS_STATUS
HwSetATIMWindow(
    _In_  PHW                     Hw,
    _In_  ULONG                   Value
    )
{
    HalSetAtimWindow(Hw->Hal, Value);

    return NDIS_STATUS_SUCCESS;
}

VOID
HwUpdateUnicastCipher(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         HwMac
    )
{
    UCHAR                       i;

    // We dont use the info that the caller passed to us. Instead
    // we walk the list of active MAC_CONTEXT's cipher list and program all non-none
    // ciphers on the HW
    UNREFERENCED_PARAMETER(HwMac);

    // Clear all ciphers from the hardware by programming NONE as the cipher.
    HalSetEncryption(Hw->Hal, DOT11_CIPHER_ALGO_NONE);
    
    // Walk the active MAC's ciphers & program all non-none ciphers
    // on the hardware.
    for (i = 0; i < HW_MAX_NUMBER_OF_MAC; i++)
    {
        if (HW_MAC_CONTEXT_MUST_MERGE(&Hw->MacContext[i]))
        {
            if ((Hw->MacContext[i].UnicastCipher != DOT11_CIPHER_ALGO_NONE) && (HwMac->Hw->MacState.SafeModeEnabled == FALSE))
            {
                // Program this cipher
                HalSetEncryption(Hw->Hal, Hw->MacContext[i].UnicastCipher);        
                MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("Programming unicast cipher %d on the hardware.\n", Hw->MacContext[i].UnicastCipher));
            }
        }
    }
}


NDIS_STATUS
HwSetPowerMgmtMode(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PDOT11_POWER_MGMT_MODE  PMMode
    )
{
    UNREFERENCED_PARAMETER(HwMac);
    
    //
    // Our implementation is such that if only one non-helper MAC contexts is enabled
    // then we enable power save on the hardware. Else it is disabled
    //
    if (!HW_MULTIPLE_MAC_ENABLED(Hw) && (PMMode->dot11PowerMode != dot11_power_mode_unknown))
    {
        NdisMoveMemory(&Hw->MacState.PowerMgmtMode, PMMode, sizeof(DOT11_POWER_MGMT_MODE));
        HalSetPowerMgmtMode(Hw->Hal, PMMode);

    }
    
    return NDIS_STATUS_SUCCESS;
}


NDIS_STATUS
HwUpdatePacketFilter(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         HwMac
    )
{
    UCHAR                       i, numLinkActive = 0;
    ULONG                       programmedPacketFilter = 0;

    // Ignore the passed in context, program the sum of all
    // the active MAC's packet filter
    UNREFERENCED_PARAMETER(HwMac);
    
    // Walk the active MAC's ciphers & program the combination of all
    // packet filters on the hardware
    for (i = 0; i < HW_MAX_NUMBER_OF_MAC; i++)
    {
        if (HW_MAC_CONTEXT_MUST_MERGE(&Hw->MacContext[i]))
        {
            programmedPacketFilter = programmedPacketFilter | Hw->MacContext[i].PacketFilter;
            
            if (HW_TEST_MAC_CONTEXT_STATUS(&Hw->MacContext[i], HW_MAC_CONTEXT_LINK_UP))
            {
                numLinkActive++;
            }
        }
    }

    if (numLinkActive > 1)
    {
        // Multiple MACs are active & have link. We need to program promiscuous packet
        // filter on the H/W to ensure we get packets from both the MACs
        programmedPacketFilter |= (NDIS_PACKET_TYPE_802_11_PROMISCUOUS_MGMT | 
                                   NDIS_PACKET_TYPE_802_11_PROMISCUOUS_DATA);

        MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("Enabling promiscuous packet filter on the hardware \n"));
    }

    // This is the combined packet filter we want to set on the hardware
    Hw->MacState.PacketFilter = programmedPacketFilter;

    MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("Setting packet filter %!x! on the hardware \n", programmedPacketFilter));

    if (!HW_TEST_ADAPTER_STATUS(Hw, HW_CANNOT_ACCESS_HARDWARE))
        HalSetPacketFilter(Hw->Hal, programmedPacketFilter);

    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
HwUpdateMulticastAddressList(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         HwMac
    )
{
    UCHAR                       i;
    BOOLEAN                     allMulticastEnabled = FALSE;

    // Ignore the passed in context, program the sum of all
    // the active MAC's multicast list
    UNREFERENCED_PARAMETER(HwMac);

    // Start with nothing in the list
    Hw->MacState.MulticastAddressCount = 0;
    
    // Walk the active MAC's ciphers & program the combination of all
    // packet filters on the hardware
    for (i = 0; i < HW_MAX_NUMBER_OF_MAC; i++)
    {
        if (HW_MAC_CONTEXT_MUST_MERGE(&Hw->MacContext[i]))
        {
            if (Hw->MacContext[i].AcceptAllMulticast)
                allMulticastEnabled = TRUE;

            if (Hw->MacContext[i].MulticastAddressCount > 0)
            {
                // Add to the list on the HW
                if ((Hw->MacState.MulticastAddressCount + Hw->MacContext[i].MulticastAddressCount)
                            < HW11_MAX_MULTICAST_LIST_SIZE)
                {
                    // Append to the end of the existing list
                    NdisMoveMemory(&Hw->MacState.MulticastAddressList[Hw->MacState.MulticastAddressCount], 
                        Hw->MacContext[i].MulticastAddressList, 
                        Hw->MacContext[i].MulticastAddressCount * DOT11_ADDRESS_SIZE
                        );
        
                    Hw->MacState.MulticastAddressCount += Hw->MacContext[i].MulticastAddressCount;
                }
                else
                {
                    // Multicast list is full, enable allMulticast
                    MpTrace(COMP_OID, DBG_NORMAL, ("Multicast list is full, enabling ALL_MULTICAST filter \n"));                    
                    allMulticastEnabled = TRUE;
                    break;
                }
            }
        }
    }

    Hw->MacState.AcceptAllMulticast = allMulticastEnabled;

    if (!HW_TEST_ADAPTER_STATUS(Hw, HW_CANNOT_ACCESS_HARDWARE))
        HalSetMulticastMask(Hw->Hal, 
            Hw->MacState.AcceptAllMulticast, 
            Hw->MacState.MulticastAddressCount, 
            Hw->MacState.MulticastAddressList
            );

    return NDIS_STATUS_SUCCESS;
}


/** Add the key to the NIC */
NDIS_STATUS
HwAddKeyEntry(
    _In_  PHW                     Hw,
    _In_  PHW_KEY_ENTRY           KeyEntry
    )
{
    UCHAR                       nicKeyIndex, i;

    if (KeyEntry->NicKeyIndex < DOT11_MAX_NUM_DEFAULT_KEY)
    {
        // This is a default key. The entry on the HW for this default key
        // should be empty
        MPASSERT(Hw->MacState.KeyTable[KeyEntry->NicKeyIndex] == NULL);

        //
        // Add it to our key table at the same entry as in the caller
        //
        nicKeyIndex = KeyEntry->NicKeyIndex;
        Hw->MacState.KeyTable[nicKeyIndex] = &KeyEntry->Key;
    }
    else
    {
        // This is a key mapping key, find an empty slot
        HW_ACQUIRE_HARDWARE_LOCK(Hw, FALSE)
        if (Hw->MacState.KeyTable[KeyEntry->NicKeyIndex] == NULL)
        {
            // The caller suggested slot is empty. Use that index
            nicKeyIndex = KeyEntry->NicKeyIndex;                
        }
        else
        {
            // Find an empty index
            nicKeyIndex = 0;            
            for (i = DOT11_MAX_NUM_DEFAULT_KEY; i < HW11_KEY_TABLE_SIZE; i++)
            {
                if (Hw->MacState.KeyTable[i] == NULL)
                {
                    // Slot is empty. Use this one
                    nicKeyIndex = i;
                    break;
                }
            }
        }

        MPASSERT(nicKeyIndex != 0);
        
        // Add it to our key table at the desired entry
        if (nicKeyIndex != 0)
        {
            KeyEntry->NicKeyIndex = nicKeyIndex;
            Hw->MacState.KeyTable[nicKeyIndex] = &KeyEntry->Key;        
        }
        HW_RELEASE_HARDWARE_LOCK(Hw, FALSE)

        if (nicKeyIndex == 0)
        {
            // Didnt find a place to store this key. Fail
            return NDIS_STATUS_FAILURE;
        }
    }
    
    //
    // Program on the hardware
    //
    
    HalAddKeyEntry(Hw->Hal, &KeyEntry->Key, nicKeyIndex);

    if (nicKeyIndex >= DOT11_MAX_NUM_DEFAULT_KEY)
    {
        Hw->MacState.KeyMappingKeyCount++;
        MPASSERT(Hw->MacState.KeyMappingKeyCount <= HW_KEY_MAPPING_KEY_TABLE_SIZE);
    }

    return NDIS_STATUS_SUCCESS;
}

VOID
HwUpdateKeyEntry(
    _In_  PHW                     Hw,
    _In_  PHW_KEY_ENTRY           KeyEntry
    )
{
    //
    // The current entry must already be pointing to this entry
    // 
    MPASSERT(Hw->MacState.KeyTable[KeyEntry->NicKeyIndex] == &KeyEntry->Key);

    HalUpdateKeyEntry(Hw->Hal, &KeyEntry->Key, KeyEntry->NicKeyIndex);    
}


VOID
HwRemoveKeyEntry(
    _In_  PHW                     Hw,
    _In_  PHW_KEY_ENTRY           KeyEntry
    )
{
    UCHAR                       index = KeyEntry->NicKeyIndex;
    
    // Invalidate key from the table we have set on the hardware
    Hw->MacState.KeyTable[index] = NULL;

    HalRemoveKeyEntry(Hw->Hal, index);

    if (index >= DOT11_MAX_NUM_DEFAULT_KEY && Hw->MacState.KeyMappingKeyCount > 0)
    {
        Hw->MacState.KeyMappingKeyCount--;
    }
}

VOID
HwFreeKey(
    _In_  PHW_KEY_ENTRY           KeyEntry
    )
{
    if (KeyEntry->Key.Valid)
    {
        // Destroy the key material
        if (KeyEntry->hCNGKey)
        {
            BCryptDestroyKey(KeyEntry->hCNGKey);
            KeyEntry->hCNGKey = NULL;
        }

        // Then free the memory
        if (KeyEntry->CNGKeyObject) 
        {
            MP_FREE_MEMORY(KeyEntry->CNGKeyObject);
            KeyEntry->CNGKeyObject = NULL;
        }

        // We are deleting the HW_KEY structure
        NdisZeroMemory(KeyEntry, sizeof(HW_KEY_ENTRY));
        KeyEntry->Key.Valid = FALSE;
    }
}

UCHAR
HwQueryDefaultKeyMask(
    _In_  PHW_MAC_CONTEXT         HwMac
    )
{
    UCHAR                       validKeyMask = 0;
    UCHAR                       index;
    PHW_KEY_ENTRY               keyEntry = NULL;

    //
    // Go through the key table, and set a bit in the mask for all valid keys
    //
    for (index = 0; index < DOT11_MAX_NUM_DEFAULT_KEY; index++)
    {
        keyEntry = &HwMac->KeyTable[index];

        if (keyEntry->Key.Valid)
        {
            // The mask is set based on the NicKeyIndex since the stuff on the
            // hardware is the one that can cause a conflict
            validKeyMask |= (1 << (keyEntry->NicKeyIndex));
        }
    }

    return validKeyMask;
}


VOID
HwPickNicKeyIndexForKey(
    _In_  PHW                     Hw,
    _In_  PHW_KEY_ENTRY           KeyEntry,
    _In_  UCHAR                   PreferredKeyId
    )
{
    PHW_MAC_CONTEXT             currentMac;
    PHW_KEY_ENTRY               tempKeyEntry;
    UCHAR                       macIndex = 0, keyIndex;
    PHW_KEY_ENTRY               defaultKeyTable[DOT11_MAX_NUM_DEFAULT_KEY];
    
    if (PreferredKeyId < DOT11_MAX_NUM_DEFAULT_KEY)
    {
        //
        // For default keys, we pick the key index with special logic. We look at
        // all the MAC contexts & we shuffle keys based on the op mode. Our AP 
        // only support WPA2 auth/ciphers & wont ever use the default key for unicast
        // traffic. This means the default key does not need to be at the specific
        // key index on the hw since the h/w would never need it for decryption (broadcast
        // packets would always be unicast to the AP). So if we ever have a conflict 
        // in key indexes, we look at the op mode and move the AP's to 
        // a free default key slot
        //

        HW_ACQUIRE_HARDWARE_LOCK(Hw, FALSE);

        //
        // Populate the private table with indices that have used up the key index
        //
        NdisZeroMemory(defaultKeyTable, sizeof(PHW_MAC_CONTEXT) * DOT11_MAX_NUM_DEFAULT_KEY);
        for (macIndex = HW_DEFAULT_PORT_MAC_INDEX ; macIndex < HW_MAX_NUMBER_OF_MAC; macIndex++)        
        {
            currentMac = &Hw->MacContext[macIndex];
            if (HW_MAC_CONTEXT_IS_VALID(currentMac))
            {
                for (keyIndex = 0; keyIndex < DOT11_MAX_NUM_DEFAULT_KEY; keyIndex++)
                {
                    tempKeyEntry = &currentMac->KeyTable[keyIndex];
                    if (tempKeyEntry->Key.Valid)
                    {
                        if ((defaultKeyTable[tempKeyEntry->NicKeyIndex] != NULL) || (tempKeyEntry->NicKeyIndex >= DOT11_MAX_NUM_DEFAULT_KEY))
                        {
                            // We already have a conflict in our default table list.
                            // We wont try to optimize & just assign the preferred id to
                            // the new key

                            MpTrace(COMP_MISC, DBG_SERIOUS, 
                                ("Not optimizing default key indices on hardware due to pre-existing conflict at %d\n", tempKeyEntry->NicKeyIndex));
                                
                            KeyEntry->NicKeyIndex = PreferredKeyId;
                            HW_RELEASE_HARDWARE_LOCK(Hw, FALSE);
                            return;                            
                        }
                        defaultKeyTable[tempKeyEntry->NicKeyIndex] = tempKeyEntry;
                    }
                }
            }
        }

        //
        // At this point, we dont have any preexisting conflicts. Check if our new key
        // would cause a conflict. If not, we are done
        //
        if (defaultKeyTable[PreferredKeyId] == NULL)
        {
            // The desired index is empty
            MpTrace(COMP_MISC, DBG_NORMAL, 
                ("Default key index %d is available\n", PreferredKeyId));

            KeyEntry->NicKeyIndex = PreferredKeyId;
        }
        else
        {
            // There is a conflict. Check if we can move one of the keys around
            MpTrace(COMP_MISC, DBG_NORMAL, 
                ("Attempting to optimize default key index conflict for index %d\n", PreferredKeyId));
        
            // Determine which one to move
            if (KeyEntry->MacContext->CurrentOpMode == DOT11_OPERATION_MODE_EXTENSIBLE_AP)
            {
                // New key is for an AP. We will try to assign it a key in an empty slot
                MpTrace(COMP_MISC, DBG_NORMAL, 
                    ("Attempting to assign empty key index to new default key at %d\n", PreferredKeyId));

                tempKeyEntry = KeyEntry;
            }
            else if (defaultKeyTable[PreferredKeyId]->MacContext->CurrentOpMode == DOT11_OPERATION_MODE_EXTENSIBLE_AP)
            {
                // Key currently using the index is for an AP. We will assign the preferred index
                // to the new one and attempt to move the existing key around
                KeyEntry->NicKeyIndex = PreferredKeyId;

                MpTrace(COMP_MISC, DBG_NORMAL, 
                    ("Attempting to move existing default key at index %d\n", PreferredKeyId));

                // The key entry that we would update
                tempKeyEntry = defaultKeyTable[PreferredKeyId];
            }
            else
            {
                // Keys from neither MACs can be moved
                MpTrace(COMP_MISC, DBG_SERIOUS, 
                    ("Not optimizing default key indices on hardware since neither MAC is an AP\n"));
                KeyEntry->NicKeyIndex = PreferredKeyId;
                
                tempKeyEntry = NULL;// Done
            }

            if (tempKeyEntry != NULL)
            {
                // Try to find an empty index for this key. If we dont find one,
                // we will use the preferred index
                tempKeyEntry->NicKeyIndex = PreferredKeyId;
                
                for (keyIndex = 0; keyIndex < DOT11_MAX_NUM_DEFAULT_KEY; keyIndex++)
                {
                    if (defaultKeyTable[keyIndex] == NULL)
                    {
                        // Found an empty spot, assign
                        MpTrace(COMP_MISC, DBG_SERIOUS, 
                            ("Moving key at index %d to empty slot at %d\n", PreferredKeyId, keyIndex));

                        tempKeyEntry->NicKeyIndex = keyIndex;
                        break;
                    }
                }
            }

        }
        
        HW_RELEASE_HARDWARE_LOCK(Hw, FALSE)
    }    
    else
    {
        //
        // For key mapping keys, we dont do smarts here to pick the NicKeyIndex. 
        // The work would be done when the key gets programmed on the H/W later
        //
        KeyEntry->NicKeyIndex = PreferredKeyId;
    }
}


VOID
HwDeleteKeyFromContext(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  UCHAR                   KeyIndex,
    _In_  BOOLEAN                 fProgramHardware
    )
{
    BOOLEAN                     isValid = HwMac->KeyTable[KeyIndex].Key.Valid;
    
    HwMac->KeyTable[KeyIndex].Key.Valid = FALSE;

    if (fProgramHardware && isValid)
    {
        //
        // Delete the corresponding key from the hardware
        //
        HwRemoveKeyEntry(HwMac->Hw, &HwMac->KeyTable[KeyIndex]);
    }

    // Clear all key state
    HwFreeKey(&(HwMac->KeyTable[KeyIndex]));

    //
    // We have invalidated it. Now even if we dont remove it from the hardware (fProgramHardware = FALSE)
    // it wont get set next time the context becomes active
    //
    if (KeyIndex >= DOT11_MAX_NUM_DEFAULT_KEY && HwMac->KeyMappingKeyCount > 0)
    {
        HwMac->KeyMappingKeyCount--;
    }
}


VOID
HwDeleteAllKeysFromContext(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 fProgramHardware
    )
{
    UCHAR                       index, defaultIndex;

    //
    // Delete all the keys from our key table
    //
    for (index = 0; index < HW11_KEY_TABLE_SIZE; index++)
    {
        HwDeleteKeyFromContext(HwMac, index, fProgramHardware);
    }

    //
    // Invalidate the default keys from all the peers nodes
    //
    for (index = 0; index < HW11_PEER_TABLE_SIZE; index++)
    {
        for (defaultIndex = 0; defaultIndex < DOT11_MAX_NUM_DEFAULT_KEY; defaultIndex++)
        {
            // These are not put the HW_MAC_CONTEXT so we dont have to 
            // clear these
            HwFreeKey(&HwMac->PeerTable[index].PrivateKeyTable[defaultIndex]);
        }
    }


}

VOID
HwDeleteAllKeysFromHw(
    _In_  PHW_MAC_CONTEXT         HwMac
    )
{
    UCHAR index;
    PHW_KEY_ENTRY KeyEntry = NULL;

    //
    // Delete all the keys from our key table
    //
    for (index = 0; index < HW11_KEY_TABLE_SIZE; index++)
    {        
        KeyEntry = &HwMac->KeyTable[index];
        if (KeyEntry->Key.Valid)
        {
           HwRemoveKeyEntry(HwMac->Hw, KeyEntry);
        }
    }
}

VOID
HwSetDefaultKeyId(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  ULONG                   KeyId
    )
{
    // We dont set the default Key ID at this time
    UNREFERENCED_PARAMETER(Hw);
    UNREFERENCED_PARAMETER(HwMac);
    UNREFERENCED_PARAMETER(KeyId);
}

NDIS_STATUS
HwSetKeyInContext(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PHW_KEY_ENTRY           KeyEntry,
    _In_  UCHAR                   KeyID,
    _In_  BOOLEAN                 Persistent,
    _In_  DOT11_CIPHER_ALGORITHM  AlgoId,
    _In_  ULONG                   KeyLength,
    _In_reads_bytes_(KeyLength)  PUCHAR                  KeyValue,
    _In_  BOOLEAN                 fProgramHardware
    )
{
    ULONG                       i;
    PDOT11_KEY_ALGO_CCMP        CCMPKey = NULL;
    PDOT11_KEY_ALGO_TKIP_MIC    TKIPKey = NULL;
    BOOLEAN                     isKeyUpdate = FALSE;
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    
    MPASSERT(HwMac->Hw->MacState.SafeModeEnabled == FALSE);

    //
    // Do not update the HwMac->KeyTable[KeyID] field. The caller has specified
    // which KeyEntry to update and only that should be modified. Thsi is because this function
    // is also called for per-Peer default keys (WPA2/Adhoc) and in that case we can multiple
    // default keys at the same index
    //

    //
    // This is serialized by NDIS OID serialization. We dont need to hold
    // a lock for this
    //

    do
    {
        //
        // If keyLength is non-zero, the key is to be added, otherwise, it is to be deleted.
        // Note: MacAddr of the key is already set.
        //    
        if (KeyLength != 0)
        {
            //
            // Adding the key. First we preprocess the key
            //
            switch (AlgoId)
            {
                case DOT11_CIPHER_ALGO_WEP:
                    //
                    // If the algoId is generic WEP, change it to WEP40 or WEP104 depending on the key length.
                    //
                    switch (KeyLength)
                    {
                        case 40 / 8:
                            AlgoId = DOT11_CIPHER_ALGO_WEP40;
                            break;

                        case 104 / 8:
                            AlgoId = DOT11_CIPHER_ALGO_WEP104;
                            break;

                        default:
                            ndisStatus = NDIS_STATUS_INVALID_DATA;
                            break;
                    }

                    // fall through

                case DOT11_CIPHER_ALGO_WEP104:
                case DOT11_CIPHER_ALGO_WEP40:
                    break;

                case DOT11_CIPHER_ALGO_CCMP:
                    //
                    // Validate the key length
                    //
                    if (KeyLength < FIELD_OFFSET(DOT11_KEY_ALGO_CCMP, ucCCMPKey)) 
                    {
                        ndisStatus = NDIS_STATUS_INVALID_DATA;
                        break;
                    }
                    
                    CCMPKey = (PDOT11_KEY_ALGO_CCMP)KeyValue;
                    if (KeyLength < FIELD_OFFSET(DOT11_KEY_ALGO_CCMP, ucCCMPKey) + 
                                    CCMPKey->ulCCMPKeyLength)
                    {
                        ndisStatus = NDIS_STATUS_INVALID_DATA;
                        break;
                    }

                    //
                    // Only support 16-byte CCMP key 
                    //
                    if (CCMPKey->ulCCMPKeyLength != 16)
                    {
                        ndisStatus = NDIS_STATUS_INVALID_DATA;
                        break;
                    }
                    
                    KeyLength = CCMPKey->ulCCMPKeyLength;
                    KeyValue = CCMPKey->ucCCMPKey;
                    
                    break;

                case DOT11_CIPHER_ALGO_TKIP:
                    //
                    // Validate the key length
                    //
                    if (KeyLength < FIELD_OFFSET(DOT11_KEY_ALGO_TKIP_MIC, ucTKIPMICKeys)) 
                    {
                        ndisStatus = NDIS_STATUS_INVALID_DATA;
                        break;
                    }
                    
                    TKIPKey = (PDOT11_KEY_ALGO_TKIP_MIC)KeyValue;
                    if (KeyLength < FIELD_OFFSET(DOT11_KEY_ALGO_TKIP_MIC, ucTKIPMICKeys) + 
                                    TKIPKey->ulTKIPKeyLength +
                                    TKIPKey->ulMICKeyLength) 
                    {
                        ndisStatus = NDIS_STATUS_INVALID_DATA;
                        break;
                    }
                    
                    //
                    // Only support 16-byte TKIP key and 8-byte Tx/Rx MIC key
                    //
                    if (TKIPKey->ulTKIPKeyLength != 16 || TKIPKey->ulMICKeyLength != 16)
                    {
                        ndisStatus = NDIS_STATUS_INVALID_DATA;
                        break;
                    }

                    KeyLength = TKIPKey->ulTKIPKeyLength;
                    KeyValue = TKIPKey->ucTKIPMICKeys;
                    
                    break;
            }

            if (ndisStatus != NDIS_STATUS_SUCCESS)
            {
                // Get out
                break;
            }

            //
            // If the current encryption algorithm is WEP, change it to more specific WEP40 or WEP104.
            //
            if (HwMac->UnicastCipher == DOT11_CIPHER_ALGO_WEP &&
                (AlgoId == DOT11_CIPHER_ALGO_WEP40 || AlgoId == DOT11_CIPHER_ALGO_WEP104))
            {
                HwMac->UnicastCipher = AlgoId;

                if (fProgramHardware)
                {
                    HwUpdateUnicastCipher(HwMac->Hw, HwMac);
                }
            }

            if (HwMac->UnicastCipher == DOT11_CIPHER_ALGO_WEP &&
                (AlgoId == DOT11_CIPHER_ALGO_WEP40 || AlgoId == DOT11_CIPHER_ALGO_WEP104))
            {
                HwMac->MulticastCipher = AlgoId;
            }

            //
            // We should never have unicast and multicast cipher with different length of WEP.
            //
            MPASSERT(!(HwMac->UnicastCipher == DOT11_CIPHER_ALGO_WEP40 && 
                     HwMac->MulticastCipher == DOT11_CIPHER_ALGO_WEP104));

            MPASSERT(!(HwMac->UnicastCipher == DOT11_CIPHER_ALGO_WEP104 && 
                     HwMac->MulticastCipher == DOT11_CIPHER_ALGO_WEP40));

            //
            // For key mapping key, its algorithm must match current unicast cipher (unless 
            // the key is for multicast/broadcast data frames).
            //
            // For key mapping key for multicast/broadcast data frames, 
            // its algorithm must match the current unicast cipher.
            //
            // For default key, its algorithm must match either the current unicast cipher
            // or the current multicast cipher. 
            //
            if (HwMac->UnicastCipher != AlgoId && 
                KeyID >= DOT11_MAX_NUM_DEFAULT_KEY && 
                DOT11_IS_UNICAST(KeyEntry->Key.MacAddr))
            {
                ndisStatus = NDIS_STATUS_INVALID_DATA;
                break;
            }

            if (HwMac->MulticastCipher != AlgoId && 
                DOT11_IS_MULTICAST(KeyEntry->Key.MacAddr))
            {
                ndisStatus = NDIS_STATUS_INVALID_DATA;
                break;
            }

            if (HwMac->UnicastCipher != AlgoId && 
                HwMac->MulticastCipher != AlgoId)
            {
                ndisStatus = NDIS_STATUS_INVALID_DATA;
                break;
            }

            // Was the old key valid, or is this a new entry
            isKeyUpdate = KeyEntry->Key.Valid;

            //
            // Update the key entry for valid key. We cannot fail after this point. Otherwise, 
            // the existing key will be modified when we fail to set the new key.
            //
            KeyEntry->Key.Persistent = Persistent;
            KeyEntry->Key.KeyLength = (UCHAR) KeyLength;
            KeyEntry->Key.AlgoId = AlgoId;
            KeyEntry->MacContext = HwMac;

            //
            // If this is not a key update, we suggest the key index to update
            // by matching the NIC key ID with software key ID. Else we dont touch this
            //
            if (!isKeyUpdate)
            {
                HwPickNicKeyIndexForKey(HwMac->Hw, KeyEntry, KeyID);
            }

            //
            // The key index for this node per the peer
            //
            KeyEntry->PeerKeyIndex = KeyID;
            
            KeyEntry->Key.Valid = TRUE;
            
            if (KeyID < DOT11_MAX_NUM_DEFAULT_KEY)
            {
                MpTrace(COMP_OID, DBG_NORMAL, ("%s default key %d (algo %d): ", 
                            (isKeyUpdate ? "Update" : "Add"), KeyID, AlgoId));
            }
            else
            {
                MpTrace(COMP_OID, DBG_NORMAL, ("%s key mapping key for %02X-%02X-%02X-%02X-%02X-%02X (algo %d): ", 
                        (isKeyUpdate ? "Update" : "Add"),
                        KeyEntry->Key.MacAddr[0], KeyEntry->Key.MacAddr[1], KeyEntry->Key.MacAddr[2], 
                        KeyEntry->Key.MacAddr[3], KeyEntry->Key.MacAddr[4], KeyEntry->Key.MacAddr[5], AlgoId));
            }

            //
            // Save the key
            //
            for (i = 0; i < KeyLength; i++) 
            {
                KeyEntry->Key.KeyValue[i] = KeyValue[i];
                MpTrace(COMP_OID, DBG_NORMAL, ("%02X", KeyValue[i]));
            }
            MpTrace(COMP_OID, DBG_NORMAL, ("\n"));

            for (i = KeyLength; i < sizeof(KeyEntry->Key.KeyValue); i++) 
                KeyEntry->Key.KeyValue[i] = 0;

            //
            // Only for per peer default keys (in PrivateKeyTable) we create the CNG handle.
            // No need to do that here
            //

            switch (AlgoId)
            {
                case DOT11_CIPHER_ALGO_WEP:
                case DOT11_CIPHER_ALGO_WEP104:
                case DOT11_CIPHER_ALGO_WEP40:
                    KeyEntry->IV = 1;
                    break;

                case DOT11_CIPHER_ALGO_CCMP:
                    KeyEntry->PN = 1;
                    KeyEntry->ReplayCounter = ((ULONGLONG)CCMPKey->ucIV48Counter[0]) |
                                             (((ULONGLONG)CCMPKey->ucIV48Counter[1]) << 8) |
                                             (((ULONGLONG)CCMPKey->ucIV48Counter[2]) << 16) |
                                             (((ULONGLONG)CCMPKey->ucIV48Counter[3]) << 24) |
                                             (((ULONGLONG)CCMPKey->ucIV48Counter[4]) << 32) |
                                             (((ULONGLONG)CCMPKey->ucIV48Counter[5]) << 40);
                    break;

                case DOT11_CIPHER_ALGO_TKIP:
                    KeyEntry->TSC = 1;

                    KeyEntry->ReplayCounter = ((ULONGLONG)TKIPKey->ucIV48Counter[0]) |
                                             (((ULONGLONG)TKIPKey->ucIV48Counter[1]) << 8) |
                                             (((ULONGLONG)TKIPKey->ucIV48Counter[2]) << 16) |
                                             (((ULONGLONG)TKIPKey->ucIV48Counter[3]) << 24) |
                                             (((ULONGLONG)TKIPKey->ucIV48Counter[4]) << 32) |
                                             (((ULONGLONG)TKIPKey->ucIV48Counter[5]) << 40);
                    NdisMoveMemory(KeyEntry->Key.RxMICKey, Add2Ptr(KeyValue, KeyLength), 8);
                    NdisMoveMemory(KeyEntry->Key.TxMICKey, Add2Ptr(KeyValue, KeyLength + 8), 8);

                    break;
            }

            //
            // If ok, program the hardware
            //
            if (fProgramHardware)
            {
                if (isKeyUpdate)
                {
                    HwUpdateKeyEntry(HwMac->Hw, KeyEntry);
                }
                else
                {
                    HwAddKeyEntry(HwMac->Hw, KeyEntry);
                }
            }
        }
        else 
        {
            //
            // Remove the key from hardware.
            // Dont call HwFreeKey. Callers like SoftwareDefaultKey
            // may need to do more work on the key
            //            
            NdisZeroMemory(&KeyEntry->Key, sizeof(NICKEY)); 
            if (fProgramHardware)
            {
                HwRemoveKeyEntry(HwMac->Hw, KeyEntry);
            }
        }
    } while (FALSE);
    
    return ndisStatus;
}

NDIS_STATUS
HwSetDefaultKeyInContext(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_reads_bytes_(DOT11_ADDRESS_SIZE)  DOT11_MAC_ADDRESS       MacAddr,
    _In_  UCHAR                   KeyID,
    _In_  BOOLEAN                 Persistent,
    _In_  DOT11_CIPHER_ALGORITHM  AlgoId,
    _In_  ULONG                   KeyLength,
    _In_reads_bytes_(KeyLength)  PUCHAR                  KeyValue,
    _In_  BOOLEAN                 fProgramHardware
    )
{
    UNREFERENCED_PARAMETER(MacAddr);

    // We dont go through the per-peer table for the keys 
    // that we want to program to the hardware    
    return HwSetKeyInContext(HwMac,
                &HwMac->KeyTable[KeyID],
                KeyID,
                Persistent,
                AlgoId,
                KeyLength,
                KeyValue,
                fProgramHardware
                );
}

NDIS_STATUS 
HwSetSoftwareDefaultKeyInContext(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_reads_bytes_(DOT11_ADDRESS_SIZE)  DOT11_MAC_ADDRESS       MacAddr,
    _In_  UCHAR                   KeyID,
    _In_  BOOLEAN                 Persistent,
    _In_  DOT11_CIPHER_ALGORITHM  AlgoId,
    _In_  ULONG                   KeyLength,
    _In_reads_bytes_(KeyLength)  PUCHAR                  KeyValue
    )
{
    NDIS_STATUS                 ndisStatus;
    BOOLEAN                     keyAdd = TRUE;
    PHW_PEER_NODE               peerNode;
    ULONG                       i;
    PVOID                       CNGKeyObject = NULL;
    PDOT11_KEY_ALGO_CCMP        CCMPKey = NULL;
    
    //
    // per-STA default key is only supported for WPA2PSK adhoc. Even though it
    // is not supported in hardware, we support per-STA default key in software.
    //
    MPASSERT(HwMac->AuthAlgorithm == DOT11_AUTH_ALGO_RSNA_PSK);
    MPASSERT(HalGetEncryptionCapabilities(HwMac->Hw->Hal) & HAL_ENCRYPTION_SUPPORT_CCMP);
    
    if ((!(HalGetEncryptionCapabilities(HwMac->Hw->Hal) & HAL_ENCRYPTION_SUPPORT_CCMP)) ||
        (HwMac->AuthAlgorithm != DOT11_AUTH_ALGO_RSNA_PSK))
    {
        return NDIS_STATUS_NOT_SUPPORTED;
    }
    
    keyAdd = (KeyLength == 0)? FALSE : TRUE;
    
    //
    // If this is a key add, allocate memory for the key structure
    //
    if (keyAdd)
    {
        MP_ALLOCATE_MEMORY(HwMac->Hw->MiniportAdapterHandle, &CNGKeyObject, HwMac->Hw->CryptoState.KeyObjLen, HW_MEMORY_TAG);
        if (CNGKeyObject == NULL)
        {
            MpTrace(COMP_OID, DBG_SERIOUS, ("Unable to allocate memory for CNG key material\n"));
            return NDIS_STATUS_RESOURCES;
        }
    }
    
    //
    // Search the peer key table to find the matching MacAddr. If there is
    // no node present, we would create a new one if this is a key add. If 
    // a key delete, we wont create the new node.
    //
    if ((MacAddr[0] == 0) && (MacAddr[1] == 0) && (MacAddr[2] == 0) &&
        (MacAddr[3] == 0) && (MacAddr[4] == 0) && (MacAddr[5] == 0))
    {
        // This is the default key for multicast TX. We use the 
        // default port to hold this key
        peerNode = &HwMac->DefaultPeer;
    }
    else
    {    
        // This is the per-STA key. Find or create the peer node
        
        peerNode = HwFindPeerNode(HwMac, MacAddr, keyAdd);
    }
    
    if (peerNode == NULL)
    {
        if (keyAdd)
        {
            //
            // We wanted to add a new node, but couldnt
            //
            MpTrace(COMP_OID, DBG_SERIOUS, ("Peer node table full. Unable to add new peer default key\n"));
            MP_FREE_MEMORY(CNGKeyObject);
            return NDIS_STATUS_RESOURCES;
        }
        else
        {
            //
            // Asked to delete a key when there is no node present holding
            // that key
            //
            MpTrace(COMP_OID, DBG_NORMAL, ("Peer node table does not contain the default key to delete\n"));
            return NDIS_STATUS_INVALID_DATA;
        }
    }

    //
    // Set the key in MAC context structure. We dont want the key to
    // get programmed on the hardware
    //
    ndisStatus = HwSetKeyInContext(HwMac, 
                    &(peerNode->PrivateKeyTable[KeyID]),
                    (UCHAR)KeyID, 
                    Persistent, 
                    AlgoId, 
                    KeyLength, 
                    KeyValue,
                    FALSE
                    );

    if (ndisStatus == NDIS_STATUS_SUCCESS) 
    {
        HW_ACQUIRE_HARDWARE_LOCK(HwMac->Hw, FALSE);
        if (KeyLength != 0)
        {
            //
            // Key being added. We have already copied the key into
            // our peer node entry. Set the flag that we are going to
            // do encryption/decryption in software
            // 
            peerNode->PrivateKeyTable[KeyID].SoftwareOnly = TRUE;
            peerNode->PrivateKeyTable[KeyID].MacContext = HwMac;

            //
            // For software keys, we save the CNG info of the key
            //
            if (peerNode->PrivateKeyTable[KeyID].hCNGKey)
            {
                // Old key exists, delete it
                BCryptDestroyKey(peerNode->PrivateKeyTable[KeyID].hCNGKey);
                peerNode->PrivateKeyTable[KeyID].hCNGKey = NULL;
            }

            if (peerNode->PrivateKeyTable[KeyID].CNGKeyObject)
            {
                MP_FREE_MEMORY(peerNode->PrivateKeyTable[KeyID].CNGKeyObject);
                peerNode->PrivateKeyTable[KeyID].CNGKeyObject = NULL;
            }

            if (AlgoId == DOT11_CIPHER_ALGO_CCMP)
            {
                CCMPKey = (PDOT11_KEY_ALGO_CCMP)KeyValue;
                
                //
                // Save the new key
                //
                ndisStatus = BCryptGenerateSymmetricKey(
                               HwMac->Hw->CryptoState.AlgoHandle,
                               &peerNode->PrivateKeyTable[KeyID].hCNGKey,
                               CNGKeyObject,
                               HwMac->Hw->CryptoState.KeyObjLen,
                               CCMPKey->ucCCMPKey,
                               CCMPKey->ulCCMPKeyLength,
                               0
                    );
                if (!NT_SUCCESS(ndisStatus))
                {
                    MpTrace(COMP_OID, DBG_SERIOUS, ("Unable to generate CNG key. Error = %d\n", ndisStatus));
                    ndisStatus = NDIS_STATUS_FAILURE;
                }
                else
                {
                    peerNode->PrivateKeyTable[KeyID].CNGKeyObject = CNGKeyObject;
                    CNGKeyObject = NULL;
                }
            }
            else
            {
                // This is not OK
                ndisStatus = NDIS_STATUS_FAILURE;
            }
        }
        else 
        {
            MPASSERT(peerNode->Valid == TRUE);

            //
            // The lifetime of our peer nodes is maintained even when 
            // there is no key. So we dont invalidate the node here. But we free the key
            //
            for (i = 0; i < DOT11_MAX_NUM_DEFAULT_KEY; i++)
            {
                // Remove any private key that may still be around
                if (peerNode->PrivateKeyTable[i].Key.Valid)
                {
                    HwFreeKey(&peerNode->PrivateKeyTable[i]);
                }
            }
        }
        HW_RELEASE_HARDWARE_LOCK(HwMac->Hw, FALSE);
        
    }

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        if (CNGKeyObject)
            MP_FREE_MEMORY(CNGKeyObject);
    }

    return ndisStatus;
}

NDIS_STATUS 
HwSetKeyMappingKeyInContext(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_reads_bytes_(DOT11_ADDRESS_SIZE)  DOT11_MAC_ADDRESS       MacAddr,
    _In_  DOT11_DIRECTION         Direction,
    _In_  BOOLEAN                 Persistent,
    _In_  DOT11_CIPHER_ALGORITHM  AlgoId,
    _In_  ULONG                   KeyLength,
    _In_reads_bytes_(KeyLength)  PUCHAR                  KeyValue,
    _In_  BOOLEAN                 fProgramHardware
    )
{
    NDIS_STATUS                 ndisStatus;
    BOOLEAN                     keyAdd = TRUE;
    BOOLEAN                     isKeyUpdate = FALSE;
    PHW_PEER_NODE               peerNode = NULL;
    ULONG                       contextKeyIndex;

    //
    // We don't support uni-direction key mapping keys
    //
    if (Direction != DOT11_DIR_BOTH)
    {
        MpTrace(COMP_OID, DBG_NORMAL, ("Only bi-directional key-mapping keys are supported\n"));
        return NDIS_STATUS_NOT_SUPPORTED;
    }

    keyAdd = (KeyLength == 0)? FALSE : TRUE;

    //
    // Search the key mapping table to find either a matching MacAddr or an empty key entry.
    //
    ndisStatus = HwFindKeyMappingKeyIndex(HwMac, MacAddr, keyAdd, &contextKeyIndex);
    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        if (keyAdd)
        {
            //
            // We wanted to add a new node, but couldnt
            //
            MpTrace(COMP_OID, DBG_NORMAL, ("Key-Mapping keytable is full. Unable to add any more keys\n"));            
            return NDIS_STATUS_RESOURCES;
        }
        else
        {
            //
            // Asked to delete a key when there is no node present holding
            // that key
            //
            MpTrace(COMP_OID, DBG_NORMAL, ("Key-Mapping keytable does not contain the key entry to delete\n"));
            return NDIS_STATUS_INVALID_DATA;
        }
    }
    
    //
    // Search the peer key table to find the matching MacAddr. If there is
    // no node present, we would create a new one if this is a key add. If 
    // a key delete, we wont create the new node.
    //
    peerNode = HwFindPeerNode(HwMac, MacAddr, keyAdd);
    if (peerNode == NULL)
    {
        if (keyAdd)
        {
            //
            // We wanted to add a new node, but couldnt
            //
            MpTrace(COMP_OID, DBG_NORMAL, ("Peer node table full. Unable to add new key-mapping key\n"));
            return NDIS_STATUS_RESOURCES;
        }
        else
        {
            //
            // Asked to delete a key when there is no node present holding
            // that key
            //
            MpTrace(COMP_OID, DBG_NORMAL, ("Peer node table does not contain the key-mapping key to delete\n"));
            return NDIS_STATUS_INVALID_DATA;
        }
    }


    //
    // This is the location that the key should be added in the context's
    // key table
    //
    peerNode->KeyMappingKey = &(HwMac->KeyTable[contextKeyIndex]);
    // Was the old key valid, or is this a new entry
    isKeyUpdate = peerNode->KeyMappingKey->Key.Valid;    
    
    //
    // Set the key in MAC context structure
    //
    ndisStatus = HwSetKeyInContext(HwMac, 
                    peerNode->KeyMappingKey,
                    (UCHAR)contextKeyIndex, 
                    Persistent, 
                    AlgoId, 
                    KeyLength, 
                    KeyValue,
                    fProgramHardware
                    );

    if (ndisStatus == NDIS_STATUS_SUCCESS)
    {
        if (keyAdd)
        {
            if (!isKeyUpdate)
            {
                // Key added
                HwMac->KeyMappingKeyCount++;
            }
        }
        else
        {
            // Key removed
            HwMac->KeyMappingKeyCount--;
        }
    }

    return ndisStatus;
}

NDIS_STATUS
HwSetOperationMode(
    _In_  PHW                     Hw,
    _In_  ULONG                   Dot11CurrentOperationMode
    )
{
    HW_HAL_RESET_PARAMETERS resetParams;

    if (Hw->MacState.OperationMode != Dot11CurrentOperationMode)
    {    
        Hw->MacState.OperationMode = Dot11CurrentOperationMode;

        if (!HW_TEST_ADAPTER_STATUS(Hw, HW_CANNOT_ACCESS_HARDWARE))
        {
            // TODO: This could be implemented better. We should update the HAL interface
            // such that we dont call this API twice
            HalSetOperationMode(Hw->Hal, Dot11CurrentOperationMode);

            // When we change op mode, we reset the HAL
            NdisZeroMemory(&resetParams, sizeof(HW_HAL_RESET_PARAMETERS));
            HwResetHAL(Hw, &resetParams, FALSE);

            // We re-set the op mode to ensure that the hardware is actually set for the correct mode
            HalSetOperationMode(Hw->Hal, Dot11CurrentOperationMode);

            if (Dot11CurrentOperationMode == DOT11_OPERATION_MODE_NETWORK_MONITOR)
            {
                Hw->MacState.NetmonModeEnabled = TRUE;
            }
        }
    }
    
    return NDIS_STATUS_SUCCESS;
}


NDIS_STATUS
HwUpdateOperationMode(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         HwMac
    )
{
    UCHAR                       i;
    BOOLEAN                     apActive = FALSE;
    BOOLEAN                     adhocActive = FALSE;

    
    // Walk the active MAC's link state & program either AP/adhoc mode or
    // the current op mode on the HW
    for (i = 0; i < HW_MAX_NUMBER_OF_MAC; i++)
    {
        if (HW_MAC_CONTEXT_MUST_MERGE(&Hw->MacContext[i]))
        {
            if (Hw->MacContext[i].CurrentOpMode == DOT11_OPERATION_MODE_EXTENSIBLE_AP)
            {
                apActive = TRUE;
            }
            else if ((Hw->MacContext[i].BssType == dot11_BSS_type_independent) &&
                    (Hw->MacContext[i].CurrentOpMode == DOT11_OPERATION_MODE_EXTENSIBLE_STATION))
            {
                adhocActive = TRUE;
            }
        }
    }

    if (adhocActive == TRUE)
    {
        // If adhoc is enabled the AP is not going to be started
        MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("Programming ADHOC mode on the HW\n"));
        
        // Program STA mode on the HW
        return HwSetOperationMode(Hw, DOT11_OPERATION_MODE_EXTENSIBLE_STATION);
    }
    else if (apActive == TRUE)
    {
        MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("Programming AP mode on the HW\n"));
        
        // Program AP mode on the HW
        return HwSetOperationMode(Hw, DOT11_OPERATION_MODE_EXTENSIBLE_AP);
    }
    else
    {
        // Program current MAC context's mode
        MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("Programming %d mode on the HW\n", HwMac->CurrentOpMode));
        return HwSetOperationMode(Hw, HwMac->CurrentOpMode);
    }

}


NDIS_STATUS
HwSetBSSType(
    _In_  PHW                     Hw,
    _In_  DOT11_BSS_TYPE          BssType
    )
{
    if (!HW_TEST_ADAPTER_STATUS(Hw, HW_CANNOT_ACCESS_HARDWARE))
        HalSetCurrentBSSType(Hw->Hal, BssType);
    Hw->MacState.BssType = BssType;

    return NDIS_STATUS_SUCCESS;
}


NDIS_STATUS
HwUpdateBSSType(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         HwMac
    )
{
    UCHAR                       i;
    BOOLEAN                     adhocActive = FALSE;
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;

    // Walk the active MAC's link state & program either adhoc mode or
    // the current op mode on the HW
    for (i = 0; i < HW_MAX_NUMBER_OF_MAC; i++)
    {
        if (HW_MAC_CONTEXT_MUST_MERGE(&Hw->MacContext[i]))
        {
            if (Hw->MacContext[i].BssType == dot11_BSS_type_independent)
            {
                adhocActive = TRUE;
            }
        }
    }

    if (adhocActive == TRUE)
    {
        MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("Programming adhoc bss type on the HW\n"));

        // Ensure that the H/W is in ExtSTA op mode
        if (Hw->MacState.OperationMode != DOT11_OPERATION_MODE_EXTENSIBLE_STATION)
        {
            ndisStatus = HwUpdateOperationMode(Hw, HwMac);
            
        }
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("Failed to switch H/W op mode to ExtSTA for adhoc \n"));
        }
        else
        {
            // Program adhoc mode on the HW
            ndisStatus = HwSetBSSType(Hw, dot11_BSS_type_independent);
        }
    }
    else
    {
        // Program current MAC context's mode
        MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("Programming %d bss type on the HW\n", HwMac->BssType));
        ndisStatus = HwSetBSSType(Hw, HwMac->BssType);
    }

    return ndisStatus;
}

NDIS_STATUS
HwUpdateBSSID(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         HwMac
    )
{
    UCHAR                       i, numLinkActive = 0;
    DOT11_MAC_ADDRESS           programmedBssid;
    DOT11_MAC_ADDRESS           broadcastAddress = {0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF};

    // If we dont find a better candidate BSSID, we use the one from the calling 
    // function    
    NdisMoveMemory(programmedBssid, HwMac->DesiredBSSID, DOT11_ADDRESS_SIZE);
    
    // Walk the active MAC's BSSID & if there are multiple active,
    // program the default, else program the valid BSSID
    for (i = 0; i < HW_MAX_NUMBER_OF_MAC; i++)
    {
        if (HW_MAC_CONTEXT_MUST_MERGE(&Hw->MacContext[i]))
        {
            if (HW_TEST_MAC_CONTEXT_STATUS(&Hw->MacContext[i], HW_MAC_CONTEXT_LINK_UP))
            {
                // Program this MAC's BSSID on the H/W
                NdisMoveMemory(programmedBssid, Hw->MacContext[i].DesiredBSSID, DOT11_ADDRESS_SIZE);
                numLinkActive++;
            }
        }
    }

    if (numLinkActive > 1)
    {
        // Multiple BSSIDs active, program broadcast BSSID
        NdisMoveMemory(programmedBssid, broadcastAddress, DOT11_ADDRESS_SIZE);
        MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("Multiple MACs active, programming broadcast address (%!x! %!x! %!x! %!x! %!x! %!x! ) as the Bssid\n", programmedBssid[0], programmedBssid[1], programmedBssid[2], programmedBssid[3], programmedBssid[4], programmedBssid[5] ));
    }
    else
    {
        // Single MAC active, program the active BSSID
        MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("Single MAC active, programming desired BSSid (%!x! %!x! %!x! %!x! %!x! %!x! ) as the Bssid\n", programmedBssid[0], programmedBssid[1], programmedBssid[2], programmedBssid[3], programmedBssid[4], programmedBssid[5] ));
    }

    // Save the BSSID for debugging purpose
    NdisMoveMemory(Hw->MacState.Bssid, programmedBssid, DOT11_ADDRESS_SIZE);
    if (!HW_TEST_ADAPTER_STATUS(Hw, HW_CANNOT_ACCESS_HARDWARE))
        HalSetBssId(Hw->Hal, programmedBssid);
    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
HwSetBeaconPeriod(
    _In_  PHW                     Hw,
    _In_  ULONG                   BeaconPeriod
    )
{
    HalSetBeaconInterval(Hw->Hal, BeaconPeriod);

    return NDIS_STATUS_SUCCESS;

}


NDIS_STATUS
HwUpdateLinkState(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         HwMac
    )
{
    UCHAR                       i;
    BOOLEAN                     linkUp = FALSE;

    UNREFERENCED_PARAMETER(HwMac);
    if (HW_TEST_ADAPTER_STATUS(Hw, HW_CANNOT_ACCESS_HARDWARE))
        return NDIS_STATUS_SUCCESS;
    
    // Walk the active MAC's link state & if any MAC has link, that
    // is programmed on the hardware
    for (i = 0; i < HW_MAX_NUMBER_OF_MAC; i++)
    {
        if (HW_MAC_CONTEXT_MUST_MERGE(&Hw->MacContext[i]))
        {
            if (HW_TEST_MAC_CONTEXT_STATUS(&Hw->MacContext[i], HW_MAC_CONTEXT_LINK_UP))
            {
                linkUp = TRUE;
            }
        }
    }
    
    MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("Programming link state %s on the hardware.\n", linkUp ? "UP":"DOWN"));
    HalUpdateConnectionState(Hw->Hal, linkUp);

    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
HwUpdateAssociateId(
    _In_  PHW                     Hw,
    _In_  USHORT                  AssociateId
    )
{
    UCHAR                       i, numActive = 0;
    USHORT                      programmedAssociateId = 0;

    if (HW_TEST_ADAPTER_STATUS(Hw, HW_CANNOT_ACCESS_HARDWARE))
        return NDIS_STATUS_SUCCESS;
    
    // Walk the active MAC's BSSID & if there are multiple active,
    // program the default (0), else program the current associate id
    for (i = 0; i < HW_MAX_NUMBER_OF_MAC; i++)
    {
        if (HW_MAC_CONTEXT_MUST_MERGE(&Hw->MacContext[i]))
        {
            numActive++;
        }
    }

    if (numActive > 1)
    {
        // Multiple MACs active, 
        programmedAssociateId = 0;
    }
    else
    {
        // Single MAC active, program the active associate ID
        programmedAssociateId = AssociateId;
    }
    
    HalSetAssociateId(Hw->Hal, programmedAssociateId);
    return NDIS_STATUS_SUCCESS;
}


// Can be called at Device IRQL (or lower IRQL)
VOID
HwPopulateBeacon(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         BeaconMac,
    _In_  PHW_TX_MSDU             BeaconMsdu
    )
{
    PDOT11_MGMT_HEADER          beaconHeader;
    PDOT11_BEACON_FRAME         fixedIEs;
    PUCHAR                      buffer, ieDestination;
    ULONG                       bufferLength = 0;
    UCHAR                       i;

    UNREFERENCED_PARAMETER(Hw);
    
    buffer = BeaconMsdu->BufferVa;
    NdisZeroMemory(buffer, MAX_TX_RX_PACKET_SIZE);

    // Populate the known frame header fields
    beaconHeader = (PDOT11_MGMT_HEADER)buffer;

    beaconHeader->FrameControl.Type = DOT11_FRAME_TYPE_MANAGEMENT;
    beaconHeader->FrameControl.Subtype = DOT11_MGMT_SUBTYPE_BEACON;

    NdisFillMemory(beaconHeader->DA, DOT11_ADDRESS_SIZE, 0xFF);
    NdisMoveMemory(beaconHeader->SA, BeaconMac->MacAddress, DOT11_ADDRESS_SIZE);
    NdisMoveMemory(beaconHeader->BSSID, BeaconMac->DesiredBSSID, DOT11_ADDRESS_SIZE);

    beaconHeader->SequenceControl.usValue = 0;
	beaconHeader->SequenceControl.SequenceNumber = BeaconMac->SequenceNumber;
    MP_INCREMENT_LIMIT_UNSAFE(BeaconMac->SequenceNumber, 4096);
    
    bufferLength = sizeof(DOT11_MGMT_HEADER);

    // The fixed IEs
    fixedIEs = (PDOT11_BEACON_FRAME)(buffer + bufferLength);
    
    fixedIEs->BeaconInterval = (USHORT)BeaconMac->BeaconPeriod;
    fixedIEs->Capability.usValue = BeaconMac->DefaultPeer.CapabilityInfo;
    bufferLength += FIELD_OFFSET(DOT11_BEACON_FRAME, InfoElements);

    MPASSERT(BeaconMac->BeaconIEBlob != NULL);
    // Fill the IEs provided by the upper layer
    if (BeaconMac->BeaconIEBlob != NULL)
    {
        ieDestination = (buffer + bufferLength);
        NdisMoveMemory(ieDestination, BeaconMac->BeaconIEBlob, BeaconMac->BeaconIEBlobSize);
        bufferLength += BeaconMac->BeaconIEBlobSize;
    }

    // Total length
    BeaconMsdu->TotalMSDULength = bufferLength;

    // Beacon rate
    BeaconMsdu->TxRateTable[0] = BeaconMac->DefaultTXMgmtRate;
    for (i = 1; i < HW_TX_RATE_TABLE_SIZE; i++)
        BeaconMsdu->TxRateTable[i] = 0;

}

// Can be called at Device IRQL (or lower IRQL)
NDIS_STATUS
HwSetupBeacon(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         BeaconMac
    )
{
    PHW_TX_MSDU                 beaconMsdu;
    LONG                        beaconIndex = 0;

    // Use the beacon index for this
    beaconIndex = (Hw->MacState.ActiveBeaconIndex + 1) % HW_BEACON_QUEUE_BUFFER_COUNT;
    beaconMsdu = &Hw->TxInfo.TxQueue[HW11_BEACON_QUEUE].MSDUArray[beaconIndex]; 

    //MpTrace(COMP_TESTING, DBG_SERIOUS, ("BEACON: Attempting to send becon\n"));

    // Fill the beacon MSDU
    HwPopulateBeacon(Hw, BeaconMac, beaconMsdu);
    
    // Now submit this to the hardware
    HwTransmitBeacon(Hw, beaconMsdu);

    // Active beacon is
    Hw->MacState.ActiveBeaconIndex = beaconIndex;

    return NDIS_STATUS_SUCCESS;
}


// Will be called at Device IRQL
VOID
HwSetBeaconIE(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PVOID                   pBeaconIEBlob,
    _In_  ULONG                   uBeaconIEBlobSize
    )
{
    // Add we do is set this new value as the beacon IE
    HwMac->BeaconIEBlob = pBeaconIEBlob;
    HwMac->BeaconIEBlobSize = uBeaconIEBlobSize;      
}

VOID
HwEnableBSSBeacon(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         HwMac
    )
{
    Hw->MacState.ActiveBeaconIndex = -1; // So we would start with 0

    // This shouldnt happen, but we will protect against this
    if (HW_TEST_ADAPTER_STATUS(Hw, HW_CANNOT_ACCESS_HARDWARE))
        return;
    
    HwSetBeaconPeriod(HwMac->Hw, HwMac->BeaconPeriod);
    HwUpdateBSSID(HwMac->Hw, HwMac);
    HwUpdateAssociateId(HwMac->Hw, 0);
    
    //
    // Create the beacon content
    //
    HwSetupBeacon(Hw, HwMac);
    
    HwDisableInterrupt(Hw, HW_ISR_TRACKING_START_BSS);
    HalStartBSS(Hw->Hal, HwMac->LastBeaconTimestamp);
    HwEnableInterrupt(Hw, HW_ISR_TRACKING_START_BSS);

    Hw->MacState.BeaconEnabled = TRUE;
}

VOID
HwDisableBSSBeacon(
    _In_  PHW                     Hw
    )
{
    // Stop beaconing
    Hw->MacState.BeaconEnabled = FALSE;
    Hw->MacState.ActiveBeaconIndex = -1;  // So we would start with 0

    // This shouldnt happen, but we will protect against this
    if (!HW_TEST_ADAPTER_STATUS(Hw, HW_CANNOT_ACCESS_HARDWARE))
        HalStopBSS(Hw->Hal);
}

NDIS_STATUS
HwStartBSS(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 BeaconEnabled
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    
    //
    // Save the MAC that has AP mode enabled on it
    //
    Hw->MacState.BSSMac = HwMac;

    if (BeaconEnabled)
    {
        // Beaconing is enabled
        HwEnableBSSBeacon(Hw, HwMac);
    }    
    else
    {
        MpTrace(COMP_ASSOC, DBG_NORMAL, ("BSS started in non-beaconing mode\n"));
    }
    
    // AP started
    Hw->MacState.BSSStarted = TRUE;
    
    return ndisStatus;
}


VOID
HwStopBSS(
    _In_  PHW                     Hw
    )
{
    //
    // Stop beaconing
    //
    HwDisableBSSBeacon(Hw);

    //
    // Clear the BSS started state
    //
    Hw->MacState.BSSStarted = FALSE;
    Hw->MacState.BSSMac = NULL;
}


VOID
HwResumeBSS(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 BeaconEnabled
    )
{
    //
    // Resume the beaconing
    //
    Hw->MacState.BSSMac = HwMac;

    if (BeaconEnabled)
    {
        Hw->MacState.ActiveBeaconIndex = -1;

        HwSetBeaconPeriod(HwMac->Hw, HwMac->BeaconPeriod);
        HwUpdateBSSID(HwMac->Hw, HwMac);
        HwUpdateAssociateId(HwMac->Hw, 0);

        //
        // Create a beacon content
        //
        HwSetupBeacon(Hw, HwMac);
        
        HwDisableInterrupt(Hw, HW_ISR_TRACKING_RESUME_BSS);    
        HalResumeBSS(Hw->Hal);
        HwEnableInterrupt(Hw, HW_ISR_TRACKING_RESUME_BSS);

        Hw->MacState.BeaconEnabled = TRUE;
    }

    // AP has been started
    Hw->MacState.BSSStarted = TRUE;
}

VOID
HwPauseBSS(
    _In_  PHW                     Hw
    )
{
    if (Hw->MacState.BeaconEnabled)
    {
        //
        // Have the HAL pause beaconing. AT this time the HAL should also
        // save state it would need to resume beaconing later (eg. TSF)
        //
        HalPauseBSS(Hw->Hal);
    }
    
    //
    // Clear the AP running state
    //
    Hw->MacState.BeaconEnabled = FALSE;
    Hw->MacState.BSSMac = NULL;
    Hw->MacState.BSSStarted = FALSE;
    Hw->MacState.ActiveBeaconIndex = -1;
}

VOID
HwPrepareToJoinBSS(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         HwMac
    )
{
    HwSetBeaconPeriod(Hw, HwMac->BeaconPeriod);
    HwUpdateBSSID(Hw, HwMac);

    HalPrepareJoin(Hw->Hal);
}

NDIS_STATUS
HwProcessProbeRequest(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_  PHW_RX_MPDU             Mpdu
    )
{
    PMP_TX_MSDU                 probeResponse = NULL;
    PDOT11_MGMT_HEADER          responseHeader;
    PDOT11_BEACON_FRAME         fixedIEs;
    PDOT11_MGMT_HEADER          requestHeader;
    PUCHAR                      buffer, ieDestination;
    USHORT                      bufferLength = 0;

    // We are blindly sending a probe response on getting
    // a probe request
    requestHeader = (PDOT11_MGMT_HEADER)Mpdu->DataStart;
    
    probeResponse = HwAllocatePrivatePacket(Hw, MAX_TX_RX_PACKET_SIZE);
    if (probeResponse == NULL)
    {
        return NDIS_STATUS_SUCCESS;// We dont fail the call
    }
    
    buffer = MP_TX_MPDU_DATA(MP_TX_MSDU_MPDU_AT(probeResponse, 0), MAX_TX_RX_PACKET_SIZE);
    NdisZeroMemory(buffer, MAX_TX_RX_PACKET_SIZE);

    // Populate the known frame header fields
    responseHeader = (PDOT11_MGMT_HEADER)buffer;

    responseHeader->FrameControl.Type = DOT11_FRAME_TYPE_MANAGEMENT;
    responseHeader->FrameControl.Subtype = DOT11_MGMT_SUBTYPE_PROBE_RESPONSE;

    NdisMoveMemory(responseHeader->DA, requestHeader->SA, DOT11_ADDRESS_SIZE);
    NdisMoveMemory(responseHeader->SA, MacContext->MacAddress, DOT11_ADDRESS_SIZE);
    NdisMoveMemory(responseHeader->BSSID, MacContext->DesiredBSSID, DOT11_ADDRESS_SIZE);

    responseHeader->SequenceControl.usValue = 0;
    bufferLength = sizeof(DOT11_MGMT_HEADER);

    // The fixed IEs
    fixedIEs = (PDOT11_BEACON_FRAME)(buffer + bufferLength);
    
    fixedIEs->BeaconInterval = (USHORT)MacContext->BeaconPeriod;
    fixedIEs->Capability.usValue = MacContext->DefaultPeer.CapabilityInfo;
    bufferLength += FIELD_OFFSET(DOT11_BEACON_FRAME, InfoElements);

    HW_ACQUIRE_HARDWARE_LOCK(Hw, TRUE);
    
    // Fill the IEs provided by the upper layer
    if (MacContext->ProbeResponseIEBlob == NULL)
    {
        // Due to low resources, we may sometimes run into this. Stop processing & bail out
        HwFreePrivatePacket(Hw, probeResponse);
        HW_RELEASE_HARDWARE_LOCK(Hw, TRUE);    
        return NDIS_STATUS_RESOURCES;
    }
    
    ieDestination = (buffer + bufferLength);
    NdisMoveMemory(ieDestination, MacContext->ProbeResponseIEBlob, MacContext->ProbeResponseIEBlobSize);
    bufferLength = bufferLength + (USHORT)MacContext->ProbeResponseIEBlobSize;
    HW_RELEASE_HARDWARE_LOCK(Hw, TRUE);

    // Total length
    (MP_TX_MSDU_MPDU_AT(probeResponse, 0))->InternalSendLength = bufferLength;

    // Send the packet
    HwSendPrivatePackets(MacContext, probeResponse, NDIS_SEND_FLAGS_DISPATCH_LEVEL);

    return NDIS_STATUS_SUCCESS;
}


VOID 
HwFillRateElement(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_reads_bytes_(RemainingLength)  PUCHAR                  IEBuffer,
    _In_  ULONG                   RemainingLength,
    _Out_ PULONG                  IELength
    )
{
    UCHAR                       rate[256], opRate;
    UCHAR                       rateNum, suppRateNum, extSuppRateNum;
    UCHAR                       i, j;
    PDOT11_RATE_SET             basicRateSet, operatingRateSet;
    PDOT11_INFO_ELEMENT         infoElement;
    ULONG                       totalRateIELength = 0;

    UNREFERENCED_PARAMETER(RemainingLength);
    *IELength = 0;
    
    // Get the basic rates set from the hardware
    NdisZeroMemory(rate, 256);
    basicRateSet = &(HalGetPhyMIB(Hw->Hal, MacContext->OperatingPhyId)->BasicRateSet);
    for (i = 0; (i < basicRateSet->uRateSetLength) && (i < 256); i++)
    {
        rate[i] = 0x80 | basicRateSet->ucRateSet[i];
    }
    rateNum = i;
    _Analysis_assume_(rateNum <= 255);

    // Get the operating rate set from the MAC context
    operatingRateSet = &MacContext->PhyContext[MacContext->OperatingPhyId].OperationalRateSet;
    for (i = 0; i < operatingRateSet->uRateSetLength; i++)
    {
        opRate = operatingRateSet->ucRateSet[i];

        for (j = 0; j < basicRateSet->uRateSetLength; j++)
        {
            if (basicRateSet->ucRateSet[j] == opRate)
                break;
        }

        // if the operational rate is not in the basic rate, add it to the rate array.
        if (j == basicRateSet->uRateSetLength)
        {
            rate[rateNum] = opRate;
            rateNum++;

            if (rateNum == sizeof(rate) / sizeof(UCHAR))
                break;
        }
    }
    
    suppRateNum = rateNum > 8 ? 8 : rateNum;

    if (RemainingLength >= (sizeof(DOT11_INFO_ELEMENT) + suppRateNum))
    {
        infoElement = (PDOT11_INFO_ELEMENT)IEBuffer;
        infoElement->ElementID = DOT11_INFO_ELEMENT_ID_SUPPORTED_RATES;
        infoElement->Length = suppRateNum;

        IEBuffer += sizeof(DOT11_INFO_ELEMENT);
        
        NdisMoveMemory(IEBuffer, rate, suppRateNum);
        IEBuffer += suppRateNum;
        totalRateIELength += (sizeof(DOT11_INFO_ELEMENT) + suppRateNum);
        RemainingLength -= totalRateIELength;

        extSuppRateNum = rateNum > suppRateNum ? rateNum - suppRateNum : 0;
        if (extSuppRateNum && 
            (RemainingLength >= (sizeof(DOT11_INFO_ELEMENT) + extSuppRateNum)))
        {
            infoElement = (PDOT11_INFO_ELEMENT)IEBuffer;
            infoElement->ElementID = DOT11_INFO_ELEMENT_ID_EXTD_SUPPORTED_RATES;
            infoElement->Length = extSuppRateNum;

            IEBuffer += sizeof(DOT11_INFO_ELEMENT);
            NdisMoveMemory(IEBuffer, (rate + 8), extSuppRateNum);
            IEBuffer += extSuppRateNum;
            totalRateIELength += (sizeof(DOT11_INFO_ELEMENT) + extSuppRateNum);
        }
    }
    else
    {
        MpTrace(COMP_MISC, DBG_SERIOUS, ("Not enough space available to fill rate IE\n"));        
    }
    *IELength = totalRateIELength;
}

// Called: DISPATCH
NDIS_STATUS
HwSendProbeRequest(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_  PDOT11_MAC_ADDRESS      BSSID,
    _In_opt_  PDOT11_SSID             SSID,
    _In_reads_bytes_(ScanAppendIEByteArrayLength)  PUCHAR                  ScanAppendIEByteArray,
    _In_  USHORT                  ScanAppendIEByteArrayLength
    )
{
    PMP_TX_MSDU                 probeRequest = NULL;
    PDOT11_MGMT_HEADER          requestHeader;
    PUCHAR                      dataStart, buffer;
    ULONG                       bufferLength = 0, ieLength;
    PDOT11_INFO_ELEMENT         infoElement;

    probeRequest = HwAllocatePrivatePacket(Hw, MAX_TX_RX_PACKET_SIZE);
    if (probeRequest == NULL)
    {
        return NDIS_STATUS_RESOURCES;// We dont fail the call
    }
    
    dataStart = MP_TX_MPDU_DATA(MP_TX_MSDU_MPDU_AT(probeRequest, 0), MAX_TX_RX_PACKET_SIZE);
    NdisZeroMemory(dataStart, MAX_TX_RX_PACKET_SIZE);
    buffer = dataStart;
    
    // Populate the known frame header fields
    requestHeader = (PDOT11_MGMT_HEADER)buffer;

    requestHeader->FrameControl.Type = DOT11_FRAME_TYPE_MANAGEMENT;
    requestHeader->FrameControl.Subtype = DOT11_MGMT_SUBTYPE_PROBE_REQUEST;

    NdisFillMemory(requestHeader->DA, DOT11_ADDRESS_SIZE, (UCHAR)0xff);
    NdisMoveMemory(requestHeader->SA, MacContext->MacAddress, DOT11_ADDRESS_SIZE);
    NdisMoveMemory(requestHeader->BSSID, BSSID, DOT11_ADDRESS_SIZE);

    requestHeader->SequenceControl.usValue = 0;
    
    bufferLength = sizeof(DOT11_MGMT_HEADER);
    buffer = dataStart + bufferLength;

    infoElement = (PDOT11_INFO_ELEMENT)buffer;
    // Fill the SSID
    infoElement->ElementID = DOT11_INFO_ELEMENT_ID_SSID;
    if (SSID != NULL)
    {
        infoElement->Length = (UCHAR)SSID->uSSIDLength;
        bufferLength += sizeof(DOT11_INFO_ELEMENT);
        buffer += sizeof(DOT11_INFO_ELEMENT);
        
        if (SSID->uSSIDLength > 0)
        {
            NdisMoveMemory(buffer, SSID->ucSSID, SSID->uSSIDLength);
            bufferLength += SSID->uSSIDLength;
            buffer += SSID->uSSIDLength;
        }
    }
    else
    {
        // Wildcard SSID
        infoElement->Length = 0;
        bufferLength += sizeof(DOT11_INFO_ELEMENT);
        buffer += sizeof(DOT11_INFO_ELEMENT);        
    }

    // Fill the rates
    HwFillRateElement(Hw, MacContext, buffer, MAX_TX_RX_PACKET_SIZE - bufferLength, &ieLength);
    bufferLength += ieLength;
    buffer = dataStart + bufferLength;

    // Fill the IEs provided by the upper layer
    if (ScanAppendIEByteArrayLength > 0) 
    {
        if ((MAX_TX_RX_PACKET_SIZE - bufferLength) > ScanAppendIEByteArrayLength)
        {
            //
            // Append IEs
            //
            NdisMoveMemory(buffer, ScanAppendIEByteArray, ScanAppendIEByteArrayLength);
            bufferLength += ScanAppendIEByteArrayLength;

            // If anything is below, its start would be offset
            buffer = dataStart + bufferLength;
        }
    }
    
    // Total length
    (MP_TX_MSDU_MPDU_AT(probeRequest, 0))->InternalSendLength = (USHORT)bufferLength;

    // Send the packet
    HwSendPrivatePackets(MacContext, probeRequest, NDIS_SEND_FLAGS_DISPATCH_LEVEL);

    return NDIS_STATUS_SUCCESS;
}


NDIS_STATUS
HwStartScan(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PMP_SCAN_REQUEST        ScanRequest
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_PHY_TYPE_INFO        dot11PhyTypeInfo;
    BOOLEAN                     useDefaultParameters = FALSE;
    PDOT11_SCAN_REQUEST_V2      dot11ScanRequest = ScanRequest->Dot11ScanRequest;
    ULONG                       scanChannel;
    LARGE_INTEGER               startTime;
    
    do
    {
        if (dot11ScanRequest->bRestrictedScan == FALSE)
        {
            if (dot11ScanRequest->uNumOfPhyTypeInfos > 0)
            {
                //
                // Perform some validation on the scan request that is hw specific
                //
                dot11PhyTypeInfo = (PDOT11_PHY_TYPE_INFO) 
                    (dot11ScanRequest->ucBuffer + dot11ScanRequest->uPhyTypeInfosOffset);

                //
                // Copy some of the information needed to perform the scan
                //        
                if (dot11PhyTypeInfo->bUseParameters)
                {
                    // This is already validated by MP portion. Just checking
                    MPASSERT(dot11PhyTypeInfo->ChDescriptionType != ch_description_type_phy_specific);
                    
                    //
                    // We will be using parameters as defined by the caller
                    // 
                    Hw->ScanContext.ProbeDelay = dot11PhyTypeInfo->uProbeDelay;
                    Hw->ScanContext.ChannelTime = dot11PhyTypeInfo->uMinChannelTime + 
                        ((dot11PhyTypeInfo->uMaxChannelTime - dot11PhyTypeInfo->uMinChannelTime) / 2);

                    //
                    // Get the channel list specified by the caller
                    //
                    Hw->ScanContext.ChannelCount = dot11PhyTypeInfo->uChannelListSize / 4;
                    MP_ALLOCATE_MEMORY(
                        Hw->MiniportAdapterHandle, 
                        &Hw->ScanContext.ChannelList,
                        dot11PhyTypeInfo->uChannelListSize,
                        HW_MEMORY_TAG
                        );
                    if (Hw->ScanContext.ChannelList == NULL)
                    {
                        ndisStatus = NDIS_STATUS_RESOURCES;
                        break;
                    }

                    if (dot11PhyTypeInfo->ChDescriptionType == ch_description_type_logical)
                    {
                        //
                        // Channels are already logical numbers. Just copy them
                        //
                        NdisMoveMemory(
                            Hw->ScanContext.ChannelList,
                            dot11PhyTypeInfo->ucChannelListBuffer,
                            dot11PhyTypeInfo->uChannelListSize
                            );
                    }
                    else
                    {
                        //
                        // We have channel frequencies. Convert to logical numbers.
                        //
                        ndisStatus = HwTranslateChannelFreqToLogicalID(
                                        (PULONG) dot11PhyTypeInfo->ucChannelListBuffer,
                                        dot11PhyTypeInfo->uChannelListSize/4,
                                        Hw->ScanContext.ChannelList
                                        );

                        if (ndisStatus != NDIS_STATUS_SUCCESS)
                            break;
                    }
                }
                else
                {
                    //
                    // Use default values for this phy
                    //
                    useDefaultParameters = TRUE;
                }
            }
            else
            {
                //
                // There are no Phy Type Info object. Use default values
                //
                useDefaultParameters = TRUE;
            }
            
            if (useDefaultParameters == TRUE)
            {
                //
                // We will be using our own defaults
                //
                Hw->ScanContext.ProbeDelay = HW_DEFAULT_PROBE_DELAY;
                if ((dot11ScanRequest->dot11ScanType == dot11_scan_type_passive) ||
                    (dot11ScanRequest->dot11ScanType == (dot11_scan_type_passive | dot11_scan_type_forced)))
                {
                    // Park longer to passive scans.
                    Hw->ScanContext.ChannelTime = HW_DEFAULT_PASSIVE_SCAN_CHANNEL_PARK_TIME;
                }
                else
                {
                    Hw->ScanContext.ChannelTime = HW_DEFAULT_ACTIVE_SCAN_CHANNEL_PARK_TIME;
                }

                if (ScanRequest->ChannelCount == 0)
                {
                    Hw->ScanContext.ChannelCount = 0;
                    Hw->ScanContext.ScanPhyId = ScanRequest->PhyId;
                    
                    ndisStatus = HalGetChannelList(Hw->Hal, 
                        ScanRequest->PhyId, 
                        &Hw->ScanContext.ChannelCount, 
                        NULL
                        );

                    if (ndisStatus != NDIS_STATUS_BUFFER_TOO_SHORT)
                    {
                        break;
                    }
            		
                    MP_ALLOCATE_MEMORY(
                        Hw->MiniportAdapterHandle, 
                        &Hw->ScanContext.ChannelList,
                        Hw->ScanContext.ChannelCount * sizeof(ULONG),
                        HW_MEMORY_TAG
                        );
                    if (Hw->ScanContext.ChannelList == NULL)
                    {
                        ndisStatus = NDIS_STATUS_RESOURCES;
                        break;
                    }

                    ndisStatus = HalGetChannelList(Hw->Hal, 
                        ScanRequest->PhyId, 
                        &Hw->ScanContext.ChannelCount, 
                        Hw->ScanContext.ChannelList
                        );

                    if (ndisStatus != NDIS_STATUS_SUCCESS)
                    {
                        break;
                    }
                }
                else
                {
                    //
                    // Use caller provided channel list
                    //
                    Hw->ScanContext.ChannelCount = ScanRequest->ChannelCount;
                    Hw->ScanContext.ScanPhyId = ScanRequest->PhyId;
                    
                    MP_ALLOCATE_MEMORY(
                        Hw->MiniportAdapterHandle, 
                        &Hw->ScanContext.ChannelList,
                        Hw->ScanContext.ChannelCount * sizeof(ULONG),
                        HW_MEMORY_TAG
                        );
                    if (Hw->ScanContext.ChannelList == NULL)
                    {
                        ndisStatus = NDIS_STATUS_RESOURCES;
                        break;
                    }

                    NdisMoveMemory(
                        Hw->ScanContext.ChannelList,
                        ScanRequest->ChannelList,
                        Hw->ScanContext.ChannelCount * sizeof(ULONG)
                        );
                }
            }
        }
        else
        {
            //
            // Normal extensible station driver by itself does not need to support restricted scan. However,
            // our station portion can use this for internal scan request
            //
            
            //
            // We are guaranteed to be in OP State as Mp portion verified it.
            //
            Hw->ScanContext.ProbeDelay = HW_DEFAULT_PROBE_DELAY;
            if ((dot11ScanRequest->dot11ScanType == dot11_scan_type_passive) ||
                (dot11ScanRequest->dot11ScanType == (dot11_scan_type_passive | dot11_scan_type_forced)))
            {
                // Park longer to passive scans.
                Hw->ScanContext.ChannelTime = HW_DEFAULT_PASSIVE_SCAN_CHANNEL_PARK_TIME;
            }
            else
            {
                Hw->ScanContext.ChannelTime = HW_DEFAULT_ACTIVE_SCAN_CHANNEL_PARK_TIME;
            }

            //
            // We will be scanning only the channel we are active on currently
            //
            Hw->ScanContext.ChannelCount = 1;
            
            MP_ALLOCATE_MEMORY(
                Hw->MiniportAdapterHandle,
                &Hw->ScanContext.ChannelList,
                sizeof(ULONG),
                HW_MEMORY_TAG
                );
            if (Hw->ScanContext.ChannelList == NULL)
            {
                ndisStatus = NDIS_STATUS_RESOURCES;
                break;
            }

            // Use the current active channel
            scanChannel = (ULONG) HalGetOperatingPhyMIB(Hw->Hal)->Channel;
            Hw->ScanContext.ScanPhyId = Hw->PhyState.OperatingPhyId;

            NdisMoveMemory(
                Hw->ScanContext.ChannelList,
                &scanChannel,
                sizeof(ULONG)
                );
        }
        
        //
        // Save the scan request structure that the caller passed
        //
        Hw->ScanContext.ScanRequest = ScanRequest;
        Hw->ScanContext.MacContext = HwMac;
        
        //
        // Reset scan state
        //
        Hw->ScanContext.CancelOperation = FALSE;
        Hw->ScanContext.CompleteIndicated = FALSE;
        Hw->ScanContext.ScanInProgress = TRUE;
        Hw->ScanContext.ChannelSwitchTime = 0xffffffffffffffff;
        Hw->ScanContext.CurrentChannelIndex = 0;
        Hw->ScanContext.CurrentStep = ScanStepSwitchChannel;

        Hw->ScanContext.PreScanPhyId = HwMac->OperatingPhyId;
        Hw->ScanContext.PreScanChannel = HwMac->PhyContext[HwMac->OperatingPhyId].Channel;

        //
        // Wakeup the NIC if it is sleeping
        //
        if (HwAwake(Hw, FALSE) == FALSE)
        {
            // Radio is OFF
            ndisStatus = NDIS_STATUS_DOT11_POWER_STATE_INVALID;
            Hw->ScanContext.ScanInProgress = FALSE;
            Hw->ScanContext.CompleteIndicated = TRUE;
            break;
        }

    	HalStartScan(Hw->Hal);

        // Add a ref for the async function and set the timer        
        HW_INCREMENT_ACTIVE_OPERATION_REF(Hw);

        startTime.QuadPart = -1;        // Run now        
        NdisSetTimerObject(Hw->ScanContext.Timer_Scan, startTime, 0, NULL);
    } while(FALSE);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        if (Hw->ScanContext.ChannelList != NULL)
            MP_FREE_MEMORY(Hw->ScanContext.ChannelList);

        //
        // In case of failure, caller free the structure
        //
        Hw->ScanContext.ScanRequest = NULL;
    }
    
    return ndisStatus;

}


NDIS_STATUS 
HwCompleteScan(
    _In_  PHW                     Hw,
    _In_opt_  PHW_MAC_CONTEXT         MacContext,
    _In_  PVOID                   Data
    )
{
    UNREFERENCED_PARAMETER(MacContext);
    UNREFERENCED_PARAMETER(Data);

    MpTrace(COMP_SCAN, DBG_LOUD, ("Scan operation completed\n"));

    //
    // Free the memory allocated for this operation
    //
    MP_FREE_MEMORY(Hw->ScanContext.ChannelList);

    Hw->ScanContext.ChannelList = NULL;
    Hw->ScanContext.ScanRequest = NULL;
    
    //
    // Indicate scan completion to the OS
    //
    if (Hw->ScanContext.CancelOperation)
        HwScanComplete(Hw->ScanContext.MacContext, NDIS_STATUS_REQUEST_ABORTED);
    else
        HwScanComplete(Hw->ScanContext.MacContext, NDIS_STATUS_SUCCESS);

    //
    // reset the scan state variables (after we have completed the scan)
    //
    Hw->ScanContext.ScanInProgress = FALSE;
    Hw->ScanContext.CompleteIndicated = TRUE;

    //restore Hw related parameter 
    HalStopScan(Hw->Hal);
	
    //
    // If we are in power saving mode and we are not associated, turn off the RF.
    // If we are associated, RF will be turned off by HwResponseToPacket.
    //
    if (Hw->MacState.PowerMgmtMode.dot11PowerMode == dot11_power_mode_powersave &&
        Hw->MacState.PowerMgmtMode.usAID == 0)
    {
        MpTrace(COMP_POWER, DBG_LOUD, (" *** RF OFF indefinitely\n"));
        HwSetRFState(Hw, RF_OFF);
    }

    // Remove the Async function ref
    HW_DECREMENT_ACTIVE_OPERATION_REF(Hw);

    return NDIS_STATUS_SUCCESS;
}

VOID
HwCancelScan(
    _In_  PHW                     Hw
    )
{
    Hw->ScanContext.CancelOperation = TRUE;

    // Wait for scan operation to be completed.
    while(Hw->ScanContext.CompleteIndicated != TRUE)   
    {
        MpTrace(COMP_SCAN, DBG_NORMAL, ("Waiting for scan operation to complete\n"));
        NdisMSleep(20 * 1000);
    }
}

NDIS_STATUS 
HwScanChannelSwitchComplete(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_  PVOID                   Data
    )
{
    LARGE_INTEGER               scanProbeDelay;
    
    UNREFERENCED_PARAMETER(Data);

    MpTrace(COMP_SCAN, DBG_LOUD, ("Scan channel switched\n"));

    //
    // The current scan state should be switch channel
    //
    MPASSERT(Hw->ScanContext.CurrentStep == ScanStepSwitchChannel);

    //
    // ScanCallback has requested a channel switch. That operation has
    // been completed. We need to:
    // 1. Transition the scan state from Switch to Scan
    // 2. Schedule the ScanCallback DPC asap to do the scan.
    //
    HW_TRANSITION_SCAN_STEP(Hw->ScanContext.CurrentStep);

    Hw->ScanContext.ProbeTXRate = HalGetBeaconRate(Hw->Hal, MacContext->OperatingPhyId);

    scanProbeDelay.QuadPart = Hw->ScanContext.ProbeDelay;
    scanProbeDelay.QuadPart = scanProbeDelay.QuadPart * -1 * 10000;

    // Appropriately delay the sending of the probe request
    NdisSetTimerObject(Hw->ScanContext.Timer_Scan, scanProbeDelay, 0, NULL);

    return NDIS_STATUS_SUCCESS;
}



VOID
HwScanTimer(
    PVOID                   SystemSpecific1,
    PVOID                   FunctionContext,
    PVOID                   SystemSpecific2,
    PVOID                   SystemSpecific3
    )
{
    PHW                         hw = (PHW)FunctionContext;
    LARGE_INTEGER               rescheduleTime;
    NDIS_STATUS                 ndisStatus;
    PHW_MAC_CONTEXT             scanningMac = hw->ScanContext.MacContext;

    UNREFERENCED_PARAMETER(SystemSpecific1);
    UNREFERENCED_PARAMETER(SystemSpecific2);
    UNREFERENCED_PARAMETER(SystemSpecific3);

    while (TRUE) 
    {
        if (hw->ScanContext.CurrentChannelIndex == hw->ScanContext.ChannelCount) 
        {
            //
            // Last channel has been scanned. Revert to the channel we had before scan started
            //
            MpTrace(COMP_SCAN, DBG_LOUD, ("Done with the last channel. Revert back to original channel\n"));

            if ((hw->PhyState.OperatingPhyId == hw->ScanContext.PreScanPhyId) && 
                (scanningMac->PhyContext[hw->ScanContext.PreScanPhyId].Channel == hw->ScanContext.PreScanChannel))
            {
                MpTrace(COMP_SCAN, DBG_LOUD, ("Already on target channel! Switch not required\n"));
                ndisStatus = NDIS_STATUS_SUCCESS;
            }
            else 
            {
                // Restore the channel
                scanningMac->PhyContext[hw->ScanContext.PreScanPhyId].Channel 
                    = hw->ScanContext.PreScanChannel;
                scanningMac->OperatingPhyId = hw->ScanContext.PreScanPhyId;
                
                ndisStatus = HwProgramPhy(hw, 
                                    scanningMac, 
                                    hw->ScanContext.PreScanPhyId,
                                    &scanningMac->PhyContext[hw->ScanContext.PreScanPhyId],
                                    HwCompleteScan
                                    );
            }
            
            if (ndisStatus != NDIS_STATUS_PENDING)
            {
                // Call the complete handler
                HwCompleteScan(hw, NULL, &ndisStatus);
            }
            
            //
            // Don't reschedule the DPC. We are done.
            //
            return;
        }
        else if (hw->ScanContext.CancelOperation)
        {
            //
            // Scan operation has been cancelled
            //
            MpTrace(COMP_SCAN, DBG_LOUD, ("Scan operation has been cancelled\n"));
            
            //
            // Set the termination condition to true. This will cancel scan automatically
            //
            hw->ScanContext.CurrentChannelIndex = hw->ScanContext.ChannelCount;

            continue;
        }
        else {
            if (hw->ScanContext.CurrentStep == ScanStepSwitchChannel)
            {
                ULONG       destChannel;
                
                //
                // Switch to the next channel. CurrentChannelIndex point to it.
                //                
                destChannel = hw->ScanContext.ChannelList[hw->ScanContext.CurrentChannelIndex];
                hw->ScanContext.CurrentChannel = destChannel;
                
                MpTrace(COMP_SCAN, DBG_LOUD, ("Switching to Channel %d\n", destChannel));
                scanningMac->PhyContext[hw->ScanContext.ScanPhyId].Channel 
                    = (UCHAR)destChannel;
                scanningMac->OperatingPhyId = hw->ScanContext.ScanPhyId;
                
                ndisStatus = HwProgramPhy(hw, 
                                    scanningMac, 
                                    scanningMac->OperatingPhyId,
                                    &scanningMac->PhyContext[scanningMac->OperatingPhyId],
                                    HwScanChannelSwitchComplete
                                    );
                                    
                if (ndisStatus != NDIS_STATUS_PENDING)
                {
                    // Call the complete handler
                    HwScanChannelSwitchComplete(hw, scanningMac, &ndisStatus);
                }

                //
                // Don't rescedule the DPC. We will do that when the channel switch completes
                //
                return;
            }
            else
            {
                // TODO: Remove support for PARTIAL_HAL
#ifndef PARTIAL_HAL
                ULONG                   i, bytesParsed = 0;
                PDOT11_SSID             dot11SSID;
                PDOT11_SCAN_REQUEST_V2  dot11ScanRequest = hw->ScanContext.ScanRequest->Dot11ScanRequest;
                
                //
                // Scan the current channel
                //
                if ((dot11ScanRequest->dot11ScanType == dot11_scan_type_active) ||
                    (dot11ScanRequest->dot11ScanType == (dot11_scan_type_active | dot11_scan_type_forced)) ||
                    (dot11ScanRequest->dot11ScanType == dot11_scan_type_auto) ||
                    (dot11ScanRequest->dot11ScanType == (dot11_scan_type_auto | dot11_scan_type_forced)))
                {
                    //
                    // For active scan send out probes. Auto is active for us.
                    // We will send out a probe request for each SSID in the request
                    //
                    if (dot11ScanRequest->uNumOfdot11SSIDs != 0)
                    {
                        //
                        // SSIDs have been specified in the scan request. Send probe requests for
                        // those
                        //
                        for (i = 0; i < dot11ScanRequest->uNumOfdot11SSIDs; i++) 
                        {
                            dot11SSID = (PDOT11_SSID) (dot11ScanRequest->ucBuffer + 
                                                        dot11ScanRequest->udot11SSIDsOffset + 
                                                        bytesParsed);

                            HwSendProbeRequest(hw,
                                scanningMac,
                                &dot11ScanRequest->dot11BSSID,
                                dot11SSID,
                                dot11ScanRequest->ucBuffer + dot11ScanRequest->uIEsOffset,
                                (USHORT) dot11ScanRequest->uIEsLength
                                );
                            
                            bytesParsed += sizeof(DOT11_SSID);
                        }
                    }
                    else
                    {
                        HwSendProbeRequest(hw,
                            scanningMac,
                            &dot11ScanRequest->dot11BSSID,
                            NULL,
                            dot11ScanRequest->ucBuffer + dot11ScanRequest->uIEsOffset,
                            (USHORT) dot11ScanRequest->uIEsLength
                            );

                    }
                }
                else 
                {
                    //
                    // If passive scanning, we will park longer. Beacons and probes
                    // are indicated up anyways. We need not do anything.
                    //
                }
#endif
                //
                // Move to the next Scan state.
                //
                HW_TRANSITION_SCAN_STEP(hw->ScanContext.CurrentStep);

                //
                // If we are going to switch to the next channel, 
                // move to the desired channel
                //
                if (hw->ScanContext.CurrentStep == ScanStepSwitchChannel) 
                {
                    hw->ScanContext.CurrentChannelIndex++;
                }

                rescheduleTime.QuadPart = hw->ScanContext.ChannelTime;
            }
        }

        //
        // Always break out. We will loop only if cancel is received.
        //
        break;
    }

    //
    // Reschedule the DPC
    //
    rescheduleTime.QuadPart = rescheduleTime.QuadPart * -1 * 10000;
    NdisSetTimerObject(hw->ScanContext.Timer_Scan, rescheduleTime, 0, NULL);
}

