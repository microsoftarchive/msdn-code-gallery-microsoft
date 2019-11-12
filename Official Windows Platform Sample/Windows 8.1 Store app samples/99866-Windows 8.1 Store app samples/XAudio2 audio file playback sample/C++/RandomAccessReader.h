//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

// A simple reader class that provides support for random access to files.
ref class RandomAccessReader
{
internal:
    RandomAccessReader(_In_ Platform::String^ fileName);

    Platform::Array<byte>^ Read(_In_ size_t bytesToRead);

    void SeekRelative(_In_ int64 offset);
    void SeekAbsolute(_In_ int64 position);
    void SeekToStart();
    void SeekToEnd();

    uint64 GetFileSize();
    uint64 GetPosition();

private:
    Microsoft::WRL::Wrappers::FileHandle    m_file;
};
