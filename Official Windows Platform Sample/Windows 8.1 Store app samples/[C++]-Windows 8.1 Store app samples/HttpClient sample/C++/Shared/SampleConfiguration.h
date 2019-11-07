//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#pragma once

#include <collection.h>
#include <Helpers.h>

namespace SDKSample
{
    public value struct Scenario
    {
        Platform::String^ Title;
        Platform::String^ ClassName;
    };

    partial ref class MainPage
    {
    public:
        static property Platform::String^ FEATURE_NAME 
        {
            Platform::String^ get() 
            {  
                return ref new Platform::String(L"HttpClient"); 
            }
        }

        static property Platform::Array<Scenario>^ scenarios 
        {
            Platform::Array<Scenario>^ get() 
            { 
                return scenariosInner; 
            }
        }

        bool TryGetUri(Platform::String^ uriString, Windows::Foundation::Uri^* uri)
        {
            // Create a Uri instance and catch exceptions related to invalid input. This method returns 'true'
            // if the Uri instance was successfully created and 'false' otherwise.
            try
            {
                Windows::Foundation::Uri^ localUri = ref new Windows::Foundation::Uri(SDKSample::HttpClientSample::Helpers::Trim(uriString));

                if (localUri->Host == nullptr)
                {
                    NotifyUser("Error: Empty host is not supported.", NotifyType::ErrorMessage);
                    return false;
                }

                if (localUri->SchemeName != "http" && localUri->SchemeName != "https")
                {
                    NotifyUser("Error: Only 'http' and 'https' schemes supported.", NotifyType::ErrorMessage);
                    return false;
                }

                *uri = localUri;
                return true;
            }
            catch (Platform::NullReferenceException^ exception)
            {
                NotifyUser("Error: URI must not be null or empty.", NotifyType::ErrorMessage);
            }
            catch (Platform::InvalidArgumentException^ exception)
            {
                NotifyUser("Error: Invalid URI", NotifyType::ErrorMessage);
            }

            return false;
        }

    private:
        static Platform::Array<Scenario>^ scenariosInner;
    };
}
