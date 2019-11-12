//
//  Windows Server (Printing) Driver Development Kit Samples.
//
//  Sample Print Provider template.
//
//  Copyright (c) 1990 - 2005 Microsoft Corporation.
//  All Rights Reserved.
//
//
//  This source code is intended only as a supplement to Microsoft
//  Development Tools and/or on-line documentation.  See these other
//  materials for detailed information regarding Microsoft code samples.
//
//  THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//  PARTICULAR PURPOSE.
//

#include "precomp.h"
#pragma hdrstop

#include "client.hpp"
#include "acallback.hpp"


/*++

Routine Description:

    Start of asycnnotify client sample program

Arguments:

    argc - argument count
    argv - pointer to an array of command line arguments

Return Value:

    S_OK         - The operation succeeded.
    E_FAIL       - The operation failed.
    E_INVALIDARG - Invalid flag/option specified.

--*/
EXTERN_C
HRESULT _cdecl
_tmain(
    _In_                INT         argc,
    _In_reads_(argc)    LPTSTR*     argv
    )
{
    HRESULT hrResult = E_FAIL;
    CmdLineArgs Args = {0};

    hrResult = ParseCommandLine(argc, argv, &Args);

    if (SUCCEEDED(hrResult))
    {
        hrResult = ValidateArgs(&Args);
    }

    if (SUCCEEDED(hrResult))
    {
        hrResult = ExecuteSample(&Args);

        Sleep(60*1000);
    }
    else
    {
        Usage();
    }

    return hrResult;
}

/*++

Routine Description:

    Validates arguments passed on the command line.

Arguments:

    pArgs - pointer to structure which holds parsed command arguments

Return Value:

    S_OK         - The operation succeeded.
    E_INVALIDARG - Invalid flag/option specified.

--*/
HRESULT
ValidateArgs(
    _In_ CmdLineArgs*    pArgs
    )
{
    HRESULT hrResult = E_INVALIDARG;

    if (pArgs->pszName && *pArgs->pszName)
    {
        hrResult = S_OK;
    }

    return hrResult;
}

/*++

Routine Description:

    Executes the command specified on the command line.
    This function is actually never called in this sample. It
    is provided here to illustrate another method of
    registering for the Async notifications.

Arguments:

    pArgs - pointer to structure which holds parsed command arguments

Return Value:

    S_OK   - The operation succeeded.
    E_FAIL - The operation failed.

--*/
HRESULT
ExecuteSample(
    _In_ CmdLineArgs*   pArgs
    )
{
    HRESULT hrResult = E_FAIL;
    HANDLE  hNotify  = NULL;

    if (SUCCEEDED(hrResult = pArgs ? S_OK : E_INVALIDARG))
    {
        HANDLE unregisterForNotificationsEvent = CreateEvent(NULL, FALSE, FALSE, NULL);

        if (SUCCEEDED(hrResult = unregisterForNotificationsEvent ? S_OK : E_INVALIDARG))
        {
            CPrintOEMAsyncNotifyCallback *pIAsynchCallback = new CPrintOEMAsyncNotifyCallback(unregisterForNotificationsEvent);

            if (SUCCEEDED(hrResult = pIAsynchCallback ? pIAsynchCallback->IsValid() : E_OUTOFMEMORY))
            {
                hrResult = RegisterForPrintAsyncNotifications(pArgs->pszName,
                                                            const_cast<GUID*>(&SAMPLE_NOTIFICATION_UI),
                                                            kAllUsers,
                                                            kUniDirectional,
                                                            pIAsynchCallback,
                                                            &hNotify);

                if (hNotify != NULL)
                {
                    //
                    // Do your own work here. In this sample we wait for a while.
                    //
                    WaitForSingleObject(unregisterForNotificationsEvent, 10 * 60 * 1000);

                    UnRegisterForPrintAsyncNotifications(hNotify);
                }
            }

            if (pIAsynchCallback)
            {
                pIAsynchCallback->Release();
            }

            CloseHandle(unregisterForNotificationsEvent);
        }
    }

    return hrResult;
}

HRESULT
ParseCommandLine(
    _In_                UINT            argc,
    _In_reads_(argc)    LPTSTR*         argv,
    _In_                CmdLineArgs*    pArgs
    )

/*++

Routine Description:

    This function collects the command line arguments into variables
    passed as arguments. This is a very simplistic way to parse the command
    line and it does not scale well when the number of command line
    arguments is large.

Arguments:

    argc - argument count
    argv - pointer to an array of command line arguments
    pArgs - pointer to structure which holds parsed command arguments

Return Value:

    S_OK         - The operation succeeded.
    E_NOTIMPL    - The flag or option is not implemented.
    E_INVALIDARG - Invalid flag/option specified.

--*/
{
    HRESULT hrResult = E_INVALIDARG;

    if (argc && argv)
    {
        //
        // Skip past the program name.
        //
        argc--, argv++;

        for (UINT i = 0; i < argc; i++)
        {
            if (*(argv+i) && (**(argv+i) == _T('-') || **(argv+i) == _T('/')))
            {
                //
                // Only single letter switches are allowed.
                //
                switch ((*(*(argv+i)+1) && !*(*(argv+i)+2)) ? *(*(argv+i)+1) : _T('\0'))
                {
                    case _T('n'):
                    {
                        i = i + 1;
                        pArgs->pszName = *(argv+i) ? *(argv+i) : _T("");
                        hrResult = S_OK;
                        break;
                    }
                    case _T('?'):
                    {
                        Usage();
                        hrResult = E_NOTIMPL;
                        break;
                    }
                    default:
                    {
                        Usage();
                        hrResult = E_INVALIDARG;
                        break;
                    }
                }
            }
        }
    }

    return hrResult;
}

/*++

Routine Description:

    Displays program usage. If new flags or options are added,
    please update this function with a description of the new
    option or flag.

Arguments:

    None

Return Value:

    None

--*/
VOID
Usage(
    VOID
    )
{
    _tprintf(L"ddkasyncnotify - Register For Async notifications test program.\n");
    _tprintf(L"usage:  ddkasyncnotify [-n unc-printer-name]\n");
    _tprintf(L"Arguments:\n");
    _tprintf(L"-n     UNC printer name.\n");
    _tprintf(L"\n");
 }



