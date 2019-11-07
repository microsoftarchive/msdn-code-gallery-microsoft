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

#include "acallback.hpp"
#include "notifydata.hpp"

#include "resources.h"

//
// Displays a message to the user loading the message text and caption from the local resources:
//

int MessagePrompt(HWND hWnd, int nCaptionId, int nTextId, DWORD dwType)
{
    WCHAR szText[256] = {};
    WCHAR szCaption[40] = {};
    int nReturn = 0;

    HINSTANCE hInstance = (HINSTANCE)GetModuleHandle(NULL);

    if (hInstance &&
        LoadString(hInstance, nCaptionId, szCaption, ARRAYSIZE(szCaption)) &&
        LoadString(hInstance, nTextId, szText, ARRAYSIZE(szText)))
    {
        nReturn = MessageBox(hWnd, szText, szCaption, dwType);
    }

    return 0;
}

//
// Default Constructor
//

CPrintOEMAsyncNotifyCallback::
CPrintOEMAsyncNotifyCallback(
    HANDLE      unregisterEvent
    ):
    m_cRef(1),
    m_unregisterEvent(unregisterEvent)
{
    m_hValid = (m_unregisterEvent != NULL) ? S_OK : E_FAIL;
}

//
// Destructor
//
CPrintOEMAsyncNotifyCallback::
~CPrintOEMAsyncNotifyCallback(
    )
{
    //
    // If this instance of the object is being deleted, then the reference
    // count should be zero.
    //
    assert(0 == m_cRef);
}

//
// Validates object state
//
HRESULT
CPrintOEMAsyncNotifyCallback::
IsValid(
    VOID
    ) const
{
    return m_hValid;
}

//
// IUnknown Method
//
STDMETHODIMP
CPrintOEMAsyncNotifyCallback::
QueryInterface(
    _In_    REFIID      riid,
    _Out_   VOID**      ppv
    )
{
    HRESULT hrResult = E_POINTER;

    if (ppv)
    {
        hrResult = E_NOINTERFACE;

        *ppv = NULL;

        if (riid == IID_IPrintAsyncNotifyCallback ||
            riid == IID_IUnknown)
        {
            *ppv = reinterpret_cast<VOID *>(this);
            hrResult = S_OK;
        }
    }

    if (SUCCEEDED(hrResult))
    {
        reinterpret_cast<IUnknown *>(*ppv)->AddRef();
    }

    return hrResult;
}

//
// IUnknown Method
//
STDMETHODIMP_(ULONG)
CPrintOEMAsyncNotifyCallback::
AddRef(
    VOID
    )
{
    return InterlockedIncrement(&m_cRef);
}

//
// IUnknown Method
//
STDMETHODIMP_(ULONG)
CPrintOEMAsyncNotifyCallback::
Release(
    VOID
    )
{
    ULONG cRef = InterlockedDecrement(&m_cRef);

    if (cRef == 0)
    {
        delete this;
        return 0;
    }
    return cRef;
}

//
// OnEventNotify is the notification method that gets called.
//
STDMETHODIMP
CPrintOEMAsyncNotifyCallback::
OnEventNotify(
    _In_opt_ IPrintAsyncNotifyChannel*       pIAsynchNotifyChannel,
    _In_     IPrintAsyncNotifyDataObject*    pNotification
    )
{
    HRESULT                     hrResult            = E_FAIL;
    EOEMDataSchema              action              = (EOEMDataSchema)0;
    ULONG                       notificationLength  = 0;
    PrintAsyncNotificationType* notificationType    = NULL;

    if (pNotification)
    {
        BYTE* pData = NULL;

        if (SUCCEEDED(hrResult = pNotification->AcquireData(&pData,
                                                            &notificationLength,
                                                            &notificationType)))
        {
            if (notificationLength != sizeof(EOEMDataSchema))
            {
                MessagePrompt(NULL,  IDS_UNKNOWNDATAFORMAT, IDS_SERVERNOTIFICATION, MB_OK);
            }
            else
            {
                action = UnmarshalSchema(pData);

                if (*notificationType == NOTIFICATION_RELEASE)
                {
                    MessagePrompt(NULL, IDS_PRINTERDELETED, IDS_UNREGISTER, MB_OK);
                    SetEvent(m_unregisterEvent);
                }
                else
                {
                    switch (action)
                    {
                        case SERVER_START_DOC:
                        {
                            MessagePrompt(NULL, IDS_STARTDOC, IDS_SERVERNOTIFICATION, MB_OK);
                            break;
                        }
                        case SERVER_END_DOC:
                        {
                            MessagePrompt(NULL,  IDS_ENDDOC, IDS_SERVERNOTIFICATION, MB_OK);
                            break;
                        }
                        case SERVER_END_DIALOG:
                        {
                            MessagePrompt(NULL,  IDS_ENDDIALOG, IDS_SERVERNOTIFICATION, MB_OK);
                            break;
                        }
                        default:
                        {
                            MessagePrompt(NULL,  IDS_UNKNOWNDATAFORMAT, IDS_SERVERNOTIFICATION, MB_OK);
                            break;
                        }
                    }
                }
            }

            pNotification->ReleaseData();
        }


    }

    if (pIAsynchNotifyChannel)
    {
        ReplyOnEventNotify(pIAsynchNotifyChannel);
    }

    return hrResult;
}

//
// ChannelClosed
//
HRESULT
CPrintOEMAsyncNotifyCallback::
ChannelClosed(
    _In_opt_ IPrintAsyncNotifyChannel*       pIAsynchNotifyChannel,
    _In_     IPrintAsyncNotifyDataObject*    pNotification
    )
{
    HRESULT                     hrResult            = E_FAIL;
    EOEMDataSchema              action              = (EOEMDataSchema)0;
    ULONG                       notificationLength  = 0;
    PrintAsyncNotificationType* notificationType    = NULL;
    BOOL                        bInvalidData        = FALSE;

    if (pIAsynchNotifyChannel)
    {
        hrResult = S_OK;

        if (pNotification)
        {
            BYTE* pData = NULL;

            if (SUCCEEDED(hrResult = pNotification->AcquireData(&pData,
                                                            &notificationLength,
                                                            &notificationType)))
            {
                if (notificationLength != sizeof(EOEMDataSchema))
                {
                    bInvalidData = TRUE;
                }
                else
                {
                    action = UnmarshalSchema(pData);
                }

                if (!bInvalidData && action == SERVER_END_DIALOG)
                {
                    MessagePrompt(NULL,  IDS_ENDDIALOG, IDS_SERVERNOTIFICATION, MB_OK);
                }
                else
                {
                    MessagePrompt(NULL,  IDS_UNKNOWNDATAFORMAT, IDS_SERVERNOTIFICATION, MB_OK);
                }

                pNotification->ReleaseData();
            }

        }
    }

    return hrResult;
}

//
// ReplyOnEventNotify
//
HRESULT
CPrintOEMAsyncNotifyCallback::
ReplyOnEventNotify(
    _In_ IPrintAsyncNotifyChannel*   pIAsynchNotifyChannel
    )
{
    HRESULT                         hrResult           = E_FAIL;
    CPrintOEMAsyncNotifyDataObject* pINotifyDataObject = NULL;
    EOEMDataSchema                  clientResponse     = CLIENT_ACKNOWLEDGED;

    if (pIAsynchNotifyChannel)
    {
        BYTE data[4] = { 0 };
        MarshalSchema(clientResponse, data);
        pINotifyDataObject = new CPrintOEMAsyncNotifyDataObject(data,
                                                                sizeof(data),
                                                                const_cast<GUID*>(&SAMPLE_NOTIFICATION_UI));

        if (SUCCEEDED(hrResult = pINotifyDataObject ? pINotifyDataObject->IsValid() : E_OUTOFMEMORY))
        {
            hrResult = pIAsynchNotifyChannel->CloseChannel(pINotifyDataObject);
        }

        if (pINotifyDataObject)
        {
            pINotifyDataObject->Release();
        }

        pIAsynchNotifyChannel->Release();
    }

    return hrResult;
}

//
// UnmarshalSchema and MarshalSchema are used to convert an EOEMDataSchema enum value into a byte array
// (and vice versa) in a platform-independent fashion (to avoid endianness and size issues on other
// platforms).  This allows the client and server to run on independent architectures without issue.
// To marshal more complex data structures than a simple enum value, it is recommended that
// the RPC Serialization Services be used.  See the MSDN documentation at:
// http://msdn2.microsoft.com/en-us/library/aa378670.aspx for details on their use.
//
EOEMDataSchema
CPrintOEMAsyncNotifyCallback::
UnmarshalSchema(
    _In_reads_(4) BYTE* pData)
{

	return (EOEMDataSchema)(pData[0]  |
                    (pData[1] << 8)  |
                    (pData[2] << 16) |
                    (pData[3] << 24));
}

void
CPrintOEMAsyncNotifyCallback::
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



