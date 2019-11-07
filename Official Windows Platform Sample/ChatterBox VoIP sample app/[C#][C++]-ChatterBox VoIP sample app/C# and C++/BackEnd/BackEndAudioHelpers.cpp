/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
#include "BackEndAudioHelpers.h"
// Begin of audio helpers
size_t inline GetWaveFormatSize(const WAVEFORMATEX& format)
{
    return (sizeof WAVEFORMATEX) + (format.wFormatTag == WAVE_FORMAT_PCM ? 0 : format.cbSize);
}

void FillPcmFormat(WAVEFORMATEX& format, WORD wChannels, int nSampleRate, WORD wBits)
{
    format.wFormatTag        = WAVE_FORMAT_PCM;
    format.nChannels         = wChannels;
    format.nSamplesPerSec    = nSampleRate;
    format.wBitsPerSample    = wBits;
    format.nBlockAlign       = format.nChannels * (format.wBitsPerSample / 8);
    format.nAvgBytesPerSec   = format.nSamplesPerSec * format.nBlockAlign;
    format.cbSize            = 0;
}

size_t BytesFromDuration(int nDurationInMs, const WAVEFORMATEX& format)
{
    return size_t(nDurationInMs * FLOAT(format.nAvgBytesPerSec) / 1000);
}

size_t SamplesFromDuration(int nDurationInMs, const WAVEFORMATEX& format)
{
    return size_t(nDurationInMs * FLOAT(format.nSamplesPerSec) / 1000);
}


BOOL WaveFormatCompare(const WAVEFORMATEX& format1, const WAVEFORMATEX& format2)
{
    size_t cbSizeFormat1 = GetWaveFormatSize(format1);
    size_t cbSizeFormat2 = GetWaveFormatSize(format2);

    return (cbSizeFormat1 == cbSizeFormat2) && (memcmp(&format1, &format2, cbSizeFormat1) == 0);
}
// End of audio helpers
