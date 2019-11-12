/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
#include "BackEndTransport.h"
#include "BackEndNativeBuffer.h"
#include <ppltasks.h>

using namespace PhoneVoIPApp::BackEnd;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Storage::Streams;
using namespace Windows::System::Threading;
using namespace Microsoft::WRL;
using namespace Microsoft::WRL::Details;

BackEndTransport::BackEndTransport()
{
}


void BackEndTransport::WriteAudio(BYTE* bytes, int byteCount)
{
    Write(bytes, byteCount, TransportMessageType::Audio, 0, 0);
}

void BackEndTransport::WriteVideo(BYTE* bytes, int byteCount, UINT64 hnsPresenationTime, UINT64 hnsSampleDuration)

{
    Write(bytes, byteCount, TransportMessageType::Video, hnsPresenationTime, hnsSampleDuration);
}

void BackEndTransport::Write(BYTE* bytes, int byteCount, TransportMessageType::Value dataType, UINT64 hnsPresenationTime, UINT64 hnsSampleDuration)
{

    static const int MaxPacketSize = 10*1024*1024;

    int bytesToSend = byteCount;

    while (bytesToSend)
    {
        int chunkSize = bytesToSend > MaxPacketSize ? MaxPacketSize : bytesToSend;
        ComPtr<NativeBuffer> spNativeBuffer = NULL;
        if (dataType == TransportMessageType::Audio)
        {
            MakeAndInitialize<NativeBuffer>(&spNativeBuffer, bytes, chunkSize, FALSE);
            AudioMessageReceived(NativeBuffer::GetIBufferFromNativeBuffer(spNativeBuffer), hnsPresenationTime, hnsSampleDuration);
        }
        else
        {
            // Temporarily duplicating this for sample so that MSS can own this
            // buffer, and will be released when the stream itself is released
            BYTE* pMem = new BYTE[chunkSize];
                
            memcpy((void*) pMem, (void*) bytes, chunkSize);

            MakeAndInitialize<NativeBuffer>(&spNativeBuffer, pMem, chunkSize, TRUE);
            VideoMessageReceived(NativeBuffer::GetIBufferFromNativeBuffer(spNativeBuffer), hnsPresenationTime, hnsSampleDuration);
        }

        // Increment byte position
        bytes += chunkSize;
        bytesToSend -= chunkSize;            
    }
    return;

}

BackEndTransport::~BackEndTransport()
{
}
