//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DirectXHelper.h"
#include "SampleSettings.h"
#include "ResidencyManager.h"
#include "TileLoader.h"

using namespace TiledResources;

using namespace Windows::Storage;
using namespace Windows::Storage::Streams;
using namespace Windows::ApplicationModel;

using namespace concurrency;

TileLoader::TileLoader(const std::wstring & filename, std::vector<D3D11_SUBRESOURCE_TILING>* tilingInfo) :
    m_filename(filename),
    m_tilingInfo(tilingInfo)
{
    m_subresourcesPerFaceInResource = static_cast<UINT>(tilingInfo->size() / 6);
    UINT tilesForSingleFaceMostDetailedMip = tilingInfo->at(1).StartTileIndexInOverallResource;

    m_openStreamTask = create_task(Package::Current->InstalledLocation->GetFileAsync(Platform::StringReference(filename.c_str()))).then([this](StorageFile^ file)
    {
        return file->OpenReadAsync();
    }).then([this, tilesForSingleFaceMostDetailedMip](IRandomAccessStream^ stream)
    {
        UINT tilesPerFace = static_cast<UINT>((stream->Size / SampleSettings::TileSizeInBytes) / 6);
        size_t offset = 0;
        for (short face = 0; face < 6; face++)
        {
            UINT tileIndexInFace = 0;
            UINT tilesInSubresource = tilesForSingleFaceMostDetailedMip;
            m_subresourcesPerFaceInFile = 0;
            while (tileIndexInFace < tilesPerFace)
            {
                size_t offset = (face * tilesPerFace) + tileIndexInFace;
                m_subresourceTileOffsets.push_back(offset);
                tileIndexInFace += tilesInSubresource;
                tilesInSubresource = max(1, tilesInSubresource / 4);
                m_subresourcesPerFaceInFile++;
            }
        }
        return stream;
    });
}

task<std::vector<byte>> TileLoader::LoadTileAsync(D3D11_TILED_RESOURCE_COORDINATE coordinate)
{
    return m_openStreamTask.then([this, coordinate](IRandomAccessStream^ stream)
    {
        UINT subresourceInFile = (coordinate.Subresource / m_subresourcesPerFaceInResource) * m_subresourcesPerFaceInFile + coordinate.Subresource % m_subresourcesPerFaceInResource;
        size_t fileOffset = (m_subresourceTileOffsets[subresourceInFile] + (coordinate.Y * m_tilingInfo->at(coordinate.Subresource).WidthInTiles + coordinate.X)) * SampleSettings::TileSizeInBytes;
        DX::ThrowIfFailed(fileOffset <= stream->Size - SampleSettings::TileSizeInBytes ? S_OK : E_UNEXPECTED);
        auto reader = ref new DataReader(stream->GetInputStreamAt(fileOffset));
        return create_task(reader->LoadAsync(SampleSettings::TileSizeInBytes)).then([this, reader](unsigned int bytesRead)
        {
            DX::ThrowIfFailed(bytesRead == SampleSettings::TileSizeInBytes ? S_OK : E_UNEXPECTED);
            std::vector<byte> tileData(SampleSettings::TileSizeInBytes);
            reader->ReadBytes(Platform::ArrayReference<byte>(tileData.data(), SampleSettings::TileSizeInBytes));
            return tileData;
        });
    });
}
