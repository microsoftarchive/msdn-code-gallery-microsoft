//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
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
    internal:
        static property Platform::String^ FEATURE_NAME 
        {
            Platform::String^ get() 
            {  
                return "Search Control C++ Sample"; 
            }
        }

        static property Platform::Array<Scenario>^ scenarios 
        {
            Platform::Array<Scenario>^ get() 
            { 
                return scenariosInner; 
            }
        }
		static Platform::String^ ReplaceUrlSearchTerms(Platform::String^ strUrl, Platform::String^ queryText);
		Windows::Foundation::EventRegistrationToken token; 

    private:
        static Platform::Array<Scenario>^ scenariosInner;
		
    };

    namespace SearchControl
    {
        // Sample specific types should be in this namespace
    }

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
