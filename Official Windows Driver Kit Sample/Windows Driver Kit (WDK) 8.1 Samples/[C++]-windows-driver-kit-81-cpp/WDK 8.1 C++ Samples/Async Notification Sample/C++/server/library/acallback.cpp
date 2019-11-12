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
// Default Constructor
//
CPrintOEMAsyncNotifyCallback::
CPrintOEMAsyncNotifyCallback(
    ):
    m_cRef(1)
{
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
// IUnknown Method
//
STDMETHODIMP
CPrintOEMAsyncNotifyCallback::
QueryInterface(
    _In_         REFIID  riid,
    _COM_Outptr_ VOID    **ppv
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
    _In_ IPrintAsyncNotifyChannel        *pIAsyncNotification,
    _In_ IPrintAsyncNotifyDataObject     *pNotification
    )
{
    HRESULT hrResult = E_FAIL;
    EOEMDataSchema* pAction   = NULL;
    ULONG uLen = 0;
    PrintAsyncNotificationType* NType;

    if (pNotification)
    {
        if (SUCCEEDED(hrResult = pNotification->AcquireData(reinterpret_cast<BYTE**>(&pAction), &uLen, &NType)))
        {
            if (pAction)
            {
                switch (*pAction)
                {
                    case CLIENT_ACKNOWLEDGED:
                    {
                        hrResult = S_OK;
                        break;
                    }
                    default:
                    {
                        break;
                    }
                }
            }

            pNotification->ReleaseData();
        }
    }

    if (pIAsyncNotification)
    {
        ReplyOnClientEventNotify(pIAsyncNotification);
    }

    return hrResult;
}

//
// ChannelClosed
//
HRESULT
CPrintOEMAsyncNotifyCallback::
ChannelClosed(
    _In_ IPrintAsyncNotifyChannel        *pIAsyncNotification,
    _In_ IPrintAsyncNotifyDataObject     *pNotification
    )
{
    UNREFERENCED_PARAMETER(pIAsyncNotification);

    HRESULT hrResult = S_OK;

    PWCHAR pAction = NULL;
    ULONG uLen = 0;
    PrintAsyncNotificationType* NType;

    if (pNotification)
    {
        if (SUCCEEDED(hrResult = pNotification->AcquireData(reinterpret_cast<BYTE**>(&pAction), &uLen, &NType)))
        {
            if (pAction)
            {
                switch (*pAction)
                {
                    case CLIENT_ACKNOWLEDGED:
                    {
                        hrResult = S_OK;
                        break;
                    }
                    default:
                    {
                        break;
                    }
                }
            }
            pNotification->ReleaseData();
        }
    }

    return hrResult;
}

//
// ReplyOnEventNotify
//
HRESULT
CPrintOEMAsyncNotifyCallback::
ReplyOnClientEventNotify(
    _In_ IPrintAsyncNotifyChannel*       pIAsynchNotifyChannel
    )
{
    HRESULT                         hrResult           = E_FAIL;
    CPrintOEMAsyncNotifyDataObject* pINotifyDataObject = NULL;
    EOEMDataSchema                  serverResponse     = SERVER_END_DIALOG;

    if (pIAsynchNotifyChannel)
    {
        pINotifyDataObject = new CPrintOEMAsyncNotifyDataObject(reinterpret_cast<BYTE*>(serverResponse),
                                                                sizeof(EOEMDataSchema),
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


