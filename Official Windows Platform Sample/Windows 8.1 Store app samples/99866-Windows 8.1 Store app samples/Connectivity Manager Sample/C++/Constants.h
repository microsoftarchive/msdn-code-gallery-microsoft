//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#pragma once
using namespace Windows::Networking::Connectivity;

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
        property ConnectionSession^ connectionSession
        {
            void set(ConnectionSession^ session)
            {
                _connectionSession = session;
            }
            ConnectionSession^ get()
            {
                return _connectionSession;
            }
        }
    internal:
        static property Platform::String^ FEATURE_NAME 
        {
            Platform::String^ get() 
            {  
                return "ConnectivityManager"; 
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
        ConnectionSession^ _connectionSession;
    };	
}
