//////////////////////////////////////////////////////////////////////////
//
// Parse.cpp
// MPEG-1 parsing code.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
//////////////////////////////////////////////////////////////////////////

#include "pch.h"
#include "MPEG1Source.h"
#include "Parse.h"

// HAS_FLAG: Test if 'b' contains a specified bit flag
#define HAS_FLAG(b, flag) (((b) & (flag)) == (flag))

#define htonl(x)   ((((x) >> 24) & 0x000000FFL) | \
    (((x) >>  8) & 0x0000FF00L) | \
    (((x) <<  8) & 0x00FF0000L) | \
    (((x) << 24) & 0xFF000000L))

// MAKE_WORD: Convert two bytes into a WORD
inline WORD MAKE_WORD(BYTE b1, BYTE b2)
{
    return ((b1 << 8) | b2);
}

// MAKE_DWORD_HOSTORDER:
// Convert the first 4 bytes of an array into a DWORD in host order (ie, no byte swapping).
inline DWORD MAKE_DWORD_HOSTORDER(const BYTE *pData)
{
    return ((DWORD*)pData)[0];
}

// MAKE_DWORD:
// Convert the first 4 bytes of an array into a DWORD in MPEG-1 stream byte order.
inline DWORD MAKE_DWORD(const BYTE *pData)
{
    return htonl( MAKE_DWORD_HOSTORDER(pData) );
}


//-------------------------------------------------------------------
// AdvanceBufferPointer
// Advances a byte array pointer.
//
// pData: The array pointer.
// cbBufferSize: The array size. On output, the size remaining.
// cbAdvance: Number of bytes to advance the pointer.
//
// This function is just a helper for keeping a valid pointer as we
// walk through a buffer.
//-------------------------------------------------------------------

inline void AdvanceBufferPointer(const BYTE* &pData, DWORD &cbBufferSize, DWORD cbAdvance)
{
    assert(cbBufferSize >= cbAdvance);
    if (cbBufferSize < cbAdvance)
    {
        throw ref new InvalidArgumentException();
    }
    cbBufferSize -= cbAdvance;
    pData += cbAdvance;
}

// ValidateBufferSize:
// Fails if cbBufferSize < cbMinRequiredSize.
inline void ValidateBufferSize(DWORD cbBufferSize, DWORD cbMinRequiredSize)
{
    if (cbBufferSize < cbMinRequiredSize)
    {
        ThrowException(MF_E_INVALID_FORMAT);
    }
}

// Forward declarations.
void ParseStreamData(const BYTE *pData, MPEG1StreamHeader &header);
void ParseStreamId(BYTE id, StreamType *pType, BYTE *pStreamNum);
LONGLONG ParsePTS(const BYTE *pData);

MFRatio GetFrameRate(BYTE frameRateCode);
MFRatio GetPixelAspectRatio(BYTE pixelAspectCode);

DWORD GetAudioBitRate(MPEG1AudioLayer layer, BYTE index);
DWORD GetSamplingFrequency(BYTE code);


//-------------------------------------------------------------------
// Buffer class
//-------------------------------------------------------------------


Buffer::Buffer(DWORD cbSize)
    : m_begin(0)
    , m_end(0)
    , m_count(0)
    , m_allocated(0)
{
    SetSize(cbSize);
    ZeroMemory(Ptr, cbSize);
}

//-------------------------------------------------------------------
// DataPtr
// Returns a pointer to the start of the buffer.
//-------------------------------------------------------------------

BYTE *Buffer::DataPtr::get()
{
    return Ptr + m_begin;
}


//-------------------------------------------------------------------
// DataSize
// Returns the size of the buffer.
//
// Note: The "size" is determined by the start and end pointers.
// The memory allocated for the buffer can be larger.
//-------------------------------------------------------------------

DWORD Buffer::DataSize::get() const
{
    assert(m_end >= m_begin);

    return m_end - m_begin;
}

// Reserves memory for the array, but does not increase the count.
void Buffer::Allocate(DWORD alloc)
{
    HRESULT hr = S_OK;
    if (alloc > m_allocated)
    {
        Array<BYTE> ^tmp = ref new Array<BYTE>(alloc);
        ZeroMemory(tmp->Data, alloc);

        assert(m_count <= m_allocated);

        // Copy the elements to the re-allocated array.
        for (DWORD i = 0; i < m_count; i++)
        {
            tmp[i] = m_array[i];
        }
        m_array = tmp;
        m_allocated = alloc;
    }
}

// Changes the count, and grows the array if needed.
void Buffer::SetSize(DWORD count)
{
    assert (m_count <= m_allocated);

    if (count > m_allocated)
    {
        Allocate(count);
    }

    m_count = count;
}


//-------------------------------------------------------------------
// Reserve
// Reserves additional bytes of memory for the buffer.
//
// This method does *not *increase the value returned by DataSize().
//
// After this method returns, the value of DataPtr() might change,
// so do not cache the old value.
//-------------------------------------------------------------------

void Buffer::Reserve(DWORD cb)
{
    if (cb > MAXDWORD - DataSize)
    {
        throw ref new InvalidArgumentException();
    }

    // If this would push the end position past the end of the array,
    // then we need to copy up the data to start of the array. We might
    // also need to realloc the array.

    if (cb > m_count - m_end)
    {
        // New end position would be past the end of the array.
        // Check if we need to grow the array.

        if (cb > CurrentFreeSize)
        {
            // Array needs to grow
            SetSize(DataSize + cb);
        }

        MoveMemory(Ptr, DataPtr, DataSize);

        // Reset begin and end.
        m_end = DataSize; // Update m_end first before resetting m_begin!
        m_begin = 0;
    }

    assert(CurrentFreeSize >= cb);
}


//-------------------------------------------------------------------
// MoveStart
// Moves the start of the buffer by cb bytes.
//
// Call this method after consuming data from the buffer.
//-------------------------------------------------------------------

void Buffer::MoveStart(DWORD cb)
{
    // Cannot advance pass the end of the buffer.
    if (cb > DataSize)
    {
        throw ref new InvalidArgumentException();
    }

    m_begin += cb;
}


//-------------------------------------------------------------------
// MoveEnd
// Moves end position of the buffer.
//-------------------------------------------------------------------

void Buffer::MoveEnd(DWORD cb)
{
    Reserve(cb);
    m_end += cb;
}


//-------------------------------------------------------------------
// CurrentFreeSize (private)
//
// Returns the size of the array minus the size of the data.
//-------------------------------------------------------------------

DWORD Buffer::CurrentFreeSize::get() const
{
    assert(m_count >= DataSize);
    return m_count - DataSize;
}


//-------------------------------------------------------------------
// Parser class
//-------------------------------------------------------------------


Parser::Parser() 
    : m_SCR(0)
    , m_muxRate(0)
    , m_bHasPacketHeader(false)
    , m_bEOS(false)
{
    ZeroMemory(&m_curPacketHeader, sizeof(m_curPacketHeader));
}

//-------------------------------------------------------------------
// GetSystemHeader class
//
// Returns a copy of the system header.
// Do not call this method unless HasSystemHeader() returns true.
//
// The caller must free the returned structure by calling
// CoTaskMemFree.
//-------------------------------------------------------------------

ExpandableStruct<MPEG1SystemHeader> ^Parser::GetSystemHeader()
{
    if (!HasSystemHeader)
    {
        ThrowException(MF_INVALID_STATE_ERR);
    }

    assert(m_header->Size > 0);

    auto header = ref new ExpandableStruct<MPEG1SystemHeader>(m_header);

    header->CopyFrom(m_header);

    return header;
}


//-------------------------------------------------------------------
// ParseBytes
// Parses as much data as possible from the pData buffer, and returns
// the amount of data parsed in pAte (*pAte <= cbLen).
//
// Return values:
//      true: The method consumed some data (*pAte > 0).
//      false: The method did not consume any data (*pAte == 0).
//
// If the method returns S_FALSE, the caller must allocate a larger
// buffer and pass in more data.
//-------------------------------------------------------------------

bool Parser::ParseBytes(const BYTE *pData, DWORD cbLen, DWORD *pAte)
{
    *pAte = 0;

    if (cbLen < 4)
    {
        return false;
    }

    DWORD cbLengthToStartCode = 0;  // How much we skip to reach the next start code.
    DWORD cbParsed = 0;             // How much we parse after the start code.

    m_bHasPacketHeader = false;

    bool result = FindNextStartCode(pData, cbLen, &cbLengthToStartCode);

    if (result)
    {
        cbLen -= cbLengthToStartCode;
        pData += cbLengthToStartCode;

        switch (MAKE_DWORD(pData))
        {
        case MPEG1_PACK_START_CODE:
            // Start of pack.
            result = ParsePackHeader(pData, cbLen, &cbParsed);
            break;

        case MPEG1_SYSTEM_HEADER_CODE:
            // Start of system header.
            result = ParseSystemHeader(pData, cbLen, &cbParsed);
            break;

        case MPEG1_STOP_CODE:
            // Stop code, end of stream.
            cbParsed = sizeof(DWORD);
            OnEndOfStream();
            break;

        default:
            // Start of packet.
            result = ParsePacketHeader(pData, cbLen, &cbParsed);
            break;

        }
    }

    if (result)
    {
        *pAte = cbLengthToStartCode + cbParsed;
    }
    return result;
};


//-------------------------------------------------------------------
// FindNextStartCode
// Looks for the next start code in the buffer.
//
// pData: Pointer to the buffer.
// cbLen: Size of the buffer.
// pAte: Receives the number of bytes *before *the start code.
//
// If no start code is found, the method returns S_FALSE.
//-------------------------------------------------------------------

bool Parser::FindNextStartCode(const BYTE *pData, DWORD cbLen, DWORD *pAte)
{
    bool result = false;

    DWORD cbLeft = cbLen;

    while (cbLeft > 4)
    {
        if ( (MAKE_DWORD_HOSTORDER(pData) & 0x00FFFFFF) == 0x00010000 )
        {
            result = true;
            break;
        }

        cbLeft -= 4;
        pData += 4;
    }
    *pAte = (cbLen - cbLeft);
    return result;
}


//-------------------------------------------------------------------
// ParsePackHeader
// Parses the start of an MPEG-1 pack.
//-------------------------------------------------------------------

bool Parser::ParsePackHeader(const BYTE *pData, DWORD cbLen, DWORD *pAte)
{
    assert( MAKE_DWORD(pData) == MPEG1_PACK_START_CODE );

    if (cbLen < MPEG1_PACK_HEADER_SIZE)
    {
        return false; // Not enough data yet.
    }

    // Check marker bits
    if ( ((pData[4] & 0xF1) != 0x21) ||
        ((pData[6] & 0x01) != 0x01) ||
        ((pData[8] & 0x01) != 0x01) ||
        ((pData[9] & 0x80) != 0x80) ||
        ((pData[11] & 0x01) != 0x01) )
    {
        ThrowException(MF_E_INVALID_FORMAT);
    }


    // Calculate the SCR.
    LONGLONG scr = ( (pData[8] & 0xFE) >> 1) |
        ( (pData[7]) << 7) |
        ( (pData[6] & 0xFE) << 14) |
        ( (pData[5]) << 22) |
        ( (pData[4] & 0x0E) << 29);

    DWORD muxRate = ( (pData[11] & 0xFE) >> 1) |
        ( (pData[10]) << 7) |
        ( (pData[9] & 0x7F) << 15);


    m_SCR = scr;
    m_muxRate = muxRate;

    *pAte = MPEG1_PACK_HEADER_SIZE;

    return true;
}


//-------------------------------------------------------------------
// ParseSystemHeader.
// Parses the MPEG-1 system header.
//
// NOTES:
// The system header optionally appears after the pack header.
// The first pack must contain a system header.
// Subsequent packs may contain a system header.
//-------------------------------------------------------------------

bool Parser::ParseSystemHeader(const BYTE *pData, DWORD cbLen, DWORD *pAte)
{
    assert( MAKE_DWORD(pData) == MPEG1_SYSTEM_HEADER_CODE );

    if (cbLen < MPEG1_SYSTEM_HEADER_MIN_SIZE)
    {
        return false; // Not enough data yet.
    }

    // Find the total header length.
    DWORD cbHeaderLen = MPEG1_SYSTEM_HEADER_PREFIX +  MAKE_WORD(pData[4], pData[5]);

    if (cbHeaderLen < MPEG1_SYSTEM_HEADER_MIN_SIZE - MPEG1_SYSTEM_HEADER_PREFIX)
    {
        ThrowException(MF_E_INVALID_FORMAT);
    }

    if (cbLen < cbHeaderLen)
    {
        return false; // Not enough data yet.
    }

    // We have enough data to parse the header.

    // Did we already see a system header?
    if (!HasSystemHeader)
    {
        // This is the first time we've seen the header. Parse it.

        // Calculate the number of stream info's in the header.
        DWORD cStreamInfo = (cbHeaderLen - MPEG1_SYSTEM_HEADER_MIN_SIZE) / MPEG1_SYSTEM_HEADER_STREAM;

        // Calculate the structure size.
        DWORD cbSize = sizeof(MPEG1SystemHeader);
        if (cStreamInfo > 1)
        {
            cbSize += sizeof(MPEG1StreamHeader) * (cStreamInfo - 1);
        }

        // Allocate room for the header.
        m_header = ref new ExpandableStruct<MPEG1SystemHeader>(cbSize);
        try
        {
            m_header->Get()->cbSize = cbSize;

            // Check marker bits
            if (((pData[6] & 0x80) != 0x80) ||
                ((pData[8] & 0x01) != 0x01) ||
                ((pData[10] & 0x20) != 0x20) ||
                (pData[11] != 0xFF))
            {
                ThrowException(MF_E_INVALID_FORMAT);
            }

            m_header->Get()->rateBound = ((pData[6] & 0x7F) << 16) | (pData[7] << 8) | (pData[8] >> 1);
            m_header->Get()->cAudioBound = pData[9] >> 2;
            m_header->Get()->bFixed = HAS_FLAG(pData[9], 0x02);
            m_header->Get()->bCSPS = HAS_FLAG(pData[9], 0x01);
            m_header->Get()->bAudioLock = HAS_FLAG(pData[10], 0x80);
            m_header->Get()->bVideoLock = HAS_FLAG(pData[10], 0x40);
            m_header->Get()->cVideoBound = pData[10] & 0x1F;
            m_header->Get()->cStreams = cStreamInfo;

            // Parse the stream information.
            const BYTE *pStreamInfo = pData + MPEG1_SYSTEM_HEADER_MIN_SIZE;

            for (DWORD i = 0; i < cStreamInfo; i++)
            {
                ParseStreamData(pStreamInfo, m_header->Get()->streams[i]);

                pStreamInfo += MPEG1_SYSTEM_HEADER_STREAM;
            }
        }
        catch (Exception^)
        {
            m_header = nullptr;
            throw;
        }
    }

    *pAte = cbHeaderLen;

    return true;
}

//-------------------------------------------------------------------
// ParsePacketHeader
//
// Parses the packet header.
//
// If the method returns S_OK, then HasPacket() returns true and the
// caller can start parsing the packet.
//-------------------------------------------------------------------

bool Parser::ParsePacketHeader(const BYTE *pData, DWORD cbLen, DWORD *pAte)
{
    if (!HasSystemHeader)
    {
        ThrowException(MF_E_INVALIDREQUEST); // We should not get a packet before the first system header.
    }

    if (cbLen < MPEG1_PACKET_HEADER_MIN_SIZE)
    {
        return false; // Not enough data yet.
    }

    // Before we parse anything else in the packet header, look for the header length.
    DWORD cbPacketLen = MAKE_WORD(pData[4], pData[5]) + MPEG1_PACKET_HEADER_MIN_SIZE;

    // We want enough data for the maximum packet header OR the total packet size, whichever is less.
    if (cbLen < cbPacketLen && cbLen < MPEG1_PACKET_HEADER_MAX_SIZE)
    {
        return false; // Not enough data yet.
    }

    // Make sure the start code is 0x000001xx
    if ((MAKE_DWORD(pData) & 0xFFFFFF00) != MPEG1_START_CODE_PREFIX)
    {
        ThrowException(MF_E_INVALID_FORMAT);
    }

    BYTE id = 0;
    StreamType type = StreamType_Unknown;
    BYTE num = 0;
    bool bHasPTS = false;

    ZeroMemory(&m_curPacketHeader, sizeof(m_curPacketHeader));

    // Find the stream ID.
    id = pData[3];
    ParseStreamId(id, &type, &num);

    DWORD cbLeft = cbPacketLen - MPEG1_PACKET_HEADER_MIN_SIZE;
    pData = pData + MPEG1_PACKET_HEADER_MIN_SIZE;
    DWORD cbPadding = 0;
    LONGLONG pts = 0;

    // Go past the stuffing bytes.
    while ((cbLeft > 0) && (*pData == 0xFF))
    {
        AdvanceBufferPointer(pData, cbLeft, 1);
        ++cbPadding;
    }

    // Check for invalid number of stuffing bytes.
    if (cbPadding > MPEG1_PACKET_HEADER_MAX_STUFFING_BYTE)
    {
        ThrowException(MF_E_INVALID_FORMAT);
    }

    // The next bits are:
    // (optional) STD buffer size (2 bytes)
    // union
    // {
    //      PTS (5 bytes)
    //      PTS + DTS (10 bytes)
    //      '0000 1111' (1 bytes)
    // }

    ValidateBufferSize(cbLeft, 1);

    if ((*pData & 0xC0) == 0x40)
    {
        // Skip STD buffer size.
        AdvanceBufferPointer(pData, cbLeft, 2);
    }

    ValidateBufferSize(cbLeft, 1);

    if ((*pData & 0xF1) == 0x21)
    {
        // PTS
        ValidateBufferSize(cbLeft, 5);

        pts = ParsePTS(pData);
        bHasPTS = true;

        AdvanceBufferPointer(pData, cbLeft, 5);
    }
    else if ((*pData & 0xF1) == 0x31)
    {
        // PTS + DTS
        ValidateBufferSize(cbLeft, 10);

        // Parse PTS but skip DTS.
        pts = ParsePTS(pData);
        bHasPTS = true;

        AdvanceBufferPointer(pData, cbLeft, 10);
    }
    else if ((*pData) == 0x0F)
    {
        AdvanceBufferPointer(pData, cbLeft, 1);
    }
    else
    {
        ThrowException(MF_E_INVALID_FORMAT); // Unexpected bit field
    }

    m_curPacketHeader.stream_id = id;
    m_curPacketHeader.type = type;
    m_curPacketHeader.number = num;
    m_curPacketHeader.cbPacketSize = cbPacketLen;
    m_curPacketHeader.cbPayload = cbLeft;
    m_curPacketHeader.bHasPTS = bHasPTS;
    m_curPacketHeader.PTS = pts;

    // Client can read the packet now.
    m_bHasPacketHeader = true;

    *pAte = cbPacketLen - cbLeft;

    return true;
}

//-------------------------------------------------------------------
// OnEndOfStream
// Called when the parser reaches the MPEG-1 stop code.
//
// Note: Obviously the parser is not guaranteed to see a stop code
// before the client reaches the end of the source data. The client
// must be prepared to handle that case.
//-------------------------------------------------------------------

void Parser::OnEndOfStream()
{
    m_bEOS = true;
    ClearPacket();
}


//-------------------------------------------------------------------
// Static functions
//-------------------------------------------------------------------


//-------------------------------------------------------------------
// ParsePTS
// Parse the 33-bit Presentation Time Stamp (PTS)
//-------------------------------------------------------------------

LONGLONG ParsePTS(const BYTE *pData)
{
    BYTE byte1 = pData[0];
    WORD word1 = MAKE_WORD(pData[1], pData[2]);
    WORD word2 = MAKE_WORD(pData[3], pData[4]);

    // Check marker bits.
    // The first byte can be '0010xxx1' or '0x11xxxx1'
    if (((byte1 & 0xE1) != 0x21) ||
        ((word1 & 0x01) != 0x01) ||
        ((word2 & 0x01) != 0x01) )
    {
        ThrowException(MF_E_INVALID_FORMAT);
    }

    LARGE_INTEGER li;

    // The PTS is 33 bits, so bit 32 goes in the high-order DWORD
    li.HighPart = (byte1 & 0x08) >> 3;

    li.LowPart = (static_cast<DWORD>(byte1 & 0x06) << 29) |
        (static_cast<DWORD>(word1 & 0xFFFE) << 14) |
        (static_cast<DWORD>(word2) >> 1);

    return li.QuadPart;
}


//-------------------------------------------------------------------
// ParseStreamData
// Parses the stream information (for one stream) in the system
// header.
//-------------------------------------------------------------------

void ParseStreamData(const BYTE *pStreamInfo, MPEG1StreamHeader &header)
{
    // Check marker bits.
    if ( (pStreamInfo[1] & 0xC0) != 0xC0 )
    {
        ThrowException(MF_E_INVALID_FORMAT); // Invalid bits
    }

    BYTE id = 0;
    BYTE num = 0;
    DWORD bound = 0;
    StreamType type = StreamType_Unknown;

    // The id is a stream code plus (for some types) a stream number, bitwise-OR'd.

    id = pStreamInfo[0];

    ParseStreamId(id, &type, &num);

    // Calculate STD bound.
    bound = pStreamInfo[2] | ((pStreamInfo[1] & 0x1F) << 8);

    if (pStreamInfo[1] & 0x20)
    {
        bound *= 1024;
    }
    else
    {
        bound *= 128;
    }

    header.stream_id = id;
    header.type = type;
    header.number = num;
    header.sizeBound = bound;
}



//-------------------------------------------------------------------
// ParseStreamId
// Parses an MPEG-1 stream ID.
//
// Note:
// The id is a stream code, plus (for some types) a stream number,
// bitwise-OR'd. This function returns the type and the stream number.
//
// See ISO/EIC 11172-1, sec 2.4.4.2
//-------------------------------------------------------------------

void ParseStreamId(BYTE id, StreamType *pType, BYTE *pStreamNum)
{
    StreamType type = StreamType_Unknown;
    BYTE num = 0;

    switch (id)
    {
    case MPEG1_STREAMTYPE_ALL_AUDIO:
        type = StreamType_AllAudio;
        break;

    case MPEG1_STREAMTYPE_ALL_VIDEO:
        type = StreamType_AllVideo;
        break;

    case MPEG1_STREAMTYPE_RESERVED:
        type = StreamType_Reserved;
        break;

    case MPEG1_STREAMTYPE_PRIVATE1:
        type = StreamType_Private1;
        break;

    case MPEG1_STREAMTYPE_PADDING:
        type = StreamType_Padding;
        break;

    case MPEG1_STREAMTYPE_PRIVATE2:
        type = StreamType_Private2;
        break;

    default:
        if ((id & 0xE0) == MPEG1_STREAMTYPE_AUDIO_MASK)
        {
            type = StreamType_Audio;
            num = id & 0x1F;
        }
        else if ((id & 0xF0) == MPEG1_STREAMTYPE_VIDEO_MASK)
        {
            type = StreamType_Video;
            num = id & 0x0F;
        }
        else if ((id & 0xF0) == MPEG1_STREAMTYPE_DATA_MASK)
        {
            type = StreamType_Data;
            num = id & 0x0F;
        }
        else
        {
            ThrowException(MF_E_INVALID_FORMAT); // Unknown stream ID code.
        }
    }

    *pType = type;
    *pStreamNum = num;
}


//-------------------------------------------------------------------
// ReadVideoSequenceHeader
// Parses a video sequence header.
//
// Call Parser::HasPacket() to ensure that pData points to the start
// of a payload, and call Parser::PacketHeader() to verify it is a
// video packet.
//-------------------------------------------------------------------

DWORD ReadVideoSequenceHeader(
    _In_reads_bytes_(cbData) const BYTE *pData,
    DWORD cbData,
    MPEG1VideoSeqHeader &seqHeader
    )
{
    DWORD cbPadding = 0;

    if (pData == nullptr)
    {
        throw ref new InvalidArgumentException();
    }

    // Skip to the sequence header code.
    while ( (cbPadding + 4) <= cbData && ((DWORD*)pData)[0] == 0 )
    {
        pData += 4;
        cbPadding += 4;
    }

    cbData -= cbPadding;

    // Check for the minimum size buffer.
    if (cbData < MPEG1_VIDEO_SEQ_HEADER_MIN_SIZE)
    {
        return cbPadding;
    }

    // Validate the sequence header code.
    if (MAKE_DWORD(pData) != MPEG1_SEQUENCE_HEADER_CODE)
    {
        ThrowException(MF_E_INVALID_FORMAT);
    }

    // Calculate the actual required size.
    DWORD cbRequired = MPEG1_VIDEO_SEQ_HEADER_MIN_SIZE;

    // Look for quantization matrices.
    if ( HAS_FLAG(pData[11], 0x02) )
    {
        // Intra quantization matrix is true.
        cbRequired += 64;
    }
    // Intra is false, look for non-intra flag
    else if ( HAS_FLAG(pData[11], 0x01) )
    {
        cbRequired += 64;
    }

    if (cbData < cbRequired)
    {
        return cbPadding;
    }

    ZeroMemory(&seqHeader, sizeof(seqHeader));

    // Check the marker bit.
    if ( !HAS_FLAG(pData[10], 0x20) )
    {
        ThrowException(MF_E_INVALID_FORMAT);
    }

    BYTE parCode = pData[7] >> 4;
    BYTE frameRateCode = pData[7] & 0x0F;

    seqHeader.pixelAspectRatio = GetPixelAspectRatio(parCode);
    seqHeader.frameRate = GetFrameRate(frameRateCode);

    seqHeader.width = (pData[4] << 4) | (pData[5] >> 4) ;
    seqHeader.height = ((pData[5] & 0x0F) << 8) | (pData[6]);
    seqHeader.bitRate = (pData[8] << 10) | (pData[9] << 2) | (pData[10] >> 6);

    if (seqHeader.bitRate == 0)
    {
        ThrowException(MF_E_INVALID_FORMAT); // Not allowed.
    }
    else if (seqHeader.bitRate == 0x3FFFF)
    {
        seqHeader.bitRate = 0; // Variable bit-rate.
    }
    else
    {
        seqHeader.bitRate = seqHeader.bitRate * 400; // Units of 400 bps
    }

    seqHeader.cbVBV_Buffer = ( ((pData[10] & 0x1F) << 5) | (pData[11] >> 3) ) * 2048;
    seqHeader.bConstrained = HAS_FLAG(pData[11], 0x04);

    seqHeader.cbHeader = cbRequired;
    CopyMemory(seqHeader.header, pData, cbRequired);

    return cbRequired + cbPadding;
}



//-------------------------------------------------------------------
// GetFrameRate
// Returns the frame rate from the picture_rate field of the sequence
// header.
//
// See ISO/IEC 11172-2, 2.4.3.2 "Sequence Header"
//-------------------------------------------------------------------

MFRatio GetFrameRate(BYTE frameRateCode)
{
    MFRatio frameRates[] =
    {
        { 0, 0 },           // invalid
        { 24000, 1001 },    // 23.976 fps
        { 24, 1 },
        { 25, 1 },
        { 30000, 1001 },    // 29.97 fps
        { 50, 1 },
        { 60000, 1001 },    // 59.94 fps
        { 60, 1 }
    };

    if (frameRateCode < 1 || frameRateCode >= ARRAYSIZE(frameRates))
    {
        ThrowException(MF_E_INVALIDTYPE);
    }

    MFRatio result = { frameRates[frameRateCode].Numerator, frameRates[frameRateCode].Denominator };
    return result;
}

//-------------------------------------------------------------------
// GetPixelAspectRatio
// Returns the pixel aspect ratio (PAR) from the pel_aspect_ratio
// field of the sequence header.
//
// See ISO/IEC 11172-2, 2.4.3.2 "Sequence Header"
//-------------------------------------------------------------------

MFRatio GetPixelAspectRatio(BYTE pixelAspectCode)
{
    DWORD height[] = { 0, 10000, 6735, 7031, 7615, 8055, 8437, 8935, 9157,
        9815, 10255, 10695, 10950, 11575, 12015 };

    const DWORD width = 10000;

    if (pixelAspectCode < 1 || pixelAspectCode >= ARRAYSIZE(height))
    {
        ThrowException(MF_E_INVALIDTYPE);
    }

    MFRatio result = { height[pixelAspectCode], width };
    return result;
}


//-------------------------------------------------------------------
// ReadAudioFrameHeader
// Parses an audio frame header.
//
// Call Parser::HasPacket() to ensure that pData points to the start
// of a payload, and call Parser::PacketHeader() to verify it is an
// audio packet.
//-------------------------------------------------------------------

DWORD ReadAudioFrameHeader(
    const BYTE *pData,
    DWORD cbData,
    MPEG1AudioFrameHeader &audioHeader
    )
{
    MPEG1AudioFrameHeader header;
    ZeroMemory(&header, sizeof(header));

    BYTE bitRateIndex = 0;
    BYTE samplingIndex = 0;

    if (cbData < MPEG1_AUDIO_FRAME_HEADER_SIZE)
    {
        return 0;
    }


    if (pData[0] != 0xFF)
    {
        ThrowException(MF_E_INVALID_FORMAT);
    }

    if (!HAS_FLAG(pData[1], 0xF8))
    {
        ThrowException(MF_E_INVALID_FORMAT);
    }

    // Layer bits
    switch (pData[1] & 0x06)
    {
    case 0x00:
        ThrowException(MF_E_INVALID_FORMAT);

    case 0x06:
        header.layer = MPEG1_Audio_Layer1;
        break;

    case 0x04:
        header.layer = MPEG1_Audio_Layer2;
        break;

    case 0x02:
        header.layer = MPEG1_Audio_Layer3;
        break;

    default:
        ThrowException(E_UNEXPECTED); // Cannot actually happen, given our bitmask above.
    }

    bitRateIndex = (pData[2] & 0xF0) >> 4;
    samplingIndex = (pData[2] & 0x0C) >> 2;

    // Bit rate.
    // Note: Accoring to ISO/IEC 11172-3, some combinations of bitrate and
    // mode are not valid. However, this is up to the decoder to validate.
    header.dwBitRate = GetAudioBitRate(header.layer, bitRateIndex);

    // Sampling frequency.
    header.dwSamplesPerSec = GetSamplingFrequency(samplingIndex);

    header.mode = static_cast<MPEG1AudioMode>((pData[3] & 0xC0) >> 6);
    header.modeExtension = (pData[3] & 0x30) >> 4;
    header.emphasis = (pData[3] & 0x03);

    // Parse the various bit flags.
    if (HAS_FLAG(pData[1], 0x01))
    {
        header.wFlags |= MPEG1_AUDIO_PROTECTION_BIT;
    }
    if (HAS_FLAG(pData[2], 0x01))
    {
        header.wFlags |= MPEG1_AUDIO_PRIVATE_BIT;
    }
    if (HAS_FLAG(pData[3], 0x08))
    {
        header.wFlags |= MPEG1_AUDIO_COPYRIGHT_BIT;
    }
    if (HAS_FLAG(pData[3], 0x04))
    {
        header.wFlags |= MPEG1_AUDIO_ORIGINAL_BIT;
    }

    if (header.mode == MPEG1_Audio_SingleChannel)
    {
        header.nChannels = 1;
    }
    else
    {
        header.nChannels = 2;
    }

    header.nBlockAlign = 1;

    CopyMemory(&audioHeader, &header, sizeof(audioHeader));
    return 4;
};


//-------------------------------------------------------------------
// GetAudioBitRate
// Returns the audio bit rate in KBits per second, from the
// bitrate_index field of the audio frame header.
//
// See ISO/IEC 11172-3, 2.4.2.3, "Header"
//-------------------------------------------------------------------

DWORD GetAudioBitRate(MPEG1AudioLayer layer, BYTE index)
{
    const DWORD MAX_BITRATE_INDEX = 14;

    // Table of bit rates.
    const DWORD bitrate[3][ (MAX_BITRATE_INDEX+1) ] =
    {
        { 0, 32, 64, 96, 128, 160, 192, 224, 256, 288, 320, 352, 384, 416, 448 },    // Layer I
        { 0, 32, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320, 384 },       // Layer II
        { 0, 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320 }         // Layer III
    };

    if (layer < MPEG1_Audio_Layer1 || layer > MPEG1_Audio_Layer3)
    {
        ThrowException(MF_E_INVALID_FORMAT);
    }
    if (index > MAX_BITRATE_INDEX)
    {
        ThrowException(MF_E_INVALID_FORMAT);
    }

    return bitrate[layer][index];
}

//-------------------------------------------------------------------
// GetSamplingFrequency
// Returns the sampling frequency in samples per second, from the
// sampling_frequency field of the audio frame header.
//
// See ISO/IEC 11172-3, 2.4.2.3, "Header"
//-------------------------------------------------------------------

DWORD GetSamplingFrequency(BYTE code)
{
    switch (code)
    {
    case 0:
        return 44100;
    case 1:
        return 48000;
    case 2:
        return 32000;
    default:
        ThrowException(MF_E_INVALID_FORMAT);
    }

    return 0;
}

