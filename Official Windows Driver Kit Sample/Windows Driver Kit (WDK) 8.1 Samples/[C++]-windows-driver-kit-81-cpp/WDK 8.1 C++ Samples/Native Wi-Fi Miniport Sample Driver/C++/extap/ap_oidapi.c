/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    ap_oids.c

Abstract:
    Implements ExtAP OID APIs with strong type
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-25-2007    Created

Notes:

--*/
#include "precomp.h"
    
#if DOT11_TRACE_ENABLED
#include "ap_oidapi.tmh"
#endif

/** commonly used MACROs */
#define VALIDATE_AP_INIT_STATE(ApPort) \
    { \
        if (ApGetState(ApPort) != AP_STATE_STOPPED) \
        { \
            ndisStatus = NDIS_STATUS_INVALID_STATE; \
            break; \
        } \
    }

#define VALIDATE_AP_OP_STATE(ApPort) \
    { \
        if (ApGetState(ApPort) != AP_STATE_STARTED) \
        { \
            ndisStatus = NDIS_STATUS_INVALID_STATE; \
            break; \
        } \
    }

/** OID_DOT11_AUTO_CONFIG_ENABLED */
NDIS_STATUS
ApQueryAutoConfigEnabled(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PULONG AutoConfigEnabled
    )
{
    // The setting is in AP config
    CfgQueryAutoConfigEnabled(
        AP_GET_CONFIG(ApPort), 
        AutoConfigEnabled
        );
    
    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
ApSetAutoConfigEnabled(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ ULONG AutoConfigEnabled
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // Set is allowed only in AP INIT state
        VALIDATE_AP_INIT_STATE(ApPort);

        // The setting is in AP config
        ndisStatus = CfgSetAutoConfigEnabled(
                        AP_GET_CONFIG(ApPort), 
                        AutoConfigEnabled
                        );
    } while (FALSE);

    return ndisStatus;
}

#if 0
/** OID_DOT11_NIC_POWER_STATE */
NDIS_STATUS
ApQueryNicPowerState(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ BOOLEAN * NicPowerState
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    return ndisStatus;
}

NDIS_STATUS
ApSetNicPowerState(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ BOOLEAN NicPowerState
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // Set is allowed only in AP INIT state
        VALIDATE_AP_INIT_STATE(ApPort);

    } while (FALSE);

    return ndisStatus;
}
#endif
/** OID_DOT11_OPERATIONAL_RATE_SET */
NDIS_STATUS
ApQueryOperationalRateSet(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PDOT11_RATE_SET OperationalRateSet
    )
{
    // The setting is in association manager
    AmQueryOperationalRateSet(
        AP_GET_ASSOC_MGR(ApPort), 
        OperationalRateSet
        );
    
    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
ApSetOperationalRateSet(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_RATE_SET OperationalRateSet
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // Set is allowed only in AP INIT state
        VALIDATE_AP_INIT_STATE(ApPort);

        // The setting is in association manager
        ndisStatus = AmSetOperationalRateSet(
                        AP_GET_ASSOC_MGR(ApPort), 
                        OperationalRateSet
                        );
    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_BEACON_PERIOD */
NDIS_STATUS
ApQueryBeaconPeriod(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PULONG BeaconPeriod
    )
{
    // The setting is in AP config
    CfgQueryBeaconPeriod(
        AP_GET_CONFIG(ApPort), 
        BeaconPeriod
        );
    
    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
ApSetBeaconPeriod(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_range_(1, 65535) ULONG BeaconPeriod
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // Set is allowed only in AP INIT state
        VALIDATE_AP_INIT_STATE(ApPort);

        // The setting is in AP config
        ndisStatus = CfgSetBeaconPeriod(
                        AP_GET_CONFIG(ApPort),
                        BeaconPeriod
                        );
    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_DTIM_PERIOD */
NDIS_STATUS
ApQueryDTimPeriod(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PULONG DTimPeriod
    )
{
    // The setting is in AP config
    CfgQueryDTimPeriod(
        AP_GET_CONFIG(ApPort), 
        DTimPeriod
        );
    
    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
ApSetDTimPeriod(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_range_(1, 255) ULONG DTimPeriod
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // Set is allowed only in AP INIT state
        VALIDATE_AP_INIT_STATE(ApPort);

        // The setting is in AP config
        ndisStatus = CfgSetDTimPeriod(
                        AP_GET_CONFIG(ApPort),
                        DTimPeriod
                        );
    } while (FALSE);

    return ndisStatus;
}

#if 0
/** OID_DOT11_RTS_THRESHOLD */
NDIS_STATUS
ApQueryRtsThreshold(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PULONG RtsThreshold
    )
{
    // The setting is in AP config
    CfgQueryRtsThreshold(
        AP_GET_CONFIG(ApPort), 
        RtsThreshold
        );
    
    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
ApSetRtsThreshold(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_range_(0, 2347) ULONG RtsThreshold
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    // The setting is in AP config
    ndisStatus = CfgSetRtsThreshold(
                    AP_GET_CONFIG(ApPort),
                    RtsThreshold
                    );

    return ndisStatus;
}

/** OID_DOT11_SHORT_RETRY_LIMIT */
NDIS_STATUS
ApQueryShortRetryLimit(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PULONG ShortRetryLimit
    )
{
    // The setting is in AP config
    CfgQueryShortRetryLimit(
        AP_GET_CONFIG(ApPort), 
        ShortRetryLimit
        );

    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
ApSetShortRetryLimit(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_range_(1, 255) ULONG ShortRetryLimit
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    // The setting is in AP config
    ndisStatus = CfgSetShortRetryLimit(
                    AP_GET_CONFIG(ApPort),
                    ShortRetryLimit
                    );

    return ndisStatus;
}

/** OID_DOT11_LONG_RETRY_LIMIT */
NDIS_STATUS
ApQueryLongRetryLimit(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PULONG LongRetryLimit
    )
{
    // The setting is in AP config
    CfgQueryLongRetryLimit(
        AP_GET_CONFIG(ApPort), 
        LongRetryLimit
        );

    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
ApSetLongRetryLimit(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_range_(1, 255) ULONG LongRetryLimit
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    // The setting is in AP config
    ndisStatus = CfgSetLongRetryLimit(
                    AP_GET_CONFIG(ApPort),
                    LongRetryLimit
                    );

    return ndisStatus;
}

/** OID_DOT11_FRAGMENTATION_THRESHOLD */
NDIS_STATUS
ApQueryFragmentationThreshold(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PULONG FragmentationThreshold
    )
{
    // The setting is in AP config
    CfgQueryFragmentationThreshold(
        AP_GET_CONFIG(ApPort), 
        FragmentationThreshold
        );

    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
ApSetFragmentationThreshold(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_range_(256, 2346) ULONG FragmentationThreshold
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    // The setting is in AP config
    ndisStatus = CfgSetFragmentationThreshold(
                    AP_GET_CONFIG(ApPort),
                    FragmentationThreshold
                    );

    return ndisStatus;
}
#endif
/** OID_DOT11_AVAILABLE_CHANNEL_LIST */
_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
ApQueryAvailableChannelList(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PDOT11_AVAILABLE_CHANNEL_LIST AvailableChannelList
    )
{
    NDIS_STATUS ndisStatus = STATUS_NOT_IMPLEMENTED;

    UNREFERENCED_PARAMETER(ApPort);
    UNREFERENCED_PARAMETER(AvailableChannelList);
    
    return ndisStatus;
}

/** Set not applicable */

/** OID_DOT11_CURRENT_CHANNEL */
NDIS_STATUS
ApQueryCurrentChannel(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PULONG CurrentChannel
    )
{
    // The setting is in AP config
    CfgQueryCurrentChannel(
        AP_GET_CONFIG(ApPort), 
        CurrentChannel
        );

    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
ApSetCurrentChannel(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_range_(1, 14) ULONG CurrentChannel
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    // The setting is in AP config
    ndisStatus = CfgSetCurrentChannel(
                    AP_GET_CONFIG(ApPort),
                    CurrentChannel
                    );

    return ndisStatus;
}

/** OID_DOT11_AVAILABLE_FREQUENCY_LIST */
_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
ApQueryAvailableFrequencyList(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PDOT11_AVAILABLE_FREQUENCY_LIST AvailableFrequencyList
    )
{
    NDIS_STATUS ndisStatus = STATUS_NOT_IMPLEMENTED;

    UNREFERENCED_PARAMETER(ApPort);
    UNREFERENCED_PARAMETER(AvailableFrequencyList);
    
    return ndisStatus;
}

/** Set not applicable */

/** OID_DOT11_CURRENT_FREQUENCY */
NDIS_STATUS
ApQueryCurrentFrequency(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PULONG CurrentFrequency
    )
{
    // The setting is in AP config
    CfgQueryCurrentFrequency(
        AP_GET_CONFIG(ApPort), 
        CurrentFrequency
        );

    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
ApSetCurrentFrequency(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_range_(0, 200) ULONG CurrentFrequency
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    // The setting is in AP config
    ndisStatus = CfgSetCurrentFrequency(
                    AP_GET_CONFIG(ApPort),
                    CurrentFrequency
                    );

    return ndisStatus;
}

/** OID_DOT11_DESIRED_SSID_LIST */
NDIS_STATUS
ApQueryDesiredSsidList(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PDOT11_SSID_LIST SsidList,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // The setting is in association manager
        ndisStatus = AmQuerySsid(
                    AP_GET_ASSOC_MGR(ApPort), 
                    SsidList,
                    InformationBufferLength,
                    BytesWritten,
                    BytesNeeded
                    );

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }
        
        // assign NDIS header
        MP_ASSIGN_NDIS_OBJECT_HEADER(
            SsidList->Header,
            NDIS_OBJECT_TYPE_DEFAULT,
            DOT11_SSID_LIST_REVISION_1,
            sizeof(DOT11_SSID_LIST)
            );
    } while (FALSE);

    return ndisStatus;
}

NDIS_STATUS
ApSetDesiredSsidList(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_SSID_LIST SsidList
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        if (!MP_VERIFY_NDIS_OBJECT_HEADER_DEFAULT(
                    SsidList->Header,
                    NDIS_OBJECT_TYPE_DEFAULT,
                    DOT11_SSID_LIST_REVISION_1,
                    sizeof(DOT11_SSID_LIST)
                    ))
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        // Set is allowed only in AP INIT state
        VALIDATE_AP_INIT_STATE(ApPort);

        // The setting is in association manager
        ndisStatus = AmSetSsid(
            AP_GET_ASSOC_MGR(ApPort), 
            SsidList
            );
    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_EXCLUDE_UNENCRYPTED */
NDIS_STATUS
ApQueryExcludeUnencrypted(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ BOOLEAN * ExcludeUnencrypted
    )
{
    // The setting is in AP association manager
    AmQueryExcludeUnencrypted(
        AP_GET_ASSOC_MGR(ApPort), 
        ExcludeUnencrypted
        );

    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
ApSetExcludeUnencrypted(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ BOOLEAN ExcludeUnencrypted
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // Set is allowed only in AP INIT state
        VALIDATE_AP_INIT_STATE(ApPort);

        // The setting is in AP association manager
        ndisStatus = AmSetExcludeUnencrypted(
                        AP_GET_ASSOC_MGR(ApPort),
                        ExcludeUnencrypted
                        );
    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_STATISTICS */
NDIS_STATUS
ApQueryStatistics(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_(InformationBufferLength) PVOID  InformationBuffer,
    _In_ _In_range_(sizeof(DOT11_STATISTICS), ULONG_MAX) ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_STATISTICS Statistics = InformationBuffer;

    do
    {
        NdisZeroMemory(InformationBuffer, InformationBufferLength);

        MP_ASSIGN_NDIS_OBJECT_HEADER(
            Statistics->Header,
            NDIS_OBJECT_TYPE_DEFAULT,
            DOT11_STATISTICS_REVISION_1,
            sizeof(DOT11_STATISTICS)
            );

        ndisStatus = VNic11QueryDot11Statistics(
                        AP_GET_VNIC(ApPort),
                        Statistics,
                        InformationBufferLength,
                        BytesWritten,
                        BytesNeeded
                        );

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }


        MPASSERT(
            Statistics->Header.Type == NDIS_OBJECT_TYPE_DEFAULT &&
            Statistics->Header.Revision == DOT11_STATISTICS_REVISION_1   &&
            Statistics->Header.Size == sizeof(DOT11_STATISTICS)
        );

    } while(FALSE);

    return ndisStatus;
}

/** Set not applicable */

/** OID_DOT11_PRIVACY_EXEMPTION_LIST */
NDIS_STATUS
ApQueryPrivacyExemptionList(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PDOT11_PRIVACY_EXEMPTION_LIST PrivacyExemptionList,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // The setting is in AP association manager
        ndisStatus = AmQueryPrivacyExemptionList(
                    AP_GET_ASSOC_MGR(ApPort), 
                    PrivacyExemptionList,
                    InformationBufferLength,
                    BytesWritten,
                    BytesNeeded
                    );

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }

        // assign NDIS header
        MP_ASSIGN_NDIS_OBJECT_HEADER(
            PrivacyExemptionList->Header,
            NDIS_OBJECT_TYPE_DEFAULT,
            DOT11_PRIVACY_EXEMPTION_LIST_REVISION_1,
            sizeof(DOT11_PRIVACY_EXEMPTION_LIST)
            );
    } while (FALSE);    

    return ndisStatus;
}

NDIS_STATUS
ApSetPrivacyExemptionList(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_PRIVACY_EXEMPTION_LIST PrivacyExemptionList
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        if (!MP_VERIFY_NDIS_OBJECT_HEADER_DEFAULT(
                    PrivacyExemptionList->Header,
                    NDIS_OBJECT_TYPE_DEFAULT,
                    DOT11_PRIVACY_EXEMPTION_LIST_REVISION_1,
                    sizeof(DOT11_PRIVACY_EXEMPTION_LIST)
                    ))
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }
        
        // Set is allowed only in AP INIT state
        VALIDATE_AP_INIT_STATE(ApPort);

        // The setting is in AP association manager
        ndisStatus = AmSetPrivacyExemptionList(
                        AP_GET_ASSOC_MGR(ApPort),
                        PrivacyExemptionList
                        );
    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_ENABLED_AUTHENTICATION_ALGORITHM */
NDIS_STATUS
ApQueryEnabledAuthenticationAlgorithm(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PDOT11_AUTH_ALGORITHM_LIST EnabledAuthenticationAlgorithm,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // The setting is in association manager
        ndisStatus = AmQueryAuthAlgorithm(
            AP_GET_ASSOC_MGR(ApPort), 
            EnabledAuthenticationAlgorithm,
            InformationBufferLength,
            BytesWritten,
            BytesNeeded
            );
        
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }
        
        // assign NDIS header
        MP_ASSIGN_NDIS_OBJECT_HEADER(
            EnabledAuthenticationAlgorithm->Header,
            NDIS_OBJECT_TYPE_DEFAULT,
            DOT11_AUTH_ALGORITHM_LIST_REVISION_1,
            sizeof(DOT11_AUTH_ALGORITHM_LIST)
            );
    } while (FALSE);
    
    return ndisStatus;
}

NDIS_STATUS
ApSetEnabledAuthenticationAlgorithm(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_AUTH_ALGORITHM_LIST EnabledAuthenticationAlgorithm
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        if (!MP_VERIFY_NDIS_OBJECT_HEADER_DEFAULT(
                    EnabledAuthenticationAlgorithm->Header,
                    NDIS_OBJECT_TYPE_DEFAULT,
                    DOT11_AUTH_ALGORITHM_LIST_REVISION_1,
                    sizeof(DOT11_AUTH_ALGORITHM_LIST)
                    ))
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }
        
        // Set is allowed only in AP INIT state
        VALIDATE_AP_INIT_STATE(ApPort);

        // The setting is in association manager
        ndisStatus = AmSetAuthAlgorithm(
            AP_GET_ASSOC_MGR(ApPort), 
            EnabledAuthenticationAlgorithm
            );
    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_ENABLED_UNICAST_CIPHER_ALGORITHM */
NDIS_STATUS
ApQueryEnabledUnicastCipherAlgorithm(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PDOT11_CIPHER_ALGORITHM_LIST EnabledUnicastCipherAlgorithm,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // The setting is in association manager
        ndisStatus = AmQueryUnicastCipherAlgorithm(
                    AP_GET_ASSOC_MGR(ApPort), 
                    EnabledUnicastCipherAlgorithm,
                    InformationBufferLength,
                    BytesWritten,
                    BytesNeeded
                    );
        
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }
        
        // assign NDIS header
        MP_ASSIGN_NDIS_OBJECT_HEADER(
            EnabledUnicastCipherAlgorithm->Header,
            NDIS_OBJECT_TYPE_DEFAULT,
            DOT11_CIPHER_ALGORITHM_LIST_REVISION_1,
            sizeof(DOT11_CIPHER_ALGORITHM_LIST)
            );
    } while (FALSE);

    return ndisStatus;
}

NDIS_STATUS
ApSetEnabledUnicastCipherAlgorithm(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_CIPHER_ALGORITHM_LIST EnabledUnicastCipherAlgorithm
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        if (!MP_VERIFY_NDIS_OBJECT_HEADER_DEFAULT(
                    EnabledUnicastCipherAlgorithm->Header,
                    NDIS_OBJECT_TYPE_DEFAULT,
                    DOT11_CIPHER_ALGORITHM_LIST_REVISION_1,
                    sizeof(DOT11_CIPHER_ALGORITHM_LIST)
                    ))
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }
        
        // Set is allowed only in AP INIT state
        VALIDATE_AP_INIT_STATE(ApPort);

        // The setting is in association manager
        ndisStatus = AmSetUnicastCipherAlgorithm(
            AP_GET_ASSOC_MGR(ApPort), 
            EnabledUnicastCipherAlgorithm
            );
    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_ENABLED_MULTICAST_CIPHER_ALGORITHM */
NDIS_STATUS
ApQueryEnabledMulticastCipherAlgorithm(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PDOT11_CIPHER_ALGORITHM_LIST EnabledMulticastCipherAlgorithm,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // The setting is in association manager
        ndisStatus = AmQueryMulticastCipherAlgorithm(
                    AP_GET_ASSOC_MGR(ApPort), 
                    EnabledMulticastCipherAlgorithm,
                    InformationBufferLength,
                    BytesWritten,
                    BytesNeeded
                    );
        
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }
        
        // assign NDIS header
        MP_ASSIGN_NDIS_OBJECT_HEADER(
            EnabledMulticastCipherAlgorithm->Header,
            NDIS_OBJECT_TYPE_DEFAULT,
            DOT11_CIPHER_ALGORITHM_LIST_REVISION_1,
            sizeof(DOT11_CIPHER_ALGORITHM_LIST)
            );
    } while (FALSE);
    
    return ndisStatus;
}

NDIS_STATUS
ApSetEnabledMulticastCipherAlgorithm(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_CIPHER_ALGORITHM_LIST EnabledMulticastCipherAlgorithm
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        if (!MP_VERIFY_NDIS_OBJECT_HEADER_DEFAULT(
                    EnabledMulticastCipherAlgorithm->Header,
                    NDIS_OBJECT_TYPE_DEFAULT,
                    DOT11_CIPHER_ALGORITHM_LIST_REVISION_1,
                    sizeof(DOT11_CIPHER_ALGORITHM_LIST)
                    ))
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }
        
        // Set is allowed only in AP INIT state
        VALIDATE_AP_INIT_STATE(ApPort);

        // The setting is in association manager
        ndisStatus = AmSetMulticastCipherAlgorithm(
            AP_GET_ASSOC_MGR(ApPort), 
            EnabledMulticastCipherAlgorithm
            );
    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_CIPHER_DEFAULT_KEY_ID */
NDIS_STATUS
ApQueryCipherDefaultKeyId(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PULONG CipherDefaultKeyId
    )
{
    // The setting is in AP config
    CfgQueryCipherDefaultKeyId(
        AP_GET_CONFIG(ApPort), 
        CipherDefaultKeyId
        );

    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
ApSetCipherDefaultKeyId(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ ULONG CipherDefaultKeyId
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    // The setting is in AP config
    ndisStatus = CfgSetCipherDefaultKeyId(
                    AP_GET_CONFIG(ApPort),
                    CipherDefaultKeyId
                    );
    
    return ndisStatus;
}

/** OID_DOT11_CIPHER_DEFAULT_KEY */
/** Query not applicable */

NDIS_STATUS
ApSetCipherDefaultKey(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_CIPHER_DEFAULT_KEY_VALUE CipherDefaultKey
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    UNREFERENCED_PARAMETER(CipherDefaultKey);
    do
    {
        if (!MP_VERIFY_NDIS_OBJECT_HEADER_DEFAULT(
                    CipherDefaultKey->Header,
                    NDIS_OBJECT_TYPE_DEFAULT,
                    DOT11_CIPHER_DEFAULT_KEY_VALUE_REVISION_1,
                    sizeof(DOT11_CIPHER_DEFAULT_KEY_VALUE)
                    ))
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }
        
        //
        // Check the validity of the defaultKey
        //
        if (CipherDefaultKey->uKeyIndex >= VNic11DefaultKeyTableSize(AP_GET_VNIC(ApPort))) 
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }


        if (!IsSupportedCipher(ApPort,
                CipherDefaultKey->AlgorithmId,
                CipherDefaultKey->usKeyLength))
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        //
        // Set HW default key
        //
        ndisStatus = VNic11SetDefaultKey(AP_GET_VNIC(ApPort),
                        CipherDefaultKey->MacAddr,
                        CipherDefaultKey->uKeyIndex, 
                        CipherDefaultKey->bStatic,
                        CipherDefaultKey->AlgorithmId,
                        CipherDefaultKey->bDelete ? 0 : CipherDefaultKey->usKeyLength,
                        CipherDefaultKey->ucKey
                        );


    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_CIPHER_KEY_MAPPING_KEY */
/** Query not applicable */

NDIS_STATUS
ApSetCipherKeyMappingKey(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_BYTE_ARRAY CipherKeyMappingKeyByteArray
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_CIPHER_KEY_MAPPING_KEY_VALUE cipherKeyMappingKey = NULL;
    PUCHAR pucBuffer = NULL;
    ULONG ulBytesRemaining = 0;
    ULONG ulBytesRequired = 0;
    
    do
    {
        if (!MP_VERIFY_NDIS_OBJECT_HEADER_DEFAULT(
                    CipherKeyMappingKeyByteArray->Header,
                    NDIS_OBJECT_TYPE_DEFAULT,
                    DOT11_CIPHER_KEY_MAPPING_KEY_VALUE_BYTE_ARRAY_REVISION_1,
                    sizeof(DOT11_BYTE_ARRAY)
                    ))
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        //
        // At least one key must be present.
        //
        if (CipherKeyMappingKeyByteArray->uNumOfBytes < sizeof(DOT11_CIPHER_KEY_MAPPING_KEY_VALUE))
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        pucBuffer = CipherKeyMappingKeyByteArray->ucBuffer;
        ulBytesRemaining = CipherKeyMappingKeyByteArray->uNumOfBytes;

        while (ulBytesRemaining)
        {
            if ( ulBytesRemaining < sizeof(DOT11_CIPHER_KEY_MAPPING_KEY_VALUE))
            {
                ndisStatus = NDIS_STATUS_INVALID_DATA;
                break;
            }

            cipherKeyMappingKey = (PDOT11_CIPHER_KEY_MAPPING_KEY_VALUE) pucBuffer;

            ulBytesRequired = (ULONG) FIELD_OFFSET(DOT11_CIPHER_KEY_MAPPING_KEY_VALUE, ucKey) +
                cipherKeyMappingKey->usKeyLength;

            if ( ulBytesRemaining < ulBytesRequired )
            {
                ndisStatus = NDIS_STATUS_INVALID_DATA;
                break;
            }

            pucBuffer += ulBytesRequired;
            ulBytesRemaining -= ulBytesRequired;

            if (!IsSupportedCipher(ApPort,
                    cipherKeyMappingKey->AlgorithmId,
                    cipherKeyMappingKey->usKeyLength))
            {
                ndisStatus = NDIS_STATUS_INVALID_DATA;
                break;
            }

            ndisStatus = VNic11SetKeyMappingKey(AP_GET_VNIC(ApPort),
                            cipherKeyMappingKey->PeerMacAddr,
                            cipherKeyMappingKey->Direction,
                            cipherKeyMappingKey->bStatic,
                            cipherKeyMappingKey->AlgorithmId,
                            cipherKeyMappingKey->bDelete ? 0 : cipherKeyMappingKey->usKeyLength,
                            cipherKeyMappingKey->ucKey
                            );
            if (ndisStatus != NDIS_STATUS_SUCCESS)
            {
                break;
            }
        }
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }
    } while (FALSE);


    return ndisStatus;
}

/** OID_DOT11_ENUM_PEER_INFO */
NDIS_STATUS
ApEnumPeerInfo(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PDOT11_PEER_INFO_LIST PeerInfo,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // The setting is in association manager
        ndisStatus = AmEnumPeerInfo(
                    AP_GET_ASSOC_MGR(ApPort), 
                    PeerInfo,
                    InformationBufferLength,
                    BytesWritten,
                    BytesNeeded
                    );

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }
        
        // assign NDIS header
        MP_ASSIGN_NDIS_OBJECT_HEADER(
            PeerInfo->Header,
            NDIS_OBJECT_TYPE_DEFAULT,
            DOT11_PEER_INFO_LIST_REVISION_1,
            sizeof(DOT11_PEER_INFO_LIST)
            );
    } while (FALSE);
    
    return ndisStatus;
}

/** Set not applicable */

/** OID_DOT11_DISASSOCIATE_REQUEST */
/** Query not applicable */

NDIS_STATUS
ApDisassociatePeerRequest(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_DISASSOCIATE_PEER_REQUEST DisassociateRequest
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    UNREFERENCED_PARAMETER(DisassociateRequest);
    do
    {
        if (!MP_VERIFY_NDIS_OBJECT_HEADER_DEFAULT(
                    DisassociateRequest->Header,
                    NDIS_OBJECT_TYPE_DEFAULT,
                    DOT11_DISASSOCIATE_PEER_REQUEST_REVISION_1,
                    sizeof(DOT11_DISASSOCIATE_PEER_REQUEST)
                    ))
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }
        
        // Set is allowed only in AP OP state
        VALIDATE_AP_OP_STATE(ApPort);

        // Let association manager handle the request
        ndisStatus = AmDisassociatePeerRequest(
                        AP_GET_ASSOC_MGR(ApPort), 
                        &DisassociateRequest->PeerMacAddr,
                        DisassociateRequest->usReason
                        );

    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_DESIRED_PHY_LIST */
NDIS_STATUS
ApQueryDesiredPhyList(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PDOT11_PHY_ID_LIST DesiredPhyList,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // The setting is in AP config
        ndisStatus = CfgQueryDesiredPhyList(
                    AP_GET_CONFIG(ApPort), 
                    DesiredPhyList,
                    InformationBufferLength,
                    BytesWritten,
                    BytesNeeded
                    );

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }
        
        // assign NDIS header
        MP_ASSIGN_NDIS_OBJECT_HEADER(
            DesiredPhyList->Header,
            NDIS_OBJECT_TYPE_DEFAULT,
            DOT11_PHY_ID_LIST_REVISION_1,
            sizeof(DOT11_PHY_ID_LIST)
            );
    } while (FALSE);
    
    return ndisStatus;
}

NDIS_STATUS
ApSetDesiredPhyList(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_PHY_ID_LIST DesiredPhyList
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        if (!MP_VERIFY_NDIS_OBJECT_HEADER_DEFAULT(
                    DesiredPhyList->Header,
                    NDIS_OBJECT_TYPE_DEFAULT,
                    DOT11_PHY_ID_LIST_REVISION_1,
                    sizeof(DOT11_PHY_ID_LIST)
                    ))
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }
        
        // Set is allowed only in AP INIT state
        VALIDATE_AP_INIT_STATE(ApPort);

        // The setting is in AP config
        ndisStatus = CfgSetDesiredPhyList(
                        AP_GET_CONFIG(ApPort),
                        DesiredPhyList
                        );
    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_CURRENT_PHY_ID */
NDIS_STATUS
ApQueryCurrentPhyId(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PULONG CurrentPhyId
    )
{
    // The setting is in AP config
    CfgQueryCurrentPhyId(
        AP_GET_CONFIG(ApPort), 
        CurrentPhyId
        );

    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
ApSetCurrentPhyId(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ ULONG CurrentPhyId
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    // The setting is in AP config
    ndisStatus = CfgSetCurrentPhyId(
                    AP_GET_CONFIG(ApPort),
                    CurrentPhyId
                    );
    return ndisStatus;
}

/** OID_DOT11_PORT_STATE_NOTIFICATION */
/** Query not applicable */

NDIS_STATUS
ApSetPortStateNotification(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_PORT_STATE_NOTIFICATION PortStateNotification
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    UNREFERENCED_PARAMETER(PortStateNotification);
    do
    {
        if (!MP_VERIFY_NDIS_OBJECT_HEADER_DEFAULT(
                    PortStateNotification->Header,
                    NDIS_OBJECT_TYPE_DEFAULT,
                    DOT11_PORT_STATE_NOTIFICATION_REVISION_1,
                    sizeof(DOT11_PORT_STATE_NOTIFICATION)
                    ))
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }
        
        // Set is allowed only in AP OP state
        VALIDATE_AP_OP_STATE(ApPort);

        // This OID is handled by association manager
        ndisStatus = AmSetStaPortState(
                        AP_GET_ASSOC_MGR(ApPort), 
                        &PortStateNotification->PeerMac, 
                        PortStateNotification->bOpen
                        );

    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_SCAN_REQUEST */
/** Query not applicable */

NDIS_STATUS
ApScanRequest(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PVOID ScanRequestId,
    _In_ PDOT11_SCAN_REQUEST_V2 ScanRequest,
    _In_ ULONG ScanRequestBufferLength
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        //
        // Validate scan request
        //
        ndisStatus = BasePortValidateScanRequest(AP_GET_MP_PORT(ApPort), ScanRequest);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }

        //
        // TODO: validate length?
        //

        //
        // If NIC power state is off, fail this request
        //
        if (!VNic11QueryNicPowerState(AP_GET_VNIC(ApPort), FALSE))
        {
            MpTrace(COMP_OID, DBG_SERIOUS, ("Port(%u): Scan failed as Nic is currently turned off.\n", AP_GET_PORT_NUMBER(ApPort)));
            ndisStatus = NDIS_STATUS_DOT11_POWER_STATE_INVALID;
            break;
        }

        //
        // If a reset is in progress on the NIC, fail this request
        //
        if (MP_TEST_PORT_STATUS(AP_GET_MP_PORT(ApPort), (MP_PORT_IN_RESET)))
        {
            MpTrace(COMP_OID, DBG_SERIOUS,  ("Port(%u): Scan failed as a reset is in progress on this adapter.\n", AP_GET_PORT_NUMBER(ApPort)));
            ndisStatus = NDIS_STATUS_RESET_IN_PROGRESS;
            break;
        }

        //
        // If a halt is in progress on the NIC, fail this request
        //
        if (MP_TEST_PORT_STATUS(AP_GET_MP_PORT(ApPort), (MP_PORT_HALTING)))
        {
            MpTrace(COMP_OID, DBG_SERIOUS,  ("Port(%u): Scan failed as this adapter is halting.\n", AP_GET_PORT_NUMBER(ApPort)));
            ndisStatus = NDIS_STATUS_DOT11_MEDIA_IN_USE;
            break;
        }      
        
        //
        // If the NIC is paused, fail this request
        //
        if (MP_TEST_PORT_STATUS(AP_GET_MP_PORT(ApPort), (MP_PORT_PAUSED | MP_PORT_PAUSING)))
        {
            MpTrace(COMP_OID, DBG_SERIOUS,  ("Port(%u): Scan failed as this adapter is pausing.\n", AP_GET_PORT_NUMBER(ApPort)));
            ndisStatus = NDIS_STATUS_DOT11_MEDIA_IN_USE;
            break;
        }

        //
        // Let association manager handle the scan request
        //
        ndisStatus = AmScanRequest(
                        AP_GET_ASSOC_MGR(ApPort), 
                        ScanRequestId, 
                        ScanRequest, 
                        ScanRequestBufferLength
                        );
    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_INCOMING_ASSOCIATION_DECISION */
/** Query not applicable */

/**
 * This is a direct OID call.
 */
NDIS_STATUS
ApSetIncomingAssociationDecision(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_INCOMING_ASSOC_DECISION IncomingAssociationDecision
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        if (!MP_VERIFY_NDIS_OBJECT_HEADER_DEFAULT(
                    IncomingAssociationDecision->Header,
                    NDIS_OBJECT_TYPE_DEFAULT,
                    DOT11_INCOMING_ASSOC_DECISION_REVISION_1,
                    sizeof(DOT11_INCOMING_ASSOC_DECISION)
                    ))
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }
        
        // Set is allowed only in AP OP state
        VALIDATE_AP_OP_STATE(ApPort);

        //
        // This OID is handled by association manager
        //
        ndisStatus = AmAssociationDecision(
                        AP_GET_ASSOC_MGR(ApPort), 
                        IncomingAssociationDecision
                        );

    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_ADDITIONAL_IE */
NDIS_STATUS
ApQueryAdditionalIe(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ PDOT11_ADDITIONAL_IE AdditionalIe,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // The setting is in AP config
        ndisStatus = CfgQueryAdditionalIe(
                    AP_GET_CONFIG(ApPort), 
                    AdditionalIe,
                    InformationBufferLength,
                    BytesWritten,
                    BytesNeeded
                    );

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }
        
        // assign NDIS header
        MP_ASSIGN_NDIS_OBJECT_HEADER(
            AdditionalIe->Header,
            NDIS_OBJECT_TYPE_DEFAULT,
            DOT11_ADDITIONAL_IE_REVISION_1,
            sizeof(DOT11_ADDITIONAL_IE)
            );
    
    } while (FALSE);
    
    return ndisStatus;
}

NDIS_STATUS
ApSetAdditionalIe(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_ADDITIONAL_IE AdditionalIe
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        if (!MP_VERIFY_NDIS_OBJECT_HEADER_DEFAULT(
                    AdditionalIe->Header,
                    NDIS_OBJECT_TYPE_DEFAULT,
                    DOT11_ADDITIONAL_IE_REVISION_1,
                    sizeof(DOT11_ADDITIONAL_IE)
                    ))
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }
        
        // The setting is in AP config
        ndisStatus = CfgSetAdditionalIe(
                        AP_GET_CONFIG(ApPort),
                        AdditionalIe
                        );
    } while (FALSE);
    
    return ndisStatus;
}

/** OID_DOT11_WPS_ENABLED */
NDIS_STATUS
ApQueryWpsEnabled(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_ BOOLEAN * WpsEnabled
    )
{
    // The setting is in association manager
    AmQueryWpsEnabled(
        AP_GET_ASSOC_MGR(ApPort), 
        WpsEnabled
        );

    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
ApSetWpsEnabled(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ BOOLEAN WpsEnabled
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    // The setting is in association manager
    ndisStatus = AmSetWpsEnabled(
        AP_GET_ASSOC_MGR(ApPort), 
        WpsEnabled
        );

    return ndisStatus;
}


/** OID_DOT11_START_AP_REQUEST */
/** Query not applicable */

NDIS_STATUS
ApStartApRequest(
    _In_ PMP_EXTAP_PORT ApPort
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // Set is allowed only in AP INIT state
        VALIDATE_AP_INIT_STATE(ApPort);

        ndisStatus = StartExtAp(ApPort);
    } while (FALSE);

    return ndisStatus;
}

/** OID_GEN_CURRENT_PACKET_FILTER */
/** Query is handled by the base port */

NDIS_STATUS
ApSetPacketFilter(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ ULONG PacketFilter
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    ULONG VNicPacketFilter;

    do
    {
        //
        // This is a generic OID call.
        // We'll just handle it here.
        // This OID is allowed before or after AP is started.
        //
        
        //
        // Any bits not supported?
        //
        if (PacketFilter & ~NDIS_PACKET_TYPE_ALL_802_11_FILTERS)
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        //
        // The packet filter we set on the VNIC is a super set of the
        // packet filter set by the OS. The hardware filters out packets that
        // the miniport internally  & the OS are not interested in and then
        // we do software filtering of packets the OS is not interested in
        //
        VNicPacketFilter = PacketFilter | 
                            NDIS_PACKET_TYPE_802_11_DIRECTED_MGMT |
                            NDIS_PACKET_TYPE_802_11_BROADCAST_MGMT |
                            NDIS_PACKET_TYPE_802_11_MULTICAST_MGMT |
                            NDIS_PACKET_TYPE_802_11_ALL_MULTICAST_MGMT;

        //
        // Set packet filter to VNIC
        //
        ndisStatus = VNic11SetPacketFilter(
                        AP_GET_VNIC(ApPort),
                        VNicPacketFilter
                        );
        
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }

        //
        // Save the new packet filter value in the base port
        //
        AP_GET_MP_PORT(ApPort)->PacketFilter = PacketFilter;

    } while(FALSE);

    return(ndisStatus);
}

/** OID_DOT11_CURRENT_OPERATION_MODE */
/** OID_DOT11_FLUSH_BSS_LIST */
/** Handled by MP Shim layer */

/** Request methods */
    
/** OID_DOT11_RESET_REQUEST */
NDIS_STATUS
ApResetRequest(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_RESET_REQUEST ResetRequest,
    _Out_ PDOT11_STATUS_INDICATION StatusIndication
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    AP_STATE apState = ApGetState(ApPort);

    StatusIndication->uStatusType = DOT11_STATUS_RESET_CONFIRM;
    StatusIndication->ndisStatus = NDIS_STATUS_SUCCESS;
    
    do
    {
        if (ResetRequest->dot11ResetType != dot11_reset_type_phy_and_mac)
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        // Reset Additional IE data
        CfgResetAdditionalIe(
            AP_GET_CONFIG(ApPort)
            );

        // 
        // Reset VNIC first
        //
        ndisStatus = VNic11Dot11Reset(
                        AP_GET_VNIC(ApPort), 
                        ResetRequest
                        );

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }

        // 
        // Stop AP
        //
        if (AP_STATE_STARTED == apState || AP_STATE_STARTING == apState)
        {
            StopExtAp(ApPort);
        }

        AmRestoreDefault( AP_GET_ASSOC_MGR(ApPort) );

        ndisStatus = VNic11Dot11ResetComplete(AP_GET_VNIC(ApPort));
    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_ENUM_BSS_LIST */
// TODO: move this OID to base port
NDIS_STATUS
ApEnumerateBSSList(
    _In_ PMP_EXTAP_PORT ApPort,
    _Inout_ PVOID InformationBuffer,
    _In_ ULONG InputBufferLength,
    _In_ ULONG OutputBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_BYTE_ARRAY dot11ByteArray;
    DOT11_COUNTRY_OR_REGION_STRING countryRegionString;

    do
    {
        *BytesWritten = 0;
        *BytesNeeded = 0;

        //
        // Check enough space for the 3 fields of the DOT11_BYTE_ARRAY
        //
        if (OutputBufferLength < FIELD_OFFSET(DOT11_BYTE_ARRAY, ucBuffer))
        {
            *BytesNeeded = sizeof(DOT11_BYTE_ARRAY);
            ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;
            break;
        }

        if (InputBufferLength < sizeof(DOT11_COUNTRY_OR_REGION_STRING))
        {
            // Unspecified country/region string
            NdisZeroMemory(countryRegionString, sizeof(DOT11_COUNTRY_OR_REGION_STRING));
        }
        else
        {
            // Copy the country/region string from the OS
            NdisMoveMemory(countryRegionString, InformationBuffer, sizeof(DOT11_COUNTRY_OR_REGION_STRING));
        }

        dot11ByteArray = (PDOT11_BYTE_ARRAY)InformationBuffer;

        dot11ByteArray->uNumOfBytes = 0;
        dot11ByteArray->uTotalNumOfBytes = 0;

        // Base port copies the BSS list
        ndisStatus = BasePortCopyBSSList(
                        AP_GET_MP_PORT(ApPort),
                        countryRegionString,
                        750000000,                      // Default expiration time
                        dot11ByteArray,
                        OutputBufferLength
                        );

        *BytesRead = sizeof(DOT11_COUNTRY_OR_REGION_STRING);

        *BytesWritten = dot11ByteArray->uNumOfBytes + 
            FIELD_OFFSET(DOT11_BYTE_ARRAY, ucBuffer);
            
        *BytesNeeded = dot11ByteArray->uTotalNumOfBytes +
            FIELD_OFFSET(DOT11_BYTE_ARRAY, ucBuffer);
    } while(FALSE);

    return ndisStatus;
}

