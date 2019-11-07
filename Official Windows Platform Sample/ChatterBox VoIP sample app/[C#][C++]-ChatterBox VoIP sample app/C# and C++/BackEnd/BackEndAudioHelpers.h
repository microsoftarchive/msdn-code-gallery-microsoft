/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
#pragma once
#include "windows.h"
#include <audioclient.h>
size_t inline GetWaveFormatSize(const WAVEFORMATEX& format);

void FillPcmFormat(WAVEFORMATEX& format, WORD wChannels, int nSampleRate, WORD wBits);
size_t BytesFromDuration(int nDurationInMs, const WAVEFORMATEX& format);

size_t SamplesFromDuration(int nDurationInMs, const WAVEFORMATEX& format);

BOOL WaveFormatCompare(const WAVEFORMATEX& format1, const WAVEFORMATEX& format2);
