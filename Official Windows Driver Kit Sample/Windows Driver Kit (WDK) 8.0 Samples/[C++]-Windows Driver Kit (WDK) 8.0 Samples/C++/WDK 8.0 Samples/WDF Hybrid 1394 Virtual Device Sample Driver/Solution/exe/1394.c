/*++

Copyright (c) 1998  Microsoft Corporation

Module Name:

    1394.c

Abstract

    1394 api wrappers

--*/

#define _1394_C
#include "pch.h"
#undef _1394_C

INT_PTR CALLBACK
BusResetDlgProc (
                 HWND             hDlg,
                 UINT                uMsg,
                 WPARAM        wParam,
                 LPARAM          lParam)
{
    static PULONG   pFlags;
    static CHAR     tmpBuff[STRING_SIZE];

    switch (uMsg) 
    {

        case WM_INITDIALOG:

            pFlags = (PULONG)lParam;

            if (*pFlags == BUS_RESET_FLAGS_FORCE_ROOT)
                CheckDlgButton( hDlg, IDC_BUS_RESET_FORCE_ROOT, BST_CHECKED);

            return(TRUE); // WM_INITDIALOG

        case WM_COMMAND:

            switch (LOWORD(wParam)) {

                case IDOK:

                    *pFlags = 0;
                    if (IsDlgButtonChecked(hDlg, IDC_BUS_RESET_FORCE_ROOT))
                        *pFlags = BUS_RESET_FLAGS_FORCE_ROOT;

                    EndDialog(hDlg, TRUE);
                    return(TRUE); // IDOK

                case IDCANCEL:
                    EndDialog(hDlg, FALSE);
                    return(TRUE); // IDCANCEL

                default:
                    return(TRUE); // default

            } // switch

            break; // WM_COMMAND

        default:
            break; // default

    } // switch

    return(FALSE);
} // BusResetDlgProc

void
w1394_BusReset(
               HWND         hWnd,
               _In_ PSTR    szDeviceName)
{
    ULONG   fulFlags;
    DWORD   dwRet, bytesReturned;

    UNREFERENCED_PARAMETER(szDeviceName);

    TRACE(TL_TRACE, (hWnd, "Enter w1394_BusReset\r\n"));

    fulFlags = 0;

    if (DialogBoxParam ( 
        (HINSTANCE) GetWindowLongPtr(hWnd, GWLP_HINSTANCE),
        "BusReset",
        hWnd,
        BusResetDlgProc,
        (LPARAM)&fulFlags)) 
    {
        dwRet = SendRequest (
            IOCTL_BUS_RESET,
            (LPVOID)&fulFlags,
            sizeof (ULONG), 
            NULL, 
            0, 
            &bytesReturned);

        if (ERROR_SUCCESS != dwRet)
        {
            TRACE(TL_WARNING, (hWnd, "SendRequest failed 0x%x\r\n", dwRet));
        }

    }


    TRACE(TL_TRACE, (hWnd, "Exit w1394_BusReset\r\n\r\n"));
    return;
} // w1394_BusReset

void
w1394_GetGenerationCount(
                         HWND         hWnd,
                         _In_ PSTR    szDeviceName)
{
    ULONG       generationCount;
    DWORD       dwRet, bytesReturned;

    UNREFERENCED_PARAMETER(szDeviceName);

    TRACE(TL_TRACE, (hWnd, "Enter w1394_GetGenerationCount\r\n"));

    dwRet = SendRequest (
        IOCTL_GET_GENERATION_COUNT,
        NULL,
        0, 
        (LPVOID) &generationCount, 
        sizeof (ULONG), 
        &bytesReturned);

    if (ERROR_SUCCESS != dwRet)
    {
        TRACE(TL_WARNING, (hWnd, "Failed SendRequest 0x%x\r\n", dwRet));
    }
    else
    {
        TRACE(TL_TRACE, (hWnd, "GenerationCount = 0x%x\r\n", generationCount));
    }

    TRACE(TL_TRACE, (hWnd, "Exit w1394_GetGenerationCount\r\n\r\n"));
    return;
} // w1394_GetGenerationCount

INT_PTR CALLBACK
GetLocalHostInfoDlgProc(
    HWND        hDlg,
    UINT        uMsg,
    WPARAM      wParam,
    LPARAM      lParam
    )
{
    static ULONG GetLocalHostInfoLevel;
    static CHAR tmpBuff[STRING_SIZE];

    switch (uMsg) 
    {

    case WM_INITDIALOG:
        {

            GetLocalHostInfoLevel = (ULONG)lParam;

            CheckRadioButton ( 
                hDlg,
                IDC_GET_HOST_UNIQUE_ID,
                IDC_GET_TOPOLOGY_MAP,
                GetLocalHostInfoLevel + (IDC_GET_HOST_UNIQUE_ID-1));

            return(TRUE); // WM_INITDIALOG
        }

    case WM_COMMAND:
        {
            switch (LOWORD(wParam)) 
            {
            case IDOK:
                {
                    if (IsDlgButtonChecked(hDlg, IDC_GET_HOST_UNIQUE_ID))
                        GetLocalHostInfoLevel = GET_HOST_UNIQUE_ID;

                    if (IsDlgButtonChecked(hDlg, IDC_GET_HOST_HOST_CAPABILITIES))
                        GetLocalHostInfoLevel = GET_HOST_CAPABILITIES;

                    if (IsDlgButtonChecked(hDlg, IDC_GET_HOST_POWER_SUPPLIED))
                        GetLocalHostInfoLevel = GET_POWER_SUPPLIED;

                    if (IsDlgButtonChecked(hDlg, IDC_GET_HOST_PHYS_ADDR_ROUTINE))
                        GetLocalHostInfoLevel = GET_PHYS_ADDR_ROUTINE;

                    if (IsDlgButtonChecked(hDlg, IDC_GET_HOST_CONFIG_ROM))
                        GetLocalHostInfoLevel = GET_HOST_CONFIG_ROM;

                    //
                    // We're not supporting this level at this time
                    // TODO: Add ability to retrieve values of Buttons
                    // for request configuration.
                    //
                    if (IsDlgButtonChecked(hDlg, IDC_GET_SPEED_MAP)) 
                    {
                        GetLocalHostInfoLevel = GET_HOST_CSR_CONTENTS;;
                    }

                    if (IsDlgButtonChecked(hDlg, IDC_GET_TOPOLOGY_MAP)) 
                    {
                        GetLocalHostInfoLevel = GET_HOST_CSR_CONTENTS;
                    }


                    EndDialog(hDlg, TRUE);
                    return(TRUE); // IDOK
                }

            case IDCANCEL:
                {
                    EndDialog(hDlg, FALSE);
                    return(TRUE); // IDCANCEL
                }

            default:
                return(TRUE); // default

            } // switch (LOWORD (wParam))
        }// case WM_COMMAND

    default:
        break; // default
    }// switch (uMsg)

    return(FALSE);
} // GetLocalHostInfoDlgProc

void
w1394_GetLocalHostInfo (
                        HWND         hWnd,
                        _In_ PSTR    szDeviceName)
{
    PGET_LOCAL_HOST_INFORMATION GetLocalHostInfo;
    DWORD dwRet, bytesReturned;
    ULONG ulBufferSize = sizeof(GET_LOCAL_HOST_INFORMATION) ;
    ULONG GetLocalHostLevel = GET_HOST_UNIQUE_ID; // Default Value

    UNREFERENCED_PARAMETER(szDeviceName);

    TRACE(TL_TRACE, (hWnd, "Enter w1394_GetLocalHostInformation\r\n"));
   
    if (DialogBoxParam (
        (HINSTANCE) GetWindowLongPtr (hWnd, GWLP_HINSTANCE),
        "GetLocalHostInfo",
        hWnd,
        GetLocalHostInfoDlgProc,
        (LPARAM)&GetLocalHostLevel))
    {
        switch (GetLocalHostLevel)
        {

        case GET_HOST_UNIQUE_ID:
            {
                ulBufferSize += sizeof(GET_LOCAL_HOST_INFO1);
                break;
            }
        case GET_HOST_CAPABILITIES:
            {
                ulBufferSize += sizeof(GET_LOCAL_HOST_INFO2);
                break;
            }

        case GET_POWER_SUPPLIED:
            {
                ulBufferSize += sizeof(GET_LOCAL_HOST_INFO3);
                break;
            }

        case GET_PHYS_ADDR_ROUTINE:
            {
                ulBufferSize += sizeof(GET_LOCAL_HOST_INFO4);
                break;
            }

            //
            // We'll add UI and application support for these later
            //
        case GET_HOST_DMA_CAPABILITIES:
        case GET_HOST_CSR_CONTENTS:
        case GET_HOST_CONFIG_ROM:
            {
                TRACE(
                    TL_WARNING, 
                    (hWnd, 
                    "Levels 5, 6 & 7 are not supported at this time\r\n\r\n"));
                return;
            }

        default: 
            break;
        } // switch
    }// if DialogBoxParam
    else
    {
        TRACE(
            TL_ERROR,
           (hWnd,
           "Failed DialogBoxParam call: 0x%x\r\n\r\n", 
           GetLastError()));
        return;
    }
 
    GetLocalHostInfo = \
        (PGET_LOCAL_HOST_INFORMATION) LocalAlloc (
        LPTR,
        ulBufferSize);
    if (NULL == GetLocalHostInfo)
    {
        TRACE(
            TL_WARNING, 
            (hWnd, 
            "Failed resource allocation: 0x%x\r\n\r\n", 
            GetLastError()));
        return;
    }

    dwRet = SendRequest ( 
        IOCTL_GET_LOCAL_HOST_INFORMATION,
        GetLocalHostInfo,
        ulBufferSize,
        GetLocalHostInfo,
        ulBufferSize,
        &bytesReturned);
    if (ERROR_SUCCESS != dwRet)
    {
        TRACE(TL_WARNING, (hWnd, "Failed SendRequest 0x%x\r\n", dwRet));
    }
    else
    {
        DisplayLocalHost (hWnd, GetLocalHostInfo);
    }

    TRACE(TL_TRACE, (hWnd, "Exit w1394_GetLocalHostInformation\r\n\r\n"));
    LocalFree (GetLocalHostInfo);
    return;

}// w1394_GetLocalHostInfo

INT_PTR CALLBACK
Get1394AddressFromDeviceObjectDlgProc(
                                      HWND        hDlg,
                                      UINT        uMsg,
                                      WPARAM      wParam,
                                      LPARAM      lParam)
{
    static PGET_1394_ADDRESS    pGet1394Address;
    static CHAR                 tmpBuff[STRING_SIZE];

    switch (uMsg) {

        case WM_INITDIALOG:

            pGet1394Address = (PGET_1394_ADDRESS)lParam;

            if (pGet1394Address->fulFlags & USE_LOCAL_NODE)
                CheckDlgButton( hDlg, IDC_GET_ADDR_USE_LOCAL_NODE, BST_CHECKED);

            return(TRUE); // WM_INITDIALOG

        case WM_COMMAND:

            switch (LOWORD(wParam)) {

                case IDOK:

                    pGet1394Address->fulFlags = 0;
                    if (IsDlgButtonChecked(hDlg, IDC_GET_ADDR_USE_LOCAL_NODE))
                        pGet1394Address->fulFlags |= USE_LOCAL_NODE;

                    EndDialog(hDlg, TRUE);
                    return(TRUE); // IDOK

                case IDCANCEL:
                    EndDialog(hDlg, FALSE);
                    return(TRUE); // IDCANCEL

                default:
                    return(TRUE); // default

            } // switch

            break; // WM_COMMAND

        default:
            break; // default

    } // switch

    return(FALSE);
} // Get1394AddressFromDeviceObjectDlgProc

void
w1394_Get1394AddressFromDeviceObject(
                                     HWND         hWnd,
                                     _In_ PSTR    szDeviceName
                                     )
{
    GET_1394_ADDRESS    get1394Address;
    DWORD               dwRet, bytesReturned;

    UNREFERENCED_PARAMETER(szDeviceName);

    TRACE(TL_TRACE, (hWnd, "Enter w1394_Get1394AddressFromDeviceObject\r\n"));

    get1394Address.fulFlags = 0;

    if (DialogBoxParam ( 
        (HINSTANCE) GetWindowLongPtr(hWnd, GWLP_HINSTANCE),
        "Get1394AddrFromDeviceObject",
        hWnd,
        Get1394AddressFromDeviceObjectDlgProc,
        (LPARAM)&get1394Address))
    {

        dwRet = SendRequest (
            IOCTL_GET_1394_ADDRESS_FROM_DEVICE_OBJECT,
            &get1394Address,
            sizeof (GET_1394_ADDRESS),
            &get1394Address,
            sizeof (GET_1394_ADDRESS),
            &bytesReturned);
        if (ERROR_SUCCESS != dwRet)
        {
            TRACE(TL_WARNING, (hWnd, "Failed SendRequest 0x%x\r\n", dwRet));
        }
        else
        {

            TRACE(TL_TRACE, (hWnd, "NA_Bus_Number = 0x%x\r\n", 
                get1394Address.NodeAddress.NA_Bus_Number));

            TRACE(TL_TRACE, (hWnd, "NA_Node_Number = 0x%x\r\n", 
                get1394Address.NodeAddress.NA_Node_Number));
        }
    }

    TRACE(TL_TRACE, (hWnd, "Exit w1394_Get1394AddressFromDeviceObject\r\n\r\n"));
    return;
} // w1394_Get1394AddressFromDeviceObject

void
w1394_Control(
              HWND         hWnd,
              _In_ PSTR    szDeviceName)
{
    DWORD       dwRet, bytesReturned;

    UNREFERENCED_PARAMETER(szDeviceName);

    TRACE(TL_TRACE, (hWnd, "Enter w1394_Control\r\n"));

    dwRet = SendRequest (
        IOCTL_CONTROL,
        NULL,
        0,
        NULL,
        0,
        &bytesReturned);
    if (ERROR_SUCCESS != dwRet)
    {
        TRACE(TL_WARNING, (hWnd, "Failed SendRequest 0x%x\r\n", dwRet));
    }

    TRACE(TL_TRACE, (hWnd, "Exit w1394_Control\r\n\r\n"));
    return;
} // w1394_Control

INT_PTR CALLBACK
GetMaxSpeedBetweenDevicesDlgProc(
                                 HWND        hDlg,
                                 UINT        uMsg,
                                 WPARAM      wParam,
                                 LPARAM      lParam)
{
    static PGET_MAX_SPEED_BETWEEN_DEVICES   pGetMaxSpeed;
    static CHAR                             tmpBuff[STRING_SIZE];

    switch (uMsg) {

        case WM_INITDIALOG:

            pGetMaxSpeed = (PGET_MAX_SPEED_BETWEEN_DEVICES)lParam;

            if (pGetMaxSpeed->fulFlags & USE_LOCAL_NODE)
                CheckDlgButton( hDlg, IDC_GET_MAX_USE_LOCAL_NODE, BST_CHECKED);

            return(TRUE); // WM_INITDIALOG

        case WM_COMMAND:

            switch (LOWORD(wParam)) {

                case IDOK:

                    pGetMaxSpeed->fulFlags = 0;
                    if (IsDlgButtonChecked(hDlg, IDC_GET_MAX_USE_LOCAL_NODE))
                        pGetMaxSpeed->fulFlags |= USE_LOCAL_NODE;

                    EndDialog(hDlg, TRUE);
                    return(TRUE); // IDOK

                case IDCANCEL:
                    EndDialog(hDlg, FALSE);
                    return(TRUE); // IDCANCEL

                default:
                    return(TRUE); // default

            } // switch

            break; // WM_COMMAND

        default:
            break; // default

    } // switch

    return(FALSE);
} // GetMaxSpeedBetweenDevicesDlgProc

void
w1394_GetMaxSpeedBetweenDevices(
                                HWND         hWnd,
                                _In_ PSTR    szDeviceName)
{
    GET_MAX_SPEED_BETWEEN_DEVICES   getMaxSpeed;
    DWORD                           dwRet, bytesTransferred;

    UNREFERENCED_PARAMETER(szDeviceName);

    TRACE(TL_TRACE, (hWnd, "Enter w1394_GetMaxSpeedBetweenDevices\r\n"));

    getMaxSpeed.fulFlags = 0;
    getMaxSpeed.ulNumberOfDestinations = 0;

    if (DialogBoxParam ( 
        (HINSTANCE) GetWindowLongPtr(hWnd, GWLP_HINSTANCE),
        "GetMaxSpeed",
        hWnd,
        GetMaxSpeedBetweenDevicesDlgProc,
        (LPARAM)&getMaxSpeed)) 
    {

        dwRet = SendRequest (
            IOCTL_GET_MAX_SPEED_BETWEEN_DEVICES,
            &getMaxSpeed,
            sizeof (GET_MAX_SPEED_BETWEEN_DEVICES),
            &getMaxSpeed,
            sizeof (GET_MAX_SPEED_BETWEEN_DEVICES),
            &bytesTransferred);
        if (ERROR_SUCCESS != dwRet)
        {
            TRACE(TL_WARNING, (hWnd, "Failed SendRequest 0x%x\r\n", dwRet));
        }
        else
        {
            TRACE(TL_TRACE, (hWnd, "fulSpeed = 0x%x\r\n", getMaxSpeed.fulSpeed));
        }
    }

    TRACE(TL_TRACE, (hWnd, "Exit w1394_GetMaxSpeedBetweenDevices\r\n\r\n"));
    return;
} // w1394_GetMaxSpeedBetweenDevices

void
w1394_GetConfigurationInfo(
                           HWND         hWnd,
                           _In_ PSTR    szDeviceName)
{
    DWORD       dwRet, bytesReturned;

    UNREFERENCED_PARAMETER(szDeviceName);

    TRACE(TL_TRACE, (hWnd, "Enter w1394_GetConfigurationInfo\r\n"));

    dwRet = SendRequest (
        IOCTL_GET_CONFIGURATION_INFORMATION,
        NULL,
        0,
        NULL,
        0,
        &bytesReturned);
    if (ERROR_SUCCESS != dwRet)
    {
        TRACE(TL_WARNING, (hWnd, "Failed SendRequest 0x%x\r\n", dwRet));
    }

    TRACE(TL_TRACE, (hWnd, "Exit w1394_GetConfigurationInfo\r\n\r\n"));
    return;
} // w1394_GetConfigurationInfo

INT_PTR CALLBACK
SetDeviceXmitPropertiesDlgProc(
                               HWND        hDlg,
                               UINT        uMsg,
                               WPARAM      wParam,
                               LPARAM      lParam)
{
    static PDEVICE_XMIT_PROPERTIES      pDeviceXmitProperties;
    static CHAR                         tmpBuff[STRING_SIZE];

    switch (uMsg) {

        case WM_INITDIALOG:

            pDeviceXmitProperties = (PDEVICE_XMIT_PROPERTIES)lParam;

            if (pDeviceXmitProperties->fulSpeed == SPEED_FLAGS_FASTEST) {

                CheckRadioButton( hDlg,
                                  IDC_SET_DEVICE_XMIT_100MBPS,
                                  IDC_SET_DEVICE_XMIT_FASTEST,
                                  IDC_SET_DEVICE_XMIT_FASTEST
                                  );
            }
            else {

                CheckRadioButton( hDlg,
                                  IDC_SET_DEVICE_XMIT_100MBPS,
                                  IDC_SET_DEVICE_XMIT_FASTEST,
                                  pDeviceXmitProperties->fulSpeed + (IDC_SET_DEVICE_XMIT_100MBPS-1)
                                  );
            }

            return(TRUE); // WM_INITDIALOG

        case WM_COMMAND:

            switch (LOWORD(wParam)) {

                case IDOK:

                    pDeviceXmitProperties->fulSpeed = 0;
                    if (IsDlgButtonChecked(hDlg, IDC_SET_DEVICE_XMIT_100MBPS))
                        pDeviceXmitProperties->fulSpeed = SPEED_FLAGS_100;

                    if (IsDlgButtonChecked(hDlg, IDC_SET_DEVICE_XMIT_200MBPS))
                        pDeviceXmitProperties->fulSpeed = SPEED_FLAGS_200;

                    if (IsDlgButtonChecked(hDlg, IDC_SET_DEVICE_XMIT_400MBPS))
                        pDeviceXmitProperties->fulSpeed = SPEED_FLAGS_400;

                    if (IsDlgButtonChecked(hDlg, IDC_SET_DEVICE_XMIT_1600MBPS))
                        pDeviceXmitProperties->fulSpeed = SPEED_FLAGS_1600;

                    if (IsDlgButtonChecked(hDlg, IDC_SET_DEVICE_XMIT_FASTEST))
                        pDeviceXmitProperties->fulSpeed = SPEED_FLAGS_FASTEST;

                    EndDialog(hDlg, TRUE);
                    return(TRUE); // IDOK

                case IDCANCEL:
                    EndDialog(hDlg, FALSE);
                    return(TRUE); // IDCANCEL

                default:
                    return(TRUE); // default

            } // switch

            break; // WM_COMMAND

        default:
            break; // default

    } // switch

    return(FALSE);
} // SetDeviceXmitPropertiesDlgProc

void
w1394_SetDeviceXmitProperties(
                              HWND         hWnd,
                              _In_ PSTR    szDeviceName)
{
    DEVICE_XMIT_PROPERTIES      deviceXmitProperties;
    DWORD                       dwRet, bytesReturned;

    UNREFERENCED_PARAMETER(szDeviceName);

    TRACE(TL_TRACE, (hWnd, "Enter w1394_SetDeviceXmitProperties\r\n"));

    deviceXmitProperties.fulSpeed = SPEED_FLAGS_200;
    deviceXmitProperties.fulPriority = 0;

    if (DialogBoxParam ( 
        (HINSTANCE) GetWindowLongPtr(hWnd, GWLP_HINSTANCE),
        "SetDeviceXmitProperties",
        hWnd,
        SetDeviceXmitPropertiesDlgProc,
        (LPARAM)&deviceXmitProperties))
    {
        dwRet = SendRequest(
            IOCTL_SET_DEVICE_XMIT_PROPERTIES,
            &deviceXmitProperties,
            sizeof (DEVICE_XMIT_PROPERTIES),
            NULL,
            0,
            &bytesReturned);
        if (ERROR_SUCCESS != dwRet)
        {
            TRACE(TL_WARNING, (hWnd, "Failed SendRequest 0x%x\r\n", dwRet));
        }

    }

    TRACE(TL_TRACE, (hWnd, "Exit w1394_SetDeviceXmitProperties\r\n\r\n"));
    return;
} // w1394_SetDeviceXmitProperties

INT_PTR CALLBACK
SendPhyConfigDlgProc(
                     HWND        hDlg,
                     UINT        uMsg,
                     WPARAM      wParam,
                     LPARAM      lParam)
{
    static PPHY_CONFIGURATION_PACKET    pPhyConfigPacket;
    static CHAR                         tmpBuff[STRING_SIZE];

    switch (uMsg) {

        case WM_INITDIALOG:

            pPhyConfigPacket = (PPHY_CONFIGURATION_PACKET)lParam;

            _ultoa_s(pPhyConfigPacket->PCP_Phys_ID, tmpBuff, (STRING_SIZE * sizeof(CHAR)), 16);
            SetDlgItemText(hDlg, IDC_SEND_PHY_PHYS_ID, tmpBuff);

            _ultoa_s(pPhyConfigPacket->PCP_Packet_ID, tmpBuff, (STRING_SIZE * sizeof(CHAR)), 16);
            SetDlgItemText(hDlg, IDC_SEND_PHY_PACKET_ID, tmpBuff);

            _ultoa_s(pPhyConfigPacket->PCP_Gap_Count, tmpBuff, (STRING_SIZE * sizeof(CHAR)), 16);
            SetDlgItemText(hDlg, IDC_SEND_PHY_GAP_COUNT, tmpBuff);

            _ultoa_s(pPhyConfigPacket->PCP_Set_Gap_Count, tmpBuff, (STRING_SIZE * sizeof(CHAR)), 16);
            SetDlgItemText(hDlg, IDC_SEND_PHY_SET_GAP_COUNT, tmpBuff);

            _ultoa_s(pPhyConfigPacket->PCP_Force_Root, tmpBuff, (STRING_SIZE * sizeof(CHAR)), 16);
            SetDlgItemText(hDlg, IDC_SEND_PHY_FORCE_ROOT, tmpBuff);

            _ultoa_s(pPhyConfigPacket->PCP_Reserved1, tmpBuff, (STRING_SIZE * sizeof(CHAR)), 16);
            SetDlgItemText(hDlg, IDC_SEND_PHY_RESERVED1, tmpBuff);

            _ultoa_s(pPhyConfigPacket->PCP_Reserved2, tmpBuff, (STRING_SIZE * sizeof(CHAR)), 16);
            SetDlgItemText(hDlg, IDC_SEND_PHY_RESERVED2, tmpBuff);

            _ultoa_s(pPhyConfigPacket->PCP_Inverse, tmpBuff, (STRING_SIZE * sizeof(CHAR)), 16);
            SetDlgItemText(hDlg, IDC_SEND_PHY_INVERSE, tmpBuff);

            return(TRUE); // WM_INITDIALOG

        case WM_COMMAND:

            switch (LOWORD(wParam)) {

               case IDOK:

                    GetDlgItemText(hDlg, IDC_SEND_PHY_PHYS_ID, tmpBuff, STRING_SIZE);
                    pPhyConfigPacket->PCP_Phys_ID = strtoul(tmpBuff, NULL, 16);

                    GetDlgItemText(hDlg, IDC_SEND_PHY_PACKET_ID, tmpBuff, STRING_SIZE);
                    pPhyConfigPacket->PCP_Packet_ID = strtoul(tmpBuff, NULL, 16);

                    GetDlgItemText(hDlg, IDC_SEND_PHY_GAP_COUNT, tmpBuff, STRING_SIZE);
                    pPhyConfigPacket->PCP_Gap_Count = strtoul(tmpBuff, NULL, 16);

                    GetDlgItemText(hDlg, IDC_SEND_PHY_SET_GAP_COUNT, tmpBuff, STRING_SIZE);
                    pPhyConfigPacket->PCP_Set_Gap_Count = strtoul(tmpBuff, NULL, 16);

                    GetDlgItemText(hDlg, IDC_SEND_PHY_FORCE_ROOT, tmpBuff, STRING_SIZE);
                    pPhyConfigPacket->PCP_Force_Root = strtoul(tmpBuff, NULL, 16);

                    GetDlgItemText(hDlg, IDC_SEND_PHY_RESERVED1, tmpBuff, STRING_SIZE);
                    pPhyConfigPacket->PCP_Reserved1 = strtoul(tmpBuff, NULL, 16);

                    GetDlgItemText(hDlg, IDC_SEND_PHY_RESERVED2, tmpBuff, STRING_SIZE);
                    pPhyConfigPacket->PCP_Reserved2 = strtoul(tmpBuff, NULL, 16);

                    GetDlgItemText(hDlg, IDC_SEND_PHY_INVERSE, tmpBuff, STRING_SIZE);
                    pPhyConfigPacket->PCP_Inverse = strtoul(tmpBuff, NULL, 16);

                    EndDialog(hDlg, TRUE);
                    return(TRUE); // IDOK

                case IDCANCEL:
                    EndDialog(hDlg, FALSE);
                    return(TRUE); // IDCANCEL

                default:
                    return(TRUE); // default

            } // switch

            break; // WM_COMMAND

        default:
            break; // default

    } // switch

    return(FALSE);
} // SendPhyConfigDlgProc

void
w1394_SendPhyConfigPacket(
                          HWND         hWnd,
                          _In_ PSTR    szDeviceName)
{
    PHY_CONFIGURATION_PACKET    phyConfigPacket;
    DWORD                       dwRet, bytesReturned;

    UNREFERENCED_PARAMETER(szDeviceName);

    TRACE(TL_TRACE, (hWnd, "Enter w1394_SendPhyConfigPacket\r\n"));

    phyConfigPacket.PCP_Phys_ID = 0;
    phyConfigPacket.PCP_Packet_ID = 0;
    phyConfigPacket.PCP_Gap_Count = 0;
    phyConfigPacket.PCP_Set_Gap_Count = 0;
    phyConfigPacket.PCP_Force_Root = 0;
    phyConfigPacket.PCP_Reserved1 = 0;
    phyConfigPacket.PCP_Reserved2 = 0;
    phyConfigPacket.PCP_Inverse = 0;

    if (DialogBoxParam ( 
        (HINSTANCE) GetWindowLongPtr(hWnd, GWLP_HINSTANCE),
        "SendPhyConfigPacket",
        hWnd,
        SendPhyConfigDlgProc,
        (LPARAM)&phyConfigPacket))
    {

        dwRet = SendRequest (
            IOCTL_SEND_PHY_CONFIGURATION_PACKET,
            &phyConfigPacket,
            sizeof (PHY_CONFIGURATION_PACKET),
            NULL,
            0,
            &bytesReturned);
        if (ERROR_SUCCESS != dwRet)
        {
            TRACE(TL_WARNING, (hWnd, "Failed SendRequest 0x%x\r\n", dwRet));
        }
    }

    TRACE(TL_TRACE, (hWnd, "Exit w1394_SendPhyConfigPacket\r\n\r\n"));
    return;
} // w1394_SendPhyConfigPacket

INT_PTR CALLBACK
BusResetNotificationDlgProc(
                            HWND        hDlg,
                            UINT        uMsg,
                            WPARAM      wParam,
                            LPARAM      lParam)
{
    static PULONG   pFlags;
    static CHAR     tmpBuff[STRING_SIZE];

    switch (uMsg) {

        case WM_INITDIALOG:

            pFlags = (PULONG)lParam;

            CheckRadioButton( hDlg,
                              IDC_BUS_RESET_NOTIFY_REGISTER,
                              IDC_BUS_RESET_NOTIFY_DEREGISTER,
                              *pFlags + (IDC_BUS_RESET_NOTIFY_REGISTER-1)
                              );

            return(TRUE); // WM_INITDIALOG

        case WM_COMMAND:

            switch (LOWORD(wParam)) {

                case IDOK:

                    if (IsDlgButtonChecked(hDlg, IDC_BUS_RESET_NOTIFY_REGISTER))
                        *pFlags = REGISTER_NOTIFICATION_ROUTINE;

                    if (IsDlgButtonChecked(hDlg, IDC_BUS_RESET_NOTIFY_DEREGISTER))
                        *pFlags = DEREGISTER_NOTIFICATION_ROUTINE;

                    EndDialog(hDlg, TRUE);
                    return(TRUE); // IDOK

                case IDCANCEL:
                    EndDialog(hDlg, FALSE);
                    return(TRUE); // IDCANCEL

                default:
                    return(TRUE); // default

            } // switch

            break; // WM_COMMAND

        default:
            break; // default

    } // switch

    return(FALSE);
} // BusResetNotificationDlgProc

void
w1394_BusResetNotification(
                           HWND         hWnd,
                           _In_ PSTR    szDeviceName)
{
    ULONG   fulFlags;
    DWORD   dwRet, bytesReturned;

    UNREFERENCED_PARAMETER(szDeviceName);

    TRACE(TL_TRACE, (hWnd, "Enter w1394_BusResetNotification\r\n"));

    fulFlags = REGISTER_NOTIFICATION_ROUTINE;

    if (DialogBoxParam( 
        (HINSTANCE) GetWindowLongPtr(hWnd, GWLP_HINSTANCE),
        "BusResetNotification",
        hWnd,
        BusResetNotificationDlgProc,
        (LPARAM)&fulFlags)) 
    {

        dwRet = SendRequest (
            IOCTL_BUS_RESET_NOTIFICATION,
            &fulFlags,
            sizeof (ULONG),
            NULL,
            0,
            &bytesReturned);
        if (ERROR_SUCCESS != dwRet)
        {
            TRACE(TL_WARNING, (hWnd, "Failed SendRequest 0x%x\r\n", dwRet));
        }
    }

    TRACE(TL_TRACE, (hWnd, "Exit w1394_BusResetNotification\r\n\r\n"));
    return;
} // w1394_BusResetNotification

INT_PTR CALLBACK
SetLocalHostInfoDlgProc(
                        HWND        hDlg,
                        UINT        uMsg,
                        WPARAM      wParam,
                        LPARAM      lParam)
{
    static PSET_LOCAL_HOST_INFORMATION      pSetLocalHostInfo;
    static PSET_LOCAL_HOST_PROPS2           pSetLocalHostProps2;
    static PSET_LOCAL_HOST_PROPS3           pSetLocalHostProps3;
    static CHAR                             tmpBuff[STRING_SIZE];

    switch (uMsg) {

        case WM_INITDIALOG:

            pSetLocalHostInfo = (PSET_LOCAL_HOST_INFORMATION)lParam;

            CheckRadioButton( hDlg,
                              IDC_SET_LOCAL_HOST_LEVEL_GAP_COUNT,
                              IDC_SET_LOCAL_HOST_LEVEL_CROM,
                              pSetLocalHostInfo->nLevel + (IDC_SET_LOCAL_HOST_LEVEL_GAP_COUNT-2)
                              );

            CheckRadioButton( hDlg,
                              IDC_SET_LOCAL_HOST_CROM_ADD,
                              IDC_SET_LOCAL_HOST_CROM_REMOVE,
                              IDC_SET_LOCAL_HOST_CROM_ADD
                              );

            if (pSetLocalHostInfo->nLevel == SET_LOCAL_HOST_PROPERTIES_GAP_COUNT) {

                pSetLocalHostProps2 = (PSET_LOCAL_HOST_PROPS2)pSetLocalHostInfo->Information;

                _ultoa_s(
                    pSetLocalHostProps2->GapCountLowerBound, 
                    tmpBuff, 
                    (STRING_SIZE * sizeof(CHAR)), 
                    16);
                SetDlgItemText(hDlg, IDC_SET_LOCAL_HOST_GAP_COUNT, tmpBuff);
            }

            if (pSetLocalHostInfo->nLevel == SET_LOCAL_HOST_PROPERTIES_MODIFY_CROM) 
            {
                pSetLocalHostProps3 = (PSET_LOCAL_HOST_PROPS3)pSetLocalHostInfo->Information;

                CheckRadioButton( hDlg,
                                  IDC_SET_LOCAL_HOST_CROM_ADD,
                                  IDC_SET_LOCAL_HOST_CROM_REMOVE,
                                  pSetLocalHostProps3->fulFlags + (IDC_SET_LOCAL_HOST_CROM_ADD-1));

                StringCbPrintf( tmpBuff, (STRING_SIZE * sizeof(tmpBuff[0])), "%p", 
                                pSetLocalHostProps3->hCromData);
                SetDlgItemText(hDlg, IDC_SET_LOCAL_HOST_CROM_HCROMDATA, tmpBuff);

                _ultoa_s(
                    pSetLocalHostProps3->nLength, 
                    tmpBuff, 
                    (STRING_SIZE * sizeof(CHAR)), 
                    16);
                SetDlgItemText(hDlg, IDC_SET_LOCAL_HOST_CROM_NLENGTH, tmpBuff);
            }

            return(TRUE); // WM_INITDIALOG

        case WM_COMMAND:

            switch (LOWORD(wParam)) {

                case IDOK:

                    if (IsDlgButtonChecked(hDlg, IDC_SET_LOCAL_HOST_LEVEL_GAP_COUNT)) {

                        pSetLocalHostInfo->nLevel = SET_LOCAL_HOST_PROPERTIES_GAP_COUNT;

                        GetDlgItemText(hDlg, IDC_SET_LOCAL_HOST_GAP_COUNT, tmpBuff, STRING_SIZE);
                        pSetLocalHostProps2->GapCountLowerBound = strtoul(tmpBuff, NULL, 16);

                    }

                    if (IsDlgButtonChecked(hDlg, IDC_SET_LOCAL_HOST_LEVEL_CROM)) 
                    {
                        pSetLocalHostInfo->nLevel = SET_LOCAL_HOST_PROPERTIES_MODIFY_CROM;

                        if (IsDlgButtonChecked(hDlg, IDC_SET_LOCAL_HOST_CROM_ADD))
                            pSetLocalHostProps3->fulFlags = SLHP_FLAG_ADD_CROM_DATA;

                        if (IsDlgButtonChecked(hDlg, IDC_SET_LOCAL_HOST_CROM_REMOVE))
                            pSetLocalHostProps3->fulFlags = SLHP_FLAG_REMOVE_CROM_DATA;

                        GetDlgItemText(hDlg, IDC_SET_LOCAL_HOST_CROM_HCROMDATA, tmpBuff, 
                                        STRING_SIZE);
                        if (!sscanf_s (tmpBuff, "%p", &(pSetLocalHostProps3->hCromData)))
                        {
                            // failed to get the handle, just return here
                            EndDialog(hDlg, TRUE);
                            return FALSE;
                        }

                        GetDlgItemText(hDlg, IDC_SET_LOCAL_HOST_CROM_NLENGTH, tmpBuff, STRING_SIZE);
                        pSetLocalHostProps3->nLength = strtoul(tmpBuff, NULL, 16);

                        // let's set the correct buffer size...
                        pSetLocalHostInfo->ulBufferSize = sizeof(SET_LOCAL_HOST_INFORMATION)+ \
                            sizeof(SET_LOCAL_HOST_PROPS3)+pSetLocalHostProps3->nLength;
                    }

                    EndDialog(hDlg, TRUE);
                    return(TRUE); // IDOK

                case IDCANCEL:
                    EndDialog(hDlg, FALSE);
                    return(TRUE); // IDCANCEL

                default:
                    return(TRUE); // default

            } // switch

            break; // WM_COMMAND

        default:
            break; // default

    } // switch

    return(FALSE);
} // SetLocalHostInfoDlgProc

void
w1394_SetLocalHostInfo(
                       HWND         hWnd,
                       _In_ PSTR    szDeviceName)
{
    PSET_LOCAL_HOST_INFORMATION     SetLocalHostInfo;
    PSET_LOCAL_HOST_PROPS3          SetLocalHostProps3;
    DWORD                           dwRet, bytesReturned;
    ULONG                           ulBufferSize;
    PULONG                          UnitDir;

    UNREFERENCED_PARAMETER(szDeviceName);

    TRACE(TL_TRACE, (hWnd, "Enter w1394_SetLocalHostInfo\r\n"));

    ulBufferSize = \
        sizeof(SET_LOCAL_HOST_INFORMATION) +\
        sizeof(SET_LOCAL_HOST_PROPS3) +\
        sizeof (ULONG); // this is going to house the return pointer.

    SetLocalHostInfo = (PSET_LOCAL_HOST_INFORMATION)LocalAlloc(LPTR, ulBufferSize);
    if (!SetLocalHostInfo) 
    {
        TRACE(TL_ERROR, (hWnd, "Unable to allocate SetLocalHostInfo!\r\n"));
        goto Exit_SetLocalHostInfo;
    }

    SetLocalHostInfo->nLevel = SET_LOCAL_HOST_PROPERTIES_MODIFY_CROM;
    SetLocalHostInfo->ulBufferSize = ulBufferSize;

    SetLocalHostProps3 = (PSET_LOCAL_HOST_PROPS3)SetLocalHostInfo->Information;

    SetLocalHostProps3->fulFlags = SLHP_FLAG_ADD_CROM_DATA;
    SetLocalHostProps3->hCromData = 0;
    SetLocalHostProps3->nLength = 12;

    UnitDir = (PULONG) &SetLocalHostProps3->Buffer;

    *UnitDir = 0xEFBE0200;
    UnitDir++;
    *UnitDir = 0x0A000012;
    UnitDir++;
    *UnitDir = 0x0B000013;

    if (DialogBoxParam( 
        (HINSTANCE) GetWindowLongPtr(hWnd, GWLP_HINSTANCE),
        "SetLocalHostInfo",
        hWnd,
        SetLocalHostInfoDlgProc,
        (LPARAM)SetLocalHostInfo)) 
    {
        ULONG inBuffer = sizeof(SET_LOCAL_HOST_INFORMATION) + ulBufferSize;
        ULONG outBuffer = sizeof(SET_LOCAL_HOST_INFORMATION) + ulBufferSize;

        dwRet = SendRequest (
            IOCTL_SET_LOCAL_HOST_INFORMATION,
            SetLocalHostInfo,
            inBuffer,
            SetLocalHostInfo,
            outBuffer,
            &bytesReturned);
        if (ERROR_SUCCESS != dwRet)
        {
            TRACE(TL_WARNING, (hWnd, "Failed SendRequest 0x%x\r\n", dwRet));
        }
        else
        {  
            TRACE(TL_TRACE, (hWnd, "hCromData = 0x%x\r\n", SetLocalHostProps3->hCromData));
        }

    }

Exit_SetLocalHostInfo:

    LocalFree(SetLocalHostInfo);

    TRACE(TL_TRACE, (hWnd, "Exit w1394_SetLocalHostInfo\r\n\r\n"));
    return;
} // w1394_SetLocalHostInfo

VOID
DisplayLocalHost (
                  HWND    hWnd,
                  PGET_LOCAL_HOST_INFORMATION pGetLocalHostInfo)
{

    if (pGetLocalHostInfo->nLevel == 1) 
    {

        PGET_LOCAL_HOST_INFO1   LocalHostInfo1;

        LocalHostInfo1 = (PGET_LOCAL_HOST_INFO1)&pGetLocalHostInfo->Information;

        TRACE(TL_TRACE, (hWnd, "UniqueId.LowPart = 0x%x\r\n", LocalHostInfo1->UniqueId.LowPart));
        TRACE(TL_TRACE, (hWnd, "UniqueId.HighPart = 0x%x\r\n", LocalHostInfo1->UniqueId.HighPart));
    }
    else if (pGetLocalHostInfo->nLevel == 2) 
    {

        PGET_LOCAL_HOST_INFO2   LocalHostInfo2;

        LocalHostInfo2 = (PGET_LOCAL_HOST_INFO2)&pGetLocalHostInfo->Information;

        TRACE(TL_TRACE, (hWnd, "HostCapabilities = 0x%x\r\n", LocalHostInfo2->HostCapabilities));
        TRACE(TL_TRACE, (hWnd, "MaxAsyncReadRequest = 0x%x\r\n", 
                        LocalHostInfo2->MaxAsyncReadRequest));
        TRACE(TL_TRACE, (hWnd, "MaxAsyncWriteRequest = 0x%x\r\n", 
                        LocalHostInfo2->MaxAsyncWriteRequest));
    }
    else if (pGetLocalHostInfo->nLevel == 3) 
    {

        PGET_LOCAL_HOST_INFO3   LocalHostInfo3;

        LocalHostInfo3 = (PGET_LOCAL_HOST_INFO3)&pGetLocalHostInfo->Information;

        TRACE(TL_TRACE, (hWnd, "deciWattsSupplied = 0x%x\r\n", LocalHostInfo3->deciWattsSupplied));
        TRACE(TL_TRACE, (hWnd, "Voltage = 0x%x\r\n", LocalHostInfo3->Voltage));
    }
    else if (pGetLocalHostInfo->nLevel == 4) 
    {

        PGET_LOCAL_HOST_INFO4   LocalHostInfo4;

        LocalHostInfo4 = (PGET_LOCAL_HOST_INFO4)&pGetLocalHostInfo->Information;

        TRACE(TL_TRACE, (hWnd, "PhysAddrMappingRoutine = 0x%x\r\n", 
                        LocalHostInfo4->PhysAddrMappingRoutine));
        TRACE(TL_TRACE, (hWnd, "Context = 0x%x\r\n", LocalHostInfo4->Context));
    }
    else if (pGetLocalHostInfo->nLevel == 7)
    {
        GET_LOCAL_HOST_INFO7    LocalHostInfo7;

        CopyMemory (&LocalHostInfo7, pGetLocalHostInfo->Information, sizeof (GET_LOCAL_HOST_INFO7));

        TRACE(TL_TRACE, (hWnd, "Host DMA Capabilities = 0x%x\r\n", LocalHostInfo7.HostDmaCapabilities));
        TRACE(TL_TRACE, (hWnd, "Max DMA Buffer Size =0x%x\r\n", LocalHostInfo7.MaxDmaBufferSize));
    }
    else if (pGetLocalHostInfo->nLevel == 6) 
    {
        PGET_LOCAL_HOST_INFO6   LocalHostInfo6;

        LocalHostInfo6 = (PGET_LOCAL_HOST_INFO6)&pGetLocalHostInfo->Information;

        TRACE(TL_TRACE, (hWnd, "CsrBaseAddress = 0x%x\r\n", LocalHostInfo6->CsrBaseAddress));
        TRACE(TL_TRACE, (hWnd, "CsrDataLength = 0x%x\r\n", LocalHostInfo6->CsrDataLength));
        TRACE(TL_TRACE, (hWnd, "CsrDataBuffer = 0x%x\r\n", &LocalHostInfo6->CsrDataBuffer));

        if (LocalHostInfo6->CsrBaseAddress.Off_Low == SPEED_MAP_LOCATION)
        {

            PSPEED_MAP  SpeedMap;

            SpeedMap = (PSPEED_MAP)&LocalHostInfo6->CsrDataBuffer;

            TRACE(TL_TRACE, (hWnd, "SpeedMap.SPD_Length = 0x%x\r\n", SpeedMap->SPD_Length));
            TRACE(TL_TRACE, (hWnd, "SpeedMap.SPD_CRC = 0x%x\r\n", SpeedMap->SPD_CRC));
            TRACE(TL_TRACE, (hWnd, "SpeedMap.SPD_Generation = 0x%x\r\n", 
                            SpeedMap->SPD_Generation));
        }
        else if (LocalHostInfo6->CsrBaseAddress.Off_Low == TOPOLOGY_MAP_LOCATION)
        {

            PTOPOLOGY_MAP   TopologyMap;
            ULONG           i;
            PSELF_ID        SelfId;
            PSELF_ID_MORE   SelfIdMore;
            BOOL            bMore;

            TopologyMap = (PTOPOLOGY_MAP)&LocalHostInfo6->CsrDataBuffer;

            TRACE(TL_TRACE, (hWnd, "TopologyMap.TOP_Length = 0x%x\r\n", 
                TopologyMap->TOP_Length));
            TRACE(TL_TRACE, (hWnd, "TopologyMap.TOP_CRC = 0x%x\r\n", 
                TopologyMap->TOP_CRC));
            TRACE(TL_TRACE, (hWnd, "TopologyMap.TOP_Generation = 0x%x\r\n", 
                TopologyMap->TOP_Generation));
            TRACE(TL_TRACE, (hWnd, "TopologyMap.TOP_Node_Count = 0x%x\r\n", 
                TopologyMap->TOP_Node_Count));
            TRACE(TL_TRACE, (hWnd, "TopologyMap.TOP_Self_ID_Count = 0x%x\r\n", 
                TopologyMap->TOP_Self_ID_Count));

            for (i=0; i < TopologyMap->TOP_Self_ID_Count; i++) 
            {

                SelfId = &TopologyMap->TOP_Self_ID_Array[i];

                TRACE(TL_TRACE, (hWnd, "SelfId[%d].SID_Phys_ID = 0x%x\r\n", i, SelfId->SID_Phys_ID));
                TRACE(TL_TRACE, (hWnd, "SelfId[%d].SID_Packet_ID = 0x%x\r\n", i, SelfId->SID_Packet_ID));
                TRACE(TL_TRACE, (hWnd, "SelfId[%d].SID_Gap_Count = 0x%x\r\n", i, SelfId->SID_Gap_Count));
                TRACE(TL_TRACE, (hWnd, "SelfId[%d].SID_Link_Active = 0x%x\r\n", i, SelfId->SID_Link_Active));
                TRACE(TL_TRACE, (hWnd, "SelfId[%d].SID_Zero = 0x%x\r\n", i, SelfId->SID_Zero));
                TRACE(TL_TRACE, (hWnd, "SelfId[%d].SID_Power_Class = 0x%x\r\n", 
                                i, SelfId->SID_Power_Class));
                TRACE(TL_TRACE, (hWnd, "SelfId[%d].SID_Contender = 0x%x\r\n", i, SelfId->SID_Contender));
                TRACE(TL_TRACE, (hWnd, "SelfId[%d].SID_Delay = 0x%x\r\n", i, SelfId->SID_Delay));
                TRACE(TL_TRACE, (hWnd, "SelfId[%d].SID_Speed = 0x%x\r\n", i, SelfId->SID_Speed));
                TRACE(TL_TRACE, (hWnd, "SelfId[%d].SID_More_Packets = 0x%x\r\n", 
                                i, SelfId->SID_More_Packets));
                TRACE(TL_TRACE, (hWnd, "SelfId[%d].SID_Initiated_Rst = 0x%x\r\n", 
                                i, SelfId->SID_Initiated_Rst));
                TRACE(TL_TRACE, (hWnd, "SelfId[%d].SID_Port3 = 0x%x\r\n", i, SelfId->SID_Port3));
                TRACE(TL_TRACE, (hWnd, "SelfId[%d].SID_Port2 = 0x%x\r\n", i, SelfId->SID_Port2));
                TRACE(TL_TRACE, (hWnd, "SelfId[%d].SID_Port1 = 0x%x\r\n", i, SelfId->SID_Port1));

                if (SelfId->SID_More_Packets)
                    bMore = TRUE;
                else
                    bMore = FALSE;

                while (bMore) 
                {

                    i++;

                    SelfIdMore = (PSELF_ID_MORE)&TopologyMap->TOP_Self_ID_Array[i];

                    TRACE(TL_TRACE, (hWnd, "SelfId[%d].SID_Phys_ID = 0x%x\r\n", 
                                    i, SelfIdMore->SID_Phys_ID));
                    TRACE(TL_TRACE, (hWnd, "SelfId[%d].SID_Packet_ID = 0x%x\r\n", 
                                    i, SelfIdMore->SID_Packet_ID));
                    TRACE(TL_TRACE, (hWnd, "SelfId[%d].SID_PortA = 0x%x\r\n", i, SelfIdMore->SID_PortA));
                    TRACE(TL_TRACE, (hWnd, "SelfId[%d].SID_Reserved2 = 0x%x\r\n", 
                                    i, SelfIdMore->SID_Reserved2));
                    TRACE(TL_TRACE, (hWnd, "SelfId[%d].SID_Sequence = 0x%x\r\n", 
                                    i, SelfIdMore->SID_Sequence));
                    TRACE(TL_TRACE, (hWnd, "SelfId[%d].SID_One = 0x%x\r\n", i, SelfIdMore->SID_One));
                    TRACE(TL_TRACE, (hWnd, "SelfId[%d].SID_PortE = 0x%x\r\n", i, SelfIdMore->SID_PortE));
                    TRACE(TL_TRACE, (hWnd, "SelfId[%d].SID_PortD = 0x%x\r\n", i, SelfIdMore->SID_PortD));
                    TRACE(TL_TRACE, (hWnd, "SelfId[%d].SID_PortC = 0x%x\r\n", i, SelfIdMore->SID_PortC));
                    TRACE(TL_TRACE, (hWnd, "SelfId[%d].SID_PortB = 0x%x\r\n", i, SelfIdMore->SID_PortB));
                    TRACE(TL_TRACE, (hWnd, "SelfId[%d].SID_More_Packets = 0x%x\r\n", 
                                    i, SelfIdMore->SID_More_Packets));
                    TRACE(TL_TRACE, (hWnd, "SelfId[%d].SID_Reserved3 = 0x%x\r\n", 
                                    i, SelfIdMore->SID_Reserved3));
                    TRACE(TL_TRACE, (hWnd, "SelfId[%d].SID_PortH = 0x%x\r\n", i, SelfIdMore->SID_PortH));
                    TRACE(TL_TRACE, (hWnd, "SelfId[%d].SID_PortG = 0x%x\r\n", i, SelfIdMore->SID_PortG));
                    TRACE(TL_TRACE, (hWnd, "SelfId[%d].SID_PortF = 0x%x\r\n", i, SelfIdMore->SID_PortF));

                    if (SelfIdMore->SID_More_Packets)
                    {
                        bMore = TRUE;
                    }
                    else
                    {
                        bMore = FALSE;
                    }
                }// while (bMore)
            } // for (TopologyMap->TopSelf_ID_Count)
        } // else if (LocalHostInfo6->CsrBaseAddress.Off_Low == TOPOLOGY_MAP_LOCATION)
    } // else if (pGetLocalHostInfo->nLevel == 6) 
}


