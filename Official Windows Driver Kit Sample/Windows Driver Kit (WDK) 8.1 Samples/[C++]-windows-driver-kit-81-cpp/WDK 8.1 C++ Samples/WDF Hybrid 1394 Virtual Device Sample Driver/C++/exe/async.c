/*++

Copyright (c) Microsoft Corporation

Module Name:

    async.c

Abstract

    1394 async wrappers

--*/

#define _ASYNC_C
#include "pch.h"
#undef _ASYNC_C

INT_PTR CALLBACK
AllocateAddressRangeDlgProc(
                            HWND        hDlg,
                            UINT        uMsg,
                            WPARAM      wParam,
                            LPARAM      lParam)
{
    static PALLOCATE_ADDRESS_RANGE      pAllocateAddressRange;
    static CHAR                         tmpBuff[STRING_SIZE];

    switch (uMsg) 
    {

    case WM_INITDIALOG:

        pAllocateAddressRange = (PALLOCATE_ADDRESS_RANGE)lParam;

        _ultoa_s(pAllocateAddressRange->nLength, tmpBuff, (STRING_SIZE * sizeof(CHAR)), 16);
        SetDlgItemText(hDlg, IDC_ASYNC_ALLOC_LENGTH, tmpBuff);

        _ultoa_s(pAllocateAddressRange->MaxSegmentSize, tmpBuff, (STRING_SIZE * sizeof(CHAR)), 16);
        SetDlgItemText(hDlg, IDC_ASYNC_ALLOC_MAX_SEGMENT_SIZE, tmpBuff);

        _ultoa_s(pAllocateAddressRange->Required1394Offset.Off_High, tmpBuff, (STRING_SIZE * sizeof(CHAR)), 16);
        SetDlgItemText(hDlg, IDC_ASYNC_ALLOC_OFFSET_HIGH, tmpBuff);

        _ultoa_s(pAllocateAddressRange->Required1394Offset.Off_Low, tmpBuff, (STRING_SIZE * sizeof(CHAR)), 16);
        SetDlgItemText(hDlg, IDC_ASYNC_ALLOC_OFFSET_LOW, tmpBuff);

        CheckRadioButton( 
            hDlg,
            IDC_ASYNC_ALLOC_USE_MDL,
            IDC_ASYNC_ALLOC_USE_NONE,
            pAllocateAddressRange->fulAllocateFlags + (IDC_ASYNC_ALLOC_USE_MDL-1));

        if (pAllocateAddressRange->fulFlags & BIG_ENDIAN_ADDRESS_RANGE)
            CheckDlgButton( hDlg, IDC_ASYNC_ALLOC_USE_BIG_ENDIAN, BST_CHECKED);

        if (pAllocateAddressRange->fulAccessType & ACCESS_FLAGS_TYPE_READ)
            CheckDlgButton( hDlg, IDC_ASYNC_ALLOC_ACCESS_READ, BST_CHECKED);

        if (pAllocateAddressRange->fulAccessType & ACCESS_FLAGS_TYPE_WRITE)
            CheckDlgButton( hDlg, IDC_ASYNC_ALLOC_ACCESS_WRITE, BST_CHECKED);

        if (pAllocateAddressRange->fulAccessType & ACCESS_FLAGS_TYPE_LOCK)
            CheckDlgButton( hDlg, IDC_ASYNC_ALLOC_ACCESS_LOCK, BST_CHECKED);

        if (pAllocateAddressRange->fulAccessType & ACCESS_FLAGS_TYPE_BROADCAST)
            CheckDlgButton( hDlg, IDC_ASYNC_ALLOC_ACCESS_BROADCAST, BST_CHECKED);

        if (pAllocateAddressRange->fulNotificationOptions & NOTIFY_FLAGS_AFTER_READ)
            CheckDlgButton( hDlg, IDC_ASYNC_ALLOC_NOTIFY_READ, BST_CHECKED);

        if (pAllocateAddressRange->fulNotificationOptions & NOTIFY_FLAGS_AFTER_WRITE)
            CheckDlgButton( hDlg, IDC_ASYNC_ALLOC_NOTIFY_WRITE, BST_CHECKED);

        if (pAllocateAddressRange->fulNotificationOptions & NOTIFY_FLAGS_AFTER_LOCK)
            CheckDlgButton( hDlg, IDC_ASYNC_ALLOC_NOTIFY_LOCK, BST_CHECKED);

        return(TRUE); // WM_INITDIALOG

    case WM_COMMAND:

        switch (LOWORD(wParam)) 
        {

        case IDOK:

            GetDlgItemText(hDlg, IDC_ASYNC_ALLOC_LENGTH, tmpBuff, STRING_SIZE);
            pAllocateAddressRange->nLength = strtoul(tmpBuff, NULL, 16);

            GetDlgItemText(hDlg, IDC_ASYNC_ALLOC_MAX_SEGMENT_SIZE, tmpBuff, STRING_SIZE);
            pAllocateAddressRange->MaxSegmentSize = strtoul(tmpBuff, NULL, 16);

            GetDlgItemText(hDlg, IDC_ASYNC_ALLOC_OFFSET_HIGH, tmpBuff, STRING_SIZE);
            pAllocateAddressRange->Required1394Offset.Off_High = (USHORT)strtoul(tmpBuff, NULL, 16);

            GetDlgItemText(hDlg, IDC_ASYNC_ALLOC_OFFSET_LOW, tmpBuff, STRING_SIZE);
            pAllocateAddressRange->Required1394Offset.Off_Low = strtoul(tmpBuff, NULL, 16);

            // fulAllocateFlags
            if (IsDlgButtonChecked(hDlg, IDC_ASYNC_ALLOC_USE_MDL))
                pAllocateAddressRange->fulAllocateFlags = ASYNC_ALLOC_USE_MDL;

            if (IsDlgButtonChecked(hDlg, IDC_ASYNC_ALLOC_USE_FIFO))
                pAllocateAddressRange->fulAllocateFlags = ASYNC_ALLOC_USE_FIFO;

            if (IsDlgButtonChecked(hDlg, IDC_ASYNC_ALLOC_USE_NONE))
                pAllocateAddressRange->fulAllocateFlags = ASYNC_ALLOC_USE_NONE;                                                          

            // fulFlags
            if (IsDlgButtonChecked(hDlg, IDC_ASYNC_ALLOC_USE_BIG_ENDIAN))
                pAllocateAddressRange->fulFlags = BIG_ENDIAN_ADDRESS_RANGE;

            // fulAccessType
            if (IsDlgButtonChecked(hDlg, IDC_ASYNC_ALLOC_ACCESS_READ))
                pAllocateAddressRange->fulAccessType |= ACCESS_FLAGS_TYPE_READ;

            if (IsDlgButtonChecked(hDlg, IDC_ASYNC_ALLOC_ACCESS_WRITE))
                pAllocateAddressRange->fulAccessType |= ACCESS_FLAGS_TYPE_WRITE;

            if (IsDlgButtonChecked(hDlg, IDC_ASYNC_ALLOC_ACCESS_LOCK))
                pAllocateAddressRange->fulAccessType |= ACCESS_FLAGS_TYPE_LOCK;

            if (IsDlgButtonChecked(hDlg, IDC_ASYNC_ALLOC_ACCESS_BROADCAST))
                pAllocateAddressRange->fulAccessType |= ACCESS_FLAGS_TYPE_BROADCAST;                                                             

            // fulNotifcationOptions
            pAllocateAddressRange->fulNotificationOptions = NOTIFY_FLAGS_NEVER;
            if (IsDlgButtonChecked(hDlg, IDC_ASYNC_ALLOC_NOTIFY_READ))
                pAllocateAddressRange->fulNotificationOptions |= NOTIFY_FLAGS_AFTER_READ;

            if (IsDlgButtonChecked(hDlg, IDC_ASYNC_ALLOC_NOTIFY_WRITE))
                pAllocateAddressRange->fulNotificationOptions |= NOTIFY_FLAGS_AFTER_WRITE;

            if (IsDlgButtonChecked(hDlg, IDC_ASYNC_ALLOC_NOTIFY_LOCK))
                pAllocateAddressRange->fulNotificationOptions |= NOTIFY_FLAGS_AFTER_LOCK;

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
} // AllocateAddressRangeDlgProc

void
w1394_AllocateAddressRange(
                           HWND         hWnd,
                           _In_ PSTR    szDeviceName)
{
    PALLOCATE_ADDRESS_RANGE  allocateAddressRange;
    DWORD                       dwRet, bytesReturned;
    ULONG ulBufferSize = sizeof (ALLOCATE_ADDRESS_RANGE) + 512;

    UNREFERENCED_PARAMETER(szDeviceName);

    TRACE(TL_TRACE, (hWnd, "Enter w1394_AllocateAddressRange\r\n"));

    allocateAddressRange = \
        (PALLOCATE_ADDRESS_RANGE) LocalAlloc (LPTR, ulBufferSize);
    if (NULL == allocateAddressRange)
    {
        TRACE(TL_WARNING, (hWnd, "Failed to allocate memory\r\n\r\n"));
        return;
    }

    allocateAddressRange->fulAllocateFlags = ASYNC_ALLOC_USE_MDL;
    allocateAddressRange->fulFlags = 0;
    allocateAddressRange->nLength = 512;
    allocateAddressRange->MaxSegmentSize = 0;
    allocateAddressRange->fulAccessType = ACCESS_FLAGS_TYPE_READ | 
        ACCESS_FLAGS_TYPE_WRITE | ACCESS_FLAGS_TYPE_LOCK | 
        ACCESS_FLAGS_TYPE_BROADCAST;
    allocateAddressRange->fulNotificationOptions = NOTIFY_FLAGS_NEVER;
    allocateAddressRange->Required1394Offset.Off_High = 0;
    allocateAddressRange->Required1394Offset.Off_Low = 0;


    if (DialogBoxParam ( 
        (HINSTANCE) GetWindowLongPtr(hWnd, GWLP_HINSTANCE),
        "AllocateAddressRange",
        hWnd,
        AllocateAddressRangeDlgProc,
        (LPARAM) allocateAddressRange)) 
    {

        dwRet = SendRequest (
            IOCTL_ALLOCATE_ADDRESS_RANGE,
            allocateAddressRange,
            ulBufferSize,
            allocateAddressRange,
            ulBufferSize,
            &bytesReturned);

        if (ERROR_SUCCESS != dwRet)
        {
            TRACE(TL_WARNING, (hWnd, "Failed SendRequest 0x%x\r\n", dwRet));
        }
        else
        {
            TRACE(
                TL_TRACE, 
                (hWnd, "Copy the hAddressRange to free the resource later\r\n"));
            TRACE(
                TL_TRACE, 
                (hWnd, "hAddressRange = %p\r\n", 
                allocateAddressRange->hAddressRange));
            TRACE(
                TL_TRACE, 
                (hWnd, "Required1394Offset.Off_High = 0x%x\r\n", 
                allocateAddressRange->Required1394Offset.Off_High));
            TRACE(
                TL_TRACE, 
                (hWnd, "Required1394Offset.Off_low = 0x%x\r\n", 
                allocateAddressRange->Required1394Offset.Off_Low));
        }

        LocalFree (allocateAddressRange);
    }

    TRACE(TL_TRACE, (hWnd, "Exit w1394_AllocateAddressRange\r\n\r\n"));
    return;
} // w1394_AllocateAddressRange


INT_PTR CALLBACK
FreeAddressRangeDlgProc(
    HWND        hDlg,
    UINT        uMsg,
    WPARAM      wParam,
    LPARAM      lParam)
{
    static PHANDLE      phAddressRange;
    static CHAR         tmpBuff[STRING_SIZE];

    switch (uMsg)
    {

    case WM_INITDIALOG:

        phAddressRange = (PHANDLE)lParam;

        // put offset low and high and buffer length in edit controls

        StringCbPrintf (tmpBuff, (STRING_SIZE * sizeof(tmpBuff[0])) , "%.1p", *phAddressRange);
        SetDlgItemText(hDlg, IDC_ASYNC_FREE_ADDRESS_HANDLE, tmpBuff);

        return(TRUE); // WM_INITDIALOG

    case WM_COMMAND:

        switch (LOWORD(wParam)) {

    case IDOK:

        GetDlgItemText(hDlg, IDC_ASYNC_FREE_ADDRESS_HANDLE, tmpBuff, STRING_SIZE);
        if (!sscanf_s (tmpBuff, "%p", phAddressRange))
        {
            // failed to get the handle, just return here
            EndDialog(hDlg, TRUE);
            return FALSE;
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
} // FreeAddressRangeDlgProc

void
w1394_FreeAddressRange (
                        HWND         hWnd,
                        _In_ PSTR    szDeviceName)
{
    HANDLE      hAddress;
    DWORD       dwRet, bytesReturned;

    UNREFERENCED_PARAMETER(szDeviceName);

    TRACE(TL_TRACE, (hWnd, "Enter w1394_FreeAddressRange\r\n"));

    hAddress = NULL;

    if (DialogBoxParam( (HINSTANCE) GetWindowLongPtr(hWnd, GWLP_HINSTANCE),
        "FreeAddressRange",
        hWnd,
        FreeAddressRangeDlgProc,
        (LPARAM) &hAddress))
    {

        dwRet = SendRequest (
            IOCTL_FREE_ADDRESS_RANGE,
            &hAddress,
            sizeof (HANDLE),
            NULL,
            0,
            &bytesReturned);
        if (ERROR_SUCCESS != dwRet)
        {
            TRACE(TL_WARNING, (hWnd, "Failed SendRequest 0x%x\r\n", dwRet));
        }

    }

    TRACE(TL_TRACE, (hWnd, "Exit w1394_FreeAddressRange\r\n\r\n"));
    return;
} // w1394_FreeAddressRange

INT_PTR CALLBACK
AsyncReadDlgProc(
                 HWND        hDlg,
                 UINT        uMsg,
                 WPARAM      wParam,
                 LPARAM      lParam)
{
    static PASYNC_READ      pAsyncRead;
    static CHAR             tmpBuff[STRING_SIZE];

    switch (uMsg) 
    {
    
        case WM_INITDIALOG:

            pAsyncRead = (PASYNC_READ)lParam;

            _ultoa_s(
                pAsyncRead->DestinationAddress.IA_Destination_ID.NA_Bus_Number,
                tmpBuff,
                (STRING_SIZE * sizeof(CHAR)), 
                16);
            SetDlgItemText(hDlg, IDC_ASYNC_READ_BUS_NUMBER, tmpBuff);

            _ultoa_s(
                pAsyncRead->DestinationAddress.IA_Destination_ID.NA_Node_Number, 
                tmpBuff, 
                (STRING_SIZE * sizeof(CHAR)), 
                16);
            SetDlgItemText(hDlg, IDC_ASYNC_READ_NODE_NUMBER, tmpBuff);

            _ultoa_s(
                pAsyncRead->DestinationAddress.IA_Destination_Offset.Off_High,
                tmpBuff, 
                (STRING_SIZE * sizeof(CHAR)), 
                16);
            SetDlgItemText(hDlg, IDC_ASYNC_READ_OFFSET_HIGH, tmpBuff);

            _ultoa_s(
                pAsyncRead->DestinationAddress.IA_Destination_Offset.Off_Low, 
                tmpBuff, 
                (STRING_SIZE * sizeof(CHAR)), 
                16);
            SetDlgItemText(hDlg, IDC_ASYNC_READ_OFFSET_LOW, tmpBuff);

            _ultoa_s(
                pAsyncRead->nNumberOfBytesToRead, 
                tmpBuff, 
                (STRING_SIZE * sizeof(CHAR)), 
                16);
            SetDlgItemText(hDlg, IDC_ASYNC_READ_BYTES_TO_READ, tmpBuff);

            _ultoa_s(
                pAsyncRead->nBlockSize, 
                tmpBuff, 
                (STRING_SIZE * sizeof(CHAR)), 
                16);
            SetDlgItemText(hDlg, IDC_ASYNC_READ_BLOCK_SIZE, tmpBuff);

            if (pAsyncRead->fulFlags & ASYNC_FLAGS_NONINCREMENTING)
                CheckDlgButton(
                hDlg, 
                IDC_ASYNC_READ_FLAG_NON_INCREMENT, 
                BST_CHECKED);

            if (pAsyncRead->bGetGeneration)
                CheckDlgButton(hDlg, IDC_ASYNC_READ_GET_GENERATION, BST_CHECKED);

            _ultoa_s(
                pAsyncRead->ulGeneration, 
                tmpBuff, 
                (STRING_SIZE * sizeof(CHAR)), 
                16);
            SetDlgItemText(hDlg, IDC_ASYNC_READ_GENERATION_COUNT, tmpBuff);

            return(TRUE); // WM_INITDIALOG

        case WM_COMMAND:

            switch (LOWORD(wParam)) 
            {

        case IDOK:

            GetDlgItemText(hDlg, IDC_ASYNC_READ_BUS_NUMBER, tmpBuff, STRING_SIZE);
            pAsyncRead->DestinationAddress.IA_Destination_ID.NA_Bus_Number = \
                (USHORT)strtoul(tmpBuff, NULL, 16);

            GetDlgItemText(hDlg, IDC_ASYNC_READ_NODE_NUMBER, tmpBuff, STRING_SIZE);
            pAsyncRead->DestinationAddress.IA_Destination_ID.NA_Node_Number = \
                (USHORT)strtoul(tmpBuff, NULL, 16);

            GetDlgItemText(
                hDlg, 
                IDC_ASYNC_READ_OFFSET_HIGH, 
                tmpBuff, 
                STRING_SIZE);
            pAsyncRead->DestinationAddress.IA_Destination_Offset.Off_High = \
                (USHORT)strtoul(tmpBuff, NULL, 16);

            GetDlgItemText(
                hDlg, 
                IDC_ASYNC_READ_OFFSET_LOW, 
                tmpBuff, 
                STRING_SIZE);
            pAsyncRead->DestinationAddress.IA_Destination_Offset.Off_Low = \
                strtoul(tmpBuff, NULL, 16);

            GetDlgItemText(
                hDlg, 
                IDC_ASYNC_READ_BYTES_TO_READ, 
                tmpBuff, 
                STRING_SIZE);
            pAsyncRead->nNumberOfBytesToRead = strtoul(tmpBuff, NULL, 16);

            GetDlgItemText(hDlg, IDC_ASYNC_READ_BLOCK_SIZE, tmpBuff, STRING_SIZE);
            pAsyncRead->nBlockSize = strtoul(tmpBuff, NULL, 16);

            pAsyncRead->fulFlags = 0;
            if (IsDlgButtonChecked(hDlg, IDC_ASYNC_READ_FLAG_NON_INCREMENT))
                pAsyncRead->fulFlags |= ASYNC_FLAGS_NONINCREMENTING;

            if (IsDlgButtonChecked(hDlg, IDC_ASYNC_READ_GET_GENERATION))
                pAsyncRead->bGetGeneration = TRUE;
            else
                pAsyncRead->bGetGeneration = FALSE;

            GetDlgItemText(
                hDlg, 
                IDC_ASYNC_READ_GENERATION_COUNT, 
                tmpBuff, 
                STRING_SIZE);
            pAsyncRead->ulGeneration = strtoul(tmpBuff, NULL, 16);

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
} // AsyncReadDlgProc

void
w1394_AsyncRead (
                 HWND         hWnd,
                 _In_ PSTR    szDeviceName)
{
    PASYNC_READ      asyncRead;
    DWORD           dwRet, bytesTransferred;
    ULONG ulBufferSize = sizeof (ASYNC_READ) + sizeof (ULONG);

    UNREFERENCED_PARAMETER(szDeviceName);

    TRACE(TL_TRACE, (hWnd, "Enter w1394_AsyncRead\r\n"));

    asyncRead =  (PASYNC_READ) LocalAlloc (LPTR, ulBufferSize);
    if (NULL == asyncRead)
    {
        TRACE(TL_WARNING, (hWnd, "Failed to allocate asyncRead\r\n\r\n"));
        return;
    }

    asyncRead->bGetGeneration = TRUE;
    asyncRead->DestinationAddress.IA_Destination_ID.NA_Bus_Number = 0x3FF;
    asyncRead->DestinationAddress.IA_Destination_ID.NA_Node_Number = 0;
    asyncRead->DestinationAddress.IA_Destination_Offset.Off_High = 0xFFFF;
    asyncRead->DestinationAddress.IA_Destination_Offset.Off_Low = 0xF0000400;
    asyncRead->nNumberOfBytesToRead = sizeof(ULONG);
    asyncRead->nBlockSize = 0;
    asyncRead->fulFlags = 0;
    asyncRead->ulGeneration = 0;


    if (DialogBoxParam( 
        (HINSTANCE) GetWindowLongPtr(hWnd, GWLP_HINSTANCE),
        "AsyncRead",
        hWnd,
        AsyncReadDlgProc,
        (LPARAM)asyncRead)) 
    {

        dwRet = SendRequest (
            IOCTL_ASYNC_READ,
            asyncRead,
            ulBufferSize,
            asyncRead,
            ulBufferSize,
            &bytesTransferred);
        if (ERROR_SUCCESS != dwRet)
        {
            TRACE(TL_WARNING, (hWnd, "Failed SendRequest 0x%x\r\n", dwRet));
        }
        else if (asyncRead->nNumberOfBytesToRead != sizeof(ULONG))
        {
            TRACE (
                TL_WARNING, 
                (hWnd, 
                "Request success, but the number of bytes in "
                "the read buffer is too small: %d\n",
                asyncRead->nNumberOfBytesToRead));
        }
        else
        {
            ULONG i;    

            for (i=0; i < (asyncRead->nNumberOfBytesToRead/sizeof(ULONG)); i++) 
            {

                PULONG ulTemp;

                ulTemp = (PULONG) asyncRead->Data[i * sizeof (ULONG)];
                if (NULL == ulTemp)
                {
                    TRACE(
                        TL_TRACE, 
                        (hWnd, 
                        "Null read attempted\r\n"));
                }
                else
                {
                TRACE(
                    TL_TRACE, 
                    (hWnd, 
                    "Quadlet[0x%x] = 0x%x\r\n", 
                    i, 
                    (ULONG)*ulTemp));
                }
            }
        }
    }

    LocalFree (asyncRead);
    TRACE(TL_TRACE, (hWnd, "Exit w1394_AsyncRead\r\n\r\n"));
    return;
} // w1394_AsyncRead

INT_PTR CALLBACK
AsyncWriteDlgProc(
                  HWND        hDlg,
                  UINT        uMsg,
                  WPARAM      wParam,
                  LPARAM      lParam)
{
    static PASYNC_WRITE     pAsyncWrite;
    static CHAR             tmpBuff[STRING_SIZE];

    switch (uMsg)
    {

        case WM_INITDIALOG:

            pAsyncWrite = (PASYNC_WRITE)lParam;

            _ultoa_s(
                pAsyncWrite->DestinationAddress.IA_Destination_ID.NA_Bus_Number, 
                tmpBuff, 
                (STRING_SIZE * sizeof(CHAR)), 
                16);
            SetDlgItemText(hDlg, IDC_ASYNC_WRITE_BUS_NUMBER, tmpBuff);

            _ultoa_s(
                pAsyncWrite->DestinationAddress.IA_Destination_ID.NA_Node_Number, 
                tmpBuff, 
                (STRING_SIZE * sizeof(CHAR)), 
                16);
            SetDlgItemText(hDlg, IDC_ASYNC_WRITE_NODE_NUMBER, tmpBuff);

            _ultoa_s(
                pAsyncWrite->DestinationAddress.IA_Destination_Offset.Off_High, 
                tmpBuff, 
                (STRING_SIZE * sizeof(CHAR)),
                16);
            SetDlgItemText(hDlg, IDC_ASYNC_WRITE_OFFSET_HIGH, tmpBuff);

            _ultoa_s(
                pAsyncWrite->DestinationAddress.IA_Destination_Offset.Off_Low, 
                tmpBuff, 
                (STRING_SIZE * sizeof(CHAR)), 
                16);
            SetDlgItemText(hDlg, IDC_ASYNC_WRITE_OFFSET_LOW, tmpBuff);

            _ultoa_s(
                pAsyncWrite->nNumberOfBytesToWrite, 
                tmpBuff,
                (STRING_SIZE * sizeof(CHAR)),
                16);
            SetDlgItemText(hDlg, IDC_ASYNC_WRITE_BYTES_TO_WRITE, tmpBuff);

            _ultoa_s(
                pAsyncWrite->nBlockSize, 
                tmpBuff, 
                (STRING_SIZE * sizeof(CHAR)),
                16);
            SetDlgItemText(hDlg, IDC_ASYNC_WRITE_BLOCK_SIZE, tmpBuff);

            if (pAsyncWrite->fulFlags & ASYNC_FLAGS_NONINCREMENTING)
                CheckDlgButton( hDlg, IDC_ASYNC_WRITE_FLAG_NON_INCRMENT, BST_CHECKED);

            if (pAsyncWrite->fulFlags & ASYNC_FLAGS_NO_STATUS)
                CheckDlgButton( hDlg, IDC_ASYNC_WRITE_FLAG_NO_STATUS, BST_CHECKED);

            if (pAsyncWrite->bGetGeneration)
                CheckDlgButton( hDlg, IDC_ASYNC_WRITE_GET_GENERATION, BST_CHECKED);

            _ultoa_s(
                pAsyncWrite->ulGeneration, 
                tmpBuff, 
                (STRING_SIZE * sizeof(CHAR)), 
                16);
            SetDlgItemText(hDlg, IDC_ASYNC_WRITE_GENERATION_COUNT, tmpBuff);

            return(TRUE); // WM_INITDIALOG

        case WM_COMMAND:

            switch (LOWORD(wParam)) {

                case IDOK:

                    GetDlgItemText(hDlg, IDC_ASYNC_WRITE_BUS_NUMBER, tmpBuff, STRING_SIZE);
                    pAsyncWrite->DestinationAddress.IA_Destination_ID.NA_Bus_Number = (USHORT)strtoul(tmpBuff, NULL, 16);

                    GetDlgItemText(hDlg, IDC_ASYNC_WRITE_NODE_NUMBER, tmpBuff, STRING_SIZE);
                    pAsyncWrite->DestinationAddress.IA_Destination_ID.NA_Node_Number = (USHORT)strtoul(tmpBuff, NULL, 16);

                    GetDlgItemText(hDlg, IDC_ASYNC_WRITE_OFFSET_HIGH, tmpBuff, STRING_SIZE);
                    pAsyncWrite->DestinationAddress.IA_Destination_Offset.Off_High = (USHORT)strtoul(tmpBuff, NULL, 16);

                    GetDlgItemText(hDlg, IDC_ASYNC_WRITE_OFFSET_LOW, tmpBuff, STRING_SIZE);
                    pAsyncWrite->DestinationAddress.IA_Destination_Offset.Off_Low = strtoul(tmpBuff, NULL, 16);

                    GetDlgItemText(hDlg, IDC_ASYNC_WRITE_BYTES_TO_WRITE, tmpBuff, STRING_SIZE);
                    pAsyncWrite->nNumberOfBytesToWrite = strtoul(tmpBuff, NULL, 16);

                    GetDlgItemText(hDlg, IDC_ASYNC_WRITE_BLOCK_SIZE, tmpBuff, STRING_SIZE);
                    pAsyncWrite->nBlockSize = strtoul(tmpBuff, NULL, 16);

                    pAsyncWrite->fulFlags = 0;
                    if (IsDlgButtonChecked(hDlg, IDC_ASYNC_WRITE_FLAG_NON_INCRMENT))
                        pAsyncWrite->fulFlags |= ASYNC_FLAGS_NONINCREMENTING;

                    if (IsDlgButtonChecked(hDlg, IDC_ASYNC_WRITE_FLAG_NO_STATUS))
                        pAsyncWrite->fulFlags |= ASYNC_FLAGS_NO_STATUS;


                    if (IsDlgButtonChecked(hDlg, IDC_ASYNC_WRITE_GET_GENERATION))
                        pAsyncWrite->bGetGeneration = TRUE;
                    else
                        pAsyncWrite->bGetGeneration = FALSE;

                    GetDlgItemText(hDlg, IDC_ASYNC_WRITE_GENERATION_COUNT, tmpBuff, STRING_SIZE);
                    pAsyncWrite->ulGeneration = strtoul(tmpBuff, NULL, 16);

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
} // AsyncWriteDlgProc

void
w1394_AsyncWrite(
                 HWND         hWnd,
                 _In_ PSTR    szDeviceName)
{
    PASYNC_WRITE     asyncWrite;
    DWORD           dwRet, bytesReturned;
    ULONG         ulBufferSize = sizeof (ASYNC_WRITE) + sizeof (ULONG);

    UNREFERENCED_PARAMETER(szDeviceName);

    TRACE(TL_TRACE, (hWnd, "Enter w1394_AsyncWrite\r\n"));

    asyncWrite = (PASYNC_WRITE) LocalAlloc (LPTR, ulBufferSize);
    if (NULL == asyncWrite)
    {
        TRACE(TL_WARNING, (hWnd, "Failed to allocate asyncWrite\r\n\r\n"));
        return;
    }

    asyncWrite->bGetGeneration = TRUE;
    asyncWrite->DestinationAddress.IA_Destination_ID.NA_Bus_Number = 0x3FF;
    asyncWrite->DestinationAddress.IA_Destination_ID.NA_Node_Number = 0;
    asyncWrite->DestinationAddress.IA_Destination_Offset.Off_High = 0;
    asyncWrite->DestinationAddress.IA_Destination_Offset.Off_Low = 0;
    asyncWrite->nNumberOfBytesToWrite = sizeof(ULONG);
    asyncWrite->nBlockSize = 0;
    asyncWrite->fulFlags = 0;
    asyncWrite->ulGeneration = 0;

    if (DialogBoxParam( (HINSTANCE) GetWindowLongPtr(hWnd, GWLP_HINSTANCE),
                        "AsyncWrite",
                        hWnd,
                        AsyncWriteDlgProc,
                        (LPARAM)asyncWrite)) 
    {

        dwRet = SendRequest (
                                IOCTL_ASYNC_WRITE,
                                asyncWrite,
                                ulBufferSize,
                                NULL,
                                0,
                                &bytesReturned);
        if (ERROR_SUCCESS != dwRet)
        {
            TRACE(TL_WARNING, (hWnd, "Failed SendRequest 0x%x\r\n", dwRet));
        }
    }

    LocalFree (asyncWrite);
    TRACE(TL_TRACE, (hWnd, "Exit w1394_AsyncWrite\r\n\r\n"));
    return;
} // w1394_AsyncWrite

INT_PTR CALLBACK
AsyncLockDlgProc(
                 HWND        hDlg,
                 UINT        uMsg,
                 WPARAM      wParam,
                 LPARAM      lParam)
{
    static PASYNC_LOCK      pAsyncLock;
    static CHAR             tmpBuff[STRING_SIZE];

    switch (uMsg)
    {

        case WM_INITDIALOG:

            pAsyncLock = (PASYNC_LOCK)lParam;

            _ultoa_s(
                pAsyncLock->DestinationAddress.IA_Destination_ID.NA_Bus_Number, 
                tmpBuff, 
                (STRING_SIZE * sizeof(CHAR)), 
                16);
            SetDlgItemText(hDlg, IDC_ASYNC_LOCK_BUS_NUMBER, tmpBuff);

            _ultoa_s(
                pAsyncLock->DestinationAddress.IA_Destination_ID.NA_Node_Number, 
                tmpBuff, 
                (STRING_SIZE * sizeof(CHAR)),
                16);
            SetDlgItemText(hDlg, IDC_ASYNC_LOCK_NODE_NUMBER, tmpBuff);

            _ultoa_s(
                pAsyncLock->DestinationAddress.IA_Destination_Offset.Off_High, 
                tmpBuff, 
                (STRING_SIZE * sizeof(CHAR)),
                16);
            SetDlgItemText(hDlg, IDC_ASYNC_LOCK_OFFSET_HIGH, tmpBuff);

            _ultoa_s(
                pAsyncLock->DestinationAddress.IA_Destination_Offset.Off_Low, 
                tmpBuff, 
                (STRING_SIZE * sizeof(CHAR)),
                16);
            SetDlgItemText(hDlg, IDC_ASYNC_LOCK_OFFSET_LOW, tmpBuff);

            CheckRadioButton( hDlg,
                              IDC_ASYNC_LOCK_32BIT,
                              IDC_ASYNC_LOCK_64BIT,
                              IDC_ASYNC_LOCK_32BIT
                              );

            _ultoa_s(
                pAsyncLock->Arguments[0], 
                tmpBuff, 
                (STRING_SIZE * sizeof(CHAR)), 
                16);
            SetDlgItemText(hDlg, IDC_ASYNC_LOCK_ARGUMENT1, tmpBuff);

            _ultoa_s(
                pAsyncLock->Arguments[1], 
                tmpBuff, 
                (STRING_SIZE * sizeof(CHAR)), 
                16);
            SetDlgItemText(hDlg, IDC_ASYNC_LOCK_ARGUMENT2, tmpBuff);

            _ultoa_s(
                pAsyncLock->DataValues[0], 
                tmpBuff, 
                (STRING_SIZE * sizeof(CHAR)), 
                16);
            SetDlgItemText(hDlg, IDC_ASYNC_LOCK_DATA1, tmpBuff);

            _ultoa_s(
                pAsyncLock->DataValues[1], 
                tmpBuff, 
                (STRING_SIZE * sizeof(CHAR)), 
                16);
            SetDlgItemText(hDlg, IDC_ASYNC_LOCK_DATA2, tmpBuff);

            if (pAsyncLock->bGetGeneration)
                CheckDlgButton( hDlg, IDC_ASYNC_LOCK_GET_GENERATION, BST_CHECKED);

            _ultoa_s(
                pAsyncLock->ulGeneration, 
                tmpBuff, 
                (STRING_SIZE * sizeof(CHAR)), 
                16);
            SetDlgItemText(hDlg, IDC_ASYNC_LOCK_GENERATION_COUNT, tmpBuff);

            CheckRadioButton( hDlg,
                              IDC_ASYNC_LOCK_MASK_SWAP,
                              IDC_ASYNC_LOCK_WRAP_ADD,
                              pAsyncLock->fulTransactionType + (IDC_ASYNC_LOCK_MASK_SWAP-1)
                              );

            return(TRUE); // WM_INITDIALOG

        case WM_COMMAND:

            switch (LOWORD(wParam))
            {

               case IDOK:

                    GetDlgItemText(hDlg, IDC_ASYNC_LOCK_BUS_NUMBER, tmpBuff, STRING_SIZE);
                    pAsyncLock->DestinationAddress.IA_Destination_ID.NA_Bus_Number = (USHORT)strtoul(tmpBuff, NULL, 16);

                    GetDlgItemText(hDlg, IDC_ASYNC_LOCK_NODE_NUMBER, tmpBuff, STRING_SIZE);
                    pAsyncLock->DestinationAddress.IA_Destination_ID.NA_Node_Number = (USHORT)strtoul(tmpBuff, NULL, 16);

                    GetDlgItemText(hDlg, IDC_ASYNC_LOCK_OFFSET_HIGH, tmpBuff, STRING_SIZE);
                    pAsyncLock->DestinationAddress.IA_Destination_Offset.Off_High = (USHORT)strtoul(tmpBuff, NULL, 16);

                    GetDlgItemText(hDlg, IDC_ASYNC_LOCK_OFFSET_LOW, tmpBuff, STRING_SIZE);
                    pAsyncLock->DestinationAddress.IA_Destination_Offset.Off_Low = strtoul(tmpBuff, NULL, 16);

                    pAsyncLock->fulTransactionType = 0;
                    if (IsDlgButtonChecked(hDlg, IDC_ASYNC_LOCK_MASK_SWAP))
                        pAsyncLock->fulTransactionType = LOCK_TRANSACTION_MASK_SWAP;

                    if (IsDlgButtonChecked(hDlg, IDC_ASYNC_LOCK_COMPARE_SWAP))
                        pAsyncLock->fulTransactionType = LOCK_TRANSACTION_COMPARE_SWAP;

                    if (IsDlgButtonChecked(hDlg, IDC_ASYNC_LOCK_LITTLE_ADD))
                        pAsyncLock->fulTransactionType = LOCK_TRANSACTION_LITTLE_ADD;

                    if (IsDlgButtonChecked(hDlg, IDC_ASYNC_LOCK_FETCH_ADD))
                        pAsyncLock->fulTransactionType = LOCK_TRANSACTION_FETCH_ADD;

                    if (IsDlgButtonChecked(hDlg, IDC_ASYNC_LOCK_BOUNDED_ADD))
                        pAsyncLock->fulTransactionType = LOCK_TRANSACTION_BOUNDED_ADD;

                    if (IsDlgButtonChecked(hDlg, IDC_ASYNC_LOCK_WRAP_ADD))
                        pAsyncLock->fulTransactionType = LOCK_TRANSACTION_WRAP_ADD;

                    if (IsDlgButtonChecked(hDlg, IDC_ASYNC_LOCK_32BIT)) {

                        if ((pAsyncLock->fulTransactionType == LOCK_TRANSACTION_FETCH_ADD) ||
                            (pAsyncLock->fulTransactionType == LOCK_TRANSACTION_LITTLE_ADD))
                        {

                            pAsyncLock->nNumberOfArgBytes = 0;
                        }
                        else
                            pAsyncLock->nNumberOfArgBytes = sizeof(ULONG);

                        pAsyncLock->nNumberOfDataBytes = sizeof(ULONG);
                    }

                    if (IsDlgButtonChecked(hDlg, IDC_ASYNC_LOCK_64BIT)) 
                    {

                        if ((pAsyncLock->fulTransactionType == LOCK_TRANSACTION_FETCH_ADD) ||
                            (pAsyncLock->fulTransactionType == LOCK_TRANSACTION_LITTLE_ADD)) 
                        {

                            pAsyncLock->nNumberOfArgBytes = 0;
                        }
                        else
                            pAsyncLock->nNumberOfArgBytes = sizeof(ULONG)*2;

                        pAsyncLock->nNumberOfDataBytes = sizeof(ULONG)*2;
                    }

                    GetDlgItemText(hDlg, IDC_ASYNC_LOCK_ARGUMENT1, tmpBuff, STRING_SIZE);
                    pAsyncLock->Arguments[0] = strtoul(tmpBuff, NULL, 16);

                    GetDlgItemText(hDlg, IDC_ASYNC_LOCK_ARGUMENT2, tmpBuff, STRING_SIZE);
                    pAsyncLock->Arguments[1] = strtoul(tmpBuff, NULL, 16);

                    GetDlgItemText(hDlg, IDC_ASYNC_LOCK_DATA1, tmpBuff, STRING_SIZE);
                    pAsyncLock->DataValues[0] = strtoul(tmpBuff, NULL, 16);

                    GetDlgItemText(hDlg, IDC_ASYNC_LOCK_DATA2, tmpBuff, STRING_SIZE);
                    pAsyncLock->DataValues[1] = strtoul(tmpBuff, NULL, 16);

                    if (IsDlgButtonChecked(hDlg, IDC_ASYNC_LOCK_GET_GENERATION))
                        pAsyncLock->bGetGeneration = TRUE;
                    else
                        pAsyncLock->bGetGeneration = FALSE;

                    GetDlgItemText(hDlg, IDC_ASYNC_LOCK_GENERATION_COUNT, tmpBuff, STRING_SIZE);
                    pAsyncLock->ulGeneration = strtoul(tmpBuff, NULL, 16);

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
} // AsyncLockDlgProc

void
w1394_AsyncLock(
                HWND         hWnd,
                _In_ PSTR    szDeviceName)
{
    ASYNC_LOCK      asyncLock;
    DWORD           dwRet, bytesReturned;

    UNREFERENCED_PARAMETER(szDeviceName);

    TRACE(TL_TRACE, (hWnd, "Enter w1394_AsyncLock\r\n"));

    asyncLock.bGetGeneration = TRUE;
    asyncLock.DestinationAddress.IA_Destination_ID.NA_Bus_Number = 0x3FF;
    asyncLock.DestinationAddress.IA_Destination_ID.NA_Node_Number = 0;
    asyncLock.DestinationAddress.IA_Destination_Offset.Off_High = 0;
    asyncLock.DestinationAddress.IA_Destination_Offset.Off_Low = 0;
    asyncLock.nNumberOfArgBytes = sizeof(ULONG);
    asyncLock.nNumberOfDataBytes = sizeof(ULONG);
    asyncLock.fulTransactionType = LOCK_TRANSACTION_MASK_SWAP;
    asyncLock.fulFlags = 0;
    asyncLock.Arguments[0] = 0;
    asyncLock.Arguments[1] = 0;
    asyncLock.DataValues[0] = 0;
    asyncLock.DataValues[1] = 0;
    asyncLock.ulGeneration = 0;

    if (DialogBoxParam (
        (HINSTANCE) GetWindowLongPtr (hWnd, GWLP_HINSTANCE),
        "AsyncLock",
        hWnd,
        AsyncLockDlgProc,
        (LPARAM)&asyncLock)) 
    {
        dwRet = SendRequest (
            IOCTL_ASYNC_LOCK, 
            &asyncLock, 
            sizeof (ASYNC_LOCK), 
            NULL, 
            0, 
            &bytesReturned);
        if (ERROR_SUCCESS != dwRet)
        {
            TRACE(TL_WARNING, (hWnd, "Failed SendRequest 0x%x\r\n", dwRet));
        }
        else
        {
            TRACE(TL_TRACE, (hWnd, "Buffer[0] = 0x%x\r\n", asyncLock.Buffer[0]));
            TRACE(TL_TRACE, (hWnd, "Buffer[1] = 0x%x\r\n", asyncLock.Buffer[1]));
        }
    }

    TRACE(TL_TRACE, (hWnd, "Exit w1394_AsyncLock\r\n\r\n"));
    return;
} // w1394_AsyncLock

INT_PTR CALLBACK
AsyncStreamDlgProc(
                   HWND        hDlg,
                   UINT        uMsg,
                   WPARAM      wParam,
                   LPARAM      lParam)
{
    static PASYNC_STREAM    pAsyncStream;
    static CHAR             tmpBuff[STRING_SIZE];

    switch (uMsg) 
    {

        case WM_INITDIALOG:

            pAsyncStream = (PASYNC_STREAM)lParam;

            _ultoa_s(
                pAsyncStream->nNumberOfBytesToStream, 
                tmpBuff, 
                (STRING_SIZE * sizeof(CHAR)), 
                16);
            SetDlgItemText(hDlg, IDC_ASYNC_STREAM_BYTES_TO_STREAM, tmpBuff);

            _ultoa_s(
                pAsyncStream->ulTag, 
                tmpBuff, 
                (STRING_SIZE * sizeof(CHAR)), 
                16);
            SetDlgItemText(hDlg, IDC_ASYNC_STREAM_TAG, tmpBuff);

            _ultoa_s(
                pAsyncStream->nChannel, 
                tmpBuff, 
                (STRING_SIZE * sizeof(CHAR)), 
                16);
            SetDlgItemText(hDlg, IDC_ASYNC_STREAM_CHANNEL, tmpBuff);

            _ultoa_s(
                pAsyncStream->ulSynch, 
                tmpBuff, 
                (STRING_SIZE * sizeof(CHAR)), 
                16);
            SetDlgItemText(hDlg, IDC_ASYNC_STREAM_SYNCH, tmpBuff);

            if (pAsyncStream->nSpeed == SPEED_FLAGS_FASTEST) 
            {
                CheckRadioButton( hDlg,
                                  IDC_ASYNC_STREAM_100MBPS,
                                  IDC_ASYNC_STREAM_FASTEST,
                                  IDC_ASYNC_STREAM_FASTEST);
            }
            else 
            {

                CheckRadioButton( hDlg,
                                  IDC_ASYNC_STREAM_100MBPS,
                                  IDC_ASYNC_STREAM_FASTEST,
                                  pAsyncStream->nSpeed + (IDC_ASYNC_STREAM_100MBPS-1));
            }

            return(TRUE); // WM_INITDIALOG

        case WM_COMMAND:

            switch (LOWORD(wParam)) 
            {

               case IDOK:

                    GetDlgItemText(hDlg, IDC_ASYNC_STREAM_BYTES_TO_STREAM, tmpBuff, STRING_SIZE);
                    pAsyncStream->nNumberOfBytesToStream = strtoul(tmpBuff, NULL, 16);

                    GetDlgItemText(hDlg, IDC_ASYNC_STREAM_TAG, tmpBuff, STRING_SIZE);
                    pAsyncStream->ulTag = strtoul(tmpBuff, NULL, 16);

                    GetDlgItemText(hDlg, IDC_ASYNC_STREAM_CHANNEL, tmpBuff, STRING_SIZE);
                    pAsyncStream->nChannel = strtoul(tmpBuff, NULL, 16);

                    GetDlgItemText(hDlg, IDC_ASYNC_STREAM_SYNCH, tmpBuff, STRING_SIZE);
                    pAsyncStream->ulSynch = strtoul(tmpBuff, NULL, 16);

                    pAsyncStream->nSpeed = 0;
                    if (IsDlgButtonChecked(hDlg, IDC_ASYNC_STREAM_100MBPS))
                        pAsyncStream->nSpeed = SPEED_FLAGS_100;

                    if (IsDlgButtonChecked(hDlg, IDC_ASYNC_STREAM_200MBPS))
                        pAsyncStream->nSpeed = SPEED_FLAGS_200;

                    if (IsDlgButtonChecked(hDlg, IDC_ASYNC_STREAM_400MBPS))
                        pAsyncStream->nSpeed = SPEED_FLAGS_400;

                    if (IsDlgButtonChecked(hDlg, IDC_ASYNC_STREAM_1600MBPS))
                        pAsyncStream->nSpeed = SPEED_FLAGS_1600;

                    if (IsDlgButtonChecked(hDlg, IDC_ASYNC_STREAM_FASTEST))
                        pAsyncStream->nSpeed = SPEED_FLAGS_FASTEST;

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
} // AsyncStreamDlgProc

void
w1394_AsyncStream(
                  HWND         hWnd,
                  _In_ PSTR    szDeviceName)
{
    ASYNC_STREAM    asyncStream;
    PASYNC_STREAM   pAsyncStream = NULL;
    DWORD           dwRet, bytesReturned;
    ULONG             ulBufferSize = sizeof (ASYNC_STREAM);
    ULONG             i; 

    UNREFERENCED_PARAMETER(szDeviceName);

    TRACE(TL_TRACE, (hWnd, "Enter w1394_AsyncStream\r\n"));

    asyncStream.nNumberOfBytesToStream = 8;
    asyncStream.fulFlags = 0;
    asyncStream.ulTag = 0;
    asyncStream.nChannel = 0;
    asyncStream.ulSynch = 0;
    asyncStream.nSpeed = SPEED_FLAGS_FASTEST;

    if (DialogBoxParam( (HINSTANCE) GetWindowLongPtr(hWnd, GWLP_HINSTANCE),
                        "AsyncStream",
                        hWnd,
                        AsyncStreamDlgProc,
                        (LPARAM)&asyncStream))
    {
        ulBufferSize += asyncStream.nNumberOfBytesToStream;

        pAsyncStream = (PASYNC_STREAM) LocalAlloc (LPTR, ulBufferSize);
        if (NULL == pAsyncStream)
        {
            TRACE(TL_WARNING, (hWnd, "Failed to allocate asyncStream\r\n\r\n"));
            return;
        }

        pAsyncStream = (PASYNC_STREAM) &asyncStream;
       

        for (i=0; i < (asyncStream.nNumberOfBytesToStream / sizeof(ULONG)); i++) 
        {
            TRACE(TL_TRACE, (hWnd, "Quadlet[0x%x] = 0x%x\r\n", i, (PULONG)&pAsyncStream->Data[i]));
        }

        dwRet = SendRequest (
            IOCTL_ASYNC_STREAM,
            pAsyncStream,
            ulBufferSize,
            pAsyncStream,
            ulBufferSize,
            &bytesReturned);
        if (ERROR_SUCCESS != dwRet)
        {
            TRACE(TL_WARNING, (hWnd, "Failed SendRequest 0x%x\r\n", dwRet));
        }
    }

    LocalFree (pAsyncStream);
    TRACE(TL_TRACE, (hWnd, "Exit w1394_AsyncStream\r\n\r\n"));
    return;
} // w1394_AsyncStream
