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

#include "notifydata.hpp"

//
// Default Constructor
//

CPrintOEMAsyncNotifyDataObject::
CPrintOEMAsyncNotifyDataObject(
    VOID
    ):
    m_hValid(E_FAIL),
    m_Data(NULL),
    m_Size(0),
    m_Type(NULL),
    m_cRef(1)
{
}

//
// Constructor
//

CPrintOEMAsyncNotifyDataObject::
CPrintOEMAsyncNotifyDataObject(
    _In_reads_bytes_opt_(Size)   BYTE*                           pData,
                            ULONG                           Size,
    _In_opt_                PrintAsyncNotificationType*     pType
    ):
    m_hValid(S_OK),
    m_Type(NULL),
    m_Size(0),
    m_Data(NULL),
    m_cRef(1)
{
    if (Size)
    {
        if (pData)
        {
            m_Data = reinterpret_cast<BYTE*>(LocalAlloc(LPTR, Size));

            if (m_Data)
            {
                CopyMemory(m_Data, pData, Size);
            }
            else
            {
                m_hValid = E_OUTOFMEMORY;
            }
        }
    }

    if (pType)
    {
        m_Type = reinterpret_cast<GUID*>(LocalAlloc(LPTR, sizeof(GUID)));

        if (m_Type)
        {
            CopyMemory(m_Type, pType, sizeof(GUID));
        }
        else
        {
            m_hValid = E_OUTOFMEMORY;
        }
    }

    m_Size = Size;

}

//
// Destructor
//

CPrintOEMAsyncNotifyDataObject::
~CPrintOEMAsyncNotifyDataObject(
    )
{
    if (m_Data)
    {
        LocalFree(reinterpret_cast<void*>(m_Data));
    }

    if (m_Type)
    {
        LocalFree(reinterpret_cast<void*>(m_Type));
    }

    //
    // If this instance of the object is being deleted, then the reference
    // count should be zero.
    //
    assert(0 == m_cRef);
}

//
// Validates object state.
//

HRESULT
CPrintOEMAsyncNotifyDataObject::
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
CPrintOEMAsyncNotifyDataObject::
QueryInterface(
    _In_  REFIID  riid,
    _Out_ VOID    **ppv
    )
{
    HRESULT hrResult = E_POINTER;

    if (ppv)
    {
        hrResult = E_NOINTERFACE;

        *ppv = NULL;

        if (riid == IID_IPrintAsyncNotifyDataObject ||
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
CPrintOEMAsyncNotifyDataObject::
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
CPrintOEMAsyncNotifyDataObject::
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
// Gets data associated with a notification.
//
STDMETHODIMP
CPrintOEMAsyncNotifyDataObject::
AcquireData(
    _Outptr_opt_result_buffer_(*pSize)  BYTE**                       ppbData,
    _Out_opt_                           ULONG*                       pSize,
    _Outptr_opt_                        PrintAsyncNotificationType** pType
    )
{
    HRESULT hrResult = E_FAIL;

    if (SUCCEEDED(hrResult = IsValid()))
    {
        if (!ppbData && !pSize && !pType)
        {
            hrResult = E_INVALIDARG;
        }
        else
        {
            this->AddRef();

            if (ppbData)
            {
                *ppbData = m_Data;
            }

            if (pSize)
            {
                *pSize   = m_Size;
            }

            if (pType)
            {
                *pType   = m_Type;
            }

            hrResult = S_OK;
        }
    }

    return hrResult;
}

//
// Releases data associated with a notification.
//
STDMETHODIMP
CPrintOEMAsyncNotifyDataObject::
ReleaseData(
    VOID
    )
{
    this->Release();

    return S_OK;
}


