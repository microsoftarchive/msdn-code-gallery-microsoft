/****************************** Module Header ******************************\
* Module Name:  PictureWriter.h
* Project:      CppUniversalAppImageToVideo
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

#pragma once
#include <mfapi.h>
#include <mfidl.h>
#include <wrl\client.h>
#include <Mfreadwrite.h>

namespace EncodeImage
{
	public ref class PictureWriter sealed
	{
	public:
		PictureWriter(Windows::Storage::Streams::IRandomAccessStream^ outputStream, unsigned int width, unsigned int height);

		void AddFrame(const Platform::Array<uint8>^ buffer, int imageWidth, int imageHeight);
		void Finalize();

	private:
		~PictureWriter();

		unsigned int _width;
		unsigned int  _height;
		const unsigned long _bufferLength;
		LONGLONG _hnsSampleTime;
		DWORD _streamIndex;
		Microsoft::WRL::ComPtr<IMFSinkWriter> _spSinkWriter;
	};

}
