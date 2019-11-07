//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

//
// The SDK Mesh format (.sdkmesh) is not a recommended file format for production games.
// It was designed to meet the specific needs of the SDK samples.  Any real-world
// applications should avoid this file format in favor of a destination format that
// meets the specific needs of the application.
//

#include "pch.h"

using namespace DirectX;

#include "DDSTextureLoader.h"
#include "SDKMesh.h"
#include "BasicLoader.h"

void SDKMesh::LoadMaterials(ID3D11Device* d3dDevice, _In_reads_(numMaterials) SDKMESH_MATERIAL* materials, uint32 numMaterials)
{
    BasicLoader^ loader = ref new BasicLoader(d3dDevice);

    wchar_t path[MAX_PATH];
    wchar_t texturePath[MAX_PATH];
    size_t convertedChars = 0;

        // This is a simple Mesh format that doesn't reuse texture data
        for (uint32 m = 0; m < numMaterials; m++)
        {
            materials[m].DiffuseTexture = nullptr;
            materials[m].NormalTexture = nullptr;
            materials[m].SpecularTexture = nullptr;
            materials[m].DiffuseRV = nullptr;
            materials[m].NormalRV = nullptr;
            materials[m].SpecularRV = nullptr;

            // load textures
            if (materials[m].DiffuseTextureName[0] != 0)
            {
                size_t size = strlen(materials[m].DiffuseTextureName) + 1;
                mbstowcs_s(&convertedChars, texturePath, size, materials[m].DiffuseTextureName, _TRUNCATE);
                swprintf_s(path, MAX_PATH, L"Media\\Textures\\%s", texturePath);
                loader->LoadTexture(ref new Platform::String(path), nullptr, &materials[m].DiffuseRV);
            }
        }
}

HRESULT SDKMesh::CreateVertexBuffer(ID3D11Device* d3dDevice, SDKMESH_VERTEX_BUFFER_HEADER* header, void* vertices)
{
    HRESULT hr = S_OK;
    header->DataOffset = 0;

    // Vertex Buffer
    D3D11_BUFFER_DESC bufferDesc;
    bufferDesc.ByteWidth = static_cast<uint32>(header->SizeBytes);
    bufferDesc.Usage = D3D11_USAGE_DEFAULT;
    bufferDesc.BindFlags = D3D11_BIND_VERTEX_BUFFER;
    bufferDesc.CPUAccessFlags = 0;
    bufferDesc.MiscFlags = 0;

    D3D11_SUBRESOURCE_DATA initData;
    initData.pSysMem = vertices;
    hr = d3dDevice->CreateBuffer(&bufferDesc, &initData, &header->VertexBuffer);

    return hr;
}

HRESULT SDKMesh::CreateIndexBuffer(ID3D11Device* d3dDevice, SDKMESH_INDEX_BUFFER_HEADER* header, void* indices)
{
    HRESULT hr = S_OK;
    header->DataOffset = 0;

    // Index Buffer
    D3D11_BUFFER_DESC bufferDesc;
    bufferDesc.ByteWidth = static_cast<uint32>(header->SizeBytes);
    bufferDesc.Usage = D3D11_USAGE_DEFAULT;
    bufferDesc.BindFlags = D3D11_BIND_INDEX_BUFFER;
    bufferDesc.CPUAccessFlags = 0;
    bufferDesc.MiscFlags = 0;

    D3D11_SUBRESOURCE_DATA initData;
    initData.pSysMem = indices;
    hr = d3dDevice->CreateBuffer(&bufferDesc, &initData, &header->IndexBuffer);

    return hr;
}

HRESULT SDKMesh::CreateFromFile(ID3D11Device* d3dDevice, WCHAR* filename, bool createAdjacencyIndices)
{
    HRESULT hr = S_OK;

    m_hFile = CreateFile2(filename, GENERIC_READ, FILE_SHARE_READ, OPEN_EXISTING, nullptr);

    if (INVALID_HANDLE_VALUE == m_hFile)
    {
        DWORD errorCode = GetLastError();
        const int msgSize = 512;
        WCHAR message[msgSize];

        DWORD result = FormatMessage(
            FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
            nullptr,
            errorCode,
            MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
            message,
            msgSize,
            nullptr
            );
        if ((result > 0) && (result < msgSize))
        {
            OutputDebugString(message);
        }

        return E_FAIL;
    }

    // Store the directory path without the filename
    wcsncpy_s(m_path, MAX_PATH, filename, wcslen(filename));
    WCHAR* lastBSlash = wcsrchr(m_path, L'\\');
    if (lastBSlash)
    {
        *(lastBSlash + 1) = L'\0';
    }
    else
    {
        *m_path = L'\0';
    }

    // Get the file size
    FILE_STANDARD_INFO fileInfo = {0};
    if (!GetFileInformationByHandleEx(m_hFile, FileStandardInfo, &fileInfo, sizeof(fileInfo)))
    {
        throw ref new Platform::FailureException();
    }

    if (fileInfo.EndOfFile.HighPart != 0)
    {
        throw ref new Platform::OutOfMemoryException();
    }

    uint32 byteCount = fileInfo.EndOfFile.LowPart;

    // Allocate memory
    m_staticMeshData = new byte[byteCount];
    if (!m_staticMeshData)
    {
        CloseHandle(m_hFile);
        return E_OUTOFMEMORY;
    }

    // Read in the file
    DWORD bytesRead;
    if (!ReadFile(m_hFile, m_staticMeshData, byteCount, &bytesRead, nullptr))
    {
        hr = E_FAIL;
    }

    CloseHandle(m_hFile);

    if (SUCCEEDED(hr))
    {
        hr = CreateFromMemory(d3dDevice, m_staticMeshData, byteCount, createAdjacencyIndices, false);
        if (FAILED(hr))
        {
            delete[] m_staticMeshData;
        }
    }

    return hr;
}

HRESULT SDKMesh::CreateFromMemory(ID3D11Device* d3dDevice, byte* data, uint32 byteCount, bool createAdjacencyIndices, bool copyStatic)
{
    HRESULT hr = E_FAIL;
    XMFLOAT3 lower;
    XMFLOAT3 upper;

    m_d3dDevice = d3dDevice;

    m_numOutstandingResources = 0;

    if (copyStatic)
    {
        SDKMESH_HEADER* header = (SDKMESH_HEADER*)data;

        size_t staticSize = static_cast<size_t>(header->HeaderSize + header->NonBufferDataSize);
        m_heapData = new BYTE[staticSize];
        if (m_heapData == nullptr)
        {
            return hr;
        }
        m_staticMeshData = m_heapData;
        CopyMemory(m_staticMeshData, data, staticSize);
    }
    else
    {
        m_heapData = data;
        m_staticMeshData = data;
    }

    // Pointer fixup
    m_meshHeader = (SDKMESH_HEADER*)m_staticMeshData;
    m_vertexBufferArray = (SDKMESH_VERTEX_BUFFER_HEADER*)(m_staticMeshData + m_meshHeader->VertexStreamHeadersOffset);
    m_indexBufferArray = (SDKMESH_INDEX_BUFFER_HEADER*)(m_staticMeshData + m_meshHeader->IndexStreamHeadersOffset);
    m_meshArray = (SDKMESH_MESH*)(m_staticMeshData + m_meshHeader->MeshDataOffset);
    m_subsetArray = (SDKMESH_SUBSET*)(m_staticMeshData + m_meshHeader->SubsetDataOffset);
    m_frameArray = (SDKMESH_FRAME*)(m_staticMeshData + m_meshHeader->FrameDataOffset);
    m_materialArray = (SDKMESH_MATERIAL*)(m_staticMeshData + m_meshHeader->MaterialDataOffset);

    // Setup subsets
    for (uint32 i = 0; i < m_meshHeader->NumMeshes; i++)
    {
        m_meshArray[i].Subsets = (uint32*)(m_staticMeshData + m_meshArray[i].SubsetOffset);
        m_meshArray[i].FrameInfluences = (uint32*)(m_staticMeshData + m_meshArray[i].FrameInfluenceOffset);
    }

    // error condition
    if (m_meshHeader->Version != SDKMESH_FILE_VERSION)
    {
        hr = E_NOINTERFACE;
        goto Error;
    }

    // Setup buffer data pointer
    byte* bufferData = data + m_meshHeader->HeaderSize + m_meshHeader->NonBufferDataSize;

    // Get the start of the buffer data
    uint64 bufferDataStart = m_meshHeader->HeaderSize + m_meshHeader->NonBufferDataSize;

    // Create vertex buffers
    m_vertices = new byte*[m_meshHeader->NumVertexBuffers];
    for (uint32 i = 0; i < m_meshHeader->NumVertexBuffers; i++)
    {
        byte* vertices = nullptr;
        vertices = (byte*)(bufferData + (m_vertexBufferArray[i].DataOffset - bufferDataStart));
        CreateVertexBuffer(d3dDevice, &m_vertexBufferArray[i], vertices);
        m_vertices[i] = vertices;
    }

    // Create index buffers
    m_indices = new byte*[m_meshHeader->NumIndexBuffers];
    for (uint32 i = 0; i < m_meshHeader->NumIndexBuffers; i++)
    {
        byte* indices = nullptr;
        indices = (byte*)(bufferData + (m_indexBufferArray[i].DataOffset - bufferDataStart));
        CreateIndexBuffer(d3dDevice, &m_indexBufferArray[i], indices);
        m_indices[i] = indices;
    }

    // Load Materials
    if (d3dDevice)
    {
        LoadMaterials(d3dDevice, m_materialArray, m_meshHeader->NumMaterials);
    }

    // Create a place to store our bind pose frame matrices
    m_bindPoseFrameMatrices = new XMMATRIX[m_meshHeader->NumFrames];
    if (!m_bindPoseFrameMatrices)
    {
        goto Error;
    }

    // Create a place to store our transformed frame matrices
    m_transformedFrameMatrices = new XMMATRIX[m_meshHeader->NumFrames];
    if (!m_transformedFrameMatrices)
    {
        goto Error;
    }
    m_worldPoseFrameMatrices = new XMMATRIX[m_meshHeader->NumFrames];
    if (!m_worldPoseFrameMatrices)
    {
        goto Error;
    }

    SDKMESH_SUBSET* subset = nullptr;
    D3D11_PRIMITIVE_TOPOLOGY primitiveType;

    // update bounding volume
    SDKMESH_MESH* currentMesh = &m_meshArray[0];
    int tris = 0;
    for (uint32 mesh = 0; mesh < m_meshHeader->NumMeshes; ++mesh)
    {
        lower.x = XMVectorGetX(g_XMFltMax); lower.y = XMVectorGetX(g_XMFltMax); lower.z = XMVectorGetX(g_XMFltMax);
        upper.x = -XMVectorGetX(g_XMFltMax); upper.y = -XMVectorGetX(g_XMFltMax); upper.z = -XMVectorGetX(g_XMFltMax);
        currentMesh = GetMesh(mesh);

        int indsize;
        if (m_indexBufferArray[currentMesh->IndexBuffer].IndexType == static_cast<uint32>(SDKMeshIndexType::Bits16))
        {
            indsize = 2;
        }
        else
        {
            indsize = 4;
        }

        for (uint32 subsetIndex = 0; subsetIndex < currentMesh->NumSubsets; subsetIndex++)
        {
            subset = GetSubset(mesh, subsetIndex); // &m_pSubsetArray[currentMesh->pSubsets[subset]];

            primitiveType = GetPrimitiveType(static_cast<SDKMeshPrimitiveType>(subset->PrimitiveType));
            assert(primitiveType == D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST); // only triangle lists are handled.

            uint32 indexCount = (uint32)subset->IndexCount;
            uint32 indexStart = (uint32)subset->IndexStart;

            uint32* ind = (uint32*)m_indices[currentMesh->IndexBuffer];
            float* verts = (float*)m_vertices[currentMesh->VertexBuffers[0]];
            uint32 stride = (uint32)m_vertexBufferArray[currentMesh->VertexBuffers[0]].StrideBytes;
            assert (stride % 4 == 0);
            stride /= 4;
            for (uint32 vertind = indexStart; vertind < indexStart + indexCount; ++vertind)
            {
                uint32 currentIndex = 0;
                if (indsize == 2)
                {
                    uint32 ind_div2 = vertind / 2;
                    currentIndex = ind[ind_div2];
                    if ((vertind % 2) == 0)
                    {
                        currentIndex = currentIndex << 16;
                        currentIndex = currentIndex >> 16;
                    }
                    else
                    {
                        currentIndex = currentIndex >> 16;
                    }
                }
                else
                {
                    currentIndex = ind[vertind];
                }
                tris++;
                XMFLOAT3* pt = (XMFLOAT3*)&(verts[stride * currentIndex]);
                if (pt->x < lower.x)
                {
                    lower.x = pt->x;
                }
                if (pt->y < lower.y)
                {
                    lower.y = pt->y;
                }
                if (pt->z < lower.z)
                {
                    lower.z = pt->z;
                }
                if (pt->x > upper.x)
                {
                    upper.x = pt->x;
                }
                if (pt->y > upper.y)
                {
                    upper.y = pt->y;
                }
                if (pt->z > upper.z)
                {
                    upper.z = pt->z;
                }
            }
        }
        XMVECTOR u = XMLoadFloat3(&upper);
        XMVECTOR l = XMLoadFloat3(&lower);
        XMVECTOR half = XMVectorSubtract(u, l);
        XMVectorScale(half, 0.5f);
        XMStoreFloat3(&currentMesh->BoundingBoxExtents, half);
        half = XMVectorAdd(l, half);
        XMStoreFloat3(&currentMesh->BoundingBoxCenter, half);
    }
    // Update
    hr = S_OK;
Error:
    return hr;
}

// Transform bind pose frame using a recursive traversal
void SDKMesh::TransformBindPoseFrame(uint32 frame, CXMMATRIX parentWorld)
{
    if (m_bindPoseFrameMatrices == nullptr)
    {
        return;
    }

    // Transform ourselves
    XMMATRIX localWorld = XMMatrixMultiply(XMLoadFloat4x4(&m_frameArray[frame].Matrix), parentWorld);
    m_bindPoseFrameMatrices[frame] = localWorld;

    // Transform our siblings
    if (m_frameArray[frame].SiblingFrame != INVALID_FRAME)
    {
        TransformBindPoseFrame(m_frameArray[frame].SiblingFrame, parentWorld);
    }

    // Transform our children
    if (m_frameArray[frame].ChildFrame != INVALID_FRAME)
    {
        TransformBindPoseFrame(m_frameArray[frame].ChildFrame, localWorld);
    }
}

// Transform frame using a recursive traversal
void SDKMesh::TransformFrame(uint32 frame, CXMMATRIX parentWorld, double time)
{
    // Get the tick data
    XMMATRIX localTransform;
    uint32 tick = GetAnimationKeyFromTime(time);

    if (INVALID_ANIMATION_DATA != m_frameArray[frame].AnimationDataIndex)
    {
        SDKANIMATION_FRAME_DATA* frameData = &m_animationFrameData[m_frameArray[frame].AnimationDataIndex];
        SDKANIMATION_DATA* data = &frameData->AnimationData[tick];

        // turn it into a matrix (ignore scaling for now)
        XMMATRIX translate = XMMatrixTranslation(data->Translation.x, data->Translation.y, data->Translation.z);
        XMVECTOR quatVector;
        XMMATRIX quatMatrix;

        if (data->Orientation.w == 0 && data->Orientation.x == 0 && data->Orientation.y == 0 && data->Orientation.z == 0)
        {
            quatVector = XMQuaternionIdentity();
        }
        else
        {
            quatVector = XMLoadFloat4(&data->Orientation);
            quatVector = XMVectorSwizzle(quatVector, 3, 0, 1, 2);
        }

        quatVector = XMQuaternionNormalize(quatVector);
        quatMatrix = XMMatrixRotationQuaternion(quatVector);
        localTransform = (quatMatrix * translate);
    }
    else
    {
        localTransform = XMLoadFloat4x4(&m_frameArray[frame].Matrix);
    }

    // Transform ourselves
    XMMATRIX localWorld = XMMatrixMultiply(localTransform, parentWorld);
    m_transformedFrameMatrices[frame] = localWorld;
    m_worldPoseFrameMatrices[frame] = localWorld;

    // Transform our siblings
    if (m_frameArray[frame].SiblingFrame != INVALID_FRAME)
    {
        TransformFrame(m_frameArray[frame].SiblingFrame, parentWorld, time);
    }

    // Transform our children
    if (m_frameArray[frame].ChildFrame != INVALID_FRAME)
    {
        TransformFrame(m_frameArray[frame].ChildFrame, localWorld, time);
    }
}

// Transform frame assuming that it is an absolute transformation
void SDKMesh::TransformFrameAbsolute(uint32 frame, double time)
{
    XMMATRIX trans1;
    XMMATRIX trans2;
    XMMATRIX rot1;
    XMMATRIX rot2;
    XMVECTOR quat1;
    XMVECTOR quat2;
    XMMATRIX to;
    XMMATRIX invTo;
    XMMATRIX from;

    uint32 tick = GetAnimationKeyFromTime(time);

    if (INVALID_ANIMATION_DATA != m_frameArray[frame].AnimationDataIndex)
    {
        SDKANIMATION_FRAME_DATA* frameData = &m_animationFrameData[m_frameArray[frame].AnimationDataIndex];
        SDKANIMATION_DATA* data = &frameData->AnimationData[tick];
        SDKANIMATION_DATA* dataOrig = &frameData->AnimationData[0];

        trans1 = XMMatrixTranslation(-dataOrig->Translation.x, -dataOrig->Translation.y, -dataOrig->Translation.z);
        trans2 = XMMatrixTranslation(data->Translation.x, data->Translation.y, data->Translation.z);
        quat1 = XMLoadFloat4(&dataOrig->Orientation);
        quat1 = XMQuaternionInverse(quat1);
        rot1 = XMMatrixRotationQuaternion(quat1);
        invTo = trans1 * rot1;

        quat2 = XMLoadFloat4(&data->Orientation);
        rot2 = XMMatrixRotationQuaternion(quat2);
        from = rot2 * trans2;

        XMMATRIX output = invTo * from;
        m_transformedFrameMatrices[frame] = output;
    }
}

#define MAX_D3D11_VERTEX_STREAMS D3D11_IA_VERTEX_INPUT_RESOURCE_SLOT_COUNT
void SDKMesh::RenderMesh(uint32 meshIndex, bool adjacent, ID3D11DeviceContext* d3dContext, uint32 diffuseSlot, uint32 normalSlot, uint32 specularSlot)
{
    if (0 < GetOutstandingBufferResources())
    {
        return;
    }

    SDKMESH_MESH* mesh = &m_meshArray[meshIndex];
    uint32 strides[MAX_D3D11_VERTEX_STREAMS];
    uint32 offsets[MAX_D3D11_VERTEX_STREAMS];
    ID3D11Buffer* vertexBuffer[MAX_D3D11_VERTEX_STREAMS];

    ZeroMemory(strides, sizeof(uint32) * MAX_D3D11_VERTEX_STREAMS);
    ZeroMemory(offsets, sizeof(uint32) * MAX_D3D11_VERTEX_STREAMS);
    ZeroMemory(vertexBuffer, sizeof(ID3D11Buffer*) * MAX_D3D11_VERTEX_STREAMS);

    if (mesh->NumVertexBuffers > MAX_D3D11_VERTEX_STREAMS)
    {
        return;
    }

    for (uint64 i = 0; i < mesh->NumVertexBuffers; i++)
    {
        vertexBuffer[i] = m_vertexBufferArray[mesh->VertexBuffers[i]].VertexBuffer;
        strides[i] = static_cast<uint32>(m_vertexBufferArray[mesh->VertexBuffers[i]].StrideBytes);
        offsets[i] = 0;
    }

    SDKMESH_INDEX_BUFFER_HEADER* indexBufferArray;
    if (adjacent)
    {
        indexBufferArray = m_adjacencyIndexBufferArray;
    }
    else
    {
        indexBufferArray = m_indexBufferArray;
    }

    ID3D11Buffer* indexBuffer = indexBufferArray[mesh->IndexBuffer].IndexBuffer;
    DXGI_FORMAT indexBufferFormat = DXGI_FORMAT_R16_UINT;
    switch (indexBufferArray[mesh->IndexBuffer].IndexType)
    {
        case SDKMeshIndexType::Bits16:
            indexBufferFormat = DXGI_FORMAT_R16_UINT;
            break;
        case SDKMeshIndexType::Bits32:
            indexBufferFormat = DXGI_FORMAT_R32_UINT;
            break;
    };

    d3dContext->IASetVertexBuffers(0, mesh->NumVertexBuffers, vertexBuffer, strides, offsets);
    d3dContext->IASetIndexBuffer(indexBuffer, indexBufferFormat, 0);

    SDKMESH_SUBSET* subset = nullptr;
    SDKMESH_MATERIAL* material = nullptr;
    D3D11_PRIMITIVE_TOPOLOGY primitiveType;

    for (uint32 i = 0; i < mesh->NumSubsets; i++)
    {
        subset = &m_subsetArray[mesh->Subsets[i]];

        primitiveType = GetPrimitiveType(static_cast<SDKMeshPrimitiveType>(subset->PrimitiveType));
        if (adjacent)
        {
            switch (primitiveType)
            {
            case D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST:
                primitiveType = D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST_ADJ;
                break;
            case D3D11_PRIMITIVE_TOPOLOGY_TRIANGLESTRIP:
                primitiveType = D3D11_PRIMITIVE_TOPOLOGY_TRIANGLESTRIP_ADJ;
                break;
            case D3D11_PRIMITIVE_TOPOLOGY_LINELIST:
                primitiveType = D3D11_PRIMITIVE_TOPOLOGY_LINELIST_ADJ;
                break;
            case D3D11_PRIMITIVE_TOPOLOGY_LINESTRIP:
                primitiveType = D3D11_PRIMITIVE_TOPOLOGY_LINESTRIP_ADJ;
                break;
            }
        }

        d3dContext->IASetPrimitiveTopology(primitiveType);

        material = &m_materialArray[subset->MaterialID];
        if (diffuseSlot != INVALID_SAMPLER_SLOT && !IsErrorResource(material->DiffuseRV))
        {
            d3dContext->PSSetShaderResources(diffuseSlot, 1, &material->DiffuseRV);
        }
        if (normalSlot != INVALID_SAMPLER_SLOT && !IsErrorResource(material->NormalRV))
        {
            d3dContext->PSSetShaderResources(normalSlot, 1, &material->NormalRV);
        }
        if (specularSlot != INVALID_SAMPLER_SLOT && !IsErrorResource(material->SpecularRV))
        {
            d3dContext->PSSetShaderResources(specularSlot, 1, &material->SpecularRV);
        }

        uint32 indexCount = (uint32)subset->IndexCount;
        uint32 indexStart = (uint32)subset->IndexStart;
        uint32 vertexStart = (uint32)subset->VertexStart;

        if (adjacent)
        {
            indexCount *= 2;
            indexStart *= 2;
        }

        d3dContext->DrawIndexed(indexCount, indexStart, vertexStart);
    }
}

void SDKMesh::RenderFrame(uint32 frame, bool adjacent, ID3D11DeviceContext* d3dContext, uint32 diffuseSlot, uint32 normalSlot, uint32 specularSlot)
{
    if (!m_staticMeshData || !m_frameArray)
    {
        return;
    }

    if (m_frameArray[frame].Mesh != INVALID_MESH)
    {
        RenderMesh(m_frameArray[frame].Mesh, adjacent, d3dContext, diffuseSlot, normalSlot, specularSlot);
    }

    // Render our children
    if (m_frameArray[frame].ChildFrame != INVALID_FRAME)
    {
        RenderFrame(m_frameArray[frame].ChildFrame, adjacent, d3dContext, diffuseSlot, normalSlot, specularSlot);
    }

    // Render our siblings
    if (m_frameArray[frame].SiblingFrame != INVALID_FRAME)
    {
        RenderFrame(m_frameArray[frame].SiblingFrame, adjacent, d3dContext, diffuseSlot, normalSlot, specularSlot);
    }
}

SDKMesh::SDKMesh() : m_numOutstandingResources(0),
    m_loading(false),
    m_hFile(0),
    m_hFileMappingObject(0),
    m_meshHeader(nullptr),
    m_staticMeshData(nullptr),
    m_heapData(nullptr),
    m_adjacencyIndexBufferArray(nullptr),
    m_animationData(nullptr),
    m_animationHeader(nullptr),
    m_vertices(nullptr),
    m_indices(nullptr),
    m_bindPoseFrameMatrices(nullptr),
    m_transformedFrameMatrices(nullptr),
    m_worldPoseFrameMatrices(nullptr),
    m_d3dDevice(nullptr)
{
}

SDKMesh::~SDKMesh()
{
    Destroy();
}

HRESULT SDKMesh::Create(ID3D11Device* d3dDevice, WCHAR* filename, bool createAdjacencyIndices)
{
    Destroy();
    return CreateFromFile(d3dDevice, filename, createAdjacencyIndices);
}

HRESULT SDKMesh::Create(ID3D11Device* d3dDevice, byte* data, uint32 byteCount, bool createAdjacencyIndices, bool copyStatic)
{
    Destroy();
    return CreateFromMemory(d3dDevice, data, byteCount, createAdjacencyIndices, copyStatic);
}

HRESULT SDKMesh::LoadAnimation(WCHAR* filename)
{
    HRESULT hr = E_FAIL;
    DWORD bytesRead = 0;
    LARGE_INTEGER move;
    HANDLE fileHandle = CreateFile2(filename, GENERIC_READ, FILE_SHARE_READ, OPEN_EXISTING, nullptr);

    if (INVALID_HANDLE_VALUE == fileHandle)
    {
        return E_FAIL;
    }

    SDKANIMATION_FILE_HEADER fileHeader;
    if (!ReadFile(fileHandle, &fileHeader, sizeof(SDKANIMATION_FILE_HEADER), &bytesRead, nullptr))
    {
        goto Error;
    }

    SAFE_DELETE_ARRAY(m_animationData);
    m_animationData = new BYTE[static_cast<size_t>((sizeof(SDKANIMATION_FILE_HEADER) + fileHeader.AnimationDataSize))];
    if (m_animationData == nullptr)
    {
        hr = E_OUTOFMEMORY;
        goto Error;
    }

    move.QuadPart = 0;
    if (!SetFilePointerEx(fileHandle, move, nullptr, FILE_BEGIN))
    {
        goto Error;
    }

    if (!ReadFile(fileHandle, m_animationData, static_cast<DWORD>(sizeof(SDKANIMATION_FILE_HEADER) + fileHeader.AnimationDataSize),
            &bytesRead, nullptr))
    {
        goto Error;
    }

    // pointer fixup
    m_animationHeader = (SDKANIMATION_FILE_HEADER*)m_animationData;
    m_animationFrameData = (SDKANIMATION_FRAME_DATA*)(m_animationData + m_animationHeader->AnimationDataOffset);

    uint64 baseOffset = sizeof(SDKANIMATION_FILE_HEADER);
    for (uint32 i = 0; i < m_animationHeader->NumFrames; i++)
    {
        m_animationFrameData[i].AnimationData = (SDKANIMATION_DATA*)(m_animationData + m_animationFrameData[i].DataOffset + baseOffset);
        SDKMESH_FRAME* frame = FindFrame(m_animationFrameData[i].FrameName);
        if (frame)
        {
            frame->AnimationDataIndex = i;
        }
    }

    hr = S_OK;

Error:
    CloseHandle(fileHandle);
    return hr;
}

void SDKMesh::Destroy()
{
    if (m_staticMeshData != nullptr)
    {
        if (m_materialArray != nullptr)
        {
            for (uint64 m = 0; m < m_meshHeader->NumMaterials; m++)
            {
                ID3D11Resource* resource = nullptr;
                if (m_materialArray[m].DiffuseRV && !IsErrorResource(m_materialArray[m].DiffuseRV))
                {
                    m_materialArray[m].DiffuseRV->GetResource(&resource);
                    SAFE_RELEASE(resource);
                    SAFE_RELEASE(m_materialArray[m].DiffuseRV);
                }
                if (m_materialArray[m].NormalRV && !IsErrorResource(m_materialArray[m].NormalRV))
                {
                    m_materialArray[m].NormalRV->GetResource(&resource);
                    SAFE_RELEASE(resource);
                    SAFE_RELEASE(m_materialArray[m].NormalRV);
                }
                if (m_materialArray[m].SpecularRV && !IsErrorResource(m_materialArray[m].SpecularRV))
                {
                    m_materialArray[m].SpecularRV->GetResource(&resource);
                    SAFE_RELEASE(resource);
                    SAFE_RELEASE(m_materialArray[m].SpecularRV);
                }
            }
        }
    }

    if (m_adjacencyIndexBufferArray != nullptr)
    {
        for (uint64 i = 0; i < m_meshHeader->NumIndexBuffers; i++)
        {
            SAFE_RELEASE(m_adjacencyIndexBufferArray[i].IndexBuffer);
        }
    }

    if (m_indexBufferArray != nullptr)
    {
        for (uint64 i = 0; i < m_meshHeader->NumIndexBuffers; i++)
        {
            SAFE_RELEASE(m_indexBufferArray[i].IndexBuffer);
        }
    }

    if (m_vertexBufferArray != nullptr)
    {
        for (uint64 i = 0; i < m_meshHeader->NumVertexBuffers; i++)
        {
            SAFE_RELEASE(m_vertexBufferArray[i].VertexBuffer);
        }
    }

    SAFE_DELETE_ARRAY(m_adjacencyIndexBufferArray);
    SAFE_DELETE_ARRAY(m_heapData);

    m_staticMeshData = nullptr;

    SAFE_DELETE_ARRAY(m_animationData);
    SAFE_DELETE_ARRAY(m_bindPoseFrameMatrices);
    SAFE_DELETE_ARRAY(m_transformedFrameMatrices);
    SAFE_DELETE_ARRAY(m_worldPoseFrameMatrices);
    SAFE_DELETE_ARRAY(m_vertices);
    SAFE_DELETE_ARRAY(m_indices);

    m_meshHeader = nullptr;
    m_vertexBufferArray = nullptr;
    m_indexBufferArray = nullptr;
    m_meshArray = nullptr;
    m_subsetArray = nullptr;
    m_frameArray = nullptr;
    m_materialArray = nullptr;
    m_animationHeader = nullptr;
    m_animationFrameData = nullptr;
    m_d3dDevice = nullptr;
}

// Transform the bind pose
void SDKMesh::TransformBindPose(CXMMATRIX world)
{
    TransformBindPoseFrame(0, world);
}

// Transform the mesh frames according to the animation for the given time
void SDKMesh::TransformMesh(CXMMATRIX world, double time)
{
    if (m_animationHeader == nullptr || FrameTransformType::Relative == static_cast<FrameTransformType>(m_animationHeader->FrameTransformType))
    {
        TransformFrame(0, world, time);

        // For each frame, move the transform to the bind pose, then
        // move it to the final position
        XMMATRIX invBindPose;
        XMMATRIX final;
        for (uint32 i = 0; i < m_meshHeader->NumFrames; i++)
        {
            XMVECTOR determinant;
            invBindPose = XMMatrixInverse(&determinant, m_bindPoseFrameMatrices[i]);

            final = invBindPose * m_transformedFrameMatrices[i];
            m_transformedFrameMatrices[i] = final;
        }
    }
    else if (FrameTransformType::Absolute == static_cast<FrameTransformType>(m_animationHeader->FrameTransformType))
    {
        for (uint32 i = 0; i < m_animationHeader->NumFrames; i++)
            TransformFrameAbsolute(i, time);
    }
}

void SDKMesh::Render(ID3D11DeviceContext* d3dContext, uint32 diffuseSlot, uint32 normalSlot, uint32 specularSlot)
{
    RenderFrame(0, false, d3dContext, diffuseSlot, normalSlot, specularSlot);
}

void SDKMesh::RenderAdjacent(ID3D11DeviceContext* d3dContext, uint32 diffuseSlot, uint32 normalSlot, uint32 specularSlot)
{
    RenderFrame(0, true, d3dContext, diffuseSlot, normalSlot, specularSlot);
}

D3D11_PRIMITIVE_TOPOLOGY SDKMesh::GetPrimitiveType(SDKMeshPrimitiveType primitiveType)
{
    D3D11_PRIMITIVE_TOPOLOGY returnType = D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST;

    switch (primitiveType)
    {
        case SDKMeshPrimitiveType::TriangleList:
            returnType = D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST;
            break;
        case SDKMeshPrimitiveType::TriangleStrip:
            returnType = D3D11_PRIMITIVE_TOPOLOGY_TRIANGLESTRIP;
            break;
        case SDKMeshPrimitiveType::LineList:
            returnType = D3D11_PRIMITIVE_TOPOLOGY_LINELIST;
            break;
        case SDKMeshPrimitiveType::LineStrip:
            returnType = D3D11_PRIMITIVE_TOPOLOGY_LINESTRIP;
            break;
        case SDKMeshPrimitiveType::PointList:
            returnType = D3D11_PRIMITIVE_TOPOLOGY_POINTLIST;
            break;
        case SDKMeshPrimitiveType::TriangleListAdjacent:
            returnType = D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST_ADJ;
            break;
        case SDKMeshPrimitiveType::TriangleStripAdjacent:
            returnType = D3D11_PRIMITIVE_TOPOLOGY_TRIANGLESTRIP_ADJ;
            break;
        case SDKMeshPrimitiveType::LineListAdjacent:
            returnType = D3D11_PRIMITIVE_TOPOLOGY_LINELIST_ADJ;
            break;
        case SDKMeshPrimitiveType::LineStripAdjacent:
            returnType = D3D11_PRIMITIVE_TOPOLOGY_LINESTRIP_ADJ;
            break;
    };

    return returnType;
}

DXGI_FORMAT SDKMesh::GetIndexBufferFormat(uint32 mesh)
{
    switch (m_indexBufferArray[m_meshArray[mesh].IndexBuffer].IndexType)
    {
        case SDKMeshIndexType::Bits16:
            return DXGI_FORMAT_R16_UINT;
        case SDKMeshIndexType::Bits32:
            return DXGI_FORMAT_R32_UINT;
    };

    return DXGI_FORMAT_R16_UINT;
}

ID3D11Buffer* SDKMesh::GetVertexBuffer(uint32 mesh, uint32 vertexBuffer)
{
    return m_vertexBufferArray[m_meshArray[mesh].VertexBuffers[vertexBuffer]].VertexBuffer;
}

ID3D11Buffer* SDKMesh::GetIndexBuffer(uint32 mesh)
{
    return m_indexBufferArray[m_meshArray[mesh].IndexBuffer].IndexBuffer;
}

SDKMeshIndexType SDKMesh::GetIndexType(uint32 mesh)
{
    return static_cast<SDKMeshIndexType>(m_indexBufferArray[m_meshArray[mesh].IndexBuffer].IndexType);
}

ID3D11Buffer* SDKMesh::GetAdjacencyIndexBuffer(uint32 mesh)
{
    return m_adjacencyIndexBufferArray[m_meshArray[mesh].IndexBuffer].IndexBuffer;
}

WCHAR* SDKMesh::GetMeshPath()
{
    return m_path;
}

uint32 SDKMesh::GetNumMeshes()
{
    if (!m_meshHeader)
    {
        return 0;
    }

    return m_meshHeader->NumMeshes;
}

uint32 SDKMesh::GetNumMaterials()
{
    if (!m_meshHeader)
    {
        return 0;
    }

    return m_meshHeader->NumMaterials;
}

uint32 SDKMesh::GetNumVertexBuffers()
{
    if (!m_meshHeader)
    {
        return 0;
    }

    return m_meshHeader->NumVertexBuffers;
}

uint32 SDKMesh::GetNumIndexBuffers()
{
    if (!m_meshHeader)
    {
        return 0;
    }

    return m_meshHeader->NumIndexBuffers;
}

ID3D11Buffer* SDKMesh::GetVertexBufferAt(uint32 vertexBuffer)
{
    return m_vertexBufferArray[vertexBuffer].VertexBuffer;
}

ID3D11Buffer* SDKMesh::GetIndexBufferAt(uint32 indexBuffer)
{
    return m_indexBufferArray[indexBuffer].IndexBuffer;
}

byte* SDKMesh::GetRawVerticesAt(uint32 vertexBuffer)
{
    return m_vertices[vertexBuffer];
}

byte* SDKMesh::GetRawIndicesAt(uint32 indexBuffer)
{
    return m_indices[indexBuffer];
}

SDKMESH_MATERIAL* SDKMesh::GetMaterial(uint32 material)
{
    return &m_materialArray[material];
}

SDKMESH_MESH* SDKMesh::GetMesh(uint32 mesh)
{
    return &m_meshArray[mesh];
}

uint32 SDKMesh::GetNumSubsets(uint32 mesh)
{
    return m_meshArray[mesh].NumSubsets;
}

SDKMESH_SUBSET* SDKMesh::GetSubset(uint32 mesh, uint32 subset)
{
    return &m_subsetArray[m_meshArray[mesh].Subsets[subset]];
}

uint32 SDKMesh::GetVertexStride(uint32 mesh, uint32 vertexBuffer)
{
    return static_cast<uint32>(m_vertexBufferArray[m_meshArray[mesh].VertexBuffers[vertexBuffer]].StrideBytes);
}

uint32 SDKMesh::GetNumFrames()
{
    return m_meshHeader->NumFrames;
}

SDKMESH_FRAME* SDKMesh::GetFrame(uint32 frame)
{
    assert(frame < m_meshHeader->NumFrames);

    if (frame < m_meshHeader->NumFrames)
    {
        return &m_frameArray[frame];
    }

    return nullptr;
}

SDKMESH_FRAME* SDKMesh::FindFrame(char* name)
{
    for (uint32 i = 0; i < m_meshHeader->NumFrames; i++)
    {
        if (_stricmp(m_frameArray[i].Name, name) == 0)
        {
            return &m_frameArray[i];
        }
    }

    return nullptr;
}

uint64 SDKMesh::GetNumVertices(uint32 mesh, uint32 vertexBuffer)
{
    return m_vertexBufferArray[m_meshArray[mesh].VertexBuffers[vertexBuffer]].NumVertices;
}

uint64 SDKMesh::GetNumIndices(uint32 mesh)
{
    return m_indexBufferArray[m_meshArray[mesh].IndexBuffer].NumIndices;
}

XMFLOAT3 SDKMesh::GetMeshBoundingBoxCenter(uint32 mesh)
{
    return m_meshArray[mesh].BoundingBoxCenter;
}

XMFLOAT3 SDKMesh::GetMeshBoundingBoxExtents(uint32 mesh)
{
    return m_meshArray[mesh].BoundingBoxExtents;
}

uint32 SDKMesh::GetOutstandingResources()
{
    uint32 outstandingResources = 0;
    if (m_meshHeader == nullptr)
    {
        return 1;
    }

    outstandingResources += GetOutstandingBufferResources();

    if (m_d3dDevice != nullptr)
    {
        for (uint32 i = 0; i < m_meshHeader->NumMaterials; i++)
        {
            if (m_materialArray[i].DiffuseTextureName[0] != 0)
            {
                if (!m_materialArray[i].DiffuseRV && !IsErrorResource(m_materialArray[i].DiffuseRV))
                {
                    outstandingResources++;
                }
            }

            if (m_materialArray[i].NormalTextureName[0] != 0)
            {
                if (!m_materialArray[i].NormalRV && !IsErrorResource(m_materialArray[i].NormalRV))
                {
                    outstandingResources++;
                }
            }

            if (m_materialArray[i].SpecularTextureName[0] != 0)
            {
                if (!m_materialArray[i].SpecularRV && !IsErrorResource(m_materialArray[i].SpecularRV))
                {
                    outstandingResources++;
                }
            }
        }
    }

    return outstandingResources;
}

uint32 SDKMesh::GetOutstandingBufferResources()
{
    uint32 outstandingResources = 0;
    if (m_meshHeader == nullptr)
    {
        return 1;
    }

    for (uint32 i = 0; i < m_meshHeader->NumVertexBuffers; i++)
    {
        if ((m_vertexBufferArray[i].VertexBuffer == nullptr) && !IsErrorResource(m_vertexBufferArray[i].VertexBuffer))
        {
            outstandingResources++;
        }
    }

    for (uint32 i = 0; i < m_meshHeader->NumIndexBuffers; i++)
    {
        if ((m_indexBufferArray[i].IndexBuffer == nullptr) && !IsErrorResource(m_indexBufferArray[i].IndexBuffer))
        {
            outstandingResources++;
        }
    }

    return outstandingResources;
}

bool SDKMesh::CheckLoadDone()
{
    if (0 == GetOutstandingResources())
    {
        m_loading = false;
        return true;
    }

    return false;
}

bool SDKMesh::IsLoaded()
{
    if (m_staticMeshData && !m_loading)
    {
        return true;
    }

    return false;
}

bool SDKMesh::IsLoading()
{
    return m_loading;
}

void SDKMesh::SetLoading(bool loading)
{
    m_loading = loading;
}

bool SDKMesh::HadLoadingError()
{
    if (m_meshHeader != nullptr)
    {
        for (uint32 i = 0; i < m_meshHeader->NumVertexBuffers; i++)
        {
            if (IsErrorResource(m_vertexBufferArray[i].VertexBuffer))
            {
                return true;
            }
        }

        for (uint32 i = 0; i < m_meshHeader->NumIndexBuffers; i++)
        {
            if (IsErrorResource(m_indexBufferArray[i].IndexBuffer))
            {
                return true;
            }
        }
    }

    return false;
}

uint32 SDKMesh::GetNumInfluences(uint32 mesh)
{
    return m_meshArray[mesh].NumFrameInfluences;
}

uint32 SDKMesh::GetAnimationKeyFromTime(double time)
{
    if (m_animationHeader == nullptr)
    {
        return 0;
    }

    uint32 tick = static_cast<uint32>(m_animationHeader->AnimationFPS * time);

    tick = tick % (m_animationHeader->NumAnimationKeys - 1);
    tick++;

    return tick;
}

bool SDKMesh::GetAnimationProperties(uint32* numKeys, float* frameTime)
{
    if (m_animationHeader == nullptr)
    {
        return false;
    }

    *numKeys = m_animationHeader->NumAnimationKeys;
    *frameTime = 1.0f / static_cast<float>(m_animationHeader->AnimationFPS);

    return true;
}

const CXMMATRIX SDKMesh::GetMeshInfluenceMatrix(uint32 mesh, uint32 influence)
{
    uint32 frame = m_meshArray[mesh].FrameInfluences[influence];
    return m_transformedFrameMatrices[frame];
}

const CXMMATRIX SDKMesh::GetWorldMatrix(uint32 frameIndex)
{
    return m_worldPoseFrameMatrices[frameIndex];
}

const CXMMATRIX SDKMesh::GetInfluenceMatrix(uint32 frameIndex)
{
    return m_transformedFrameMatrices[frameIndex];
}

