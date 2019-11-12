/*++

Copyright (c) Microsoft Corporation

Module Name: 

    util.h

Abstract

    Prototype, defines for util functions.
--*/

HANDLE
OpenDevice (
           HANDLE       hWnd,
           _In_ PSTR    szDeviceName);

VOID
WriteTextToEditControl (
                        HWND         hWndEdit, 
                        _In_ PCHAR   str);

// Generic singly linked list routines

typedef struct _LIST_NODE {
    struct _LIST_NODE *pNext;
} LIST_NODE, *PLIST_NODE;

void 
InsertTailList (
                PLIST_NODE head, 
                PLIST_NODE entry);

BOOL 
RemoveEntryList (
                PLIST_NODE head, 
                PLIST_NODE entry);

void 
InsertHeadList (
                PLIST_NODE head, 
                PLIST_NODE entry);

BOOL
IsNodeOnList (
             PLIST_NODE head, 
             PLIST_NODE entry);

DWORD
WINAPI
RemoveVirtualDriver (
                     HWND                 hWnd,
                     PVIRT_DEVICE    pVirtualDevice,
                     ULONG                BusNumber);

DWORD
WINAPI
AddVirtualDriver (
                  HWND                hWnd,
                  PVIRT_DEVICE    pVirtualDevice,
                  ULONG               BusNumber);
DWORD
FillDeviceList (
                HWND            hWnd,
                LPGUID          guid,
                PDEVICE_DATA    pDeviceData);


DWORD
SendRequest (
             _In_ DWORD Ioctl,
             _In_opt_ PVOID InputBuffer,
             _In_opt_ ULONG InputBufferSize,
             _Out_opt_ PVOID OutputBuffer,
             _In_opt_ ULONG OutputBufferSize,
             _Out_ LPDWORD bytesReturned);


