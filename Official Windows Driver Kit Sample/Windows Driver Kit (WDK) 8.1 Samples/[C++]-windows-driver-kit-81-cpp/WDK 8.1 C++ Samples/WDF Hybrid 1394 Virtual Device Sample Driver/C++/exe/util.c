/*++

Copyright (c) 1997-1998  Microsoft Corporation

Module Name: 

    util.c

Abstract

    Misc. functions that are convient to have around.
--*/

#define _UTIL_C
#include "pch.h"
#undef _UTIL_C

HANDLE
OpenDevice (
            HANDLE       hWnd,
            _In_ PSTR    szDeviceName)
{
    HANDLE  hDevice;
    CHAR    tmpBuff[STRING_SIZE];

    hDevice = CreateFile (
        szDeviceName,
        GENERIC_WRITE | GENERIC_READ,
        FILE_SHARE_WRITE | FILE_SHARE_READ,
        NULL,
        OPEN_EXISTING,
        FILE_ATTRIBUTE_NORMAL,
        NULL);

    if (INVALID_HANDLE_VALUE == hDevice) 
    {

        TRACE(TL_ERROR, (hWnd, "Failed to open device!\r\n"));

        if (hWnd) 
        {
            StringCbPrintf (
                tmpBuff, 
                (STRING_SIZE * sizeof(tmpBuff[0])), 
                "Error Opening Device 0x%x\r\n", 
                GetLastError());

            WriteTextToEditControl (hWnd, tmpBuff);
        }
    }

    return (hDevice);
} // OpenDevice

DWORD
WINAPI
RemoveVirtualDriver (
                     HWND           hWnd,
                     PVIRT_DEVICE   pVirtualDevice,
                     ULONG          BusNumber)
{
    HANDLE                      hDevice = INVALID_HANDLE_VALUE;
    ULONG                       ulStrLen = 0;
    size_t                      retVal = 0;
    UCHAR                       BusName[16] = "\\\\.\\1394BUS";
    PIEEE1394_API_REQUEST       p1394ApiReq = NULL;
    PIEEE1394_VDEV_PNP_REQUEST  pDevPnpReq = NULL;
    DWORD                       dwBytesRet = 0;
    DWORD                       dwRet = ERROR_SUCCESS;
    ULONG                       p1394ApiReqLen = 0;

    if (!pVirtualDevice->DeviceID)
    {
        dwRet = ERROR_INVALID_PARAMETER;

        goto Exit_RemoveVirtualDriver;
    }

    _itoa_s (
        BusNumber, 
        (CHAR *) &BusName[11], 
        ((sizeof (BusName) / sizeof (BusName[0])) - (strlen((CHAR *)BusName))),
        10);


    hDevice = OpenDevice (hWnd, (PSTR) &BusName);
    if (hDevice != INVALID_HANDLE_VALUE) 
    {

        //taking care of NULL termination character
        retVal = strlen(pVirtualDevice->DeviceID) + 1;

        if (S_OK != (SizeTToULong (retVal, &ulStrLen)))
        {
            dwRet = GetLastError();
            TRACE (TL_ERROR, (hWnd, "SizeTToULong failed: %d\n", dwRet));

            goto Exit_RemoveVirtualDriver;
        }



        if ( pVirtualDevice->fulFlags & IEEE1394_REQUEST_FLAG_UNICODE ) 
        {
            p1394ApiReq = \
                (PIEEE1394_API_REQUEST) LocalAlloc (
                LPTR, 
                sizeof(IEEE1394_API_REQUEST)+(ulStrLen * sizeof(WCHAR)));

            if (NULL == p1394ApiReq)
            {
                dwRet = GetLastError();
                TRACE (TL_FATAL, (hWnd, "LocalAlloc failed: %d\n", dwRet));

                goto Exit_RemoveVirtualDriver;
            }

            p1394ApiReqLen = \
                sizeof(IEEE1394_API_REQUEST)+(ulStrLen * sizeof(WCHAR));
        } 
        else 
        {
            p1394ApiReq = (PIEEE1394_API_REQUEST) LocalAlloc (
                LPTR, 
                sizeof(IEEE1394_API_REQUEST)+ulStrLen);
            
            if (NULL == p1394ApiReq)
            {
                dwRet = GetLastError();
                TRACE (TL_FATAL, (hWnd, "LocalAlloc failed: %d\n", dwRet));

                goto Exit_RemoveVirtualDriver;
            }

            p1394ApiReqLen = sizeof(IEEE1394_API_REQUEST)+ ulStrLen;
        }

        p1394ApiReq->RequestNumber = IEEE1394_API_REMOVE_VIRTUAL_DEVICE;
        p1394ApiReq->Flags = 0;

        pDevPnpReq = &p1394ApiReq->u.RemoveVirtualDevice;

        pDevPnpReq->fulFlags = pVirtualDevice->fulFlags;
        pDevPnpReq->Reserved = 0;
        pDevPnpReq->InstanceId = pVirtualDevice->InstanceID;
        
        if (pVirtualDevice->fulFlags & IEEE1394_REQUEST_FLAG_UNICODE) 
        {
            if (S_OK != StringCchCopyNW (
                (LPWSTR)&pDevPnpReq->DeviceId,
                ulStrLen,
                (LPWSTR)pVirtualDevice->DeviceID, 
                ulStrLen))
            {
                TRACE (TL_FATAL, (hWnd, "StringCchCopyNW failed\n"));
                dwRet = ERROR_INSUFFICIENT_BUFFER;

                goto Exit_RemoveVirtualDriver;
            }
        
            if (!DeviceIoControl (
                hDevice,
                IOCTL_IEEE1394_API_REQUEST,
                p1394ApiReq,
                p1394ApiReqLen,
                NULL,
                0,
                &dwBytesRet,
                NULL))
            {
                dwRet = GetLastError();

                goto Exit_RemoveVirtualDriver;
            }
        } 
        else 
        {
            if (S_OK != StringCchCopyNA (
                (STRSAFE_LPSTR) &pDevPnpReq->DeviceId,
                ulStrLen,
                (STRSAFE_LPSTR) pVirtualDevice->DeviceID, 
                ulStrLen))
            {
                TRACE (TL_FATAL, (hWnd, "StringCchCopyNA failed\n"));
                dwRet = ERROR_INSUFFICIENT_BUFFER;

                goto Exit_RemoveVirtualDriver;
            }

            if (!DeviceIoControl (
                hDevice,
                IOCTL_IEEE1394_API_REQUEST,
                p1394ApiReq,
                p1394ApiReqLen,
                NULL,
                0,
                &dwBytesRet,
                NULL))
            {
                dwRet = GetLastError();
            }
        }
    }
    else 
    {
        dwRet = ERROR_INVALID_HANDLE;
    }

Exit_RemoveVirtualDriver:

    if (INVALID_HANDLE_VALUE != hDevice)
    {
        CloseHandle (hDevice);
    }

    if (p1394ApiReq)
    {
        LocalFree (p1394ApiReq);
    }

    return dwRet;
}

DWORD
WINAPI
AddVirtualDriver (
                  HWND                hWnd,
                  PVIRT_DEVICE    pVirtualDevice,
                  ULONG               BusNumber)
{
    HANDLE                      hDevice;
    ULONG                       ulStrLen;
    size_t                      retVal = 0;
    UCHAR                       BusName[16] = "\\\\.\\1394BUS";
    PIEEE1394_API_REQUEST       p1394ApiReq;
    PIEEE1394_VDEV_PNP_REQUEST  pDevPnpReq;
    DWORD                       dwBytesRet;
    DWORD                       dwRet;
    ULONG                       p1394ApiReqLen = 0;

    TRACE(TL_TRACE, (hWnd, "AddVirtual Driver\r\n"));

    _itoa_s (
        BusNumber, 
        (CHAR *) &BusName[11], 
        ((sizeof (BusName) / sizeof (BusName[0])) - (strlen((CHAR *)BusName))),
        10);


    TRACE (TL_TRACE, (hWnd, "Bus: %s\r\n", BusName));

    hDevice = OpenDevice (hWnd, (PSTR) &BusName);
    if (hDevice != INVALID_HANDLE_VALUE)
    {

        //taking care of NULL terminating character
        retVal = strlen(pVirtualDevice->DeviceID) + 1;

        if (S_OK != (SizeTToULong (retVal, &ulStrLen)))
        {
            dwRet = GetLastError();
            TRACE (TL_ERROR, (hWnd, "SizeTToULong failed: %d\n", dwRet));
            return dwRet;
        }


        if ( pVirtualDevice->fulFlags & IEEE1394_REQUEST_FLAG_UNICODE ) 
        {
            p1394ApiReq = (PIEEE1394_API_REQUEST) LocalAlloc (
                LPTR, 
                sizeof(IEEE1394_API_REQUEST)+(ulStrLen * sizeof(WCHAR)));

            if (NULL == p1394ApiReq)
            {
                DWORD gle = GetLastError();

                TRACE (TL_FATAL, (hWnd, "LocalAlloc failed: %d\n", gle));
                return gle;
            }

            p1394ApiReqLen = \
                sizeof(IEEE1394_API_REQUEST)+(ulStrLen * sizeof(WCHAR));
        } 
        else 
        {
            p1394ApiReq = (PIEEE1394_API_REQUEST) LocalAlloc(LPTR, 
                sizeof(IEEE1394_API_REQUEST)+ulStrLen);

            if (NULL == p1394ApiReq)
            {
                DWORD gle = GetLastError();

                TRACE (TL_FATAL, (hWnd, "LocalAlloc failed: %d\n", gle));
                return gle;
            }

            p1394ApiReqLen = sizeof(IEEE1394_API_REQUEST)+ ulStrLen;
        }

        p1394ApiReq->RequestNumber = IEEE1394_API_ADD_VIRTUAL_DEVICE;
        p1394ApiReq->Flags = pVirtualDevice->fulFlags;

        pDevPnpReq = &p1394ApiReq->u.AddVirtualDevice;

        pDevPnpReq->fulFlags = pVirtualDevice->fulFlags;
        pDevPnpReq->Reserved = 0;
        pDevPnpReq->InstanceId = pVirtualDevice->InstanceID;

        if ( pVirtualDevice->fulFlags & IEEE1394_REQUEST_FLAG_UNICODE ) 
        {
            if (S_OK != StringCchCopyNW (
                (LPWSTR)&pDevPnpReq->DeviceId,
                ulStrLen,
                (LPWSTR)pVirtualDevice->DeviceID, 
                ulStrLen))
                            
            {
                TRACE (TL_FATAL, (hWnd, "StringCchCopyNW failed\n"));
                return ERROR_INSUFFICIENT_BUFFER;
            }


            dwRet = DeviceIoControl (
                hDevice,
                IOCTL_IEEE1394_API_REQUEST,
                p1394ApiReq,
                p1394ApiReqLen,
                NULL,
                0,
                &dwBytesRet,
                NULL);
        } 
        else 
        {
            if (S_OK != StringCchCopyNA (
                (STRSAFE_LPSTR) &pDevPnpReq->DeviceId,
                ulStrLen,
                (STRSAFE_LPSTR) pVirtualDevice->DeviceID, 
                ulStrLen))
            {
                TRACE (TL_FATAL, (hWnd, "StringCchCopyNA failed\n"));
                return ERROR_INSUFFICIENT_BUFFER;
            }

            dwRet = DeviceIoControl (
                hDevice,
                IOCTL_IEEE1394_API_REQUEST,
                p1394ApiReq,
                p1394ApiReqLen,
                NULL,
                0,
                &dwBytesRet,
                NULL);
        }

        if (!dwRet)
        {
            
            dwRet = GetLastError();
            TRACE(
                TL_WARNING, 
                (hWnd, 
                "Failed IOCTL_IEEE1394_API_REQEST 0x%x\r\n", 
                dwRet));
        }
        else 
        {
            TRACE(TL_WARNING, (hWnd, "Succeeded IOCTL_IEEE1394_API_REQEST\r\n"));
            dwRet = ERROR_SUCCESS;
        }

        // free resources

        CloseHandle (hDevice);
        LocalFree(p1394ApiReq);
    }
    else 
    {
        dwRet = ERROR_INVALID_HANDLE;
    }

    return dwRet;
}

DWORD
FillDeviceList (
                HWND            hWnd,
                LPGUID          InterfaceGuid,
                PDEVICE_DATA    pDeviceData)
{
    ULONG dwError = ERROR_SUCCESS;
    HDEVINFO HardwareDeviceInfo;
    SP_DEVICE_INTERFACE_DATA DeviceInterfaceData;
    PSP_DEVICE_INTERFACE_DETAIL_DATA DeviceInterfaceDetailData = NULL;
    ULONG Length, RequiredLength = 0;
    BOOL bResult;

    INT i = 0;

    // 
    // We'll only be looking for one device right now.
    //

    TRACE(TL_TRACE, (hWnd, "Entering FillDeviceList\r\n"));


    HardwareDeviceInfo = SetupDiGetClassDevs (
        InterfaceGuid, 
        NULL, 
        NULL,
        (DIGCF_PRESENT | DIGCF_DEVICEINTERFACE));

    if (INVALID_HANDLE_VALUE == HardwareDeviceInfo) 
    {
        dwError = GetLastError();
        TRACE(TL_WARNING, (hWnd, "Failed SetupDiGetClassDevs 0x%x\r\n", dwError));
        return dwError;
    }

    DeviceInterfaceData.cbSize = sizeof (SP_DEVICE_INTERFACE_DATA);

    bResult = SetupDiEnumDeviceInterfaces (
        HardwareDeviceInfo, 
        0, 
        InterfaceGuid, 
        0,
        &DeviceInterfaceData);

    if (!bResult) 
    {
        dwError = GetLastError();
        TRACE(
            TL_WARNING, 
            (hWnd, 
            "SetupDiEnumDeviceInterfaces failed: 0x%x\r\n", 
            GetLastError()));

        SetupDiDestroyDeviceInfoList (HardwareDeviceInfo);

        return dwError;
    }

    SetupDiGetDeviceInterfaceDetail (
        HardwareDeviceInfo, 
        &DeviceInterfaceData, 
        NULL, 
        0,
        &RequiredLength, 
        NULL);

    DeviceInterfaceDetailData = \
        (PSP_DEVICE_INTERFACE_DETAIL_DATA) LocalAlloc (
        LMEM_FIXED, 
        RequiredLength);

    if (NULL == DeviceInterfaceDetailData) 
    {
        dwError = ERROR_NOT_ENOUGH_MEMORY;
        TRACE(TL_WARNING, (hWnd, "Failed Allocate DeviceInterfaceDetail\r\n"));

        SetupDiDestroyDeviceInfoList (HardwareDeviceInfo);       

        return dwError;
    }

    DeviceInterfaceDetailData->cbSize = sizeof(SP_DEVICE_INTERFACE_DETAIL_DATA);

    Length = RequiredLength;

    bResult = SetupDiGetDeviceInterfaceDetail (
        HardwareDeviceInfo, 
        &DeviceInterfaceData,
        DeviceInterfaceDetailData, 
        Length, 
        &RequiredLength,
        NULL);

    if (!bResult) 
    {
        dwError = GetLastError();
        TRACE(
            TL_WARNING, 
            (hWnd, 
            "Failed SetupDiGetDeviceInterfaceDetail 0x%x\r\n", 
            dwError));
       
        LocalFree (DeviceInterfaceDetailData);
        SetupDiDestroyDeviceInfoList (HardwareDeviceInfo);  

        return dwError;
    }

    pDeviceData->numDevices++;

    if (S_OK != StringCbCopy(
        pDeviceData->deviceList[i].DeviceName, 
        sizeof(pDeviceData->deviceList[i].DeviceName),
        DeviceInterfaceDetailData->DevicePath))
    {
        TRACE (TL_FATAL, (hWnd, "StringCcbCopy failed\n"));
        return ERROR_INSUFFICIENT_BUFFER;
    }
    
    LocalFree(DeviceInterfaceDetailData);
    SetupDiDestroyDeviceInfoList (HardwareDeviceInfo);  
   

    TRACE(TL_TRACE, (hWnd, "Exiting FillDeviceList 0x%x\r\n", dwError));
    return dwError;
}

DWORD
SendRequest (
    _In_ DWORD Ioctl,
    _In_opt_ PVOID InputBuffer,
    _In_opt_ ULONG InputBufferSize,
    _Out_opt_ PVOID OutputBuffer,
    _In_opt_ ULONG OutputBufferSize,
    _Out_ LPDWORD bytesReturned)
{
    // 
    // Everything out of this test application will be going synchronously.
    // 

    if  (!DeviceIoControl (
        g_hTestDevice, 
        Ioctl,
        InputBuffer,
        InputBufferSize, 
        OutputBuffer, 
        OutputBufferSize, 
        bytesReturned, 
        NULL))
    {
        return GetLastError();
    }

    return ERROR_SUCCESS;
}

VOID
WriteTextToEditControl (
                       HWND hWndEdit, 
                       _In_ PCHAR str)
{
    INT     iLength;

    // get the end of buffer for edit control
    iLength = GetWindowTextLength (hWndEdit);

    // set current selection to that offset
    SendMessage (hWndEdit, EM_SETSEL, iLength, iLength);

    // add text
    SendMessage (hWndEdit, EM_REPLACESEL, 0, (LPARAM) str);

} // WriteTextToEditControl

//
// Generic singly linked list routines.
//

//***********************************************************************
//
void 
InsertTailList (
               PLIST_NODE head, 
               PLIST_NODE entry)
{
PLIST_NODE pCurrent = head;

    entry->pNext = 0;
    while(pCurrent->pNext)
        pCurrent = pCurrent->pNext;
    pCurrent->pNext = entry;

}

//***********************************************************************
//
BOOL 
RemoveEntryList (
                 PLIST_NODE head, 
                 PLIST_NODE entry)
{
PLIST_NODE pCurrent = head;

    while(pCurrent->pNext != entry){
        pCurrent = pCurrent->pNext;
        if(pCurrent == 0) return FALSE;
    }
    pCurrent->pNext = entry->pNext;
    return TRUE;
}
    


//***********************************************************************
//
void 
InsertHeadList (
                PLIST_NODE head, 
                PLIST_NODE entry)
{
    entry->pNext = head->pNext;
    head->pNext = entry;
}

//****************************************************************************
//
BOOL 
IsNodeOnList (
              PLIST_NODE head, 
              PLIST_NODE entry)
{
PLIST_NODE pCurrent = head;

    while(pCurrent->pNext != entry){
        pCurrent = pCurrent->pNext;
        if(pCurrent == 0) return FALSE;
    }
    return TRUE;
}


