//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"

using namespace Microsoft::WRL;
using namespace concurrency;
using namespace Windows::Foundation::Collections;
using namespace Windows::Storage;
using namespace Windows::Storage::Search;
using namespace Windows::Storage::Streams;
using namespace Windows::Storage::FileProperties;

FontLoader::FontLoader(
    _In_ StorageFolder^ location,
    _In_ IDWriteFactory* dwriteFactory
    ) :
    m_refCount(),
    m_location(location),
    m_fontFileCount(),
    m_fontFileStreams(),
    m_fontFileStreamIndex(),
    m_currentFontFile(),
    m_dwriteFactory(dwriteFactory)
{
}

task<void> FontLoader::LoadAsync()
{
    // Locate the "fonts" sub-folder within the document folder
    return task<StorageFolder^>(m_location->GetFolderAsync("fonts")).then([=](StorageFolder^ folder)
    {
        // Enumerate a list of .TTF files in the storage location
        auto filters = ref new Platform::Collections::Vector<Platform::String^>();
        filters->Append(".ttf");

        auto queryOptions = ref new QueryOptions(CommonFileQuery::DefaultQuery, filters);
        auto queryResult = folder->CreateFileQueryWithOptions(queryOptions);

        return queryResult->GetFilesAsync();

    }).then([=](IVectorView<StorageFile^>^ files)
    {
        m_fontFileCount = files->Size;

        std::vector<task<IBuffer^>> tasks;

        for (uint32 i = 0; i < m_fontFileCount; ++i)
        {
            auto file = dynamic_cast<StorageFile^>(files->GetAt(i));

            tasks.push_back(task<IBuffer^>(FileIO::ReadBufferAsync(file)));
        }

        return when_all(tasks.begin(), tasks.end());

    }).then([=](std::vector<IBuffer^> buffers)
    {
        for each (IBuffer^ buffer in buffers)
        {
            auto fileData = ref new Platform::Array<byte>(buffer->Length);
            DataReader::FromBuffer(buffer)->ReadBytes(fileData);

            ComPtr<FontFileStream> fontFileStream(new FontFileStream(fileData));
            m_fontFileStreams.push_back(fontFileStream);
        }
    });
}

HRESULT STDMETHODCALLTYPE FontLoader::QueryInterface(
    REFIID uuid,
    _Outptr_ void** object
    )
{
    if (    uuid == IID_IUnknown
        ||  uuid == __uuidof(IDWriteFontCollectionLoader)
        ||  uuid == __uuidof(IDWriteFontFileEnumerator)
        ||  uuid == __uuidof(IDWriteFontFileLoader)
        )
    {
        *object = this;
        AddRef();
        return S_OK;
    }
    else
    {
        *object = nullptr;
        return E_NOINTERFACE;
    }
}

ULONG STDMETHODCALLTYPE FontLoader::AddRef()
{
    return static_cast<ULONG>(InterlockedIncrement(&m_refCount));
}

ULONG STDMETHODCALLTYPE FontLoader::Release()
{
    ULONG newCount = static_cast<ULONG>(InterlockedDecrement(&m_refCount));

    if (newCount == 0)
        delete this;

    return newCount;
}

//  Called by DirectWrite to create an enumerator for the fonts in the font collection.
//  The font collection key being passed in is the same key the application passes to
//  DirectWrite when calling CreateCustomFontCollection API.
//
HRESULT STDMETHODCALLTYPE FontLoader::CreateEnumeratorFromKey(
    _In_ IDWriteFactory* factory,
    _In_reads_bytes_(fontCollectionKeySize) void const* fontCollectionKey,
    uint32 fontCollectionKeySize,
    _Outptr_ IDWriteFontFileEnumerator** fontFileEnumerator
    )
{
    *fontFileEnumerator = ComPtr<IDWriteFontFileEnumerator>(this).Detach();
    return S_OK;
}

HRESULT STDMETHODCALLTYPE FontLoader::MoveNext(OUT BOOL* hasCurrentFile)
{
    *hasCurrentFile = FALSE;

    if (m_fontFileStreamIndex < m_fontFileCount)
    {
        DX::ThrowIfFailed(
            m_dwriteFactory->CreateCustomFontFileReference(
                &m_fontFileStreamIndex,
                sizeof(size_t),
                this,
                &m_currentFontFile
                )
            );

        *hasCurrentFile = TRUE;
        ++m_fontFileStreamIndex;
    }

    return S_OK;
}

HRESULT STDMETHODCALLTYPE FontLoader::GetCurrentFontFile(OUT IDWriteFontFile** currentFontFile)
{
    *currentFontFile = ComPtr<IDWriteFontFile>(m_currentFontFile.Get()).Detach();
    return S_OK;
}

//  Called by DirectWrite to create a font file stream. The font file reference
//  key being passed in is the same key the application passes to DirectWrite
//  when calling CreateCustomFontFileReference.
//
HRESULT STDMETHODCALLTYPE FontLoader::CreateStreamFromKey(
    _In_reads_bytes_(fontFileReferenceKeySize) void const* fontFileReferenceKey,
    uint32 fontFileReferenceKeySize,
    _Outptr_ IDWriteFontFileStream** fontFileStream
    )
{
    if (fontFileReferenceKeySize != sizeof(size_t))
    {
        return E_INVALIDARG;
    }

    size_t fontFileStreamIndex = *(static_cast<size_t const*>(fontFileReferenceKey));

    *fontFileStream = ComPtr<IDWriteFontFileStream>(m_fontFileStreams.at(fontFileStreamIndex).Get()).Detach();

    return S_OK;
}
