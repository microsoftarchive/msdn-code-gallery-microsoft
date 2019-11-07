//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "ResidencyManager.h"

namespace TiledResources
{
    class TileLoader
    {
    public:
        TileLoader(const std::wstring & filename, std::vector<D3D11_SUBRESOURCE_TILING>* tilingInfo);
        concurrency::task<std::vector<byte>> LoadTileAsync(D3D11_TILED_RESOURCE_COORDINATE coordinate);
    private:
        std::vector<D3D11_SUBRESOURCE_TILING>* m_tilingInfo;
        std::vector<size_t> m_subresourceTileOffsets;
        Windows::Storage::Streams::IRandomAccessStream^ m_fileStream;
        concurrency::task<Windows::Storage::Streams::IRandomAccessStream^> m_openStreamTask;
        concurrency::task<std::vector<byte>> m_getFileTask;
        Microsoft::WRL::Wrappers::FileHandle m_fileHandle;
        std::wstring m_filename;
        UINT m_subresourcesPerFaceInResource;
        UINT m_subresourcesPerFaceInFile;
    };
}
