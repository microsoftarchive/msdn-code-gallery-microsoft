//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved


#pragma once

using namespace Windows::Foundation;

namespace Wrapper
{
    public value struct DspSettings
    {
        unsigned int percentOfScreen;
        BOOL isEnabled;
    };

    public ref class WinRTComponent sealed
    {
    public:
        WinRTComponent ();

        bool Initialize(Object^ o);

        void UpdateDsp(int percentOfScreen);
        void Enable();
        void Disable();
        DspSettings GetDspSetting();

    private:
        ~WinRTComponent ();
        Microsoft::WRL::ComPtr<IMft0> m_spImpl;
    };
}
// WinRTComponent.h
