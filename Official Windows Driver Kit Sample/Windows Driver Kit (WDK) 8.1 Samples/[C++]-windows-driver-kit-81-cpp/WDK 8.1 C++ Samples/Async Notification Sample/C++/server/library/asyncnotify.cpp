//
//  Windows Server (Printing) Driver Development Kit Samples.
//
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

_Analysis_mode_(_Analysis_code_type_user_driver_)

#include "acallback.hpp"
#include "notifydata.hpp"

//
// MarshalSchema is used to marshal an EOEMDataSchema enum value into a byte array
// in a platform-independent fashion (to avoid endianness and size issues on other platforms).
// This allows the client and server to run on independent architectures without issue.
// To marshal more complex data structures than a simple enum value, it is recommended that
// the RPC Serialization Services be used.  See the MSDN documentation at:
// http://msdn2.microsoft.com/en-us/library/aa378670.aspx for details on their use.
//
void
MarshalSchema(
    _In_ EOEMDataSchema const &in,
    _Out_ BYTE (&out)[4]
    )
{
    out[0] = (BYTE)(in);
    out[1] = (BYTE)(in >> 8);
    out[2] = (BYTE)(in >> 16);
    out[3] = (BYTE)(in >> 24);
}

VOID
SendAsyncNotification(
    _In_ LPWSTR     pPrinterName,
    EOEMDataSchema  action
    )
{
    IPrintAsyncNotifyChannel *pIAsynchNotification = NULL;

    CPrintOEMAsyncNotifyCallback *pIAsynchCallback = new CPrintOEMAsyncNotifyCallback;

    if (pIAsynchCallback)
    {
        RouterCreatePrintAsyncNotificationChannel(pPrinterName,
                                                  const_cast<GUID*>(&SAMPLE_NOTIFICATION_UI),
                                                  kPerUser,
                                                  kBiDirectional,
                                                  pIAsynchCallback,
                                                  &pIAsynchNotification);

        pIAsynchCallback->Release();
    }

    if (pIAsynchNotification)
    {
        BYTE data[4] = { 0 };
        MarshalSchema(action, data);
        CPrintOEMAsyncNotifyDataObject *pClientNotification = new CPrintOEMAsyncNotifyDataObject(data,
                                                                                                 sizeof(data),
                                                                                                 const_cast<GUID*>(&SAMPLE_NOTIFICATION_UI));

        if (pClientNotification)
        {
            pIAsynchNotification->SendNotification(pClientNotification);

            pClientNotification->Release();
        }

        pIAsynchNotification->Release();
    }
}

VOID
SendAsyncUINotification(
    _In_ LPWSTR          pPrinterName
    )
{
    WCHAR pszMsg[] = L"<?xml version=\"1.0\" ?>" \
                     L"<asyncPrintUIRequest xmlns=\"http://schemas.microsoft.com/2003/print/asyncui/v1/request\">" \
                     L"<v1><requestOpen><balloonUI><title>AsyncUI sample</title><body>This text is a sample.</body>" \
                     L"</balloonUI></requestOpen></v1></asyncPrintUIRequest>";

    IPrintAsyncNotifyChannel *pIAsynchNotification = NULL;

    CPrintOEMAsyncNotifyCallback *pIAsynchCallback = new CPrintOEMAsyncNotifyCallback;

    if (pIAsynchCallback)
    {
        RouterCreatePrintAsyncNotificationChannel(pPrinterName,
                                                const_cast<GUID*>(&MS_ASYNCNOTIFY_UI),
                                                kPerUser,
                                                kBiDirectional,
                                                pIAsynchCallback,
                                                &pIAsynchNotification);

        pIAsynchCallback->Release();
    }

    if (pIAsynchNotification)
    {
        CPrintOEMAsyncNotifyDataObject *pClientNotification = new CPrintOEMAsyncNotifyDataObject(reinterpret_cast<BYTE*>(pszMsg),
                                                                                                 sizeof(pszMsg),
                                                                                                 const_cast<GUID*>(&MS_ASYNCNOTIFY_UI));

        if (pClientNotification)
        {
            pIAsynchNotification->SendNotification(pClientNotification);

            pClientNotification->Release();
        }

        pIAsynchNotification->Release();
    }
}



