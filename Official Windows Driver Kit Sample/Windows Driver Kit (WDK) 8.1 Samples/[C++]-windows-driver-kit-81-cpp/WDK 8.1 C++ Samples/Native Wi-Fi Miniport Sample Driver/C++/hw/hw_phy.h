/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    hw_phy.h

Abstract:
    Contains defines used for phy functionality 
    in the HW layer
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#pragma once

BOOLEAN
HwQueryShortSlotTimeOptionImplemented(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId
    );


BOOLEAN
HwQueryDsssOfdmOptionImplemented(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId
    );

BOOLEAN
HwQueryShortPreambleOptionImplemented(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId
    );


BOOLEAN
HwQueryPbccOptionImplemented(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId
    );

BOOLEAN
HwQueryChannelAgilityPresent(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId
    );

BOOLEAN
HwQueryNicPowerState(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId
    );

BOOLEAN
HwQueryHardwarePhyState(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId
    );

BOOLEAN
HwQuerySoftwarePhyState(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId
    );


NDIS_STATUS
HwQueryDiversitySelectionRX(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId,
    _In_  ULONG                   MaxEntries,
    _Out_ PDOT11_DIVERSITY_SELECTION_RX_LIST Dot11DiversitySelectionRXList
    );

NDIS_STATUS
HwQueryRegDomainsSupportValue(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId,
    _In_  ULONG                   MaxEntries,
    _Out_ PDOT11_REG_DOMAINS_SUPPORT_VALUE    Dot11RegDomainsSupportValue
    );

LONG
HwQueryMinRSSI(
    _In_  PHW                     Hw,
    _In_  ULONG                   DataRate
    );

LONG
HwQueryMaxRSSI(
    _In_  PHW                     Hw,
    _In_  ULONG                   DataRate
    );

NDIS_STATUS
HwQuerySupportedDataRatesValue(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId,
    _Out_ PDOT11_SUPPORTED_DATA_RATES_VALUE_V2    Dot11SupportedDataRatesValue
    );

ULONG
HwQueryCCAModeSupported(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId
    );


ULONG
HwQueryCurrentTXPowerLevel(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId
    );


DOT11_DIVERSITY_SUPPORT
HwQueryDiversitySupport(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId
    );


ULONG
HwQueryEDThreshold(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId
    );


ULONG
HwQueryFrequencyBandsSupported(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId
    );


BOOLEAN
HwQueryMultiDomainCapabilityImplemented(
    _In_  PHW                     Hw
    );

DOT11_TEMP_TYPE
HwQueryTempType(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId
    );

NDIS_STATUS
HwPersistRadioPowerState(
    _In_  PHW                     Hw,
    _In_  BOOLEAN                 RadioOff
    );

NDIS_STATUS
HwSetNicPowerState(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId,
    _In_  BOOLEAN                 PowerOn
    );

BOOLEAN
HwAwake(
    _In_  PHW                     Hw,
	_In_  BOOLEAN                 DeviceIRQL
	);

NDIS_TIMER_FUNCTION HwAwakeTimer;

NDIS_TIMER_FUNCTION HwDozeTimer;

BOOLEAN
HwSetRFState(
    _In_  PHW                     Hw,
    _In_  UCHAR	                NewRFState
    );

BOOLEAN
HwSetRFOn(
    _In_  PHW                     Hw,
	_In_  UCHAR                   MaxRetries
	);

NDIS_STATUS
HwSetOperationalRateSet(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId,
    _In_  PDOT11_RATE_SET         Dot11RateSet
    );

NDIS_STATUS
HwValidateChannel(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId,
    _In_  UCHAR                   Channel
    );

NDIS_STATUS
HwSetChannel(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId,
    _In_  UCHAR                   Channel
    );

NDIS_STATUS
HwSetOperatingPhyId(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId
    );

NDIS_STATUS
HwSetPhyContext(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId,
    _In_  PHW_PHY_CONTEXT         PhyContext
    );

NDIS_IO_WORKITEM_FUNCTION HwPhyProgramWorkItem;

NDIS_STATUS
HwProgramPhy(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  ULONG                   PhyId,
    _In_  PHW_PHY_CONTEXT         PhyContext,
    _In_opt_  HW_GENERIC_CALLBACK_FUNC    CompletionCallback
    );

