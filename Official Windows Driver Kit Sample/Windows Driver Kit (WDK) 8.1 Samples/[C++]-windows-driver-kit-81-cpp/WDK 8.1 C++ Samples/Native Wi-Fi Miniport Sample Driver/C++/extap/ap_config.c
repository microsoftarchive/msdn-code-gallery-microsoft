/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    ap_config.c

Abstract:
    Implements the ExtAP configuration management
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-21-2007    Created

Notes:

--*/
#include "precomp.h"
    
#if DOT11_TRACE_ENABLED
#include "ap_config.tmh"
#endif

// define tracing components
#define COMP_AP_CONFIG

/** Initialize AP configurations */
NDIS_STATUS
ApInitializeConfig(
    _In_ PAP_CONFIG ApConfig,
    _In_ PMP_EXTAP_PORT ApPort
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    
    MPASSERT(ApConfig != NULL);
    MPASSERT(ApPort != NULL);

    do
    {
        ApConfig->ApPort = ApPort;

        CfgSetDefault(ApConfig);
        
        // TODO: anything needs to be done here?
    } while (FALSE);
    
    return ndisStatus;
}

/** Deinitialize AP configurations */
VOID
ApDeinitializeConfig(
    _In_ PAP_CONFIG ApConfig
    )
{
    MPASSERT(ApConfig != NULL);

    // 
    // clean up first
    //
    CfgCleanup(ApConfig);
    
    // TODO: anything needs to be done here

}



/** Internal functions that are invoked by other configuration functions */
/** 
 * Set AP configuration to its default based on the hardware capability 
 * and registry settings
 */
VOID
CfgSetDefault(
    _In_ PAP_CONFIG ApConfig
    )
{
    ULONG i;
    PAP_CAPABIITY capability = NULL;
    PAP_REG_INFO regInfo = NULL;
    
    MPASSERT(ApConfig->ApPort != NULL);

    capability = AP_GET_CAPABILITY(ApConfig->ApPort);
    regInfo = AP_GET_REG_INFO(ApConfig->ApPort);

    // TODO: add capability logic
    
    ApConfig->AutoConfigEnabled = AP_DEFAULT_ENABLED_AUTO_CONFIG;

    ApConfig->BeaconPeriod = regInfo->BeaconPeriod;

    ApConfig->DTimPeriod = regInfo->DTimPeriod;

#if 0
    ApConfig->RtsThreshold = regInfo->RtsThreshold;

    ApConfig->ShortRetryLimit = regInfo->ShortRetryLimit;

    ApConfig->LongRetryLimit = regInfo->LongRetryLimit;

    ApConfig->FragmentationThreshold = regInfo->FragmentationThreshold;

    ApConfig->CurrentChannel = regInfo->DefaultChannel;

    ApConfig->CurrentFrequency = regInfo->DefaultChannel;

    ApConfig->CurrentPhyId = AP_DEFAULT_PHY_ID;

#endif

    ApConfig->DesiredPhyCount = AP_DEFAULT_DESIRED_PHY_ID_COUNT;

    // 
    // init desired PHY IDs 
    //
    for (i = 0; i < ApConfig->DesiredPhyCount; i++)
    {
        ApConfig->DesiredPhyList[i] = AP_DEFAULT_DESIRED_PHY_ID;
    }

    // 
    // additional IEs for beacon and probe response 
    //
    ApConfig->AdditionalBeaconIESize = ApConfig->AdditionalResponseIESize = AP_DEFAULT_ADDITIONAL_IE_SIZE;

    ApConfig->AdditionalBeaconIEData = ApConfig->AdditionalResponseIEData = AP_DEFAULT_ADDITIONAL_IE_DATA;

}

/** Clean up AP configuration */
VOID
CfgCleanup(
    _In_ PAP_CONFIG ApConfig
    )
{
    PMP_EXTAP_PORT apPort = NULL;

    // 
    // Reset Additional IE Data 
    //
    CfgResetAdditionalIe(ApConfig);

    // 
    // remember AP port
    //
    apPort = ApConfig->ApPort;
    
    // 
    // clear everything
    //
    NdisZeroMemory(ApConfig, sizeof(AP_CONFIG));

    // 
    // reset AP port
    //
    ApConfig->ApPort = apPort;
}

/**
 * Internal functions for OIDs
 */

VOID
CfgQueryAutoConfigEnabled(
    _In_ PAP_CONFIG Config,
    _Out_ PULONG AutoConfigEnabled
    )
{
    //
    // return the one saved in base port
    //
    *AutoConfigEnabled = AP_GET_MP_PORT(Config->ApPort)->AutoConfigEnabled;
}

NDIS_STATUS
CfgSetAutoConfigEnabled(
    _In_ PAP_CONFIG Config,
    _In_ ULONG AutoConfigEnabled
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    //
    // Set it in the base port
    //
    AP_GET_MP_PORT(Config->ApPort)->AutoConfigEnabled = AutoConfigEnabled;

    //
    // Save a copy for AP
    //
    Config->AutoConfigEnabled = AutoConfigEnabled & ALLOWED_AUTO_CONFIG_FLAGS;

    return ndisStatus;
}
  
NDIS_STATUS
CfgSetBeaconPeriod(
    _In_ PAP_CONFIG Config,
    _In_range_(1, 65535) ULONG BeaconPeriod
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // 
        // validate beacon period
        //
        if (BeaconPeriod < 1 || BeaconPeriod > 65535)
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        // 
        // call VNic to set it first
        //
        ndisStatus = VNic11SetBeaconPeriod(
                        AP_GET_VNIC(Config->ApPort),
                        BeaconPeriod
                        );

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }
        
        Config->BeaconPeriod = BeaconPeriod;

    } while (FALSE);
    
    return ndisStatus;
}
    
NDIS_STATUS
CfgSetDTimPeriod(
    _In_ PAP_CONFIG Config,
    _In_range_(1, 255) ULONG DTimPeriod
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    // TODO: call VNIC to set it

    Config->DTimPeriod = DTimPeriod;

    return ndisStatus;
}

#if 0
NDIS_STATUS
CfgSetRtsThreshold(
    _In_ PAP_CONFIG Config,
    _In_range_(0, 2347) ULONG RtsThreshold
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    // TODO: call VNIC to set it

    Config->RtsThreshold = RtsThreshold;

    return ndisStatus;
}

NDIS_STATUS
CfgSetShortRetryLimit(
    _In_ PAP_CONFIG Config,
    _In_range_(1, 255) ULONG ShortRetryLimit
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    // TODO: call VNIC to set it

    Config->ShortRetryLimit = ShortRetryLimit;

    return ndisStatus;
}

NDIS_STATUS
CfgSetLongRetryLimit(
    _In_ PAP_CONFIG Config,
    _In_range_(1, 255) ULONG LongRetryLimit
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    // TODO: call VNIC to set it

    Config->LongRetryLimit = LongRetryLimit;

    return ndisStatus;
}

NDIS_STATUS
CfgSetFragmentationThreshold(
    _In_ PAP_CONFIG Config,
    _In_range_(256, 2346) ULONG FragmentationThreshold
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    // TODO: call VNIC to set it

    Config->FragmentationThreshold = FragmentationThreshold;

    return ndisStatus;
}
#endif

VOID
CfgQueryCurrentChannel(
    _In_ PAP_CONFIG Config,
    _Out_ PULONG CurrentChannel
    )
{
    *CurrentChannel = VNic11QueryCurrentChannel(
                        AP_GET_VNIC(Config->ApPort), 
                        TRUE                // query the current channel for the selected PHY
                        );
}

/**
 * Callback function for setting current channel/frequency.
 * Need to complete pending OID.
 *
 * \param Data NDIS_STATUS *
 */
NDIS_STATUS
ApChannelSwitchCompletionHandler(
    _In_ PMP_PORT Port,
    _In_ PVOID Data
    )
{
    NDIS_STATUS ndisStatus = *(NDIS_STATUS *)Data;

    Port11CompletePendingOidRequest(Port, ndisStatus);

    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
CfgSetCurrentChannel(
    _In_ PAP_CONFIG Config,
    _In_range_(1, 14) ULONG CurrentChannel
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // 
        // If autoconfig is enabled, we dont allow this to be set
        //
        if (AP_GET_MP_PORT(Config->ApPort)->AutoConfigEnabled & DOT11_PHY_AUTO_CONFIG_ENABLED_FLAG)
        {
            ndisStatus = NDIS_STATUS_DOT11_AUTO_CONFIG_ENABLED;
            break;
        }

        //
        // validate channel
        // TODO: channel shall be in the available channel list
        //
        if (CurrentChannel < 1 || CurrentChannel > 14)
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        ASSERT(ApGetState(Config->ApPort) != AP_STATE_STARTED);
        
        // 
        // call VNIC to set it
        //

        ndisStatus = VNic11SetChannel(
                        AP_GET_VNIC(Config->ApPort), 
                        CurrentChannel, 
                        VNic11QuerySelectedPhyId(AP_GET_VNIC(Config->ApPort)), 
                        FALSE,
                        ApChannelSwitchCompletionHandler
                        );
    } while (FALSE);

    return ndisStatus;
}

VOID
CfgQueryCurrentFrequency(
    _In_ PAP_CONFIG Config,
    _Out_ PULONG CurrentFrequency
    )
{
    // all hardware calls work with channel
    *CurrentFrequency = VNic11QueryCurrentChannel(
                        AP_GET_VNIC(Config->ApPort), 
                        TRUE                // query the current frequency for the selected PHY
                        );

}

NDIS_STATUS
CfgSetCurrentFrequency(
    _In_ PAP_CONFIG Config,
    _In_range_(0, 200) ULONG CurrentFrequency
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // 
        // If autoconfig is enabled, we dont allow this to be set
        //
        if (AP_GET_MP_PORT(Config->ApPort)->AutoConfigEnabled & DOT11_PHY_AUTO_CONFIG_ENABLED_FLAG)
        {
            ndisStatus = NDIS_STATUS_DOT11_AUTO_CONFIG_ENABLED;
            break;
        }

        //
        // validate frequency
        // TODO: frequency shall be in available frequency list
        //
        if (CurrentFrequency > 200)
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }
        
        ASSERT(ApGetState(Config->ApPort) != AP_STATE_STARTED);
        
        // 
        // call VNIC to set it (everything converts to channel on the H/W)
        //

        ndisStatus = VNic11SetChannel(
                        AP_GET_VNIC(Config->ApPort), 
                        CurrentFrequency, 
                        VNic11QuerySelectedPhyId(AP_GET_VNIC(Config->ApPort)), 
                        FALSE,
                        ApChannelSwitchCompletionHandler
                        );
    } while (FALSE);

    return ndisStatus;
}

NDIS_STATUS
CfgSetCipherDefaultKeyId(
    _In_ PAP_CONFIG Config,
    _In_ ULONG CipherDefaultKeyId
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        ndisStatus =
        VNic11SetDefaultKeyId
        (
            AP_GET_VNIC(Config->ApPort), 
            CipherDefaultKeyId
        );
        if (NDIS_STATUS_SUCCESS != ndisStatus)
        {
            break;
        }

        Config->CipherDefaultKeyId = CipherDefaultKeyId;
    }
    while (FALSE);

    return ndisStatus;
}

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
CfgQueryDesiredPhyList(
    _In_ PAP_CONFIG Config,
    _Out_ PDOT11_PHY_ID_LIST DesiredPhyList,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_when_invalid_ndis_length_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    ULONG requiredSize = 0; 
    
    do
    {
        if (!GetRequiredListSize(
                FIELD_OFFSET(DOT11_PHY_ID_LIST, dot11PhyId), 
                sizeof(ULONG), 
                Config->DesiredPhyCount, 
                &requiredSize
                ))
        {
            // 
            // this shold not happen
            //
            MPASSERT(FALSE);
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        if (InformationBufferLength < requiredSize)
        {
            // 
            // the buffer is not big enough
            //
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(requiredSize);
        }

        // 
        // the buffer is big enough, copy the list
        //
        DesiredPhyList->uNumOfEntries = 
            DesiredPhyList->uTotalNumOfEntries = Config->DesiredPhyCount;

        RtlCopyMemory(
            DesiredPhyList->dot11PhyId,
            Config->DesiredPhyList,
            Config->DesiredPhyCount * sizeof(ULONG)
            );
        
        *BytesWritten = requiredSize;
    } while (FALSE);
    
    return ndisStatus;
}

NDIS_STATUS
CfgSetDesiredPhyList(
    _In_ PAP_CONFIG Config,
    _In_ PDOT11_PHY_ID_LIST DesiredPhyList
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    ULONG index;
    DOT11_SUPPORTED_PHY_TYPES supportedPhyTypes;
    BOOLEAN anyPhyId = FALSE;
    ULONG PhyIdAny = DOT11_PHY_ID_ANY;
    
    do
    {
        if (0 == DesiredPhyList->uNumOfEntries)
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        //
        // only support up to AP_DESIRED_PHY_MAX_COUNT PHY types
        //
        if (DesiredPhyList->uNumOfEntries > AP_DESIRED_PHY_MAX_COUNT)
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }
        
        // 
        // validate PHY IDs
        //
        
        //
        // Make sure we support all the PHY IDs in the list. Since we are using
        // sequential PHY IDs, the logic below works
        //
        VNic11QuerySupportedPHYTypes(AP_GET_VNIC(Config->ApPort), 0, &supportedPhyTypes);
        
        for (index = 0; index < DesiredPhyList->uNumOfEntries; index++)
        {
            if (DesiredPhyList->dot11PhyId[index] == DOT11_PHY_ID_ANY)
            {
                anyPhyId = TRUE;
            }
            else if (DesiredPhyList->dot11PhyId[index] >= supportedPhyTypes.uTotalNumOfEntries) 
            {
                //
                // Invalid PHY ID
                //
                ndisStatus = NDIS_STATUS_INVALID_DATA;
                break;
            }
        }

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }
        
        //
        // Call VNIC to set it
        //
        ndisStatus = VNic11SetDesiredPhyIdList(
                        AP_GET_VNIC(Config->ApPort),
                        anyPhyId?&PhyIdAny:DesiredPhyList->dot11PhyId,
                        anyPhyId?1:DesiredPhyList->uNumOfEntries
                        );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }

        //
        // Copy the desired PHY list.
        //
        if (anyPhyId)
        {
            Config->DesiredPhyCount = 1;
            Config->DesiredPhyList[0] = DOT11_PHY_ID_ANY;
        }
        else
        {
            Config->DesiredPhyCount = DesiredPhyList->uNumOfEntries;
            RtlCopyMemory(
                Config->DesiredPhyList,
                DesiredPhyList->dot11PhyId,
                Config->DesiredPhyCount * sizeof(ULONG)
                );
        }

        
    } while (FALSE);
    
    return ndisStatus;
}

VOID
CfgQueryCurrentPhyId(
    _In_ PAP_CONFIG Config,
    _Out_ PULONG CurrentPhyId
    )
{
    *CurrentPhyId = VNic11QuerySelectedPhyId(AP_GET_VNIC(Config->ApPort));
}

NDIS_STATUS
CfgSetCurrentPhyId(
    _In_ PAP_CONFIG Config,
    _In_ ULONG CurrentPhyId
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    DOT11_SUPPORTED_PHY_TYPES supportedPhyTypes;

    do
    {
        //
        // validate PHY ID
        // Since we are using sequential PHY IDs, the logic below works.
        //
        VNic11QuerySupportedPHYTypes(AP_GET_VNIC(Config->ApPort), 0, &supportedPhyTypes);

        if (CurrentPhyId >= supportedPhyTypes.uTotalNumOfEntries) 
        {
            //
            // Invalid PHY ID
            //
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }
        
        // 
        // call VNIC to set it
        //
        VNic11SetSelectedPhyId(AP_GET_VNIC(Config->ApPort), CurrentPhyId);

    } while (FALSE);
    
    return ndisStatus;
}

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
CfgQueryAdditionalIe(
    _In_ PAP_CONFIG Config,
    _Out_ PDOT11_ADDITIONAL_IE AdditionalIe,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_when_invalid_ndis_length_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    ULONG requiredSize = 0;
    ULONG beaconIeOffset = sizeof(DOT11_ADDITIONAL_IE);
    ULONG responseIeOffset = 0;
    
    do
    {
        // 
        // add required size for beacon IEs
        //
        if (RtlULongAdd(beaconIeOffset, Config->AdditionalBeaconIESize, &responseIeOffset) != STATUS_SUCCESS)
        {
            // 
            // This shall not happen because we validated the IEs before
            //
            MPASSERT(FALSE);
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        // 
        // add required size for response IEs
        //
        if (RtlULongAdd(responseIeOffset, Config->AdditionalResponseIESize, &requiredSize) != STATUS_SUCCESS)
        {
            // 
            // This shall not happen because we validated the IEs before
            //
            MPASSERT(FALSE);
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        if (InformationBufferLength < requiredSize)
        {
            // 
            // the buffer is not big enough
            //
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(requiredSize);
        }

        // 
        // the buffer is big enough, copy the IEs
        // copy beacon IEs
        //
        RtlCopyMemory(
                (BYTE *)AdditionalIe + beaconIeOffset,
                Config->AdditionalBeaconIEData,
                Config->AdditionalBeaconIESize
                );

        AdditionalIe->uBeaconIEsLength = Config->AdditionalBeaconIESize;
        AdditionalIe->uBeaconIEsOffset = beaconIeOffset;
        
        // 
        // copy response IEs
        //
        RtlCopyMemory(
                (BYTE *)AdditionalIe + responseIeOffset,
                Config->AdditionalResponseIEData,
                Config->AdditionalResponseIESize
                );

        AdditionalIe->uResponseIEsLength = Config->AdditionalResponseIESize;
        AdditionalIe->uResponseIEsOffset = responseIeOffset;
        
        *BytesWritten = requiredSize;
    } while (FALSE);
    
    return ndisStatus;
}

NDIS_STATUS
CfgSetAdditionalIe(
    _In_ PAP_CONFIG Config,
    _In_ PDOT11_ADDITIONAL_IE AdditionalIe
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PVOID newBeaconIeData = NULL; 
    PVOID newResponseIeData = NULL;
    PVOID oldBeaconIeData = NULL;
    ULONG oldBeaconIeSize = 0;
    PVOID oldResponseIeData = NULL;
    ULONG oldResponseIeSize = 0;
    PVOID beaconIEBlob = NULL;
    USHORT beaconIEBlobSize = 0;
    PVOID responseIEBlob = NULL;
    USHORT responseIEBlobSize = 0;
    USHORT maxIEBlobSize;
    PVNIC vnic = AP_GET_VNIC(Config->ApPort);
    
    UNREFERENCED_PARAMETER(Config);
    UNREFERENCED_PARAMETER(AdditionalIe);

    do
    {
        // TODO: validate the IE

        // 
        // allocate memory for the new IE data and copy the IEs
        //
        if (AdditionalIe->uBeaconIEsLength > 0)
        {
            MP_ALLOCATE_MEMORY(
                AP_GET_MP_HANDLE(Config->ApPort), 
                &newBeaconIeData, 
                AdditionalIe->uBeaconIEsLength, 
                EXTAP_MEMORY_TAG
                );
            
            if (NULL == newBeaconIeData)
            {
                MpTrace(COMP_OID, DBG_SERIOUS, ("Port(%u): Failed to allocate %d bytes for extap port\n",
                                    AP_GET_PORT_NUMBER(Config->ApPort),
                                    AdditionalIe->uBeaconIEsLength));
                ndisStatus = NDIS_STATUS_RESOURCES;
                break;
            }

            // 
            // copy IEs
            //
            RtlCopyMemory(
                    newBeaconIeData,
                    (BYTE *)AdditionalIe + AdditionalIe->uBeaconIEsOffset,
                    AdditionalIe->uBeaconIEsLength
                    );
        }

        if (AdditionalIe->uResponseIEsLength > 0)
        {
            MP_ALLOCATE_MEMORY(
                AP_GET_MP_HANDLE(Config->ApPort), 
                &newResponseIeData, 
                AdditionalIe->uResponseIEsLength, 
                EXTAP_MEMORY_TAG
                );
            
            if (NULL == newResponseIeData)
            {
                MpTrace(COMP_OID, DBG_SERIOUS, ("Port(%u): Failed to allocate %d bytes for extap port\n",
                                    AP_GET_PORT_NUMBER(Config->ApPort),
                                    AdditionalIe->uResponseIEsLength));
                ndisStatus = NDIS_STATUS_RESOURCES;
                break;
            }
            
            // 
            // copy IEs
            //
            RtlCopyMemory(
                    newResponseIeData,
                    (BYTE *)AdditionalIe + AdditionalIe->uResponseIEsOffset,
                    AdditionalIe->uResponseIEsLength
                    );
        }

        // allocate memory for the new IEs

        MP_ALLOCATE_MEMORY(
            AP_GET_MP_HANDLE(Config->ApPort),
            &beaconIEBlob, 
            AP11_MAX_IE_BLOB_SIZE,
            EXTAP_MEMORY_TAG
            );

        MP_ALLOCATE_MEMORY(
            AP_GET_MP_HANDLE(Config->ApPort),
            &responseIEBlob, 
            AP11_MAX_IE_BLOB_SIZE,
            EXTAP_MEMORY_TAG
            );

        if (! beaconIEBlob || ! responseIEBlob)
        {
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }

        // back up the old IEs 

        oldBeaconIeData = Config->AdditionalBeaconIEData;
        oldBeaconIeSize = Config->AdditionalBeaconIESize;

        oldResponseIeData = Config->AdditionalResponseIEData;
        oldResponseIeSize = Config->AdditionalResponseIESize;

        // 
        // cache new IEs
        //
        Config->AdditionalBeaconIEData = newBeaconIeData;
        Config->AdditionalBeaconIESize = AdditionalIe->uBeaconIEsLength;
        newBeaconIeData = NULL;

        Config->AdditionalResponseIEData = newResponseIeData;
        Config->AdditionalResponseIESize = AdditionalIe->uResponseIEsLength;
        newResponseIeData = NULL;

        // construct the new IE blobs

        maxIEBlobSize = AP11_MAX_IE_BLOB_SIZE;
        ndisStatus = ConstructAPIEBlob(Config->ApPort, beaconIEBlob, FALSE, &maxIEBlobSize, &beaconIEBlobSize);
        if (ndisStatus == NDIS_STATUS_SUCCESS)
        {
            maxIEBlobSize = AP11_MAX_IE_BLOB_SIZE;
            ndisStatus = ConstructAPIEBlob(Config->ApPort, responseIEBlob, TRUE, &maxIEBlobSize, &responseIEBlobSize);
        }

        if (ndisStatus == NDIS_STATUS_SUCCESS)
        {
            // call VNIC to set the new IEs

            ndisStatus = VNic11SetBeaconIE(
                    vnic,
                    beaconIEBlob,
                    beaconIEBlobSize
                    );

            if (ndisStatus == NDIS_STATUS_SUCCESS)
            {
                ndisStatus = VNic11SetProbeResponseIE(
                        vnic,
                        responseIEBlob,
                        responseIEBlobSize
                        );
            }
        }

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            //
            // revert to the old IE data on failure
            //
            newBeaconIeData = Config->AdditionalBeaconIEData;
            Config->AdditionalBeaconIEData = oldBeaconIeData;
            Config->AdditionalBeaconIESize = oldBeaconIeSize;

            newResponseIeData = Config->AdditionalResponseIEData;
            Config->AdditionalResponseIEData = oldResponseIeData;
            Config->AdditionalResponseIESize = oldResponseIeSize;

            break;
        }

        // 
        // free the old IEs
        //
        if (oldBeaconIeData != NULL)
        {
            MP_FREE_MEMORY(oldBeaconIeData);
        }
        
        if (oldResponseIeData != NULL)
        {
            MP_FREE_MEMORY(oldResponseIeData);
        }

    } while (FALSE);

    if (newBeaconIeData != NULL)
    {
        MP_FREE_MEMORY(newBeaconIeData);
    }

    if (newResponseIeData != NULL)
    {
        MP_FREE_MEMORY(newResponseIeData);
    }

    if (beaconIEBlob)
    {
        if (ndisStatus == NDIS_STATUS_SUCCESS)
        {
            CfgSaveBeaconIe(Config, beaconIEBlob, beaconIEBlobSize);
        }
        else
        {
            MP_FREE_MEMORY(beaconIEBlob);
        }
    }
    if (responseIEBlob)
    {
        MP_FREE_MEMORY(responseIEBlob);
    }
    
    return ndisStatus;
}


VOID
CfgResetAdditionalIe(
    _In_ PAP_CONFIG Config
    )
{
    if (Config->AdditionalBeaconIEData)
    {
        MP_FREE_MEMORY(Config->AdditionalBeaconIEData);
        Config->AdditionalBeaconIEData = NULL;
    }
    Config->AdditionalBeaconIESize = 0;

    if (Config->AdditionalResponseIEData)
    {
        MP_FREE_MEMORY(Config->AdditionalResponseIEData);
        Config->AdditionalResponseIEData = NULL;
    }
    Config->AdditionalResponseIESize = 0;

    if (Config->ApBeaconIEData)
    {
        MP_FREE_MEMORY(Config->ApBeaconIEData);
        Config->ApBeaconIEData = NULL;
    }
    Config->ApBeaconIESize = 0;
}

VOID
CfgSaveBeaconIe(
    _In_ PAP_CONFIG Config,
    _In_ PVOID ApBeaconIEData,
    _In_ ULONG ApBeaconIESize
    )
{
    if (ApBeaconIEData && ApBeaconIESize > 0)
    {
        if (Config->ApBeaconIEData)
        {
            MP_FREE_MEMORY(Config->ApBeaconIEData);
            Config->ApBeaconIEData = NULL;
        }
        Config->ApBeaconIESize = 0;

        Config->ApBeaconIEData = ApBeaconIEData;
        Config->ApBeaconIESize = ApBeaconIESize;
    }
}
