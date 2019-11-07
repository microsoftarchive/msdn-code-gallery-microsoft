//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "BasicMath.h"
#include <vector>
#include <map>
#include <memory>

namespace BasicSprites
{
    namespace Parameters
    {
        // The index buffer format for feature-level 9.1 devices may only be 16 bits.
        // With 4 vertices per sprite, this allows a maximum of (1 << 16) / 4 sprites.
        static unsigned int MaximumCapacityCompatible = (1 << 16) / 4;
    }

    enum class PositionUnits
    {
        DIPs,         // Interpret position as device-independent pixel values.
        Pixels,       // Interpret position as pixel values.
        Normalized,   // Interpret position as a fraction of the render target dimensions.
        UniformWidth, // Interpret position as a fraction of the render target width.
        UniformHeight // Interpret position as a fraction of the render target height.
    };

    enum class SizeUnits
    {
        DIPs,      // Interpret size as device-independent pixel values.
        Pixels,    // Interpret size as pixel values.
        Normalized // Interpret size as a multiplier of the pixel size of the sprite.
    };

    enum class BlendMode
    {
        Alpha,   // Use alpha blending (out = old * (1 - new.a) + new * new.a).
        Additive // Use additive blending (out = old + new * new.a).
    };

    enum class RenderTechnique
    {
        Replication,
        Instancing,
        GeometryShader
    };

    struct ReplicationVertex
    {
        float2 pos;
        float2 tex;
        unsigned int color;
    };

    struct InstancingVertex
    {
        float2 pos;
        float2 tex;
    };

    struct InstanceData
    {
        float2 origin;
        float2 offset;
        float rotation;
        unsigned int color;
    };

    struct TextureMapElement
    {
        Microsoft::WRL::ComPtr<ID3D11ShaderResourceView> srv;
        float2 size;
    };

    struct SpriteRunInfo
    {
        ID3D11ShaderResourceView* textureView;
        ID3D11BlendState1* blendState;
        unsigned int numSprites;
    };

    ref class SpriteBatch
    {
    internal:
        SpriteBatch();
        void Initialize(
            _In_ ID3D11Device1* d3dDevice,
            _In_ int capacity
            );
        void AddTexture(
            _In_ ID3D11Texture2D* texture
            );
        void RemoveTexture(
            _In_ ID3D11Texture2D* texture
            );
        void Begin();
        void End();
        void Draw(
            _In_ ID3D11Texture2D* texture,
            _In_ float2 position,
            _In_ PositionUnits positionUnits
            );
        void Draw(
            _In_ ID3D11Texture2D* texture,
            _In_ float2 position,
            _In_ PositionUnits positionUnits,
            _In_ float2 size,
            _In_ SizeUnits sizeUnits
            );
        void Draw(
            _In_ ID3D11Texture2D* texture,
            _In_ float2 position,
            _In_ PositionUnits positionUnits,
            _In_ float2 size,
            _In_ SizeUnits sizeUnits,
            _In_ float4 color
            );
        void Draw(
            _In_ ID3D11Texture2D* texture,
            _In_ float2 position,
            _In_ PositionUnits positionUnits,
            _In_ float2 size,
            _In_ SizeUnits sizeUnits,
            _In_ float4 color,
            _In_ float rotation
            );
        void Draw(
            _In_ ID3D11Texture2D* texture,
            _In_ float2 position,
            _In_ PositionUnits positionUnits,
            _In_ float2 size,
            _In_ SizeUnits sizeUnits,
            _In_ float4 color,
            _In_ float rotation,
            _In_ BlendMode blendMode
            );

    private:
        unsigned int MakeUnorm(float4 color);
        float2 StandardOrigin(float2 position, PositionUnits positionUnits, float2 renderTargetSize, float dpi);
        float2 StandardOffset(float2 size, SizeUnits sizeUnits, float2 spriteSize, float dpi);

        Microsoft::WRL::ComPtr<ID3D11Device1> m_d3dDevice;
        Microsoft::WRL::ComPtr<ID3D11DeviceContext1> m_d3dContext;
        Microsoft::WRL::ComPtr<ID3D11Buffer> m_vertexBuffer;
        Microsoft::WRL::ComPtr<ID3D11Buffer> m_instanceDataBuffer;
        Microsoft::WRL::ComPtr<ID3D11Buffer> m_indexBuffer;
        Microsoft::WRL::ComPtr<ID3D11BlendState1> m_blendStateAlpha;
        Microsoft::WRL::ComPtr<ID3D11BlendState1> m_blendStateAdditive;
        Microsoft::WRL::ComPtr<ID3D11InputLayout> m_inputLayout;
        Microsoft::WRL::ComPtr<ID3D11VertexShader> m_vertexShader;
        Microsoft::WRL::ComPtr<ID3D11GeometryShader> m_geometryShader;
        Microsoft::WRL::ComPtr<ID3D11PixelShader> m_pixelShader;
        Microsoft::WRL::ComPtr<ID3D11SamplerState> m_linearSampler;
        Microsoft::WRL::ComPtr<ID3D11Buffer> m_renderTargetInfoCbuffer;

        std::unique_ptr<ReplicationVertex[]> m_vertexData;
        std::unique_ptr<InstanceData[]> m_instanceData;
        std::map<ID3D11Texture2D*, TextureMapElement> m_textureMap;
        std::vector<SpriteRunInfo> m_spriteRuns;

        RenderTechnique m_technique;
        ID3D11ShaderResourceView* m_currentTextureView;
        ID3D11BlendState1* m_currentBlendState;
        float2 m_renderTargetSize;
        int m_capacity;
        int m_spritesInRun;
        int m_numSpritesDrawn;
        float m_dpi;
    };
}
