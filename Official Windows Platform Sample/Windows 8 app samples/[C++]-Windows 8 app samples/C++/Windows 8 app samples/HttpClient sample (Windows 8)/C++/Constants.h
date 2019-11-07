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
                *uri = ref new Windows::Foundation::Uri(Helpers::Trim(uriString));
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

    class Helpers
    {
    public:
        static void ScenarioStarted(Windows::UI::Xaml::Controls::Button^ startButton, Windows::UI::Xaml::Controls::Button^ cancelButton)
        {
            startButton->IsEnabled = false;
            cancelButton->IsEnabled = true;
        }

        static void ScenarioCompleted(Windows::UI::Xaml::Controls::Button^ startButton, Windows::UI::Xaml::Controls::Button^ cancelButton)
        {
            startButton->IsEnabled = true;
            cancelButton->IsEnabled = false;
        }

        static Platform::String^ Trim(Platform::String^ s)
        {
            const char16* first = s->Begin();
            const char16* last = s->End();
        
            while (first != last && iswspace(*first))
            {
                ++first;
            }
        
            while (first != last && iswspace(last[-1]))
            {
                --last;
            }
        
            return ref new Platform::String(first, (int)(last - first));
        }

        static void ReplaceAll(std::wstring& value, _In_z_ const char16* from, _In_z_ const char16* to)
        {
            auto fromLength = wcslen(from);
            std::wstring::size_type offset;
            
            for (offset = value.find(from); offset != std::wstring::npos; offset = value.find(from, offset))
            {
                value.replace(offset, fromLength, to);
            }
        }

        enum NotifyType
        {
            StatusMessage,
            ErrorMessage
        };
    };
}
