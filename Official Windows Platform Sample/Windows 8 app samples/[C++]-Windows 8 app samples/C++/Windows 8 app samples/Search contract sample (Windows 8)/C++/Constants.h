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
                return ref new Platform::String(L"Search contract C++ sample");
            }
        }

        static property Platform::Array<Scenario>^ scenarios
        {
            Platform::Array<Scenario>^ get()
            {
                return scenariosInner;
            }
        }

    internal:
        void ProcessQueryText(Platform::String^ queryText);
        static Platform::String^ ReplaceUrlSearchTerms(Platform::String^ strUrl, Platform::String^ queryText);
        static const int SearchPaneMaxSuggestions = 5;
        Windows::Foundation::EventRegistrationToken token;

    private:
        static Platform::Array<Scenario>^ scenariosInner;
    };

    class Finally
    {
    public:
        Finally(std::function<void()> f) : finallyClause(f) {}
        ~Finally()
        {
            finallyClause();
        }

    private:
        std::function<void()> finallyClause;
    };
}
