#include <strsafe.h>
#include <assert.h>

#pragma once

///<summary>
///Printf to the debugger
///</summary>
inline void DbgPrint(PCWSTR FormatString, ...) {
	WCHAR buffer[1024];
	static ULONG line = 0;
	va_list arglist;
	va_start(arglist, FormatString);

	StringCbPrintf(buffer, sizeof(buffer), L"%03d:", line++);
	OutputDebugString(buffer);

	StringCbVPrintf(buffer, sizeof(buffer), FormatString, arglist);
	OutputDebugString(buffer);
}

namespace Samples {
    namespace Devices {

        [uuid("999BAD24-9ACD-45BB-8669-2A2FC0288B04")]
        delegate void OnIoComplete(HRESULT Result, DWORD BytesReturned);

        ///<summary>wrapper class for the result of an IOCTL</summary>
        struct DeviceControlResult {
            HRESULT Result;
            ULONG BytesTransferred;

            DeviceControlResult() : Result(E_NOT_SET),
                                    BytesTransferred(0)
            {
                return;
            }

            DeviceControlResult(HRESULT Hr, DWORD Cb) : Result(Hr), 
                                                        BytesTransferred(Cb) 
            {
                return;
            }
        };

		///<summary>
		///Starts the requested IOCTL
		///</summary>
		///<returns>returns a task which will complete when the IOCTL completes</returns>
        inline
        concurrency::task<DeviceControlResult>
        StartIoctl(
            _In_ IDeviceIoControl* Device,
            _In_ ULONG DeviceControlCode,
            _In_reads_bytes_opt_(InputBufferCb) PUCHAR InputBuffer,
            _In_ ULONG InputBufferCb,
            _Out_writes_bytes_opt_(OutputBufferCb) PUCHAR OutputBuffer,
            _In_ ULONG OutputBufferCb,
            _Out_opt_ ULONG_PTR* CancelToken = NULL
            )
        {
            concurrency::task_completion_event<DeviceControlResult> tce;
            concurrency::task<DeviceControlResult> t(tce);

            auto callback = ref new OnIoComplete(
                [tce](HRESULT Hr, DWORD Cb) {
                    tce.set(DeviceControlResult(Hr, Cb));
                }
            );

            auto cb2 = (IDeviceRequestCompletionCallback*) callback;

            HRESULT hr= Device->DeviceIoControlAsync(
                                    DeviceControlCode,
                                    InputBuffer,
                                    InputBufferCb,
                                    OutputBuffer,
                                    OutputBufferCb,
                                    cb2,
                                    CancelToken 
                                    );

            if (hr != S_OK) {
                tce.set(DeviceControlResult(hr, 0));
            }

            return t;
        }
    }
}
