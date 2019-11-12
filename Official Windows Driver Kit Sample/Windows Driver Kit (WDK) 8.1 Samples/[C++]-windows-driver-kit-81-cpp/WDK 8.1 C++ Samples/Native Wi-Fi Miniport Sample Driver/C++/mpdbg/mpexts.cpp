/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    mpexts.c

Abstract:
    Kernel debugger extension for Atheros Wireless Sample Driver
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#include "precomp.h"

__inline
PSTR
GetPortTypeString(
    ULONG PortType
    )
{
    PSTR pszString = NULL;
    
    switch (PortType)
    {
        case HELPER_PORT:
            pszString = "Helper";
            break;
        case EXTSTA_PORT:
            pszString = "Extensible Station";
            break;
        case EXTAP_PORT:
            pszString = "Extensible AP";
            break;
        default:
            pszString = "Unknown Port Type";
            break;

    }

    return pszString;
}

__inline
PSTR
GetConnStateString(
    ULONG PortType
    )
{
    PSTR pszString = NULL;
    
    switch (PortType)
    {
        case CONN_STATE_DISCONNECTED:
            pszString = "Disconnected";
            break;
        case CONN_STATE_IN_RESET:
            pszString = "Reset";
            break;
        case CONN_STATE_READY_TO_CONNECT:
            pszString = "Ready to Connect";
            break;
        case CONN_STATE_READY_TO_ROAM:
            pszString = "Ready to Roam";
            break;
        default:
            pszString = "Unknown Connection State";
            break;
    }

    return pszString;
}


__inline
PSTR
GetAssocStateString(
    ULONG AssocState
    )
{
    PSTR pszString = NULL;

    switch (AssocState)
    {

        case ASSOC_STATE_NOT_ASSOCIATED:
            pszString = "Not Associated";
            break;
        case ASSOC_STATE_READY_TO_ASSOCIATE:
            pszString = "Ready to associate";
            break;
        case ASSOC_STATE_STARTED_ASSOCIATION:
            pszString = "Started association";
            break;
        case ASSOC_STATE_WAITING_FOR_JOIN:
            pszString = "Waiting for Join";
            break;
        case ASSOC_STATE_JOINED:
            pszString = "Joined";
            break;
        case ASSOC_STATE_REMOTELY_DEAUTHENTICATED:
            pszString = "Remotely Deauthenticated";
            break;
        case ASSOC_STATE_WAITING_FOR_AUTHENTICATE:
            pszString = "Waiting for authenticate";
            break;
        case ASSOC_STATE_RECEIVED_AUTHENTICATE:
            pszString = "Received authenticate";
            break;
        case ASSOC_STATE_REMOTELY_DISASSOCIATED:
            pszString = "Remotely Disassociated";
            break;
        case ASSOC_STATE_WAITING_FOR_ASSOCIATE:
            pszString = "Waiting for Associate";
            break;
        case ASSOC_STATE_RECEIVED_ASSOCIATE:
            pszString = "Received Associate";
            break;
        case ASSOC_STATE_ASSOCIATED:
            pszString = "Associated";
            break;
        default:
            pszString = "Unknown Association State";
            break;
    }

    return pszString;
}

__inline
PSTR
GetPendingOpString(
    ULONG OpType
    )
{
    PSTR pszString = NULL;
    
    switch (OpType)
    {
        case PENDING_OP_JOIN_REQ:
            pszString = "Join request";
            break;
        case PENDING_OP_CONN_START:
            pszString = "Connection start";
            break;
        case PENDING_OP_EX_ACCESS_REQ:
            pszString = "Exclusive access request";
            break;
        case PENDING_OP_RESET_REQ:
            pszString = "Reset request";
            break;
        case PENDING_OP_CH_SW_REQ:
            pszString = "Channel switch request";
            break;
        case PENDING_OP_START_BSS_REQ:
            pszString = "Start BSS request";
            break;
        case PENDING_OP_STOP_BSS_REQ:
            pszString = "Stop BSS request";
            break;
        case PENDING_OP_DEF_KEY:
            pszString = "Set default key request";
            break;
        case PENDING_OP_OPERATING_PHY_ID:
            pszString = "Set operating Phy ID request";
            break;
        case PENDING_OP_DESIRED_PHY_ID_LIST:
            pszString = "Set desired phyID list request";
            break;
        case PENDING_OP_NIC_POWER_STATE:
            pszString = "Set NIC power state";
            break;        
    }

    return pszString;
}

__inline
PSTR
GetHvlPendingOpString(
    ULONG OpType
    )
{
    PSTR pszString = NULL;
    
    switch (OpType)
    {
        case HVL_PENDING_OP_EX_ACCESS:
            pszString = "Exclusive access request";
            break;
    }

    return pszString;
}




/*
  A built-in help for the extension dll
*/
DECLARE_API( help )
{
    dprintf("Help for " EXTENSION_NAME ".dll\n"
            "  help             - Shows this help\n"
            "  mp [address]     - Miniport information\n"
            "  hvl [address]    - HVL specific state\n"
            "  port <address>   - Port specific state\n"
            "  vnic <address>   - VNic specific state\n"
            "  hw [address]     - HW specific state\n"
            "  mac <address>    - Hw Mac specific state\n"
            );
}

ULONG64 GetPointerFromAddress(
    ULONG64 Location)
{
    ULONG64 Value;

    if (!ReadPointer(Location,&Value))
    {
        dprintf("unable to read from %p\n",Location);
        return 0;
    }

    return Value;
}

ULONG64
get_adapter()
{
    ULONG64 adapterAddr = 0;

    // This reads the global adapter pointer maintained in the miniport
    // for the debugging purpose
    adapterAddr = GetExpression("poi(" DRIVER_NAME "!g_pAdapter)");
    if (!adapterAddr)
    {
        dprintf("ERROR: " EXTENSION_NAME " failed to retrive the value for " DRIVER_NAME "!g_pAdapter\n");
        dprintf("please make sure that the symbols are correct\n");
    }

    return adapterAddr;
}

void
dump_hvlctx(ULONG64 CtxAddr)
{
    ULONG OffsetLink = 0, OffsetVNicList = 0;
    ULONG64 VNicListHead = 0, Flink = 0;
    ULONG iters = 0;

    GetFieldOffset(DRIVER_NAME "!_HVL_CONTEXT", "VNicList", &OffsetVNicList);
    GetFieldOffset(DRIVER_NAME "!_VNIC", "CtxLink", &OffsetLink);
    
    VNicListHead = CtxAddr + OffsetVNicList;
    
    GetFieldValue(VNicListHead, DRIVER_NAME "!_LIST_ENTRY", "Flink", Flink);

    while (VNicListHead != Flink)
    {
        if (iters++ > 10)
        {
            break;
        }
        dprintf("\t\tVNIC %p\n", Flink - OffsetLink);

        if (GetFieldValue(Flink, DRIVER_NAME "!_LIST_ENTRY", "Flink", Flink))
        {
            break;
        }
    }
    
}

void
dump_inactiveCtx(ULONG64 hvlAddr)
{
    ULONG64 InactiveCtxHead = 0, Flink= 0, ctxAddr = 0;
    ULONG InactiveCtxOffset = 0, OffsetLink = 0, iters = 0; 
    
    GetFieldOffset(DRIVER_NAME "!_HVL_CONTEXT", "Link", &OffsetLink);
    
    GetFieldOffset(DRIVER_NAME "!_HVL", "InactiveContextList", &InactiveCtxOffset);
    
    InactiveCtxHead = hvlAddr + InactiveCtxOffset;
    
    GetFieldValue(InactiveCtxHead, DRIVER_NAME "!_LIST_ENTRY", "Flink", Flink);

    if (InactiveCtxHead == Flink)
    {
        dprintf("\nThere are no inactive contexts");
    }
    else
    {
        dprintf("\nInactive contexts");
    }
            
    while (InactiveCtxHead != Flink)
    {
        if (iters++ > 10)
        {
            break;
        }

        ctxAddr = Flink - OffsetLink;
        
        dprintf("\n\tContext at %p \n", ctxAddr);

        dump_hvlctx(ctxAddr);
        
        if (GetFieldValue(Flink, DRIVER_NAME "!_LIST_ENTRY", "Flink", Flink))
        {
            break;
        }
    }
}

DECLARE_API( mp )
{
    ULONG64 adapterAddr = 0;
    ULONG ulNumPorts = 0, StatusFlags = 0, OffsetPortList = 0;
    ULONG64 PortAddr = 0, PortStartAddr = 0, PortType = 0, HelperPortAddr = 0, HvlAddr = 0, HwAddr = 0;
    ULONG64 VNicAddr = 0;
    ULONG bytesRead = 0;

    adapterAddr = GetExpression(args);

    if (!adapterAddr)
    {
        // Argument not specified look for global adapter pointer
        adapterAddr = get_adapter();
        if (!adapterAddr)
        {
            return;
        }
        dprintf("Adapter at %p \n", adapterAddr);
    }
    
    GetFieldValue(adapterAddr, DRIVER_NAME "!_ADAPTER", "Status", StatusFlags);
    if (MP_ADAPTER_PAUSED & StatusFlags)
    {
        dprintf("\t\tAdapter is PAUSED. \n");    
    }
    
    if (MP_ADAPTER_PAUSING & StatusFlags)
    {
        dprintf("\t\tAdapter is PAUSING. \n");    
    }
    
    if (MP_ADAPTER_HALTING & StatusFlags)
    {
        dprintf("\t\tAdapter is HALTING. \n");    
    }
    
    if (MP_ADAPTER_IN_RESET & StatusFlags)
    {
        dprintf("\t\tAdapter is in RESET. \n");    
    }

    if (MP_ADAPTER_SURPRISE_REMOVED & StatusFlags)
    {
        dprintf("\t\tAdapter has been SURPRISE REMOVED. \n");    
    }
    
    GetFieldValue(adapterAddr, DRIVER_NAME "!_ADAPTER", "Hvl", HvlAddr);
    dprintf("\tHvl at %p \n", HvlAddr);
    
    GetFieldValue(adapterAddr, DRIVER_NAME "!_ADAPTER", "Hw", HwAddr);
    dprintf("\tHardware at %p \n", HwAddr);
    
    GetFieldValue(adapterAddr, DRIVER_NAME "!_ADAPTER", "HelperPort", HelperPortAddr);
    
    GetFieldValue(HelperPortAddr, DRIVER_NAME "!_MP_PORT", "VNic", VNicAddr);

    dprintf("\t\tHelper port at %p ", HelperPortAddr);
    dprintf("VNIC at %p \n", VNicAddr);

    GetFieldValue(adapterAddr, DRIVER_NAME "!_ADAPTER", "NumberOfPorts", ulNumPorts);

    GetFieldOffset(DRIVER_NAME "!_ADAPTER", "PortList", &OffsetPortList);
    PortStartAddr = adapterAddr + OffsetPortList;
    
    for (unsigned int i = 0; i < ulNumPorts; i++)
    {
        ReadMemory(PortStartAddr + i * sizeof(ULONG_PTR), &PortAddr, sizeof(ULONG_PTR), &bytesRead);
        
        GetFieldValue(PortAddr, DRIVER_NAME "!_MP_PORT", "PortType", PortType);
        GetFieldValue(PortAddr, DRIVER_NAME "!_MP_PORT", "VNic", VNicAddr);
        
        dprintf("\t\t%s Port at %p ", GetPortTypeString((ULONG)PortType), PortAddr);
        dprintf("VNIC at %p \n", VNicAddr);
    }
}

DECLARE_API( port )
{
    ULONG64 PortAddr = 0, PortNumber = 0, PortType = 0, ChildPortAddr = 0;
    ULONG AssocState = 0, ConnectState = 0;
    

    PortAddr = GetExpression(args);
    if (!PortAddr)
    {
        return;
    }    
    GetFieldValue(PortAddr, DRIVER_NAME "!_MP_PORT", "PortType", PortType);
    GetFieldValue(PortAddr, DRIVER_NAME "!_MP_PORT", "PortNumber", PortNumber);

    dprintf("%s Port \t Port number %d\n", GetPortTypeString((ULONG)PortType), PortNumber);

    switch (PortType)
    {
        case EXTSTA_PORT:
            GetFieldValue(PortAddr, DRIVER_NAME "!_MP_PORT", "ChildPort", ChildPortAddr);
            dprintf("\t Station port %p\n", ChildPortAddr);
            
            GetFieldValue(ChildPortAddr, DRIVER_NAME "!_MP_EXTSTA_PORT", "ConnectContext.AssociateState", AssocState);
            dprintf("\t\t%s\n", GetAssocStateString(AssocState));

            GetFieldValue(ChildPortAddr, DRIVER_NAME "!_MP_EXTSTA_PORT", "ConnectContext.ConnectState", ConnectState);
            dprintf("\t\t%s\n", GetConnStateString(ConnectState));
            
            break;

        case EXTAP_PORT:
            GetFieldValue(PortAddr, DRIVER_NAME "!_MP_PORT", "ChildPort", ChildPortAddr);
            dprintf("\t Ap port %p\n", ChildPortAddr);

            break;
    }
}

DECLARE_API( vnic )
{
    ULONG64 VNicAddr = 0, HvlCtx = 0, CtxSRef = 0, PortAddr = 0, Flink = 0, PendingOpHead = 0, HwCtx = 0;
    BOOLEAN fActive = FALSE, fResetInProgress = FALSE;
    ULONG iters = 0, Type =0;
    ULONG OffsetLink = 0, PendingOpOffset = 0;

    VNicAddr = GetExpression(args);
    if (!VNicAddr)
    {
        return;
    }    

    GetFieldValue(VNicAddr, DRIVER_NAME "!_VNIC", "fResetInProgress", fResetInProgress);
    if (fResetInProgress)
    {
        dprintf("VNIC is being Reset. \n");
    }
    
    GetFieldValue(VNicAddr, DRIVER_NAME "!_VNIC", "fActive", fActive);

    dprintf("VNIC is %s. \n", fActive?"Active":"Inactive");

    GetFieldValue(VNicAddr, DRIVER_NAME "!_VNIC", "pHvlCtx", HvlCtx);

    dprintf("HVL Context is %p \n", HvlCtx);
    
    GetFieldValue(VNicAddr, DRIVER_NAME "!_VNIC", "pvHwContext", HwCtx);

    dprintf("Hardware MAC context is %p \n", HwCtx);
    
    GetFieldValue(VNicAddr, DRIVER_NAME "!_VNIC", "pvPort", PortAddr);

    dprintf("Corresponding port is %p \n", PortAddr);
    
    GetFieldValue(VNicAddr, DRIVER_NAME "!_VNIC", "CtxtSRefCount", CtxSRef);

    dprintf("Context switch ref count is %d \n", CtxSRef);

    GetFieldOffset(DRIVER_NAME "!_PENDING_OP", "Link", &OffsetLink);
    
    GetFieldOffset(DRIVER_NAME "!_VNIC", "PendingOpQueue", &PendingOpOffset);
    PendingOpHead = VNicAddr + PendingOpOffset;
    
    GetFieldValue(PendingOpHead, DRIVER_NAME "!_LIST_ENTRY", "Flink", Flink);

    if (PendingOpHead == Flink)
    {
        dprintf("There are no pending operations \n");
    }
    else
    {
        dprintf("Pending operations \n");
    }
    
        
    while (PendingOpHead != Flink)
    {
        if (iters++ > 10)
        {
            break;
        }

        GetFieldValue(Flink - OffsetLink, DRIVER_NAME "!_PENDING_OP", "Type", Type);
        
        dprintf("\t%s\n", GetPendingOpString(Type));

        if (GetFieldValue(Flink, DRIVER_NAME "!_LIST_ENTRY", "Flink", Flink))
        {
            break;
        }
    }
}

DECLARE_API( mac )
{
    ULONG64 MacAddr = 0;
    ULONG RefCount = 0, PhyId = 0, StatusFlags = 0;

    MacAddr = GetExpression(args);
    if (!MacAddr)
    {
        return;
    }    
    
    dprintf("Hardware MAC context at %p ", MacAddr);
    GetFieldValue(MacAddr, DRIVER_NAME "!_HW_MAC_CONTEXT", "Status", StatusFlags);
    if (HW_MAC_CONTEXT_VALID & StatusFlags)
    {
        dprintf("is Valid, ");

        if (HW_MAC_CONTEXT_ACTIVE & StatusFlags)
        {
            dprintf("Active and ");
        }
        else if (HW_MAC_CONTEXT_ACTIVATING & StatusFlags)
        {
            dprintf("Being Activated and ");
        }   
        else
        {
            dprintf("Inactive and ");
        }   
    
        if (HW_MAC_CONTEXT_LINK_UP & StatusFlags)
        {
            dprintf("Connected\n");
        }
        else
        {
            dprintf("Not Connected\n");
        }   

        if (HW_MAC_CONTEXT_IN_DOT11_RESET & StatusFlags)
        {
            dprintf("\tHardware MAC context is being reset\n");
        }

        GetFieldValue(MacAddr, DRIVER_NAME "!_HW_MAC_CONTEXT", "SendRefCount", RefCount);
        dprintf("\tSend ref count is %d \n", RefCount);

        GetFieldValue(MacAddr, DRIVER_NAME "!_HW_MAC_CONTEXT", "RecvRefCount", RefCount);
        dprintf("\tReceive ref count is %d \n", RefCount);

        GetFieldValue(MacAddr, DRIVER_NAME "!_HW_MAC_CONTEXT", "GenericRefCount", RefCount);
        dprintf("\tGeneric ref count is %d \n", RefCount);

        GetFieldValue(MacAddr, DRIVER_NAME "!_HW_MAC_CONTEXT", "OperatingPhyId", PhyId);
        dprintf("\tOperation Phy ID %d \n", RefCount);

    }
    else
    {
        dprintf("is InValid\n");
    }
}



DECLARE_API( hvl )
{
    ULONG64 HvlAddr = 0, ActiveCtx = 0, VNic = 0, adapterAddr = 0, PendingOpHead = 0, Flink = 0, ExAccessReq = 0, ExAccessReqVNic = 0;
    BOOLEAN fVirtualizationEnabled = FALSE;
    ULONG StatusFlag = 0;
    ULONG iters = 0, Type =0;
    ULONG OffsetLink = 0, PendingOpOffset = 0, RefCount = 0;
    
    HvlAddr = GetExpression(args);

    if (!HvlAddr)
    {
        adapterAddr = get_adapter();
        if (!adapterAddr)
        {
            return;
        }
        
        GetFieldValue(adapterAddr, DRIVER_NAME "!_ADAPTER", "Hvl", HvlAddr);
        dprintf("\n\tHvl at %p \n", HvlAddr);
    }    
    
    GetFieldValue(HvlAddr, DRIVER_NAME "!_HVL", "fVirtualizationEnabled", fVirtualizationEnabled);

    dprintf("Virtualization is %s. \n", fVirtualizationEnabled?"enabled":"disabled");

    GetFieldValue(HvlAddr, DRIVER_NAME "!_HVL", "ulStatusFlags", StatusFlag);

    if (StatusFlag & HVL_TIMED_CTXS_BLOCKED)
    {
        dprintf("Timed context switches are blocked \n");
    }

    if (StatusFlag & HVL_CTXS_IN_PROGRESS)
    {
        dprintf("Context switches is currently in progress\n");
    }

    GetFieldValue(HvlAddr, DRIVER_NAME "!_HVL", "pExAccessVNic", VNic);
    if(VNic)
    {
        dprintf("Exclusive access is obtain by VNIC %p \n", VNic);
    }

    GetFieldValue(HvlAddr, DRIVER_NAME "!_HVL", "pExAccessDelegatedVNic", VNic);
    if(VNic)
    {
        dprintf("Exclusive access has been delegated to VNIC %p \n", VNic);
    }

    GetFieldValue(HvlAddr, DRIVER_NAME "!_HVL", "pActiveContext", ActiveCtx);
    dprintf("\nActive context is %p \n", ActiveCtx);

    dump_hvlctx(ActiveCtx);

    dump_inactiveCtx(HvlAddr);

    GetFieldOffset(DRIVER_NAME "!_HVL_PENDING_OP", "Link", &OffsetLink);
    
    GetFieldOffset(DRIVER_NAME "!_HVL", "PendingOpQueue", &PendingOpOffset);
    PendingOpHead = HvlAddr + PendingOpOffset;
    
    GetFieldValue(PendingOpHead, DRIVER_NAME "!_LIST_ENTRY", "Flink", Flink);

    if (PendingOpHead == Flink)
    {
        dprintf("\nThere are no pending operations \n");
    }
    else
    {
        dprintf("\nPending operations \n");
    }    
        
    while (PendingOpHead != Flink)
    {
        if (iters++ > 10)
        {
            break;
        }

        GetFieldValue(Flink - OffsetLink, DRIVER_NAME "!_HVL_PENDING_OP", "Type", Type);
        
        dprintf("\t%s\n", GetHvlPendingOpString(Type));

        GetFieldValue(Flink - OffsetLink, DRIVER_NAME "!_HVL_PENDING_OP", "pvOpData", ExAccessReq);
        
        GetFieldValue(ExAccessReq, DRIVER_NAME "!_HVL_EX_ACCESS_REQ", "pVNic", ExAccessReqVNic);
        GetFieldValue(ExAccessReq, DRIVER_NAME "!_HVL_EX_ACCESS_REQ", "ulRefCount", RefCount);
        
        dprintf("\t\tVNic = %p Ref = %d\n", ExAccessReqVNic, RefCount);

        if (GetFieldValue(Flink, DRIVER_NAME "!_LIST_ENTRY", "Flink", Flink))
        {
            break;
        }
    }
}

DECLARE_API( hw )
{
    ULONG64 HwAddr = 0, adapterAddr = 0, MacStartAddr = 0, MacAddr = 0, HalAddr = 0;
    ULONG StatusFlags = 0, OffsetMacCtx = 0, sizeofMacCtx = 0;
    
    HwAddr = GetExpression(args);

    if (!HwAddr)
    {
        adapterAddr = get_adapter();
        if (!adapterAddr)
        {
            return;
        }
        
        GetFieldValue(adapterAddr, DRIVER_NAME "!_ADAPTER", "Hw", HwAddr);
        dprintf("Hardware at %p \n", HwAddr);
    }    

    // Find the address of the HAL
    GetFieldValue(HwAddr, DRIVER_NAME "!_HW", "Hal", HalAddr);
    dprintf("HAL at %p\n", HalAddr);    
    
    GetFieldValue(HwAddr, DRIVER_NAME "!_HW", "Status", StatusFlags);
    if (HW_ADAPTER_PAUSED & StatusFlags)
    {
        dprintf("Hardware is PAUSED. \n");    
    }
    
    if (HW_ADAPTER_PAUSING & StatusFlags)
    {
        dprintf("Hardware is PAUSING. \n");    
    }
    
    if (HW_ADAPTER_HALTING & StatusFlags)
    {
        dprintf("Hardware is HALTING. \n");    
    }
    
    if (HW_ADAPTER_IN_RESET & StatusFlags)
    {
        dprintf("Hardware is in RESET. \n");    
    }

    if (HW_ADAPTER_SURPRISE_REMOVED & StatusFlags)
    {
        dprintf("Hardware has been SURPRISE REMOVED. \n");    
    }

    GetFieldOffset(DRIVER_NAME "!_HW", "MacContext", &OffsetMacCtx);
    MacStartAddr = HwAddr + OffsetMacCtx;

    sizeofMacCtx = GetTypeSize (DRIVER_NAME "!_HW_MAC_CONTEXT");
    
    for (unsigned int i = 0; i < HW_MAX_NUMBER_OF_MAC; i++)
    {
        MacAddr = MacStartAddr + i * sizeofMacCtx;

        GetFieldValue(MacAddr, DRIVER_NAME "!_HW_MAC_CONTEXT", "Status", StatusFlags);

        dprintf("\tHardware MAC context at %p ", MacAddr);

        if (HW_MAC_CONTEXT_VALID & StatusFlags)
        {
            dprintf("is Valid, ");
            if (HW_MAC_CONTEXT_ACTIVE & StatusFlags)
            {
                dprintf("Active and ");
            }
            else
            {
                dprintf("Inactive and ");
            }   

            if (HW_MAC_CONTEXT_LINK_UP & StatusFlags)
            {
                dprintf("Connected\n");
            }
            else
            {
                dprintf("Not Connected\n");
            }   
            
        }
        else
        {
            dprintf("is InValid\n");
        }
        
    }
}
