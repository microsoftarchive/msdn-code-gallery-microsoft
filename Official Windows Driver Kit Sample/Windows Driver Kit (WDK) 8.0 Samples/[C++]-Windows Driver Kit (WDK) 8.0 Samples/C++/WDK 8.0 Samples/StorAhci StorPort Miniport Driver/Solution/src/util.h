/*++

Copyright (C) Microsoft Corporation, 2009

Module Name:

    util.h

Abstract:

    Internal support routines used in STORAHCI

Notes:

Revision History:

        Nathan Obr (natobr),  February 2005
        Michael Xing (xiaoxing),  December 2009
--*/


#pragma once

_At_buffer_(Buffer, _I_, BufferSize, _Post_equal_to_(0))
__inline
VOID
AhciZeroMemory(
    _Out_writes_(BufferSize) PCHAR Buffer,
    _In_ ULONG BufferSize
    )
{
    ULONG i;

    for (i = 0; i < BufferSize; i++) {
        Buffer[i] = 0;
    }
}

_At_buffer_(Buffer, _I_, BufferSize, _Post_equal_to_(Fill))
__inline
VOID
AhciFillMemory(
    _Out_writes_(BufferSize) PCHAR Buffer,
    _In_ ULONG BufferSize,
    _In_ CHAR  Fill
    )
{
    ULONG i;

    for (i = 0; i < BufferSize; i++) {
        Buffer[i] = Fill;
    }
}

__inline
BOOLEAN
IsPortValid(
    _In_ PAHCI_ADAPTER_EXTENSION AdapterExtension,
    _In_ ULONG PathId
    )
{
    return ( (PathId <= AdapterExtension->HighestPort) && (AdapterExtension->PortExtension[PathId] != NULL) );
}

__inline
BOOLEAN
IsPortStartCapable(
    _In_ PAHCI_CHANNEL_EXTENSION ChannelExtension
    )
/*
    Indicates if a port can be started or port can process IO
*/
{
    return ( (ChannelExtension->AdapterExtension->StateFlags.StoppedState == 0) &&
             (ChannelExtension->AdapterExtension->StateFlags.PowerDown == 0) &&
             (ChannelExtension->StateFlags.Initialized == 1) );
}


__inline
PAHCI_SRB_EXTENSION
GetSrbExtension (
    _In_ PSCSI_REQUEST_BLOCK Srb
    )
{
    //use lower 32bit is good enough for this calculation even on 64bit system, as 128 only takes 8 bits.
    ULONG_PTR left = ((ULONG_PTR)Srb->SrbExtension) % 128;

    if (left == 0) {
        //the buffer is already aligned.
        return (PAHCI_SRB_EXTENSION)Srb->SrbExtension;
    } else {
        //need to align to 128.
        return (PAHCI_SRB_EXTENSION)( ((PCHAR)Srb->SrbExtension) + 128 - left );
    }
}

__inline
VOID
MarkSrbToBeCompleted(
    _In_ PSCSI_REQUEST_BLOCK Srb
    )
{
    PAHCI_SRB_EXTENSION srbExtension = GetSrbExtension(Srb);
    SETMASK(srbExtension->Flags, ATA_FLAGS_COMPLETE_SRB);
}

__inline
BOOLEAN
IsDataTransferNeeded(
    _In_ PSCSI_REQUEST_BLOCK Srb
    )
{
    PAHCI_SRB_EXTENSION srbExtension = GetSrbExtension(Srb);
    return ( (srbExtension->Flags & (ATA_FLAGS_DATA_IN | ATA_FLAGS_DATA_OUT)) || (srbExtension->DataBuffer != NULL) );
}


__inline
BOOLEAN
IsDumpMode(
    _In_ PAHCI_ADAPTER_EXTENSION AdapterExtension
    )
/*
Return Value:
    TRUE: miniport works in crashdump, hibernate etc.
    FALSE: miniport works in normal mode.
*/
{
    return (AdapterExtension->DumpMode > 0);
}

__inline
BOOLEAN
IsDumpCrashMode(
    _In_ PAHCI_ADAPTER_EXTENSION AdapterExtension
    )
{
    return (AdapterExtension->DumpMode == DUMP_MODE_CRASH);
}

__inline
BOOLEAN
IsDumpHiberMode(
    _In_ PAHCI_ADAPTER_EXTENSION AdapterExtension
    )
{
    return (AdapterExtension->DumpMode == DUMP_MODE_HIBER);
}

__inline
BOOLEAN
IsDumpMarkMemoryMode(
    _In_ PAHCI_ADAPTER_EXTENSION AdapterExtension
    )
{
    return (AdapterExtension->DumpMode == DUMP_MODE_MARK_MEMORY);
}

__inline
BOOLEAN
IsDumpResumeMode(
    _In_ PAHCI_ADAPTER_EXTENSION AdapterExtension
    )
{
    return (AdapterExtension->DumpMode == DUMP_MODE_RESUME);
}


__inline
BOOLEAN
IsMsnEnabled(
    _In_ PAHCI_DEVICE_EXTENSION DeviceExtension
    )
{
    return ( (DeviceExtension->DeviceParameters.AtaDeviceType == DeviceIsAta) &&
             (DeviceExtension->IdentifyDeviceData->MsnSupport == 1) );
}

__inline
BOOLEAN
IsRmfEnabled(
    _In_ PAHCI_DEVICE_EXTENSION DeviceExtension
    )
{
    return ( (DeviceExtension->DeviceParameters.AtaDeviceType == DeviceIsAta) &&
             (DeviceExtension->IdentifyDeviceData->CommandSetSupport.RemovableMediaFeature == 1) &&
             (DeviceExtension->IdentifyDeviceData->GeneralConfiguration.RemovableMedia == 1) );
}

__inline
BOOLEAN
NoFlushDevice(
    _In_ PAHCI_DEVICE_EXTENSION DeviceExtension
    )
{
    return ( (DeviceExtension->DeviceParameters.AtaDeviceType == DeviceIsAta) &&
             (DeviceExtension->IdentifyDeviceData->CommandSetSupport.WriteCache == 0) &&
             (DeviceExtension->IdentifyDeviceData->CommandSetSupport.FlushCache == 0) &&
             (DeviceExtension->IdentifyDeviceData->CommandSetSupport.FlushCacheExt == 0) );
}

__inline
BOOLEAN
IsTapeDevice(
    _In_ PAHCI_DEVICE_EXTENSION DeviceExtension
    )
{
    return ( (DeviceExtension->DeviceParameters.AtaDeviceType == DeviceIsAtapi) &&
             (DeviceExtension->IdentifyPacketData->GeneralConfiguration.CommandPacketType == 1) );
}


__inline
BOOLEAN
IsNCQCommand (
    _In_ PAHCI_SRB_EXTENSION SrbExtension
    )
/*++
    This could be a macro

Return Value:
    TRUE if CommandRegister is 0x60, 0x61, 0x63, 0x64 or 0x65
--*/
{
    UCHAR command = IDE_COMMAND_NOT_VALID;

    // NvCachePassCommand needs to be checked first as it's also ATA command
    if ( IsAtaHintPayload(SrbExtension->AtaFunction) ) {
        command = SrbExtension->HintPayload.Command;
    } else if ( IsAtaCommand(SrbExtension->AtaFunction) ) {
        command = SrbExtension->TaskFile.Current.bCommandReg;
    }

    if ( (command == IDE_COMMAND_READ_FPDMA_QUEUED) ||
         (command == IDE_COMMAND_WRITE_FPDMA_QUEUED) ||
         (command == IDE_COMMAND_NCQ_NON_DATA) ||
         (command == IDE_COMMAND_SEND_FPDMA_QUEUED) ||
         (command == IDE_COMMAND_RECEIVE_FPDMA_QUEUED) ) {
        return TRUE;
    } else {
        return FALSE;
    }
}

__inline
BOOLEAN
IsNCQWriteCommand (
    _In_ PAHCI_SRB_EXTENSION SrbExtension
    )
/*++
    This could be a macro

Return Value:
    TRUE if CommandRegister is 0x60, 0x61, 0x63, 0x64 or 0x65
--*/
{
    UCHAR command = IDE_COMMAND_NOT_VALID;

    // NvCachePassCommand needs to be checked first as it's also ATA command
    if ( IsAtaHintPayload(SrbExtension->AtaFunction) ) {
        command = SrbExtension->HintPayload.Command;
    } else if ( IsAtaCommand(SrbExtension->AtaFunction) ) {
        command = SrbExtension->TaskFile.Current.bCommandReg;
    }

    if (command == IDE_COMMAND_WRITE_FPDMA_QUEUED) {
        return TRUE;
    } else {
        return FALSE;
    }
}

__inline
BOOLEAN
IsNormalCommand (
    _In_ PSCSI_REQUEST_BLOCK Srb
    )
{
/*++
    This could be a macro

Return Value:
    TRUE if any Command Register is any non NCQ IO command
--*/
    PAHCI_SRB_EXTENSION srbExtension = GetSrbExtension(Srb);

    if ((srbExtension->TaskFile.Current.bCommandReg == IDE_COMMAND_READ) ||
        (srbExtension->TaskFile.Current.bCommandReg == IDE_COMMAND_WRITE) ||
        (srbExtension->TaskFile.Current.bCommandReg == IDE_COMMAND_READ_EXT) ||
        (srbExtension->TaskFile.Current.bCommandReg == IDE_COMMAND_WRITE_EXT) ||
        (srbExtension->TaskFile.Current.bCommandReg == IDE_COMMAND_READ_DMA) ||
        (srbExtension->TaskFile.Current.bCommandReg == IDE_COMMAND_WRITE_DMA) ||
        (srbExtension->TaskFile.Current.bCommandReg == IDE_COMMAND_READ_DMA_EXT) ||
        (srbExtension->TaskFile.Current.bCommandReg == IDE_COMMAND_WRITE_DMA_EXT) ||
        (srbExtension->TaskFile.Current.bCommandReg == IDE_COMMAND_WRITE_DMA_FUA_EXT) ||
        (srbExtension->TaskFile.Current.bCommandReg == IDE_COMMAND_READ_DMA_QUEUED_EXT) ||
        (srbExtension->TaskFile.Current.bCommandReg == IDE_COMMAND_WRITE_DMA_QUEUED_EXT) ||
        (srbExtension->TaskFile.Current.bCommandReg == IDE_COMMAND_WRITE_DMA_QUEUED_FUA_EXT) ||
        (srbExtension->TaskFile.Current.bCommandReg == IDE_COMMAND_READ_MULTIPLE) ||
        (srbExtension->TaskFile.Current.bCommandReg == IDE_COMMAND_WRITE_MULTIPLE) ||
        (srbExtension->TaskFile.Current.bCommandReg == IDE_COMMAND_READ_MULTIPLE_EXT) ||
        (srbExtension->TaskFile.Current.bCommandReg == IDE_COMMAND_WRITE_MULTIPLE_EXT) ||
        (srbExtension->TaskFile.Current.bCommandReg == IDE_COMMAND_WRITE_MULTIPLE_FUA_EXT) ||
        (srbExtension->TaskFile.Current.bCommandReg == IDE_COMMAND_WRITE_DMA_QUEUED) ||
        (srbExtension->AtaFunction == ATA_FUNCTION_ATAPI_COMMAND) ) {
        return TRUE;
    } else {
        return FALSE;
    }
}

__inline
BOOLEAN
NeedRequestSense (
    _In_ PSCSI_REQUEST_BLOCK Srb
    )
{
    PAHCI_SRB_EXTENSION srbExtension = GetSrbExtension(Srb);

    return ( IsAtapiCommand(srbExtension->AtaFunction) &&
             !IsRequestSenseSrb(srbExtension->AtaFunction) &&
             (Srb->SrbStatus == SRB_STATUS_ERROR) &&
             (Srb->SenseInfoBuffer != NULL) &&
             (Srb->SenseInfoBufferLength > 0) );
}

BOOLEAN
__inline
IsDeviceSupportsTrim (
    _In_ PAHCI_CHANNEL_EXTENSION ChannelExtension
    )
{
    // word 169: bit 0 -- Trim function is supported.
    return (ChannelExtension->DeviceExtension[0].IdentifyDeviceData->DataSetManagementFeature.SupportsTrim == 1);
}


__inline
BOOLEAN
IsDeviceSupportsAN(
    _In_ PIDENTIFY_PACKET_DATA IdentifyPacketData
    )
{
    // This bit is actually from IDENTIFY_PACKET_DATA structure for ATAPI devices.
    return (IdentifyPacketData->SerialAtaFeaturesSupported.AsynchronousNotification == TRUE);
}

__inline
BOOLEAN
IsDeviceEnabledAN(
    _In_ PIDENTIFY_PACKET_DATA IdentifyPacketData
    )
{
    // This bit is actually from IDENTIFY_PACKET_DATA structure for ATAPI devices.
    return (IdentifyPacketData->SerialAtaFeaturesEnabled.AsynchronousNotification == TRUE);
}

__inline
BOOLEAN
IsExternalPort(
    _In_ PAHCI_CHANNEL_EXTENSION ChannelExtension
    )
{
    if ( (ChannelExtension->Px->CMD.HPCP) ||
         ((ChannelExtension->AdapterExtension->CAP.SXS) && (ChannelExtension->Px->CMD.ESP)) ||
         ((ChannelExtension->AdapterExtension->CAP.SMPS) && (ChannelExtension->Px->CMD.MPSP)) ) {
        //1. Px->CMD.HPCP indicates that the port is hot-pluggable. (both signal and power cable)
        //2. CAP.SXS && Px->CMD.ESP indicates that it's an ESATA port. (only signal cable)
        //3. CAP.SMPS && Px->CMD.MPSP indicates that Mechanical Switch is implemented on the port.
        return TRUE;
    }

    return FALSE;
}

__inline
BOOLEAN
IsLPMCapablePort(
    _In_ PAHCI_CHANNEL_EXTENSION ChannelExtension
    )
{
    // if a port is marked eSATA port but not having mechanical switch nor supporting Cold Presence Detection, don't turn on LPM on it.
    if ( (ChannelExtension->Px->CMD.HPCP) ||
         ((ChannelExtension->AdapterExtension->CAP.SXS) && (ChannelExtension->Px->CMD.ESP)) ) {

        if ( ((ChannelExtension->AdapterExtension->CAP.SMPS == 0) || (ChannelExtension->Px->CMD.MPSP == 0)) &&
             (ChannelExtension->Px->CMD.CPD == 0) ) {
            return FALSE;
        }
    }

    return TRUE;
}

__inline
BOOLEAN
IsDeviceHybridInfoEnabled(
    _In_ PAHCI_CHANNEL_EXTENSION ChannelExtension
    )
{

    if ( (ChannelExtension->DeviceExtension[0].DeviceParameters.AtaDeviceType ==  DeviceIsAta) &&
         (ChannelExtension->DeviceExtension[0].IdentifyDeviceData->SerialAtaFeaturesSupported.HybridInformation == 1) &&
         (ChannelExtension->DeviceExtension[0].IdentifyDeviceData->SerialAtaFeaturesEnabled.HybridInformation == 1) ) {

        return TRUE;
    }

    return FALSE;
}

__inline
BOOLEAN
IsD3ColdAllowed(
    _In_ PAHCI_ADAPTER_EXTENSION AdapterExtension
    )
{
    BOOLEAN allowed = TRUE;

    UNREFERENCED_PARAMETER(AdapterExtension);


    return allowed;
}

__inline
BOOLEAN
IsPortD3ColdEnabled(
    _In_ PAHCI_CHANNEL_EXTENSION ChannelExtension
    )
{
    // if device is D3Cold capable, return TRUE.
    if (ChannelExtension->StateFlags.D3ColdEnabled == 1) {
        return TRUE;
    }

    return FALSE;
}










__inline
BOOLEAN
AdapterPoFxEnabled (
    _In_ PAHCI_ADAPTER_EXTENSION AdapterExtension
)
{
    return (AdapterExtension->StateFlags.PoFxEnabled == 1);
}

__inline
BOOLEAN
PortPoFxEnabled (
    _In_ PAHCI_CHANNEL_EXTENSION ChannelExtension
    )
{
    return (ChannelExtension->StateFlags.PoFxEnabled == 1);
}

__inline
BOOLEAN
AdapterAcquireActiveReference (
    _In_ PAHCI_ADAPTER_EXTENSION AdapterExtension,
    _Inout_opt_ PBOOLEAN         InIdle
    )
{
    BOOLEAN idle = FALSE;
    BOOLEAN referenceAcquired = FALSE;

    if (AdapterPoFxEnabled(AdapterExtension)) {
        ULONG status;
        status = StorPortPoFxActivateComponent(AdapterExtension,
                                               NULL,
                                               NULL,
                                               0,
                                               0);
        // STOR_STATUS_BUSY indicates that ActivateComponent is not completed yet.
        idle = (status == STOR_STATUS_BUSY);

        if ( (status == STOR_STATUS_SUCCESS) || (status == STOR_STATUS_BUSY) ) {
            referenceAcquired = TRUE;
        }
    }

    if (InIdle != NULL) {
        *InIdle = idle;
    }

    return referenceAcquired;
}

__inline
VOID
AdapterReleaseActiveReference (
    _In_ PAHCI_ADAPTER_EXTENSION AdapterExtension
    )
{
    if (AdapterPoFxEnabled(AdapterExtension)) {
        StorPortPoFxIdleComponent(AdapterExtension,
                                  NULL,
                                  NULL,
                                  0,
                                  0);
    }
}

__inline
BOOLEAN
PortAcquireActiveReference (
    _In_ PAHCI_CHANNEL_EXTENSION ChannelExtension,
    _In_opt_ PSCSI_REQUEST_BLOCK Srb,
    _Inout_opt_ PBOOLEAN         InIdle
    )
{
    BOOLEAN idle = FALSE;
    BOOLEAN referenceAcquired = FALSE;

    if (PortPoFxEnabled(ChannelExtension)) {
        ULONG status;

        status = StorPortPoFxActivateComponent(ChannelExtension->AdapterExtension,
                                               (PSTOR_ADDRESS)&ChannelExtension->DeviceExtension[0].DeviceAddress,
                                               Srb,
                                               0,
                                               0);
        // STOR_STATUS_BUSY indicates that ActivateComponent is not completed yet.
        idle = (status == STOR_STATUS_BUSY);

        if ( (status == STOR_STATUS_SUCCESS) || (status == STOR_STATUS_BUSY) ) {
            if (Srb != NULL) {
                PAHCI_SRB_EXTENSION srbExtension = GetSrbExtension(Srb);
                SETMASK(srbExtension->Flags, ATA_FLAGS_ACTIVE_REFERENCE);
            }

            referenceAcquired = TRUE;
        }

    } else {
        UNREFERENCED_PARAMETER(Srb);
    }

    if (InIdle != NULL) {
        *InIdle = idle;
    }

    return referenceAcquired;
}

__inline
VOID
PortReleaseActiveReference (
    _In_ PAHCI_CHANNEL_EXTENSION ChannelExtension,
    _In_opt_ PSCSI_REQUEST_BLOCK Srb
    )
{
    if (PortPoFxEnabled(ChannelExtension)) {
        if (Srb != NULL) {
            PAHCI_SRB_EXTENSION srbExtension = GetSrbExtension(Srb);
            CLRMASK(srbExtension->Flags, ATA_FLAGS_ACTIVE_REFERENCE);
        }

        StorPortPoFxIdleComponent(ChannelExtension->AdapterExtension,
                                  (PSTOR_ADDRESS)&ChannelExtension->DeviceExtension[0].DeviceAddress,
                                  Srb,
                                  0,
                                  0);
    } else {
        UNREFERENCED_PARAMETER(Srb);
    }
}


__inline
BOOLEAN
DeviceIdentificationComplete(
    _In_ PAHCI_ADAPTER_EXTENSION AdapterExtension
    )
{
    ULONG i;

    for (i = 0; i <= AdapterExtension->HighestPort; i++) {
        if ( (AdapterExtension->PortExtension[i] != NULL) &&
             (AdapterExtension->PortExtension[i]->DeviceExtension->DeviceParameters.AtaDeviceType == DeviceUnknown) ) {
            return FALSE;
        }
    }

    return TRUE;
}

__inline
PULONG
SrbGetDataTransferLength (
    _In_ PSCSI_REQUEST_BLOCK Srb
    )
{
    PAHCI_SRB_EXTENSION srbExtension = GetSrbExtension(Srb);

    return ((srbExtension->DataTransferLength > 0) ? &srbExtension->DataTransferLength : &Srb->DataTransferLength);
}

__inline
PCDB
SrbGetCdb (
    _In_ PSCSI_REQUEST_BLOCK Srb
    )
{
    PAHCI_SRB_EXTENSION srbExtension = GetSrbExtension(Srb);

    return (IsNewCdbUsed(srbExtension->Flags) ? &srbExtension->Cdb : (PCDB)Srb->Cdb);
}

__inline
UCHAR
NumberOfSetBits (
    _In_ ULONG Value
    )
/*++
    This routine emulates the __popcnt intrinsic function.

Return Value:
    Count of '1's in the ULONG value
--*/
{
    //
    // Partition into groups of bit pairs. Compute population count for each
    // bit pair.
    //
    Value -= (Value >> 1) & 0x55555555;

    //
    // Sum population count of adjacent pairs into quads.
    //
    Value = (Value & 0x33333333) + ((Value >> 2) & 0x33333333);

    //
    // Sum population count of adjacent quads into octets. Lower quad in each
    // octet has desired sum and upper quad is garbage.
    //
    Value = (Value + (Value >> 4)) & 0x0F0F0F0F;

    //
    // The lower quads in each octet must now be accumulated by multiplying with
    // a magic multiplier:
    //
    //   0p0q0r0s * 0x01010101 =         :0p0q0r0s
    //                                 0p:0q0r0s
    //                               0p0q:0r0s
    //                             0p0q0r:0s
    //                           000pxxww:vvuutt0s
    //
    // The octet vv contains the final interesting result.
    //
    Value *= 0x01010101;

    return (UCHAR)(Value >> 24);
}

VOID
RecordExecutionHistory(
    PAHCI_CHANNEL_EXTENSION ChannelExtension,
    ULONG Function
  );

VOID
RecordInterruptHistory(
    PAHCI_CHANNEL_EXTENSION ChannelExtension,
    ULONG PxIS,
    ULONG PxSSTS,
    ULONG PxSERR,
    ULONG PxCI,
    ULONG PxSACT,
    ULONG Function
  );

VOID
Set_PxIE(
    PAHCI_CHANNEL_EXTENSION ChannelExtension,
    PAHCI_INTERRUPT_ENABLE IE
    );


__inline
BOOLEAN
IsFuaSupported(
    _In_ PAHCI_CHANNEL_EXTENSION ChannelExtension
    )
{
    return (ChannelExtension->DeviceExtension->DeviceParameters.StateFlags.FuaSupported == 1);
}

__inline
BOOLEAN
IsDeviceSupportsHIPM(
    _In_ PIDENTIFY_DEVICE_DATA IdentifyDeviceData
    )
{
    return (IdentifyDeviceData->SerialAtaCapabilities.HIPM == TRUE);
}

__inline
BOOLEAN
IsDeviceSupportsDIPM(
    _In_ PIDENTIFY_DEVICE_DATA IdentifyDeviceData
    )
{
    return (IdentifyDeviceData->SerialAtaFeaturesSupported.DIPM == TRUE);
}

__inline
BOOLEAN
NoLpmSupport(
    _In_ PAHCI_CHANNEL_EXTENSION ChannelExtension
    )
{
    if (ChannelExtension->AdapterExtension->StateFlags.StoppedState == 1) {
        // adapter is stopped, Px registers are not accessible.
        return TRUE;
    }

    if (ChannelExtension->AdapterExtension->CAP.PSC == 0) {
        // adapter doesn't support Partial state
        return TRUE;
    }

    // return TRUE if HIPM and DIPM are not supported.
    return ( (ChannelExtension->AdapterExtension->CAP.SALP == 0) &&
             (!IsDeviceSupportsDIPM(ChannelExtension->DeviceExtension[0].IdentifyDeviceData)) );
}


__inline
BOOLEAN
NeedToSetTransferMode(
    _In_ PAHCI_CHANNEL_EXTENSION ChannelExtension
    )
{
    UNREFERENCED_PARAMETER(ChannelExtension); // make WDK sample build clean
    return FALSE;
}

__inline
BOOLEAN
IsSingleIoDevice(
    _In_ PAHCI_ADAPTER_EXTENSION AdapterExtension
    )
{
    UNREFERENCED_PARAMETER(AdapterExtension); // make WDK sample build clean
    return FALSE;
}

__inline
BOOLEAN
AdapterResetInInit(
    _In_ PAHCI_ADAPTER_EXTENSION AdapterExtension
    )
{
    UNREFERENCED_PARAMETER(AdapterExtension); // make WDK sample build clean
    return FALSE;
}

__inline
BOOLEAN
IgnoreHotPlug(
    _In_ PAHCI_CHANNEL_EXTENSION ChannelExtension
    )
{
    return (ChannelExtension->StateFlags.IgnoreHotplugInterrupt == 1);
}

__inline
BOOLEAN
AdapterNoNonQueuedErrorRecovery(
    _In_ PAHCI_ADAPTER_EXTENSION AdapterExtension
    )
{
    UNREFERENCED_PARAMETER(AdapterExtension); // make WDK sample build clean
    return FALSE;
}

__inline
BOOLEAN
CloResetEnabled(
    _In_ PAHCI_ADAPTER_EXTENSION AdapterExtension
    )
{
    UNREFERENCED_PARAMETER(AdapterExtension); // make WDK sample build clean
    return (AdapterExtension->CAP.SCLO == 1);
}

__inline
BOOLEAN
IsNCQSupported(
    _In_ PAHCI_CHANNEL_EXTENSION ChannelExtension
    )
{
    if ( IsAtaDevice(&ChannelExtension->DeviceExtension->DeviceParameters) &&
         (ChannelExtension->AdapterExtension->CAP.SNCQ == 1) &&
         (ChannelExtension->DeviceExtension->IdentifyDeviceData->SerialAtaCapabilities.NCQ == 1) &&
         (ChannelExtension->DeviceExtension->IdentifyDeviceData->QueueDepth > 1) ) {
        //Queue Depth is a 0 based value (i.e. 0x0 == 1). StorAHCI reserves slot 0 for internal use.
        return TRUE;
    }
    return FALSE;
}

__inline
ULONG
GetOccupiedSlots (
    _In_ PAHCI_CHANNEL_EXTENSION ChannelExtension
    )
{
    return ( ChannelExtension->SlotManager.CommandsIssued |
             ChannelExtension->SlotManager.NCQueueSlice |
             ChannelExtension->SlotManager.NormalQueueSlice |
             ChannelExtension->SlotManager.SingleIoSlice |
             ChannelExtension->SlotManager.CommandsToComplete );
}

__inline
BOOLEAN
ErrorRecoveryIsPending (
    _In_ PAHCI_CHANNEL_EXTENSION ChannelExtension
    )
{
    return ( (ChannelExtension->StateFlags.CallAhciReset == 1) ||
             (ChannelExtension->StateFlags.CallAhciReportBusChange == 1) ||
             (ChannelExtension->StateFlags.CallAhciNonQueuedErrorRecovery == 1) );
}

__inline
BOOLEAN
IsMiniportInternalSrb (
    _In_ PAHCI_CHANNEL_EXTENSION ChannelExtension,
    _In_ PSCSI_REQUEST_BLOCK Srb
    )
{
    return ( (Srb == &ChannelExtension->Local.Srb) || (Srb == &ChannelExtension->Sense.Srb) );
}

__inline
VOID
PortClearPendingInterrupt (
    _In_ PAHCI_CHANNEL_EXTENSION ChannelExtension
    )
{
    StorPortWriteRegisterUlong(ChannelExtension->AdapterExtension, &ChannelExtension->Px->SERR.AsUlong, (ULONG)~0);
    StorPortWriteRegisterUlong(ChannelExtension->AdapterExtension, &ChannelExtension->Px->IS.AsUlong, (ULONG)~0);
    StorPortWriteRegisterUlong(ChannelExtension->AdapterExtension, ChannelExtension->AdapterExtension->IS, (1 << ChannelExtension->PortNumber));
}

__inline
BOOLEAN
PartialToSlumberTransitionIsAllowed (
    _In_ PAHCI_CHANNEL_EXTENSION ChannelExtension,
    _In_ AHCI_COMMAND            CMD,
    _In_ ULONG                   CI,
    _In_ ULONG                   SACT
    )
{
    if ( (CI != 0) || (SACT != 0) ) {
        //device still has request pending
        return FALSE;
    }

    if ((ChannelExtension->LastUserLpmPowerSetting  & 0x3) == 0) {
        //Neither HIPM nor DIPM is allowed. e.g. LastUserLpmPowerSetting --- bit 0: HIPM; bit 1: DIPM
        return FALSE;
    }

    if ((ChannelExtension->AutoPartialToSlumberInterval == 0)) {
        //Software auto Partial to Slumber is not enabled yet.
        return FALSE;
    }

    if (ChannelExtension->StartState.ChannelNextStartState != StartComplete) {
        //port is not started yet
        return FALSE;
    }

    if ( NoLpmSupport(ChannelExtension) || !IsLPMCapablePort(ChannelExtension) ) {
        //link power management is not supported
        return FALSE;
    }

    if ( ( (ChannelExtension->AdapterExtension->CAP.SALP == 0) || (CMD.ALPE == 0) ) &&
         (!IsDeviceSupportsDIPM(ChannelExtension->DeviceExtension[0].IdentifyDeviceData)) ) {
        //Neither HIPM nor DIPM is enabled.
        return FALSE;
    }

    if ( ((ChannelExtension->LastUserLpmPowerSetting  & 0x2) == 1) &&
         (ChannelExtension->DeviceExtension->IdentifyDeviceData->SerialAtaCapabilities.DeviceAutoPS == 1) &&
         (ChannelExtension->DeviceExtension->IdentifyDeviceData->SerialAtaFeaturesEnabled.DeviceAutoPS == 1) )   {
        //DIPM is enabled; device supports and enabled Device auto Partial to Slumber.
        //note that IdentifyDeviceData applies to both ATA and ATAPI devices.
        return FALSE;
    }

    if ( (ChannelExtension->AdapterExtension->CAP.SALP == 1) &&
         (CMD.ALPE != 0) &&
         ( (CMD.ASP == 1) ||
           ( (ChannelExtension->AdapterExtension->CAP2.APST != 0) && (CMD.APSTE != 0) ) ) ) {
        // HIPM is enabled. AND
        //     either Host initiates Slumber automatically, OR
        //     Host supports and enabled auto Partial to Slumber.
        return FALSE;
    }

    return TRUE;
}


VOID
PortBusChangeProcess (
    _In_ PAHCI_CHANNEL_EXTENSION ChannelExtension
    );

PAHCI_MEMORY_REGISTERS
GetABARAddress(
    _In_ PAHCI_ADAPTER_EXTENSION AdapterExtension,
    _In_ PPORT_CONFIGURATION_INFORMATION ConfigInfo
    );

VOID
AhciCompleteJustSlottedRequest(
    _In_ PAHCI_CHANNEL_EXTENSION ChannelExtension,
    _In_ PSCSI_REQUEST_BLOCK Srb,
    _In_ BOOLEAN AtDIRQL
    );

VOID
AhciCompleteRequest(
    _In_ PAHCI_CHANNEL_EXTENSION ChannelExtension,
    _In_ PSCSI_REQUEST_BLOCK Srb,
    _In_ BOOLEAN AtDIRQL
    );

BOOLEAN
UpdateSetFeatureCommands(
    _In_ PAHCI_CHANNEL_EXTENSION ChannelExtension,
    _In_ UCHAR OldFeatures,
    _In_ UCHAR NewFeatures,
    _In_ UCHAR OldSectorCount,
    _In_ UCHAR NewSectorCount
  );

VOID
GetAvailableSlot(
    PAHCI_CHANNEL_EXTENSION ChannelExtension,
    PSCSI_REQUEST_BLOCK Srb
    );

VOID
ReleaseSlottedCommand(
    _In_ PAHCI_CHANNEL_EXTENSION ChannelExtension,
    _In_ UCHAR SlotNumber,
    _In_ BOOLEAN AtDIRQL
    );

VOID
RestorePreservedSettings(
    _In_ PAHCI_CHANNEL_EXTENSION ChannelExtension,
    _In_ BOOLEAN AtDIRQL
    );

VOID
AhciPortIssueInitCommands(
    PAHCI_CHANNEL_EXTENSION ChannelExtension
  );

_Success_(return != FALSE)
BOOLEAN
CompareId (
    _In_opt_ PSTR DeviceId,
    _In_ ULONG  DeviceIdLength,
    _In_opt_ PZZSTR TargetId,
    _In_ ULONG  TargetIdLength,
    _Inout_opt_ PULONG Value
);

ULONG
GetStringLength (
    _In_ PSTR   String,
    _In_ ULONG  MaxLength
    );


