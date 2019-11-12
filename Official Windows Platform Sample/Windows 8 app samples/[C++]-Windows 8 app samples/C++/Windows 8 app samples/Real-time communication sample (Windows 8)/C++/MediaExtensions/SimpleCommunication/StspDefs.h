//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once
#ifndef DECLSPEC_UUID
#define DECLSPEC_UUID(x)    __declspec(uuid(x))
#endif

#ifndef DECLSPEC_NOVTABLE
#define DECLSPEC_NOVTABLE   __declspec(novtable)
#endif

enum StspOperation
{
    StspOperation_Unknown,
    StspOperation_ClientRequestDescription,
    StspOperation_ClientRequestStart,
    StspOperation_ClientRequestStop,
    StspOperation_ServerDescription,
    StspOperation_ServerSample,
    StspOperation_Last,
};

struct StspOperationHeader
{
    DWORD cbDataSize;
    StspOperation eOperation;
};

struct StspStreamDescription
{
    GUID guiMajorType;
    GUID guiSubType;
    DWORD dwStreamId;
    UINT32 cbAttributesSize;
};

struct StspDescription
{
    UINT32 cNumStreams;
    StspStreamDescription aStreams[1];
};

enum StspSampleFlags
{
    StspSampleFlag_BottomFieldFirst,
    StspSampleFlag_CleanPoint,
    StspSampleFlag_DerivedFromTopField,
    StspSampleFlag_Discontinuity,
    StspSampleFlag_Interlaced,
    StspSampleFlag_RepeatFirstField,
    StspSampleFlag_SingleField,
};

struct StspSampleHeader
{
    DWORD dwStreamId;
    LONGLONG ullTimestamp;
    LONGLONG ullDuration;
    DWORD dwFlags;
    DWORD dwFlagMasks;
};

enum StspNetworkType
{
    StspNetworkType_IPv4,
    StspNetworkType_IPv6,
};

extern wchar_t const __declspec(selectany) c_szStspScheme[] = L"stsp";
extern wchar_t const __declspec(selectany) c_szStspSchemeWithColon[] = L"stsp:";
unsigned short const c_wStspDefaultPort = 10010;

HRESULT FilterOutputMediaType(IMFMediaType *pSourceMediaType, IMFMediaType *pDestinationMediaType);
HRESULT ValidateInputMediaType(REFGUID guidMajorType, REFGUID guidSubtype, IMFMediaType *pMediaType);