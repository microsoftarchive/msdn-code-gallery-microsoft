//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"

using namespace DirectX;

#include "MarbleMaze.h"
#include "BasicLoader.h"
#include "BasicTimer.h"
#include "DDSTextureLoader.h"

using namespace Microsoft::WRL;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;
using namespace Windows::System;
using namespace Windows::Storage;
using namespace Windows::UI::ViewManagement;

MarbleMaze::MarbleMaze() :
    m_gameState(GameState::Initial),
    m_windowActive(false),
    m_viewState(ApplicationViewState::FullScreenLandscape),
    m_deferredResourcesReady(false)
{
    m_lightStrength = 0.0f;
    m_targetLightStrength = 0.0f;
    m_resetCamera = true;
    m_resetMarbleRotation = true;

    // Checkpoints (from start to goal).
    m_checkpoints.push_back(XMFLOAT3( 45.7f,  -43.6f, -45.0f)); // Start
    m_checkpoints.push_back(XMFLOAT3(120.7f,  -35.0f, -45.0f)); // Checkpoint 1
    m_checkpoints.push_back(XMFLOAT3(297.6f, -194.6f, -45.0f)); // Checkpoint 2
    m_checkpoints.push_back(XMFLOAT3(770.1f, -391.5f, -45.0f)); // Checkpoint 3
    m_checkpoints.push_back(XMFLOAT3(552.0f, -148.6f, -45.0f)); // Checkpoint 4
    m_checkpoints.push_back(XMFLOAT3(846.8f, -377.0f, -45.0f)); // Goal

    m_persistentState = ref new PersistentState();
    m_persistentState->Initialize(ApplicationData::Current->LocalSettings->Values, "MarbleMaze");
}

void MarbleMaze::Initialize(Windows::UI::Core::CoreWindow^ window, float dpi)
{
    // Apps must start processing events in 5 seconds or less,
    // so we only set up essential initial state and resources here.
    // Additional setup and resource loading happens in
    // LoadDeferredResources, which is typically called from
    // DirectXApp::Load().

    // Must happen before initializing base.
    m_audio.Initialize();

    // Returns accelerometer ref if there is one; nullptr otherwise.
    m_accelerometer = Windows::Devices::Sensors::Accelerometer::GetDefault();

    DirectXBase::Initialize(window, dpi);

    LoadState();
}

void MarbleMaze::HandleDeviceLost()
{
    m_deferredResourcesReady = false;

    m_d3dContext->OMSetRenderTargets(0, nullptr, nullptr);
    m_d3dDepthStencilView = nullptr;
    m_d3dRenderTargetView = nullptr;

    m_d2dContext->SetTarget(nullptr);
    m_d2dTargetBitmap = nullptr;
    m_d2dContext = nullptr;
    m_d2dDevice = nullptr;

    m_d3dContext->Flush();

    DirectXBase::HandleDeviceLost();

    LoadDeferredResources(false, true);
}

void MarbleMaze::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();
    m_camera = ref new Camera();
    m_sampleOverlay = ref new SampleOverlay();
    m_loadScreen = ref new LoadScreen();

    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        "DirectX Marble Maze game sample"
        );

    m_loadScreen->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get()
        );

    UserInterface::GetInstance().Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get()
        );
}

inline D2D1_RECT_F ConvertRect(Windows::Foundation::Rect source)
{
    // ignore the source.X and source.Y  These are the location on the screen
    // yet we don't want to use them because all coordinates are window relative.
    return D2D1::RectF(0.0f, 0.0f, source.Width, source.Height);
}

inline void InflateRect(D2D1_RECT_F& rect, float x, float y)
{
    rect.left -= x;
    rect.right += x;
    rect.top -= y;
    rect.bottom += y;
}

void MarbleMaze::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();

    // update the 3D projection matrix
    m_camera->SetProjectionParameters(
        40.0f,                                                  // use a 70-degree vertical field of view
        m_renderTargetSize.Width /  m_renderTargetSize.Height,  // specify the aspect ratio of the window
        50.0f,                                                  // specify the nearest Z-distance at which to draw vertices
        500.0f                                                  // specify the farthest Z-distance at which to draw vertice
        );

    // MarbleMaze specific
    float4x4 projection;
    m_camera->GetProjectionMatrix(&projection);
    m_mazeConstantBufferData.projection = projection;
    m_marbleConstantBufferData.projection = projection;

    // user interface
    const float padding = 32.0f;
    D2D1_RECT_F clientRect = ConvertRect(m_windowBounds);
    InflateRect(clientRect, -padding, -padding);

    bool snapped = m_viewState == ApplicationViewState::Snapped;

    D2D1_RECT_F topHalfRect = clientRect;
    topHalfRect.bottom = ((clientRect.top + clientRect.bottom) / 2.0f) - (padding / 2.0f);
    D2D1_RECT_F bottomHalfRect = clientRect;
    bottomHalfRect.top = topHalfRect.bottom + padding;

    m_startGameButton.Initialize();
    m_startGameButton.SetAlignment(AlignCenter, AlignFar);
    m_startGameButton.SetContainer(topHalfRect);
    m_startGameButton.SetText(L"Start Game");
    m_startGameButton.SetTextColor(D2D1::ColorF(D2D1::ColorF::White));
    m_startGameButton.GetTextStyle().SetFontWeight(DWRITE_FONT_WEIGHT_BLACK);
    if (snapped)
    {
        m_startGameButton.SetPadding(D2D1::SizeF(8.0f, 4.0f));
        m_startGameButton.GetTextStyle().SetFontSize(36.0f);
    }
    else
    {
        m_startGameButton.SetPadding(D2D1::SizeF(32.0f, 16.0f));
        m_startGameButton.GetTextStyle().SetFontSize(72.0f);
    }
    UserInterface::GetInstance().RegisterElement(&m_startGameButton);

    m_highScoreButton.Initialize();
    m_highScoreButton.SetAlignment(AlignCenter, AlignNear);
    m_highScoreButton.SetContainer(bottomHalfRect);
    m_highScoreButton.SetText(L"High Scores");
    m_highScoreButton.SetTextColor(D2D1::ColorF(D2D1::ColorF::White));
    m_highScoreButton.GetTextStyle().SetFontWeight(DWRITE_FONT_WEIGHT_BLACK);
    if (snapped)
    {
        m_highScoreButton.SetPadding(D2D1::SizeF(8.0f, 4.0f));
        m_highScoreButton.GetTextStyle().SetFontSize(36.0f);
    }
    else
    {
        m_highScoreButton.SetPadding(D2D1::SizeF(32.0f, 16.0f));
        m_highScoreButton.GetTextStyle().SetFontSize(72.0f);
    }
    UserInterface::GetInstance().RegisterElement(&m_highScoreButton);

    m_highScoreTable.Initialize();
    m_highScoreTable.SetAlignment(AlignCenter, AlignCenter);
    m_highScoreTable.SetContainer(clientRect);
    m_highScoreTable.SetTextColor(D2D1::ColorF(D2D1::ColorF::White));
    m_highScoreTable.GetTextStyle().SetFontWeight(DWRITE_FONT_WEIGHT_BOLD);
    m_highScoreTable.GetTextStyle().SetTextAlignment(DWRITE_TEXT_ALIGNMENT_CENTER);
    if (snapped)
    {
        m_highScoreTable.GetTextStyle().SetFontSize(20.0f);
    }
    else
    {
        m_highScoreTable.GetTextStyle().SetFontSize(60.0f);
    }
    UserInterface::GetInstance().RegisterElement(&m_highScoreTable);

    m_preGameCountdownTimer.Initialize();
    m_preGameCountdownTimer.SetAlignment(AlignCenter, AlignCenter);
    m_preGameCountdownTimer.SetContainer(clientRect);
    m_preGameCountdownTimer.SetTextColor(D2D1::ColorF(D2D1::ColorF::White));
    m_preGameCountdownTimer.GetTextStyle().SetFontWeight(DWRITE_FONT_WEIGHT_BLACK);
    if (snapped)
    {
        m_preGameCountdownTimer.GetTextStyle().SetFontSize(72.0f);
    }
    else
    {
        m_preGameCountdownTimer.GetTextStyle().SetFontSize(144.0f);
    }
    UserInterface::GetInstance().RegisterElement(&m_preGameCountdownTimer);

    m_inGameStopwatchTimer.Initialize();
    m_inGameStopwatchTimer.SetAlignment(AlignNear, AlignFar);
    m_inGameStopwatchTimer.SetContainer(clientRect);
    m_inGameStopwatchTimer.SetTextColor(D2D1::ColorF(D2D1::ColorF::White, 0.75f));
    m_inGameStopwatchTimer.GetTextStyle().SetFontWeight(DWRITE_FONT_WEIGHT_BLACK);
    if (snapped)
    {
        m_inGameStopwatchTimer.GetTextStyle().SetFontSize(48.0f);
    }
    else
    {
        m_inGameStopwatchTimer.GetTextStyle().SetFontSize(96.0f);
    }
    UserInterface::GetInstance().RegisterElement(&m_inGameStopwatchTimer);

    m_checkpointText.Initialize();
    m_checkpointText.SetAlignment(AlignCenter, AlignCenter);
    m_checkpointText.SetContainer(clientRect);
    m_checkpointText.SetText(L"Checkpoint!");
    m_checkpointText.SetTextColor(D2D1::ColorF(D2D1::ColorF::White));
    m_checkpointText.GetTextStyle().SetFontWeight(DWRITE_FONT_WEIGHT_BLACK);
    if (snapped)
    {
        m_checkpointText.GetTextStyle().SetFontSize(36.0f);
    }
    else
    {
        m_checkpointText.GetTextStyle().SetFontSize(72.0f);
    }
    UserInterface::GetInstance().RegisterElement(&m_checkpointText);

    m_pausedText.Initialize();
    m_pausedText.SetAlignment(AlignCenter, AlignCenter);
    m_pausedText.SetContainer(clientRect);
    m_pausedText.SetText(L"Paused");
    m_pausedText.SetTextColor(D2D1::ColorF(D2D1::ColorF::White));
    m_pausedText.GetTextStyle().SetFontWeight(DWRITE_FONT_WEIGHT_BLACK);
    if (snapped)
    {
        m_pausedText.GetTextStyle().SetFontSize(36.0f);
    }
    else
    {
        m_pausedText.GetTextStyle().SetFontSize(72.0f);
    }
    UserInterface::GetInstance().RegisterElement(&m_pausedText);

    m_resultsText.Initialize();
    m_resultsText.SetAlignment(AlignCenter, AlignCenter);
    m_resultsText.SetContainer(clientRect);
    m_resultsText.SetTextColor(D2D1::ColorF(D2D1::ColorF::White));
    m_resultsText.GetTextStyle().SetFontWeight(DWRITE_FONT_WEIGHT_BOLD);
    m_resultsText.GetTextStyle().SetTextAlignment(DWRITE_TEXT_ALIGNMENT_CENTER);
    if (snapped)
    {
        m_resultsText.GetTextStyle().SetFontSize(36.0f);
    }
    else
    {
        m_resultsText.GetTextStyle().SetFontSize(72.0f);
    }
    UserInterface::GetInstance().RegisterElement(&m_resultsText);

    m_sampleOverlay->UpdateForWindowSizeChange();
    if ((!m_deferredResourcesReady) && m_loadScreen)
    {
        m_loadScreen->UpdateForWindowSizeChange();
    }
}

void MarbleMaze::LoadDeferredResources(bool delay, bool deviceOnly)
{
    BasicTimer^ loadingTimer = ref new BasicTimer;
    loadingTimer->Reset();

    BasicLoader^ loader = ref new BasicLoader(m_d3dDevice.Get());

    D3D11_INPUT_ELEMENT_DESC layoutDesc[] =
        {
            { "POSITION", 0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 0,  D3D11_INPUT_PER_VERTEX_DATA, 0 },
            { "NORMAL",   0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 12, D3D11_INPUT_PER_VERTEX_DATA, 0 },
            { "TEXCOORD", 0, DXGI_FORMAT_R32G32_FLOAT,    0, 24, D3D11_INPUT_PER_VERTEX_DATA, 0 },
            { "TANGENT",  0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 32, D3D11_INPUT_PER_VERTEX_DATA, 0 },
        };
    m_vertexStride = 44; // must set this to match the size of layoutDesc above

    loader->LoadShader(
        L"BasicVertexShader.cso",
        layoutDesc,
        ARRAYSIZE(layoutDesc),
        &m_vertexShader,
        &m_inputLayout
        );

    // create the constant buffer for updating model and camera data.
    D3D11_BUFFER_DESC constantBufferDesc = {0};
    constantBufferDesc.ByteWidth           = ((sizeof(ConstantBuffer) + 15) / 16) * 16; // multiple of 16 bytes
    constantBufferDesc.Usage               = D3D11_USAGE_DEFAULT;
    constantBufferDesc.BindFlags           = D3D11_BIND_CONSTANT_BUFFER;
    constantBufferDesc.CPUAccessFlags      = 0;
    constantBufferDesc.MiscFlags           = 0;
    // this will not be used as a structured buffer, so this parameter is ignored
    constantBufferDesc.StructureByteStride = 0;

    DX::ThrowIfFailed(
        m_d3dDevice->CreateBuffer(
            &constantBufferDesc,
            nullptr,             // leave the buffer uninitialized
            &m_constantBuffer
            )
        );

    loader->LoadShader(
        L"BasicPixelShader.cso",
        &m_pixelShader
        );

    // create the blend state.
    D3D11_BLEND_DESC blendDesc = {0};
    blendDesc.RenderTarget[0].BlendEnable = TRUE;
    blendDesc.RenderTarget[0].SrcBlend = D3D11_BLEND_SRC_ALPHA;
    blendDesc.RenderTarget[0].DestBlend = D3D11_BLEND_INV_SRC_ALPHA;
    blendDesc.RenderTarget[0].BlendOp = D3D11_BLEND_OP_ADD;
    blendDesc.RenderTarget[0].SrcBlendAlpha = D3D11_BLEND_ONE;
    blendDesc.RenderTarget[0].DestBlendAlpha = D3D11_BLEND_ZERO;
    blendDesc.RenderTarget[0].BlendOpAlpha = D3D11_BLEND_OP_ADD;
    blendDesc.RenderTarget[0].RenderTargetWriteMask = D3D11_COLOR_WRITE_ENABLE_ALL;

    DX::ThrowIfFailed(
        m_d3dDevice->CreateBlendState(
            &blendDesc,
            &m_blendState
            )
        );

    // create the sampler
    D3D11_SAMPLER_DESC samplerDesc;
    samplerDesc.Filter = D3D11_FILTER_ANISOTROPIC;
    samplerDesc.AddressU = D3D11_TEXTURE_ADDRESS_WRAP;
    samplerDesc.AddressV = D3D11_TEXTURE_ADDRESS_WRAP;
    samplerDesc.AddressW = D3D11_TEXTURE_ADDRESS_WRAP;
    samplerDesc.MipLODBias = 0.0f;
    samplerDesc.MaxAnisotropy = m_featureLevel > D3D_FEATURE_LEVEL_9_1 ? 4 : 2;
    samplerDesc.ComparisonFunc = D3D11_COMPARISON_NEVER;
    samplerDesc.BorderColor[0] = 0.0f;
    samplerDesc.BorderColor[1] = 0.0f;
    samplerDesc.BorderColor[2] = 0.0f;
    samplerDesc.BorderColor[3] = 0.0f;
    // allow use of all mip levels
    samplerDesc.MinLOD = 0;
    samplerDesc.MaxLOD = D3D11_FLOAT32_MAX;

    DX::ThrowIfFailed(
        m_d3dDevice->CreateSamplerState(
            &samplerDesc,
            &m_sampler
            )
        );

    // Load the meshes.
    DX::ThrowIfFailed(
        m_mazeMesh.Create(
            m_d3dDevice.Get(),
            L"Media\\Models\\maze1.sdkmesh",
            false
            )
        );

    DX::ThrowIfFailed(
        m_marbleMesh.Create(
            m_d3dDevice.Get(),
            L"Media\\Models\\marble2.sdkmesh",
            false
            )
        );

    // Extract mesh geometry for physics system.
    DX::ThrowIfFailed(
        ExtractTrianglesFromMesh(
            m_mazeMesh,
            "Mesh_walls",
            m_collision.m_wallTriList
            )
        );

    DX::ThrowIfFailed(
        ExtractTrianglesFromMesh(
            m_mazeMesh,
            "Mesh_Floor",
            m_collision.m_groundTriList
            )
        );

    DX::ThrowIfFailed(
        ExtractTrianglesFromMesh(
            m_mazeMesh,
            "Mesh_floorSides",
            m_collision.m_floorTriList
            )
        );

    m_physics.SetCollision(&m_collision);
    float radius = m_marbleMesh.GetMeshBoundingBoxExtents(0).x / 2;
    m_physics.SetRadius(radius);

    if (!deviceOnly)
    {
        // When handling device lost, we only need to recreate the graphics-device related
        // resources. All other delayed resources that only need to be created on app
        // startup go here.

        m_audio.CreateResources();
    }

    if (delay)
    {
        while (loadingTimer->Total < 3.5)
        {
            // MarbleMaze doesn't take long to load resources,
            // so we're simulating a longer load time to demonstrate
            // a more real world example
            loadingTimer->Update();
        }
    }

    m_deferredResourcesReady = true;
}

void MarbleMaze::Render()
{
    // Bind the render targets.
    m_d3dContext->OMSetRenderTargets(
        1,
        m_d3dRenderTargetView.GetAddressOf(),
        m_d3dDepthStencilView.Get()
        );

    // Clear the render target and depth stencil to default values.
    const float clearColor[4] = { 0.0f, 0.0f, 0.0f, 1.0f };

    m_d3dContext->ClearRenderTargetView(
        m_d3dRenderTargetView.Get(),
        clearColor
        );

    m_d3dContext->ClearDepthStencilView(
        m_d3dDepthStencilView.Get(),
        D3D11_CLEAR_DEPTH,
        1.0f,
        0
        );

    if (!m_deferredResourcesReady)
    {
        // Only render the loading screen for now.

        m_loadScreen->Render();
        return;
    }

    m_d3dContext->IASetInputLayout(m_inputLayout.Get());

    FLOAT blendFactors[4] = { 0, };
    m_d3dContext->OMSetBlendState(m_blendState.Get(), blendFactors, 0xffffffff);

    // Set the vertex shader stage state.
    m_d3dContext->VSSetShader(
        m_vertexShader.Get(),   // use this vertex shader
        nullptr,                // don't use shader linkage
        0                       // don't use shader linkage
        );

    // Set the pixel shader stage state.
    m_d3dContext->PSSetShader(
        m_pixelShader.Get(),    // use this pixel shader
        nullptr,                // don't use shader linkage
        0                       // don't use shader linkage
        );

    m_d3dContext->PSSetSamplers(
        0,                       // starting at the first sampler slot
        1,                       // set one sampler binding
        m_sampler.GetAddressOf() // to use this sampler
        );

#pragma region Rendering Maze

    // Update the constant buffer with the new data.
    m_d3dContext->UpdateSubresource(
        m_constantBuffer.Get(),
        0,
        nullptr,
        &m_mazeConstantBufferData,
        0,
        0
        );

    m_d3dContext->VSSetConstantBuffers(
        0,                // starting at the first constant buffer slot
        1,                // set one constant buffer binding
        m_constantBuffer.GetAddressOf() // to use this buffer
        );

    m_d3dContext->PSSetConstantBuffers(
        0,                // starting at the first constant buffer slot
        1,                // set one constant buffer binding
        m_constantBuffer.GetAddressOf() // to use this buffer
        );

    m_mazeMesh.Render(m_d3dContext.Get(), 0, INVALID_SAMPLER_SLOT, INVALID_SAMPLER_SLOT);

#pragma endregion

#pragma region Rendering Marble

    // update the constant buffer with the new data
    m_d3dContext->UpdateSubresource(
        m_constantBuffer.Get(),
        0,
        nullptr,
        &m_marbleConstantBufferData,
        0,
        0
        );

    m_d3dContext->VSSetConstantBuffers(
        0,                // starting at the first constant buffer slot
        1,                // set one constant buffer binding
        m_constantBuffer.GetAddressOf() // to use this buffer
        );

    m_d3dContext->PSSetConstantBuffers(
        0,                // starting at the first constant buffer slot
        1,                // set one constant buffer binding
        m_constantBuffer.GetAddressOf() // to use this buffer
        );

    m_marbleMesh.Render(m_d3dContext.Get(), 0, INVALID_SAMPLER_SLOT, INVALID_SAMPLER_SLOT);

#pragma endregion

    // Process audio.
    m_audio.Render();

    // Draw the user interface and the overlay.
    UserInterface::GetInstance().Render();

    m_sampleOverlay->Render();
}

void MarbleMaze::SetGameState(GameState nextState)
{
    // previous state cleanup
    switch (m_gameState)
    {
    case GameState::MainMenu:
        m_startGameButton.SetVisible(false);
        m_highScoreButton.SetVisible(false);
        break;

    case GameState::HighScoreDisplay:
        m_highScoreTable.SetVisible(false);
        break;

    case GameState::PreGameCountdown:
        m_inGameStopwatchTimer.Reset();
        m_inGameStopwatchTimer.SetVisible(true);
        break;

    case GameState::PostGameResults:
        m_resultsText.SetVisible(false);
        break;
    }

    // next state setup
    switch (nextState)
    {
    case GameState::MainMenu:
        m_startGameButton.SetVisible(true);
        m_startGameButton.SetSelected(true);
        m_highScoreButton.SetVisible(true);
        m_highScoreButton.SetSelected(false);
        m_pausedText.SetVisible(false);

        m_resetCamera = true;
        m_resetMarbleRotation = true;
        m_physics.SetPosition(XMFLOAT3(305, -210, -43));
        m_targetLightStrength = 0.6f;
        break;

    case GameState::HighScoreDisplay:
        m_highScoreTable.SetVisible(true);
        break;

    case GameState::PreGameCountdown:
        m_inGameStopwatchTimer.SetVisible(false);
        m_preGameCountdownTimer.SetVisible(true);
        m_preGameCountdownTimer.StartCountdown(3);

        ResetCheckpoints();
        m_resetCamera = true;
        m_resetMarbleRotation = true;
        m_physics.SetPosition(m_checkpoints[0]);
        m_physics.SetVelocity(XMFLOAT3(0, 0, 0));
        m_targetLightStrength = 1.0f;
        break;

    case GameState::InGameActive:
        m_pausedText.SetVisible(false);
        m_inGameStopwatchTimer.Start();
        m_targetLightStrength = 1.0f;
        break;

    case GameState::InGamePaused:
        m_pausedText.SetVisible(true);
        m_inGameStopwatchTimer.Stop();
        m_targetLightStrength = 0.6f;
        break;

    case GameState::PostGameResults:
        m_inGameStopwatchTimer.Stop();
        m_inGameStopwatchTimer.SetVisible(false);
        m_resultsText.SetVisible(true);
        {
            WCHAR formattedTime[32];
            m_inGameStopwatchTimer.GetFormattedTime(formattedTime, m_newHighScore.elapsedTime);
            WCHAR buffer[64];
            swprintf_s(
                buffer,
                L"%s\nYour time: %s",
                (m_newHighScore.wasJustAdded ? L"New High Score!" : L"Finished!"),
                formattedTime
                );
            m_resultsText.SetText(buffer);
            m_resultsText.SetVisible(true);
        }
        m_targetLightStrength = 0.6f;
        break;
    }

    m_gameState = nextState;
}

void MarbleMaze::ResetCheckpoints()
{
    m_currentCheckpoint = 0;
}

CheckpointState MarbleMaze::UpdateCheckpoints()
{
    if (m_currentCheckpoint >= (m_checkpoints.size() - 1))
        return CheckpointState::None;

    const float checkpointRadius = 20.0f;
    float radius = m_physics.GetRadius();
    float horizDistSq = (radius + checkpointRadius) * (radius + checkpointRadius);
    XMVECTOR horizDistSqLimit = XMVectorSet(horizDistSq, horizDistSq, horizDistSq, horizDistSq);
    float vertDistSq = radius * radius;
    XMVECTOR vertDistSqLimit = XMVectorSet(vertDistSq, vertDistSq, vertDistSq, vertDistSq);

    XMVECTOR position = XMLoadFloat3(&m_physics.GetPosition());
    XMVECTOR up = XMVectorSet(0.0f, 0.0f, -1.0f, 0.0f);

    for (size_t i = m_currentCheckpoint + 1; i < m_checkpoints.size(); ++i)
    {
        XMVECTOR checkpointPos = XMLoadFloat3(&m_checkpoints[i]);
        XMVECTOR posCheckSpace = position - checkpointPos;
        XMVECTOR posVertical = up * XMVector3Dot(up, posCheckSpace);
        XMVECTOR posHorizontal = posCheckSpace - posVertical;
        XMVECTOR vHorizDistSq = XMVector3LengthSq(posHorizontal);
        XMVECTOR vVertDistSq = XMVector3LengthSq(posVertical);

        XMVECTOR check = XMVectorAndInt(
            XMVectorLessOrEqual(vHorizDistSq, horizDistSqLimit),
            XMVectorLessOrEqual(vVertDistSq, vertDistSqLimit)
            );
        if (XMVector3EqualInt(check, XMVectorTrueInt()))
        {
            m_currentCheckpoint = i;

            if (i == (m_checkpoints.size() - 1))
                return CheckpointState::Goal;
            else
                return CheckpointState::Save;
        }
    }

    return CheckpointState::None;
}

void MarbleMaze::Update(float timeTotal, float timeDelta)
{
    // When the game is first loaded, we display a load screen
    // and load any deferred resources that might be too expensive
    // to load during initialization.
    if (!m_deferredResourcesReady)
    {
        // At this point we can draw a progress bar, or if we had
        // loaded audio, we could play audio during the loading process.

        return;
    }

    if (!m_audio.m_isAudioStarted)
    {
        m_audio.Start();
    }

    UserInterface::GetInstance().Update(timeTotal, timeDelta);

    if (m_gameState == GameState::Initial)
        SetGameState(GameState::MainMenu);

    switch (m_gameState)
    {
        case GameState::PreGameCountdown:
            if (m_preGameCountdownTimer.IsCountdownComplete())
                SetGameState(GameState::InGameActive);
            break;
    }

#pragma region Process Input

    float combinedTiltX = 0.0f;
    float combinedTiltY = 0.0f;

    // This array contains the constants from XINPUT that map to the
    // particular buttons that are used by the game.
    // The index of the array is used to associate the state of that button in
    // the wasButtonDown, isButtonDown, and combinedButtonPressed variables.

    static const WORD buttons[] =
    {
        XINPUT_GAMEPAD_A,
        XINPUT_GAMEPAD_START,
        XINPUT_GAMEPAD_DPAD_UP,
        XINPUT_GAMEPAD_DPAD_DOWN,
        XINPUT_GAMEPAD_DPAD_LEFT,
        XINPUT_GAMEPAD_DPAD_RIGHT,
        XINPUT_GAMEPAD_BACK,
    };

    static const int buttonCount = ARRAYSIZE(buttons);
    static bool wasButtonDown[XUSER_MAX_COUNT][buttonCount] = { false, };
    bool isButtonDown[XUSER_MAX_COUNT][buttonCount] = { false, };

    // Account for input on any connected controller.
    XINPUT_STATE inputState = {0};
    for (DWORD userIndex = 0; userIndex < XUSER_MAX_COUNT; ++userIndex)
    {
        DWORD result = XInputGetState(userIndex, &inputState);
        if (result != ERROR_SUCCESS)
            continue;

        SHORT thumbLeftX = inputState.Gamepad.sThumbLX;
        if (abs(thumbLeftX) < XINPUT_GAMEPAD_LEFT_THUMB_DEADZONE)
            thumbLeftX = 0;

        SHORT thumbLeftY = inputState.Gamepad.sThumbLY;
        if (abs(thumbLeftY) < XINPUT_GAMEPAD_LEFT_THUMB_DEADZONE)
            thumbLeftY = 0;

        combinedTiltX += static_cast<float>(thumbLeftX) / 32768.0f;
        combinedTiltY += static_cast<float>(thumbLeftY) / 32768.0f;

        for (int i = 0; i < buttonCount; ++i)
            isButtonDown[userIndex][i] = (inputState.Gamepad.wButtons & buttons[i]) == buttons[i];
    }

    bool combinedButtonPressed[buttonCount] = { false, };
    for (int i = 0; i < buttonCount; ++i)
    {
        for (DWORD userIndex = 0; userIndex < XUSER_MAX_COUNT; ++userIndex)
        {
            bool pressed = !wasButtonDown[userIndex][i] && isButtonDown[userIndex][i];
            combinedButtonPressed[i] = combinedButtonPressed[i] || pressed;
        }
    }

    // Check whether the user paused or resumed the game.
    // XINPUT_GAMEPAD_START
    if (combinedButtonPressed[1] || m_pauseKeyPressed)
    {
        m_pauseKeyPressed = false;
        if (m_gameState == GameState::InGameActive)
            SetGameState(GameState::InGamePaused);
        else if (m_gameState == GameState::InGamePaused)
            SetGameState(GameState::InGameActive);
    }

    // Check whether the user restarted the game or cleared the high score table.
    // XINPUT_GAMEPAD_BACK
    if (combinedButtonPressed[6] || m_homeKeyPressed)
    {
        m_homeKeyPressed = false;
        if (m_gameState == GameState::InGameActive ||
            m_gameState == GameState::InGamePaused ||
            m_gameState == GameState::PreGameCountdown)
        {
            SetGameState(GameState::MainMenu);
            m_inGameStopwatchTimer.SetVisible(false);
            m_preGameCountdownTimer.SetVisible(false);
        }
        else if (m_gameState == GameState::HighScoreDisplay)
        {
            m_highScoreTable.Reset();
        }
    }

    // Check whether the user chose a button from the UI.
    bool anyPoints = !m_pointQueue.empty();
    while (!m_pointQueue.empty())
    {
        UserInterface::GetInstance().HitTest(m_pointQueue.front());
        m_pointQueue.pop();
    }

    // Handle menu navigation.

    // XINPUT_GAMEPAD_A or XINPUT_GAMEPAD_START
    bool chooseSelection = (combinedButtonPressed[0] || combinedButtonPressed[1]);

    // XINPUT_GAMEPAD_DPAD_UP
    bool moveUp = combinedButtonPressed[2];

    // XINPUT_GAMEPAD_DPAD_DOWN
    bool moveDown = combinedButtonPressed[3];

    switch (m_gameState)
    {
    case GameState::MainMenu:
        if (chooseSelection)
        {
            m_audio.PlaySoundEffect(MenuSelectedEvent);

            if (m_startGameButton.GetSelected())
                m_startGameButton.SetPressed(true);

            if (m_highScoreButton.GetSelected())
                m_highScoreButton.SetPressed(true);
        }
        if (moveUp || moveDown)
        {
            m_startGameButton.SetSelected(!m_startGameButton.GetSelected());
            m_highScoreButton.SetSelected(!m_startGameButton.GetSelected());

            m_audio.PlaySoundEffect(MenuChangeEvent);
        }
        break;

    case GameState::HighScoreDisplay:
        if (chooseSelection || anyPoints)
            SetGameState(GameState::MainMenu);
        break;

    case GameState::PostGameResults:
        if (chooseSelection || anyPoints)
            SetGameState(GameState::HighScoreDisplay);
        break;

    case GameState::InGamePaused:
        if (m_pausedText.IsPressed())
        {
            m_pausedText.SetPressed(false);
            SetGameState(GameState::InGameActive);
        }
        break;
    }

    // Update button state for next frame.
    memcpy(wasButtonDown, isButtonDown, sizeof(wasButtonDown));

    // Update the game state if the user chose a menu option.
    if (m_startGameButton.IsPressed())
    {
        SetGameState(GameState::PreGameCountdown);
        m_startGameButton.SetPressed(false);
    }
    if (m_highScoreButton.IsPressed())
    {
        SetGameState(GameState::HighScoreDisplay);
        m_highScoreButton.SetPressed(false);
    }

    // Account for touch input.
    const float touchScalingFactor = 2.0f;
    for (TouchMap::const_iterator iter = m_touches.cbegin(); iter != m_touches.cend(); ++iter)
    {
        combinedTiltX += iter->second.x * touchScalingFactor;
        combinedTiltY += iter->second.y * touchScalingFactor;
    }

    // Account for sensors.
    const float acceleromterScalingFactor = 3.5f;
    if (m_accelerometer != nullptr)
    {
        Windows::Devices::Sensors::AccelerometerReading^ reading =
            m_accelerometer->GetCurrentReading();

        if (reading != nullptr)
        {
            combinedTiltX += static_cast<float>(reading->AccelerationX) * acceleromterScalingFactor;
            combinedTiltY += static_cast<float>(reading->AccelerationY) * acceleromterScalingFactor;
        }
    }

    // Clamp input.
    combinedTiltX = max(-1, min(1, combinedTiltX));
    combinedTiltY = max(-1, min(1, combinedTiltY));

    if (m_gameState != GameState::PreGameCountdown &&
        m_gameState != GameState::InGameActive &&
        m_gameState != GameState::InGamePaused)
    {
        // Ignore tilt when the menu is active.
        combinedTiltX = 0.0f;
        combinedTiltY = 0.0f;
    }

#pragma endregion

#pragma region Physics

    const float maxTilt = 1.0f / 8.0f;
    XMVECTOR gravity = XMVectorSet(combinedTiltX * maxTilt, combinedTiltY * maxTilt, 1.0f, 0.0f);
    gravity = XMVector3Normalize(gravity);

    XMFLOAT3 g;
    XMStoreFloat3(&g, gravity);
    m_physics.SetGravity(g);

    if (m_gameState == GameState::InGameActive)
    {
        // Only update physics when gameplay is active.
        m_physics.UpdatePhysicsSimulation(timeDelta);

        // Handle checkpoints.
        switch (UpdateCheckpoints())
        {
        case CheckpointState::Save:
            // Display checkpoint notice.
            m_checkpointText.SetVisible(true);
            m_checkpointText.SetTextOpacity(1.0f);
            m_checkpointText.FadeOut(2.0f);
            m_audio.PlaySoundEffect(CheckpointEvent);
            SaveState();
            break;

        case CheckpointState::Goal:
            m_inGameStopwatchTimer.Stop();
            m_newHighScore.elapsedTime = m_inGameStopwatchTimer.GetElapsedTime();
            SYSTEMTIME systemTime;
            GetLocalTime(&systemTime);
            WCHAR buffer[64];
            swprintf_s(buffer, L"%d/%d/%d", systemTime.wYear, systemTime.wMonth, systemTime.wDay);
            m_newHighScore.tag = ref new Platform::String(buffer);
            m_highScoreTable.AddScoreToTable(m_newHighScore);

            m_audio.PlaySoundEffect(CheckpointEvent);
            m_audio.StopSoundEffect(RollingEvent);

            // Display game results.
            SetGameState(GameState::PostGameResults);
            SaveState();
            break;
        }
    }

    float3 marblePosition;
    memcpy(&marblePosition, &m_physics.GetPosition(), sizeof(float3));
    static float3 oldMarblePosition = marblePosition;

    const float4x4 initialMarbleRotationMatrix = mul(rotationY(90.0f), rotationX(90.0f));
    static float4x4 marbleRotationMatrix = initialMarbleRotationMatrix;

    // Check whether the marble fell off of the maze.
    const float fadeOutDepth = 0.0f;
    const float resetDepth = 80.0f;
    if (marblePosition.z >= fadeOutDepth)
    {
        m_targetLightStrength = 0.0f;
    }
    if (marblePosition.z >= resetDepth)
    {
        // Reset marble.
        memcpy(&marblePosition, &m_checkpoints[m_currentCheckpoint], sizeof(XMFLOAT3));
        oldMarblePosition = marblePosition;
        m_physics.SetPosition((const XMFLOAT3&)marblePosition);
        m_physics.SetVelocity(XMFLOAT3(0, 0, 0));
        m_lightStrength = 0.0f;
        m_targetLightStrength = 1.0f;

        m_resetCamera = true;
        m_resetMarbleRotation = true;
        m_audio.PlaySoundEffect(FallingEvent);
    }

    float3 marbleRotation = (oldMarblePosition - marblePosition) / m_physics.GetRadius();
    oldMarblePosition = marblePosition;

    if (m_resetMarbleRotation)
    {
        marbleRotationMatrix = initialMarbleRotationMatrix;
        m_resetMarbleRotation = false;
    }
    else
    {
        marbleRotationMatrix = mul(rotationY(marbleRotation.x * 180.0f / 3.1415926535f), marbleRotationMatrix);
        marbleRotationMatrix = mul(rotationX(marbleRotation.y * -180.0f / 3.1415926535f), marbleRotationMatrix);
    }

#pragma endregion

#pragma region Update Camera

    static float eyeDistance = 200.0f;
    static float3 eyePosition = float3(0, 0, 0);

    // Gradually move the camera above the marble.
    float3 targetEyePosition = marblePosition - (eyeDistance * float3(g.x, g.y, g.z));
    if (m_resetCamera)
    {
        eyePosition = targetEyePosition;
        m_resetCamera = false;
    }
    else
    {
        eyePosition = eyePosition + ((targetEyePosition - eyePosition) * min(1, timeDelta * 8));
    }

    // Look at the marble.
    if ((m_gameState == GameState::MainMenu) || (m_gameState == GameState::HighScoreDisplay))
    {
        // Override camera position for menus.
        eyePosition = marblePosition + float3(75.0f, -150.0f, -75.0f);
        m_camera->SetViewParameters(eyePosition, marblePosition, float3(0.0f, 0.0f, -1.0f));
    }
    else
    {
        m_camera->SetViewParameters(eyePosition, marblePosition, float3(0.0f, 1.0f, 0.0f));
    }

#pragma endregion

#pragma region Update Constant Buffers

    // Update the model matrices based on the simulation.
    m_mazeConstantBufferData.model = identity();
    m_marbleConstantBufferData.model = mul(
        translation(marblePosition.x, marblePosition.y, marblePosition.z),
        marbleRotationMatrix
        );

    // Update the view matrix based on the camera.
    float4x4 view;
    m_camera->GetViewMatrix(&view);
    m_mazeConstantBufferData.view = view;
    m_marbleConstantBufferData.view = view;

    // Update lighting constants.
    m_lightStrength += (m_targetLightStrength - m_lightStrength) * min(1, timeDelta * 4);

    m_mazeConstantBufferData.marblePosition = marblePosition;
    m_mazeConstantBufferData.marbleRadius = m_physics.GetRadius();
    m_mazeConstantBufferData.lightStrength = m_lightStrength;
    m_marbleConstantBufferData.marblePosition = marblePosition;
    m_marbleConstantBufferData.marbleRadius = m_physics.GetRadius();
    m_marbleConstantBufferData.lightStrength = m_lightStrength;

#pragma endregion

#pragma region Update Audio

    if (! m_audio.HasEngineExperiencedCriticalError())
    {
        if (m_gameState == GameState::InGameActive)
        {
            float wallDistances[8];
            int returnedCount = m_physics.GetRoomDimensions(wallDistances, ARRAYSIZE(wallDistances));
            assert(returnedCount == ARRAYSIZE(wallDistances));
            m_audio.SetRoomSize(m_physics.GetRoomSize(), wallDistances);
            CollisionInfo ci = m_physics.GetCollisionInfo();

            // Calculate roll sound, and pitch according to velocity.
            XMFLOAT3 velocity = m_physics.GetVelocity();
            XMFLOAT3 position = m_physics.GetPosition();
            float volumeX = abs(velocity.x) / 200;
            if (volumeX > 1.0) volumeX = 1.0;
            if (volumeX < 0.0) volumeX = 0.0;
            float volumeY = abs(velocity.y) / 200;
            if (volumeY > 1.0) volumeY = 1.0;
            if (volumeY < 0.0) volumeY = 0.0;
            float volume = max(volumeX, volumeY);

            // Pitch of the rolling sound ranges from .85 to 1.05f,
            // increasing logarithmically.
            float pitch = .85f + (volume * volume / 5.0f);

            // Play the roll sound only if the marble is actually rolling.
            if (ci.isRollingOnFloor && volume > 0)
            {
                if (!m_audio.IsSoundEffectStarted(RollingEvent))
                {
                    m_audio.PlaySoundEffect(RollingEvent);
                }

                // Update the volume and pitch by the velocity.
                m_audio.SetSoundEffectVolume(RollingEvent, volume);
                m_audio.SetSoundEffectPitch(RollingEvent, pitch);

                // The rolling sound has at most 8000Hz sounds, so we linearly
                // ramp up the low-pass filter the faster we go.
                // We also reduce the Q-value of the filter, starting with a
                // relatively broad cutoff and get progressively tighter.
                m_audio.SetSoundEffectFilter(
                    RollingEvent,
                    600.0f + 8000.0f * volume,
                    XAUDIO2_MAX_FILTER_ONEOVERQ - volume*volume
                    );
            }
            else
            {
                m_audio.SetSoundEffectVolume(RollingEvent, 0);
            }

            if (ci.elasticCollision && ci.maxCollisionSpeed > 10)
            {
                m_audio.PlaySoundEffect(CollisionEvent);

                float collisionVolume = ci.maxCollisionSpeed/150.0f;
                collisionVolume = min(collisionVolume * collisionVolume, 1.0f);
                m_audio.SetSoundEffectVolume(CollisionEvent, collisionVolume);
            }
        }
        else
        {
            m_audio.SetSoundEffectVolume(RollingEvent, 0);
        }
    }
#pragma endregion

}

void MarbleMaze::SaveState()
{
    m_persistentState->SaveXMFLOAT3(":Position", m_physics.GetPosition());
    m_persistentState->SaveXMFLOAT3(":Velocity", m_physics.GetVelocity());
    m_persistentState->SaveSingle(":ElapsedTime", m_inGameStopwatchTimer.GetElapsedTime());

    m_persistentState->SaveInt32(":GameState", static_cast<int>(m_gameState));
    m_persistentState->SaveInt32(":Checkpoint", static_cast<int>(m_currentCheckpoint));

    int i = 0;
    HighScoreEntries entries = m_highScoreTable.GetEntries();
    const int bufferLength = 16;
    char16 str[bufferLength];

    m_persistentState->SaveInt32(":ScoreCount", static_cast<int>(entries.size()));
    for (auto iter = entries.begin(); iter != entries.end(); ++iter)
    {
        int len = swprintf_s(str, bufferLength, L"%d", i++);
        Platform::String^ string = ref new Platform::String(str, len);

        m_persistentState->SaveSingle(Platform::String::Concat(":ScoreTime", string), iter->elapsedTime);
        m_persistentState->SaveString(Platform::String::Concat(":ScoreTag", string), iter->tag);
    }
}

void MarbleMaze::LoadState()
{
    XMFLOAT3 position = m_persistentState->LoadXMFLOAT3(":Position", m_physics.GetPosition());
    XMFLOAT3 velocity = m_persistentState->LoadXMFLOAT3(":Velocity", m_physics.GetVelocity());
    float elapsedTime = m_persistentState->LoadSingle(":ElapsedTime", 0.0f);

    int gameState = m_persistentState->LoadInt32(":GameState", static_cast<int>(m_gameState));
    int currentCheckpoint = m_persistentState->LoadInt32(":Checkpoint", static_cast<int>(m_currentCheckpoint));

    switch (static_cast<GameState>(gameState))
    {
    case GameState::Initial:
        break;

    case GameState::MainMenu:
    case GameState::HighScoreDisplay:
    case GameState::PreGameCountdown:
    case GameState::PostGameResults:
        SetGameState(GameState::MainMenu);
        break;

    case GameState::InGameActive:
    case GameState::InGamePaused:
        m_inGameStopwatchTimer.SetVisible(true);
        m_inGameStopwatchTimer.SetElapsedTime(elapsedTime);
        m_physics.SetPosition(position);
        m_physics.SetVelocity(velocity);
        m_currentCheckpoint = currentCheckpoint;
        SetGameState(GameState::InGamePaused);
        break;
    }

    int count = m_persistentState->LoadInt32(":ScoreCount", 0);

    const int bufferLength = 16;
    char16 str[bufferLength];

    for (int i = 0; i < count; i++)
    {
        HighScoreEntry entry;
        int len = swprintf_s(str, bufferLength, L"%d", i);
        Platform::String^ string = ref new Platform::String(str, len);

        entry.elapsedTime = m_persistentState->LoadSingle(Platform::String::Concat(":ScoreTime", string), 0.0f);
        entry.tag = m_persistentState->LoadString(Platform::String::Concat(":ScoreTag", string), L"");
        m_highScoreTable.AddScoreToTable(entry);
    }
}

inline XMFLOAT2 PointToTouch(Windows::Foundation::Point point, Windows::Foundation::Rect bounds)
{
    float touchRadius = min(bounds.Width, bounds.Height);
    float dx = (point.X - (bounds.Width / 2.0f)) / touchRadius;
    float dy = ((bounds.Height / 2.0f) - point.Y) / touchRadius;

    return XMFLOAT2(dx, dy);
}

void MarbleMaze::AddTouch(int id, Windows::Foundation::Point point)
{
    m_touches[id] = PointToTouch(point, m_windowBounds);

    m_pointQueue.push(D2D1::Point2F(point.X, point.Y));
}

void MarbleMaze::UpdateTouch(int id, Windows::Foundation::Point point)
{
    if (m_touches.find(id) != m_touches.end())
        m_touches[id] = PointToTouch(point, m_windowBounds);
}

void MarbleMaze::RemoveTouch(int id)
{
    m_touches.erase(id);
}

void MarbleMaze::KeyDown(VirtualKey key)
{
    if (key == VirtualKey::P)       // Pause
    {
        m_pauseKeyActive = true;
    }
    if (key == VirtualKey::Home)
    {
        m_homeKeyActive = true;
    }
}

void MarbleMaze::KeyUp(VirtualKey key)
{
    if (key == VirtualKey::P)
    {
        if (m_pauseKeyActive)
        {
            // trigger pause only once on button release
            m_pauseKeyPressed = true;
            m_pauseKeyActive = false;
        }
    }
    if (key == VirtualKey::Home)
    {
        if (m_homeKeyActive)
        {
            m_homeKeyPressed = true;
            m_homeKeyActive = false;
        }
    }
}

void MarbleMaze::OnSuspending()
{
    SaveState();
    m_audio.SuspendAudio();
}

void MarbleMaze::OnResuming()
{
    m_audio.ResumeAudio();
}

void MarbleMaze::OnFocusChange(bool active)
{
    static bool lostFocusPause = false;

    if (m_deferredResourcesReady)
    {
        if (m_windowActive != active)
        {
            if (active)
            {
                m_audio.ResumeAudio();
                if ((m_gameState == GameState::InGamePaused) && lostFocusPause)
                {
                    SetGameState(GameState::InGameActive);
                }
            }
            else
            {
                m_audio.SuspendAudio();
                if (m_gameState == GameState::InGameActive)
                {
                    SetGameState(GameState::InGamePaused);
                    lostFocusPause = true;
                    SaveState();
                }
                else if (m_gameState == GameState::PreGameCountdown)
                {
                    SetGameState(GameState::MainMenu);
                    m_inGameStopwatchTimer.SetVisible(false);
                    m_preGameCountdownTimer.SetVisible(false);
                }
            }
            m_windowActive = active;
        }
    }
}

void MarbleMaze::OnViewChange(ApplicationViewState state)
{
    m_viewState = state;
}


FORCEINLINE int FindMeshIndexByName(SDKMesh &mesh, const char *meshName)
{
    UINT meshCount = mesh.GetNumMeshes();
    for (UINT i = 0; i < meshCount; ++i)
    {
        if (0 == _stricmp(mesh.GetMesh(i)->Name, meshName))
            return i;
    }

    return -1; // Not found.
}

HRESULT MarbleMaze::ExtractTrianglesFromMesh(
    SDKMesh &mesh,
    const char *meshName,
    std::vector<Triangle> &triangles
    )
{
    triangles.clear();

    int meshIndex = FindMeshIndexByName(mesh, meshName);
    if (meshIndex < 0)
    {
        return E_FAIL;
    }
    SDKMESH_MESH *currentmesh = mesh.GetMesh(meshIndex);

    for (UINT i = 0; i < currentmesh->NumSubsets; ++i)
    {
        SDKMESH_SUBSET *subsetmesh = mesh.GetSubset(meshIndex, i);

        USHORT *indices = (USHORT*)mesh.GetRawIndicesAt(currentmesh->IndexBuffer) + subsetmesh->IndexStart;
        BYTE *vertices = mesh.GetRawVerticesAt(currentmesh->VertexBuffers[0]) + (subsetmesh->VertexStart * m_vertexStride);
        for (UINT j = 0; j < subsetmesh->IndexCount; j += 3)
        {
            XMFLOAT3 a, b, c;
            memcpy(&a, vertices + (*(indices++) * m_vertexStride), sizeof(XMFLOAT3));
            memcpy(&b, vertices + (*(indices++) * m_vertexStride), sizeof(XMFLOAT3));
            memcpy(&c, vertices + (*(indices++) * m_vertexStride), sizeof(XMFLOAT3));
            triangles.push_back(Triangle(a, b, c));
        }
    }

    return S_OK;
}

