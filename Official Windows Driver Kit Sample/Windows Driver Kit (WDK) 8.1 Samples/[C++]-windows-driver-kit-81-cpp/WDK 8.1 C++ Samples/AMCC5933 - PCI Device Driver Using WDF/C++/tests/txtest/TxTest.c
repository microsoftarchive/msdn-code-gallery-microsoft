/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    TxTest.c - Test AMCC S5933 Developer's Kit Ref. board driver (AMCC5933.SYS)

Abstract:

Environment:

    User mode

--*/

#include <windows.h>
#include <stdio.h>
#include <stdlib.h>
#include <malloc.h>

#pragma warning(disable:4201)  // disable nameless struct/union warnings
#include <winioctl.h>
#include <setupapi.h>
#pragma warning(default:4201)

#include "public.h"

#define BUFSIZE (1024 * 11)

static
ULONG   bufsize = BUFSIZE;


VOID
Dump(
   UCHAR * Buffer,
   ULONG   Length
   );

HANDLE
OpenDevice(
    IN CONST GUID * InterfaceGuid,
    IN ULONG        FileFlagOptions
    )
;

//-----------------------------------------------------------------------------
//
//-----------------------------------------------------------------------------
int
__cdecl
main(
    _In_ int argc,
    _In_reads_(argc) char* argv[]
    )
{
    HANDLE      hISAdevice;
    HANDLE      hPCIdevice;
    PDWORD      inbuf;
    PDWORD      outbuf;
    ULONG       count;
    DWORD       numread;
    OVERLAPPED  ol = {0};
    ULONG       offset;
    ULONG       length;
    DWORD       numwritten;
    BOOL        okay;
    ULONG       i;
    BOOL        hasErrors = FALSE;

    if (argc != 2) {
        bufsize = BUFSIZE;
    } else {
        if (sscanf_s( argv[1], "%d", &bufsize ) == 0) {
            printf("bad buffer size\n");
            exit(1);
        }
        if (bufsize % sizeof(DWORD)) {

            bufsize += sizeof(DWORD) - (bufsize % sizeof(DWORD));

            printf("Increasing buffer size to %d bytes "
                   "(integral of DWORDs)\n", bufsize);
        }
    }
    printf("Buffer size is %d bytes\n", bufsize);

    hISAdevice = OpenDevice(&GUID_DEVINTERFACE_AMCC_ISA,
                            FILE_FLAG_OVERLAPPED );

    if (hISAdevice == INVALID_HANDLE_VALUE) {
        puts("(ISA) Can't open S5933DK1 device");
        exit(1);
    }

    hPCIdevice = OpenDevice(&GUID_DEVINTERFACE_AMCC_PCI, 0);

    if (hPCIdevice == INVALID_HANDLE_VALUE) {
        puts("(PCI) Can't open AMCC5933 device");
        CloseHandle(hISAdevice);
        exit(1);
    }

    inbuf = (PDWORD) malloc(bufsize);
    if (inbuf == NULL) {
        puts("memory allocation for input buffer failed\n");
        exit(1);
    }

    outbuf = (PDWORD) malloc(bufsize);
    if (outbuf == NULL) {
        puts("memory allocation for output buffer failed\n");
        exit(1);
    }

    count = bufsize / sizeof(DWORD);

    for (i=0; i < count; i++) {
        outbuf[i] = i;
    }

    memset(inbuf, 0, bufsize);

    ol.hEvent = CreateEvent(NULL, TRUE, FALSE, NULL);

    okay = DeviceIoControl( hISAdevice,
                            IOCTL_READ_DMA,
                            NULL,
                            0,
                            inbuf,
                            bufsize,
                            &numread,
                            &ol );

    if (!okay && GetLastError() == ERROR_IO_PENDING) {

        okay = WriteFile( hPCIdevice,
                          outbuf,
                          bufsize,
                          &numwritten,
                          NULL );

        if (okay) {
            printf("(PCI) WriteFile succeeded in writing %d bytes\n",
                   numwritten);
        } else {
            printf("(PCI) WriteFile failed: error %d\n", GetLastError());
            hasErrors = TRUE;
        }

        okay = GetOverlappedResult( hISAdevice, &ol, &numread, TRUE );

        if (okay) {
            printf("(ISA) Overlapped IOCTL_READ_DMA succeeded in "
                   "reading %d bytes\n", numread);

            for (i=0; i < count; i++) {

                if (inbuf[i] != outbuf[i]) {

                    offset = (i < 16) ? 0 : i-16;
                    length = (i < bufsize - 16) ? 48 : 48 - (bufsize - i);

                    printf("However, data element %d doesn't match\n", i);

                    printf("\n-----------outbuf --------\n");
                    Dump(( UCHAR*) &outbuf[offset], length );
                    printf("\n-----------inbuf ---------\n");
                    Dump( (UCHAR*) &inbuf[offset],  length );
                    hasErrors = TRUE;
                    break;
                }
            }
            if (i == count) {
                printf("Data written equals data read.\n");
            }
        } else {
            printf("(ISA) Overlapped IOCTL_READ_DMA failed - %d\n", GetLastError());
            hasErrors = TRUE;
        }
    }
    else {
        printf("(ISA) IOCTL_READ_DMA failed - %d\n", GetLastError());
        hasErrors = TRUE;
    }

    CloseHandle(ol.hEvent);
    free(inbuf);
    free(outbuf);
    CloseHandle(hPCIdevice);
    CloseHandle(hISAdevice);

    exit((hasErrors)?1:0);
}

//--------------------------------------------------------------
//
//--------------------------------------------------------------
VOID
Dump(
   UCHAR * Buffer,
   ULONG   Length
   )
{
   ULONG   i;
   ULONG   WholeLines;
   UCHAR * Current = Buffer;

   if (Length == 0) {
       printf("\n");
       return;
   }

   //
   //  model of output below
   //
   //  00000000:  10 40 08 00 5A B8 0E 33 08 00 5A B8 00 96 AA AA
   //  00000010:  03 00 00 00 08 00 45 00 3C 64 FD 07 40 00 20 06
   //  00000020:  85 41 81 21 CD 03 81 21 CD 04
   //
   WholeLines = Length / 16;

   for (i=0; i<WholeLines; i++) {
      printf("%p: %02X %02X %02X %02X %02X %02X %02X %02X-"
                 "%02X %02X %02X %02X %02X %02X %02X %02X  "
                  "%c%c%c%c%c%c%c%c%c%c%c%c%c%c%c%c\n",
                Current,
                Current[0], Current[1], Current[2], Current[3],
                Current[4], Current[5], Current[6], Current[7],
                Current[8], Current[9], Current[10],Current[11],
                Current[12],Current[13],Current[14],Current[15],
                isprint(Current[0])  ? Current[0] : '.',
                isprint(Current[1])  ? Current[1] : '.',
                isprint(Current[2])  ? Current[2] : '.',
                isprint(Current[3])  ? Current[3] : '.',
                isprint(Current[4])  ? Current[4] : '.',
                isprint(Current[5])  ? Current[5] : '.',
                isprint(Current[6])  ? Current[6] : '.',
                isprint(Current[7])  ? Current[7] : '.',
                isprint(Current[8])  ? Current[8] : '.',
                isprint(Current[9])  ? Current[9] : '.',
                isprint(Current[10]) ? Current[10] : '.',
                isprint(Current[11]) ? Current[11] : '.',
                isprint(Current[12]) ? Current[12] : '.',
                isprint(Current[13]) ? Current[13] : '.',
                isprint(Current[14]) ? Current[14] : '.',
                isprint(Current[15]) ? Current[15] : '.' );

      Current += 16;
      Length  -= 16;
   }

   if (Length) {
      printf("%05X: ", (Current-Buffer));
      for (i=0; i<Length; i++) {
         printf("%02X ", Current[i]);
      }

      for (; i<16; i++) printf("   "); printf(" ");

      for (i=0; i<Length; i++) {
         printf("%c", isprint(Current[i]) ? Current[i] : '.');
      }
      printf("\n");
   }
   else
      printf("\n");

   return;
}

HANDLE
OpenDevice(
    IN CONST GUID * InterfaceGuid,
    IN ULONG        FileFlagOptions
    )
{
    SP_DEVICE_INTERFACE_DATA DeviceInterfaceData;
    SP_DEVINFO_DATA DeviceInfoData;
    HDEVINFO hDevInfo;
    PSP_DEVICE_INTERFACE_DETAIL_DATA pDeviceInterfaceDetail = NULL;
    HANDLE hDevice;

    ULONG size;
    int count, i, index;
    BOOL status = TRUE;
    TCHAR *DeviceName = NULL;
    TCHAR *DeviceLocation = NULL;


    //
    //  Retreive the device information for all PLX devices.
    //
    hDevInfo =     SetupDiGetClassDevs(InterfaceGuid,
                                                NULL,
                                                NULL,
                                                DIGCF_DEVICEINTERFACE |
                                                DIGCF_PRESENT );
    if(INVALID_HANDLE_VALUE == hDevInfo) {
        printf("No AMCC devices are present and enabled in the system\n");
        return INVALID_HANDLE_VALUE;
    }
    //
    //  Initialize the SP_DEVICE_INTERFACE_DATA Structure.
    //
    DeviceInterfaceData.cbSize  = sizeof(SP_DEVICE_INTERFACE_DATA);

    //
    //  Determine how many devices are present.
    //
    count = 0;
    while(SetupDiEnumDeviceInterfaces(hDevInfo,
                                    NULL,
                                    InterfaceGuid,
                                    count++,  //Cycle through the available devices.
                                    &DeviceInterfaceData));

    //
    // Since the last call fails when all devices have been enumerated,
    // decrement the count to get the true device count.
    //
    count--;

    //
    //  If the count is zero then there are no devices present.
    //
    if(count == 0)
    {
        printf("No AMCC devices are present and enabled in the system.\n");
        goto End;
    }


    //
    //  Initialize the appropriate data structures in preparation for
    //  the SetupDi calls.
    //
    DeviceInterfaceData.cbSize = sizeof(SP_DEVICE_INTERFACE_DATA);
    DeviceInfoData.cbSize = sizeof(SP_DEVINFO_DATA);


    //
    //  Loop through the device list to allow user to choose
    //  a device.  If there is only one device, select it
    //  by default.
    //
    i = 0;
    while(SetupDiEnumDeviceInterfaces(hDevInfo,
                                NULL,
                                (LPGUID)InterfaceGuid,
                                i,
                                &DeviceInterfaceData))
    {

        //
        // Determine the size required for the DeviceInterfaceData
        //
        SetupDiGetDeviceInterfaceDetail(hDevInfo,
                                        &DeviceInterfaceData,
                                        NULL,
                                        0,
                                        &size,
                                        NULL);

        if(GetLastError() != ERROR_INSUFFICIENT_BUFFER)
        {
            printf("SetupDiGetDeviceInterfaceDetail failed, Error: %d", GetLastError());
            goto End;
        }
        pDeviceInterfaceDetail = (PSP_DEVICE_INTERFACE_DETAIL_DATA)malloc(size);
        if(!pDeviceInterfaceDetail)
        {
            printf("Insufficient memory.\n");
            goto End;
        }

        //
        // Initialize structure and retrieve data.
        //
        pDeviceInterfaceDetail->cbSize = sizeof(SP_DEVICE_INTERFACE_DETAIL_DATA);
        status = SetupDiGetDeviceInterfaceDetail(hDevInfo,
                                    &DeviceInterfaceData,
                                    pDeviceInterfaceDetail,
                                    size,
                                    NULL,
                                    &DeviceInfoData);

        free(pDeviceInterfaceDetail);
        pDeviceInterfaceDetail = NULL;

        if(!status)
        {
            printf("SetupDiGetDeviceInterfaceDetail failed, Error: %d", GetLastError());
            goto End;
        }

        //
        //  Get the Device Name
        //  Calls to SetupDiGetDeviceRegistryProperty require two consecutive
        //  calls, first to get required buffer size and second to get
        //  the data.
        //
        SetupDiGetDeviceRegistryProperty(hDevInfo,
                                        &DeviceInfoData,
                                        SPDRP_DEVICEDESC,
                                        NULL,
                                        (PBYTE)DeviceName,
                                        0,
                                        &size);

        if(GetLastError() != ERROR_INSUFFICIENT_BUFFER)
        {
            printf("SetupDiGetDeviceRegistryProperty failed, Error: %d", GetLastError());
            goto End;
        }

        DeviceName = (TCHAR*) malloc(size);
        if(!DeviceName)
        {
            printf("Insufficient memory.\n");
            goto End;
        }

        status = SetupDiGetDeviceRegistryProperty(hDevInfo,
                                        &DeviceInfoData,
                                        SPDRP_DEVICEDESC,
                                        NULL,
                                        (PBYTE)DeviceName,
                                        size,
                                        NULL);
        if(!status)
        {
            printf("SetupDiGetDeviceRegistryProperty failed, Error: %d", GetLastError());
            free(DeviceName);
            goto End;
        }

        //
        //  Now retrieve the Device Location.
        //
        SetupDiGetDeviceRegistryProperty(hDevInfo,
                                        &DeviceInfoData,
                                        SPDRP_LOCATION_INFORMATION,
                                        NULL,
                                        (PBYTE)DeviceLocation,
                                        0,
                                        &size);

        if(GetLastError() == ERROR_INSUFFICIENT_BUFFER)
        {
            DeviceLocation = (TCHAR*) malloc(size);

            if (DeviceLocation != NULL)
            {
                status = SetupDiGetDeviceRegistryProperty(hDevInfo,
                                                          &DeviceInfoData,
                                                          SPDRP_LOCATION_INFORMATION,
                                                          NULL,
                                                          (PBYTE)DeviceLocation,
                                                          size,
                                                          NULL);
                if(!status)
                {
                    free(DeviceLocation);
                    DeviceLocation = NULL;
                }
            }

        } else {
            DeviceLocation = NULL;
        }

        //
        // If there is more than one device print description.
        //
        if(count > 1)
        {
            printf("%d- ", i);
        }

        printf("%s\n", DeviceName);

        if(DeviceLocation)
        {
            printf("        %s\n", DeviceLocation);
        }

        free(DeviceName);
        free(DeviceLocation);

        i++; // Cycle through the available devices.
    }

    //
    //  Select device.
    //
    index = 0;
    if(count > 1)
    {
        printf("\nSelect Device: ");
        (void)scanf_s("%d", &index);
    }

    //
    //  Get information for specific device.
    //

    status = SetupDiEnumDeviceInterfaces(hDevInfo,
                                    NULL,
                                    (LPGUID)InterfaceGuid,
                                    index,
                                    &DeviceInterfaceData);

    if(!status)
    {
        printf("SetupDiEnumDeviceInterfaces failed, Error: %d", GetLastError());
        goto End;
    }
    //
    // Determine the size required for the DeviceInterfaceData
    //
    SetupDiGetDeviceInterfaceDetail(hDevInfo,
                                    &DeviceInterfaceData,
                                    NULL,
                                    0,
                                    &size,
                                    NULL);

    if(GetLastError() != ERROR_INSUFFICIENT_BUFFER)
    {
        printf("SetupDiGetDeviceInterfaceDetail failed, Error: %d", GetLastError());
        goto End;
    }
    pDeviceInterfaceDetail = (PSP_DEVICE_INTERFACE_DETAIL_DATA)malloc(size);
    if(!pDeviceInterfaceDetail)
    {
        printf("Insufficient memory.\n");
        goto End;
    }

    //
    // Initialize structure and retrieve data.
    //
    pDeviceInterfaceDetail->cbSize = sizeof(SP_DEVICE_INTERFACE_DETAIL_DATA);
    status = SetupDiGetDeviceInterfaceDetail(hDevInfo,
                                &DeviceInterfaceData,
                                pDeviceInterfaceDetail,
                                size,
                                NULL,
                                &DeviceInfoData);
    if(!status)
    {
        printf("SetupDiGetDeviceInterfaceDetail failed, Error: %d", GetLastError());
        goto End;
    }

    //
    //  Get handle to device.
    //
    hDevice = CreateFile(pDeviceInterfaceDetail->DevicePath,
                            GENERIC_READ|GENERIC_WRITE,
                            FILE_SHARE_READ | FILE_SHARE_WRITE,
                            NULL,
                            OPEN_EXISTING,
                            FileFlagOptions,
                            NULL
                            );

    if(hDevice == INVALID_HANDLE_VALUE)
    {
        printf("CreateFile failed.  Error:%d", GetLastError());
    }

    free(pDeviceInterfaceDetail);
    SetupDiDestroyDeviceInfoList(hDevInfo);

    return hDevice;

End:
    if(pDeviceInterfaceDetail) {
        free(pDeviceInterfaceDetail);
    }
    SetupDiDestroyDeviceInfoList(hDevInfo);
    return INVALID_HANDLE_VALUE;
}



