//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#pragma once

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
                return "JSON";
            }
        }

        static property Platform::Array<Scenario>^ scenarios 
        {
            Platform::Array<Scenario>^ get() 
            {
                return scenariosInner; 
            }
        }
    private:
        static Platform::Array<Scenario>^ scenariosInner;
    };

    namespace Json
    {
        // Sample specific types should be in this namespace
    }
}
