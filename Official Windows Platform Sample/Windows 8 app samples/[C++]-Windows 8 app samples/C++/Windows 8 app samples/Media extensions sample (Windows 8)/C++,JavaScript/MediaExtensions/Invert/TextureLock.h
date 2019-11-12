#pragma once
#include <assert.h>
#include <D3D11.h>
#include <wrl\client.h>

using namespace Microsoft::WRL;

class TextureLock
{
public:
	TextureLock( ID3D11DeviceContext *pContext, ID3D11Texture2D *pTex ) : _pTex(pTex), _pContext(pContext), _bLocked(false) {}
		
	~TextureLock()
	{
		assert(_pContext);
		assert(_pTex);
		if (_pTex && _bLocked)
		{
			
			_pContext->Unmap(_pTex.Get(), _uiIndex);
			_bLocked = false;
		}
	}
	
	HRESULT Map(UINT uiIndex, D3D11_MAP mapType, UINT mapFlags)
	{
		HRESULT hr = S_OK;
		assert(_pTex);
		assert(_pContext);
		assert(!_bLocked);

		hr = _pContext->Map(_pTex.Get(), uiIndex, mapType, mapFlags, &map);
		_bLocked = SUCCEEDED(hr);
		_uiIndex = uiIndex;
		return hr;
	}
	
	D3D11_MAPPED_SUBRESOURCE map;
	
private:
	ComPtr<ID3D11Texture2D> _pTex;
	ComPtr<ID3D11DeviceContext> _pContext;
	UINT _uiIndex;
	bool _bLocked;
};