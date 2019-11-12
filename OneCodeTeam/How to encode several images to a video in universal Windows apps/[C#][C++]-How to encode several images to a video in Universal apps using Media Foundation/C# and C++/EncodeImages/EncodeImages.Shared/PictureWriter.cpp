/****************************** Module Header ******************************\
* Module Name:  PictureWriter.cpp
* Project:      CSUniversalAppImageToVideo
* Copyright (c) Microsoft Corporation.
*
* This class implement encoding images to video. We set the duration to one
* second per image default.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/


#include "pch.h"

#include "PictureWriter.h"

#pragma comment(lib, "Mfreadwrite.lib")
#pragma comment(lib, "Mfplat.lib")
#pragma comment(lib, "Mfuuid.lib")
using namespace Microsoft::WRL;
using namespace EncodeImages;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Storage::Streams;

const unsigned int RATE_NUM = 10;
const unsigned int RATE_DENOM = 1;
const unsigned int BITRATE = 3000000;
const unsigned int ASPECT_NUM = 1;
const unsigned int ASPECT_DENOM = 1;
const unsigned long  BPP_IN = 32;
const long long DURATION = 10000000 * (long long)RATE_DENOM / (long long)RATE_NUM; // In hundred-nanoseconds
const unsigned int ONE_SECOND = RATE_NUM / RATE_DENOM;
const unsigned int FRAME_NUM = 10 * ONE_SECOND;

#define CHK(statement)	{HRESULT _hr = (statement); if (FAILED(_hr)) { throw ref new Platform::COMException(_hr); };}

PictureWriter::PictureWriter(IRandomAccessStream^ videoStream, unsigned int width, unsigned int height)
	: _bufferLength(width * height * BPP_IN / 8)
	, _width(width)
	, _height(height)
	, _hnsSampleTime(0)
	, _streamIndex(0)
{
	CHK(MFStartup(MF_VERSION));

	ComPtr<IMFByteStream> spByteStream;
	CHK(MFCreateMFByteStreamOnStreamEx((IUnknown*)videoStream, &spByteStream));
	
	// Create the Sink Writer

	ComPtr<IMFAttributes> spAttr;
	CHK(MFCreateAttributes(&spAttr, 10));
	CHK(spAttr->SetUINT32(MF_READWRITE_ENABLE_HARDWARE_TRANSFORMS, true));

	CHK(MFCreateSinkWriterFromURL(L".mp4", spByteStream.Get(), spAttr.Get(), &_spSinkWriter));
	
	// Setup the output media type   

	ComPtr<IMFMediaType> spTypeOut;
	CHK(MFCreateMediaType(&spTypeOut));
	CHK(spTypeOut->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Video));
	CHK(spTypeOut->SetGUID(MF_MT_SUBTYPE, MFVideoFormat_H264));
	CHK(spTypeOut->SetUINT32(MF_MT_AVG_BITRATE, BITRATE));
	CHK(spTypeOut->SetUINT32(MF_MT_INTERLACE_MODE, MFVideoInterlace_Progressive));
	CHK(MFSetAttributeSize(spTypeOut.Get(), MF_MT_FRAME_SIZE, _width, _height));
	CHK(MFSetAttributeRatio(spTypeOut.Get(), MF_MT_FRAME_RATE, RATE_NUM, RATE_DENOM));
	CHK(MFSetAttributeRatio(spTypeOut.Get(), MF_MT_PIXEL_ASPECT_RATIO, ASPECT_NUM, ASPECT_DENOM));

	CHK(_spSinkWriter->AddStream(spTypeOut.Get(), &_streamIndex));

	// Setup the input media type   

	ComPtr<IMFMediaType> spTypeIn;
	CHK(MFCreateMediaType(&spTypeIn));
	CHK(spTypeIn->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Video));
	CHK(spTypeIn->SetGUID(MF_MT_SUBTYPE, MFVideoFormat_RGB32));
	CHK(spTypeIn->SetUINT32(MF_MT_INTERLACE_MODE, MFVideoInterlace_Progressive));
	CHK(MFSetAttributeSize(spTypeIn.Get(), MF_MT_FRAME_SIZE, _width, _height));
	CHK(MFSetAttributeRatio(spTypeIn.Get(), MF_MT_FRAME_RATE, RATE_NUM, RATE_DENOM));
	CHK(MFSetAttributeRatio(spTypeIn.Get(), MF_MT_PIXEL_ASPECT_RATIO, ASPECT_NUM, ASPECT_DENOM));

	CHK(_spSinkWriter->SetInputMediaType(_streamIndex, spTypeIn.Get(), nullptr));

	CHK(_spSinkWriter->BeginWriting());
}

PictureWriter::~PictureWriter()
{
	_spSinkWriter = nullptr; // Release before shutting down MF
	
	CHK(MFShutdown());
}

void PictureWriter::AddFrame(const Platform::Array<uint8>^ videoFrameBuffer, int imageWidth, int imageHeight)
{
	// Create a media sample   
	ComPtr<IMFSample> spSample;
	CHK(MFCreateSample(&spSample));
	CHK(spSample->SetSampleDuration(DURATION));
	CHK(spSample->SetSampleTime(_hnsSampleTime));
	_hnsSampleTime += DURATION;

	// Add a media buffer
	ComPtr<IMFMediaBuffer> spBuffer;
	CHK(MFCreateMemoryBuffer(_bufferLength, &spBuffer));
	CHK(spBuffer->SetCurrentLength(_bufferLength));
	CHK(spSample->AddBuffer(spBuffer.Get()));

	// Copy the picture into the buffer
	unsigned char *pbBuffer = nullptr;
	CHK(spBuffer->Lock(&pbBuffer, nullptr, nullptr));
	BYTE* buffer = (BYTE*)videoFrameBuffer->begin() + 4 * imageWidth * (imageHeight - 1);
	CHK(MFCopyImage(pbBuffer + 4 * _width * (_height - imageHeight),
		4 * _width, buffer, -4 * imageWidth, 4 * imageWidth, imageHeight));
	// [-or-]
	//memcpy(pbBuffer, videoFrameBuffer->begin(), _height * imageWidth * 4);
	CHK(spBuffer->Unlock());

	// Write the media sample   
	CHK(_spSinkWriter->WriteSample(_streamIndex, spSample.Get()));
}

void PictureWriter::Finalize()
{
	CHK(_spSinkWriter->Finalize());
}