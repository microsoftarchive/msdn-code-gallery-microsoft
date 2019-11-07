/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    Main.cpp

Abstract:

    A simple test for bthecho sample.

Environment:

    user mode only

--*/

#define INITGUID

#include <windows.h>
#include <strsafe.h>
#include <setupapi.h>
#include <stdio.h>
#include <stdlib.h>
#include "public.h"

char testData[] = "WDF Bluetooth Sample Echo";
char replyData[sizeof(testData)];

PSP_DEVICE_INTERFACE_DETAIL_DATA
GetDeviceInterfaceDetailData(
    _In_ LPGUID InterfaceGuid
    );

BOOL
DoEcho(
    HANDLE hDevice
    );

VOID 
__cdecl 
main()
{
    HANDLE hDevice;
    BOOL echo;
    PSP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData = NULL;
        
    deviceInterfaceDetailData = GetDeviceInterfaceDetailData((LPGUID)&BTHECHOSAMPLE_DEVICE_INTERFACE);
    if (deviceInterfaceDetailData == NULL) {
        printf("Failed to get the device interface detail data\n");
        exit(1);
    }
    

    printf("DevicePath: %s\n", deviceInterfaceDetailData->DevicePath);

    hDevice = CreateFile(deviceInterfaceDetailData->DevicePath,
                         GENERIC_READ|GENERIC_WRITE,
                         FILE_SHARE_READ | FILE_SHARE_WRITE,
                         NULL,
                         OPEN_EXISTING,
                         0,
                         NULL );

    if (hDevice == INVALID_HANDLE_VALUE) {
        printf("Failed to open device. Error %d\n",GetLastError());
        LocalFree(deviceInterfaceDetailData);
        exit(1);
    } 

    printf("Opened device successfully\n");

    do
    {
        echo = DoEcho(hDevice);
    } while (echo);
                
    CloseHandle(hDevice);
    printf("Closed device\n");

    LocalFree(deviceInterfaceDetailData);
    
}

BOOL
DoEcho(
    HANDLE hDevice
    )
{
    BOOL retval = FALSE;
    DWORD cbWritten = 0;
    DWORD cbRead = 0;
    
    BOOL bRet = WriteFile(
        hDevice,
        testData,
        sizeof(testData),
        &cbWritten,
        NULL
        );

    if (!bRet)
    {
        printf("Write failed. Error: %d\n", GetLastError());
        goto exit;
    }
    else
    {
        printf("Written \t\t%d bytes: %s\n", cbWritten, testData);
    }

    bRet = ReadFile(
        hDevice,
        replyData,
        cbWritten,
        &cbRead,
        NULL
        );

    if (!bRet)
    {
        printf("Read failed. Error: %d\n", GetLastError());
        goto exit;
    }
    else
    {
        printf("Reply from server \t%d bytes: %s\n", cbRead, replyData);
    }

    if (cbRead == cbWritten)
    {
        retval = memcmp(replyData, testData, cbRead) ? FALSE : TRUE;
    }

exit:
    return retval;
}

PSP_DEVICE_INTERFACE_DETAIL_DATA
GetDeviceInterfaceDetailData(
    _In_ LPGUID InterfaceGuid
    )
{
    HDEVINFO HardwareDeviceInfo;
    SP_DEVICE_INTERFACE_DATA DeviceInterfaceData;
    PSP_DEVICE_INTERFACE_DETAIL_DATA DeviceInterfaceDetailData = NULL;
    ULONG Length, RequiredLength = 0;
    BOOL bResult;

    HardwareDeviceInfo = SetupDiGetClassDevs(
                             InterfaceGuid,
                             NULL,
                             NULL,
                             (DIGCF_PRESENT | DIGCF_DEVICEINTERFACE));

    if (HardwareDeviceInfo == INVALID_HANDLE_VALUE) {
        printf("SetupDiGetClassDevs failed!\n");
        exit(1);
    }

    DeviceInterfaceData.cbSize = sizeof(SP_DEVICE_INTERFACE_DATA);

    bResult = SetupDiEnumDeviceInterfaces(HardwareDeviceInfo,
                                              0,
                                              InterfaceGuid,
                                              0,
                                              &DeviceInterfaceData);

    if (bResult == FALSE) {

        LPVOID lpMsgBuf = NULL;

        if (FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER |
                          FORMAT_MESSAGE_FROM_SYSTEM |
                          FORMAT_MESSAGE_IGNORE_INSERTS,
                          NULL,
                          GetLastError(),
                          MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
                          (LPSTR) &lpMsgBuf,
                          0,
                          NULL
                          )) {

            printf("Error: %s", (LPSTR)lpMsgBuf);
            LocalFree(lpMsgBuf);
        }

        printf("SetupDiEnumDeviceInterfaces failed.\n");
        SetupDiDestroyDeviceInfoList(HardwareDeviceInfo);
        exit(1);
    }

    SetupDiGetDeviceInterfaceDetail(
        HardwareDeviceInfo,
        &DeviceInterfaceData,
        NULL,
        0,
        &RequiredLength,
        NULL
        );

    DeviceInterfaceDetailData = (PSP_DEVICE_INTERFACE_DETAIL_DATA) LocalAlloc(LMEM_FIXED, RequiredLength);

    if (DeviceInterfaceDetailData == NULL) {
        SetupDiDestroyDeviceInfoList(HardwareDeviceInfo);
        printf("Failed to allocate memory.\n");
        exit(1);
    }

    DeviceInterfaceDetailData->cbSize = sizeof(SP_DEVICE_INTERFACE_DETAIL_DATA);

    Length = RequiredLength;

    bResult = SetupDiGetDeviceInterfaceDetail(
                  HardwareDeviceInfo,
                  &DeviceInterfaceData,
                  DeviceInterfaceDetailData,
                  Length,
                  &RequiredLength,
                  NULL);

    if (bResult == FALSE) {

        LPVOID lpMsgBuf = NULL;

        if (FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER |
                         FORMAT_MESSAGE_FROM_SYSTEM |
                         FORMAT_MESSAGE_IGNORE_INSERTS,
                         NULL,
                         GetLastError(),
                         MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
                         (LPSTR) &lpMsgBuf,
                         0,
                         NULL)) {
            MessageBox(NULL, (LPCTSTR) lpMsgBuf, "Error", MB_OK);
            LocalFree(lpMsgBuf);
        }
        
        SetupDiDestroyDeviceInfoList(HardwareDeviceInfo);       
        printf("Error in SetupDiGetDeviceInterfaceDetail\n");
        LocalFree(DeviceInterfaceDetailData);
        DeviceInterfaceDetailData = NULL;
        exit(1);
    }

    SetupDiDestroyDeviceInfoList(HardwareDeviceInfo);

    return DeviceInterfaceDetailData;
    
}

