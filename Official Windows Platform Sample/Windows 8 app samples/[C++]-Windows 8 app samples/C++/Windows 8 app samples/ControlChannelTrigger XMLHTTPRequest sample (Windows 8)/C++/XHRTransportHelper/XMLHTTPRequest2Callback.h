//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//

#pragma once

#include "pch.h"

using namespace Microsoft::WRL;
using namespace Microsoft::WRL::Details;
using namespace Platform;

namespace XHRTransportHelper
{

    delegate void RequestSucceedHandler();
    delegate void RequestFailHandler();

    class XMLHTTPRequest2Callback : public Microsoft::WRL::RuntimeClass<
        RuntimeClassFlags<RuntimeClassType::ClassicCom>,
        IXMLHTTPRequest2Callback>
    {
    public:
        IFACEMETHODIMP OnRedirect(IXMLHTTPRequest2 *pXHR, const wchar_t *pwszRedirectUrl);
        IFACEMETHODIMP OnHeadersAvailable(IXMLHTTPRequest2 *pXHR, DWORD dwStatus, const wchar_t *pwszStatus);
        IFACEMETHODIMP OnDataAvailable(IXMLHTTPRequest2 *pXHR, ISequentialStream *pResponseStream);
        IFACEMETHODIMP OnResponseReceived(IXMLHTTPRequest2 *pXHR, ISequentialStream *pResponseStream);
        IFACEMETHODIMP OnError(IXMLHTTPRequest2 *pXHR, HRESULT hrError);

    private:

        const static int MAX_BUFFER_LENGTH = 4096;

        DWORD m_dwStatus;

        RequestSucceedHandler^ m_sucessHandler;
        RequestFailHandler^ m_failHandler;

        XMLHTTPRequest2Callback();
        ~XMLHTTPRequest2Callback();
        IFACEMETHODIMP ReadFromStream(_In_opt_ ISequentialStream *pStream);
        HRESULT Initialize(RequestSucceedHandler^ sucessHandler, RequestFailHandler^ failHandler);

        IFACEMETHODIMP RuntimeClassInitialize();
        IFACEMETHODIMP RuntimeClassInitialize(RequestSucceedHandler^ sucessHandler, RequestFailHandler^ failHandler);
        friend HRESULT Microsoft::WRL::Details::MakeAndInitialize<XMLHTTPRequest2Callback,XMLHTTPRequest2Callback, RequestSucceedHandler^, RequestFailHandler^>
            (XMLHTTPRequest2Callback **, RequestSucceedHandler^ &&, RequestFailHandler^ &&);
        friend HRESULT Microsoft::WRL::Details::MakeAndInitialize<XMLHTTPRequest2Callback,XMLHTTPRequest2Callback>(XMLHTTPRequest2Callback **);
    };
}
