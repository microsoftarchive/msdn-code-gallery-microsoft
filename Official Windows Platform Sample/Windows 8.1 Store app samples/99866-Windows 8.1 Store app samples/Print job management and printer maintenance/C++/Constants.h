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
                return "Device App For Printers 2"; 
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

    namespace DeviceAppForPrinters2
    {
        ref class DisplayStrings
        {
        public:
            static property Platform::String^ NO_PRINTER_ENUMERATED
            {
                Platform::String^ get()
                {
                    return
                        "No printers were enumerated. \r\n" \
                        "Please ensure that you have the appropriate device metadata staged in the system's local " \
                        "metadata store.\r\n" \
                        "Device metadata may be authored and staged using the device metadata authoring wizard\r\n" \
                        "http://msdn.microsoft.com/en-us/library/windows/hardware/hh454213(v=vs.85).aspx";
                }
            }

            static property Platform::String^ PRINTERS_ENUMERATING
            {
                Platform::String^ get()
                {
                    return "Enumerating printers. Please wait";
                }
            }

            static property Platform::String^ PRINTERS_ENUMERATED
            {
                Platform::String^ get()
                {
                    return "Printers enumerated. Please select a printer to proceed";
                }
            }

            static property Platform::String^ ENUMERATE_PRINTERS_TO_CONTINUE
            {
                Platform::String^ get()
                {
                    return "Please enumerate printers to contiue";
                }
            }

            static property Platform::String^ BIDI_RESPONSE_RECEIVED
            {
                Platform::String^ get()
                {
                    return "Bidi response received";
                }
            }
        };
    }
}
