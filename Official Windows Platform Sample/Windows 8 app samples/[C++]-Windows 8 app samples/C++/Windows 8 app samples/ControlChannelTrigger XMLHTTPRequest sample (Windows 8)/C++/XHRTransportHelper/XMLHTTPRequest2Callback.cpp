//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//

#include "pch.h"

using namespace XHRTransportHelper;

XMLHTTPRequest2Callback::XMLHTTPRequest2Callback()
{

}

XMLHTTPRequest2Callback::~XMLHTTPRequest2Callback()
{
}

IFACEMETHODIMP XMLHTTPRequest2Callback::OnRedirect(
    IXMLHTTPRequest2 *pXHR,
    const wchar_t *pwszRedirectUrl
)
{
    UNREFERENCED_PARAMETER(pXHR);
    UNREFERENCED_PARAMETER(pwszRedirectUrl);

    //
    // Real-world app should take care of this event when it sends the canary
    // request. The pwszRedirectUrl could be remembered and then use the new
    // url to do the always-connected HTTP connection.
    //

    return S_OK;
}

IFACEMETHODIMP XMLHTTPRequest2Callback::ReadFromStream(
    _In_opt_ ISequentialStream *pStream
)
{
    HRESULT hr = S_OK;
    BYTE buffer[MAX_BUFFER_LENGTH];
    DWORD cbRead = 0;

    if (pStream == NULL)
    {
        hr = E_INVALIDARG;
        goto Exit;
    }

    while (cbRead < MAX_BUFFER_LENGTH)
    {
        hr = pStream->Read(buffer, MAX_BUFFER_LENGTH - 1, &cbRead);

        if (FAILED(hr) ||
            cbRead == 0)
        {
            goto Exit;
        }

        buffer[cbRead] = 0;
    }

Exit:

    return hr;
}

IFACEMETHODIMP XMLHTTPRequest2Callback::OnResponseReceived(
    IXMLHTTPRequest2 *pXHR,
    ISequentialStream *pResponseStream
)
{
    UNREFERENCED_PARAMETER(pXHR);

    //
    // This callack is the best place to fetch all the response data.
    // ReadFromStream focuses on the demonstratation of how to read data from
    // pResponseStream without any real-world scenario.
    //

    ReadFromStream(pResponseStream);

    if (m_sucessHandler != nullptr)
    {
        try
        {
            m_sucessHandler();
        }
        catch (Exception^)
        {
            //
            // Real application should catch special exception and handle it
            // properly.
            //
        }
    }
    return S_OK;
}

IFACEMETHODIMP XMLHTTPRequest2Callback::OnError(
    IXMLHTTPRequest2 *pXHR,
    HRESULT hrError
)
{
    UNREFERENCED_PARAMETER(pXHR);

    if (m_failHandler != nullptr)
    {
        try
        {
            m_failHandler();
        }
        catch (Exception^)
        {
            //
            // Real application should catch special exception and handle it
            // properly.
            //
        }
    }
    return S_OK;
}

HRESULT XMLHTTPRequest2Callback::Initialize(
    RequestSucceedHandler^ sucessHandler,
    RequestFailHandler^ failHandler
)
{
    m_sucessHandler = sucessHandler;
    m_failHandler = failHandler;
    return S_OK;
}

IFACEMETHODIMP XMLHTTPRequest2Callback::RuntimeClassInitialize()
{
    return Initialize(nullptr, nullptr);
}

IFACEMETHODIMP XMLHTTPRequest2Callback::RuntimeClassInitialize(
    RequestSucceedHandler^ sucessHandler,
    RequestFailHandler^ failHandler
)
{
    return Initialize(sucessHandler, failHandler);
}

IFACEMETHODIMP  XMLHTTPRequest2Callback::OnHeadersAvailable(
    IXMLHTTPRequest2 *pXHR,
    DWORD dwStatus,
    const wchar_t *pwszStatus
)
{
    UNREFERENCED_PARAMETER(pXHR);
    UNREFERENCED_PARAMETER(pwszStatus);

    m_dwStatus = dwStatus;
    return S_OK;
}

IFACEMETHODIMP XMLHTTPRequest2Callback::OnDataAvailable(
    IXMLHTTPRequest2 *pXHR,
    ISequentialStream *pResponseStream
)
{
    UNREFERENCED_PARAMETER(pXHR);
    UNREFERENCED_PARAMETER(pResponseStream);

    //
    // For application that needs to do a real-time chunk-by-chunk processing,
    // add the data reading in this callback. However the work must be done
    // as fast as possible, and must not block this thread, for example, waiting
    // on another event object.
    //

    return S_OK;
}
