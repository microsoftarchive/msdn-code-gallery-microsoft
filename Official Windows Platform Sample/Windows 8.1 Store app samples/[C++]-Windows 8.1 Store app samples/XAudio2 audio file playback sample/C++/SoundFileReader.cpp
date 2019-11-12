//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "SoundFileReader.h"
#include "RandomAccessReader.h"

namespace
{
    //
    // 4 Character Tags in the RIFF File of Interest (read backwards)
    //
    const uint32 FOURCC_RIFF_TAG      = 'FFIR';
    const uint32 FOURCC_FORMAT_TAG    = ' tmf';
    const uint32 FOURCC_DATA_TAG      = 'atad';
    const uint32 FOURCC_WAVE_FILE_TAG = 'EVAW';

    //
    // The header of every 'chunk' of data in the RIFF file
    //
    struct ChunkHeader
    {
        uint32 tag;
        uint32 size;
    };

    //
    // Helper function to find a riff-chunk with-in the sent bounds.
    // It is assumed the bounds begin at the start of a chunk
    //
    uint64 FindChunk(
        RandomAccessReader^ file,
        uint32 tag,
        uint64 startLoc,
        uint64 endLoc
        )
    {
        // Move to start of search
        file->SeekAbsolute(startLoc);

        uint64 newLoc = startLoc;
        while (endLoc > (newLoc + sizeof(ChunkHeader)))
        {
            Platform::Array<byte>^ headerBytes = file->Read(sizeof(ChunkHeader));
            ChunkHeader* header = reinterpret_cast<ChunkHeader*>(headerBytes->Data);
            if (header->tag == tag)
            {
                // Found the requested tag
                return newLoc;
            }
            file->SeekRelative(static_cast<int64>(header->size));
            newLoc += header->size + sizeof(*header);
        }

        // Chunk with sent tag was not found
        throw ref new Platform::FailureException();
    }

    //
    // Read the riff chunk header at the send location
    //
    void ReadHeader(
        RandomAccessReader^ file,
        uint64 loc,
        ChunkHeader& header
        )
    {
        // Move to read location
        file->SeekAbsolute(loc);
        Platform::Array<byte>^ headerBytes = file->Read(sizeof(ChunkHeader));
        header = *reinterpret_cast<ChunkHeader*>(headerBytes->Data);
    }
}

//--------------------------------------------------------------------------------------
// Name: SoundFileReader constructor
// Desc: Any failure to construct will throw.
//       If the constructor succeeds this is a fully usable object
//--------------------------------------------------------------------------------------
SoundFileReader::SoundFileReader(
    _In_ Platform::String^ fileName
    )
{
    //
    // Open the file for random read access
    //
    RandomAccessReader^ riffFile = ref new RandomAccessReader(fileName);
    uint64 fileSize = riffFile->GetFileSize();

    //
    // Locate, read and validate the RIFF chunk
    //
    uint64 riffLoc = FindChunk(riffFile, FOURCC_RIFF_TAG, 0, fileSize);

    // Read beyond the riff header.
    ChunkHeader chunkHeader;
    ReadHeader(riffFile, riffLoc, chunkHeader);

    uint32 tag = 0;
    Platform::Array<byte>^ riffData = riffFile->Read(sizeof(tag));
    tag = *reinterpret_cast<uint32*>(riffData->Data);
    if (tag != FOURCC_WAVE_FILE_TAG)
    {
        // Only support .wav files
        throw ref new Platform::FailureException();
    }
    uint64 riffChildrenStart = riffLoc + sizeof(chunkHeader) + sizeof(tag);
    uint64 riffChildrenEnd   = riffLoc + sizeof(chunkHeader) + chunkHeader.size;

    //
    // Find, read and validate the format chunk (a child within the RIFF chunk)
    //
    uint64 formatLoc = FindChunk(riffFile, FOURCC_FORMAT_TAG, riffChildrenStart, riffChildrenEnd);
    ReadHeader(riffFile, formatLoc, chunkHeader);
    if (chunkHeader.size < sizeof(WAVEFORMATEX))
    {
        // Format data of unsupported size; must be unsupported format
        throw ref new Platform::FailureException();
    }
    m_soundFormat = riffFile->Read(chunkHeader.size);
    WAVEFORMATEX format = *reinterpret_cast<WAVEFORMATEX*>(m_soundFormat->Data);
    if (format.wFormatTag != WAVE_FORMAT_PCM
        && format.wFormatTag != WAVE_FORMAT_ADPCM)
    {
        // This is not PCM or ADPCM, which is all we support
        throw ref new Platform::FailureException();
    }

    //
    // Locate, the PCM data in the data chunk
    //
    uint64 dataChunkStart = FindChunk(riffFile, FOURCC_DATA_TAG, riffChildrenStart, riffChildrenEnd);
    ReadHeader(riffFile, dataChunkStart, chunkHeader);

    //
    // Now read the PCM data and setup the buffer
    //
    m_soundData = riffFile->Read(chunkHeader.size);
}

//--------------------------------------------------------------------------------------
// Name: SoundFileReader::GetSoundData
// Desc: Sound Data accessor
//--------------------------------------------------------------------------------------
Platform::Array<byte>^ SoundFileReader::GetSoundData() const
{
    return m_soundData;
}

//--------------------------------------------------------------------------------------
// Name: SoundFileReader::GetSoundFormat
// Desc: Sound Format Accessor
//--------------------------------------------------------------------------------------
WAVEFORMATEX* SoundFileReader::GetSoundFormat() const
{
    return reinterpret_cast<WAVEFORMATEX*>(m_soundFormat->Data);
}
