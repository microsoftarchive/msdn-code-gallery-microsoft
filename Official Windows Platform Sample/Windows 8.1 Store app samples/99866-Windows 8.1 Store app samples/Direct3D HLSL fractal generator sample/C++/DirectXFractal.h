//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DirectXBase.h"
#include "SampleOverlay.h"
#include "AutoThrottle.h"

namespace SampleSettings
{
    static const int TargetIterations = 256;
    namespace Performance
    {
        static const float TargetFrameTime = 1.0f / 20.0f;
        static const int InitialIterationsPerFrame = 32;
        static const float InitialDownSampleRatio = 3.0f;
        static const float DownSampleRatioMax = 8.0f;
        static const float DownSampleRatioDeltaDecrease = 0.01f;
        static const float DownSampleRatioDeltaIncrease = 0.05f;
    }
    namespace Manipulation
    {
        static const double MinimumF8Scale = 1.0f;
        static const double MinimumF16Scale = 1.0e-2f;
        static const double MinimumF32Scale = 2.0e-5f;
        static const double MinimumF64Scale = 1.0e-13;
        static const double MaximumScale = 4.0;
        static const float MaximumTranslationMagnitude = 2.0f;
    }
    namespace DirectCompute
    {
        static const int ThreadGroupSizeX = 16;
        static const int ThreadGroupSizeY = 16;
    }
    namespace ThreePhase
    {
        static const int InitialIterations = 4;
        static const int StepIterations = 4;
        static const int FinalIterations = 4;
    }
}

enum class FractalTechnique
{
    ThreePhase,
    SinglePass,
    DirectCompute
};

struct ViewTransform
{
    double scale;
    double rotation;
    double translationX;
    double translationY;
};

struct FractalVertex
{
    float2 pos;
    float2 tex;
    float2 bounds;
};

struct DrawRectCB
{
    float4x4 boundsTransform;
    float2 texScale;
};

struct ComputeCB
{
    float2 originDtid00;
    float2 deltaPerDtidX;
    float2 deltaPerDtidY;
};

struct ComputeDoubleCB
{
    double2 originDtid00;
    double2 deltaPerDtidX;
    double2 deltaPerDtidY;
};

struct FractalBufferElement
{
    int iteration;
};


ref class DirectXFractal : public DirectXBase
{
internal:
    DirectXFractal();

    virtual void HandleDeviceLost() override;
    virtual void CreateDeviceIndependentResources() override;
    virtual void CreateDeviceResources() override;
    virtual void CreateWindowSizeDependentResources() override;
    virtual void Render() override;
    void Update(float timeTotal, float timeDelta);
    void HandleViewManipulation(float dRotation, float dScale, float dX, float dY);
    void SaveInternalState(_In_ Windows::Foundation::Collections::IPropertySet^ state);
    void LoadInternalState(_In_ Windows::Foundation::Collections::IPropertySet^ state);

private:
    void SaveInternalValue(
        _In_ Windows::Foundation::Collections::IPropertySet^ state,
        _In_ Platform::String^ key,
        _In_ Platform::Object^ value
        );
    int RoundUpTo16(int value);
    void DetermineFractalTechnique();

    SampleOverlay^                                    m_sampleOverlay;
    AutoThrottle^                                     m_movingThrottle;
    Microsoft::WRL::ComPtr<ID3D11InputLayout>         m_inputLayout;
    Microsoft::WRL::ComPtr<ID3D11VertexShader>        m_vertexShader;
    Microsoft::WRL::ComPtr<ID3D11PixelShader>         m_renderTexturePS;
    Microsoft::WRL::ComPtr<ID3D11Buffer>              m_vertexBuffer;
    Microsoft::WRL::ComPtr<ID3D11Buffer>              m_indexBuffer;
    Microsoft::WRL::ComPtr<ID3D11PixelShader>         m_singlePassPS;
    Microsoft::WRL::ComPtr<ID3D11PixelShader>         m_initialPS;
    Microsoft::WRL::ComPtr<ID3D11PixelShader>         m_stepPS;
    Microsoft::WRL::ComPtr<ID3D11PixelShader>         m_finalPS;
    Microsoft::WRL::ComPtr<ID3D11ComputeShader>       m_directComputeCS;
    Microsoft::WRL::ComPtr<ID3D11PixelShader>         m_renderBufferPS;
    Microsoft::WRL::ComPtr<ID3D11Buffer>              m_drawRectCB;
    Microsoft::WRL::ComPtr<ID3D11Buffer>              m_computeCB;
    Microsoft::WRL::ComPtr<ID3D11Buffer>              m_bufferInfoCB;
    Microsoft::WRL::ComPtr<ID3D11ShaderResourceView>  m_gradient;
    Microsoft::WRL::ComPtr<ID3D11SamplerState>        m_linearSampler;
    Microsoft::WRL::ComPtr<ID3D11RenderTargetView>    m_tempRTV[2];
    Microsoft::WRL::ComPtr<ID3D11UnorderedAccessView> m_tempUAV;
    Microsoft::WRL::ComPtr<ID3D11ShaderResourceView>  m_tempSRV[2];
    Microsoft::WRL::ComPtr<ID3D11RenderTargetView>    m_finalRTV;
    Microsoft::WRL::ComPtr<ID3D11ShaderResourceView>  m_finalSRV;
    Microsoft::WRL::ComPtr<ID3D11SamplerState>        m_pointSampler;
    ViewTransform                                     m_transform;
    int                                               m_indexCount;
    bool                                              m_fastGenerate;
    bool                                              m_complete;
    float                                             m_downSampleRatio;
    bool                                              m_useDoubles;
    bool                                              m_useUnormTextures;
    int                                               m_bufferWidth;
    int                                               m_bufferHeight;
    int                                               m_numIterationsPerFrame;
    int                                               m_completedIterations;
    FractalTechnique                                  m_technique;
};
