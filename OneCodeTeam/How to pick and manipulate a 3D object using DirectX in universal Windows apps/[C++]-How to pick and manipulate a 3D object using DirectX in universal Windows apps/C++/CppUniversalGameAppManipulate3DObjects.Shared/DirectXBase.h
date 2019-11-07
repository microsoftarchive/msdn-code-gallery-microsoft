#pragma once

#include "DirectXHelper.h"

// Helper class that initializes DirectX APIs for both 2D and 3D rendering.
// Some of the code in this class may be omitted if only 2D or only 3D rendering is being used.
ref class DirectXBase abstract
{
internal:
	DirectXBase();

public:
	virtual void Initialize(Windows::UI::Core::CoreWindow^ window, Windows::UI::Xaml::Controls::SwapChainPanel^ panel, float dpi);
	virtual void HandleDeviceLost();
	virtual void CreateDeviceIndependentResources();
	virtual void CreateDeviceResources();
	virtual void SetDpi(float dpi);
	virtual void CreateWindowSizeDependentResources();
	virtual void UpdateForWindowSizeChange();
	virtual void Render(float* backgroundColor) = 0;
	virtual void Present();
	virtual float ConvertDipsToPixels(float dips);
	void ValidateDevice();


protected private:
	Platform::Agile<Windows::UI::Core::CoreWindow> m_window;
	Windows::UI::Xaml::Controls::SwapChainPanel^ m_panel;

	// DirectWrite & Windows Imaging Component Objects.
	Microsoft::WRL::ComPtr<IDWriteFactory1> m_dwriteFactory;
	Microsoft::WRL::ComPtr<IWICImagingFactory2> m_wicFactory;

	// DirectX Core Objects. Required for 2D and 3D.
	Microsoft::WRL::ComPtr<ID3D11Device1> m_d3dDevice;
	Microsoft::WRL::ComPtr<ID3D11DeviceContext1> m_d3dContext;
	Microsoft::WRL::ComPtr<IDXGISwapChain1> m_swapChain;
	Microsoft::WRL::ComPtr<ID3D11RenderTargetView> m_d3dRenderTargetView;

	// Direct2D Rendering Objects. Required for 2D.
	Microsoft::WRL::ComPtr<ID2D1Factory1> m_d2dFactory;
	Microsoft::WRL::ComPtr<ID2D1Device> m_d2dDevice;
	Microsoft::WRL::ComPtr<ID2D1DeviceContext> m_d2dContext;
	Microsoft::WRL::ComPtr<ID2D1Bitmap1> m_d2dTargetBitmap;

	// Direct3D Rendering Objects. Required for 3D.
	Microsoft::WRL::ComPtr<ID3D11DepthStencilView> m_d3dDepthStencilView;
	

	// Cached renderer properties.
	D3D_FEATURE_LEVEL m_d3dFeatureLevel;
	Windows::Foundation::Size m_d3dRenderTargetSize;
	Windows::Foundation::Rect m_windowBounds;
	float m_dpi;
	Windows::Graphics::Display::DisplayOrientations m_orientation;

	// Transforms used for display orientation.
	D2D1::Matrix3x2F m_orientationTransform2D;
	DirectX::XMFLOAT4X4 m_orientationTransform3D;
};
