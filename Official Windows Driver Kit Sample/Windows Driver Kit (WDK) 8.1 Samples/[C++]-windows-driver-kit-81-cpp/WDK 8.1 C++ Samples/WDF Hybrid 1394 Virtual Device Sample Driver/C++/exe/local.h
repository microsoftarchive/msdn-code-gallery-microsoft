/*++

Copyright (c) Microsoft Corporation

Module Name:

    local.h
--*/

//
// Once the use has selected the test device
// we'll get and use a handle to it for submitting IOCLTs to the 
// hybrid stack.
//
HANDLE               g_hTestDevice;



#define STRING_SIZE             1024
#define APP_TIMER_ID      0x0001
#define EDIT_ID                 0x0001


#ifdef _WDF1394_C
HINSTANCE               g_hInstance;
HWND                    g_hWndEdit;

PSTR                    szBusName = "\\\\.\\1394BUS0";

DEVICE_DATA             DeviceData;
DEVICE_DATA             VirtDeviceData;

PSTR                    SelectedDevice;

SYSTEMTIME              AsyncStartTime;
SYSTEMTIME              IsochStartTime;
SYSTEMTIME              StreamStartTime;


#else

extern HINSTANCE        g_hInstance;
extern HWND             g_hWndEdit;

extern PDEVICE_DATA     DeviceData;
extern SYSTEMTIME       AsyncStartTime;
extern SYSTEMTIME       IsochStartTime;
extern SYSTEMTIME       StreamStartTime;



#endif // _WDF1394_C

INT_PTR CALLBACK
SelectDeviceDlgProc (
                    HWND        hDlg,
                    UINT        uMsg,
                    WPARAM      wParam,
                    LPARAM      lParam);

void
w1394_SelectTestDevice (
                        HWND        hWnd);

INT_PTR CALLBACK
w1394_AboutDlgProc(
                   HWND    hDlg,
                   UINT    uMsg,
                   WPARAM  wParam,
                   LPARAM  lParam);

LRESULT CALLBACK
w1394_AppWndProc(
                 HWND    hWnd,
                 UINT    iMsg,
                 WPARAM  wParam,
                 LPARAM  lParam);

int WINAPI
WinMain (
         _In_ HINSTANCE hInstance,
         _In_opt_ HINSTANCE hPrevInstance,
         _In_ PSTR szCmdLine,
         _In_ int iCmdShow);


