//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#include "stdafx.h"
#include "flickr.wsdl.h"
#include "WsHelpers.h"
#include "FlickrUploader.h"
#include "window.h"
#include <Ddeml.h>


FlickrUploader::FlickrUploader(void)
{
}


FlickrUploader::~FlickrUploader(void)
{
}


// IMPORTANT: To enable uploading to Flickr in this appliation, you'll need to provide your own values
// for the static variables below (flickr_api_key and flickr_secret).
// Visit http://www.flickr.com/services/api/auth.howto.desktop.html to obtain these values.
static wchar_t* flickr_api_key = L"";
static wchar_t* flickr_secret = L"";

static const wchar_t* flickr_auth_getFrob_method_name = L"flickr.auth.getFrob";
static const wchar_t* flickr_auth_getToken_method_name = L"flickr.auth.getToken";
static const wchar_t* flickr_soap_endpoint_url = L"http://api.flickr.com/services/soap/";
static const wchar_t* flickr_upload_photo_url = L"http://api.flickr.com/services/upload/";

//
// Obtain frob and connect with Flickr
//
std::wstring FlickrUploader::Connect()
{
    // Request a frob
    std::wstring frob = ObtainFrob();
    // Create a login link
    std::wstring url = CreateLoginLink(frob);
    // Starting Web Browser
    ::ShellExecute(nullptr, L"open", url.c_str(), nullptr, nullptr, SW_SHOWNORMAL);
    return frob;
}
//
// Return Token
//
std::wstring FlickrUploader::GetToken(const std::wstring& frob)
{
    std::wstring outputString;
    WS_ERROR* error = nullptr;
    WS_HEAP* heap = nullptr;
    WS_SERVICE_PROXY* proxy = nullptr;

    HRESULT hr = CreateWebProxy(&heap, &proxy, &error);
    if(SUCCEEDED(hr))
    {
        _FlickrRequest request = 
        {
            (wchar_t*)(flickr_auth_getToken_method_name), 
            (wchar_t*) flickr_api_key, // api_key
            nullptr, // api_sig computed later
            (wchar_t*)frob.c_str()
        };

        std::wstring params = flickr_secret;
        params += L"api_key";
        params += request.api_key;
        params += L"frob";
        params += request.frob;
        params += L"method";
        params += request.method;

        std::wstring api_sig = CalculateMD5Hash(params);
        request.api_sig = const_cast<wchar_t*>(api_sig.c_str());
        wchar_t* token = nullptr;
        hr = FlickrTokenRequestPortBinding_flickr_auth_getToken(proxy, &request, &token, heap, nullptr, 0, nullptr, error);
        if (SUCCEEDED(hr))
        {
            bool errorFound = false;
            std::wstring value = GetXmlElementValueByName(token, L"token", &errorFound);
            if (!errorFound)
            {
                outputString = value;
            }
        }
    }

    CloseWebProxy(&heap, &proxy, &error);

    return outputString;
}
//
// Upload the photo to Flickr and return the corresponding photo ID
//
std::wstring FlickrUploader::UploadPhotos(const std::wstring& token, const std::wstring& fileName, bool* errorFound)
{
    std::wstring outputString;
    HINTERNET session = nullptr;
    HINTERNET connect = nullptr;
    HINTERNET request = nullptr;

    WINHTTP_AUTOPROXY_OPTIONS  autoProxyOptions;
    WINHTTP_PROXY_INFO         proxyInfo;
    unsigned long proxyInfoSize = sizeof(proxyInfo);
    ZeroMemory(&autoProxyOptions, sizeof(autoProxyOptions));
    ZeroMemory(&proxyInfo, sizeof(proxyInfo));

    // Create the WinHTTP session.
    session = ::WinHttpOpen( L"Hilo/1.0", WINHTTP_ACCESS_TYPE_NO_PROXY, WINHTTP_NO_PROXY_NAME, WINHTTP_NO_PROXY_BYPASS, 0);
    if (session)
    {
        connect = ::WinHttpConnect(session, L"api.flickr.com", INTERNET_DEFAULT_HTTP_PORT, 0);
    }
    if (connect)
    {
        request = ::WinHttpOpenRequest(connect, L"POST", L"/services/upload/", L"HTTP/1.1", WINHTTP_NO_REFERER, WINHTTP_DEFAULT_ACCEPT_TYPES, 0);
    }
    if (request)
    {
        autoProxyOptions.dwFlags = WINHTTP_AUTOPROXY_AUTO_DETECT;
        // Use DHCP and DNS-based auto-detection.
        autoProxyOptions.dwAutoDetectFlags = WINHTTP_AUTO_DETECT_TYPE_DHCP | WINHTTP_AUTO_DETECT_TYPE_DNS_A;
        // If obtaining the PAC script requires NTLM/Negotiate authentication, then automatically supply the client domain credentials.
        autoProxyOptions.fAutoLogonIfChallenged = true;

        int result = TRUE;
        if (FALSE != ::WinHttpGetProxyForUrl(session, L"http://api.flickr.com/services/upload/", &autoProxyOptions, &proxyInfo))
        {
            // A proxy configuration was found, set it on the request handle.
            result = ::WinHttpSetOption(request, WINHTTP_OPTION_PROXY, &proxyInfo, proxyInfoSize);
        }

        if (result)
        {
            result = SendWebRequest(&request, token, fileName);
        }

        if (result)
        {
            // Receive the response and get photo ID
            outputString = GetPhotoId(&request, errorFound);
        }
    }
    // Clean up
    if (proxyInfo.lpszProxy)
    {
        GlobalFree(proxyInfo.lpszProxy);
    }
    if (proxyInfo.lpszProxyBypass)
    {
        GlobalFree(proxyInfo.lpszProxyBypass);
    }
    if (request)
    {
        WinHttpCloseHandle(request);
    }
    if (connect)
    {
        WinHttpCloseHandle(connect);
    }
    if (session)
    {
        WinHttpCloseHandle(session);
    }
    return outputString;
}
//
// Send request to Flickr to upload the specified file
//
int FlickrUploader::SendWebRequest(const HINTERNET *request, const std::wstring& token, const std::wstring& fileName)
{
    static const char* mimeBoundary = "EBA799EB-D9A2-472B-AE86-568D4645707E";
    static const wchar_t* contentType = L"Content-Type: multipart/form-data; boundary=EBA799EB-D9A2-472B-AE86-568D4645707E\r\n";
    // Parameters put in alphabetical order to generate the api_sig
    std::wstring params = flickr_secret;
    params += L"api_key";
    params += flickr_api_key;
    params += L"auth_token";
    params += token;
    std::wstring api_sig = CalculateMD5Hash(params);
    int result = ::WinHttpAddRequestHeaders(*request, contentType, (unsigned long)-1, WINHTTP_ADDREQ_FLAG_ADD);
    if (result)
    {
        std::wostringstream sb;

        sb << L"--" << mimeBoundary << L"\r\n";
        sb << L"Content-Disposition: form-data; name=\"api_key\"\r\n";
        sb << L"\r\n" << flickr_api_key << L"\r\n";

        sb << L"--" << mimeBoundary << L"\r\n";
        sb << L"Content-Disposition: form-data; name=\"auth_token\"\r\n";
        sb << L"\r\n" << token << L"\r\n";

        sb << L"--" << mimeBoundary << L"\r\n";
        sb << L"Content-Disposition: form-data; name=\"api_sig\"\r\n";
        sb << L"\r\n" << api_sig.c_str() << L"\r\n";

        sb << L"--" << mimeBoundary << L"\r\n";
        sb << L"Content-Disposition: form-data; name=\"photo\"; filename=\"" << fileName << L"\"\r\n\r\n";

        // Convert wstring to string
        std::wstring wideString = sb.str();
        int stringSize = WideCharToMultiByte(CP_ACP, 0, wideString.c_str(), -1, nullptr, 0, nullptr, nullptr);
        char* temp = new char[stringSize];
        WideCharToMultiByte(CP_ACP, 0, wideString.c_str(), -1, temp, stringSize, nullptr, nullptr);
        std::string str = temp;
        delete [] temp;

        // Add the photo to the stream
        std::ifstream f(fileName, std::ios::binary);
        std::ostringstream sb_ascii;
        sb_ascii << str;
        sb_ascii << f.rdbuf();
        sb_ascii << "\r\n--" << mimeBoundary << "\r\n";
        str = sb_ascii.str();
        result = WinHttpSendRequest(
            *request,
            WINHTTP_NO_ADDITIONAL_HEADERS,
            0,
            (void*)str.c_str(),
            static_cast<unsigned long>(str.length()),
            static_cast<unsigned long>(str.length()),
            0);
    }
    return result;
}
//
// Receive response from Flickr and pass the photoId
//
std::wstring FlickrUploader::GetPhotoId(const HINTERNET *request, bool* errorFound)
{
    std::wstring outputString;
    int result = ::WinHttpReceiveResponse(*request, nullptr);
    unsigned long dwSize = sizeof(unsigned long);
    if (result)
    {
        wchar_t headers[1024];
        dwSize = ARRAYSIZE(headers) * sizeof(wchar_t);
        result = ::WinHttpQueryHeaders(*request, WINHTTP_QUERY_RAW_HEADERS, nullptr, headers, &dwSize, nullptr);
    }
    if (result)
    {
        char resultText[1024] = {0};
        unsigned long bytesRead;
        dwSize = ARRAYSIZE(resultText) * sizeof(char);
        result =::WinHttpReadData(*request, resultText, dwSize, &bytesRead);
        if (result)
        {
            // Convert string to wstring
            int wideSize = MultiByteToWideChar(CP_UTF8, 0, resultText, -1, 0, 0);
            wchar_t* wideString = new wchar_t[wideSize];
            result = MultiByteToWideChar(CP_UTF8, 0, resultText, -1, wideString, wideSize);
            if (result)
            {
                std::wstring photoId = GetXmlElementValueByName(wideString, L"photoid", errorFound);
                if (!(*errorFound))
                {
                    outputString = photoId;
                }
            }
            delete [] wideString;
        }
    }
    return outputString;
}
//
// Create Login Link
//
std::wstring FlickrUploader::CreateLoginLink(const std::wstring& frob)
{
    // Parameters put in alphabetical order to generate the api_sig
    std::wstring params = flickr_secret;
    params += L"api_key";
    params += flickr_api_key;
    params += L"frob";
    params += frob;
    params += L"perms";
    params += L"write";

    std::wstring api_sig = CalculateMD5Hash(params);
    std::wostringstream stringStream;
    stringStream << L"http://www.flickr.com/services/auth/?api_key=" << flickr_api_key << L"&perms=write&frob=" << frob << L"&api_sig=" << api_sig;

    return stringStream.str();
}
//
// Request a frob to identify the login session
//
std::wstring FlickrUploader::ObtainFrob()
{
    WS_ERROR* error = nullptr;
    WS_HEAP* heap = nullptr;
    WS_SERVICE_PROXY* proxy = nullptr;
    std::wstring outputString;
    HRESULT hr = CreateWebProxy(&heap, &proxy, &error);
    if(SUCCEEDED(hr))
    {
        _FlickrRequest request = 
        {
            (wchar_t*) flickr_auth_getFrob_method_name, 
            (wchar_t*) flickr_api_key, // api_key
            nullptr, // api_sig computed later
            nullptr // frob is not needed
        };

        std::wstring params = flickr_secret;
        params += L"api_key";
        params += request.api_key;
        params += L"method";
        params += request.method;

        std::wstring api_sig = CalculateMD5Hash(params);
        request.api_sig = const_cast<wchar_t*>(api_sig.c_str());
        wchar_t* frob = nullptr;
        hr = FlickrFrobRequestPortBinding_flickr_auth_getFrob(
            proxy,
            &request,
            &frob, 
            heap, nullptr, 0, nullptr, error);

        if (SUCCEEDED(hr))
        {
            bool errorFound = false;
            std::wstring result = GetXmlElementValueByName(frob, L"frob", &errorFound);
            if (!errorFound)
            {
                outputString = result;
            }
        }
    }

    CloseWebProxy(&heap, &proxy, &error);

    return outputString;
}

//
// Create web service proxy
//
HRESULT FlickrUploader:: CreateWebProxy (WS_HEAP** heap, WS_SERVICE_PROXY** proxy,  WS_ERROR** error)
{
    WS_ENVELOPE_VERSION soapVersion = WS_ENVELOPE_VERSION_SOAP_1_2;
    WS_ADDRESSING_VERSION addressingVersion = WS_ADDRESSING_VERSION_TRANSPORT;
    // The Flickr API expects all data to be UTF-8 encoded.
    WS_ENCODING encoding = WS_ENCODING_XML_UTF8;

    WS_CHANNEL_PROPERTY channelProperties[3] =
    {
        {
            WS_CHANNEL_PROPERTY_ENVELOPE_VERSION,
                &soapVersion,
                sizeof(WS_ENVELOPE_VERSION)
        },
        {
            WS_CHANNEL_PROPERTY_ADDRESSING_VERSION,
                &addressingVersion ,
                sizeof(WS_ADDRESSING_VERSION)
            },
            {
                WS_CHANNEL_PROPERTY_ENCODING,
                    &encoding,
                    sizeof(WS_ENCODING)
            }
    };

    WS_ENDPOINT_ADDRESS address = 
    {
        {
            static_cast<unsigned long>(wcslen(flickr_soap_endpoint_url)), 
                const_cast<wchar_t*>(flickr_soap_endpoint_url)
        }
    };

    // Create an error object for storing rich error information
    HRESULT hr = WsCreateError(nullptr, 0, error);
    if(SUCCEEDED(hr))
    {
        // Create a heap to store deserialized data
        hr = WsCreateHeap(/*maxSize*/ 4096, /*trimSize*/ 512, nullptr, 0, heap, *error);
    }

    if(SUCCEEDED(hr))
    {
        // Create the proxy
        hr = WsCreateServiceProxy(
            WS_CHANNEL_TYPE_REQUEST, 
            WS_HTTP_CHANNEL_BINDING, 
            nullptr,
            nullptr, 
            0, 
            channelProperties,
            ARRAYSIZE(channelProperties),
            proxy, 
            *error);
    }

    if(SUCCEEDED(hr))
    {
        hr = WsOpenServiceProxy(*proxy, &address, nullptr, *error);
    }
    return hr;
}
//
// Parse the XML data returned from Flickr, if error happens, return the error message.
//
std::wstring FlickrUploader::GetXmlElementValueByName(const std::wstring& xmlContent, const std::wstring& elementName, bool* errorFound)
{
    *errorFound = false;
    HGLOBAL memory;
    // Allocate a global memory for the xml content
    memory = ::GlobalAlloc(GMEM_MOVEABLE, xmlContent.length() * sizeof(wchar_t));
    void* data = ::GlobalLock(memory);
    // Fill memory
    ::memcpy(data, xmlContent.c_str(), xmlContent.length() * sizeof(wchar_t));
    ::GlobalUnlock(memory);

    // Create stream based on the allocated memory
    ComPtr<IStream> stream;
    HRESULT hr = ::CreateStreamOnHGlobal(memory, FALSE, &stream);

    // Create Xml Reader Input based on the stream
    ComPtr<IXmlReaderInput> readerInput;
    if (SUCCEEDED(hr))
    {
        hr = CreateXmlReaderInputWithEncodingCodePage(stream, 0, CP_WINNEUTRAL, false, 0, &readerInput);
    }
    // Create Xml Reader
    ComPtr<IXmlReader> reader;
    if (SUCCEEDED(hr))
    {
        hr = CreateXmlReader(IID_IXmlReader, reinterpret_cast<void**>(&reader), 0);
    }
    if (SUCCEEDED(hr))
    {
        hr = reader->SetInput(readerInput);
    }
    std::wstring resultString;
    if (SUCCEEDED(hr))
    {
        XmlNodeType nodeType;
        // Parse xml file
        bool found = false;
        while (S_OK == (hr = reader->Read(&nodeType)) && !found) 
        {
            switch (nodeType)
            {
            case XmlNodeType_Element:
                {
                    const wchar_t* name;
                    if (FAILED(hr = reader->GetQualifiedName(&name, nullptr)))
                    {
                        *errorFound = true;
                        resultString = L"Parsing content from Flickr failed unexpectedly.";
                        break;
                    }

                    if (wcscmp(name, elementName.c_str()) == 0)
                    {
                        // Read next node
                        reader->Read(&nodeType);
                        unsigned int len = 0;
                        const wchar_t* element = nullptr;
                        if (SUCCEEDED(reader->GetValue(&element, &len)))
                        {
                            // Find the value
                            resultString = element;
                            found = true;
                        }
                    }
                    else if (wcscmp(name, L"err") == 0) // We got an error code
                    {
                        *errorFound = true;
                    }
                    break;
                }
            case XmlNodeType_Attribute:
                {
                    if (*errorFound)
                    {
                        const wchar_t* name;
                        if (FAILED(hr = reader->GetQualifiedName(&name, nullptr)))
                        {
                            *errorFound = true;
                            resultString = L"Parsing content from Flickr failed unexpectedly.";
                            break;
                        }

                        if (wcscmp(name, L"msg") == 0)
                        {
                            unsigned int len = 0;
                            const wchar_t* element = nullptr;
                            if (SUCCEEDED(reader->GetValue(&element, &len)))
                            {
                                // Get the error message
                                resultString = element;
                                found = true;
                            }
                        }
                    }
                    break;
                }
            }
        }
    }
    return resultString;
}
//
// Generate MD5 hash from the given string
//
std::wstring FlickrUploader::CalculateMD5Hash(const std::wstring& buffer)
{
#define NT_SUCCESS(Status)          (((NTSTATUS)(Status)) >= 0)
#define STATUS_UNSUCCESSFUL         ((NTSTATUS)0xC0000001L)

    // Convert wstring to string
    std::string byteString(buffer.begin(), buffer.end());

    // Open an algorithm handle
    BCRYPT_ALG_HANDLE algorithm = nullptr;
    NTSTATUS status = BCryptOpenAlgorithmProvider(&algorithm, BCRYPT_MD5_ALGORITHM, nullptr, 0);

    // Calculate the size of the buffer to hold the hash object
    unsigned long dataSize = 0;
    unsigned long hashObjectSize = 0;
    if (NT_SUCCESS(status))
    {
        status = BCryptGetProperty(algorithm, BCRYPT_OBJECT_LENGTH, (unsigned char*)&hashObjectSize, sizeof(unsigned long), &dataSize, 0);
    }
    // Allocate the hash object on the heap
    unsigned char* hashObject = nullptr;
    if (NT_SUCCESS(status))
    {
        hashObject = (unsigned char*) HeapAlloc(GetProcessHeap (), 0, hashObjectSize);
        if (nullptr == hashObject)
        {
            status = STATUS_UNSUCCESSFUL;
        }
    }
    // Calculate the length of the hash
    unsigned long  hashSize = 0;
    if (NT_SUCCESS(status))
    {
        status = BCryptGetProperty(algorithm, BCRYPT_HASH_LENGTH, (unsigned char*)&hashSize, sizeof(unsigned long), &dataSize, 0);
    }
    // Allocate the hash buffer on the heap
    unsigned char* hash = nullptr;
    if (NT_SUCCESS(status))
    {
        hash = (unsigned char*)HeapAlloc (GetProcessHeap(), 0, hashSize);

        if (nullptr == hash)
        {
            status = STATUS_UNSUCCESSFUL;
        }
    }
    // Create a hash
    BCRYPT_HASH_HANDLE cryptHash = nullptr;
    if (NT_SUCCESS(status))
    {
        status = BCryptCreateHash(algorithm, &cryptHash, hashObject, hashObjectSize, nullptr, 0, 0);
    }
    // Hash data
    if (NT_SUCCESS(status))
    {
        status = BCryptHashData(cryptHash, (unsigned char*)byteString.c_str(), static_cast<unsigned long>(byteString.length()), 0);
    }
    // Close the hash and get hash data
    if (NT_SUCCESS(status))
    {
        status = BCryptFinishHash(cryptHash, hash, hashSize, 0);
    }

    std::wstring resultString;
    // If no issues, then copy the bytes to the output string 
    if (NT_SUCCESS(status))
    {
        std::wostringstream hexString;
        for (unsigned short i = 0; i < hashSize; i++)
        {
            hexString << std::setfill(L'0') << std::setw(2) << std::hex << hash[i];
        }

        resultString =  hexString.str();
    }

    // Cleanup
    if(algorithm)
    {
        BCryptCloseAlgorithmProvider(algorithm, 0);
    }

    if (cryptHash)
    {
        BCryptDestroyHash(cryptHash);
    }

    if(hashObject)
    {
        HeapFree(GetProcessHeap(), 0, hashObject);
    }

    if(hash)
    {
        HeapFree(GetProcessHeap(), 0, hash);
    }
    return resultString;
}
//
// Check if the developer set Flickr key
//
bool FlickrUploader::CheckForValidFlickrValues()
{
    if (wcsnlen_s(flickr_api_key, 32) == 0 || wcsnlen_s(flickr_secret, 16) == 0)
    {
        // Bring up message box to show warning
        ::TaskDialog(
            nullptr,
            nullptr,
            L"Developer Warning",
            L"IMPORTANT: To enable uploading to Flickr in this application, you'll need to provide your own values for these two static variables (flickr_api_key and flickr_secret).",
            L"View FlickerUploader.cpp for more information.",
            TDCBF_OK_BUTTON,
            TD_WARNING_ICON,
            nullptr);

        return false;
    }

    return true;
}
//
// Close service proxy and clean up
//
void FlickrUploader::CloseWebProxy (WS_HEAP** heap, WS_SERVICE_PROXY** proxy,  WS_ERROR** error)
{
    if (proxy != nullptr && *proxy != nullptr)
    {
        WsCloseServiceProxy(*proxy, nullptr, nullptr);

        WsFreeServiceProxy(*proxy);
    }

    if (heap != nullptr && *heap != nullptr)
    {
        WsFreeHeap(*heap);
    }

    if (error != nullptr && *error != nullptr)
    {
        WsFreeError(*error);
    }
}
