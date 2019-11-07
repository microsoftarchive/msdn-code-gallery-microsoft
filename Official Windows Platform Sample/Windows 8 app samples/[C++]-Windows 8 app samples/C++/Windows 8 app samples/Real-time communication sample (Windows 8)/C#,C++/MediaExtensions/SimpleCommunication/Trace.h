//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

enum
{
    TRACE_LEVEL_LOW, 
    TRACE_LEVEL_NORMAL, 
    TRACE_LEVEL_HIGH,
};

extern DWORD g_dwTraceLevel;
void Trace(DWORD dwLevel, LPCWSTR pszFormat, ...);
void TraceError(LPCSTR pszFile, long nLine, LPCSTR pszFunc, const void * pThis, HRESULT hr);
void PrintSampleInfo(IMFSample *pSample);
void PrintAttributes(IMFAttributes *pAttributes);

#if 0
#define TRACE Trace

#define TRACEHR(hrVal) \
{ \
    __if_exists(this){ TraceError(__FILE__, __LINE__, __FUNCTION__, this, (hrVal)); } \
    __if_not_exists(this){ TraceError(__FILE__, __LINE__, __FUNCTION__, NULL, (hrVal)); } \
}

#define TRACEHR_RET(hrVal) \
{ \
    __if_exists(this){ TraceError(__FILE__, __LINE__, __FUNCTION__, this, (hrVal)); } \
    __if_not_exists(this){ TraceError(__FILE__, __LINE__, __FUNCTION__, NULL, (hrVal)); } \
    return hrVal; \
}

#else
#define TRACE
#define TRACEHR(hrVal)
#define TRACEHR_RET(hrVal) {return hrVal;}
#endif