//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DirectXPage.xaml.h"
#include <windows.ui.xaml.media.dxinterop.h>

using namespace ShaderCompiler;

using namespace DirectX;
using namespace Microsoft::WRL;
using namespace Platform;
using namespace Windows::ApplicationModel;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::Storage;
using namespace Windows::UI;
using namespace Windows::UI::Core;
using namespace Windows::UI::Input;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

using namespace concurrency;
using namespace std;

DirectXPage::DirectXPage():
    m_windowVisible(true),
    m_compiledOnce(false)
{
    InitializeComponent();

    m_shaderModelSuffix = "";

    // Register event handlers for page lifecycle.
    CoreWindow^ window = Window::Current->CoreWindow;

    window->VisibilityChanged +=
        ref new TypedEventHandler<CoreWindow^, VisibilityChangedEventArgs^>(this, &DirectXPage::OnVisibilityChanged);

    PreviewPanel->CompositionScaleChanged +=
        ref new Windows::Foundation::TypedEventHandler<SwapChainPanel^, Object^>(this, &DirectXPage::OnCompositionScaleChanged);

    // Register the rendering event, called every time XAML renders the screen.
    m_eventToken = CompositionTarget::Rendering::add(ref new EventHandler<Object^>(this, &DirectXPage::OnRendering));

    // Whenever this screen is not being used anymore, you can unregister this event with the following line:
    // CompositionTarget::Rendering::remove(m_eventToken);

    // Disable all pointer visual feedback for better performance when touching.
    auto pointerVisualizationSettings = PointerVisualizationSettings::GetForCurrentView();
    pointerVisualizationSettings->IsContactFeedbackEnabled = false;
    pointerVisualizationSettings->IsBarrelButtonFeedbackEnabled = false;

    // At this point we have access to the device.
    // We can create the device-dependent resources.
    m_deviceResources = std::make_shared<DX::DeviceResources>();
    m_pageNotifier.reset(new PageNotifier(this));
    m_deviceResources->RegisterDeviceNotify(m_pageNotifier.get());

    // Enable multi-line text.
    HeaderCodeTextBox->AcceptsReturn = true;
    SourceCodeTextBox->AcceptsReturn = true;
    OutputTextBox->AcceptsReturn = true;

    // Initialize the contents with the default header and source.
    create_task(Package::Current->InstalledLocation->GetFileAsync("DefaultShaderLibrary.hlsl")).then([this](StorageFile^ file)
    {
        return FileIO::ReadLinesAsync(file);
    }).then([this](IVector<String^>^ codeLines)
    {
        String^ sourceCode;
        CodeRegion region = CodeRegion::None;
        for (auto line : codeLines)
        {
            wstring testLine(line->Begin(), line->End());
            if (testLine.find(L"@@@ Begin Header") != testLine.npos)
            {
                region = CodeRegion::Header;
                continue;
            }
            if (testLine.find(L"@@@ Begin Source") != testLine.npos)
            {
                region = CodeRegion::Source;
                continue;
            }
            if (testLine.find(L"@@@ Begin Hidden") != testLine.npos)
            {
                region = CodeRegion::Hidden;
                continue;
            }
            if (testLine.find(L"@@@ End") != testLine.npos)
            {
                region = CodeRegion::None;
                continue;
            }
            switch (region)
            {
            case CodeRegion::Header:
                m_headerCode += line + "\n";
                break;
            case CodeRegion::Source:
                sourceCode += line + "\n";
                break;
            case CodeRegion::Hidden:
                m_hiddenCode += line + "\n";
                break;
            default:
                // Ignore text outside of the defined code regions.
                break;
            }
        }
        HeaderCodeTextBox->Text = m_headerCode;
        SourceCodeTextBox->Text = sourceCode;
    });

    CreateDeviceDependentResources();
}

void DirectXPage::CreateDeviceDependentResources()
{
    // Create the preview window resources.
    auto device = m_deviceResources->GetD3DDevice();
    auto context = m_deviceResources->GetD3DDeviceContext();

    // Create and bind a wrapping bilinear sampler.
    D3D11_SAMPLER_DESC samplerDesc;
    ZeroMemory(&samplerDesc, sizeof(samplerDesc));
    samplerDesc.Filter = D3D11_FILTER_ANISOTROPIC;
    samplerDesc.AddressU = D3D11_TEXTURE_ADDRESS_WRAP;
    samplerDesc.AddressV = D3D11_TEXTURE_ADDRESS_WRAP;
    samplerDesc.AddressW = D3D11_TEXTURE_ADDRESS_WRAP;
    samplerDesc.ComparisonFunc = D3D11_COMPARISON_NEVER;
    samplerDesc.MaxAnisotropy = 2;
    samplerDesc.MaxLOD = D3D11_FLOAT32_MAX;
    ComPtr<ID3D11SamplerState> sampler;
    DX::ThrowIfFailed(device->CreateSamplerState(&samplerDesc, &sampler));
    context->PSSetSamplers(0, 1, sampler.GetAddressOf());

    // Create and bind vertex and index buffers for a simple cube.
    float cubeVertices[] =
    {
        -0.5f, 0.5f, -0.5f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, // +Y (top face)
         0.5f, 0.5f, -0.5f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
         0.5f, 0.5f, 0.5f, 1.0f, 1.0f, 0.0f, 1.0f, 0.0f,
        -0.5f, 0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f,

        -0.5f, -0.5f, 0.5f, 0.0f, 0.0f, 0.0f, -1.0f, 0.0f, // -Y (bottom face)
         0.5f, -0.5f, 0.5f, 1.0f, 0.0f, 0.0f, -1.0f, 0.0f,
         0.5f, -0.5f, -0.5f, 1.0f, 1.0f, 0.0f, -1.0f, 0.0f,
        -0.5f, -0.5f, -0.5f, 0.0f, 1.0f, 0.0f, -1.0f, 0.0f,

        0.5f, 0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, // +X (right face)
        0.5f, 0.5f, -0.5f, 1.0f, 0.0f, 1.0f, 0.0f, 0.0f,
        0.5f, -0.5f, -0.5f, 1.0f, 1.0f, 1.0f, 0.0f, 0.0f,
        0.5f, -0.5f, 0.5f, 0.0f, 1.0f, 1.0f, 0.0f, 0.0f,

        -0.5f, 0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f, // -X (left face)
        -0.5f, 0.5f, 0.5f, 1.0f, 0.0f, -1.0f, 0.0f, 0.0f,
        -0.5f, -0.5f, 0.5f, 1.0f, 1.0f, -1.0f, 0.0f, 0.0f,
        -0.5f, -0.5f, -0.5f, 0.0f, 1.0f, -1.0f, 0.0f, 0.0f,

        -0.5f, 0.5f, 0.5f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, // +Z (front face)
         0.5f, 0.5f, 0.5f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f,
         0.5f, -0.5f, 0.5f, 1.0f, 1.0f, 0.0f, 0.0f, 1.0f,
        -0.5f, -0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f,

         0.5f, 0.5f, -0.5f, 0.0f, 0.0f, 0.0f, 0.0f, -1.0f, // -Z (back face)
        -0.5f, 0.5f, -0.5f, 1.0f, 0.0f, 0.0f, 0.0f, -1.0f,
        -0.5f, -0.5f, -0.5f, 1.0f, 1.0f, 0.0f, 0.0f, -1.0f,
         0.5f, -0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f, -1.0f,
    };
    D3D11_BUFFER_DESC vertexBufferDesc = {0};
    vertexBufferDesc.ByteWidth = sizeof(cubeVertices);
    vertexBufferDesc.Usage = D3D11_USAGE_DEFAULT;
    vertexBufferDesc.BindFlags = D3D11_BIND_VERTEX_BUFFER;
    D3D11_SUBRESOURCE_DATA vertexBufferData = {0};
    vertexBufferData.pSysMem = cubeVertices;
    ComPtr<ID3D11Buffer> vertexBuffer;
    DX::ThrowIfFailed(device->CreateBuffer(&vertexBufferDesc, &vertexBufferData, &vertexBuffer));
    UINT stride = sizeof(float) * 8;
    UINT offset = 0;
    context->IASetVertexBuffers(0, 1, vertexBuffer.GetAddressOf(), &stride, &offset);

    unsigned short cubeIndices[] =
    {
        0, 1, 2,
        0, 2, 3,

        4, 5, 6,
        4, 6, 7,

        8, 9, 10,
        8, 10, 11,

        12, 13, 14,
        12, 14, 15,

        16, 17, 18,
        16, 18, 19,

        20, 21, 22,
        20, 22, 23
    };
    D3D11_BUFFER_DESC indexBufferDesc = {0};
    indexBufferDesc.ByteWidth = sizeof(cubeIndices);
    indexBufferDesc.Usage = D3D11_USAGE_DEFAULT;
    indexBufferDesc.BindFlags = D3D11_BIND_INDEX_BUFFER;
    D3D11_SUBRESOURCE_DATA indexBufferData = {0};
    indexBufferData.pSysMem = cubeIndices;
    ComPtr<ID3D11Buffer> indexBuffer;
    DX::ThrowIfFailed(m_deviceResources->GetD3DDevice()->CreateBuffer(&indexBufferDesc, &indexBufferData, &indexBuffer));
    context->IASetIndexBuffer(indexBuffer.Get(), DXGI_FORMAT_R16_UINT, 0);
    context->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);

    // Create constant buffers.
    D3D11_BUFFER_DESC cameraDataConstantBuffer = {0};
    cameraDataConstantBuffer.ByteWidth = sizeof(XMFLOAT4X4) * 3;
    cameraDataConstantBuffer.Usage = D3D11_USAGE_DYNAMIC;
    cameraDataConstantBuffer.BindFlags = D3D11_BIND_CONSTANT_BUFFER;
    cameraDataConstantBuffer.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
    DX::ThrowIfFailed(device->CreateBuffer(&cameraDataConstantBuffer, nullptr, &m_cameraConstantBuffer));
    D3D11_BUFFER_DESC signalConstantBuffer = {0};
    signalConstantBuffer.ByteWidth = sizeof(float) * 4;
    signalConstantBuffer.Usage = D3D11_USAGE_DYNAMIC;
    signalConstantBuffer.BindFlags = D3D11_BIND_CONSTANT_BUFFER;
    signalConstantBuffer.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
    DX::ThrowIfFailed(device->CreateBuffer(&signalConstantBuffer, nullptr, &m_signalConstantBuffer));
    D3D11_BUFFER_DESC hiddenConstantBuffer = {0};
    hiddenConstantBuffer.ByteWidth = sizeof(float) * 4;
    hiddenConstantBuffer.Usage = D3D11_USAGE_DYNAMIC;
    hiddenConstantBuffer.BindFlags = D3D11_BIND_CONSTANT_BUFFER;
    hiddenConstantBuffer.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
    DX::ThrowIfFailed(device->CreateBuffer(&hiddenConstantBuffer, nullptr, &m_linkingConstantBuffer));
    ID3D11Buffer* constantBuffers[] =
    {
        m_cameraConstantBuffer.Get(),
        m_signalConstantBuffer.Get(),
        m_linkingConstantBuffer.Get()
    };
    context->VSSetConstantBuffers(0, ARRAYSIZE(constantBuffers), constantBuffers);
    context->PSSetConstantBuffers(0, ARRAYSIZE(constantBuffers), constantBuffers);

    // Load a default BC1 texture.
    DX::ReadDataAsync(L"texture.bin").then([this](vector<byte> fileData)
    {
        auto context = m_deviceResources->GetD3DDeviceContext();
        CD3D11_TEXTURE2D_DESC textureDesc(DXGI_FORMAT_B8G8R8A8_UNORM, 256, 256, 1, 1);
        ComPtr<ID3D11Texture2D> texture;
        D3D11_SUBRESOURCE_DATA initialData = {fileData.data(), 1024, 0};
        DX::ThrowIfFailed(m_deviceResources->GetD3DDevice()->CreateTexture2D(&textureDesc, &initialData, &texture));
        CD3D11_SHADER_RESOURCE_VIEW_DESC textureViewDesc(texture.Get(), D3D11_SRV_DIMENSION_TEXTURE2D);
        ComPtr<ID3D11ShaderResourceView> textureView;
        DX::ThrowIfFailed(m_deviceResources->GetD3DDevice()->CreateShaderResourceView(texture.Get(), &textureViewDesc, &textureView));
        context->PSSetShaderResources(0, 1, textureView.GetAddressOf());
    });

    Recompile();
}

// Saves the current state of the app for suspend and terminate events.
void DirectXPage::SaveInternalState(IPropertySet^ state)
{
    m_deviceResources->Trim();
}

// Loads the current state of the app for resume events.
void DirectXPage::LoadInternalState(IPropertySet^ state)
{
}

// Called every time XAML decides to render a frame.
void DirectXPage::OnRendering(Object^ sender, Object^ args)
{
    auto context = m_deviceResources->GetD3DDeviceContext();
    if (!m_compiledOnce)
    {
        return;
    }
    m_timer.Tick([](){});
    if (m_windowVisible)
    {
        XMFLOAT4 clearColor;
        XMStoreFloat4(&clearColor, DirectX::Colors::MidnightBlue);
        if (FogToggleButton->IsChecked->Value)
        {
            clearColor = XMFLOAT4(0.4f, 0.9f, 0.5f, 1.0f);
        }
        if (GrayscaleToggleButton->IsChecked->Value)
        {
            float luminance = 0.2126f * clearColor.x + 0.7152f * clearColor.y + 0.0722f * clearColor.z;
            clearColor.x = clearColor.y = clearColor.z = luminance;
        }
        // Update time-variant constant buffers.
        double timeTotal = m_timer.GetTotalSeconds();
        double timeFactor = timeTotal / 8.0f;
        D3D11_MAPPED_SUBRESOURCE mappedResource = {0};
        DX::ThrowIfFailed(context->Map(m_signalConstantBuffer.Get(), 0, D3D11_MAP_WRITE_DISCARD, 0, &mappedResource));
        static_cast<float*>(mappedResource.pData)[0] = static_cast<float>(sin(2.0 * M_PI * timeFactor)); // Sine wave with period 1 second.
        static_cast<float*>(mappedResource.pData)[1] = static_cast<float*>(mappedResource.pData)[0] > 0.0f ? 1.0f : -1.0f; // Square wave based off of the sine wave.
        static_cast<float*>(mappedResource.pData)[3] = static_cast<float>(2.0 * (timeFactor - floor(timeFactor)) - 1.0); // Sawtooth wave.
        static_cast<float*>(mappedResource.pData)[2] = 2.0f * abs(static_cast<float*>(mappedResource.pData)[3]) - 1.0f; // Triangle wave based off of the sawtooth wave.
        context->Unmap(m_signalConstantBuffer.Get(), 0);
        DX::ThrowIfFailed(context->Map(m_linkingConstantBuffer.Get(), 0, D3D11_MAP_WRITE_DISCARD, 0, &mappedResource));
        XMFLOAT3 lightPos = XMFLOAT3(sinf(static_cast<float>(timeTotal)), 1.0f + 0.5f * cosf(static_cast<float>(timeTotal) / XM_PI), cosf(static_cast<float>(timeTotal)));
        XMStoreFloat3(static_cast<XMFLOAT3*>(mappedResource.pData), XMVector3Normalize(XMLoadFloat3(&lightPos)));
        context->Unmap(m_linkingConstantBuffer.Get(), 0);

        context->ClearRenderTargetView(m_deviceResources->GetBackBufferRenderTargetView(), &clearColor.x);
        context->ClearDepthStencilView(m_deviceResources->GetDepthStencilView(), D3D11_CLEAR_DEPTH, 1.0f, 0);
        ID3D11RenderTargetView* targetViews[] = {m_deviceResources->GetBackBufferRenderTargetView()};
        context->OMSetRenderTargets(1, targetViews, m_deviceResources->GetDepthStencilView());
        context->DrawIndexed(36, 0, 0);
        m_deviceResources->Present();
    }
}

// Window event handlers.

void DirectXPage::OnVisibilityChanged(CoreWindow^ sender, VisibilityChangedEventArgs^ args)
{
    m_windowVisible = args->Visible;
}

void DirectXPage::OnCompositionScaleChanged(SwapChainPanel^ sender, Platform::Object^ args)
{
    m_deviceResources->SetCompositionScale(sender->CompositionScaleX, sender->CompositionScaleY);
}

void DirectXPage::SourceCodeTextChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::TextChangedEventArgs^ e)
{
    Recompile();
}

void DirectXPage::ToggleButtonClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Recompile();
}

void DirectXPage::Recompile()
{
    auto device = m_deviceResources->GetD3DDevice();
    auto context = m_deviceResources->GetD3DDeviceContext();

    // Snapshot the values of all controls.
    String^ sourceWide = m_headerCode + "\n" + m_hiddenCode + "\n#line 0\n" + SourceCodeTextBox->Text + "\n";
    bool enableLighting = LightingToggleButton->IsChecked->Value;
    bool enableDepthFog = FogToggleButton->IsChecked->Value;
    bool enableGrayscale = GrayscaleToggleButton->IsChecked->Value;
    bool enableBenchmark = BenchmarkToggleButton->IsChecked->Value;

    OutputTextBox->Text = "";
    OutputTextBox->FontSize = 12.0;

    // Convert the library source string to ASCII and compile it.
    string source;
    try
    {
        source = ConvertString(sourceWide);
    }
    catch (string& error)
    {
        OutputTextBox->Text = ConvertString(error);
        return;
    }

    ComPtr<ID3DBlob> codeBlob;
    ComPtr<ID3DBlob> errorBlob;
    HRESULT hr = D3DCompile(
        source.c_str(),
        source.size(),
        "ShaderModule",
        NULL,
        NULL,
        NULL,
        ("lib" + m_shaderModelSuffix).c_str(),
        D3DCOMPILE_OPTIMIZATION_LEVEL3,
        0,
        &codeBlob,
        &errorBlob
        );

    // Note that even with successful compilation, this sample's library code will produce
    // warning messages as it targets level 9.1 but includes sampling instructions. The compiler
    // warns that these instructions are not available in the vertex shader stage for 9.1, indicating
    // that it is not valid to use those functions for function call nodes in a vertex shader graph.
    if (FAILED(hr))
    {
        OutputTextBox->Text = "Compilation failed.\n";
        if (errorBlob != nullptr)
        {
            string errorString(static_cast<char*>(errorBlob->GetBufferPointer()));
            errorString = CleanOutputMessage(errorString);
            OutputTextBox->Text += ConvertString(errorString);
        }
        return;
    }

    DX::ThrowIfFailed(codeBlob != nullptr ? S_OK : E_UNEXPECTED);

    // Load the compiled library code into a module object.
    ComPtr<ID3D11Module> shaderLibrary;
    DX::ThrowIfFailed(D3DLoadModule(codeBlob->GetBufferPointer(), codeBlob->GetBufferSize(), &shaderLibrary));

    // Create an instance of the library and define resource bindings for it.
    // In this sample we use the same slots as the source library however this is not required.
    ComPtr<ID3D11ModuleInstance> shaderLibraryInstance;
    DX::ThrowIfFailed(shaderLibrary->CreateInstance("", &shaderLibraryInstance));
    // HRESULTs for Bind methods are intentionally ignored as compiler optimizations may eliminate the source
    // bindings. In these cases, the Bind operation will fail, but the final shader will function normally.
    shaderLibraryInstance->BindResource(0, 0, 1);
    shaderLibraryInstance->BindSampler(0, 0, 1);
    shaderLibraryInstance->BindConstantBuffer(0, 0, 0);
    shaderLibraryInstance->BindConstantBuffer(1, 1, 0);
    shaderLibraryInstance->BindConstantBuffer(2, 2, 0);

    ComPtr<ID3DBlob> vertexShaderBlob;
    ComPtr<ID3DBlob> pixelShaderBlob;

    LARGE_INTEGER linkStartTime;
    QueryPerformanceCounter(&linkStartTime);
    for (int iteration = 0; iteration < SampleSettings::Benchmark::Iterations; iteration++)
    {
        // Construct a linking graph to represent the vertex shader.
        try
        {
            ComPtr<ID3D11FunctionLinkingGraph> vertexShaderGraph;
            DX::ThrowIfFailed(D3DCreateFunctionLinkingGraph(0, &vertexShaderGraph));

            // Define the main input node which will be fed by the Input Assembler pipeline stage.
            static const D3D11_PARAMETER_DESC vertexShaderInputParameters[] =
            {
                {"inputPos",  "POSITION0", D3D_SVT_FLOAT, D3D_SVC_VECTOR, 1, 3, D3D_INTERPOLATION_LINEAR, D3D_PF_IN, 0, 0, 0, 0},
                {"inputTex",  "TEXCOORD0", D3D_SVT_FLOAT, D3D_SVC_VECTOR, 1, 2, D3D_INTERPOLATION_LINEAR, D3D_PF_IN, 0, 0, 0, 0},
                {"inputNorm", "NORMAL0",   D3D_SVT_FLOAT, D3D_SVC_VECTOR, 1, 3, D3D_INTERPOLATION_LINEAR, D3D_PF_IN, 0, 0, 0, 0}
            };
            ComPtr<ID3D11LinkingNode> vertexShaderInputNode;
            LinkingThrowIfFailed(vertexShaderGraph->SetInputSignature(vertexShaderInputParameters, ARRAYSIZE(vertexShaderInputParameters), &vertexShaderInputNode), vertexShaderGraph.Get());

            // VertexFunction in the sample HLSL code takes float4 arguments, so use the Float3ToFloat4SetW1 helper function to homogenize the coordinates.
            // As both the position and normal values must be converted, we need two distinct call nodes.
            ComPtr<ID3D11LinkingNode> homogenizeCallNodeForPos;
            LinkingThrowIfFailed(vertexShaderGraph->CallFunction("", shaderLibrary.Get(), "Float3ToFloat4SetW1", &homogenizeCallNodeForPos), vertexShaderGraph.Get());
            ComPtr<ID3D11LinkingNode> homogenizeCallNodeForNorm;
            LinkingThrowIfFailed(vertexShaderGraph->CallFunction("", shaderLibrary.Get(), "Float3ToFloat4SetW1", &homogenizeCallNodeForNorm), vertexShaderGraph.Get());

            // Define the graph edges from the input node to the helper function nodes.
            LinkingThrowIfFailed(vertexShaderGraph->PassValue(vertexShaderInputNode.Get(), 0, homogenizeCallNodeForPos.Get(), 0), vertexShaderGraph.Get());
            LinkingThrowIfFailed(vertexShaderGraph->PassValue(vertexShaderInputNode.Get(), 2, homogenizeCallNodeForNorm.Get(), 0), vertexShaderGraph.Get());

            // Create a node for the main VertexFunction call using the output of the helper functions.
            ComPtr<ID3D11LinkingNode> vertexFunctionCallNode;
            LinkingThrowIfFailed(vertexShaderGraph->CallFunction("", shaderLibrary.Get(), "VertexFunction", &vertexFunctionCallNode), vertexShaderGraph.Get());

            // Define the graph edges from the input node and helper function nodes.
            LinkingThrowIfFailed(vertexShaderGraph->PassValue(homogenizeCallNodeForPos.Get(), D3D_RETURN_PARAMETER_INDEX, vertexFunctionCallNode.Get(), 0), vertexShaderGraph.Get());
            LinkingThrowIfFailed(vertexShaderGraph->PassValue(vertexShaderInputNode.Get(), 1, vertexFunctionCallNode.Get(), 1), vertexShaderGraph.Get());
            LinkingThrowIfFailed(vertexShaderGraph->PassValue(homogenizeCallNodeForNorm.Get(), D3D_RETURN_PARAMETER_INDEX, vertexFunctionCallNode.Get(), 2), vertexShaderGraph.Get());

            // Define the main output node which will feed the Pixel Shader pipeline stage.
            static const D3D11_PARAMETER_DESC vertexShaderOutputParameters[] =
            {
                {"outputTex",  "TEXCOORD0",   D3D_SVT_FLOAT, D3D_SVC_VECTOR, 1, 2, D3D_INTERPOLATION_UNDEFINED, D3D_PF_OUT, 0, 0, 0, 0},
                {"outputNorm", "NORMAL0",     D3D_SVT_FLOAT, D3D_SVC_VECTOR, 1, 3, D3D_INTERPOLATION_UNDEFINED, D3D_PF_OUT, 0, 0, 0, 0},
                {"outputPos",  "SV_POSITION", D3D_SVT_FLOAT, D3D_SVC_VECTOR, 1, 4, D3D_INTERPOLATION_UNDEFINED, D3D_PF_OUT, 0, 0, 0, 0}
            };
            ComPtr<ID3D11LinkingNode> vertexShaderOutputNode;
            LinkingThrowIfFailed(vertexShaderGraph->SetOutputSignature(vertexShaderOutputParameters, ARRAYSIZE(vertexShaderOutputParameters), &vertexShaderOutputNode), vertexShaderGraph.Get());
            LinkingThrowIfFailed(vertexShaderGraph->PassValue(vertexFunctionCallNode.Get(), 0, vertexShaderOutputNode.Get(), 2), vertexShaderGraph.Get());
            LinkingThrowIfFailed(vertexShaderGraph->PassValue(vertexFunctionCallNode.Get(), 1, vertexShaderOutputNode.Get(), 0), vertexShaderGraph.Get());
            LinkingThrowIfFailed(vertexShaderGraph->PassValue(vertexFunctionCallNode.Get(), 2, vertexShaderOutputNode.Get(), 1), vertexShaderGraph.Get());

            // Finalize the vertex shader graph.
            ComPtr<ID3D11ModuleInstance> vertexShaderGraphInstance;
            LinkingThrowIfFailed(vertexShaderGraph->CreateModuleInstance(&vertexShaderGraphInstance, nullptr), vertexShaderGraph.Get());

            // Create a linker and hook up the module instance.
            ComPtr<ID3D11Linker> linker;
            DX::ThrowIfFailed(D3DCreateLinker(&linker));
            DX::ThrowIfFailed(linker->UseLibrary(shaderLibraryInstance.Get()));

            // Link the vertex shader.
            ComPtr<ID3DBlob> errorBlob;
            if (FAILED(linker->Link(vertexShaderGraphInstance.Get(), "main", ("vs" + m_shaderModelSuffix).c_str(), 0, &vertexShaderBlob, &errorBlob)))
            {
                throw errorBlob;
            }

            DX::ThrowIfFailed(vertexShaderBlob != nullptr ? S_OK : E_UNEXPECTED);

            if (!enableBenchmark)
            {
                ComPtr<ID3DBlob> disassembly;
                hr = D3DDisassemble(vertexShaderBlob->GetBufferPointer(), vertexShaderBlob->GetBufferSize(), 0, nullptr, &disassembly);
                if (disassembly != nullptr)
                {
                    OutputTextBox->Text += "Vertex Shader Disassembly:\n";
                    OutputTextBox->Text += ConvertString(static_cast<char*>(disassembly->GetBufferPointer()));
                }
            }
        }
        catch (ComPtr<ID3DBlob>& error)
        {
            OutputTextBox->Text = "Vertex Shader Linking Failed.\n";
            if (error != nullptr)
            {
                OutputTextBox->Text += ConvertString(static_cast<char*>(error->GetBufferPointer())) + "\n";
            }
            return;
        }

        if (!enableBenchmark)
        {
            OutputTextBox->Text += "\n\n";
        }

        // Construct a linking graph to represent the pixel shader.
        try
        {
            ComPtr<ID3D11FunctionLinkingGraph> pixelShaderGraph;
            DX::ThrowIfFailed(D3DCreateFunctionLinkingGraph(0, &pixelShaderGraph));

            // Define the main input node which will be fed by the Input Assembler pipeline stage.
            static const D3D11_PARAMETER_DESC pixelShaderInputParameters[] =
            {
                {"inputTex",  "TEXCOORD0",   D3D_SVT_FLOAT, D3D_SVC_VECTOR, 1, 2, D3D_INTERPOLATION_UNDEFINED, D3D_PF_IN, 0, 0, 0, 0},
                {"inputNorm", "NORMAL0",     D3D_SVT_FLOAT, D3D_SVC_VECTOR, 1, 3, D3D_INTERPOLATION_UNDEFINED, D3D_PF_IN, 0, 0, 0, 0},
                {"inputPos",  "SV_POSITION", D3D_SVT_FLOAT, D3D_SVC_VECTOR, 1, 4, D3D_INTERPOLATION_UNDEFINED, D3D_PF_IN, 0, 0, 0, 0}
            };
            ComPtr<ID3D11LinkingNode> pixelShaderInputNode;
            LinkingThrowIfFailed(pixelShaderGraph->SetInputSignature(pixelShaderInputParameters, ARRAYSIZE(pixelShaderInputParameters), &pixelShaderInputNode), pixelShaderGraph.Get());

            // Create a node for the main ColorFunction call and connect it to the pixel shader inputs.
            ComPtr<ID3D11LinkingNode> colorValueNode;
            LinkingThrowIfFailed(pixelShaderGraph->CallFunction("", shaderLibrary.Get(), "ColorFunction", &colorValueNode), pixelShaderGraph.Get());

            // Define the graph edges from the input node.
            LinkingThrowIfFailed(pixelShaderGraph->PassValue(pixelShaderInputNode.Get(), 0, colorValueNode.Get(), 0), pixelShaderGraph.Get());
            LinkingThrowIfFailed(pixelShaderGraph->PassValue(pixelShaderInputNode.Get(), 1, colorValueNode.Get(), 1), pixelShaderGraph.Get());

            // Optionally insert additional function calls based on the toggle switches.
            // After each call, we will swap nodes to ensure that the return value on the colorValueNode
            // variable always represents the output color for the shader call graph.
            if (enableLighting)
            {
                ComPtr<ID3D11LinkingNode> tempNode;
                LinkingThrowIfFailed(pixelShaderGraph->CallFunction("", shaderLibrary.Get(), "AddLighting", &tempNode), pixelShaderGraph.Get());
                LinkingThrowIfFailed(pixelShaderGraph->PassValue(colorValueNode.Get(), D3D_RETURN_PARAMETER_INDEX, tempNode.Get(), 0), pixelShaderGraph.Get());
                LinkingThrowIfFailed(pixelShaderGraph->PassValue(pixelShaderInputNode.Get(), 1, tempNode.Get(), 1), pixelShaderGraph.Get());
                colorValueNode.Swap(tempNode);
            }

            if (enableDepthFog)
            {
                ComPtr<ID3D11LinkingNode> tempNode;
                LinkingThrowIfFailed(pixelShaderGraph->CallFunction("", shaderLibrary.Get(), "AddDepthFog", &tempNode), pixelShaderGraph.Get());
                LinkingThrowIfFailed(pixelShaderGraph->PassValue(colorValueNode.Get(), D3D_RETURN_PARAMETER_INDEX, tempNode.Get(), 0), pixelShaderGraph.Get());
                LinkingThrowIfFailed(pixelShaderGraph->PassValueWithSwizzle(pixelShaderInputNode.Get(), 2, "z", tempNode.Get(), 1, "x"), pixelShaderGraph.Get());
                colorValueNode.Swap(tempNode);
            }

            if (enableGrayscale)
            {
                ComPtr<ID3D11LinkingNode> tempNode;
                LinkingThrowIfFailed(pixelShaderGraph->CallFunction("", shaderLibrary.Get(), "AddGrayscale", &tempNode), pixelShaderGraph.Get());
                LinkingThrowIfFailed(pixelShaderGraph->PassValue(colorValueNode.Get(), D3D_RETURN_PARAMETER_INDEX, tempNode.Get(), 0), pixelShaderGraph.Get());
                colorValueNode.Swap(tempNode);
            }

            // ColorFunction in the sample HLSL code returns a float3 argument, so use the Float3ToFloat4SetW1 helper function to set alpha (w) to 1.
            ComPtr<ID3D11LinkingNode> fillAlphaCallNode;
            LinkingThrowIfFailed(pixelShaderGraph->CallFunction("", shaderLibrary.Get(), "Float3ToFloat4SetW1", &fillAlphaCallNode), pixelShaderGraph.Get());

            // Define the graph edges from the color node to the helper function node.
            LinkingThrowIfFailed(pixelShaderGraph->PassValue(colorValueNode.Get(), D3D_RETURN_PARAMETER_INDEX, fillAlphaCallNode.Get(), 0), pixelShaderGraph.Get());

            // Define the main output node which will feed the Output Merger pipeline stage.
            D3D11_PARAMETER_DESC pixelShaderOutputParameters[] =
            {
                {"outputColor", "SV_TARGET", D3D_SVT_FLOAT, D3D_SVC_VECTOR, 1, 4, D3D_INTERPOLATION_UNDEFINED, D3D_PF_OUT, 0, 0, 0, 0}
            };
            ComPtr<ID3D11LinkingNode> pixelShaderOutputNode;
            LinkingThrowIfFailed(pixelShaderGraph->SetOutputSignature(pixelShaderOutputParameters, ARRAYSIZE(pixelShaderOutputParameters), &pixelShaderOutputNode), pixelShaderGraph.Get());
            LinkingThrowIfFailed(pixelShaderGraph->PassValue(fillAlphaCallNode.Get(), D3D_RETURN_PARAMETER_INDEX, pixelShaderOutputNode.Get(), 0), pixelShaderGraph.Get());

            // Finalize the pixel shader graph.
            ComPtr<ID3D11ModuleInstance> pixelShaderGraphInstance;
            LinkingThrowIfFailed(pixelShaderGraph->CreateModuleInstance(&pixelShaderGraphInstance, nullptr), pixelShaderGraph.Get());

            // Create a linker and hook up the module instance.
            ComPtr<ID3D11Linker> linker;
            DX::ThrowIfFailed(D3DCreateLinker(&linker));
            DX::ThrowIfFailed(linker->UseLibrary(shaderLibraryInstance.Get()));

            // Link the pixel shader.
            ComPtr<ID3DBlob> errorBlob;
            if (FAILED(linker->Link(pixelShaderGraphInstance.Get(), "main", ("ps" + m_shaderModelSuffix).c_str(), 0, &pixelShaderBlob, &errorBlob)))
            {
                throw errorBlob;
            }

            DX::ThrowIfFailed(pixelShaderBlob != nullptr ? S_OK : E_UNEXPECTED);

            if (!enableBenchmark)
            {
                ComPtr<ID3DBlob> disassembly;
                hr = D3DDisassemble(pixelShaderBlob->GetBufferPointer(), pixelShaderBlob->GetBufferSize(), 0, nullptr, &disassembly);
                if (disassembly != nullptr)
                {
                    OutputTextBox->Text += "Pixel Shader Disassembly:\n";
                    OutputTextBox->Text += ConvertString(static_cast<char*>(disassembly->GetBufferPointer()));
                }
            }
        }
        catch (ComPtr<ID3DBlob>& error)
        {
            OutputTextBox->Text = "Pixel Shader Linking Failed.\n";
            if (error != nullptr)
            {
                OutputTextBox->Text += ConvertString(static_cast<char*>(error->GetBufferPointer())) + "\n";
            }
            return;
        }
        if (!enableBenchmark)
        {
            break;
        }
    }
    LARGE_INTEGER linkStopTime;
    QueryPerformanceCounter(&linkStopTime);

    if (enableBenchmark)
    {
        // In order to create the same shaders without using linking, string concatenation and
        // full compilation must be used instead. Code to accomplish this is provided below.
        LARGE_INTEGER compileStartTime;
        QueryPerformanceCounter(&compileStartTime);
        for (int iteration = 0; iteration < SampleSettings::Benchmark::Iterations; iteration++)
        {
            string extendedSource = source;

            // Add code representing the equivalent shader graph as above.
            extendedSource +=
                "void vsmain(\n"
                "    in float3 inputPos : POSITION0, in float2 inputTex : TEXCOORD0, in float3 inputNorm : NORMAL0,\n"
                "    out float2 outputTex : TEXCOORD0, out float3 outputNorm : NORMAL0, out float4 outputPos : SV_POSITION)\n"
                "    {\n"
                "        float4 pos = float4(inputPos, 1.0f);\n"
                "        float4 norm = float4(inputNorm, 1.0f);\n"
                "        VertexFunction(pos, inputTex, norm);\n"
                "        outputTex = inputTex;\n"
                "        outputNorm = norm.xyz;\n"
                "        outputPos = pos;\n"
                "    }\n";

            ComPtr<ID3DBlob> errorBlob;
            HRESULT hr = D3DCompile(
                extendedSource.c_str(),
                extendedSource.size(),
                "ShaderModule",
                NULL,
                NULL,
                "vsmain",
                ("vs" + m_shaderModelSuffix).c_str(),
                D3DCOMPILE_OPTIMIZATION_LEVEL3,
                0,
                &vertexShaderBlob,
                &errorBlob
                );

            if (FAILED(hr))
            {
                OutputTextBox->Text = "Vertex Shader Compilation Failed.\n";
                if (errorBlob != nullptr)
                {
                    string errorString(static_cast<char*>(errorBlob->GetBufferPointer()));
                    errorString = CleanOutputMessage(errorString);
                    OutputTextBox->Text += ConvertString(errorString);
                }
                return;
            }

            extendedSource +=
                "void psmain(\n"
                "    in float2 inputTex : TEXCOORD0, in float3 inputNorm : NORMAL0, in float4 inputPos : SV_POSITION,\n"
                "    out float4 outputColor : SV_TARGET)\n"
                "    {\n"
                "        float3 color = ColorFunction(inputTex, inputNorm);\n";

            if (enableLighting)
            {
                extendedSource +=
                    "        color = AddLighting(color, inputNorm);\n";
            }

            if (enableDepthFog)
            {
                extendedSource +=
                    "        color = AddDepthFog(color, inputPos.z);\n";
            }

            if (enableGrayscale)
            {
                extendedSource +=
                    "        color = AddGrayscale(color);\n";
            }

            extendedSource +=
                "        outputColor = float4(color, 1.0f);\n"
                "    }\n";

            hr = D3DCompile(
                extendedSource.c_str(),
                extendedSource.size(),
                "ShaderModule",
                NULL,
                NULL,
                "psmain",
                ("ps" + m_shaderModelSuffix).c_str(),
                D3DCOMPILE_OPTIMIZATION_LEVEL3,
                0,
                &pixelShaderBlob,
                &errorBlob
                );

            if (FAILED(hr))
            {
                OutputTextBox->Text = "Pixel Shader Compilation Failed.\n";
                if (errorBlob != nullptr)
                {
                    string errorString(static_cast<char*>(errorBlob->GetBufferPointer()));
                    errorString = CleanOutputMessage(errorString);
                    OutputTextBox->Text += ConvertString(errorString);
                }
                return;
            }
        }
        LARGE_INTEGER compileStopTime;
        QueryPerformanceCounter(&compileStopTime);

        LARGE_INTEGER frequency;
        QueryPerformanceFrequency(&frequency);

        double linkTimePerIteration = static_cast<double>(linkStopTime.QuadPart - linkStartTime.QuadPart) / static_cast<double>(frequency.QuadPart * SampleSettings::Benchmark::Iterations);
        double compileTimePerIteration = static_cast<double>(compileStopTime.QuadPart - compileStartTime.QuadPart) / static_cast<double>(frequency.QuadPart * SampleSettings::Benchmark::Iterations);

        OutputTextBox->FontSize = 20.0;
        OutputTextBox->Text = "Benchmark Mode Results:\n";
        OutputTextBox->Text += "Compile time: " + static_cast<int>(compileTimePerIteration * 1e6).ToString() + " µs\n";
        OutputTextBox->Text += "Link time: " + static_cast<int>(linkTimePerIteration * 1e6).ToString() + " µs (" + static_cast<int>(100.0f * compileTimePerIteration / linkTimePerIteration).ToString() + "% faster)\n";
    }

    // Scroll the output to the end of the text as the most interesting disassembly is at the bottom.
    OutputTextBoxScrollViewer->ChangeView(0.0, 1e9, 1.0f, true);

    // Create the new shaders, replacing the previous ones bound to the pipeline.
    // The objects themselves may be released as the runtime will persist them as long as they are bound.
    ComPtr<ID3D11VertexShader> vertexShader;
    DX::ThrowIfFailed(
        device->CreateVertexShader(
            vertexShaderBlob->GetBufferPointer(),
            vertexShaderBlob->GetBufferSize(),
            nullptr,
            &vertexShader
            )
        );
    context->VSSetShader(vertexShader.Get(), nullptr, 0);
    D3D11_INPUT_ELEMENT_DESC inputLayoutDesc[] =
    {
        { "POSITION", 0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 0,  D3D11_INPUT_PER_VERTEX_DATA, 0 },
        { "TEXCOORD", 0, DXGI_FORMAT_R32G32_FLOAT,    0, 12, D3D11_INPUT_PER_VERTEX_DATA, 0 },
        { "NORMAL",   0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 20, D3D11_INPUT_PER_VERTEX_DATA, 0 }
    };
    ComPtr<ID3D11InputLayout> inputLayout;
    DX::ThrowIfFailed(device->CreateInputLayout(inputLayoutDesc, ARRAYSIZE(inputLayoutDesc), vertexShaderBlob->GetBufferPointer(), vertexShaderBlob->GetBufferSize(), &inputLayout));
    context->IASetInputLayout(inputLayout.Get());

    ComPtr<ID3D11PixelShader> pixelShader;
    DX::ThrowIfFailed(
        device->CreatePixelShader(
            pixelShaderBlob->GetBufferPointer(),
            pixelShaderBlob->GetBufferSize(),
            nullptr,
            &pixelShader
            )
        );
    context->PSSetShader(pixelShader.Get(), nullptr, 0);

    m_compiledOnce = true;
}

string DirectXPage::ConvertString(String^ platformString)
{
    string stdString;
    stdString.reserve(platformString->Length());
    int index = 0;
    for (auto c : platformString)
    {
        if (c >= 128)
        {
            ostringstream message;
            message << "Source string contains non-ASCII character at offset " << index << " (";
            message << "0x" << hex << static_cast<unsigned int>(c) << ").";
            throw message.str();
        }
        stdString += static_cast<char>(c);
        index++;
    }
    return stdString;
}

String^ DirectXPage::ConvertString(const string& stdString)
{
    String^ platformString;
    for (auto c : stdString)
    {
        platformString += static_cast<wchar_t>(c);
    }
    return platformString;
}

void DirectXPage::PreviewPanelSizeChanged(Object^ sender, SizeChangedEventArgs^ e)
{
    CreatePanelSizeDependentResources();
}

void DirectXPage::CreatePanelSizeDependentResources()
{
    auto context = m_deviceResources->GetD3DDeviceContext();
    if (m_swapChainPanelDeviceResourcesInitialized)
    {
        m_deviceResources->SetLogicalSize(Size(static_cast<float>(PreviewPanel->ActualWidth), static_cast<float>(PreviewPanel->ActualHeight)));
    }
    else
    {
        m_deviceResources->SetSwapChainPanel(PreviewPanel);
        m_swapChainPanelDeviceResourcesInitialized = true;
        if (m_deviceResources->GetDeviceFeatureLevel() < D3D_FEATURE_LEVEL_10_0)
        {
            // Disable the depth fog toggle button for Feature Level 9 devices, as the implementation
            // in this sample requires the ability to read from the SV_POSITION variable, functionality
            // only available in Shader Model 4.0 and higher, which requires Feature Level 10 or higher.
            FogToggleButton->IsEnabled = false;
        }
        if (m_deviceResources->GetDeviceFeatureLevel() >= D3D_FEATURE_LEVEL_11_0)
        {
            m_shaderModelSuffix = "_5_0";
        }
        else if (m_deviceResources->GetDeviceFeatureLevel() >= D3D_FEATURE_LEVEL_10_1)
        {
            m_shaderModelSuffix = "_4_1";
        }
        else if (m_deviceResources->GetDeviceFeatureLevel() >= D3D_FEATURE_LEVEL_10_0)
        {
            m_shaderModelSuffix = "_4_0";
        }
        else if (m_deviceResources->GetDeviceFeatureLevel() >= D3D_FEATURE_LEVEL_9_3)
        {
            m_shaderModelSuffix = "_4_0_level_9_3";
        }
        else
        {
            m_shaderModelSuffix = "_4_0_level_9_1";
        }
        Recompile();
    }

    D3D11_MAPPED_SUBRESOURCE mappedResource = {0};
    DX::ThrowIfFailed(context->Map(m_cameraConstantBuffer.Get(), 0, D3D11_MAP_WRITE_DISCARD, 0, &mappedResource));
    XMStoreFloat4x4(&static_cast<XMFLOAT4X4*>(mappedResource.pData)[0], XMMatrixTranspose( // Model matrix.
        XMMatrixRotationY(45.0f * XM_PI / 180.0f)
        ));
    XMFLOAT3 eye(0.0f, 1.2f, 3.0f);
    XMFLOAT3 at(0.0f, 0.0f, 0.0f);
    XMFLOAT3 up(0.0f, 1.0f, 0.0f);
    XMStoreFloat4x4(
        &static_cast<XMFLOAT4X4*>(mappedResource.pData)[1],
        XMMatrixTranspose( // View matrix.
            XMMatrixLookAtRH(
                XMLoadFloat3(&eye),
                XMLoadFloat3(&at),
                XMLoadFloat3(&up)
                )
            )
        );
    XMFLOAT4X4 orientationTransform = m_deviceResources->GetOrientationTransform3D();
    XMStoreFloat4x4(
        &static_cast<XMFLOAT4X4*>(mappedResource.pData)[2],
        XMMatrixTranspose( // Projection matrix.
            XMMatrixMultiply(
                XMMatrixPerspectiveFovRH(
                    70.0f * XM_PI / 180.0f,
                    static_cast<float>(PreviewPanel->ActualWidth) / static_cast<float>(PreviewPanel->ActualHeight),
                    0.1f,
                    10.0f
                    ),
                XMLoadFloat4x4(&orientationTransform)
                )
            )
        );
    XMFLOAT4X4 *data = static_cast<XMFLOAT4X4*>(mappedResource.pData);
    context->Unmap(m_cameraConstantBuffer.Get(), 0);
}

string DirectXPage::CleanOutputMessage(const string& message)
{
    istringstream messageStream(message);
    ostringstream processedStream;
    while (!messageStream.eof())
    {
        string line;
        getline(messageStream, line);

        // Strip source path.
        auto pos = line.find("ShaderModule");
        if (pos != line.npos)
        {
            line = line.substr(pos + 12);
        }

        processedStream << line << endl;
    }
    return processedStream.str();
}

void DirectXPage::LinkingThrowIfFailed(HRESULT hr, ID3D11FunctionLinkingGraph* graph)
{
    if (FAILED(hr))
    {
        ComPtr<ID3DBlob> error;
        DX::ThrowIfFailed(graph->GetLastError(&error));
        throw error;
    }
}

void PageNotifier::OnDeviceLost()
{
}

void PageNotifier::OnDeviceRestored()
{
    m_page->CreateDeviceDependentResources();
    m_page->CreatePanelSizeDependentResources();
}
