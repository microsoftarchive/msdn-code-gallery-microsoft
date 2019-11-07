#pragma once

#include <specstrings.h>
#include <deviceaccess.h>
#include <winioctl.h>
#include <strsafe.h>

#include <wrl\client.h>

// #include "ppltasks_mod2.h"
#include <ppltasks.h>

#include <functional>

#include "public.h"
#include "support.h"
#include "repeater.h"

using namespace Windows::Foundation;
using namespace Platform;
using namespace Microsoft::WRL;
using namespace concurrency;

const BYTE c_NumBarsInGraph = 8;
const BYTE c_NumSwitches = 8;
const BYTE c_SegmentMask[] = {
    0xD7, // 0
    0x06, // 1
    0xB3, // 2
    0xA7, // 3
    0x66, // 4
    0xE5, // 5
    0xF4, // 6
    0x07, // 7
    0xF7, // 8
    0x67, // 9
};
const unsigned int c_SegmentMaskCe = _countof(c_SegmentMask);

inline void THROW_HR(HRESULT hr) 
{
    if (hr != 0) {
        throw Platform::Exception::ReCreateException(hr);  
    }
}

namespace Samples {
    namespace Devices {
        namespace Fx2 {

            ref class Fx2Device;
            ref class SwitchStateChangedEventArgs;
            ref class BarGraphDisplayResult;

            public delegate void SwitchStateChangedHandler(Fx2Device^ Sender, SwitchStateChangedEventArgs^ EventArgs);

            /// <summary>
            /// The device projection for an OSR Fx2 device
            /// </summary>
            public ref class Fx2Device sealed
            {

            private:
                String^ m_Id;

                ComPtr<IDeviceIoControl> m_DeviceIoControl;
                
                //
                // Fields to track the switch state of the fx2 board.
                //
                struct {
                    LONG ListenerCount;
                    Repeater Repeater;
                
                    SWITCH_STATE Buffer;
                    ULONG_PTR CancelContext;
                    ULONG FailureCount;
                } m_SwitchState;
                event SwitchStateChangedHandler^ m_SwitchStateChangedEvent;

                Fx2Device(__in IDeviceIoControl* DeviceControl, __in String^ Id);
                ~Fx2Device ();

                void StartEventLoop();
                void StopEventLoop();

            public:

                /// <summary>Returns the device selector that an app should use to locate Fx2 devices</summary>
                /// <returns>The selector string for the Fx2 device interface</returns>
                static String^ GetDeviceSelector();

                static Fx2Device^ FromId(__in String^ InterfacePath);

                property String^ Id {String^ get();}
                property BYTE SevenSegmentDisplay {BYTE get(); void set(__in BYTE value);}

                void SetBarGraphDisplay(__in const Array<bool>^ Values);
                IAsyncOperation<BarGraphDisplayResult^>^ GetBarGraphDisplayAsync(void);

                property Array<bool>^ SwitchState {Array<bool>^ get();}

                event SwitchStateChangedHandler^ SwitchStateChanged {
                    EventRegistrationToken add(__in SwitchStateChangedHandler^ h);
                    void remove(__in EventRegistrationToken cookie);
                    void raise(__in Fx2Device^ Sender, __in SwitchStateChangedEventArgs^ args) {m_SwitchStateChangedEvent::raise(Sender, args);}
                };
            };

            public ref class SwitchStateChangedEventArgs sealed {
                friend ref class Fx2Device;

            private:
                HRESULT m_Hr;
                Array<bool>^ m_Switches;
                SWITCH_STATE m_State;

                SwitchStateChangedEventArgs(__in HRESULT Hr) {
                    m_Hr = Hr;
                    m_Switches = nullptr;
                    ZeroMemory(&m_State, sizeof(m_State));
                }

                SwitchStateChangedEventArgs(__in SWITCH_STATE State) {
                    m_Hr = S_OK;
                    m_Switches = ref new Array<bool>(c_NumSwitches);
                    m_State = State;
                    for(int i = 0; i < c_NumSwitches; i += 1) {
                        m_Switches[i] = ((State.SwitchesAsUChar & (1 << i)) != 0);
                    }
                }

                void CheckResult(void) {
                    if (m_Hr != S_OK) {
                        throw Platform::Exception::CreateException(m_Hr, "Error requesting switch state");
                    }
                }

            public:
                
                property BYTE AsByte {BYTE get() {CheckResult(); return m_State.SwitchesAsUChar;}};
                property Array<bool>^ SwitchState {Array<bool>^ get() {CheckResult(); return m_Switches;}};
            };

            public ref class BarGraphDisplayResult sealed { 
                friend ref class Fx2Device;

            private:
                Array<bool>^ m_Bars;
                BAR_GRAPH_STATE m_State;
                ULONG_PTR m_CancelToken;

                void UpdateBars() {
                    for(int i = 0; i < c_NumBarsInGraph; i += 1) {
                        m_Bars[i] = ((m_State.BarsAsUChar & (1 << i)) != 0);
                    }
                }

            public:
                BarGraphDisplayResult() : m_CancelToken(0) {
                    m_Bars = ref new Array<bool>(c_NumBarsInGraph);
                    ZeroMemory(&m_State, sizeof(m_State));
                }

                property BYTE AsByte {BYTE get() {return m_State.BarsAsUChar;}};
                property Array<bool>^ BarGraphDisplay {Array<bool>^ get() {return m_Bars;}}

            internal:
                ULONG_PTR* GetCancelToken(void) {return &m_CancelToken;}
            };
        }
    }
}
using namespace Samples::Devices;
using namespace Samples::Devices::Fx2;
