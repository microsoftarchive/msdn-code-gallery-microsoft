/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    hal_intf.h

Abstract:
    Contains defines for the HAL layer. The HW layer calls into this
    to control the physical NIC. The implementation of the hal interfaces
    is vendor specific
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#ifndef WLAN_HAL_H
#define WLAN_HAL_H

#pragma pack(push, 1)


/** Maximum number of on-NIC multicast address entries */
#define HW_MAX_MCAST_LIST_SIZE    32

/**
 * This enumeration type lists all the possible error entries that can be placed
 * in the Event log by the miniport. Vendors should add any new error entries to this
 * enumeration type before using it. This will provide one consolidated place to check
 * for all error codes.
 */
typedef enum _HW11_ERROR_LOG_ENTRY{
    ERRLOG_READ_PCI_SLOT_FAILED = 101,
    ERRLOG_WRITE_PCI_SLOT_FAILED,
    ERRLOG_VENDOR_DEVICE_MISMATCH,
    ERRLOG_INVALID_PCI_CONFIGURATION,
    ERRLOG_BUS_MASTER_DISABLED,
    ERRLOG_QUERY_ADAPTER_RESOURCES_FAILED,
    ERRLOG_NO_IO_RESOURCE,
    ERRLOG_NO_INTERRUPT_RESOURCE,
    ERRLOG_NO_MEMORY_RESOURCE,
    ERRLOG_OUT_OF_SG_RESOURCES,
    ERRLOG_REGISTER_INTERRUPT_FAILED,
    ERRLOG_REGISTER_IO_PORT_RANGE_FAILED,
    ERRLOG_MAP_IO_SPACE_FAILED,
    ERRLOG_REMOVE_MINIPORT
} HW11_ERROR_LOG_ENTRY;

#define MAX_TX_RX_PACKET_SIZE 				2500

/**
 * This is the key structure that the HW layer passes to the HAL. This structure
 * is also used internally in the HW later.
 */
typedef struct _NICKEY
{
    BOOLEAN                     Persistent;
    BOOLEAN                     Valid;
    DOT11_MAC_ADDRESS           MacAddr;
    DOT11_CIPHER_ALGORITHM      AlgoId;
    UCHAR                       KeyLength;              // length of KeyValue in bytes
    UCHAR                       KeyValue[16];           // 128 bits
    UCHAR                       TxMICKey[8];            // for TKIP only
    UCHAR                       RxMICKey[8];            // for TKIP only
} NICKEY, *PNICKEY;

/**
 * PHY specific MIB. The MIB could be different for different vendors.This structure
 * is also used internally in the HW later, so no fields should be removed from
 * this structure
 */
typedef struct _NICPHYMIB       
{
    DOT11_PHY_TYPE                      PhyType;
    ULONG                               PhyID;
    DOT11_RATE_SET                      OperationalRateSet;
    DOT11_RATE_SET                      BasicRateSet;
    UCHAR                               Channel;
    DOT11_SUPPORTED_DATA_RATES_VALUE_V2	SupportedDataRatesValue;
} NICPHYMIB, *PNICPHYMIB;

/** These queue type defines are used for setting up the TX queues on the NIC */
#define LOW_QUEUE			0
#define NORMAL_QUEUE		1
#define HIGH_QUEUE			2
#define BEACON_QUEUE		3
#define TX_QUEUE_NUM		4

/** Maximum number of phy types supported by the hardware */
#define MAX_NUM_PHY_TYPES                       5

/** Maintains the current state of the radio */
typedef enum _RF_POWER_STATE {
	RF_ON,                          // Radio ON
	RF_SLEEP,                       // Radio in low power save
	RF_OFF,                         // Radio OFF due to power save
	RF_SHUT_DOWN                    // Radio turned OFF
} RF_POWER_STATE, *PRF_POWER_STATE;

#pragma pack(pop)

/**
 * The HAL_TX_ITERATOR is used to maintain association between the HW and the HAL
 * for sends. It represents one TX descriptor on the NIC & is used by the HW
 * layer to identify one DMA destination. The HAL layer can either track it as an
 * index into its TX descriptors array or as a pointer
 */
typedef PVOID HAL_TX_ITERATOR, *PHAL_TX_ITERATOR;

typedef PVOID HAL_TX_DESC_HANDLE, *PHAL_TX_DESC_HANDLE;

/**
 * The HAL_RX_ITERATOR is used to maintain association between the HW and the HAL
 * for receives. It represents one received MPDU. The HAL layer can either track 
 * it as an index into its RX descriptors array or as a pointer
 */
typedef PVOID HAL_RX_ITERATOR, *PHAL_RX_ITERATOR;

typedef PVOID HAL_RX_DESC_HANDLE, *PHAL_RX_DESC_HANDLE;

/**
 * These packet type definitions are passed to the HAL by the HW on TX. The HAL can
 * uses these to determine whether it needs to do special processing for the
 * packet
 */
typedef enum _HAL_PACKET_TYPE
{
    HAL_PACKET_DATA,
    HAL_PACKET_ATIM,
    HAL_PACKET_PS_POLL,
    HAL_PACKET_PROBE_RESPONSE,
    HAL_PACKET_BEACON
}HAL_PACKET_TYPE;

/**
 * This is the structure that the HAL returns to the HW on receive processing. 
 * It contains the status of the current receive
 */
typedef struct _HAL_RX_DESC_STATUS
{
    USHORT Length;
    USHORT ReceiveFinished : 1;
    USHORT ReceiveSuccessful : 1;
    USHORT FirstSegment : 1;
    USHORT LastSegment : 1;
    USHORT Unicast : 1;
    USHORT Multicast : 1;
    USHORT Broadcast : 1;
    USHORT HardwareError : 1;
    USHORT FifoOverflow : 1;
    USHORT CRCError : 1;
    USHORT ICVError : 1;
    USHORT Decrypted : 1;
    USHORT Unused : 4;
    USHORT Rate;
    ULONGLONG TimeStamp;
    /** 
     * The HAL can use these fields for storing private data such as descriptor
     * index, etc
     */
    ULONG HalPrivate[2];
}HAL_RX_DESC_STATUS, *PHAL_RX_DESC_STATUS;

/**
 * The send item maintains the association between the HW and the HAL for sends.
 * It represents 1 send MSDU
 */
typedef struct _HAL_SEND_ITEM
{
    HAL_TX_ITERATOR firstIterator;
    HAL_TX_ITERATOR lastIterator;
    USHORT DescNum;
}HAL_SEND_ITEM, *PHAL_SEND_ITEM;

/**
 * This is the structure that the HAL returns to the HW on send complete processing. 
 * It contains the status of the current send
 */
typedef struct _HAL_TX_DESC_STATUS {
	UCHAR RetryCount;
	UCHAR RtcRetryCount;
	UCHAR TransmitSuccess:1;
	UCHAR LastSegment:1;
	UCHAR FirstSegment:1;
	UCHAR SendPending : 1;
	UCHAR Unused : 4;

} HAL_TX_DESC_STATUS, *PHAL_TX_DESC_STATUS;

/**
 * This is used by the HW to inform the HAL about what the PS bit in this packet
 * should be
 */
typedef enum _HAL_TX_DESC_PS_BIT_ENUM
{
    TxDescPsBitUnspecified = 0,
    TxDescPsBitSet,
    TxDescPsBitClear
}HAL_TX_DESC_PS_BIT_ENUM;

/**
 * These are the number of rates that the upper layer would specify
 */
#define HAL_TX_RATE_TABLE_SIZE    4

/**
 * This structure is used by the HW to send data. Depending on the
 * bits in the structure, this may represent a full MPDU, or a portion of
 * an MPDU (eg. one SG element)
 */
typedef struct _HAL_TX_DESC_INFO
{
	PUCHAR MacHeader;
	ULONG PhysicalAddressLow;
	ULONG BufferLen;
	ULONG PacketLen;
	/** The rates to use for sending this packet. A 0 indicates an invalid rate */
	USHORT RateTable[HAL_TX_RATE_TABLE_SIZE];
	USHORT RTSCTSRate;	
	/** This is true for the first segment (address to DMA) of an MPDU */
	USHORT FirstSeg:1;
	/** This is true for the last segment (address to DMA) of an MPDU. Both
	 * FirstSeg and LastSeg can be set at the same time 
	 */
	USHORT LastSeg:1;
	USHORT Multicast:1;
	USHORT Broadcast:1;	
	USHORT ShortPreamble:1;
	/** This is true if there are additional MPDUs in this MSDU */
	USHORT MoreFrag:1;
	USHORT RTSEnabled:1;
	USHORT CTSToSelf:1;
	/** This is true if this packet should be sent without encryption. If false
	 * the upper layer would provide the KeyIndex */
	USHORT NoEncrypt:1;	
	/** This is true only for beacon frames & informs the HAL to keep resending 
	 * this frame */
	USHORT EOL:1;
	USHORT Unused:6;
	/** This is the key  to use to encrypt this packet. This should be looked at
	 * only if NoEncrypt is false */
	UCHAR KeyIndex;
	/** This points to the HAL descriptor that was reserved for this send */
	HAL_TX_ITERATOR FirstDescIter;
	HAL_PACKET_TYPE PacketType;
	HAL_TX_DESC_PS_BIT_ENUM PsBitSetting;
} HAL_TX_DESC_INFO, *PHAL_TX_DESC_INFO;

/** This represents the type of packets that would be sent on this queue */
typedef enum _HAL_TX_QUEUE_TYPE
{
    /** Beacons only */
    HAL_QUEUE_TYPE_BEACON,

    /** Not currently used */
    HAL_QUEUE_TYPE_HIGH_PRIORITY,

    /** HW layer internal packets such as null data packets, probe responses */
    HAL_QUEUE_TYPE_MANAGEMENT,

    /** All other packet types */
    HAL_QUEUE_TYPE_DATA
}HAL_TX_QUEUE_TYPE, *PHAL_TX_QUEUE_TYPE;

/** This structure contains configuration information for TX queue setup */
typedef struct _HAL_TX_QUEUE_SETUP_INFO
{
    ULONG DescNum;
    HAL_TX_QUEUE_TYPE QueueType;
}HAL_TX_QUEUE_SETUP_INFO, *PHAL_TX_QUEUE_SETUP_INFO;

/** The HAL reports the hardware PS capability through this interface */
typedef struct _HAL_POWERSAVE_CAP
{
    BOOLEAN CanSendPSPoll;
    BOOLEAN SupportHwWakeup;
    // If hw won't wake up automatically in power save mode, the upper layer will wake up the NIC using software timer
    // The LeadTime is the time needed for the NIC to return to its operational mode. The unit is milliseconds.
    // The field is only valid when SupportHwWakeup is FALSE
    ULONG   SoftwareWakeupLeadTime; 
}HAL_POWERSAVE_CAP, *PHAL_POWERSAVE_CAP;

typedef struct _WLAN_HAL *PWLAN_HAL;

/** 
 * These bits are returned by the GetEncryptionCapabilities API & 
 * return information about the cipher capabilities of the HW
 */
#define HAL_ENCRYPTION_SUPPORT_TKIP   0x0001
#define HAL_ENCRYPTION_SUPPORT_CCMP   0x0002
#define HAL_ENCRYPTION_SUPPORT_KEYMAPPINGKEYTABLE 0x0004
#define HAL_ENCRYPTION_SUPPORT_PERSTATIONKEYTABLE 0x0008
#define HAL_ENCRYPTION_RESERVE_IV_FIELD           0x0010
#define HAL_ENCRYPTION_SUPPORT_PERSTADEFAULTKEY   0x0020 

#define HAL_ISR_SURPRISE_REMOVED ((ULONG)-1)

/** These map to the interrupt bits on the hardware. These are set in the interrupt
 * mask and returned as the interrupt status 
 */
#define HAL_ISR_TX_DONE           0x00000001 // at least one TX queue has finished
#define HAL_ISR_BEACON_DONE       0x00000002 // beacon Tx queue has generated an interrupt. 
#define HAL_ISR_TX_Q1_DONE        0x00000004 // Tx queue 1 has finished send. Queue 1 has the next highest priority after beacon queue
#define HAL_ISR_TX_Q2_DONE        0x00000008
#define HAL_ISR_TX_Q3_DONE        0x00000010
#define HAL_ISR_TX_Q4_DONE        0x00000020
#define HAL_ISR_TX_Q5_DONE        0x00000040
#define HAL_ISR_TX_Q6_DONE        0x00000080
#define HAL_ISR_TX_Q8_DONE        0x00000100
#define HAL_ISR_TX_Q9_DONE        0x00000200

#define HAL_ISR_RX_DONE           0x00001000
#define HAL_ISR_RX_DS_UNAVAILABLE 0x00002000
#define HAL_ISR_RX_FIFO_OVERFLOW  0x00004000
#define HAL_ISR_ATIM_END          0x00100000
#define HAL_ISR_BEACON_INTERRUPT  0x00200000
#define HAL_ISR_PHY_STATE_CHANGED 0x01000000

typedef struct _NICPHYMIB *PNICPHYMIB;

/**
 * This is the main HAL interface. This structure is populated by the HAL
 * and returned to the upper layer. This interface is opaque to the upper layer
 */
typedef struct _WLAN_HAL
{
    // ParsePciConfiguration is the first function called after HAL is created
    NDIS_STATUS (* ParsePciConfiguration)(PWLAN_HAL hal, PUCHAR buffer, ULONG bufferSize);    

    // ReadRegistryConfiguration is the second function called to read initialization parameter from registry
    NDIS_STATUS (* ReadRegistryConfiguration)(PWLAN_HAL hal);

    // AddAdapterResource is the third function called to allocate system resource, such as port to the device
    NDIS_STATUS (* AddAdapterResource)(PWLAN_HAL hal, PCM_PARTIAL_RESOURCE_DESCRIPTOR resource);    

    // Initialize is the last function called before calling Start. All variables should be initialized after this step
    NDIS_STATUS (* Initialize)(PWLAN_HAL hal);    
    NDIS_STATUS (* Start)(PWLAN_HAL hal, BOOLEAN isReset);
    NDIS_STATUS (* Stop)(PWLAN_HAL hal);

    VOID (* FreeNic)(PWLAN_HAL hal);
    VOID (* HaltNic)(PWLAN_HAL hal);

    // Notifiying the HAL that the reset is started
    NDIS_STATUS (* ResetStart)(PWLAN_HAL hal);
    // Physical reset the device
    NDIS_STATUS (* ResetDevice)(PWLAN_HAL hal);
    // Notifiying the HAL that the reset is ended
    NDIS_STATUS (* ResetEnd)(PWLAN_HAL hal);

    
    PUCHAR (* GetPermanentAddress)(PWLAN_HAL hal);
    NDIS_STATUS (* AssignMACAddress)(PWLAN_HAL hal, ULONG macId, PUCHAR macAddress);
    VOID (* SetBssId)(PWLAN_HAL hal, UCHAR *bssId);
    
	// Send functions
	NDIS_STATUS (* AllocateTxQueues)(PWLAN_HAL hal, ULONG queueNum);
    NDIS_STATUS (* SetupTxQueue)(PWLAN_HAL hal, ULONG queueIndex, PHAL_TX_QUEUE_SETUP_INFO setupInfo);
    NDIS_STATUS (* ResetTxDescs)(PWLAN_HAL hal, ULONG queueIndex);
    NDIS_STATUS (* ReleaseTxDescs)(PWLAN_HAL hal, ULONG queueIndex);
    NDIS_STATUS (* ReleaseTxQueues)(PWLAN_HAL hal);	
	NDIS_STATUS (* GetTxDesc)(PWLAN_HAL hal, HAL_TX_ITERATOR iterator, PHAL_TX_DESC_HANDLE desc);
	NDIS_STATUS (* TxIterMoveNext)(PWLAN_HAL hal, PHAL_TX_ITERATOR iter);
	NDIS_STATUS (* GetNextToCheckTxDescIterator)(PWLAN_HAL hal, ULONG queueIndex, PHAL_TX_ITERATOR descIterator);
    USHORT (* GetTxDescBusyNum) (PWLAN_HAL hal, ULONG queueIndex);
	NDIS_STATUS (* ReserveNextTxDescsForTransmit)(PWLAN_HAL hal, ULONG queueIndex, USHORT reserveDescNum, HAL_TX_ITERATOR* startIter);
	NDIS_STATUS (* Transmit)(PWLAN_HAL hal, PHAL_SEND_ITEM sendItem);
	NDIS_STATUS (* TransmitBeacon)(PWLAN_HAL hal, PHAL_SEND_ITEM sendItem, BOOLEAN autoResend);
	NDIS_STATUS (* ReleaseTxDesc)(PWLAN_HAL hal, HAL_TX_ITERATOR startIter, HAL_TX_ITERATOR lastIter, USHORT num);	
	BOOLEAN (* IsSendItemCompleted)(PWLAN_HAL hal, PHAL_SEND_ITEM sendItem);
	VOID (* FillTxDescriptor)(PWLAN_HAL hal, PHAL_TX_DESC_INFO descInfo, HAL_TX_ITERATOR descIter);
	VOID (* GetTxDescStatus)(PWLAN_HAL hal, HAL_TX_ITERATOR descIter, PHAL_TX_DESC_STATUS descStatus);
    VOID (* CheckForSends)(PWLAN_HAL hal, ULONG queueId, BOOLEAN forceSend);

    // Receive function
    NDIS_STATUS (* AllocateRxDescs)(PWLAN_HAL hal, ULONG descNum);
    NDIS_STATUS (* ReleaseRxDescs)(PWLAN_HAL hal);
    NDIS_STATUS (* ResetRxDescs)(PWLAN_HAL hal);
    NDIS_STATUS (* ReturnRxDesc)(PWLAN_HAL hal, ULONG bufLen, PUCHAR bufVirAddr, NDIS_PHYSICAL_ADDRESS bufPhyAddr, HAL_RX_ITERATOR* returnIter);
    NDIS_STATUS (* ReserveRxDesc)(PWLAN_HAL hal, HAL_RX_ITERATOR rxDescIter);
    VOID (* GetRxStatus)(PWLAN_HAL hal, HAL_RX_ITERATOR rxDescIter, PHAL_RX_DESC_STATUS rxDescStatus);
    BOOLEAN (* IsRxDescReady)(PWLAN_HAL hal, HAL_RX_ITERATOR rxDescIter);
    VOID (* SetPacketFilter)(PWLAN_HAL hal, ULONG packetFilter);
    ULONG (* GetRSSI)(PWLAN_HAL hal, PHAL_RX_DESC_STATUS rxDesc, UCHAR* rxBuffer);
    ULONG (* GetCalibratedRSSI)(PWLAN_HAL hal, PHAL_RX_DESC_STATUS rxDesc, UCHAR* rxBuffer);    
    NDIS_STATUS (* StartReceive)(PWLAN_HAL hal);

    // Power save & management
    VOID (* SetPowerMgmtMode)(PWLAN_HAL hal, PDOT11_POWER_MGMT_MODE mode);
    VOID (* SetRFPowerState)(PWLAN_HAL hal, UCHAR powerState);
    UCHAR (* GetRFPowerState)(PWLAN_HAL hal);    
    ULONG (* GetTXPowerLevel)(PWLAN_HAL hal, ULONG phyId);
    VOID (* SetDevicePowerState)(PWLAN_HAL hal, NDIS_DEVICE_POWER_STATE devicePowerState);
    VOID (* GetPowerSaveCapabilities)(PWLAN_HAL hal, HAL_POWERSAVE_CAP* cap);
    VOID (* PowerSaveStateWakeup)(PWLAN_HAL hal);
    VOID (* PowerSaveStateRestore)(PWLAN_HAL hal);    
    // Phy management
    PDOT11_SUPPORTED_PHY_TYPES (* GetSupportedPhyTypes)(PWLAN_HAL hal);    
    PNICPHYMIB (* GetPhyMIB)(PWLAN_HAL hal, ULONG phyID);
    PNICPHYMIB (* GetOperatingPhyMIB)(PWLAN_HAL hal);
    ULONG (* GetOperatingPhyId)(PWLAN_HAL hal);
    VOID (* SetOperatingPhyId)(PWLAN_HAL hal, ULONG phyId);

    // Interrupt Handling
    VOID (* EnableInterrupt)(PWLAN_HAL hal);
    VOID (* DisableInterrupt)(PWLAN_HAL hal);
    VOID (* SetIntrMask)(PWLAN_HAL hal, ULONG intrMask);
    VOID (* SetInterruptHandle) (PWLAN_HAL hal, NDIS_HANDLE intrHandle);
    VOID (* InterlockedOrIntrMask)(PWLAN_HAL hal, ULONG intrMask);
    VOID (* InterlockedAndIntrMask)(PWLAN_HAL hal, ULONG intrMask);    
    BOOLEAN (* IsInterruptEnabled)(PWLAN_HAL hal);
    VOID (* ClearInterrupt)(PWLAN_HAL hal);
    ULONG (* GetIntrMask)(PWLAN_HAL hal);
    VOID   (* ReadIsr)(PWLAN_HAL hal, PULONG halIsr, PULONG deviceIsr);
    BOOLEAN (* ProcessIsr)(PWLAN_HAL hal, ULONG isr);
    
    // Beacon related
    VOID (*SetBeaconInterval)(PWLAN_HAL hal, ULONG beaconInterval);
    USHORT (*GetBeaconRate)(PWLAN_HAL hal, ULONG phyId);
    VOID (* PrepareJoin)(PWLAN_HAL hal);

    // Channel switch & scan
    VOID (*StartScan)(PWLAN_HAL hal);
    VOID (*StopScan)(PWLAN_HAL hal);
    NDIS_STATUS (* GetChannelList)(PWLAN_HAL hal, ULONG phyId, PULONG num, ULONG* channelList);   
    NDIS_STATUS (* ValidateChannel)(PWLAN_HAL hal, ULONG phyId, UCHAR channel);
    NDIS_STATUS (* SwitchChannel)(PWLAN_HAL hal, ULONG phyId, UCHAR channel, BOOLEAN HighIRQLevel);

    // Encryption & Key management
    VOID (* RemoveKeyEntry)(PWLAN_HAL hal, UCHAR index);
    VOID (* AddKeyEntry)(PWLAN_HAL hal, PNICKEY key, UCHAR index);
    VOID (* UpdateKeyEntry)(PWLAN_HAL hal, PNICKEY key, UCHAR index);
    // Adds the new cipher to the list of enabled ciphers. If new cipher is 0, this
    // disables all previous ciphers
    VOID (* SetEncryption)(PWLAN_HAL hal, DOT11_CIPHER_ALGORITHM algoId);
    ULONG (* GetEncryptionCapabilities)(PWLAN_HAL hal);

    // MP State update
    VOID (* SetCurrentBSSType)(PWLAN_HAL hal, DOT11_BSS_TYPE type);
    VOID (* EnableSafeMode)(PWLAN_HAL hal, BOOLEAN safeModeEnabled);
    VOID (* SetOperationMode)(PWLAN_HAL hal, ULONG operationMode);
    VOID (* UpdateConnectionState)(PWLAN_HAL hal, BOOLEAN connected);
    VOID (* EnableAdHocCoordinator)(PWLAN_HAL hal, BOOLEAN enable);

    // Misc parameter set & get
    ULONG (* GetShortRetryLimit)(PWLAN_HAL hal);
    ULONG (* GetLongRetryLimit)(PWLAN_HAL hal);  
    VOID (* SetMacAddress)(PWLAN_HAL hal, UCHAR* macAddress);
    VOID (* SetAtimWindow)(PWLAN_HAL hal, ULONG atimWindow);
    NDIS_STATUS (* SetMulticastMask)(PWLAN_HAL hal, BOOLEAN bAcceptAll, ULONG addrCount, UCHAR (* addrList)[6]);    

    VOID (* StartBSS)(PWLAN_HAL hal, ULONGLONG lastBeaconTimestamp);
    VOID (* StopBSS)(PWLAN_HAL hal);
    
    VOID (* SetAssociateId)(PWLAN_HAL hal, USHORT id);

    VOID (* PauseBSS)(PWLAN_HAL hal);
    VOID (* ResumeBSS)(PWLAN_HAL hal);
    
}WLAN_HAL;

/**
 * This function creates and returns a new WLAN_HAL structure for the HW layer to use
 * to talk to the hardware. The PNPID of the device is passed as a parameter to this
 * function.
 */
NDIS_STATUS HalCreateWLANHal(NDIS_HANDLE miniportAdapterHandle, USHORT vendorId, USHORT deviceId, USHORT revisionId, PWLAN_HAL *hal);

#define HalInitialize(hal) (*(hal)->Initialize)(hal)
#define HalReadRegistryConfiguration(hal) (*(hal->ReadRegistryConfiguration))(hal)
#define HalStart(hal, isReset) (*(hal->Start))(hal, isReset)
#define HalStop(hal) (*(hal->Stop))(hal)

#define HalAllocateTxQueues(hal, queueNum) (*(hal)->AllocateTxQueues)(hal, queueNum)
#define HalSetupTxQueue(hal, queueIndex, setupInfo) (*(hal->SetupTxQueue))(hal, queueIndex, setupInfo)
#define HalReleaseTxDescs(hal, queueIndex) ((*hal->ReleaseTxDescs))(hal, queueIndex)
#define HalReleaseTxQueues(hal) (*(hal)->ReleaseTxQueues)(hal)
#define HalResetTxDescs(hal, queueIndex) (*(hal->ResetTxDescs))(hal, queueIndex)
#define HalTxIterMoveNext(hal, iter) (*(hal->TxIterMoveNext))(hal, iter)
#define HalReserveNextTxDescsForTransmit(hal, queueIndex, reserveDescNum, startIter) (*(hal->ReserveNextTxDescsForTransmit))(hal, queueIndex, reserveDescNum, startIter)
#define HalTransmit(hal, sendItem) (*(hal->Transmit))(hal, sendItem)
#define HalTransmitBeacon(hal, sendItem, autoResend) (*(hal->TransmitBeacon))(hal, sendItem, autoResend)

#define HalIsSendItemCompleted(hal, sendItem) (*(hal->IsSendItemCompleted))(hal, sendItem)
#define HalFillTxDescriptor(hal, descInfo, descIter) (*(hal->FillTxDescriptor))(hal, descInfo, descIter)
#define HalGetTxDescStatus(hal, descIter, descStatus) (*(hal->GetTxDescStatus))(hal, descIter, descStatus)
#define HalReleaseTxDesc(hal, startIter, lastIter, num) (*(hal->ReleaseTxDesc))(hal, startIter, lastIter, num)
#define HalGetNextToCheckTxDescIterator(hal, queueIndex, descIterator) (*(hal->GetNextToCheckTxDescIterator))(hal, queueIndex, descIterator)
#define HalGetTxDescBusyNum(hal, queueIndex) (*(hal->GetTxDescBusyNum))(hal, queueIndex)


#define HalGetPermanentAddress(hal) (*(hal->GetPermanentAddress))(hal)
#define HalAssignMACAddress(hal, macId, macAddress) (*(hal->AssignMACAddress))(hal, macId, macAddress)

#define HalSetRFPowerState(hal, powerState) (*(hal->SetRFPowerState))(hal, powerState)
#define HalPowerSaveStateWakeup(hal) (*(hal->PowerSaveStateWakeup))(hal)
#define HalPowerSaveStateRestore(hal) (*(hal->PowerSaveStateRestore))(hal)

#define HalSetBssId(hal, bssId) (*(hal->SetBssId))(hal, bssId)
#define HalPrepareJoin(hal) (*(hal->PrepareJoin))(hal)
#define HalEnableAdHocCoordinator(hal, enable) (* (hal->EnableAdHocCoordinator))(hal, enable)

#define HalGetSupportedPhyTypes(hal) (*(hal->GetSupportedPhyTypes))(hal)
#define HalSetBeaconInterval(hal, beaconInterval) (*(hal->SetBeaconInterval))(hal, beaconInterval)
#define HalGetPhyMIB(hal, phyID) (*(hal->GetPhyMIB))(hal, phyID)
#define HalGetOperatingPhyMIB(hal) (*(hal->GetOperatingPhyMIB))(hal)
#define HalStartScan(hal) (*(hal->StartScan))(hal)
#define HalStopScan(hal) (*(hal->StopScan))(hal)
#define HalGetBeaconRate(hal, phyId) (*(hal->GetBeaconRate))(hal, phyId)
#define HalValidateChannel(hal, phyId, channel) (*(hal->ValidateChannel))(hal, phyId, channel)
#define HalSwitchChannel(hal, phyId, channel, highIRQLevel) (*(hal->SwitchChannel))(hal, phyId, channel, highIRQLevel)
#define HalGetRFPowerState(hal) (*(hal->GetRFPowerState))(hal)
#define HalSetCurrentBSSType(hal, type) (*(hal->SetCurrentBSSType))(hal, type)
#define HalEnableSafeMode(hal, safemodeEnabled) (*(hal->EnableSafeMode))(hal, safemodeEnabled)
#define HalSetPowerMgmtMode(hal, pwrMode) (*(hal->SetPowerMgmtMode))(hal, pwrMode)
#define HalGetPowerSaveCapabilities(hal, cap) (*(hal)->GetPowerSaveCapabilities)(hal, cap)
#define HalGetOperatingPhyId(hal) (*(hal->GetOperatingPhyId))(hal)
#define HalSetOperatingPhyId(hal, phyId) (*(hal->SetOperatingPhyId))(hal, phyId)
#define HalGetShortRetryLimit(hal) (*(hal->GetShortRetryLimit))(hal)
#define HalGetLongRetryLimit(hal) (*(hal->GetLongRetryLimit))(hal)

#define HalSetMacAddress(hal, macAddress) (*(hal->SetMacAddress))(hal, macAddress)
#define HalGetTXPowerLevel(hal, phyId) (*(hal->GetTXPowerLevel))(hal, phyId)
#define HalSetOperationMode(hal, operationMode) (*(hal->SetOperationMode))(hal, operationMode)
#define HalSetAtimWindow(hal, atimWindow) (*(hal->SetAtimWindow))(hal, atimWindow)
#define HalGetChannelList(hal, phyId, num, channelList) (*(hal->GetChannelList))(hal, phyId, num, channelList);
#define HalGetRSSI(hal, rxDesc, rxBuffer) (*(hal->GetRSSI))(hal, rxDesc, rxBuffer)
#define HalGetCalibratedRSSI(hal, rxDesc, rxBuffer) (*(hal->GetCalibratedRSSI))(hal, rxDesc, rxBuffer)
#define HalReadIsr(hal, halIsr, deviceIsr) (*(hal)->ReadIsr)(hal, halIsr, deviceIsr)
#define HalProcessIsr(hal, deviceIsr) (*(hal)->ProcessIsr)(hal, deviceIsr)

#define HalAllocateRxDescs(hal, descNum) (*(hal)->AllocateRxDescs)(hal, descNum);
#define HalReturnRxDesc(hal, bufLen, bufVirAddr, bufPhyAddr, returnIter) (*(hal)->ReturnRxDesc)(hal, bufLen, bufVirAddr, bufPhyAddr, returnIter)
#define HalGetRxStatus(hal, rxDescIter, rxDescStatus) (*(hal)->GetRxStatus)(hal, rxDescIter, rxDescStatus)
#define HalReserveRxDesc(hal, rxDescIter) (*(hal)->ReserveRxDesc)(hal, rxDescIter)
#define HalReleaseRxDescs(hal) (*(hal)->ReleaseRxDescs)(hal)
#define HalResetRxDescs(hal) (*(hal)->ResetRxDescs)(hal)
#define HalIsRxDescReady(hal, rxDescIter) (*(hal)->IsRxDescReady)(hal, rxDescIter)
#define HalStartReceive(hal) (*(hal)->StartReceive)(hal)

#define HalFreeNic(hal) (*(hal)->FreeNic)(hal)
#define HalGetEncryptionCapabilities(hal) (*(hal)->GetEncryptionCapabilities)(hal)

#define HalEnableInterrupt(hal) (*(hal)->EnableInterrupt)(hal)
#define HalDisableInterrupt(hal) (*(hal)->DisableInterrupt)(hal)
#define HalInterlockedAndIntrMask(hal, intrMask) (*(hal)->InterlockedAndIntrMask)(hal, intrMask)
#define HalInterlockedOrIntrMask(hal, intrMask) (*(hal)->InterlockedOrIntrMask)(hal, intrMask)
#define HalSetIntrMask(hal, intrMask) (*(hal)->SetIntrMask)(hal, intrMask)
#define HalSetInterruptHandle(hal, intrHandle) (*(hal)->SetInterruptHandle)(hal, intrHandle)

#define HalIsInterruptEnabled(hal) (*(hal)->IsInterruptEnabled)(hal)
#define HalClearInterrupt(hal) (*(hal)->ClearInterrupt)(hal)
#define HalGetIntrMask(hal) (*(hal)->GetIntrMask)(hal)

#define HalHaltNic(hal) (*(hal)->HaltNic)(hal)
#define HalParsePciConfiguration(hal, buffer, bufferSize) (*(hal)->ParsePciConfiguration)(hal, buffer, bufferSize)
#define HalAddAdapterResource(hal, resource) (*(hal)->AddAdapterResource)(hal, resource)
#define HalSetDevicePowerState(hal, devicePowerState) (*(hal)->SetDevicePowerState)(hal, devicePowerState)
#define HalSetMulticastMask(hal, bAcceptAll, addrCount, addrList) (*(hal)->SetMulticastMask)(hal, bAcceptAll, addrCount, addrList)
#define HalRemoveKeyEntry(hal, index) (*(hal)->RemoveKeyEntry)(hal, index)
#define HalAddKeyEntry(hal, key, index) (*(hal)->AddKeyEntry)(hal, key, index)
#define HalUpdateKeyEntry(hal, key, index) (*(hal)->UpdateKeyEntry)(hal, key, index)
#define HalSetEncryption(hal, algoId) (*(hal)->SetEncryption)(hal, algoId)

#define HalResetStart(hal) (*(hal)->ResetStart)(hal)
#define HalResetEnd(hal) (*(hal)->ResetEnd)(hal)

#define HalSetPacketFilter(hal, packetFilter) (*(hal)->SetPacketFilter)(hal, packetFilter)
#define HalCheckForSends(hal, queueId, forceSend) (*(hal)->CheckForSends)(hal, queueId, forceSend)


#define HalStartBSS(hal, lastBeaconTimestamp) (*(hal)->StartBSS)(hal, lastBeaconTimestamp)
#define HalStopBSS(hal) (*(hal)->StopBSS)(hal)
#define HalSetAssociateId(hal, id) (*(hal)->SetAssociateId)(hal, id);
#define HalUpdateConnectionState(hal, state) (*(hal)->UpdateConnectionState)(hal, state)

#define HalPauseBSS(hal) (*(hal)->PauseBSS)(hal)
#define HalResumeBSS(hal) (*(hal)->ResumeBSS)(hal)


#endif
