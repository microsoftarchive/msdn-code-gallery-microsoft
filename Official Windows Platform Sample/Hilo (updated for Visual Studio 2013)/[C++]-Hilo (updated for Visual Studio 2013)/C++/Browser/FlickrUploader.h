//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#pragma once
class FlickrUploader
{
public:
    FlickrUploader(void);
    ~FlickrUploader(void);

    std::wstring Connect();
    std::wstring GetToken(const std::wstring& frob);
    std::wstring UploadPhotos(const std::wstring& token, const std::wstring& fileName,  bool* errorFound);
    bool CheckForValidFlickrValues();

private:
    std::wstring ObtainFrob();
    std::wstring CreateLoginLink(const std::wstring& frob);
    HRESULT CreateWebProxy (WS_HEAP** heap, WS_SERVICE_PROXY** proxy,  WS_ERROR** error);
    void CloseWebProxy (WS_HEAP** heap, WS_SERVICE_PROXY** proxy,  WS_ERROR** error);
    HRESULT ConnectWebService(const std::wstring& params);
    int SendWebRequest(const HINTERNET *request, const std::wstring& token, const std::wstring& fileName);
    std::wstring GetPhotoId(const HINTERNET *request, bool* errorFound);
    std::wstring GetXmlElementValueByName(const std::wstring& xmlContent, const std::wstring& elementName, bool* errorFound);
    std::wstring CalculateMD5Hash(const std::wstring& buffer);
};
