/*++

Copyright (c) 1998  Microsoft Corporation

Module Name:

    1394.h

Abstract

    1394 api wrappers

--*/

VOID
DisplayLocalHost (
                  HWND    hWnd,
                  PGET_LOCAL_HOST_INFORMATION pGetLocalHostInfo);

INT_PTR CALLBACK
BusResetDlgProc(
    HWND        hDlg,
    UINT        uMsg,
    WPARAM      wParam,
    LPARAM      lParam
    );

void
w1394_BusReset(
    HWND         hWnd,
    _In_ PSTR    szDeviceName
    );

void
w1394_GetGenerationCount(
    HWND         hWnd,
    _In_ PSTR    szDeviceName
    );

INT_PTR CALLBACK
GetLocalHostInfoDlgProc(
    HWND        hDlg,
    UINT        uMsg,
    WPARAM      wParam,
    LPARAM      lParam
    );

void
w1394_GetLocalHostInfo(
    HWND         hWnd,
    _In_ PSTR    szDeviceName
    );

INT_PTR CALLBACK
Get1394AddressFromDeviceObjectDlgProc(
    HWND        hDlg,
    UINT        uMsg,
    WPARAM      wParam,
    LPARAM      lParam
    );

void
w1394_Get1394AddressFromDeviceObject(
    HWND         hWnd,
    _In_ PSTR    szDeviceName
    );

void
w1394_Control(
    HWND         hWnd,
    _In_ PSTR    szDeviceName
    );

INT_PTR CALLBACK
GetMaxSpeedBetweenDevicesDlgProc(
    HWND        hDlg,
    UINT        uMsg,
    WPARAM      wParam,
    LPARAM      lParam
    );

void
w1394_GetMaxSpeedBetweenDevices(
    HWND         hWnd,
    _In_ PSTR    szDeviceName
    );

void
w1394_GetConfigurationInfo(
    HWND         hWnd,
    _In_ PSTR    szDeviceName
    );

INT_PTR CALLBACK
SetDeviceXmitPropertiesDlgProc(
    HWND        hDlg,
    UINT        uMsg,
    WPARAM      wParam,
    LPARAM      lParam
    );

void
w1394_SetDeviceXmitProperties(
    HWND         hWnd,
    _In_ PSTR    szDeviceName
    );

INT_PTR CALLBACK
SendPhyConfigDlgProc(
    HWND        hDlg,
    UINT        uMsg,
    WPARAM      wParam,
    LPARAM      lParam
    );

void
w1394_SendPhyConfigPacket(
    HWND         hWnd,
    _In_ PSTR    szDeviceName
    );

/* Commenting this code. This function is not defined. 
The application is not calling it

void
w1394_GetSpeedTopologyMaps(
    HWND    hWnd,
    PSTR    szDeviceName
    );
*/

INT_PTR CALLBACK
BusResetNotificationDlgProc(
    HWND        hDlg,
    UINT        uMsg,
    WPARAM      wParam,
    LPARAM      lParam
    );

void
w1394_BusResetNotification(
    HWND         hWnd,
    _In_ PSTR    szDeviceName
    );

INT_PTR CALLBACK
SetLocalHostInfoDlgProc(
    HWND        hDlg,
    UINT        uMsg,
    WPARAM      wParam,
    LPARAM      lParam
    );

void
w1394_SetLocalHostInfo(
    HWND         hWnd,
    _In_ PSTR    szDeviceName
    );


