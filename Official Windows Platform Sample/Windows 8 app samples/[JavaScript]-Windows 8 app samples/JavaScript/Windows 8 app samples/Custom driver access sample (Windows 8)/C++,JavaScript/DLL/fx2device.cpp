#include "fx2device.h"

Fx2Device::Fx2Device(_In_ IDeviceIoControl* DeviceControl, _In_ String^ Id) : m_DeviceIoControl(DeviceControl), 
                                                                              m_Id(Id) {
    OutputDebugString(L"Fx2 constructor invoked\n");
    return;
}

Fx2Device::~Fx2Device ()
{
    OutputDebugString(L"Fx2Device destructor invoked\n");
    return;
}

String^
Fx2Device::GetDeviceSelector()
{
    return L"System.Devices.InterfaceClassGuid:=\"{573E8C73-0CB4-4471-A1BF-FAB26C31D384}\" AND "
           L"System.Devices.InterfaceEnabled:=System.StructuredQueryType.Boolean#True";
}

Fx2Device^
Fx2Device::FromId(
    _In_ String^ InstancePath
    )
{
    ComPtr<ICreateDeviceAccessAsync> access;
    ComPtr<IDeviceIoControl> deviceControl;
    HRESULT hr;

    hr = CreateDeviceAccessInstance(InstancePath->Data(),
                                    GENERIC_READ | GENERIC_WRITE,
                                    &access);
    THROW_HR(hr);

    //
    // Wait on the open
    //

    hr = access->Wait(INFINITE);
    THROW_HR(hr);

    hr = access->GetResult(IID_PPV_ARGS(&deviceControl));
    THROW_HR(hr);

    OutputDebugStringW(L"Creating device\n");

    //
    // Create an instance of the class and initialize it.
    //
    return ref new Fx2Device(deviceControl.Get(), InstancePath);
}

String^
Fx2Device::Id::get(void) {
    return m_Id;
}

BYTE
Fx2Device::SevenSegmentDisplay::get(VOID) {

    BYTE ssValue = 0;
    ULONG numBytesReturned;
    HRESULT hr;

    hr = m_DeviceIoControl->DeviceIoControlSync(
            IOCTL_OSRUSBFX2_GET_7_SEGMENT_DISPLAY,
            NULL,
            0,
            &ssValue,
            sizeof(ssValue),
            &numBytesReturned
            );
    THROW_HR(hr);

    for(int i = 0; i < c_SegmentMaskCe; i += 1) {
        if (ssValue == c_SegmentMask[i]) {
            return (BYTE) i;
        }
    }

    throw ref new InvalidArgumentException(L"Seven segment display value is not between 0 and 9");
}

void
Fx2Device::SevenSegmentDisplay::set(
    _In_ BYTE Value
    )
{
    BYTE ssValue;
    HRESULT hr;
    DWORD bytesRead;

    if (Value >= c_SegmentMaskCe) {
        throw ref new InvalidArgumentException(L"Value must be between 0 and 9");
    }

    ssValue = c_SegmentMask[Value];

    hr = m_DeviceIoControl->DeviceIoControlSync(
        IOCTL_OSRUSBFX2_SET_7_SEGMENT_DISPLAY,
        &ssValue,
        sizeof(ssValue),
        NULL,
        0,
        &bytesRead
        );
    THROW_HR(hr);
}

Array<bool>^
Fx2Device::SwitchState::get(VOID) {

    BYTE ssValue = 0;
    ULONG numBytesReturned;
    HRESULT hr;

    hr = m_DeviceIoControl->DeviceIoControlSync(
            IOCTL_OSRUSBFX2_READ_SWITCHES,
            NULL,
            0,
            &ssValue,
            sizeof(ssValue),
            &numBytesReturned
            );
    THROW_HR(hr);

    Array<bool>^ result = ref new Array<bool>(c_NumSwitches);

    for(int i = 0; i < c_NumSwitches; i += 1) {
        result[i] = ((ssValue & (1 << i)) != 0);
    }

    return result;
}

void
Fx2Device::SetBarGraphDisplay(
    _In_ const Array<bool>^ Values
    )
{
    BAR_GRAPH_STATE state;
    int valuesCe = Values->Length;
    DWORD bytesRead;
    

    state.BarsAsUChar = 0;
    for(int i = 0; i < min(c_NumBarsInGraph, valuesCe); i += 1) {
        state.BarsAsUChar |= (Values[i] ? 1 : 0) << i;
    }

    HRESULT hr = m_DeviceIoControl->DeviceIoControlSync(
        IOCTL_OSRUSBFX2_SET_BAR_GRAPH_DISPLAY,
        (PBYTE) &state,
        sizeof(state),
        NULL,
        0,
        &bytesRead
        );

    THROW_HR(hr);
}

EventRegistrationToken 
Fx2Device::SwitchStateChanged::add(
    __in SwitchStateChangedHandler^ h
    ) 
{
    EventRegistrationToken t = m_SwitchStateChangedEvent::add(h);

    if (InterlockedIncrement(&m_SwitchState.ListenerCount) == 1) {
        m_SwitchState.Repeater.Start(
            [this](cancellation_token cancelToken, task_continuation_context& context) {
                
                // register for cancellation and abort the IOCTL if it happens
                auto cancelTokenRegistration = cancelToken.register_callback([=]() {
                    m_DeviceIoControl->CancelOperation(m_SwitchState.CancelContext);
                });

                // Create a task chain.  Send an ioctl to get the next switch
                // state change.  Chain a task to convert that into the switch
                // change event.
                auto op = 
                StartIoctl(m_DeviceIoControl.Get(), 
                            IOCTL_OSRUSBFX2_GET_INTERRUPT_MESSAGE,
                            NULL,
                            0,
                            (PBYTE) &m_SwitchState.Buffer,
                            sizeof(m_SwitchState.Buffer),
                            &m_SwitchState.CancelContext).
                then([=](DeviceControlResult dcr) {

                    bool abort = false;
                    SwitchStateChangedEventArgs^ e = nullptr;

                    // unregister for cancellation
                    cancelToken.deregister_callback(cancelTokenRegistration);

                    if (dcr.Result == HRESULT_FROM_WIN32(ERROR_OPERATION_ABORTED)) {
                        // fall through for cancellation
                        abort = true;
                    } else if (dcr.Result != S_OK) {
                        // Update the error count.  If it exceeds our limit then abort
                        // the reader.
                        m_SwitchState.FailureCount += 1;
                        if (m_SwitchState.FailureCount > 10) {
                            e = ref new SwitchStateChangedEventArgs(dcr.Result);
                            abort = true;
                        }
                    } else {
                        // Success.  Reset the failure count.
                        e = ref new SwitchStateChangedEventArgs(m_SwitchState.Buffer);
                        m_SwitchState.FailureCount = 0;
                    }

                    // Raise the event if appropriate.
                    if (e != nullptr) {
                        m_SwitchStateChangedEvent(this, e);
                    }

                    return !abort;
                }, context);    // Since this task raises an event back to javascript
                                // it must run in the context of the original call to 
                                // start the Repeater (which was from the UI thread)

                // Return the task chain.  The repeater will run this each 
                // time it starts an I/O operation, and takes care of starting
                // the next task
                return op;
            }
        );
    }
    return t;
}

void
Fx2Device::SwitchStateChanged::remove(
    __in EventRegistrationToken cookie
    )
{
    m_SwitchStateChangedEvent::remove(cookie);

    if (InterlockedDecrement(&m_SwitchState.ListenerCount) == 0) {
        m_SwitchState.Repeater.Stop();
    }
}

IAsyncOperation<BarGraphDisplayResult^>^ Fx2Device::GetBarGraphDisplayAsync(void) {
    
    // create an async operation object and return it to the caller.  The lambda
    // provided is the Start function for the operation, and returns a chain of tasks 
    // that eventually provides the BarGraphDisplayResult^ object to the caller.

    return create_async([this](cancellation_token cancelToken) {

        // create the final result object, which we use to store the response 
        // from the driver.
        BarGraphDisplayResult^ bgr = ref new BarGraphDisplayResult();

        // Hook up cancellation using the cancel_token.
        auto registrationToken = cancelToken.register_callback([this, bgr]() {
            ULONG_PTR t = *(bgr->GetCancelToken());
            if (t != 0) {
                m_DeviceIoControl->CancelOperation(t);
            }
        });

        // StartIoctl returns a task that completes when the IOCTL completes.
        // Attach a follow-on task to convert the IOCTL results into bar graph
        // results.
        auto taskChain = 
            StartIoctl(this->m_DeviceIoControl.Get(),
                       IOCTL_OSRUSBFX2_GET_BAR_GRAPH_DISPLAY,
                       nullptr,
                       0,
                       (PUCHAR) &(bgr->m_State),
                       sizeof(bgr->m_State),
                       bgr->GetCancelToken()).
            then([bgr,cancelToken,registrationToken](DeviceControlResult result) {
                cancelToken.deregister_callback(registrationToken);
                if ((result.Result == S_OK) && 
                    (result.BytesTransferred != sizeof(BAR_GRAPH_STATE))) {
                    result.Result = E_FAIL;
                }

                if (result.Result != S_OK) {
                    throw Platform::Exception::CreateException(result.Result, L"Error getting bar graph display");
                }

                bgr->UpdateBars();
                return bgr;
            });

        return taskChain;
    });
}
