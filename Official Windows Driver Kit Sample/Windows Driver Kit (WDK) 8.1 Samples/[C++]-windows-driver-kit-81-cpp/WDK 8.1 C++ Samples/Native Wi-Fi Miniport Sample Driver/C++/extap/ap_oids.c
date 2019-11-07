/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    ap_oids.c

Abstract:
    ExtAP OIDs processing (validate buffer and call corresponding strong type APIs)
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-13-2007    Created

Notes:
    These functions only validate the buffer size. 
    The strong type APIs need to validate the buffer content.
    
--*/

#include "precomp.h"
#include "ap_oidapi.h"

#if DOT11_TRACE_ENABLED
#include "ap_oids.tmh"
#endif

/** commonly used MACROs */

/** initialize output parameters BytesWritten and BytesNeeded for query */
#define INIT_BYTE_PARAMETERS_FOR_QUERY() \
    { \
        *BytesWritten = 0; \
        *BytesNeeded = 0; \
    }

/** initialize output parameters BytesRead and BytesNeeded for set */
#define INIT_BYTE_PARAMETERS_FOR_SET() \
    { \
        *BytesRead = 0; \
        *BytesNeeded = 0; \
    }

      
/** OID_DOT11_AUTO_CONFIG_ENABLED */
NDIS_STATUS
ApOidQueryAutoConfigEnabled(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_QUERY();

        if (InformationBufferLength < sizeof(ULONG))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(ULONG));
        }

        ndisStatus = ApQueryAutoConfigEnabled(
                    ApPort, 
                    (PULONG)InformationBuffer
                    );

        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesWritten = sizeof(ULONG);
        }
    } while (FALSE);

    return ndisStatus;
}

NDIS_STATUS
ApOidSetAutoConfigEnabled(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_SET();

        if (InformationBufferLength < sizeof(ULONG))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(ULONG));
        }

        ndisStatus = ApSetAutoConfigEnabled(
                    ApPort, 
                    *(PULONG)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesRead = sizeof(ULONG);
        }
    } while (FALSE);

    return ndisStatus;
}

#if 0       // remove it when confirmed
/** OID_DOT11_NIC_POWER_STATE */
NDIS_STATUS
ApOidQueryNicPowerState(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_QUERY();

        if (InformationBufferLength < sizeof(BOOLEAN))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(BOOLEAN));
        }

        ndisStatus = ApQueryNicPowerState(
                    ApPort, 
                    (BOOLEAN *)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesWritten = sizeof(BOOLEAN);
        }
    } while (FALSE);
    
    return ndisStatus;
}
    
NDIS_STATUS
ApOidSetNicPowerState(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_SET();

        if (InformationBufferLength < sizeof(BOOLEAN))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(BOOLEAN));
        }

        ndisStatus = ApSetNicPowerState(
                    ApPort, 
                    *(BOOLEAN *)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesRead = sizeof(BOOLEAN);
        }
    } while (FALSE);

    return ndisStatus;
}
#endif

/** OID_DOT11_OPERATIONAL_RATE_SET */
NDIS_STATUS
ApOidQueryOperationalRateSet(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_QUERY();

        if (InformationBufferLength < sizeof(DOT11_RATE_SET))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(DOT11_RATE_SET));
        }

        ndisStatus = ApQueryOperationalRateSet(
                    ApPort, 
                    (PDOT11_RATE_SET)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesWritten = sizeof(DOT11_RATE_SET);
        }
    } while (FALSE);

    return ndisStatus;
}
    

NDIS_STATUS
ApOidSetOperationalRateSet(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_SET();

        if (InformationBufferLength < sizeof(DOT11_RATE_SET))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(DOT11_RATE_SET));
        }

        if (((PDOT11_RATE_SET)InformationBuffer)->uRateSetLength > DOT11_RATE_SET_MAX_LENGTH)
        {
            ndisStatus = NDIS_STATUS_INVALID_PARAMETER;
            break;
        }

        ndisStatus = ApSetOperationalRateSet(
                    ApPort, 
                    (PDOT11_RATE_SET)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesRead = sizeof(DOT11_RATE_SET);
        }
    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_BEACON_PERIOD */
NDIS_STATUS
ApOidQueryBeaconPeriod(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_QUERY();

        if (InformationBufferLength < sizeof(ULONG))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(ULONG));
        }

        ndisStatus = ApQueryBeaconPeriod(
                    ApPort, 
                    (PULONG)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesWritten = sizeof(ULONG);
        }
    } while (FALSE);

    return ndisStatus;
}

NDIS_STATUS
ApOidSetBeaconPeriod(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_SET();

        if (InformationBufferLength < sizeof(ULONG))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(ULONG));
        }

        ndisStatus = ApSetBeaconPeriod(
                    ApPort, 
                    *(PULONG)InformationBuffer
                    );
            
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesRead = sizeof(ULONG);
        }
    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_DTIM_PERIOD */
NDIS_STATUS
ApOidQueryDTimPeriod(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_QUERY();

        if (InformationBufferLength < sizeof(ULONG))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(ULONG));
        }

        ndisStatus = ApQueryDTimPeriod(
                    ApPort, 
                    (PULONG)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesWritten = sizeof(ULONG);
        }
    } while (FALSE);

    return ndisStatus;
}

NDIS_STATUS
ApOidSetDTimPeriod(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_SET();

        if (InformationBufferLength < sizeof(ULONG))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(ULONG));
        }

        ndisStatus = ApSetDTimPeriod(
                    ApPort, 
                    *(PULONG)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesRead = sizeof(ULONG);
        }
    } while (FALSE);

    return ndisStatus;
}

#if 0       // remove it when confirmed
/** OID_DOT11_RTS_THRESHOLD */
NDIS_STATUS
ApOidQueryRtsThreshold(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_QUERY();

        if (InformationBufferLength < sizeof(ULONG))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(ULONG));
        }

        ndisStatus = ApQueryRtsThreshold(
                    ApPort, 
                    (PULONG)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesWritten = sizeof(ULONG);
        }
    } while (FALSE);

    return ndisStatus;
}

NDIS_STATUS
ApOidSetRtsThreshold(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_SET();

        if (InformationBufferLength < sizeof(ULONG))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(ULONG));
        }

        ndisStatus = ApSetRtsThreshold(
                    ApPort, 
                    *(PULONG)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesRead = sizeof(ULONG);
        }
    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_SHORT_RETRY_LIMIT */
NDIS_STATUS
ApOidQueryShortRetryLimit(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_QUERY();

        if (InformationBufferLength < sizeof(ULONG))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(ULONG));
        }

        ndisStatus = ApQueryShortRetryLimit(
                    ApPort, 
                    (PULONG)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesWritten = sizeof(ULONG);
        }
    } while (FALSE);

    return ndisStatus;
}

NDIS_STATUS
ApOidSetShortRetryLimit(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_SET();

        if (InformationBufferLength < sizeof(ULONG))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(ULONG));
        }

        ndisStatus = ApSetShortRetryLimit(
                    ApPort, 
                    *(PULONG)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesRead = sizeof(ULONG);
        }
    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_LONG_RETRY_LIMIT */
NDIS_STATUS
ApOidQueryLongRetryLimit(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_QUERY();

        if (InformationBufferLength < sizeof(ULONG))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(ULONG));
        }

        ndisStatus = ApQueryLongRetryLimit(
                    ApPort, 
                    (PULONG)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesWritten = sizeof(ULONG);
        }
    } while (FALSE);
    
    return ndisStatus;
}

NDIS_STATUS
ApOidSetLongRetryLimit(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_SET();

        if (InformationBufferLength < sizeof(ULONG))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(ULONG));
        }

        ndisStatus = ApSetLongRetryLimit(
                    ApPort, 
                    *(PULONG)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesRead = sizeof(ULONG);
        }
    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_FRAGMENTATION_THRESHOLD */
NDIS_STATUS
ApOidQueryFragmentationThreshold(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_QUERY();

        if (InformationBufferLength < sizeof(ULONG))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(ULONG));
        }

        ndisStatus = ApQueryFragmentationThreshold(
                    ApPort, 
                    (PULONG)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesWritten = sizeof(ULONG);
        }
    } while (FALSE);

    return ndisStatus;
}

NDIS_STATUS
ApOidSetFragmentationThreshold(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_SET();

        if (InformationBufferLength < sizeof(ULONG))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(ULONG));
        }

        ndisStatus = ApSetFragmentationThreshold(
                    ApPort, 
                    *(PULONG)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesRead = sizeof(ULONG);
        }
    } while (FALSE);

    return ndisStatus;
}
#endif

/** OID_DOT11_AVAILABLE_CHANNEL_LIST */
NDIS_STATUS
ApOidQueryAvailableChannelList(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    _Analysis_assume_( sizeof(DOT11_AVAILABLE_CHANNEL_LIST) <= InformationBufferLength );

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_QUERY();

        // TODO: check buffer
        if (InformationBufferLength < sizeof(ULONG))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(ULONG));
        }

        ndisStatus = ApQueryAvailableChannelList(
                    ApPort, 
                    (PDOT11_AVAILABLE_CHANNEL_LIST)InformationBuffer
                    );

        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesWritten = sizeof(ULONG);
        }
    } while (FALSE);

    return ndisStatus;
}

/** Set not applicable */

/** OID_DOT11_CURRENT_CHANNEL */
NDIS_STATUS
ApOidQueryCurrentChannel(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_QUERY();

        if (InformationBufferLength < sizeof(ULONG))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(ULONG));
        }

        ndisStatus = ApQueryCurrentChannel(
                    ApPort, 
                    (PULONG)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesWritten = sizeof(ULONG);
        }
    } while (FALSE);

    return ndisStatus;
}

NDIS_STATUS
ApOidSetCurrentChannel(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_SET();

        if (InformationBufferLength < sizeof(ULONG))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(ULONG));
        }

        // we set the channel only if we are in Init state. The spec allows us to ignore
        // the channel given through set channel in OP state if the miniport driver is aware
        // of other conditions that makes it impossible to switch channels and maintain service.
        if (ApGetState(ApPort) == AP_STATE_STOPPED)
        {
            ndisStatus = ApSetCurrentChannel(
                        ApPort, 
                        *(PULONG)InformationBuffer
                        );
        }
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesRead = sizeof(ULONG);
        }
    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_AVAILABLE_FREQUENCY_LIST */
NDIS_STATUS
ApOidQueryAvailableFrequencyList(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    _Analysis_assume_( sizeof(DOT11_AVAILABLE_FREQUENCY_LIST) <= InformationBufferLength );

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_QUERY();

        // TODO: check buffer
        if (InformationBufferLength < sizeof(ULONG))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(ULONG));
        }

        ndisStatus = ApQueryAvailableFrequencyList(
                    ApPort, 
                    (PDOT11_AVAILABLE_FREQUENCY_LIST)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesWritten = sizeof(ULONG);
        }
    } while (FALSE);

    return ndisStatus;
}

/** Set not applicable */

/** OID_DOT11_CURRENT_FREQUENCY */
NDIS_STATUS
ApOidQueryCurrentFrequency(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_QUERY();

        if (InformationBufferLength < sizeof(ULONG))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(ULONG));
        }

        ndisStatus = ApQueryCurrentFrequency(
                    ApPort, 
                    (PULONG)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesWritten = sizeof(ULONG);
        }
    } while (FALSE);

    return ndisStatus;
}

NDIS_STATUS
ApOidSetCurrentFrequency(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_SET();

        if (InformationBufferLength < sizeof(ULONG))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(ULONG));
        }

        // we set the frequency only if we are in Init state. The spec allows us to ignore
        // the channel given through set channel in OP state if the miniport driver is aware
        // of other conditions that makes it impossible to switch channels and maintain service.
        if (ApGetState(ApPort) == AP_STATE_STOPPED)
        {
            ndisStatus = ApSetCurrentFrequency(
                        ApPort, 
                        *(PULONG)InformationBuffer
                        );
        }
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesRead = sizeof(ULONG);
        }
    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_DESIRED_SSID_LIST */
NDIS_STATUS
ApOidQueryDesiredSsidList(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    _Analysis_assume_( sizeof(DOT11_SSID_LIST) <= InformationBufferLength );

    // init output parameters
    INIT_BYTE_PARAMETERS_FOR_QUERY();

    // let the strong type API check buffer size
    // because the size of SSID list is not known in advance

    ndisStatus = ApQueryDesiredSsidList(
                ApPort, 
                (PDOT11_SSID_LIST)InformationBuffer,
                InformationBufferLength,
                BytesWritten,
                BytesNeeded
                );

    return ndisStatus;
}

NDIS_STATUS
ApOidSetDesiredSsidList(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    ULONG requiredListSize = 0;
    
    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_SET();

        ndisStatus = ValiateSsidListSize(
                        (PDOT11_SSID_LIST)InformationBuffer, 
                        InformationBufferLength,
                        &requiredListSize
                        );

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            if (NDIS_STATUS_INVALID_LENGTH == ndisStatus)
            {
                // set needed size
                *BytesNeeded = requiredListSize;
            }
            
            break;
        }
        
        ndisStatus = ApSetDesiredSsidList(
                    ApPort, 
                    (PDOT11_SSID_LIST)InformationBuffer
                    );

        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesRead = requiredListSize;
        }
    } while (FALSE);
    return ndisStatus;
}

/** OID_DOT11_EXCLUDE_UNENCRYPTED */
NDIS_STATUS
ApOidQueryExcludeUnencrypted(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_QUERY();

        if (InformationBufferLength < sizeof(BOOLEAN))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(BOOLEAN));
        }

        ndisStatus = ApQueryExcludeUnencrypted(
                    ApPort, 
                    (BOOLEAN *)InformationBuffer);
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesWritten = sizeof(BOOLEAN);
        }
    } while (FALSE);

    return ndisStatus;
}

NDIS_STATUS
ApOidSetExcludeUnencrypted(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_SET();

        if (InformationBufferLength < sizeof(BOOLEAN))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(BOOLEAN));
        }

        ndisStatus = ApSetExcludeUnencrypted(
                    ApPort, 
                    *(BOOLEAN *)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesRead = sizeof(BOOLEAN);
        }
    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_STATISTICS */
NDIS_STATUS
ApOidQueryStatistics(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ _In_range_(sizeof(DOT11_STATISTICS), ULONG_MAX) ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    // init output parameters
    INIT_BYTE_PARAMETERS_FOR_QUERY();

    // let the strong type API check buffer size
    // because the size of statistics is not known in advance
    
    ndisStatus = ApQueryStatistics(
                ApPort, 
                (PDOT11_STATISTICS)InformationBuffer,
                InformationBufferLength,
                BytesWritten,
                BytesNeeded
                );

    return ndisStatus;
}

/** Set not applicable */

/** OID_DOT11_PRIVACY_EXEMPTION_LIST */
NDIS_STATUS
ApOidQueryPrivacyExemptionList(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    _Analysis_assume_( sizeof(DOT11_PRIVACY_EXEMPTION_LIST) <= InformationBufferLength );

    // init output parameters
    INIT_BYTE_PARAMETERS_FOR_QUERY();

    // let the strong type API check buffer size
    // because the size of privacy exemption list is not known in advance
    ndisStatus = ApQueryPrivacyExemptionList(
                ApPort, 
                (PDOT11_PRIVACY_EXEMPTION_LIST)InformationBuffer,
                InformationBufferLength,
                BytesWritten,
                BytesNeeded
                );

    return ndisStatus;
}

NDIS_STATUS
ApOidSetPrivacyExemptionList(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    ULONG requiredListSize = 0;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_SET();

        ndisStatus = ValiatePrivacyExemptionListSize(
                        (PDOT11_PRIVACY_EXEMPTION_LIST)InformationBuffer, 
                        InformationBufferLength,
                        &requiredListSize
                        );
        
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            if (NDIS_STATUS_INVALID_LENGTH == ndisStatus)
            {
                // set needed size
                *BytesNeeded = requiredListSize;
            }
            
            break;
        }
        
        ndisStatus = ApSetPrivacyExemptionList(
                    ApPort, 
                    (PDOT11_PRIVACY_EXEMPTION_LIST)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesRead = requiredListSize;
        }
    } while (FALSE);
    
    return ndisStatus;
}

/** OID_DOT11_ENABLED_AUTHENTICATION_ALGORITHM */
NDIS_STATUS
ApOidQueryEnabledAuthenticationAlgorithm(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    _Analysis_assume_( sizeof(DOT11_AUTH_ALGORITHM_LIST) <= InformationBufferLength );

    // init output parameters
    INIT_BYTE_PARAMETERS_FOR_QUERY();

    // let the strong type API check buffer size
    // because the size of auth algorithm list is not known in advance
    ndisStatus = ApQueryEnabledAuthenticationAlgorithm(
                ApPort, 
                (PDOT11_AUTH_ALGORITHM_LIST)InformationBuffer,
                InformationBufferLength,
                BytesWritten,
                BytesNeeded
                );

    return ndisStatus;
}

NDIS_STATUS
ApOidSetEnabledAuthenticationAlgorithm(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    ULONG requiredListSize = 0;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_SET();

        ndisStatus = ValiateAuthAlgorithmListSize(
                        (PDOT11_AUTH_ALGORITHM_LIST)InformationBuffer, 
                        InformationBufferLength,
                        &requiredListSize
                        );
        
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            if (NDIS_STATUS_INVALID_LENGTH == ndisStatus)
            {
                // set needed size
                *BytesNeeded = requiredListSize;
            }
            
            break;
        }
        
        ndisStatus = ApSetEnabledAuthenticationAlgorithm(
                    ApPort, 
                    (PDOT11_AUTH_ALGORITHM_LIST)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesRead = requiredListSize;
        }
    } while (FALSE);
    
    return ndisStatus;
}

/** OID_DOT11_ENABLED_UNICAST_CIPHER_ALGORITHM */
NDIS_STATUS
ApOidQueryEnabledUnicastCipherAlgorithm(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    _Analysis_assume_( sizeof(DOT11_CIPHER_ALGORITHM_LIST) <= InformationBufferLength );

    // init output parameters
    INIT_BYTE_PARAMETERS_FOR_QUERY();

    // let the strong type API check buffer size
    // because the size of cipher algorithm list is not known in advance
    ndisStatus = ApQueryEnabledUnicastCipherAlgorithm(
                ApPort, 
                (PDOT11_CIPHER_ALGORITHM_LIST)InformationBuffer,
                InformationBufferLength,
                BytesWritten,
                BytesNeeded
                );

    return ndisStatus;
}

NDIS_STATUS
ApOidSetEnabledUnicastCipherAlgorithm(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    ULONG requiredListSize = 0;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_SET();

        ndisStatus = ValiateCipherAlgorithmListSize(
                        (PDOT11_CIPHER_ALGORITHM_LIST)InformationBuffer, 
                        InformationBufferLength,
                        &requiredListSize
                        );
        
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            if (NDIS_STATUS_INVALID_LENGTH == ndisStatus)
            {
                // set needed size
                *BytesNeeded = requiredListSize;
            }
            
            break;
        }
        
        ndisStatus = ApSetEnabledUnicastCipherAlgorithm(
                    ApPort, 
                    (PDOT11_CIPHER_ALGORITHM_LIST)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesRead = requiredListSize;
        }
    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_ENABLED_MULTICAST_CIPHER_ALGORITHM */
NDIS_STATUS
ApOidQueryEnabledMulticastCipherAlgorithm(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    _Analysis_assume_( sizeof(DOT11_CIPHER_ALGORITHM_LIST) <= InformationBufferLength );

    // init output parameters
    INIT_BYTE_PARAMETERS_FOR_QUERY();

    // let the strong type API check buffer size
    // because the size of cipher algorithm list is not known in advance
    ndisStatus = ApQueryEnabledMulticastCipherAlgorithm(
                ApPort, 
                (PDOT11_CIPHER_ALGORITHM_LIST)InformationBuffer,
                InformationBufferLength,
                BytesWritten,
                BytesNeeded
                );
        
    return ndisStatus;
}

NDIS_STATUS
ApOidSetEnabledMulticastCipherAlgorithm(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    ULONG requiredListSize = 0;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_SET();

        ndisStatus = ValiateCipherAlgorithmListSize(
                        (PDOT11_CIPHER_ALGORITHM_LIST)InformationBuffer, 
                        InformationBufferLength,
                        &requiredListSize
                        );
        
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            if (NDIS_STATUS_INVALID_LENGTH == ndisStatus)
            {
                // set needed size
                *BytesNeeded = requiredListSize;
            }
            
            break;
        }
        
        ndisStatus = ApSetEnabledMulticastCipherAlgorithm(
                    ApPort, 
                    (PDOT11_CIPHER_ALGORITHM_LIST)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesRead = requiredListSize;
        }
    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_CIPHER_DEFAULT_KEY_ID */
NDIS_STATUS
ApOidQueryCipherDefaultKeyId(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_QUERY();

        if (InformationBufferLength < sizeof(ULONG))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(ULONG));
        }

        ndisStatus = ApQueryCipherDefaultKeyId(
                    ApPort, 
                    (PULONG)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesWritten = sizeof(ULONG);
        }
    } while (FALSE);

    return ndisStatus;
}

NDIS_STATUS
ApOidSetCipherDefaultKeyId(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_SET();

        if (InformationBufferLength < sizeof(ULONG))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(ULONG));
        }

        ndisStatus = ApSetCipherDefaultKeyId(
                    ApPort, 
                    *(PULONG)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesRead = sizeof(ULONG);
        }
    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_CIPHER_DEFAULT_KEY */
/** Query not applicable */

NDIS_STATUS
ApOidSetCipherDefaultKey(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    ULONG requiredSize = 0;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_SET();

        ndisStatus = ValiateCipherDefaultKeySize(
                        (PDOT11_CIPHER_DEFAULT_KEY_VALUE)InformationBuffer, 
                        InformationBufferLength,
                        &requiredSize
                        );
        
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            if (NDIS_STATUS_INVALID_LENGTH == ndisStatus)
            {
                // set needed size
                *BytesNeeded = requiredSize;
            }
            
            break;
        }
        
        ndisStatus = ApSetCipherDefaultKey(
                    ApPort, 
                    (PDOT11_CIPHER_DEFAULT_KEY_VALUE)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesRead = requiredSize;
        }
    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_CIPHER_KEY_MAPPING_KEY */
/** Query not applicable */

NDIS_STATUS
ApOidSetCipherKeyMappingKey(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    ULONG requiredSize = 0;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_SET();

        ndisStatus = ValidateDot11ByteArray(
                        (PDOT11_BYTE_ARRAY)InformationBuffer, 
                        InformationBufferLength,
                        &requiredSize
                        );
        
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            if (NDIS_STATUS_INVALID_LENGTH == ndisStatus)
            {
                // set needed size
                *BytesNeeded = requiredSize;
            }
            
            break;
        }
        
        ndisStatus = ApSetCipherKeyMappingKey(
                    ApPort, 
                    (PDOT11_BYTE_ARRAY)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesRead = requiredSize;
        }
    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_ENUM_PEER_INFO */
NDIS_STATUS
ApOidEnumPeerInfo(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    _Analysis_assume_( sizeof(DOT11_PEER_INFO_LIST) <= InformationBufferLength );

    // init output parameters
    INIT_BYTE_PARAMETERS_FOR_QUERY();

    // let the strong type API check buffer size
    // because the size of association info list is not known in advance
    ndisStatus = ApEnumPeerInfo(
                ApPort, 
                (PDOT11_PEER_INFO_LIST)InformationBuffer,
                InformationBufferLength,
                BytesWritten,
                BytesNeeded
                );
            
    return ndisStatus;
}

/** Set not applicable */

/** OID_DOT11_DISASSOCIATE_REQUEST */
/** Query not applicable */

NDIS_STATUS
ApOidDisassociatePeerRequest(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_SET();

        if (InformationBufferLength < sizeof(DOT11_DISASSOCIATE_PEER_REQUEST))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(DOT11_DISASSOCIATE_PEER_REQUEST));
        }

        ndisStatus = ApDisassociatePeerRequest(
                    ApPort, 
                    (PDOT11_DISASSOCIATE_PEER_REQUEST)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesRead = sizeof(DOT11_DISASSOCIATE_PEER_REQUEST);
        }
    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_DESIRED_PHY_LIST */
NDIS_STATUS
ApOidQueryDesiredPhyList(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    _Analysis_assume_( sizeof(DOT11_PHY_ID_LIST) <= InformationBufferLength );

    // init output parameters
    INIT_BYTE_PARAMETERS_FOR_QUERY();

    // let the strong type API check buffer size
    // because the size of PHY list is not known in advance
    ndisStatus = ApQueryDesiredPhyList(
                ApPort, 
                (PDOT11_PHY_ID_LIST)InformationBuffer,
                InformationBufferLength,
                BytesWritten,
                BytesNeeded
                );

    return ndisStatus;
}

NDIS_STATUS
ApOidSetDesiredPhyList(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    ULONG requiredListSize = 0;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_SET();

        ndisStatus = ValiatePhyIdListSize(
                        (PDOT11_PHY_ID_LIST)InformationBuffer, 
                        InformationBufferLength,
                        &requiredListSize
                        );
        
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            if (NDIS_STATUS_INVALID_LENGTH == ndisStatus)
            {
                // set needed size
                *BytesNeeded = requiredListSize;
            }
            
            break;
        }
        
        ndisStatus = ApSetDesiredPhyList(
                    ApPort, 
                    (PDOT11_PHY_ID_LIST)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesRead = requiredListSize;
        }
    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_CURRENT_PHY_ID */
NDIS_STATUS
ApOidQueryCurrentPhyId(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_QUERY();

        if (InformationBufferLength < sizeof(ULONG))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(ULONG));
        }

        ndisStatus = ApQueryCurrentPhyId(
                    ApPort, 
                    (PULONG)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesWritten = sizeof(ULONG);
        }
    } while (FALSE);

    return ndisStatus;
}

NDIS_STATUS
ApOidSetCurrentPhyId(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_SET();

        if (InformationBufferLength < sizeof(ULONG))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(ULONG));
        }

        ndisStatus = ApSetCurrentPhyId(
                    ApPort, 
                    *(PULONG)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesRead = sizeof(ULONG);
        }
    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_PORT_STATE_NOTIFICATION */
/** Query not applicable */

NDIS_STATUS
ApOidSetPortStateNotification(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_SET();

        if (InformationBufferLength < sizeof(DOT11_PORT_STATE_NOTIFICATION))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(DOT11_PORT_STATE_NOTIFICATION));
        }

        ndisStatus = ApSetPortStateNotification(
                    ApPort, 
                    (PDOT11_PORT_STATE_NOTIFICATION)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesRead = sizeof(DOT11_PORT_STATE_NOTIFICATION);
        }
    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_SCAN_REQUEST */
/** Query not applicable */
#define MIN_SCAN_REQUEST_SIZE FIELD_OFFSET(DOT11_SCAN_REQUEST_V2, ucBuffer)

NDIS_STATUS
ApOidScanRequest(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_SET();

        if (InformationBufferLength < MIN_SCAN_REQUEST_SIZE)
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(MIN_SCAN_REQUEST_SIZE);
        }

        ndisStatus = ApScanRequest(
                    ApPort, 
                    AP_GET_MP_PORT(ApPort)->PendingOidRequest->RequestId,
                    (PDOT11_SCAN_REQUEST_V2)InformationBuffer,
                    InformationBufferLength
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesRead = InformationBufferLength;
        }
    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_INCOMING_ASSOCIATION_DECISION */
/** Query not applicable */

NDIS_STATUS
ApOidSetIncomingAssociationDecision(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_INCOMING_ASSOC_DECISION pAssocDecision = (PDOT11_INCOMING_ASSOC_DECISION) InformationBuffer;
    ULONG ulExpectedBufferSize = sizeof(DOT11_INCOMING_ASSOC_DECISION);

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_SET();

        if (InformationBufferLength < sizeof(DOT11_INCOMING_ASSOC_DECISION))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(DOT11_INCOMING_ASSOC_DECISION));
        }

        if ( pAssocDecision->uAssocResponseIEsLength )
        {
            ndisStatus = RtlULongAdd(
                        pAssocDecision->uAssocResponseIEsOffset,
                        pAssocDecision->uAssocResponseIEsLength,
                        &ulExpectedBufferSize
                        );
            if (NDIS_STATUS_SUCCESS != ndisStatus)
            {
                break;
            }

            if (ulExpectedBufferSize < sizeof(DOT11_INCOMING_ASSOC_DECISION))
            {
                // Unexpected buffer from OS.
                MPASSERT(FALSE);
                ulExpectedBufferSize = sizeof(DOT11_INCOMING_ASSOC_DECISION);
            }

            if (InformationBufferLength < ulExpectedBufferSize)
            {
                SET_NEEDED_BUFFER_SIZE_AND_BREAK(ulExpectedBufferSize);
            }
        }

        ndisStatus = ApSetIncomingAssociationDecision(
                    ApPort, 
                    pAssocDecision
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesRead = ulExpectedBufferSize;
        }
    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_ADDITIONAL_IE */
NDIS_STATUS
ApOidQueryAdditionalIe(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ _In_range_(sizeof(DOT11_ADDITIONAL_IE), ULONG_MAX) ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded    
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    // init output parameters
    INIT_BYTE_PARAMETERS_FOR_QUERY();

    // let the strong type API check buffer size
    // because the size of addition IEs is not known in advance
    ndisStatus = ApQueryAdditionalIe(
                ApPort, 
                (PDOT11_ADDITIONAL_IE)InformationBuffer,
                InformationBufferLength,
                BytesWritten,
                BytesNeeded
                );
        
    return ndisStatus;
}

NDIS_STATUS
ApOidSetAdditionalIe(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    ULONG requiredSize = 0;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_SET();

        ndisStatus = ValiateAdditionalIeSize(
                        (PDOT11_ADDITIONAL_IE)InformationBuffer, 
                        InformationBufferLength,
                        &requiredSize
                        );
        
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            if (NDIS_STATUS_INVALID_LENGTH == ndisStatus)
            {
                // set needed size
                *BytesNeeded = requiredSize;
            }
            
            break;
        }
        
        ndisStatus = ApSetAdditionalIe(
                    ApPort, 
                    (PDOT11_ADDITIONAL_IE)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesRead = requiredSize;
        }
    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_WPS_ENABLED */
NDIS_STATUS
ApOidQueryWpsEnabled(
    _In_ PMP_EXTAP_PORT ApPort,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_QUERY();

        if (InformationBufferLength < sizeof(BOOLEAN))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(BOOLEAN));
        }

        ndisStatus = ApQueryWpsEnabled(
                    ApPort, 
                    (BOOLEAN *)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesWritten = sizeof(BOOLEAN);
        }
    } while (FALSE);

    return ndisStatus;
}

NDIS_STATUS
ApOidSetWpsEnabled(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_SET();

        if (InformationBufferLength < sizeof(BOOLEAN))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(BOOLEAN));
        }

        ndisStatus = ApSetWpsEnabled(
                    ApPort, 
                    *(BOOLEAN *)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesRead = sizeof(BOOLEAN);
        }
    } while (FALSE);

    return ndisStatus;
}




/** OID_DOT11_START_AP_REQUEST */
/** Query not applicable */

NDIS_STATUS
ApOidStartApRequest(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    UNREFERENCED_PARAMETER(InformationBuffer);
    UNREFERENCED_PARAMETER(InformationBufferLength);
    
    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_SET();

        // no parameter for starting AP
        ndisStatus = ApStartApRequest(ApPort);
    } while (FALSE);

    return ndisStatus;
}

/** OID_GEN_CURRENT_PACKET_FILTER */
/** Query is handled by the base port */

NDIS_STATUS
ApOidSetPacketFilter(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        // init output parameters
        INIT_BYTE_PARAMETERS_FOR_SET();

        if (InformationBufferLength < sizeof(ULONG))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(ULONG));
        }

        ndisStatus = ApSetPacketFilter(
                    ApPort, 
                    *(PULONG)InformationBuffer
                    );
        
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            *BytesRead = sizeof(ULONG);
        }
    } while (FALSE);

    return ndisStatus;
}

NDIS_STATUS
ApDot11ResetHandler(
    _In_  PMP_PORT                Port,
    _In_  PVOID                   NdisOidRequest
    )    
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PMP_EXTAP_PORT apPort = MP_GET_AP_PORT(Port);
    PNDIS_OID_REQUEST oidRequest = (PNDIS_OID_REQUEST)NdisOidRequest;
    DOT11_RESET_REQUEST resetRequest = {0};

    // make a copy of the request before calling the strong type API
    // because we don't want to use the same input/output in that API
    RtlCopyMemory(
            &resetRequest,
            oidRequest->DATA.METHOD_INFORMATION.InformationBuffer,
            sizeof(DOT11_RESET_REQUEST)
            );
    
    ndisStatus = ApResetRequest(
                    apPort, 
                    &resetRequest,
                    (PDOT11_STATUS_INDICATION)oidRequest->DATA.METHOD_INFORMATION.InformationBuffer
                    );

    if (NDIS_STATUS_SUCCESS == ndisStatus)
    {
        oidRequest->DATA.METHOD_INFORMATION.BytesWritten = sizeof(DOT11_STATUS_INDICATION);
    }

    return ndisStatus;
}

/** Methods */
/** OID_DOT11_RESET_REQUEST */
NDIS_STATUS
ApOidResetRequest(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PNDIS_OID_REQUEST NdisOidRequest
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PULONG BytesNeeded = (PULONG)&NdisOidRequest->DATA.METHOD_INFORMATION.BytesNeeded;

    do
    {
        // init output parameters
        NdisOidRequest->DATA.METHOD_INFORMATION.BytesRead = NdisOidRequest->DATA.METHOD_INFORMATION.BytesWritten = 0;

        // check output buffer
        if (NdisOidRequest->DATA.METHOD_INFORMATION.OutputBufferLength < sizeof(DOT11_STATUS_INDICATION))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(DOT11_STATUS_INDICATION));
        }

        // check input buffer
        if (NdisOidRequest->DATA.METHOD_INFORMATION.InputBufferLength < sizeof(DOT11_RESET_REQUEST))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(DOT11_RESET_REQUEST));
        }

        NdisOidRequest->DATA.METHOD_INFORMATION.BytesRead = sizeof(DOT11_RESET_REQUEST);

        ndisStatus = ApDot11ResetHandler(
            AP_GET_MP_PORT(ApPort),
            NdisOidRequest
            );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }
    } while (FALSE);

    return ndisStatus;
}

/** OID_DOT11_ENUM_BSS_LIST */
// TODO: move this OID to base port
NDIS_STATUS
ApOidEnumerateBSSList(
    _In_ PMP_EXTAP_PORT ApPort,
    _Inout_ PVOID InformationBuffer,
    _In_ ULONG InputBufferLength,
    _In_ ULONG OutputBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    return ApEnumerateBSSList(
                ApPort, 
                InformationBuffer, 
                InputBufferLength, 
                OutputBufferLength, 
                BytesRead, 
                BytesWritten, 
                BytesNeeded
                );
}

/** OID query */
_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
ApQueryInformation(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ NDIS_OID Oid,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    MPASSERT(ApPort != NULL);
    // may pass in a null buffer to get the required size
    MPASSERT(InformationBufferLength == 0 || InformationBuffer != NULL);
    MPASSERT(BytesWritten != NULL);
    MPASSERT(BytesNeeded != NULL);
    
    MpTrace(COMP_OID, DBG_TRACE,  ("Port(%u): Querying OID: 0x%08x\n", 
                        AP_GET_PORT_NUMBER(ApPort), Oid));

    switch (Oid)
    {
        case OID_DOT11_AUTO_CONFIG_ENABLED:
            {
                ndisStatus = ApOidQueryAutoConfigEnabled(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesWritten, 
                                BytesNeeded
                                );
            }
            break;
#if 0
        case OID_DOT11_NIC_POWER_STATE:
            {
                ndisStatus = ApOidQueryNicPowerState(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesWritten, 
                                BytesNeeded
                                );
            }
            break;
#endif    
        case OID_DOT11_OPERATIONAL_RATE_SET:
            {
                ndisStatus = ApOidQueryOperationalRateSet(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesWritten, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_BEACON_PERIOD:
            {
                ndisStatus = ApOidQueryBeaconPeriod(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesWritten, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_DTIM_PERIOD:
            {
                ndisStatus = ApOidQueryDTimPeriod(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesWritten, 
                                BytesNeeded
                                );
            }        
            break;
#if 0            
        case OID_DOT11_RTS_THRESHOLD:
            {
                ndisStatus = ApOidQueryRtsThreshold(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesWritten, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_SHORT_RETRY_LIMIT:
            {
                ndisStatus = ApOidQueryShortRetryLimit(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesWritten, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_LONG_RETRY_LIMIT:
            {
                ndisStatus = ApOidQueryLongRetryLimit(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesWritten, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_FRAGMENTATION_THRESHOLD:
            {
                ndisStatus = ApOidQueryFragmentationThreshold(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesWritten, 
                                BytesNeeded
                                );
            }
            break;
#endif            
        case OID_DOT11_AVAILABLE_CHANNEL_LIST:
            {
                ndisStatus = ApOidQueryAvailableChannelList(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesWritten, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_CURRENT_CHANNEL:
            {
                ndisStatus = ApOidQueryCurrentChannel(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesWritten, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_AVAILABLE_FREQUENCY_LIST:
            {
                ndisStatus = ApOidQueryAvailableFrequencyList(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesWritten, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_CURRENT_FREQUENCY:
            {
                ndisStatus = ApOidQueryCurrentFrequency(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesWritten, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_DESIRED_SSID_LIST:
            {
                ndisStatus = ApOidQueryDesiredSsidList(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesWritten, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_EXCLUDE_UNENCRYPTED:
            {
                ndisStatus = ApOidQueryExcludeUnencrypted(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesWritten, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_PRIVACY_EXEMPTION_LIST:
            {
                ndisStatus = ApOidQueryPrivacyExemptionList(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesWritten, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_STATISTICS:
            {
                ndisStatus = ApOidQueryStatistics(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesWritten, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_ENABLED_AUTHENTICATION_ALGORITHM:
            {
                ndisStatus = ApOidQueryEnabledAuthenticationAlgorithm(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesWritten, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_ENABLED_UNICAST_CIPHER_ALGORITHM:
            {
                ndisStatus = ApOidQueryEnabledUnicastCipherAlgorithm(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesWritten, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_ENABLED_MULTICAST_CIPHER_ALGORITHM:
            {
                ndisStatus = ApOidQueryEnabledMulticastCipherAlgorithm(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesWritten, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_CIPHER_DEFAULT_KEY_ID:
            {
                ndisStatus = ApOidQueryCipherDefaultKeyId(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesWritten, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_ENUM_PEER_INFO:
            {
                ndisStatus = ApOidEnumPeerInfo(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesWritten, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_DESIRED_PHY_LIST:
            {
                ndisStatus = ApOidQueryDesiredPhyList(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesWritten, 
                                BytesNeeded
                                );
            }        
            break;
            
        case OID_DOT11_CURRENT_PHY_ID:
            {
                ndisStatus = ApOidQueryCurrentPhyId(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesWritten, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_ADDITIONAL_IE:
            {
                ndisStatus = ApOidQueryAdditionalIe(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesWritten, 
                                BytesNeeded
                                );
            }
        
            break;
            
        case OID_DOT11_WPS_ENABLED:
            {
                ndisStatus = ApOidQueryWpsEnabled(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesWritten, 
                                BytesNeeded
                                );
            }
            break;
            
            
        /** OID_DOT11_NIC_POWER_STATE */
        /** OID_DOT11_CURRENT_OPERATION_MODE */
        /** OID_DOT11_FLUSH_BSS_LIST */
        /** OID_DOT11_SCAN_REQUEST */
        /** OID_DOT11_RTS_THRESHOLD */
        /** OID_DOT11_SHORT_RETRY_LIMIT */
        /** OID_DOT11_LONG_RETRY_LIMIT */
        /** OID_DOT11_FRAGMENTATION_THRESHOLD */
        /** Handled by base port */
        case OID_DOT11_NIC_POWER_STATE:
        case OID_DOT11_CURRENT_OPERATION_MODE:
        case OID_DOT11_FLUSH_BSS_LIST:
        case OID_DOT11_SCAN_REQUEST:
        case OID_DOT11_RTS_THRESHOLD:
        case OID_DOT11_SHORT_RETRY_LIMIT:
        case OID_DOT11_LONG_RETRY_LIMIT:
        case OID_DOT11_FRAGMENTATION_THRESHOLD:
        default:
            // Not recognized OIDs go to the BasePort for processing
            ndisStatus = NDIS_STATUS_NOT_RECOGNIZED;
            break;
    }


    MpTrace(COMP_OID, DBG_NORMAL,  ("Port(%u): ExtAP OID query completed! OID 0x%08x, ndisStatus = 0x%08x\n",
                            AP_GET_PORT_NUMBER(ApPort), Oid, ndisStatus));

    return ndisStatus;
}

/** OID set */
_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
ApSetInformation(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ NDIS_OID Oid,
    _In_reads_bytes_(InformationBufferLength) PVOID InformationBuffer,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesRead,
    _Out_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    MPASSERT(ApPort != NULL);
    // may pass in a null buffer
    MPASSERT(InformationBufferLength == 0 || InformationBuffer != NULL);
    MPASSERT(BytesRead != NULL);
    MPASSERT(BytesNeeded != NULL);

    MpTrace(COMP_OID, DBG_TRACE,  ("Port(%u): Setting OID: 0x%08x\n", AP_GET_PORT_NUMBER(ApPort), Oid));
    
    switch (Oid)
    {
        case OID_GEN_CURRENT_PACKET_FILTER:
            {
                ndisStatus = ApOidSetPacketFilter(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesRead, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_AUTO_CONFIG_ENABLED:
            {
                ndisStatus = ApOidSetAutoConfigEnabled(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesRead, 
                                BytesNeeded
                                );
            }
            break;
#if 0            
        case OID_DOT11_NIC_POWER_STATE:
            {
                ndisStatus = ApOidSetNicPowerState(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesRead, 
                                BytesNeeded
                                );
            }
            break;
#endif            
        case OID_DOT11_OPERATIONAL_RATE_SET:
            {
                ndisStatus = ApOidSetOperationalRateSet(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesRead, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_BEACON_PERIOD:
            {
                ndisStatus = ApOidSetBeaconPeriod(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesRead, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_DTIM_PERIOD:
            {
                ndisStatus = ApOidSetDTimPeriod(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesRead, 
                                BytesNeeded
                                );
            }
            break;
#if 0            
        case OID_DOT11_RTS_THRESHOLD:
            {
                ndisStatus = ApOidSetRtsThreshold(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesRead, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_SHORT_RETRY_LIMIT:
            {
                ndisStatus = ApOidSetShortRetryLimit(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesRead, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_LONG_RETRY_LIMIT:
            {
                ndisStatus = ApOidSetLongRetryLimit(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesRead, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_FRAGMENTATION_THRESHOLD:
            {
                ndisStatus = ApOidSetFragmentationThreshold(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesRead, 
                                BytesNeeded
                                );
            }
            break;
#endif            
        case OID_DOT11_CURRENT_CHANNEL:
            {
                ndisStatus = ApOidSetCurrentChannel(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesRead, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_CURRENT_FREQUENCY:
            {
                ndisStatus = ApOidSetCurrentFrequency(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesRead, 
                                BytesNeeded
                                );
            }        
            break;
            
        case OID_DOT11_DESIRED_SSID_LIST:
            {
                ndisStatus = ApOidSetDesiredSsidList(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesRead, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_EXCLUDE_UNENCRYPTED:
            {
                ndisStatus = ApOidSetExcludeUnencrypted(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesRead, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_PRIVACY_EXEMPTION_LIST:
            {
                ndisStatus = ApOidSetPrivacyExemptionList(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesRead, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_ENABLED_AUTHENTICATION_ALGORITHM:
            {
                ndisStatus = ApOidSetEnabledAuthenticationAlgorithm(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesRead, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_ENABLED_UNICAST_CIPHER_ALGORITHM:
            {
                ndisStatus = ApOidSetEnabledUnicastCipherAlgorithm(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesRead, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_ENABLED_MULTICAST_CIPHER_ALGORITHM:
            {
                ndisStatus = ApOidSetEnabledMulticastCipherAlgorithm(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesRead, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_CIPHER_DEFAULT_KEY_ID:
            {
                ndisStatus = ApOidSetCipherDefaultKeyId(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesRead, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_CIPHER_DEFAULT_KEY:
            {
                ndisStatus = ApOidSetCipherDefaultKey(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesRead, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_CIPHER_KEY_MAPPING_KEY:
            {
                ndisStatus = ApOidSetCipherKeyMappingKey(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesRead, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_DISASSOCIATE_PEER_REQUEST:
            {
                ndisStatus = ApOidDisassociatePeerRequest(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesRead, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_DESIRED_PHY_LIST:
            {
                ndisStatus = ApOidSetDesiredPhyList(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesRead, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_CURRENT_PHY_ID:
            {
                ndisStatus = ApOidSetCurrentPhyId(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesRead, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_PORT_STATE_NOTIFICATION:
            {
                ndisStatus = ApOidSetPortStateNotification(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesRead, 
                                BytesNeeded
                                );
            }
            break;
        
        case OID_DOT11_SCAN_REQUEST:
            {
                ndisStatus = ApOidScanRequest(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesRead, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_INCOMING_ASSOCIATION_DECISION:
            {
                ndisStatus = ApOidSetIncomingAssociationDecision(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesRead, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_ADDITIONAL_IE:
            {
                ndisStatus = ApOidSetAdditionalIe(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesRead, 
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_WPS_ENABLED:
            {
                ndisStatus = ApOidSetWpsEnabled(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesRead, 
                                BytesNeeded
                                );
            }        
            break;
            
            
        case OID_DOT11_START_AP_REQUEST:
            {
                ndisStatus = ApOidStartApRequest(
                                ApPort, 
                                InformationBuffer, 
                                InformationBufferLength, 
                                BytesRead, 
                                BytesNeeded
                                );
            }
            break;
            
        /** OID_DOT11_NIC_POWER_STATE */
        /** OID_DOT11_CURRENT_OPERATION_MODE */
        /** OID_DOT11_FLUSH_BSS_LIST */
        /** OID_DOT11_RTS_THRESHOLD */
        /** OID_DOT11_SHORT_RETRY_LIMIT */
        /** OID_DOT11_LONG_RETRY_LIMIT */
        /** OID_DOT11_FRAGMENTATION_THRESHOLD */
        /** Handled by base port */
        case OID_DOT11_NIC_POWER_STATE:
        case OID_DOT11_CURRENT_OPERATION_MODE:
        case OID_DOT11_FLUSH_BSS_LIST:
        case OID_DOT11_RTS_THRESHOLD:
        case OID_DOT11_SHORT_RETRY_LIMIT:
        case OID_DOT11_LONG_RETRY_LIMIT:
        case OID_DOT11_FRAGMENTATION_THRESHOLD:
        default:
            // Not recognized OIDs go to the BasePort for processing
            ndisStatus = NDIS_STATUS_NOT_RECOGNIZED;
            break;
    }


    MpTrace(COMP_OID, DBG_NORMAL,  ("Port(%u): ExtAP OID set completed! OID 0x%08x, ndisStatus = 0x%08x\n",
                            AP_GET_PORT_NUMBER(ApPort), Oid, ndisStatus));
    
//    MpExit;
    return ndisStatus;
}

/** OID set and query */
NDIS_STATUS
ApQuerySetInformation(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PNDIS_OID_REQUEST NdisOidRequest
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    NDIS_OID oid = NdisOidRequest->DATA.METHOD_INFORMATION.Oid;
    NDIS_PORT_NUMBER portNumber = AP_GET_PORT_NUMBER(ApPort);
    
    MPASSERT(ApPort != NULL);
    MPASSERT(NdisOidRequest->DATA.METHOD_INFORMATION.InformationBuffer != NULL);
    MPASSERT(NdisOidRequest->DATA.METHOD_INFORMATION.InputBufferLength > 0);
    
    MpTrace(COMP_OID, DBG_TRACE,  ("Port(%u): Querying/Setting OID: 0x%08x\n", portNumber, oid));

    switch(oid)
    {
        case OID_DOT11_RESET_REQUEST:
            {
                ndisStatus = ApOidResetRequest(
                                ApPort,
                                NdisOidRequest
                                );
            }
            break;

        case OID_DOT11_ENUM_BSS_LIST:       //TODO: Base port will handle it
            {
                ndisStatus = ApOidEnumerateBSSList(
                                ApPort,
                                NdisOidRequest->DATA.METHOD_INFORMATION.InformationBuffer,
                                NdisOidRequest->DATA.METHOD_INFORMATION.InputBufferLength,
                                NdisOidRequest->DATA.METHOD_INFORMATION.OutputBufferLength,
                                (PULONG)&NdisOidRequest->DATA.METHOD_INFORMATION.BytesRead,
                                (PULONG)&NdisOidRequest->DATA.METHOD_INFORMATION.BytesWritten,
                                (PULONG)&NdisOidRequest->DATA.METHOD_INFORMATION.BytesNeeded
                                );
            }
            break;

        default:
            ndisStatus = NDIS_STATUS_NOT_RECOGNIZED;
            break;
    }

    MpTrace(COMP_OID, DBG_NORMAL,  ("Port(%u): ExtAP OID query/set completed! OID 0x%08x, ndisStatus = 0x%08x\n",
                            portNumber, oid, ndisStatus));
    
    return ndisStatus;
}

/** AP OIDs handling */
NDIS_STATUS
Ap11OidHandler(
    _In_ PMP_PORT Port,
    _In_ PNDIS_OID_REQUEST NdisOidRequest
    )
{

    NDIS_STATUS ndisStatus = NDIS_STATUS_NOT_SUPPORTED;

    MPASSERT(Port != NULL);
    MPASSERT(NdisOidRequest != NULL);
    
    switch(NdisOidRequest->RequestType)
    {
        case NdisRequestQueryInformation:
        case NdisRequestQueryStatistics:
            ndisStatus = ApQueryInformation(
                            MP_GET_AP_PORT(Port),
                            NdisOidRequest->DATA.QUERY_INFORMATION.Oid,
                            NdisOidRequest->DATA.QUERY_INFORMATION.InformationBuffer,
                            NdisOidRequest->DATA.QUERY_INFORMATION.InformationBufferLength,
                            (PULONG)&NdisOidRequest->DATA.QUERY_INFORMATION.BytesWritten,
                            (PULONG)&NdisOidRequest->DATA.QUERY_INFORMATION.BytesNeeded
                            );
            break;

        case NdisRequestSetInformation:
            ndisStatus = ApSetInformation(
                            MP_GET_AP_PORT(Port),
                            NdisOidRequest->DATA.SET_INFORMATION.Oid,
                            NdisOidRequest->DATA.SET_INFORMATION.InformationBuffer,
                            NdisOidRequest->DATA.SET_INFORMATION.InformationBufferLength,
                            (PULONG)&NdisOidRequest->DATA.SET_INFORMATION.BytesRead,
                            (PULONG)&NdisOidRequest->DATA.SET_INFORMATION.BytesNeeded
                            );
            break;

        case NdisRequestMethod:
            ndisStatus = ApQuerySetInformation(
                            MP_GET_AP_PORT(Port),
                            NdisOidRequest
                            );
            break;


        default:
            ndisStatus = NDIS_STATUS_NOT_RECOGNIZED;
            break;
    }

    if (NDIS_STATUS_NOT_RECOGNIZED == ndisStatus)
    {
        // Let the base port process it
        ndisStatus = BasePortOidHandler(Port, NdisOidRequest);
    }
    else if (ndisStatus != NDIS_STATUS_SUCCESS && ndisStatus != NDIS_STATUS_PENDING)
    {
        // OID has failed here
        MpTrace(COMP_OID, DBG_NORMAL, ("Port(%u): NDIS_OID_REQUEST failed in ExtAP Port. Status = 0x%08x\n", 
                                Port->PortNumber, ndisStatus));
    }
    
    return ndisStatus;
}

/** 
 * AP direct OIDs handling 
 * Direct OIDs are not serialized. They may come simultaneously.
 * They may also overlap with OIDs.
 */
NDIS_STATUS
Ap11DirectOidHandler(
    _In_ PMP_PORT Port,
    _In_ PNDIS_OID_REQUEST NdisOidRequest
    )
{

    NDIS_STATUS ndisStatus = NDIS_STATUS_NOT_SUPPORTED;
    PMP_EXTAP_PORT ApPort;
    
    MPASSERT(Port != NULL);
    MPASSERT(NdisOidRequest != NULL);

    ApPort = MP_GET_AP_PORT(Port);
    
    // 
    // Reference AP port first,
    // so that AP port stays valid during the direct OID call.
    //
    ApRefPort(ApPort);
    
    switch(NdisOidRequest->RequestType)
    {
        case NdisRequestSetInformation:
            ndisStatus = ApSetInformation(
                            ApPort,
                            NdisOidRequest->DATA.SET_INFORMATION.Oid,
                            NdisOidRequest->DATA.SET_INFORMATION.InformationBuffer,
                            NdisOidRequest->DATA.SET_INFORMATION.InformationBufferLength,
                            (PULONG)&NdisOidRequest->DATA.SET_INFORMATION.BytesRead,
                            (PULONG)&NdisOidRequest->DATA.SET_INFORMATION.BytesNeeded
                            );
            break;

        //
        // At present, only Set is allowed in direct OID calls.
        //
        default:
            ndisStatus = NDIS_STATUS_NOT_SUPPORTED;
            break;
    }

    if (ndisStatus != NDIS_STATUS_SUCCESS && ndisStatus != NDIS_STATUS_PENDING)
    {
        // OID has failed here
        MpTrace(COMP_OID, DBG_NORMAL, ("Port(%u): Direct NDIS_OID_REQUEST failed in ExtAP Port. Status = 0x%08x\n", 
                            Port->PortNumber, ndisStatus));
    }
    
    // 
    // Dereference AP port
    //
    ApDerefPort(ApPort);

    return ndisStatus;
}

NDIS_STATUS
Ap11Fill80211Attributes(
    _In_ PMP_PORT Port,
    _Inout_ PNDIS_MINIPORT_ADAPTER_NATIVE_802_11_ATTRIBUTES Attr
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PVNIC vnic = Port->VNic;
    ULONG algoPairCountNeeded = 0;
    ULONG algoPairCountWritten = 0;
    ULONG bufferSize = 0;

    do
    {
        // support ExtAP mode
        Attr->OpModeCapability |= DOT11_OPERATION_MODE_EXTENSIBLE_AP;


        MP_ALLOCATE_MEMORY(
            Port->MiniportAdapterHandle, 
            &Attr->ExtAPAttributes,
            sizeof(DOT11_EXTAP_ATTRIBUTES),
            PORT_MEMORY_TAG
            );
        
        if (NULL == Attr->ExtAPAttributes)
        {
            MpTrace(COMP_OID, DBG_SERIOUS, ("Port(%u): Failed to allocate %d bytes for ExtAP capability.\n",
                                    Port->PortNumber, sizeof(DOT11_EXTAP_ATTRIBUTES)));
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }

        NdisZeroMemory(Attr->ExtAPAttributes, sizeof(DOT11_EXTAP_ATTRIBUTES));
        
        MP_ASSIGN_NDIS_OBJECT_HEADER(
            Attr->ExtAPAttributes->Header, 
            NDIS_OBJECT_TYPE_DEFAULT,
            DOT11_EXTAP_ATTRIBUTES_REVISION_1,
            sizeof(DOT11_EXTAP_ATTRIBUTES));

        // TODO: query base port?
        //
        // Use default values
        //
        Attr->ExtAPAttributes->uScanSSIDListSize = AP_SCAN_SSID_LIST_MAX_SIZE;
        Attr->ExtAPAttributes->uPrivacyExemptionListSize = AP_PRIVACY_EXEMPTION_LIST_MAX_SIZE;
        Attr->ExtAPAttributes->uDefaultKeyTableSize = VNic11DefaultKeyTableSize(vnic);
        Attr->ExtAPAttributes->uWEPKeyValueMaxLength = VNic11WEP104Implemented(vnic) ? 
                                                 104 / 8 : (VNic11WEP40Implemented(vnic) ? 40 / 8 : 0);

        Attr->ExtAPAttributes->uDesiredSSIDListSize = AP_DESIRED_SSID_LIST_MAX_SIZE;
        Attr->ExtAPAttributes->bStrictlyOrderedServiceClassImplemented = AP_STRICTLY_ORDERED_SERVICE_CLASS_IMPLEMENTED;
        Attr->ExtAPAttributes->uAssociationTableSize = AP_DEFAULT_ALLOWED_ASSOCIATION_COUNT;

        //
        // 11d stuff.
        //
        Attr->ExtAPAttributes->uNumSupportedCountryOrRegionStrings = 0;
        Attr->ExtAPAttributes->pSupportedCountryOrRegionStrings = NULL;
        
        do
        {
            //
            // Get unicast algorithm pair list
            //
            ndisStatus = ApQuerySupportedUnicastAlgoPairs(
                            Port, 
                            NULL, 
                            0, 
                            &algoPairCountWritten, 
                            &algoPairCountNeeded
                            );

            MPASSERT(NDIS_STATUS_INVALID_LENGTH == ndisStatus);
            if (ndisStatus != NDIS_STATUS_INVALID_LENGTH)
            {
                break;
            }

            MPASSERT(algoPairCountNeeded > 0);
            
            // 
            // allocate memory for unicast algorithm pair list
            //
            
            if (RtlULongMult(sizeof(DOT11_AUTH_CIPHER_PAIR), algoPairCountNeeded, &bufferSize) != STATUS_SUCCESS)
            {
                // should not overflow
                MPASSERT(FALSE);
                ndisStatus = NDIS_STATUS_INVALID_DATA;
                break;
            }
            
            MP_ALLOCATE_MEMORY(
                Port->MiniportAdapterHandle, 
                &Attr->ExtAPAttributes->pInfraSupportedUcastAlgoPairs,
                bufferSize,
                PORT_MEMORY_TAG
                );
            
            if (NULL == Attr->ExtAPAttributes->pInfraSupportedUcastAlgoPairs)
            {
                MpTrace(COMP_OID, DBG_SERIOUS, ("Port(%u): Failed to allocate %d bytes for unicast algorithm pairs.\n",
                                        Port->PortNumber, bufferSize));
                ndisStatus = NDIS_STATUS_RESOURCES;
                break;
            }
            
            NdisZeroMemory(Attr->ExtAPAttributes->pInfraSupportedUcastAlgoPairs, bufferSize);

            ndisStatus = ApQuerySupportedUnicastAlgoPairs(
                            Port, 
                            Attr->ExtAPAttributes->pInfraSupportedUcastAlgoPairs, 
                            algoPairCountNeeded, 
                            &algoPairCountWritten, 
                            &algoPairCountNeeded
                            );

            // should succeed
            MPASSERT(NDIS_STATUS_SUCCESS == ndisStatus);
            if (ndisStatus != NDIS_STATUS_SUCCESS)
            {
                MpTrace(COMP_OID, DBG_SERIOUS, ("Port(%u): Failed to query unicast algorithm pairs.\n", Port->PortNumber));
                break;
            }

            Attr->ExtAPAttributes->uInfraNumSupportedUcastAlgoPairs = algoPairCountWritten;
            
            //
            // Get multicast algorithm pair list for infrastructure
            //
            ndisStatus = ApQuerySupportedMulticastAlgoPairs(
                            Port, 
                            NULL, 
                            0, 
                            &algoPairCountWritten, 
                            &algoPairCountNeeded
                            );

            MPASSERT(NDIS_STATUS_INVALID_LENGTH == ndisStatus);
            if (ndisStatus != NDIS_STATUS_INVALID_LENGTH)
            {
                break;
            }

            MPASSERT(algoPairCountNeeded > 0);
            
            // 
            // allocate memory for unicast algorithm pair list
            //
            
            if (RtlULongMult(sizeof(DOT11_AUTH_CIPHER_PAIR), algoPairCountNeeded, &bufferSize) != STATUS_SUCCESS)
            {
                // should not overflow
                MPASSERT(FALSE);
                ndisStatus = NDIS_STATUS_INVALID_DATA;
                break;
            }
            
            MP_ALLOCATE_MEMORY(
                Port->MiniportAdapterHandle, 
                &Attr->ExtAPAttributes->pInfraSupportedMcastAlgoPairs,
                bufferSize,
                PORT_MEMORY_TAG
                );
            
            if (NULL == Attr->ExtAPAttributes->pInfraSupportedMcastAlgoPairs)
            {
                MpTrace(COMP_OID, DBG_SERIOUS, ("Port(%u): Failed to allocate %d bytes for multicast algorithm pairs.\n",
                                    Port->PortNumber,
                                    bufferSize));
                ndisStatus = NDIS_STATUS_RESOURCES;
                break;
            }
            
            NdisZeroMemory(Attr->ExtAPAttributes->pInfraSupportedMcastAlgoPairs, bufferSize);

            ndisStatus = ApQuerySupportedMulticastAlgoPairs(
                            Port, 
                            Attr->ExtAPAttributes->pInfraSupportedMcastAlgoPairs, 
                            algoPairCountNeeded, 
                            &algoPairCountWritten, 
                            &algoPairCountNeeded
                            );

            // should succeed
            MPASSERT(NDIS_STATUS_SUCCESS == ndisStatus);
            if (ndisStatus != NDIS_STATUS_SUCCESS)
            {
                MpTrace(COMP_OID, DBG_SERIOUS, ("Port(%u): Failed to query multicast algorithm pairs. Status = 0x%08x.\n",
                                        Port->PortNumber,
                                        ndisStatus));
                break;
            }

            Attr->ExtAPAttributes->uInfraNumSupportedMcastAlgoPairs = algoPairCountWritten;
        }    while (FALSE);

    } while (FALSE);
    
    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        Ap11Cleanup80211Attributes(Port, Attr);
    }
    
    return ndisStatus;
}

VOID
Ap11Cleanup80211Attributes(
    _In_ PMP_PORT Port,
    _In_ PNDIS_MINIPORT_ADAPTER_NATIVE_802_11_ATTRIBUTES Attr
    )
{
    UNREFERENCED_PARAMETER(Port);
    
    if (Attr->ExtAPAttributes)
    {
        if (Attr->ExtAPAttributes->pSupportedCountryOrRegionStrings)
        {
            MP_FREE_MEMORY(Attr->ExtAPAttributes->pSupportedCountryOrRegionStrings);
        }
        if (Attr->ExtAPAttributes->pInfraSupportedUcastAlgoPairs)
        {
            MP_FREE_MEMORY(Attr->ExtAPAttributes->pInfraSupportedUcastAlgoPairs);
        }

        if (Attr->ExtAPAttributes->pInfraSupportedMcastAlgoPairs)
        {
            MP_FREE_MEMORY(Attr->ExtAPAttributes->pInfraSupportedMcastAlgoPairs);
        }

        MP_FREE_MEMORY(Attr->ExtAPAttributes);

        Attr->ExtAPAttributes = NULL;
    }
}

