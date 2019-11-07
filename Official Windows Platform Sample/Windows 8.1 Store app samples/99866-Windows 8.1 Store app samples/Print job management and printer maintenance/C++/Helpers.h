//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Helpers.h
// Declaration of routines related to exceptions and other helpers.
//
#pragma once
namespace SDKSample
{
    namespace DeviceAppForPrinters2
    {
        /// <summary>
        /// Throws a Platform::Exception when provided with a failure HRESULT.
        /// </summary>
        inline void ThrowIfFailed(HRESULT hr)
        {
            if (FAILED(hr))
            {
                throw Platform::Exception::CreateException(hr);
            }
        }

        class ExceptionHelper
        {
        public:
            /// <summary>
            /// Converts exception details into a display string
            /// </summary>
            static Platform::String^ ToString(Platform::Exception^ e)
            {
                std::wstringstream ws;
                ws << L"Caught exception: 0x" << std::hex << e->HResult << L". Please debug the app";
                return ref new Platform::String(ws.str().c_str());
            }

            /// <summary>
            /// Converts exception details into a display string
            /// </summary>
            static Platform::String^ ToString(std::exception e)
            {
                return "Caught C++ exception. Please debug the app";
            }
        };

        /// <summary>
        /// RAII-style object that manages the memory allocation and deallocation of a BSTR instance.
        /// </summary>
        class AutoBSTR
        {
        public:
            AutoBSTR() 
                : string(nullptr)
            { }

            AutoBSTR(Platform::String^ string)
            {
                this->string = ::SysAllocString(string->Data());
                if (string == nullptr)
                {
                    throw ref new Platform::OutOfMemoryException("Cannot allocate BSTR");
                }
            }

            ~AutoBSTR()
            {
                if (string)
                {
                    ::SysFreeString(string);
                }
            }

            BSTR* Get()
            {
                return &string;
            }

            operator BSTR() const
            {
                return string;
            }

        private:
            BSTR string;
        };
    }
}