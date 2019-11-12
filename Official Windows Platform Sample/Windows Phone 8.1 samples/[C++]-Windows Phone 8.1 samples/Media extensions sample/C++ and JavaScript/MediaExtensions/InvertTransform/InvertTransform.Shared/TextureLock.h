// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.

#pragma once

using namespace Microsoft::WRL;

class TextureLock sealed
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