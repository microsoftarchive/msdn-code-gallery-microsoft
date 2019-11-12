//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "SerialPort.h"

using namespace SDKSample;
using namespace SDKSample::UsbCdcControl;
using namespace Windows::Devices::Usb;

Windows::Foundation::IAsyncOperation<UsbSerialPort^>^ UsbSerialPort::Create(
        Windows::Devices::Usb::UsbDevice^ device
    )
{
    auto that = ref new UsbSerialPort(device);
    return Concurrency::create_async([that]() -> Concurrency::task<UsbSerialPort^>
    {
        return that->TryInitialize().then([that](Concurrency::task<bool> task)
        {
            if (task.get())
            {
                return that;
            }
            else
            {
                return (UsbSerialPort^)nullptr;
            }
        });
    });
}

UsbSerialPort::UsbSerialPort(
        Windows::Devices::Usb::UsbDevice^ device
    )
{
    this->device = device;

    this->dtrEnable = false;
    this->rtsEnable = false;

    this->readTimeout = Constants::InfiniteTimeout;

    this->cdcControl = nullptr;
    this->cdcData = nullptr;
}

Concurrency::task<bool> UsbSerialPort::TryInitialize()
{
    return Concurrency::create_task([this]()
    {
        auto device = this->device;

        Windows::Devices::Usb::UsbInterface^ cdcControl = nullptr;
        Windows::Devices::Usb::UsbInterface^ cdcData = nullptr;
        std::vector<Concurrency::task<Windows::Devices::Usb::UsbInterface^>> getCdcControlTasks;

        for (unsigned int i = 0; i < device->Configuration->UsbInterfaces->Size; i++)
        {
            auto interf = device->Configuration->UsbInterfaces->GetAt(i);
            if (!interf)
            {
                continue;
            }

            for (unsigned int j = 0; j < interf->InterfaceSettings->Size; j ++)
            {
                auto setting = interf->InterfaceSettings->GetAt(j);
                const auto index = setting->InterfaceDescriptor->InterfaceNumber;
                const auto interfaceClass = setting->InterfaceDescriptor->ClassCode;

                if (interfaceClass == InterfaceClass::CdcControl)
                {
                    cdcControl = interf;
                }
                else if (interfaceClass == InterfaceClass::CdcData)
                {
                    cdcData = interf;
                }
                else if (interfaceClass == InterfaceClass::VendorSpecific)
                {
                    if (cdcControl == nullptr)
                    {
                        // Test SetLineCoding
                        auto getCdcControlTask = this->SetLineCoding(index, Constants::DteRateTestValue, (BYTE)StopBits::One, (BYTE)Parity::None, Constants::DataBitsTestValue).then([this, index, interf](Concurrency::task<unsigned int> task)
                        {
                            unsigned int written = 0;
                            try
                            {
                                written = task.get();
                            }
                            catch (Platform::COMException^ exception)
                            {
                                Concurrency::cancel_current_task();
                            }

                            // Test GetLineCoding
                            return this->GetLineCoding(index).then([interf](Concurrency::task<Windows::Storage::Streams::IBuffer^> task)
                            {
                                Windows::Storage::Streams::IBuffer^ gotBuffer = nullptr;
                                try
                                {
                                    gotBuffer = task.get();
                                }
                                catch (Platform::COMException^ exception)
                                {
                                    Concurrency::cancel_current_task();
                                }

                                if (gotBuffer != nullptr && gotBuffer->Length == Constants::ExpectedResultGetLineCoding)
                                {
                                    Microsoft::WRL::ComPtr<Windows::Storage::Streams::IBufferByteAccess> pBufferByteAccess;
                                    Microsoft::WRL::ComPtr<IUnknown> pBuffer((IUnknown*)gotBuffer); 
                                    pBuffer.As(&pBufferByteAccess);
                                    BYTE* szGotControlBuffer = nullptr;
                                    pBufferByteAccess->Buffer(&szGotControlBuffer);
                                    DWORD dwBPS = !szGotControlBuffer ? MAXDWORD : *(LPDWORD)szGotControlBuffer;
                                    if (dwBPS == Constants::DteRateTestValue)
                                    {
                                        // This could be a CdcControl.
                                        return interf;
                                    }
                                }
                                return (Windows::Devices::Usb::UsbInterface^)nullptr;
                            });
                        });

                        getCdcControlTasks.push_back(getCdcControlTask);
                    }

                    if (!cdcData)
                    {
                        if (interf->BulkInPipes->Size > 0 && interf->BulkOutPipes->Size > 0)
                        {
                            // This could be a CdcData.
                            cdcData = interf;
                        }
                    }
                }
            }
        }

        return Concurrency::when_all(begin(getCdcControlTasks), end(getCdcControlTasks)).then([this, cdcControl, cdcData, getCdcControlTasks](Concurrency::task<std::vector<Windows::Devices::Usb::UsbInterface^>> tasks)
        {
            if (getCdcControlTasks.size() <= 0)
            {
                // This when_all task has to fail.
                this->cdcControl = cdcControl;
            }
            else
            {
                try
                {
                    this->cdcControl = tasks.get()[0];
                }
                catch(const Concurrency::task_canceled &)
                {
                    this->cdcControl = nullptr;
                }
            }
            this->cdcData = cdcData;

            if (!!this->cdcControl && !!this->cdcData)
            {
                // Flush the pipes.
                for (unsigned int in = 0; in < this->cdcData->BulkInPipes->Size; in++)
                {
                    this->cdcData->BulkInPipes->GetAt(in)->FlushBuffer();
                }
                return true;
            }
            else
            {
                return false;
            }
        });
    });
}

Windows::Foundation::IAsyncOperation<Windows::Foundation::HResult>^ UsbSerialPort::Open(
        int baudRate,
        Parity parity,
        int dataBits,
        StopBits stopBits
    )
{
    this->baudRate  = baudRate;
    this->parity    = parity;
    this->dataBits  = dataBits;
    this->stopBits  = stopBits;

    return Concurrency::create_async([this]() -> Concurrency::task<Windows::Foundation::HResult>
    {
        return Concurrency::create_task([this]()
        {
            if (this->cdcControl != nullptr)
            {
                // Do SetLineCoding
                return this->SetLineCoding(this->cdcControl->InterfaceNumber, this->baudRate, (BYTE)this->stopBits, (BYTE)this->parity, this->dataBits);
            }
            return Concurrency::task<unsigned int>([](){ return 0; });
        }
        ).then([this](unsigned int len)
        {
            if (len != Constants::ExpectedResultSetLineCoding)
            {
                return concurrency::create_task([]()
                {
                    Windows::Foundation::HResult hr;
                    hr.Value = E_NOINTERFACE;
                    return hr;
                });
            }

            // Do SetControlLineState
            return concurrency::create_task(this->SetControlLineState(this->cdcControl->InterfaceNumber)).then([](unsigned int)
            {
                Windows::Foundation::HResult hr;
                hr.Value = S_OK;
                return hr;
            });
        });
    });
}

Windows::Foundation::IAsyncOperation<unsigned int>^ UsbSerialPort::SetControlRequest(
        BYTE request,
        SHORT value,
        Windows::Storage::Streams::IBuffer^ buffer
    )
{
    return Concurrency::create_async([this, request, value, buffer]() -> Concurrency::task<unsigned int>
    {
        auto requestType = ref new UsbControlRequestType();
        requestType->AsByte = RequestType::Set;
        return this->UsbControlRequestForSet(this->cdcControl->InterfaceNumber, requestType, request, value, buffer);
    });
}

Windows::Foundation::IAsyncOperation<int>^ UsbSerialPort::Read(
        Windows::Storage::Streams::IBuffer^ buffer, 
        int offset, 
        int count
    )
{
    return Concurrency::create_async([this, buffer, offset, count](Concurrency::cancellation_token ct)
    {
        return this->ReadInternal(buffer, offset, count, this->readTimeout, ct).then([buffer](int readcount)
        {
            buffer->Length = readcount;
            return readcount;
        });
    });
}

Concurrency::task<int> UsbSerialPort::ReadInternal(
        Windows::Storage::Streams::IBuffer^ buffer,
        int offset_, 
        int count_,
        int timeout_,
        Concurrency::cancellation_token ct
    )
{
    // Use task_continuation_context::use_arbitrary to make the thread in background.
    return Concurrency::create_task([this, buffer, offset_, count_, timeout_, ct](){}).then([this, buffer, offset_, count_, timeout_, ct](Concurrency::task<void>)
    {
        int offset = offset_;
        int count = count_;
        int timeout = timeout_;
        int totalReadcount = 0;

        while (timeout == Constants::InfiniteTimeout || timeout >= 0)
        {
            CONST ULONGLONG ullStart = GetTickCount64();
            auto readPartialTask = this->ReadPartial(buffer, offset, count, timeout, ct);

            // Call wait() in a background thread so that it won't block UI thread.
            readPartialTask.wait();
            const int readcount = readPartialTask.get();

            // Timeout.
            if (readcount == UsbSerialPort::ReadPartialResultTimeout)
            {
                break;
            }
            // Cancel.
            else if (readcount == UsbSerialPort::ReadPartialResultCanceled)
            {
                Concurrency::cancel_current_task();
            }

            totalReadcount += readcount;

            if (readcount >= count)
            {
                break; // completed;
            }

            // Prepare for the next ReadPartial.
            if (timeout >= 0)
            {
                timeout = timeout - (int)(GetTickCount64() - ullStart);
            }
            offset += readcount;
            count  -= readcount;
        }
        return totalReadcount;
    }
    , Concurrency::task_continuation_context::use_arbitrary());
}

/// <summary>
/// Creates a task that completes after the specified delay.
/// </summary>
/// <param name="timeout">a specified delay by millisecond unit.</param>
/// <param name="cancel">a cancellation_token to cancel this timeout task.</param>
Concurrency::task<void> timeoutTask(unsigned int timeout, Concurrency::cancellation_token cancel)
{
    // A task completion event that is set when a timer fires.
    Concurrency::task_completion_event<void> tce;

    // Create a non-repeating timer.
    auto fire_once = new Concurrency::timer<int>(timeout, 0, nullptr, false);

    // Create a call object that sets the completion event after the timer fires.
    auto callback = new Concurrency::call<int>([tce](int)
    {
        tce.set();
    });

    // Connect the timer to the callback and start the timer.
    fire_once->link_target(callback);
    fire_once->start();

    // cancellation_token also can set the completion event for cleanup.
    auto cookie = cancel.register_callback([tce]()
    {
        tce.set();
    });

    // Create a task that completes after the completion event is set.
    Concurrency::task<void> event_set(tce);

    // Create a continuation task that cleans up resources and 
    // and return that continuation task. 
    return event_set.then([fire_once, callback, cancel, cookie](Concurrency::task<void> task)
    {
        fire_once->stop();
        delete callback;
        delete fire_once;
        cancel.deregister_callback(cookie);
    });
}

/// <summary>
/// Creates a task that completes by user's cancellation.
/// </summary>
/// <param name="cancelByUser">a cancellation_token to notify of a user's cancellation.</param>
/// <param name="cancelThisTask">a cancellation_token to cancel this task.</param>
Concurrency::task<void> cancelTask(Concurrency::cancellation_token cancelByUser, Concurrency::cancellation_token cancelThisTask)
{
    // A task completion event that is set when the cancellation_token fires.
    Concurrency::task_completion_event<void> tce;

    // Register a cancel callback that sets the completion event when a user's cancellation is fired.
    auto cookieCancelByUser = cancelByUser.register_callback([tce]()
    {
        tce.set();
    });

    // Another cancellation_token also can set the completion event for cleanup.
    auto cookieCancelThisTask = cancelThisTask.register_callback([tce]()
    {
        tce.set();
    });

    // Create a task that completes after the completion event is set.
    Concurrency::task<void> event_set(tce);

    // Create a continuation task that cleans up resources and 
    // and return that continuation task. 
    return event_set.then([cancelByUser, cancelThisTask, cookieCancelByUser, cookieCancelThisTask]()
    {
        // Deregister the cancel callback.
        cancelByUser.deregister_callback(cookieCancelByUser);
        cancelThisTask.deregister_callback(cookieCancelThisTask);
    });
}

Concurrency::task<int> UsbSerialPort::ReadPartial(
        Windows::Storage::Streams::IBuffer^ buffer,
        int offset, 
        int count,
        int timeout,
        Concurrency::cancellation_token ct
    )
{
    // Buffer check.
    if ((int)buffer->Length < (offset + count))
    {
        return concurrency::create_task([](){ return UsbSerialPort::ReadPartialResultInvalidArgument; });
    }

    auto inputStream = this->cdcData->BulkInPipes->GetAt(0)->InputStream;
    auto reader = ref new Windows::Storage::Streams::DataReader(inputStream);

    concurrency::cancellation_token_source cancellationTokenSource;

    // LoadAsync task.
    auto loadAsyncTask = Concurrency::create_task(reader->LoadAsync(count), cancellationTokenSource.get_token()).then([](concurrency::task<unsigned int> task)
    {
        unsigned int loaded = 0;
        try
        {
            loaded = task.get();
        }
        catch (Platform::COMException^ exception)
        {
            // Device removed.
            if (exception->HResult == HRESULT_FROM_WIN32(ERROR_GEN_FAILURE))
            {
                concurrency::cancel_current_task();
            }
            // LoadAsync operation aborted.
            else if (exception->HResult == HRESULT_FROM_WIN32(ERROR_OPERATION_ABORTED))
            {
                concurrency::cancel_current_task();
            }
            else
            {
                throw;
            }
        }
        return (int)loaded;
    });

    // Timeout task.
    unsigned int timeoutvalue = timeout == Constants::InfiniteTimeout ? Concurrency::COOPERATIVE_TIMEOUT_INFINITE : timeout;
    auto timeoutTask = ::timeoutTask(timeoutvalue, cancellationTokenSource.get_token()).then([]()
    {
        return UsbSerialPort::ReadPartialResultTimeout;
    });

    // Cancel task.
    auto cancelTask = ::cancelTask(ct, cancellationTokenSource.get_token()).then([]()
    {
        return UsbSerialPort::ReadPartialResultCanceled;
    });

    return (loadAsyncTask || timeoutTask || cancelTask).then([buffer, offset, cancellationTokenSource, reader](int result)
    {
        cancellationTokenSource.cancel(); // cancel all incomplete tasks.

        const int loadedCount = result;

        if (loadedCount > 0)
        {
            auto readBuffer = reader->ReadBuffer(loadedCount);
            Microsoft::WRL::ComPtr<Windows::Storage::Streams::IBufferByteAccess> pBufferByteAccessSrc;
            Microsoft::WRL::ComPtr<IUnknown> pBufferSrc((IUnknown*)readBuffer); 
            pBufferSrc.As(&pBufferByteAccessSrc);
            BYTE* srcBuffer = nullptr;
            pBufferByteAccessSrc->Buffer(&srcBuffer);

            Microsoft::WRL::ComPtr<Windows::Storage::Streams::IBufferByteAccess> pBufferByteAccessDst;
            Microsoft::WRL::ComPtr<IUnknown> pBufferDst((IUnknown*)buffer); 
            pBufferDst.As(&pBufferByteAccessDst);
            BYTE* dstBuffer = nullptr;
            pBufferByteAccessDst->Buffer(&dstBuffer);

            if ((!!srcBuffer) && (!!dstBuffer))
            {
                CopyMemory(dstBuffer + offset, srcBuffer, loadedCount);
            }
        }

        return loadedCount;
    });
}

Windows::Foundation::IAsyncOperation<Windows::Foundation::HResult>^ UsbSerialPort::Write(
        Windows::Storage::Streams::IBuffer^ buffer,
        int offset,
        int count
    )
{
    auto outputStream = this->cdcData->BulkOutPipes->GetAt(0)->OutputStream;
    auto writer = ref new Windows::Storage::Streams::DataWriter(outputStream);

    return Concurrency::create_async([buffer, offset, count, writer]()->Concurrency::task<Windows::Foundation::HResult>
    {
        // Overflow check.
        if ((int)buffer->Length < (offset + count))
        {
            return Concurrency::create_task([]()
            {
                Windows::Foundation::HResult hr;
                hr.Value = E_INVALIDARG;
                return hr;
            });
        }

        writer->WriteBuffer(buffer, offset, count);

        return Concurrency::create_task(writer->StoreAsync()).then([](unsigned int written)
        {
            Windows::Foundation::HResult hr;
            hr.Value = S_OK;
            return hr;
        });
    });
}

bool UsbSerialPort::DtrEnable_get()
{
    return this->dtrEnable;
}

Windows::Foundation::IAsyncAction^ UsbSerialPort::DtrEnable_set(bool value)
{
    return Concurrency::create_async([this, value]()
    {
        this->dtrEnable = value;
        return Concurrency::create_task(this->SetControlLineState(this->cdcControl->InterfaceNumber)).then([](unsigned int count)
        {
            return;
        });
    });
}

bool UsbSerialPort::RtsEnable_get()
{
    return this->rtsEnable;
}

Windows::Foundation::IAsyncAction^ UsbSerialPort::RtsEnable_set(bool value)
{
    return Concurrency::create_async([this, value]()
    {
        this->rtsEnable = value;
        return Concurrency::create_task(this->SetControlLineState(this->cdcControl->InterfaceNumber)).then([](unsigned int count)
        {
            return;
        });
    });
}

int UsbSerialPort::ReadTimeout::get ()
{
    return this->readTimeout;
}
void UsbSerialPort::ReadTimeout::set (int value)
{
    this->readTimeout = value;
}

Concurrency::task<unsigned int> UsbSerialPort::UsbControlRequestForSet(
        unsigned int index,
        UsbControlRequestType^ requestType,
        BYTE request,
        SHORT value,
        Windows::Storage::Streams::IBuffer^ buffer
    )
{
    auto packet = ref new UsbSetupPacket();
    packet->RequestType = requestType;
    packet->Request = request;
    packet->Value = value;
    packet->Length = buffer ? buffer->Length : 0;
    packet->Index = index;

    return concurrency::create_task(this->device->SendControlOutTransferAsync(packet, buffer));
}

Concurrency::task<Windows::Storage::Streams::IBuffer^> UsbSerialPort::GetLineCoding(unsigned int index)
{
    auto buffer = ref new Windows::Storage::Streams::Buffer(Constants::ExpectedResultGetLineCoding);
    buffer->Length = Constants::ExpectedResultGetLineCoding;

    auto requestType = ref new UsbControlRequestType();
    requestType->AsByte = RequestType::Get;

    auto packet = ref new UsbSetupPacket();
    packet->RequestType = requestType;
    packet->Request = RequestCode::GetLineCoding;
    packet->Value = 0;
    packet->Length = buffer->Length;
    packet->Index = index;
            
    return concurrency::create_task(this->device->SendControlInTransferAsync(packet, buffer));
}

Concurrency::task<unsigned int> UsbSerialPort::SetLineCoding(
        unsigned int index,
        DWORD dteRate,
        BYTE charFormat,
        BYTE parityType,
        BYTE dataBits
    )
{
    // SetLineCoding
    auto writer = ref new Windows::Storage::Streams::DataWriter();
    writer->ByteOrder = Windows::Storage::Streams::ByteOrder::LittleEndian;
    writer->WriteUInt32(dteRate);
    writer->WriteByte(charFormat);
    writer->WriteByte(parityType);
    writer->WriteByte(dataBits);
    auto buffer = writer->DetachBuffer();

    auto requestType = ref new UsbControlRequestType();
    requestType->AsByte = RequestType::Set;

    return
        this->UsbControlRequestForSet(
            index,
            requestType,
            RequestCode::SetLineCoding,
            0,
            buffer);
}

Concurrency::task<unsigned int> UsbSerialPort::SetControlLineState(
        unsigned int index
    )
{
    // SetControlLineState

    auto requestType = ref new UsbControlRequestType();
    requestType->AsByte = RequestType::Set;

    unsigned int value = (this->rtsEnable ? 1 : 0) << 1 | (this->dtrEnable ? 1 : 0);

    return
        this->UsbControlRequestForSet(
            index,
            requestType,
            RequestCode::SetControlLineState,
            value,
            nullptr);
}